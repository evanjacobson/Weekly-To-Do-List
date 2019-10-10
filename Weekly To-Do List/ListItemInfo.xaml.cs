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
    /// Interaction logic for ListItemInfo.xaml
    /// </summary>
    public partial class ListItemInfo : Window
    {
        private Entry entry;
        DateTime assignDate;
        DateTime dueDate;

    /* constructor for Today's List & Weekly List */
        public ListItemInfo(ListBoxItem l, DateTime day)
        {
            assignDate = day.Date;
            FinishConstructor(l.Content as Entry);
        }

    /* constructor for slot machine */
        public ListItemInfo(Entry e, DateTime day)
        {
            assignDate = day.Date;
            FinishConstructor(e);
        }
    
    /* finishing constructor for both */
        public void FinishConstructor(Entry e)
        {
            InitializeComponent();
            entry = e;
            dueDate = (e.Repeats) ? assignDate : e.DueOn;
            Repeats.Content = RepeatContent();
            Repeats.Visibility = (e.Repeats) ? Visibility.Visible : Visibility.Hidden;
            
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            Assignment.Text = entry.Assignment;
            Due_On.Text = dueDate.ToShortDateString();
            Assign_On.Text = assignDate.ToShortDateString();
            Notes.Text = entry.Notes;
            Class.Text = entry.ForClass.Name;
            Title = Assignment.Text;

            Delete.IsEnabled = (entry.HasDeletedDate(assignDate)) ? false : true;
            Completed.IsEnabled = (entry.HasCompletedDate(assignDate) || entry.Completed == true) ? false : true;

            Closing += ListItemInfo_Closing;
        }

    /* display repeat frequency */
        private string RepeatContent()
        {
            String s = "";
            if (entry.Repeats)
            {
                if (entry.Repeats_Daily)
                    s = "Repeats Daily";
                if (entry.Repeats_Weekly)
                    s = "Repeats Weekly";
                if (entry.Repeats_Monthly)
                    s = "Repeats Monthly";
                if (entry.Repeats_Yearly)
                    s = "Repeats Yearly";
            }
            else
            {
                s = "";
            }
            return s;
        }

    
    /* delete event */
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (!entry.Repeats)
            {
                HandleData.RemoveEntry(entry);
                MessageBox.Show($"{Assignment.Text} Deleted");
                Owner = null;
                Close();
            }
            else
            {
                MessageBoxResult result = HandleData.CustomMessageBox("Would You Like to Delete Only This Event, or All Instances of This Event?", "Choose How You Would Like to Delete This Event", "This Instance", "All Instances", "Cancel", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if ((result == MessageBoxResult.Yes) || (result == MessageBoxResult.No))
                {
                    if (result == MessageBoxResult.Yes)
                    {
                        entry.AddDeletedDay(assignDate);
                    }
                    else if (result == MessageBoxResult.No)
                    {
                        entry.Deleted = true;
                        MessageBox.Show($"{Assignment.Text} Deleted");
                    }
                    Owner = null;
                    Close();
                }
            }
        }

    /* completed event */
        private void Completed_Click(object sender, RoutedEventArgs e)
        {
            if (!entry.Repeats)
            {
                HandleData.RemoveEntry(entry);
                entry.Completed = true;
                HandleData.AddToList(entry);
            }
            else
            {
                entry.AddCompletedDay(assignDate);
            }

            Owner = null;
            Close();

            MessageBox.Show($"{Assignment.Text} Marked as Complete for {Assign_On.Text}!");
        }

    /* closing event */
        private void ListItemInfo_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            HandleData.AutoSave(sender, e, this);
        }

    }
}
