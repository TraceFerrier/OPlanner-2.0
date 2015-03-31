using System;
using System.Windows.Input;

namespace PlannerNameSpace.ViewModel.CustomCommands
{
    public static class ViewModelCommands
    {
        private static readonly RoutedCommand _createScenarioItem = new RoutedCommand("CreateScenarioItem", typeof(ViewModelCommands));
        public static RoutedCommand CreateScenarioItem { get { return _createScenarioItem; } }

        private static readonly RoutedCommand _deleteScenarioItem = new RoutedCommand("DeleteScenarioItem", typeof(ViewModelCommands));
        public static RoutedCommand DeleteScenarioItem { get { return _deleteScenarioItem; } }

        private static readonly RoutedCommand _createBacklogItem = new RoutedCommand("CreateBacklogItem", typeof(ViewModelCommands));
        public static RoutedCommand CreateBacklogItem { get { return _createBacklogItem; } }

        private static readonly RoutedCommand _deleteBacklogItem = new RoutedCommand("DeleteBacklogItem", typeof(ViewModelCommands));
        public static RoutedCommand DeleteBacklogItem { get { return _deleteBacklogItem; } }

        private static readonly RoutedCommand _createWorkItem = new RoutedCommand("CreateWorkItem", typeof(ViewModelCommands));
        public static RoutedCommand CreateWorkItem { get { return _createWorkItem; } }

        private static readonly RoutedCommand _deleteWorkItem = new RoutedCommand("DeleteWorkItem", typeof(ViewModelCommands));
        public static RoutedCommand DeleteWorkItem { get { return _deleteWorkItem; } }

        private static readonly RoutedCommand _refreshView = new RoutedCommand("RefreshView", typeof(ViewModelCommands));
        public static RoutedCommand RefreshView { get { return _refreshView; } }

        private static readonly RoutedCommand _assignScenarioFeatures = new RoutedCommand("AssignScenarioFeatures", typeof(ViewModelCommands));
        public static RoutedCommand AssignScenarioFeatures { get { return _assignScenarioFeatures; } }

    }
}
