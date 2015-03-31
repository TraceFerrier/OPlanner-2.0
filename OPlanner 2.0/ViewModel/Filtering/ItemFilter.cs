using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlannerNameSpace.ViewModel.Filtering;
using System.Windows.Data;

namespace PlannerNameSpace.ViewModel.Filtering
{
    public class ItemFilter
    {
        private string m_filterProperty;
        private string m_filterPropertyFriendlyName;
        private FilterValueCollection m_filterPropertyValues;
        public ICollectionView PropertyValuesView { get; private set; }

        public ItemFilter(string filterProperty, string filterPropertyFriendlyName)
        {
            m_filterProperty = filterProperty;
            m_filterPropertyFriendlyName = filterPropertyFriendlyName;
            m_filterPropertyValues = new FilterValueCollection();
            PropertyValuesView = CollectionViewSource.GetDefaultView(FilterPropertyValues);
            SortUtils.SetCustomSorting(PropertyValuesView, new ItemPropertySort<StoreItem>("Title"), ListSortDirection.Ascending);
        }

        public string FilterPropertyFriendlyName
        {
            get { return m_filterPropertyFriendlyName; }
            set { m_filterPropertyFriendlyName = value; }
        }

        public string FilterProperty
        {
            get { return m_filterProperty; }
            set { m_filterProperty = value; }
        }

        public void AddPropertyValue(object value)
        {
            m_filterPropertyValues.AddValue(value);
        }

        public AsyncObservableCollection<FilterValue> FilterPropertyValues
        {
            get { return m_filterPropertyValues.FilterPropertyValues; }
        }
    }
}
