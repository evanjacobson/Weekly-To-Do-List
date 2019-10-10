using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;

namespace Weekly_ToDo_List
{
    public static class HandleData
    {
        private static List<Entry> entries = new List<Entry>();
        private static bool modified;
        private static String currentFilePath = ""; // file path
        private static List<Classes> classes = LoadClasses();

        /* ENTRY RELATED METHODS */
        public static List<Entry> GetList(System.Windows.Controls.CheckBox DeletedCheckBox, System.Windows.Controls.CheckBox CompletedCheckBox)
        {
            List<Entry> list = new List<Entry>();
            if (DeletedCheckBox.IsVisible)
            {
                if ((DeletedCheckBox.IsChecked.Value == true) && (CompletedCheckBox.IsChecked.Value == true))
                {
                    list = HandleData.GetEntries(true, true);
                }
                else if ((DeletedCheckBox.IsChecked.Value == false) && (CompletedCheckBox.IsChecked.Value == true))
                {
                    list = HandleData.GetEntries(false, true);
                }
                else if ((DeletedCheckBox.IsChecked.Value == true) && (CompletedCheckBox.IsChecked.Value == false))
                {
                    list = HandleData.GetEntries(true, false);
                }
                else
                {
                    list = HandleData.GetEntries(false, false);
                }
            }
            else
            {
                if (CompletedCheckBox.IsChecked.Value == true)
                {
                    list = HandleData.GetEntries(false, true);
                }
                else
                {
                    list = HandleData.GetEntries(false, false);
                }
            }
            return list;
        }
        public static void ReplaceEntryList(List<Entry> list)
        {
            entries = list;
        }
        public static void AddToList(Entry e)
        {
            entries.Add(e);
            SetModified(true);
        }
        public static List<Entry> GetEntries()
        {
            List<Entry> entryList = new List<Entry>();
            foreach (Entry e in entries)
            {
                if ((e.Deleted == false) && (e.Completed == false))
                {
                    entryList.Add(e);
                }
            }
            return entryList;
        }
        public static List<Entry> GetAllEntries()
        {
            return entries;
        }
        public static List<Entry> GetEntries(bool IncludeDeletedEntries, bool IncludeCompletedEntries)
        {
            List<Entry> entryList = new List<Entry>();
            foreach (Entry e in entries)
            {
                if ((e.Deleted == false) && (e.Completed == false))
                {
                    entryList.Add(e);
                }
                else
                {
                    if (IncludeDeletedEntries && IncludeCompletedEntries)
                    {
                        return entries;
                    }
                    else
                    {
                        if (IncludeDeletedEntries)
                        {
                            if ((e.Deleted))
                            {
                                entryList.Add(e);
                            }
                        }
                        if (IncludeCompletedEntries)
                        {
                            if (e.Completed)
                            {
                                entryList.Add(e);
                            }
                        }
                    }
                }
            }
            return entryList;
        }
        public static void RemoveEntry(Entry e)
        {
            if (!StoreDeleted())
            {
                entries.Remove(e);
            }
            else
            {
                e.Deleted = true;
            }
            SetModified(true);
        }
        public static void MarkCompleted(Entry e)
        {
            e.Completed = true;
        }
        public static List<Entry> LinqQueryList(List<Entry> list, DateTime sunday, DateTime saturday, bool sortTimeAsc, bool sortTimeDesc)
        {
            //linq query to determine if it should be populated
            //these queries check for:
            //query line 1: if it's supposed to go on this week's calendar
            //line 2: if it repeats daily or weekly, just add it straight on
            //line 3: if it repeats monthly, add a month and check if it fits within that week
            //line 4: if it repeats yearly, add a year and check if it fits within that week
            //line 5: order by...
            //line 6: select  
            IEnumerable<Entry> en = list
                .Where(x =>
                    (x.Repeats && (
                        (x.AssignToDate >= sunday && x.AssignToDate <= saturday)
                        || (x.Repeats_Daily && (sunday >= x.AssignToDate))
                        || (x.Repeats_Weekly && (sunday >= x.AssignToDate))
                        || (x.Repeats_Monthly && ((x.AssignToDate.AddMonths(HandleData.NumMonths(x, sunday)) >= sunday) && (x.AssignToDate.AddMonths(HandleData.NumMonths(x, saturday)) <= saturday)))
                        || (x.Repeats_Yearly && ((x.AssignToDate.AddYears(HandleData.NumYears(x, sunday)) >= sunday) && (x.AssignToDate.AddYears(HandleData.NumYears(x, saturday))) <= saturday)))
                    )
                    ||
                    (x.AssignToDate >= sunday && x.AssignToDate <= saturday)
                    )
                  .Select(x => x);

            if (sortTimeAsc)
            {
                en = en.OrderBy(x => x.MinutesToComplete);
            }
            else if (sortTimeDesc)
            {
                en = en.OrderByDescending(x => x.MinutesToComplete);
            }

            return en.ToList();
        }
        public static ListBoxItem CheckDateAddToBox(Entry e, DateTime date, DateTime TodayOrSunday, System.Windows.Controls.CheckBox DeletedCheckBox, System.Windows.Controls.CheckBox CompletedCheckBox, System.Windows.Controls.ListBox box)
        {
            Entry entry = new Entry();
            entry = e;

            ListBoxItem item = new ListBoxItem
            {
                Content = entry,
                Background = HandleData.GetClassColor(e)
            };

            item.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;

            if (!((date > entry.DueOn.Date) || (date < entry.AssignToDate.Date)))
            {
                if (!(date >= e.AssignToDate.Date) || (date <= e.DueOn.Date))
                {
                    if ((entry.HasDeletedDate(date)) || (entry.HasCompletedDate(date)))
                    {
                        if (((entry.HasDeletedDate(TodayOrSunday)) && (DeletedCheckBox.IsChecked.Value == true))
                           || ((entry.HasCompletedDate(TodayOrSunday)) && (CompletedCheckBox.IsChecked.Value == true)))
                        {
                            box.Items.Add(item);
                            //item.Selected += Item_Selected;
                            return item;
                        }
                    }
                    else
                    {
                        box.Items.Add(item);
                        //item.Selected += Item_Selected;
                        return item;
                    }
                }
            }
            return new ListBoxItem();
        }



