using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlannerNameSpace.ViewModel.Filtering;
using System.Reflection;

namespace PlannerNameSpace
{
    public partial class StoreItem
    {
        public virtual AsyncObservableCollection<ItemFilter> Filters
        {
            get { return null; }
        }

        protected virtual void MaintainFilters(ChangeType changeType)
        {
            if (Filters != null)
            {
                switch (changeType)
                {
                    case ChangeType.Added:
                    case ChangeType.Updated:
                        foreach (ItemFilter filter in Filters)
                        {
                            object value = GetPropertyValue(this, filter.FilterProperty);
                            if (value != null)
                            {
                                filter.AddPropertyValue(value);
                            }
                            else
                            {
                                filter.AddPropertyValue(GetDummyItem<BacklogItem>(DummyItemType.NoneType));
                            }
                        }
                        break;
                }
            }
        }

        protected static object GetPropertyValue<T>(T item, string publicPropertyName)
        {
            PropertyInfo prop = item.GetType().GetProperty(publicPropertyName);
            if (prop != null)
            {
                return prop.GetValue(item);
            }

            return null;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Given an ItemFilter, returns true if the given item should be accepted.
        /// </summary>
        //------------------------------------------------------------------------------------
        protected static bool ShouldAcceptFilterableItem(ItemFilter itemFilter, StoreItem itemToFilter)
        {
            // If the given itemToFilter property value matches any of the specified 'active' values
            // in the given filter, OR none of the values are active (indicating that the user hasn't
            // specified that filtering should be performed for this filter), then accept the item.
            bool shouldAccept = false;
            int activeValueCount = 0;
            object itemValue = GetPropertyValue(itemToFilter, itemFilter.FilterProperty);
            foreach (FilterValue value in itemFilter.FilterPropertyValues)
            {
                if (value.IsActive)
                {
                    activeValueCount++;
                    StoreItem filterValueAsStoreItem = value.Value as StoreItem;
                    if (filterValueAsStoreItem != null)
                    {
                        if (filterValueAsStoreItem == itemValue)
                        {
                            shouldAccept = true;
                            break;
                        }
                    }
                    else
                    {
                        if (StringUtils.StringsMatch(value.Value as string, itemValue as string))
                        {
                            shouldAccept = true;
                            break;
                        }
                    }
                }
            }

            if (activeValueCount == 0)
            {
                shouldAccept = true;
            }

            return shouldAccept;
        }

        public bool ShouldAcceptFilterableItem()
        {
            // Every filter must accept the item for the item to be accepted.
            foreach (ItemFilter itemFilter in Filters)
            {
                if (!ShouldAcceptFilterableItem(itemFilter, this))
                {
                    return false;
                }
            }

            return true;
        }
    }

    public partial class ExperienceItem
    {
        public override AsyncObservableCollection<ItemFilter> Filters
        {
            get
            {
                return ScenarioFilterCollection.ItemFilters;
            }
        }

        public static ItemFilterCollection ScenarioFilterCollection;

        public static void InitializeFilters()
        {
            if (ScenarioFilterCollection == null)
            {
                ScenarioFilterCollection = new ItemFilterCollection();
                //m_scenarioFilters.AddFilter(new ItemFilter(StringUtils.GetPropertyName((ScenarioItem s) => s.ParentPillarItem), Constants.PillarFriendlyName));
                //m_scenarioFilters.AddFilter(new ItemFilter(StringUtils.GetPropertyName((ScenarioItem s) => s.ParentTrainItem), Constants.TrainFriendlyName));
            }
        }
    }

    public partial class BacklogItem
    {
        public override AsyncObservableCollection<ItemFilter> Filters
        {
            get
            {
                return BacklogFilterCollection.ItemFilters;
            }
        }

        public static ItemFilterCollection BacklogFilterCollection;
        public static void InitializeFilters()
        {
            if (BacklogFilterCollection == null)
            {
                BacklogFilterCollection = new ItemFilterCollection();
                BacklogFilterCollection.AddFilter(new ItemFilter(StringUtils.GetPropertyName((BacklogItem b) => b.ParentPillarItem), Constants.PillarFriendlyName));
                BacklogFilterCollection.AddFilter(new ItemFilter(StringUtils.GetPropertyName((BacklogItem b) => b.ParentTrainItem), Constants.TrainFriendlyName));
            }
        }

    }

}
