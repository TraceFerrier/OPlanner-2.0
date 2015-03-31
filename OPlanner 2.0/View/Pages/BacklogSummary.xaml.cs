using FirstFloor.ModernUI.Windows;
using FirstFloor.ModernUI.Windows.Navigation;
using PlannerNameSpace.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace PlannerNameSpace.View.Pages
{
    /// <summary>
    /// Interaction logic for BacklogDetails.xaml
    /// </summary>
    public partial class BacklogSummary : UserControl, IContent
    {
        public BacklogSummary()
        {
            InitializeComponent();
            Loaded += BacklogDetails_Loaded;
        }

        void BacklogDetails_Loaded(object sender, RoutedEventArgs e)
        {
            BacklogItemsListBox.SelectedIndex = 0;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// This will be called when the user navigates to the same LinkGroup.Links.Link
        /// with a different parameter (after the # at the end of the uri).
        /// </summary>
        //------------------------------------------------------------------------------------
        public void OnFragmentNavigation(FragmentNavigationEventArgs e)
        {
            BacklogDetailsContentControl.ContentTemplate = (DataTemplate)Application.Current.FindResource(e.Fragment);
        }

        public void OnNavigatedFrom(NavigationEventArgs e)
        {
        }

        public void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        public void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
        }

    }
}
