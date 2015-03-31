using System;
using System.Windows.Input;

namespace PlannerNameSpace.ViewModel.CustomCommands
{
    public static class FilterCommands
    {
        private static readonly RoutedCommand _filterSelectionChanged = new RoutedCommand("FilterSelectionChanged", typeof(FilterCommands));

        public static RoutedCommand FilterSelectionChanged { get { return _filterSelectionChanged; } }
    }
}
