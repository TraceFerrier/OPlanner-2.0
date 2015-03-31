using PlannerNameSpace.ViewModel.Filtering;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace PlannerNameSpace.ViewModel
{

    public class ScenarioSummaryViewModel : BaseItemViewModel<ExperienceItem>
    {
        public ScenarioSummaryViewModel()
        {
            ItemsView = CollectionViewSource.GetDefaultView(ExperienceItem.Items);
        }

        public override AsyncObservableCollection<ItemFilter> ItemFilters
        {
            get { return ExperienceItem.ScenarioFilterCollection.ItemFilters; }
        }

        public override void CreateItem()
        {
            ExperienceItem newItem = ScheduleStore.Instance.CreateStoreItem<ExperienceItem>(ItemTypeID.Experience);

            newItem.BeginSaveImmediate();
            newItem.BusinessRank = 9999;
            newItem.Title = "New Experience";
            newItem.SaveImmediate();
        }

        public void AssignScenarioFeatures()
        {
        }

        public bool CanAssignScenarioFeatures(object parameter)
        {
            return true;
        }

        public void DeleteBacklogItem()
        {
        }

        public bool CanDeleteBacklogItem(object parameter)
        {
            return SelectedBacklogItem != null;
        }

        BacklogItem SelectedBacklogItem
        {
            get
            {
                if (SelectedItem != null)
                {
                    return SelectedItem.SelectedBacklogItem;
                }

                return null;
            }
        }

    }
}
