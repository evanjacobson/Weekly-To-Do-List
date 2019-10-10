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
using System.Windows.Shapes;

namespace Weekly_ToDo_List
{
    /// <summary>
    /// Interaction logic for List.xaml
    /// </summary>
    public partial class Today : Window
    {
        private DateTime sunday = DateTime.Now.Sunday();
        private DateTime saturday = DateTime.Now.Saturday();
        private bool sortTimeAsc = false;
        private bool sortTimeDesc = false;
        private bool sortDefault = true;
        private DateTime today;

        public Today(DateTime date)
        {
            this.today = date;
            sunday = date.Sunday();
            saturday = date.Saturday();
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            InitializeComponent();

            Closing += List_Closing;
            DeletedCheckBox.Visibility = (HandleData.StoreDeleted()) ? Visibility.Visible : Visibility.Hidden;

            Refresh();
        }

        private void Refresh()
        {
            SetWeekLabel();
            DeletedCheckBox.Visibility = (HandleData.StoreDeleted()) ? Visibility.Visible : Visibility.Hidden;
            Wipe();
            PopulateListBox(HandleData.GetList(DeletedCheckBox, CompletedCheckBox));
        }

        private void Wipe()
        {
            Box.Items.Clear();
        }


        private void SetWeekLabel()
        {
            Week_Label.Content = $"{today.DayOfWeek}, {today.ToString("MMMM")} {today.Day}, {today.Year}";
        }


        /* populate list box */
        private void PopulateListBox(List<Entry> list)
        {
            List<Entry> result = HandleData.LinqQueryList(list, sunday, saturday, sortTimeAsc, sortTimeDesc);

            foreach (Entry e in result)
            {
                DayOfWeek day = e.AssignToDate.DayOfWeek;

                if (e.Repeats_Daily)
                {
                    day = today.DayOfWeek;
                }
                if (e.Repeats_Monthly)
                {
                    if (HandleData.NumMonths(e, sunday) > 0)
                    {
                        DateTime month = e.AssignToDate.AddMonths(HandleData.NumMonths(e, today));
                        day = month.DayOfWeek;
                    }
                }
                else if (e.Repeats_Yearly)
                {
                    DateTime year = e.AssignToDate.AddYears(HandleData.NumYears(e, today));
                    day = year.DayOfWeek;
                }

                if (day == today.DayOfWeek)
                {
                    HandleData.CheckDateAddToBox(e, today.Date, today, DeletedCheckBox, CompletedCheckBox, Box).Selected += Item_Selected;
                }
            }
        }

        private void Yesterday()
        {
            today = today.AddDays(-1);
            sunday = today.Sunday();
            saturday = today.Saturday();


            Refresh();
        }

        private void Tomorrow()
        {
            today = today.AddDays(1);
            sunday = today.Sunday();
            saturday = today.Saturday();

            Refresh();
        }

        private void GoToToday()
        {
            sunday = DateTime.Now.Sunday();
            saturday = DateTime.Now.Saturday();
            today = DateTime.Now;
            Refresh();
        }

        //sort handler
        private void SortViewControl(bool TimeAscending, bool TimeDescending, bool Default)
        {
            Time_Ascending.IsCheckable = TimeAscending;
            Time_Ascending.IsChecked = TimeAscending;
            sortTimeAsc = TimeAscending;
            Time_Descending.IsCheckable = TimeDescending;
            Time_Descending.IsChecked = TimeDescending;
            sortTimeDesc = TimeDescending;
            Sort_Default.IsCheckable = Default;
            Sort_Default.IsChecked = Default;
            sortDefault = Default;
        }

        /* selected event */
        private void Item_Selected(object sender, RoutedEventArgs e)
        {
            if (sender is ListBoxItem)
            {
                ListBoxItem l = sender as ListBoxItem;
                if (l != null)
                {
                    ListItemInfo lio = new ListItemInfo(l, today);
                    lio.Owner = this;
                    lio.Show();
                    lio.Closing += Lio_Closing;
                }
            }
        }

    /* closing events */
        private void Lio_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Refresh();
        }

        private void List_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            HandleData.AutoSave(sender, e, this);
        }

        private void AddItem_Closed(object sender, EventArgs e)
        {
            Refresh();
        }

        private void Settings_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Refresh();
        }

        /*  CLICK EVENTS */
        private void Load_Click(object sender, RoutedEventArgs e)
        {
            HandleData.LoadToDoList();
            Refresh();
        }

        private void Add_Items(object sender, RoutedEventArgs e)
        {
            AddItem addItem = new AddItem();
            addItem.Owner = this;
            addItem.Show();
            addItem.Closing += AddItem_Closed;
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
            Settings settings = new Settings();
            settings.Owner = this;
            settings.Show();
            settings.Owner = null;

            settings.Closing += Settings_Closing;
        }

        private void Yesterday_Click(object sender, RoutedEventArgs e)
        {
            Yesterday();
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            HandleData.NewClick(this);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Owner = this;
            main.Show();
            main.Owner = null;
            Close();
        }

        private void Refresh(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void Time_Ascending_Click(object sender, RoutedEventArgs e)
        {
            SortViewControl(true, false, false);
            Refresh();
        }

        private void Time_Descending_Click(object sender, RoutedEventArgs e)
        {
            SortViewControl(false, true, false);
            Refresh();
        }

        private void Sort_Default_Click(object sender, RoutedEventArgs e)
        {
            SortViewControl(false, false, true);
            Refresh();
        }

        private void Week_Label_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            GoToToday();
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

        private void Today_Click(object sender, RoutedEventArgs e)
        {
            GoToToday();
        }

        private void Tomrorow_Click(object sender, RoutedEventArgs e)
        {
            Tomorrow();
        }       
    }
}
