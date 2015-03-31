using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PlannerNameSpace.Assets
{
    public partial class ListBoxStylesResourceDictionary : ResourceDictionary
    {
        public ListBoxStylesResourceDictionary()
        {
            InitializeComponent();
        }

        public void SelectCurrentItem(object sender, KeyboardFocusChangedEventArgs e)
        {
            ListBoxItem item = (ListBoxItem)sender;
            item.IsSelected = true;
        }
    }
}
