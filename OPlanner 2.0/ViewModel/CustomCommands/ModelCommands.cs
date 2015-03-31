using System;
using System.Windows.Input;

namespace PlannerNameSpace.ViewModel.CustomCommands
{
    public static class ModelCommands
    {
        private static readonly RoutedCommand _saveChanges = new RoutedCommand("SaveChanges", typeof(ModelCommands));
        public static RoutedCommand SaveChanges { get { return _saveChanges; } }

        private static readonly RoutedCommand _refresh = new RoutedCommand("Refresh", typeof(ModelCommands));
        public static RoutedCommand Refresh { get { return _refresh; } }

        private static readonly RoutedCommand _undoChanges = new RoutedCommand("UndoChanges", typeof(ModelCommands));
        public static RoutedCommand UndoChanges { get { return _undoChanges; } }

        private static readonly RoutedCommand _showChangeListViewer = new RoutedCommand("ShowChangeListViewer", typeof(ModelCommands));
        public static RoutedCommand ShowChangeListViewer { get { return _showChangeListViewer; } }

    }
}
