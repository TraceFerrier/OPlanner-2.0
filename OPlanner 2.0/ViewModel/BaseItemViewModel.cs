using PlannerNameSpace.ViewModel.Filtering;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace PlannerNameSpace.ViewModel
{
    public abstract class BaseItemViewModel<T> : BasePropertyChanged where T : StoreItem
    {
        private ICollectionView m_itemsView;

        public ICollectionView ItemsView
        {
            get 
            { 
                return m_itemsView; 
            }

            set 
            { 
                m_itemsView = value;
                m_itemsView.Filter = new Predicate<object>(ShouldAcceptItem);
            }
        }

        public abstract AsyncObservableCollection<ItemFilter> ItemFilters { get; }

        private AsyncObservableCollection<BaseSort> m_sortingCriteria;
        public virtual AsyncObservableCollection<BaseSort> SortingCriteria
        {
            get
            {
                if (m_sortingCriteria == null)
                {
                    m_sortingCriteria = new AsyncObservableCollection<BaseSort>();
                    m_sortingCriteria.Add(new BusinessRankSort());
                    m_sortingCriteria.Add(new ItemPropertySort<BacklogItem>(StringUtils.GetPropertyName((BacklogItem b) => b.Title)));
                }

                return m_sortingCriteria;
            }
        }

        private BaseSort m_selectedSortCriteria;
        public virtual BaseSort SelectedSortCriteria
        {
            get
            {
                if (m_selectedSortCriteria == null)
                {
                    m_selectedSortCriteria = SortingCriteria[0];
                    SortUtils.SetCustomSorting(ItemsView, m_selectedSortCriteria, ListSortDirection.Ascending);
                }

                return m_selectedSortCriteria;
            }

            set
            {
                m_selectedSortCriteria = value;
                SortUtils.SetCustomSorting(ItemsView, m_selectedSortCriteria, ListSortDirection.Ascending);
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Filter predicate for the ViewModel - all derived class items will be run
        /// through this filter, subject to the current user selections in the ItemFilters
        /// set.
        /// </summary>
        //------------------------------------------------------------------------------------
        public bool ShouldAcceptItem(object item)
        {
            T viewModelItem = item as T;
            if (viewModelItem != null)
            {
                return viewModelItem.ShouldAcceptFilterableItem();
            }

            return false;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// The currently selected item in the backlog list box.
        /// </summary>
        //------------------------------------------------------------------------------------
        private T m_selectedItem;
        public T SelectedItem
        {
            get
            {
                if (m_selectedItem == null)
                {
                }

                return m_selectedItem;
            }

            set
            {
                m_selectedItem = value;
                NotifyPropertyChangedByName();
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// FilterSelectionsChanged command
        /// </summary>
        //------------------------------------------------------------------------------------
        public virtual void FilterSelectionsChanged()
        {
            ItemsView.Refresh();
        }

        public virtual bool CanFilterSelectionsChange(object parameter)
        {
            return true;
        }

        public abstract void CreateItem();

        public virtual bool CanCreateItem(object parameter)
        {
            return true;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// DeleteItem command
        /// </summary>
        //------------------------------------------------------------------------------------
        public virtual void DeleteItem()
        {
            if (SelectedItem != null)
            {
                SelectedItem.DeleteItem();
            }
        }

        public virtual bool CanDeleteItem(object parameter)
        {
            return SelectedItem != null;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// RefreshView command
        /// </summary>
        //------------------------------------------------------------------------------------
        public virtual void RefreshView()
        {
            ItemsView.Refresh();
        }

        public bool CanRefreshView(object parameter)
        {
            return true;
        }


    }
}
