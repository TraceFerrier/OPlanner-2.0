using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerNameSpace
{
    public class FilterValue
    {
        private object m_value;
        private bool m_isActive;

        public FilterValue(object value)
        {
            m_value = value;
            m_isActive = false;
        }

        public object Value
        {
            get
            {
                return m_value;
            }
        }

        public string Title
        {
            get
            {
                if (m_value == null)
                {
                    return Constants.c_None;
                }
                else
                {
                    StoreItem item = m_value as StoreItem;
                    if (item != null)
                    {
                        return item.Title;
                    }
                    else
                    {
                        return m_value.ToString();
                    }
                }
            }
        }

        public bool IsActive
        {
            get { return m_isActive; }
            set { m_isActive = value; }
        }


    }
}
