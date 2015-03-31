using System.Windows;
using PlannerNameSpace.Model;

namespace PlannerNameSpace
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        //static App()
        //{
        //    // Help perf with modern UI
        //    System.Windows.Media.Animation.Timeline.DesiredFrameRateProperty.OverrideMetadata(typeof(System.Windows.Media.Animation.Timeline),
        //        new FrameworkPropertyMetadata { DefaultValue = 10 });
        //}

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Planner.Instance.Startup(this);
            //MainWindow window = new MainWindow();
            //PlannerSplash splashWindow = new PlannerSplash();
            //splashWindow.Style = null;// App.Current.Resources["EmptyWindow"] as Style;
            //splashWindow.ContentSource = new Uri("/View/Content/PlannerSplashContent.xaml", UriKind.Relative);
            //splashWindow.Show();
            //window.Style = App.Current.Resources["EmptyWindow"] as Style;

            //window.Style = null;
            //window.ContentSource = new Uri("/View/Content/About.xaml", UriKind.Relative);
            //window.Show();
        }
    }
}
