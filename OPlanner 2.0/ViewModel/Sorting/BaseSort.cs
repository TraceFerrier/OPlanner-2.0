using System;
using System.Collections;
using System.ComponentModel;

namespace PlannerNameSpace
{
    public abstract class BaseSort : IComparer
    {
        protected Nullable<ListSortDirection> m_direction;
        public void SetSortDirection(ListSortDirection direction)
        {
            m_direction = direction;
        }

        public virtual int Compare(object x, object y)
        {
            return 0;
        }

        protected string m_sortTitle;

        public virtual string SortTitle
        {
            get { return m_sortTitle; }
            set { m_sortTitle = value; }
        }

        public override string ToString()
        {
            return SortTitle;
        }
    }
}
