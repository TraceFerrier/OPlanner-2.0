using PlannerNameSpace.ViewModel.Filtering;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace PlannerNameSpace.ViewModel
{
    public class BacklogSummaryViewModel : BaseItemViewModel<BacklogItem>
    {

        public BacklogSummaryViewModel()
        {
            ItemsView = CollectionViewSource.GetDefaultView(BacklogItem.Items);
        }

        public override AsyncObservableCollection<ItemFilter> ItemFilters
        {
            get { return BacklogItem.BacklogFilterCollection.ItemFilters; }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// CreateItem command
        /// </summary>
        //------------------------------------------------------------------------------------
        public override void CreateItem()
        {
            // Make sure we've got a selected item in the view, so we'll have a
            // default mentor item (if the user doesn't already have an item selected).
            if (ItemsView.CurrentItem == null)
            {
                ItemsView.MoveCurrentToFirst();
            }

            BacklogItem mentorBacklogItem = ItemsView.CurrentItem as BacklogItem;
            BacklogItem newItem = HostItemStore.Instance.CreateStoreItem<BacklogItem>(ItemTypeID.BacklogItem);

            newItem.BeginSaveImmediate();
            newItem.ShipCycle = Constants.OfficeCurrentShipCycle;
            newItem.BusinessRank = 9999;
            newItem.Title = "New Backlog Item";

            // The mentor item is the item the user had selected when electing to create a new item.
            // We'll base the new item's basic properties on that of the mentor item.
            if (mentorBacklogItem != null)
            {
                newItem.ShipCycle = mentorBacklogItem.ShipCycle;
                newItem.ParentPillarItem = mentorBacklogItem.ParentPillarItem;
                newItem.ParentTrainItem = mentorBacklogItem.ParentTrainItem;
            }

            newItem.SaveImmediate();
            ItemsView.MoveCurrentTo(newItem);
        }

        public bool CanCreateBacklogItem(object parameter)
        {
            return true;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// CreateWorkItem command
        /// </summary>
        //------------------------------------------------------------------------------------
        public void CreateWorkItem()
        {
            if (SelectedItem != null)
            {
                WorkItem newWorkItem = WorkItem.CreateWorkItem(SelectedItem);
                newWorkItem.SaveNewItem();
                SelectedItem.WorkItemsView.MoveCurrentTo(newWorkItem);
            }
        }

        public bool CanCreateWorkItem()
        {
            return ItemsView.CurrentItem != null;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// DeleteWorkItem command
        /// </summary>
        //------------------------------------------------------------------------------------
        public void DeleteWorkItem()
        {
            if (SelectedWorkItem != null)
            {
                SelectedWorkItem.DeleteItem();
            }
        }

        public bool CanDeleteWorkItem()
        {
            return SelectedWorkItem != null;
        }

        WorkItem SelectedWorkItem
        {
            get
            {
                if (SelectedItem != null)
                {
                    return SelectedItem.SelectedWorkItem;
                }

                return null;
            }
        }

    }
}
