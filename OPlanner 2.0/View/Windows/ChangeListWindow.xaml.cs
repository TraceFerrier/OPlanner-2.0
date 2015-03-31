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

namespace PlannerNameSpace
{
    /// <summary>
    /// Interaction logic for ChangeListWindow.xaml
    /// </summary>
    public partial class ChangeListWindow : Window
    {
        AsyncObservableCollection<StoreItem> ChangeList;

        public ChangeListWindow()
        {
            InitializeComponent();

            this.Owner = Planner.Instance.MainWindow;
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;

            ChangeList = Planner.Instance.ItemRepository.ChangeList;
            ChangeListItemsControl.ItemsSource = ChangeList;
            ChangeList.CollectionChanged += ChangeList_CollectionChanged;
            UpdateChangeUI();
        }

        void ChangeList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateChangeUI();
        }

        void UpdateChangeUI()
        {
            ChangeCountBox.Text = "Changes to save: " + ChangeList.Count;

            if (ChangeList.Count > 0)
            {
                TitlePanel.Background = Brushes.Red;
            }
            else
            {
                TitlePanel.Background = Brushes.Green;
            }
        }
    }
}
