using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Weekly_ToDo_List
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Settings : Window
    {
        private string newFile = Properties.Settings.Default.DefaultFile;

        public Settings()
        {
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            InitializeComponent();
            FillClassBoxes();
            DirectoryTextBlock.Text = Properties.Settings.Default.DefaultFile;

            AutoSave.IsChecked = (Properties.Settings.Default.AutoSave == true) ? true : false;
            AutoLoad.IsChecked = (Properties.Settings.Default.AutoLoad == true) ? true : false;
            StoreDeleted.IsChecked = (Properties.Settings.Default.StoreDeleted == true) ? true : false;

            Closing += Settings_Closing;
            RefreshBox();
       }

        private void SaveChanges()
        {
            if (AddClass.Text.Length > 0)
            {
                string color = cp.SelectedColorText;
                color = (color == "" || color == null) ? "Colors.Transparent" : color;

                Classes newClass = new Classes(AddClass.Text, color);

                HandleData.AddToClasses(newClass);

                HandleData.SaveClasses(HandleData.GetClasses());
            }
            if (RemoveClass.SelectedIndex > -1)
            {
                if (EditColor.IsChecked.Value)
                {
                    string color = cp.SelectedColorText;
                    color = (color == "" || color == null) ? "Colors.Transparent" : color;
                    HandleData.EditClassColor((Classes)RemoveClass.SelectedItem, color);
                }
                else if (DeleteClass.IsChecked.Value)
                {
                    HandleData.RemoveClass((Classes)RemoveClass.SelectedItem);
                }
            }
            if (DirectoryTextBlock.Text.Length > 0)
            {
                Properties.Settings.Default.DefaultFile = newFile;
                Properties.Settings.Default.DefaultPath = System.IO.Path.GetDirectoryName(newFile);
            }

            Properties.Settings.Default.AutoSave = (AutoSave.IsChecked.Value == true) ? true : false;
            Properties.Settings.Default.AutoLoad = (AutoLoad.IsChecked.Value == true) ? true : false;
            Properties.Settings.Default.StoreDeleted = (StoreDeleted.IsChecked.Value == true) ? true : false;

            Properties.Settings.Default.Save();
            ClearForm();
            FillClassBoxes();
        }

        private void Settings_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!HandleData.StoreDeleted())
            {
                List<Entry> list = new List<Entry>();
                foreach(Entry entry in HandleData.GetAllEntries())
                {
                    if (!entry.Deleted)
                        list.Add(entry);
                }
                HandleData.ReplaceEntryList(list);
            }
        }

        private void FillClassBoxes()
        {
            RemoveClass.Items.Clear();
            List<Classes> list = HandleData.LoadClasses();
            foreach (Classes c in list)
            {
                RemoveClass.Items.Add(c);   
            }
            if(RemoveClass.Items.Count == 0)
            {
                RemoveClass.Items.Add(new Classes("Misc","Gray"));
            }
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "ToDoList (.todolist)|*.todolist"; // Filter files by extension

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string s = openFileDialog.FileName;
                HandleData.SetFilePath(s);
                newFile = s;
                DirectoryTextBlock.Text = s;

            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Are You Sure You Want To Restore The Settings?", "Restore Settings", MessageBoxButtons.YesNo);

            if (dialogResult == System.Windows.Forms.DialogResult.Yes)
            {
                Properties.Settings.Default.Reset();
                RefreshBox();
                System.Windows.MessageBox.Show("Settings Restored to Default");
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveChanges();
            RefreshBox();
            
        }

        public void RefreshBox()
        {
            RemoveClass.Items.Clear();
            FillClassBoxes();
            RemoveClass.SelectedIndex = 0;
            cp.SelectedColor = null;
            AutoLoad.IsChecked = (Properties.Settings.Default.AutoLoad == true) ? true : false;
            StoreDeleted.IsChecked = (Properties.Settings.Default.StoreDeleted == true) ? true : false;
            AutoSave.IsChecked = (Properties.Settings.Default.AutoSave == true) ? true : false;
            AddClass.Text = "";
        }

        private void ClearForm()
        {
            AddClass.Text = "";
            RemoveClass.SelectedIndex = 0;
        }

        private void DeleteEdit(object sender, RoutedEventArgs e)
        {
            if((DeleteClass.IsChecked.Value) || (EditColor.IsChecked.Value))
            {
                AddClass.Text = "";
                AddClass.IsEnabled = false;
            }
            else
            {
                AddClass.IsEnabled = true;
            }

            if (DeleteClass.IsChecked.Value)
            {
                cp.IsEnabled = false;
            }
            else
            {
                cp.IsEnabled = true;
            }
            if( (DeleteClass.IsChecked.Value) && (EditColor.IsChecked.Value))
            { 
                System.Windows.Controls.CheckBox check = sender as System.Windows.Controls.CheckBox;
                if(check != null)
                {
                    if(check.Name == DeleteClass.Name)
                    {
                        EditColor.IsChecked = false;
                    }
                    else
                    {
                        DeleteClass.IsChecked = false;
                    }
                }
            }
        }

        private void RemoveClass_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(AddClass.Text.Length == 0)
            {
                Classes classes = RemoveClass.SelectedItem as Classes;
                if(classes != null)
                {
                    Color? color = (Color?)ColorConverter.ConvertFromString(classes.Color);
                    cp.SelectedColor = color;
                }
            }
        }

        private void AddClass_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(AddClass.Text.Length == 1)
            {
                cp.SelectedColor = null;
            }
        }

        private void RemoveClass_DropDownOpened(object sender, EventArgs e)
        {
            if(RemoveClass.Items.Count == 1)
            {
                Classes classes = RemoveClass.SelectedItem as Classes;
                if (classes != null)
                {
                    Color? color = (Color?)ColorConverter.ConvertFromString(classes.Color);
                    cp.SelectedColor = color;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs eventArgs)
        {
            MessageBoxResult result = System.Windows.MessageBox.Show("Permanently Delete all Deleted and Completed Items?", "Purge List", MessageBoxButton.YesNo);
            if(result == MessageBoxResult.Yes)
            {
                List<Entry> allEntries = HandleData.GetAllEntries();
                List<Entry> resultList = new List<Entry>();
                foreach(Entry e in allEntries)
                {
                    if (!e.Deleted && !e.Completed)
                        resultList.Add(e);
                }
                HandleData.ReplaceEntryList(resultList);
            }
        }
    }
}
