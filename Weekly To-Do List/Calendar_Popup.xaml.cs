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
    /// Interaction logic for Calendar_Popup.xaml
    /// </summary>
    public partial class Calendar_Popup : Window
    {
        public Calendar_Popup()
        {
            InitializeComponent();
        }

        private void Calendar_Item_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Calendar_Item.SelectedDate.HasValue)
            {
                Today today = new Today(Calendar_Item.SelectedDate.Value);
                today.Owner = this.Owner;
                today.Show();
                today.Owner = null;
                Close();

            }
            
        }
    }
}
