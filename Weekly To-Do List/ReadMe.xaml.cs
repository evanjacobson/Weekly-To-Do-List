﻿using System;
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
    /// Interaction logic for ReadMe.xaml
    /// </summary>
    public partial class ReadMe : Window
    {
        public ReadMe()
        {
            InitializeComponent();

            Text.Text = "Evan Jacobson --- Weekly To-Do List --- Final Project\n\n\n\n__________________________________________Background__________________________________________\n This project started off as an attempt to create a weekly to-do list, hence its name.However, I ended up building a program\nwhich can handle as many weeks of data as you would like to add.The program has 3 main features: a \"daily\" to-do list, a slot machine,\nand a weekly to-do list.The daily and weekly to-do lists are implemented with basically the same code(I worked hard to implement)\nthe daily to-do list in a less-resource-intensive way but it took too long and I ran out of time). The program also features color-coding\nof classes, repeating activities, a clickable calendar, a syllabus library, and several helpful settings(auto-save and auto-load,\nthe choice between file-size optimization and record-keeping). \n\n\n\n\n__________________________________________DEFAULT Settings__________________________________________\n- Auto-Save is enabled by default: any changes are automatically saved\n- Auto-Load is enabled by default: the program will automatically load your most recent file upon opening the program\n- Storage of deleted entries is disabled by default: the program will remove deleted, non-repeating entries, as well as repeating entries that have\n\"all instances\" deleted.You can toggle this option, but if you choose to disable it, it will immediately get rid of any deleted entries.If enabled,\nyou have the option to see deleted entries on the weekly and daily lists.\n- you can add and remove classes, as well as edit the colors for classes. \n- the \"misc\" class is always on the list by default, and can not be removed. this prevents certain errors from occurring throughout the program, \nwhere it is dependent on a class instance.\n- the default file can be chosen in settings with the OpenFileDialog, but it will be overridden when you open or create a new list\n- you can purge all entries(completed and deleted) to make the file smaller and have the program run more efficiently\n\n\n\n\n__________________________________________How to Use \"Weekly To-Do List\"__________________________________________\n\n************************* File Management: A file is always required when using the application*************************\nWhen you open the application for the first time, it will give you a popup reading \"Auto-Load is enabled, but there is no file found. \nPlease Open or Create a new file, or disable Auto-Load.\" This ensures that when you add to the list, it is actually able to save. \nThis message box will also be shown on any other window if there is no file loaded. \n\n\n\n************************* Menu Items: Global*************************\n\n--------File--------\n\"New List\"\n    ---- clicking \"New List\" will either auto-save (if enabled) or give you the option to save before generating and loading a new list file via \n SaveFileDialog.The new list will also be automatically added into the default settings.If auto-load is enabled, it will be the new default list\n\"Open List\"\n    ---- clicking \"Open list\" will either auto-save(if enabled) or give you the option to save before opening the OpenFileDialog, loading that list,\n and making that file the new default file\n\"Save\"\n    ---- \"Save\" will save the file without opening a SaveFileDialog, if the option is available\n\"Save As\"\n    ---- \"SaveAs\" will open a SaveFileDialog for your current file\n\"Exit\"\n    ---- \"Exit\" will either auto-save(if enabled) or give you the option to save before terminating the program\n\n--------List--------\n\"Add Items\"\n    ---- \"Add Items\" allows you to create new to-do list items.The assignment name cannot be more than 16 characters.This allows a better viewing \n experience for the slot machine.In this screen, you can set the assignment name, choose the class, add the estimated time to complete the task,\n set the \"Do\" date(date to complete it) and the \"due\" date(date it\\'s due). You can also choose to have the item be a repeating event, and add notes. \n\n\"Settings\"\n    ---- \"Settings\" gives you the options to change various main features of the program. You can add classes + their color, edit class colors, remove classes,\n    choose a default file, and toggle auto-load, storage of deleted items, and auto-save. \n\n--------View--------\n\"Calendar\"\n    ---- \"Calendar\" opens up a calendar where you can select a day and it will automatically open up that day\'s To-Do List\n\"Syllabus Library\"\n    ---- \"Syllabus Library\" opens up a window where you can add, remove, and open files.It stores only the local paths to the files so it has a very minimal \n additional storage cost.\n\n--------Help--------\n\"Read Me\"\n    ---- Opens this file\n\"About\"\n    ---- Opens an \"about\" window with information on the version, creator, etc.\n\n\n\n************************* Additional Menu Items on the List Screens *************************\n\n--------Sort List--------\n\"Default\"\n    ---- Default sorting, based on the algorithm to determine which items go on which days\n\"Time To Complete - Ascending\"\n    ---- Uses a simple LINQ query to order the list based on time to complete, from smallest to largest\n\"Time To Complete - Descending\"\n    ---- Uses a simple LINQ query to order the list based on time to complete, from largest to smallest\n\n\n\n************************* The Home Screen *************************\n The Home screen is the hub for the program. It is where you can navigate between the different displays. One cool feature to note is that the\ndate on the \"Today\'s To-Do List\" is dynamic, and it will always show the current month and day!\n\n\n\n************************* The \"Today\" Screen*************************\n I spent a lot of time with the algorithms for populating the list items, and I ran out of time to properly develop the algorithms for this page,\nso I reused the algorithm from the \"Weekly To-Do List\" screen.With that being said, this screen does function differently. You can look at each\nday individually which is nice.Also, the \"Calendar\" menu item, when clicked, will open into this screen for whichever day you choose and populate\nits items. You can toggle the ability to see completed items, and if deleted-item-storage is enabled in Settings, you can also toggle viewing those.\nYou can click on any item and it will show its details, and give you the ability to mark it completed or deleted.You can also sort the list items by \ntime to complete.If you double click the label that shows the day, it will go back to the current date. \n\n\n\n************************* The \"Slot Machine\" Screen*************************\n The slot machine was my favorite part of the whole project to create. It uses 3 Listboxes and a timer to randomly choose 3 to-do items.All of the \nListBoxItem backgrounds are the colors that are chosen in the Settings screen for that class. If 2 or more are detected, you win the game and get to do \nthat item.The lever is a custom animation using two ellipses and several DoubleAnimations.It took a while to get it right but I really like it. \nAlso, I spent a while custom-editing and tweaking sound effects for the slot machine so make sure to have the sound on!\n\n\n\n************************* The \"Weekly To-Do List\" Screen*************************\nThis screen shows a weekly-view of the to-do list.You can toggle the ability to see completed items, and if deleted-item-storage is enabled in Settings, \nyou can also toggle viewing those. You can click on any item and it will show its details, and give you the ability to mark it completed or deleted.All of\nthe ListBoxItem backgrounds are the colors that are chosen in the Settings screen for that class. You can choose to view different weeks by clicking \"Next Week\"\nor \"Last Week\" as well.You can also sort the list items by time to complete.\n\n\n\n************************* Navigation Between Screens *************************\nThere is a \"back\" button on all of the main screens to ensure easy navigation.\n\n\n\n************************* How Are Items Stored and Managed?*************************\nThis was by far the most difficult part of the project.I spent literally 3 days on figuring out how to delete individual instances of repeating items, and probably over \n30 hours trying to solve this problem in 4 different ways. My goal was to optimize the algorithm for file size, because potentially indefinitely-repeating items would crash \nthe program. I ended up deciding on an algorithm in which it detects whether or not the item is repeating, and it then calculates the dates which the item would repeat on. \nFor deleting and completing repeating events, the solution was to store two List<DateTime> in each instance of a list entry, and then add each deleted date to the list, and check\nthe list when populating the events into their respective days. Ultimately, the algorithm was not very complicated, but it took me so long to figure out the right way to attack this\nproblem. \n\n\n\n\n__________________________________________Packages/Libraries Used__________________________________________\n- Extended.Wpf.Toolkit (Version 3.4.0) by Xceed\n--- Color Picker in List-->Settings";
        }
    }
}


