using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlannerNameSpace.ViewModel.Filtering;
using System.Windows.Data;

namespace PlannerNameSpace
{
    public static class SortUtils
    {
        public static void SetCustomSorting(ICollectionView collectionView, BaseSort comparer, ListSortDirection direction)
        {
            ListCollectionView sortView = (ListCollectionView)collectionView;
            comparer.SetSortDirection(direction);
            sortView.CustomSort = comparer;
        }

    }
}
