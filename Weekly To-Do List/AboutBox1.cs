using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Weekly_ToDo_List
{
    partial class AboutBox1 : Form
    {
        public AboutBox1()
        {
            InitializeComponent();
            this.Text = String.Format("About {0}", "Weekly To-Do List");
            this.labelProductName.Text = "Weekly To-Do List";
            this.labelVersion.Text = String.Format("Version {0}", "1.0");
            this.labelCopyright.Text = "Copyright (c) Evan Jacobson 2018";
            this.labelCompanyName.Text = "JacobsonEvan";
            this.textBoxDescription.Text = "Release of Weekly To-Do List";
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
