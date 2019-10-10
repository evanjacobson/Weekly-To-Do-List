using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


namespace Weekly_ToDo_List
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {        

        public MainWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            InitializeComponent();

            MonthLabel.Content = DateTime.Now.ToString("MMM");
            DayLabel.Content = DateTime.Now.Day.ToString();

            Closing += MainWindow_Closing;

            if((Properties.Settings.Default.AutoLoad == true) && (Properties.Settings.Default.DefaultFile != null) && (Properties.Settings.Default.DefaultFile != ""))
            {
                HandleData.LoadToDoList(Properties.Settings.Default.DefaultFile);
            }
        }

        /* navigation */

        private void Slot_Machine(object sender, RoutedEventArgs e)
        {
            SlotMachine slot = new SlotMachine();
            slot.Owner = this;
            slot.Show();
            slot.Owner = null;
            Close();
        }

        private void To_Do_List(object sender, RoutedEventArgs e)
        {
            List toDoList = new List();
            toDoList.Owner = this;
            toDoList.Show();
            toDoList.Owner = null;
            Close();
        }

        private void Today_ToDoList(object sender, RoutedEventArgs e)
        {
            Today today = new Today(DateTime.Now);
            today.Owner = this;
            today.Show();
            today.Owner = null;
            Close();
        }

        /*  CLICK EVENTS */
        private void Load_Click(object sender, RoutedEventArgs e)
        {
            HandleData.LoadToDoList();
        }

        private void Add_Items(object sender, RoutedEventArgs e)
        {
            AddItems();
        }

        private void AddItems()
        {
            AddItem addItem = new AddItem();
            addItem.Owner = this;
            addItem.Show();
        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            HandleData.SaveAs();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            HandleData.Save(Properties.Settings.Default.DefaultFile);
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            HandleData.OpenSettings(this);
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            HandleData.NewClick(this);
        }

        private void Calendar_Click(object sender, RoutedEventArgs e)
        {
            HandleData.CalendarClick(sender, e, this);
        }

        private void Syllabus_Click(object sender, RoutedEventArgs e)
        {
            HandleData.Syllabus(this);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            HandleData.Exit();
        }

        private void Read_Me_Click(object sender, RoutedEventArgs e)
        {
            HandleData.ReadMe(sender, e, this);
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            HandleData.AboutWindow(sender, e, this);
        }

        /* closing event */
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            HandleData.AutoSave(sender, e, this);
        }
    }
}
