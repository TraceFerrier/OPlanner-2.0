using System.Reflection;

namespace PlannerNameSpace
{
    internal static class PlannerOld
    {
        public static MyEventManager EventManager;
        public static ProductTreeManager ProductTreeManager;

        private static MainWindow g_mainWindow;

        public static MainWindow MainWindow
        {
            get { return g_mainWindow; }
            set { g_mainWindow = value; }
        }

        public static void Startup()
        {
            EventManager = new MyEventManager();
            ProductTreeManager = new ProductTreeManager();
        }

    }
}
