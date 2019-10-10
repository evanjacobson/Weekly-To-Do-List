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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class AddItem : Window
    {
        private bool hasAssignment = false;
        private bool hasDueDate = false;
        private bool hasAssignedDate = false;

        public AddItem()
        {
            HandleData.CheckForFile();

            InitializeComponent();
            foreach(Classes s in HandleData.GetClasses())
            {
                ClassPicker.Items.Add(s);
            }
            ClassPicker.SelectedIndex = 0;

            Repeats_Daily.Checked += Repeat_Check;
            Repeats_Weekly.Checked += Repeat_Check;
            Repeats_Monthly.Checked += Repeat_Check;
            Repeats_Yearly.Checked += Repeat_Check;
            Repeats_Daily.Unchecked += Repeat_Check;
            Repeats_Weekly.Unchecked += Repeat_Check;
            Repeats_Monthly.Unchecked += Repeat_Check;
            Repeats_Yearly.Unchecked += Repeat_Check;
            Repeats_Never.Checked += Repeat_Check;
            Repeats_Never.Unchecked += Repeat_Check;

            Closing += AddItem_Closing;
        }

    
    
    

    //Add the item
        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            if(Properties.Settings.Default.DefaultFile == "")
            {
                MessageBox.Show("Please choose a file in order to begin your list");
                return;
            }


            DateTime due = DueBy.SelectedDate.Value;
            DateTime assign = assignToDay.SelectedDate.Value;

            if(due < assign)
            {
                MessageBox.Show("Error: Due Date Cannot Be Before the Assigned Date");
                return;
            }

            String label = assignment.Text;
            Classes forclass = (Classes)ClassPicker.SelectedItem;
            String notes = Notes_TextBox.Text;
            List<bool> repeats = new List<bool>();
            repeats.Add((Repeats_Daily.IsChecked.Value) ? true : false);
            repeats.Add((Repeats_Weekly.IsChecked.Value) ? true : false);
            repeats.Add((Repeats_Monthly.IsChecked.Value) ? true : false);
            repeats.Add((Repeats_Yearly.IsChecked.Value) ? true : false);

            int minutesToComplete = (int)Minutes.Value;

            Entry ent = new Entry(label, forclass, notes, assign, due, minutesToComplete, (Repeats_Daily.IsChecked.Value == true) ? true : false, (Repeats_Weekly.IsChecked.Value == true) ? true : false, (Repeats_Monthly.IsChecked.Value == true) ? true : false, (Repeats_Yearly.IsChecked.Value == true) ? true : false);
            HandleData.AddToList(ent);
            ClearItems();
        }


    
    //generate listboxitem
        public static ListBoxItem GenListBoxItem(Entry e)
        {
            ListBoxItem item = new ListBoxItem
            {
                Content = e
            };

            return item;
        }

     //clear items
        private void ClearItems()
        {
            assignment.Text = "";
            ClassPicker.SelectedIndex = 0;
            DueBy.Text = "";
            assignToDay.Text = "";
            Notes_TextBox.Text = "";
            hasAssignedDate = false;
            hasAssignment = false;
            hasDueDate = false;
            EnableAddClass();
            Minutes.Value = 0;
            MinutesDisplay.Text = "";
            Repeats_Daily.IsChecked = false;
            Repeats_Monthly.IsChecked = false;
            Repeats_Weekly.IsChecked = false;
            Repeats_Yearly.IsChecked = false;
            Repeats_Never.IsChecked = true;
            DueBy.DisplayDateEnd = null;
            assignToDay.DisplayDateEnd = null;
        }

    //date changed event  -- keep safe range (cannot end before it starts)
        private void DueBy_DateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DueBy.SelectedDate.HasValue)
            {
                assignToDay.DisplayDateEnd = DueBy.SelectedDate;
                if (assignToDay.SelectedDate.HasValue && DueBy.SelectedDate.Value < assignToDay.SelectedDate.Value)
                {
                    assignToDay.SelectedDate = null;
                }
                else
                {
                    hasDueDate = true;
                    EnableAddClass();
                }
            }
            else
            {
                assignToDay.DisplayDateEnd = null;
            }
        }

    //date changed event -- keep safe range (cannot start before it ends)
        private void Assigned_DateChanged(object sender, SelectionChangedEventArgs e)
        {
            if(assignToDay.SelectedDate.HasValue)
            {
                DueBy.DisplayDateStart = assignToDay.SelectedDate;
                if(DueBy.SelectedDate.HasValue && DueBy.SelectedDate.Value < assignToDay.SelectedDate.Value)
                {
                    DueBy.SelectedDate = null;
                }
                else
                {
                    hasAssignedDate = true;
                    EnableAddClass();
                }
            }
            else
            {
                DueBy.DisplayDateStart = null;
            }
        }

    //check if requirements have been met to add the class safely
        private void EnableAddClass()
        {
            if(hasAssignment && hasAssignedDate && hasDueDate)
                Add_Item.IsEnabled = true;   
            else
                Add_Item.IsEnabled = false;
        }

    //text changed event --- requirement for safe constructor
        private void assignment_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox)
            {
                TextBox t = sender as TextBox;
                if (t != null)
                {
                    if (t.Text.Length > 0)
                    {
                        hasAssignment = true;
                        EnableAddClass();
                    }
                }
            }
        }

        //handling events for the minutes slider & display
        //update slider text display
        private void Minutes_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        { 
            MinutesDisplay.Text = Minutes.Value.ToString();
        }
        //integers ONLY in the minutes textbox
        private void MinutesDisplay_KeyDown(object sender, KeyEventArgs e)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(e.Key.ToString(), "\\d+"))
                e.Handled = true;
        }
        //move the minutes slider along with the textbox
        private void MinutesDisplay_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(MinutesDisplay.Text.Length != 0)
            {
                try
                {
                    int val = Convert.ToInt32(MinutesDisplay.Text);
                    if(val > Minutes.Maximum)
                    {
                        Minutes.Maximum = val;
                    }
                    else
                    {
                        Minutes.Maximum = 120;
                    }
                    Minutes.Value = val;
                }
                catch(Exception ex)
                {
                    Minutes.Value = 0;
                    Console.Write(ex.Message);
                }
                
            }
            else
            {
                Minutes.Value = 0;
                MinutesDisplay.Text = "";
                Minutes.Maximum = 120;
            }
            
        }

        //radiobutton event
        private void Repeat_Check(object sender, RoutedEventArgs e)
        {
            if (Repeats_Never.IsChecked.Value == true)
            {
                DueLabel.Content = "Due On*";
                DoLabel.Content = "Assign to day*";
            }
            else
            {
                DueLabel.Content = "End Date*";
                DoLabel.Content = "Start Date*";
            }
        }

        //click event
        private void DueToday_Click(object sender, RoutedEventArgs e)
        {
            DueBy.Text = DateTime.Now.Date.ToShortDateString();
        }
        //click event   
        private void AssignToday_Click(object sender, RoutedEventArgs e)
        {
            assignToDay.Text = DateTime.Now.Date.ToShortDateString();
        }
        //click event
        private void ClearItems_Click(object sender, RoutedEventArgs e)
        {
            ClearItems();
        }


        //closing event
        private void AddItem_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            HandleData.AutoSave(sender, e, this);
        }
    }
}
