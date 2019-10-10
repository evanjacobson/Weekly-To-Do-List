using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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
    /// Interaction logic for Syllabus_Library.xaml
    /// </summary>
    public partial class Syllabus_Library : Window
    {
        List<Syllabus> syllabi = new List<Syllabus>();
        public Syllabus_Library()
        {
            InitializeComponent();
            syllabi = LoadSyllabi();
            PopulateSyllabi();
        }

        private void PopulateSyllabi()
        {
            SyllabiBox.Items.Clear();
            foreach(Syllabus s in syllabi)
            {
                ListBoxItem item = new ListBoxItem { Content = s };
                item.Selected += Item_Selected;
                SyllabiBox.Items.Add(item);
            }
        }

        private void Item_Selected(object sender, RoutedEventArgs e)
        {
            Open.IsEnabled = true;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Syllabus syllabus = new Syllabus(openFileDialog.SafeFileName, openFileDialog.FileName);
                syllabi.Add(syllabus);
                SaveSyllabi(syllabi);
                PopulateSyllabi();
            }            
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            ListBoxItem item = SyllabiBox.SelectedItem as ListBoxItem;
            if (item != null)
            {
                Syllabus s = item.Content as Syllabus;
                if (s != null)
                {
                    syllabi.Remove(s);
                    SaveSyllabi(syllabi);
                    SyllabiBox.Items.Remove(item);
                    Open.IsEnabled = false;
                    PopulateSyllabi();
                }
            }
        }

        private static List<Syllabus> LoadSyllabi()
        {
            System.Windows.Clipboard.SetText(Properties.Settings.Default.Syllabi);
            List<Syllabus> classe = new List<Syllabus>();
            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(Properties.Settings.Default.Syllabi)))
            {
                BinaryFormatter bf = new BinaryFormatter();

                if (ms.Length > 0)
                {
                    List<Syllabus> stored = (List<Syllabus>)bf.Deserialize(ms);
                    foreach (Syllabus c in stored)
                    {
                        if (c != null)
                        {
                            classe.Add(c);
                        }
                    }
                    return classe;
                }
                List<Syllabus> classes = new List<Syllabus>();
                return classes;
            }
        }

        private static void SaveSyllabi(List<Syllabus> classes)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, classes);
                ms.Position = 0;
                byte[] buffer = new byte[(int)ms.Length];
                ms.Read(buffer, 0, buffer.Length);
                Properties.Settings.Default.Syllabi = Convert.ToBase64String(buffer);
                Properties.Settings.Default.Save();
            }
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            ListBoxItem item = SyllabiBox.SelectedItem as ListBoxItem;
            if (item != null)
            {
                Syllabus s = item.Content as Syllabus;
                if (s != null)
                {
                    System.Diagnostics.Process.Start(s.FilePath);
                }
            }
        }
    }

    [Serializable()]
    class Syllabus
    {
        public Syllabus(String Name, String FilePath)
        {
            this.Name = Name;
            this.FilePath = FilePath;
        }

        public override string ToString()
        {
            return Name;
        }
        public String Name { get; set; }
        public String FilePath { get; set; }
    }
}
