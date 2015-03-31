using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace PlannerNameSpace.ViewModel.Filtering
{
    public class ItemFilterCollection
    {
        private AsyncObservableCollection<ItemFilter> m_itemFilters;

        public ItemFilterCollection()
        {
            m_itemFilters = new AsyncObservableCollection<ItemFilter>();
        }

        public AsyncObservableCollection<ItemFilter> ItemFilters
        {
            get { return m_itemFilters; }
            set { m_itemFilters = value; }
        }

        public void AddFilter(ItemFilter filter)
        {
            m_itemFilters.Add(filter);
        }

    }
}