        /* CLASSES RELATED */
        public static List<Classes> LoadClasses()
        {
            System.Windows.Clipboard.SetText(Properties.Settings.Default.ClassList);
            List<Classes> classe = new List<Classes>();
            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(Properties.Settings.Default.ClassList)))
            {
                BinaryFormatter bf = new BinaryFormatter();

                if (ms.Length > 0)
                {
                    List<Classes> stored = (List<Classes>)bf.Deserialize(ms);
                    foreach (Classes c in stored)
                    {
                        if (c != null)
                        {
                            classe.Add(c);
                        }
                    }
                    return classe;
                }
                else Properties.Settings.Default.ClassList = "AAEAAAD/////AQAAAAAAAAAMAgAAAEdXZWVrbHkgVG9EbyBMaXN0LCBWZXJzaW9uPTEuMC4wLjAsIEN1bHR1cmU9bmV1dHJhbCwgUHVibGljS2V5VG9rZW49bnVsbAQBAAAAhgFTeXN0ZW0uQ29sbGVjdGlvbnMuR2VuZXJpYy5MaXN0YDFbW1dlZWtseV9Ub0RvX0xpc3QuQ2xhc3NlcywgV2Vla2x5IFRvRG8gTGlzdCwgVmVyc2lvbj0xLjAuMC4wLCBDdWx0dXJlPW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPW51bGxdXQMAAAAGX2l0ZW1zBV9zaXplCF92ZXJzaW9uBAAAGldlZWtseV9Ub0RvX0xpc3QuQ2xhc3Nlc1tdAgAAAAgICQMAAAABAAAAAQAAAAcDAAAAAAEAAAAEAAAABBhXZWVrbHlfVG9Eb19MaXN0LkNsYXNzZXMCAAAACQQAAAANAwUEAAAAGFdlZWtseV9Ub0RvX0xpc3QuQ2xhc3NlcwMAAAAEbmFtZQVjb2xvcgJpZAEBAAgCAAAABgUAAAAETWlzYwYGAAAACUxpZ2h0R3JheQAAAAAL";
                List<Classes> classes = new List<Classes>();
                classes.Add(new Classes("Misc", "Gray"));
                return classes;

            }
        }

        public static void AddToClasses(Classes c)
        {
            classes.Add(c);
        }

        public static List<Classes> GetClasses()
        {
            return classes;
        }

        public static void RemoveClass(Classes c)
        {
            List<Classes> newList = new List<Classes>();
            foreach (Classes classes in GetClasses())
            {
                if (classes.ID != c.ID)
                {
                    newList.Add(classes);
                }
            }
            SaveClasses(newList);
        }

        public static void EditClassColor(Classes c, String s)
        {
            List<Classes> list = LoadClasses();
            List<Classes> newList = new List<Classes>();
            foreach (Classes classes in list)
            {
                if (classes.ID == c.ID)
                {
                    classes.Color = s;
                }
                newList.Add(classes);
            }
            SaveClasses(newList);
        }

        public static SolidColorBrush GetClassColor(Entry e)
        {
            List<Classes> list = LoadClasses();
            foreach (Classes c in list)
            {
                if (c.ID == e.ForClass.ID)
                {
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString(c.Color));
                }
            }
            return new SolidColorBrush(Colors.Transparent);
        }

        static int newNumForClass = 0;
        public static int GenerateNumForNewClass()
        {
            return newNumForClass++;
        }

        public static void SaveClasses(List<Classes> classes)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, classes);
                ms.Position = 0;
                byte[] buffer = new byte[(int)ms.Length];
                ms.Read(buffer, 0, buffer.Length);
                Properties.Settings.Default.ClassList = Convert.ToBase64String(buffer);
                Properties.Settings.Default.Save();
            }
        }




        /* DATES */
        public static int NumMonths(Entry x, DateTime day)
        {
            return (((day.Year - x.AssignToDate.Year) * 12) + day.Month - x.AssignToDate.Month);
        }

        public static int NumYears(Entry x, DateTime day)
        {
            return day.Year - x.AssignToDate.Year;
        }




        /* COUNTING */
        public static int NumEntries()
        {
            return entries.Count;
        }




        /* MENUS & WINDOWS */
        public static void Syllabus(Window window)
        {
            Syllabus_Library syllabus = new Syllabus_Library();
            syllabus.Owner = window;
            syllabus.Show();
            syllabus.Owner = null;
        }
        internal static void AboutWindow(object sender, RoutedEventArgs e, Window window)
        {
            AboutBox1 about = new AboutBox1();
            about.Show();
        }

        internal static void ReadMe(object sender, RoutedEventArgs e, Window window)
        {
            ReadMe read = new ReadMe();
            read.Owner = window;
            read.Show();
            read.Owner = null;
        }

        internal static void CalendarClick(object sender, RoutedEventArgs e, Window window)
        {
            Calendar_Popup cal = new Calendar_Popup();
            cal.Owner = window;
            cal.Show();
            cal.Owner = null;
            window.Owner = cal;
        }

        public static void NewList()
        {
            if (IsModified())
            {
                DialogResult dialogResult = DialogResult.Yes;
                if (Properties.Settings.Default.AutoSave == false)
                    dialogResult = System.Windows.Forms.MessageBox.Show("Would you like to save your changes before creating the new list?", "There are Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

                if (dialogResult == DialogResult.Yes)
                {
                    Save(Properties.Settings.Default.DefaultFile);
                    System.Windows.MessageBox.Show("Saved. Now Select Your New File:");
                    entries = new List<Entry>();
                    currentFilePath = "";
                    Save();
                    LoadToDoList(currentFilePath);
                }
                else if (dialogResult == DialogResult.No)
                {
                    entries = new List<Entry>();
                    SaveAs();
                    LoadToDoList(currentFilePath);
                }
                //else cancel
            }
            else
            {
                entries = new List<Entry>();
                SaveAs();
                LoadToDoList(currentFilePath);
            }
        }

        public static void OpenAddItemWindow(Window window)
        {
            AddItem add = new AddItem();
            add.Owner = window;
            add.Show();
            add.Owner = null;
        }

        public static void SaveAs()
        {
            currentFilePath = "";
            Save();
        }

        public static void Save()
        {
            if (currentFilePath == "")
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();

                // http://msdn.microsoft.com/en-us/library/system.io.path.getdirectoryname.aspx


                saveFileDialog.DefaultExt = ".todolist"; // Default file extension
                saveFileDialog.Filter = "ToDoList (.todolist)|*.todolist"; // Filter files by extension
                // Show save file dialog box
                if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Properties.Settings.Default.DefaultFile = saveFileDialog.FileName;
                    Properties.Settings.Default.Save();
                    Save(saveFileDialog.FileName);
                }
            }
            else
            {
                Save(currentFilePath);
            }
        }

        public static void Save(string filePath)
        {
            try
            {
                using (var fStream = new FileStream(filePath, FileMode.Create))
                {
                    var binFormatter = new BinaryFormatter();

                    binFormatter.Serialize(fStream, entries);
                    fStream.Close();

                    currentFilePath = filePath;
                    SetModified(false);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }

        }

        public static void LoadToDoList()
        {
            if (IsModified())
            {
                DialogResult dialogResult = DialogResult.Yes;
                if (Properties.Settings.Default.AutoSave == false)
                    dialogResult = System.Windows.Forms.MessageBox.Show("Would you like to save your changes before creating the new list?", "There are Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

                if (dialogResult == DialogResult.Yes)
                    Save(Properties.Settings.Default.DefaultFile);
                else if (dialogResult == DialogResult.Cancel)
                    return;
            }

            if (currentFilePath != "")
            {
                LoadToDoList(currentFilePath);
            }
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "ToDoList (.todolist)|*.todolist"; // Filter files by extension

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LoadToDoList(openFileDialog.FileName);
                Properties.Settings.Default.DefaultPath = Path.GetDirectoryName(openFileDialog.FileName);
                Properties.Settings.Default.DefaultFile = openFileDialog.FileName;
                currentFilePath = Properties.Settings.Default.DefaultFile;
                Properties.Settings.Default.Save();
            }
        }

        public static void LoadToDoList(String filePath)
        {
            entries.Clear();
            if (filePath == "")
            {
                try
                {
                    LoadToDoList();
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            }
            else
            {
                try
                {
                    var fStream = new System.IO.FileStream(filePath, FileMode.Open);
                    currentFilePath = fStream.Name;
                    var binFormatter = new BinaryFormatter();
                    List<Entry> items = (List<Entry>)binFormatter.Deserialize(fStream);
                    fStream.Close();

                    foreach (Entry e in items)
                    {
                        entries.Add(e);
                    }
                }
                catch (FileNotFoundException e)
                {
                    System.Windows.MessageBox.Show($"{e.Message}\n\nYour Default File Does Not Exist. Please Generate a New File");
                    LoadToDoList();
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                    NewList();
                }


            }

        }

        public static void NewClick(Window window)
        {
            HandleData.NewList();
            MainWindow main = new MainWindow();
            main.Owner = window;
            main.Show();
            main.Owner = null;
            window.Close();
            HandleData.OpenAddItemWindow(main);
        }

        public static void OpenSettings(Window window)
        {
            Settings settings = new Settings();
            settings.Owner = window;
            settings.Show();
            settings.Owner = null;
        }

        public static void Exit()
        {
            if (Properties.Settings.Default.DefaultFile == null || Properties.Settings.Default.DefaultFile == "")
            {
                System.Windows.Application.Current.Shutdown();
            }
            else if (Properties.Settings.Default.AutoSave == true)
            {
                Save(Properties.Settings.Default.DefaultFile);
                System.Windows.Application.Current.Shutdown();
            }
            else
            {
                if (IsModified())
                {
                    MessageBoxResult result = System.Windows.MessageBox.Show("There are unsaved changes. Would you like to save?", "Unsaved Changes", MessageBoxButton.YesNoCancel);
                    if (result == MessageBoxResult.Yes)
                    {
                        Save(Properties.Settings.Default.DefaultFile);
                        System.Windows.Application.Current.Shutdown();
                    }
                    else if (result == MessageBoxResult.No)
                    {
                        System.Windows.Application.Current.Shutdown();
                    }
                }
                else
                {
                    System.Windows.Application.Current.Shutdown();
                }
            }
        }



        /* SETTINGS */
        public static bool StoreDeleted()
        {
            return (Properties.Settings.Default.StoreDeleted == true) ? true : false;
        }

        public static String GetFilePath()
        {
            return Properties.Settings.Default.DefaultFile;
        }

        public static String GetCurrentFilePath()
        {
            return currentFilePath;
        }

        public static void SetFilePath(String s)
        {
            currentFilePath = s;
            Properties.Settings.Default.DefaultFile = s;
        }

        public static void CheckForFile()
        {
            if ((Properties.Settings.Default.AutoLoad == true) && ((Properties.Settings.Default.DefaultFile == null) || (Properties.Settings.Default.DefaultFile == "")))
            {
                MessageBoxResult result = HandleData.CustomMessageBox("Auto-Load is enabled, but there is no file found. Please Open or Create a new file, or disable Auto-Load", "No file found", "Open List", "New List", "Disable Auto-Load", MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation);
                if (result == MessageBoxResult.Yes)
                {
                    //open file
                    HandleData.LoadToDoList();
                }
                else if (result == MessageBoxResult.No)
                {
                    //new file
                    HandleData.NewList();
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    //disable auto-load
                    Properties.Settings.Default.AutoLoad = false;
                }
            }
        }

        public static void AutoSave(object sender, System.ComponentModel.CancelEventArgs e, Window window)
        {
            if (IsModified())
            {
                if (Properties.Settings.Default.AutoSave == true)
                {
                    if (HandleData.GetCurrentFilePath() == "")
                    {
                        MessageBoxResult result = System.Windows.MessageBox.Show(window, "You have enabled Auto-Save, but there is no file found. Would you like to save your changes?", "File not found", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel);
                        if (result == MessageBoxResult.Yes)
                        {
                            HandleData.Save(HandleData.GetCurrentFilePath());
                        }
                        else if (result == MessageBoxResult.Cancel)
                        {
                            e.Cancel = true;
                        }
                    }
                    else
                    {
                        //System.Windows.MessageBox.Show($"{entries.Count}");
                        HandleData.Save(HandleData.GetCurrentFilePath());
                        //System.Windows.MessageBox.Show($"{entries.Count}");
                    }
                }
            }
        }




        /* CUSTOM FORM */
        public static MessageBoxResult CustomMessageBox(String Text, String Caption, String YesButtonText, String NoButtonText, String CancelButtonText, MessageBoxButton Button, MessageBoxImage Image)
        {
            System.Windows.Style style = new System.Windows.Style();
            style.Setters.Add(new Setter(Xceed.Wpf.Toolkit.MessageBox.YesButtonContentProperty, YesButtonText));
            style.Setters.Add(new Setter(Xceed.Wpf.Toolkit.MessageBox.NoButtonContentProperty, NoButtonText));
            style.Setters.Add(new Setter(Xceed.Wpf.Toolkit.MessageBox.CancelButtonContentProperty, CancelButtonText));
            return  Xceed.Wpf.Toolkit.MessageBox.Show(Text, Caption, Button, Image, MessageBoxResult.Yes, style);
        }




        /* MODIFIED? */
        public static Boolean SetModified(bool i)
        {
            //0 true
            if (i == true)
            {
                modified = true;
                return true;
            }
            //else false
            modified = false;
            return false;
        }
        public static Boolean IsModified()
        {
            if (modified) return true;
            else return false;
        }

    }
}
