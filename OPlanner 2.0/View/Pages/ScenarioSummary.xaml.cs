using FirstFloor.ModernUI.Windows;
using FirstFloor.ModernUI.Windows.Navigation;
using PlannerNameSpace.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace PlannerNameSpace.View.Pages
{
    /// <summary>
    /// Interaction logic for ScenarioSummary.xaml
    /// </summary>
    public partial class ScenarioSummary : UserControl, IContent
    {
        public ScenarioSummary()
        {
            InitializeComponent();
            Loaded += ScenarioSummary_Loaded;
        }

        void ScenarioSummary_Loaded(object sender, RoutedEventArgs e)
        {
            ScenarioItemsListBox.SelectedIndex = 0;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// This will be called when the user navigates to the same LinkGroup.Links.Link
        /// with a different parameter (after the # at the end of the uri).
        /// </summary>
        //------------------------------------------------------------------------------------
        public void OnFragmentNavigation(FragmentNavigationEventArgs e)
        {
            ScenarioContentControl.ContentTemplate = (DataTemplate)Application.Current.FindResource(e.Fragment);
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
