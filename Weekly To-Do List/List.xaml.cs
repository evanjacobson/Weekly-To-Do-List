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
    public partial class List : Window
    {
        private DateTime sunday = DateTime.Now.Sunday();
        private DateTime saturday = DateTime.Now.Saturday();
        private bool sortTimeAsc = false;
        private bool sortTimeDesc = false;
        private bool sortDefault = true;

        public List()
        {
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            InitializeComponent();

            Refresh();

            Closing += List_Closing;
            DeletedCheckBox.Visibility = (HandleData.StoreDeleted()) ? Visibility.Visible : Visibility.Hidden;
        }

    

        private void PopulateListBox(List<Entry> list)
        {
            List<Entry> newList = HandleData.LinqQueryList(list, sunday, saturday, sortTimeAsc, sortTimeDesc);
            
            foreach (Entry e in newList)
                GenerateListBoxItem(e);
        }
       
        public void GenerateListBoxItem(Entry e)
        {
            ListBox box;
            DayOfWeek day = e.AssignToDate.DayOfWeek;
            if (!e.Repeats_Daily)
            {
                if (e.Repeats_Monthly)
                {
                    if (HandleData.NumMonths(e, sunday) > 0)
                    {
                        DateTime month = e.AssignToDate.AddMonths(HandleData.NumMonths(e, sunday));
                        day = month.DayOfWeek;
                    }
                }
                else if (e.Repeats_Yearly)
                {
                    DateTime year = e.AssignToDate.AddYears(HandleData.NumYears(e, sunday));
                    day = year.DayOfWeek;
                }

                switch (day)
                {
                    case DayOfWeek.Sunday:
                        box = Sunday;
                        break;
                    case DayOfWeek.Monday:
                        box = Monday;
                        break;
                    case DayOfWeek.Tuesday:
                        box = Tuesday;
                        break;
                    case DayOfWeek.Wednesday:
                        box = Wednesday;
                        break;
                    case DayOfWeek.Thursday:
                        box = Thursday;
                        break;
                    case DayOfWeek.Friday:
                        box = Friday;
                        break;
                    case DayOfWeek.Saturday:
                        box = Saturday;
                        break;
                    default:
                        box = Saturday;
                        break;
                }
                AddToBox(e, box);
            }
            else
            {
                for (var i = 0; i < 7; i++){
                    switch (i)
                    {
                        case 0:
                            AddToBox(e, Sunday);
                            break;
                        case 1:
                            AddToBox(e, Monday);
                            break;
                        case 2:
                            AddToBox(e, Tuesday);
                            break;
                        case 3:
                            AddToBox(e, Wednesday);
                            break;
                        case 4:
                            AddToBox(e, Thursday);
                            break;
                        case 5:
                            AddToBox(e, Friday);
                            break;
                        case 6:
                            AddToBox(e, Saturday);
                            break;
                        default:
                            break;
                    }
                    }
                }
            }

        private void AddToBox(Entry e, ListBox box)
        {
            DateTime date = box.GetDateTime(sunday);
            HandleData.CheckDateAddToBox(e, date, date, DeletedCheckBox, CompletedCheckBox, box).Selected += Item_Selected;
        }        

        private void Refresh()
        {
            SetWeekLabel();
            Wipe();
            DeletedCheckBox.Visibility = (HandleData.StoreDeleted()) ? Visibility.Visible : Visibility.Hidden;
            PopulateListBox(HandleData.GetList(DeletedCheckBox, CompletedCheckBox));
        }

        private void SetWeekLabel()
        {
            Week_Label.Content = "Week of " + sunday.ToShortDateString();
        }

        private void Wipe()
        {
            Sunday.Items.Clear();
            Monday.Items.Clear();
            Tuesday.Items.Clear();
            Wednesday.Items.Clear();
            Thursday.Items.Clear();
            Friday.Items.Clear();
            Saturday.Items.Clear();
        }

        
    //switch weeks
        private void LastWeek()
        {
            sunday = sunday.AddDays(-7).Sunday();
            saturday = saturday.AddDays(-7).Saturday();
            Refresh();
        }

        private void NextWeek()
        {
            sunday = sunday.AddDays(7).Sunday();
            saturday = saturday.AddDays(7).Saturday();
            Refresh();
        }

        private void ThisWeek()
        {
            sunday = DateTime.Now.Sunday();
            saturday = DateTime.Now.Saturday();
            Refresh();
        }

        
    /* sort handler */

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
                    ListItemInfo lio = new ListItemInfo(l, (l.Parent as ListBox).GetDateTime(sunday));
                    lio.Owner = this;
                    lio.Show();
                    lio.Closing += Lio_Closing;
                }
            }
        }


        /* closing events */
        private void List_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            HandleData.AutoSave(sender, e, this);
        }

        private void Lio_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Refresh();
        }
        private void AddItem_Closed(object sender, EventArgs e)
        {
            Refresh();
        }

        private void Settings_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Refresh();
        }


        /* CLICK EVENTS */

        private void Refresh(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

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

        private void Last_Week_Click(object sender, RoutedEventArgs e)
        {
            LastWeek();
        }

        private void Next_Week_Click(object sender, RoutedEventArgs e)
        {
            NextWeek();
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

        private void This_Week_Click(object sender, RoutedEventArgs e)
        {
            ThisWeek();
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

        private void Calendar_Click(object sender, RoutedEventArgs e)
        {
            HandleData.CalendarClick(sender, e, this);
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

        private void Syllabus_Click(object sender, RoutedEventArgs e)
        {
            HandleData.Syllabus(this);
        }
    }
}
