using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Weekly_ToDo_List
{
    /// <summary>
    /// Interaction logic for ChoreWheel.xaml
    /// </summary>
    public partial class SlotMachine : Window
    {
        private DateTime sunday = DateTime.Now.Sunday();
        private DateTime saturday = DateTime.Now.Saturday();
        private List<Entry> slots = new List<Entry>();
        private Color rectColor;
        private SolidColorBrush rectFill;
        private MediaPlayer sound = new MediaPlayer();
        private MediaPlayer whir = new MediaPlayer();
        private Uri whirsound = new Uri(@"C:\Users\Administrator\source\repos\Weekly ToDo List\Weekly ToDo List\bin\Debug\slot_whir.wav");
        private Uri leversound = new Uri(@"C:\Users\Administrator\source\repos\Weekly ToDo List\Weekly ToDo List\bin\Debug\lever.wav");
        private Uri slothitsound = new Uri(@"C:\Users\Administrator\source\repos\Weekly ToDo List\Weekly ToDo List\bin\Debug\slot_hit.wav");
        private Uri cheeringsound = new Uri(@"C:\Users\Administrator\source\repos\Weekly ToDo List\Weekly ToDo List\bin\Debug\cheering.wav");
        private Uri trombonesound = new Uri(@"C:\Users\Administrator\source\repos\Weekly ToDo List\Weekly ToDo List\bin\Debug\trombone.wav");
        private Uri tadasound = new Uri(@"C:\Users\Administrator\source\repos\Weekly ToDo List\Weekly ToDo List\bin\Debug\tada.wav");

        public SlotMachine()
        {
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            InitializeComponent();
            LoadSlots();
            rectColor = (Color)ColorConverter.ConvertFromString("#FFF4F4F5");
            rectFill = new SolidColorBrush(rectColor);
            sound.Open(leversound);

            Closing += ChoreWheel_Closing;
        }

        private void Reset()
        {
            WeekItems.IsEnabled = true;
            TodaysItems.IsEnabled = true;
            Lever.Visibility = Visibility.Visible;
        }

        private void LoadSlots()
        {
            slots.Clear();

            List<Entry> list = HandleData.LinqQueryList(HandleData.GetEntries(), sunday, saturday, false, false);
            

            if (TodaysItems.IsChecked.Value == true)
            {
                foreach (Entry e in list)
                {
                    DateTime today = DateTime.Now.Date;
                    if (e.Repeats)
                    {
                        bool isDeleted = e.HasDeletedDate(today);
                        bool isCompleted = e.HasCompletedDate(today);

                        if (!isDeleted && !isCompleted)
                        {
                            if (e.Repeats_Daily && (e.AssignToDate.Date <= today.Date) && (today.Date <= e.DueOn))
                            {
                                slots.Add(e);
                            }
                            else if (e.Repeats_Weekly)
                            {
                                if (e.AssignToDate.DayOfWeek == today.DayOfWeek)
                                {
                                    slots.Add(e);
                                }
                            }
                            else if (e.Repeats_Monthly)
                            {
                                if (e.AssignToDate.Day == today.Day)
                                    slots.Add(e);
                            }
                            else if (e.Repeats_Yearly)
                            {
                                if (e.AssignToDate.Day == today.Day)
                                    slots.Add(e);
                            }
                        }
                    }
                    else
                    {
                        if (e.AssignToDate.Date == today.Date && !e.Deleted && !e.Completed)
                            slots.Add(e);
                    }
                }
            }
            else if (WeekItems.IsChecked.Value == true)
            {
                foreach (Entry e in list)
                {
                    DateTime start = e.AssignToDate.Date;
                    bool isDeleted = e.HasDeletedDate(start);
                    bool isCompleted = e.HasCompletedDate(start);

                    if (!(e.DueOn <= sunday || e.AssignToDate >= saturday))
                    {
                        if(!e.Deleted && !e.Completed)
                        {
                            if (e.Repeats)
                            {
                                while (start <= e.DueOn.Date)
                                {
                                    if (!(e.HasCompletedDate(start) || e.HasDeletedDate(start)))
                                    {
                                        if (e.Repeats_Yearly)
                                        {
                                            if ((Math.Abs(e.AssignToDate.DayOfYear - saturday.DayOfYear) <= 7))
                                            {
                                                slots.Add(e);
                                                break;
                                            }
                                        }
                                        else if (e.Repeats_Monthly)
                                        {
                                            if (Math.Abs(saturday.Day - e.AssignToDate.Day) <= 7)
                                            {
                                                slots.Add(e);
                                                break;
                                            }
                                        }
                                        slots.Add(e);
                                        break;
                                    }
                                    start.AddDays(1);
                                }
                            }
                            else
                            {
                                if (e.Repeats_Yearly)
                                {
                                    if ((Math.Abs(e.AssignToDate.DayOfYear - saturday.DayOfYear) <= 7))
                                    {
                                        slots.Add(e);
                                    }
                                }
                                else if (e.Repeats_Monthly)
                                {
                                    if (Math.Abs(saturday.Day - e.AssignToDate.Day) <= 7)
                                    {
                                        slots.Add(e);
                                    }
                                }
                                else
                                {
                                    slots.Add(e);
                                }
                            }
                                
                        }
                    }
                }
            }
        }
        

    /* ANIMATIONS */
        private DoubleAnimation LeverLeverSpinHeight = new DoubleAnimation
        {
             From = 137,
             To = 71,
             Duration = new Duration(new TimeSpan(0, 0, 0, 0, 400)),
             AutoReverse = true
        };

        private DoubleAnimation LeverLeverSpinPosition = new DoubleAnimation
        {
            From = 12,
            To = 77,
            Duration = new Duration(new TimeSpan(0, 0, 0, 0, 400)),
            AutoReverse = true
        };

        private DoubleAnimation LeverBallSpinTop = new DoubleAnimation
        {
            From = 2,
            To = 60,
            Duration = new Duration(new TimeSpan(0, 0, 0, 0, 400)),
            AutoReverse = true
        };

        private DoubleAnimation LeverBallSpinLeft = new DoubleAnimation
        {
            From = 5,
            To = 1, 
            Duration = new Duration(new TimeSpan(0, 0, 0, 0, 400)),
            AutoReverse = true
        };


    //variables for timer 
        private int numTicks = 0;
        private int totalTicks = 0;
        private static System.Windows.Forms.Timer timer;
    
    //begin the slot game
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            HandleData.CheckForFile();
            if (slots.Count < 1)
            {
                MessageBox.Show(this, "There are no available to-do items. Either they don't exist, they are deleted, or they are completed. Please choose a different list or add some items!", "There are no to-do-list items!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            sound.Open(leversound);
            sound.Play();

            LeverLever.BeginAnimation(Ellipse.HeightProperty, LeverLeverSpinHeight);
            LeverLever.BeginAnimation(Canvas.TopProperty, LeverLeverSpinPosition);
            LeverBall.BeginAnimation(Canvas.TopProperty, LeverBallSpinTop);
            LeverBall.BeginAnimation(Canvas.LeftProperty, LeverBallSpinLeft);


            Lever.Visibility = Visibility.Hidden;
            Viewer1.Fill = rectFill;
            Viewer2.Fill = rectFill;
            Viewer3.Fill = rectFill;

            Jackpot.Visibility = Visibility.Hidden;
            Loser.Visibility = Visibility.Hidden;
            Winner.Visibility = Visibility.Hidden;



            timer = new System.Windows.Forms.Timer();

            totalTicks = 6000;

            timer.Interval = 100;
            timer.Tick += Timer_Tick;
            timer.Start();
            WeekItems.IsEnabled = false;
            TodaysItems.IsEnabled = false;
        }
    
        //timer tick event
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!(numTicks < 600))
            {
                whir.Open(whirsound);
                whir.Play();
            }

            if ((numTicks > (totalTicks / 4)) && (numTicks % (totalTicks / 3) == 0))
            {
                sound.Open(slothitsound);
                sound.Play();
            }
            if (numTicks <= (totalTicks / 3))
            {
                slots.Shuffle();
                Slot1.Content = slots.First();
                Slot1.Background = HandleData.GetClassColor(slots.First());
                slots.Shuffle();
                Slot2.Content = slots.First();
                Slot2.Background = HandleData.GetClassColor(slots.First());
                slots.Shuffle();
                Slot3.Content = slots.First();
                Slot3.Background = HandleData.GetClassColor(slots.First());
            }
            else if (numTicks <= (2 * (totalTicks / 3)))
            {
                slots.Shuffle();
                Slot2.Content = slots.First();
                Slot2.Background = HandleData.GetClassColor(slots.First());
                slots.Shuffle();
                Slot3.Content = slots.First();
                Slot3.Background = HandleData.GetClassColor(slots.First());
            }
            else if (numTicks < totalTicks)
            {
                slots.Shuffle();
                Slot3.Content = slots.First();
                Slot3.Background = HandleData.GetClassColor(slots.First());
            }

            numTicks += 100;

            if (numTicks >= totalTicks)
            {
                sound.Open(slothitsound);
                sound.Play();
                timer.Stop();
                timer.Dispose();
                DisplayResult();
                numTicks = 0;
                timer = null;
            }
        }

    //display slot results
        private void DisplayResult()
        {
            if (Slot1.Content == Slot2.Content)
            {
                if (Slot2.Content == Slot3.Content)
                    DisplayJackpot();
                else
                    DisplayWinner();

                DisplayResult(Slot1.Content as Entry, Viewer1, Viewer2);
            }
            else if (Slot1.Content == Slot3.Content)
            {
                if (Slot2.Content == Slot3.Content)
                    DisplayJackpot();
                else
                    DisplayWinner();
                DisplayResult(Slot1.Content as Entry, Viewer1, Viewer3);
            }
            else if (Slot2.Content == Slot3.Content)
            {
                if (Slot1.Content == Slot2.Content)
                    DisplayJackpot();
                else
                    DisplayWinner();
                DisplayResult(Slot2.Content as Entry, Viewer2, Viewer3);
            }
            else
                DisplayLoser();
            Reset();
        }

    //winner
        private void DisplayWinner()
        {
            Winner.Visibility = Visibility.Visible;
            sound.Open(cheeringsound);
            sound.Play();
        }

    //loser
        private void DisplayLoser()
        {
            Loser.Visibility = Visibility.Visible;
            sound.Open(trombonesound);
            sound.Volume = 0.2;
            sound.Play();
        }

    //jackpot
        private void DisplayJackpot()
        {
            Jackpot.Visibility = Visibility.Visible;
            Viewer1.Fill = new SolidColorBrush(Colors.Green);
            sound.Open(tadasound);
            sound.Play();
        }

    //color the slots upon winning
        private void DisplayResult(Entry e1, Rectangle rect1, Rectangle rect2)
        {
            ShowTask(e1);
            rect1.Fill = new SolidColorBrush(Colors.Green);
            rect2.Fill = new SolidColorBrush(Colors.Green);
        }

    //display result
        private void ShowTask(Entry e)
        {
            DateTime date = DateTime.Now.Date;
            
            while((e.AssignToDate > date) || (e.HasDeletedDate(date) || e.HasCompletedDate(date)))
            {
                date = date.AddDays(1);
            }

            ListItemInfo listItemInfo = new ListItemInfo(e, date);
            listItemInfo.Owner = this;
            listItemInfo.Show();
        }
        

        /* click events */

        private void New_Click(object sender, RoutedEventArgs e)
        {
            HandleData.NewClick(this);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Owner = this;
            main.Show();
            main.Owner = null;
            Close();
        }

        private void Switch_Slots(object sender, RoutedEventArgs e)
        {
            LoadSlots();
        }

        private void Calendar_Click(object sender, RoutedEventArgs e)
        {
            HandleData.CalendarClick(sender, e, this);
        }

        private void Read_Me_Click(object sender, RoutedEventArgs e)
        {
            HandleData.ReadMe(sender, e, this);
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            HandleData.AboutWindow(sender, e, this);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            HandleData.Exit();
        }

        private void Syllabus_Click(object sender, RoutedEventArgs e)
        {
            HandleData.Syllabus(this);
        }

        /*  CLICK EVENTS */
        private void Load_Click(object sender, RoutedEventArgs e)
        {
            HandleData.LoadToDoList();
        }

        private void Add_Items(object sender, RoutedEventArgs e)
        {
            AddItem addItem = new AddItem();
            addItem.Owner = this;
            addItem.Show();
            addItem.Closed += AddItem_Closed;
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

        /* closing */
        private void ChoreWheel_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            HandleData.AutoSave(sender, e, this);
            if (timer != null)
                timer.Dispose();
        }

        private void AddItem_Closed(object sender, EventArgs e)
        {
            HandleData.SetModified(true);
        }

    }
}
