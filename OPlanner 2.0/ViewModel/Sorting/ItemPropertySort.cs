using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace PlannerNameSpace
{
    class ItemPropertySort<T> : BaseSort
    {
        PropertyInfo SortProp;
        public ItemPropertySort(string propName, ListSortDirection direction)
        {
            Init(propName, direction);
        }

        public ItemPropertySort(string propName)
        {
            Init(propName, ListSortDirection.Ascending);
        }

        void Init(string propName, ListSortDirection direction)
        {
            m_sortTitle = propName;
            m_direction = direction;
            SortProp = typeof(T).GetProperty(propName);
        }

        public override int Compare(object x, object y)
        {
            object compareX = x;
            object compareY = y;
            if (x is FilterValue && y is FilterValue)
            {
                FilterValue filterValueX = (FilterValue)x;
                FilterValue filterValueY = (FilterValue)y;
                compareX = filterValueX.Value;
                compareY = filterValueY.Value;
            }

            StoreItem itemX = (StoreItem)compareX;
            StoreItem itemY = (StoreItem)compareY;
            bool isXEarlier = false;

            if (itemX == itemY)
            {
                return 0;
            }
            else if (itemX.IsDummyItem && itemY.IsDummyItem)
            {
                isXEarlier = itemX.IsAllItem;
            }
            else if (itemX.IsDummyItem)
            {
                isXEarlier = true;
            }
            else if (itemY.IsDummyItem)
            {
                isXEarlier = false;
            }
            else
            {
                object xValue = SortProp.GetValue(itemX);
                object yValue = SortProp.GetValue(itemY);
                if (xValue is int && yValue is int)
                {
                    int intX = (int)xValue;
                    int intY = (int)yValue;
                    if (intX == intY)
                    {
                        return 0;
                    }

                    isXEarlier = intX < intY;
                }
                else
                {
                    string strX = (string)xValue;
                    string strY = (string)yValue;

                    int compare = strX.CompareTo(strY);
                    if (compare == 0)
                    {
                        return 0;
                    }

                    isXEarlier = compare < 0;
                }
            }

            if (m_direction == ListSortDirection.Ascending)
            {
                return isXEarlier ? -1 : 1;
            }
            else
            {
                return isXEarlier ? 1 : -1;
            }

        }
    }
}
