using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerNameSpace.ViewModel.Filtering
{
    class FilterValueCollection
    {
        private AsyncObservableCollection<FilterValue> m_filterPropertyValues;

        public FilterValueCollection()
        {
            m_filterPropertyValues = new AsyncObservableCollection<FilterValue>();
        }

        public bool Contains(object value)
        {
            foreach (FilterValue filterValue in m_filterPropertyValues)
            {
                if (filterValue.Value == value)
                {
                    return true;
                }
            }

            return false;
        }

        public void AddValue(object value)
        {
            if (!Contains(value))
            {
                m_filterPropertyValues.Add(new FilterValue(value));
            }
        }

        public AsyncObservableCollection<FilterValue> FilterPropertyValues
        {
            get { return m_filterPropertyValues; }
        }

    }
}
