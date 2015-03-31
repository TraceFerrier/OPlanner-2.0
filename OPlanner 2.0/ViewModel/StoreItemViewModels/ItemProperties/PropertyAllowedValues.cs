using System.Collections.Generic;

namespace PlannerNameSpace
{
    public static class PropertyAllowedValues
    {
        private static Dictionary<ItemTypeID, AsyncObservableCollection<AllowedValue>> m_subtypeAllowedValues;

        public static AsyncObservableCollection<AllowedValue> GetAvailableSubtypes(StoreItem item)
        {
            ItemTypeID itemType = item.StoreItemType;
            if (m_subtypeAllowedValues == null)
            {
                m_subtypeAllowedValues = new Dictionary<ItemTypeID, AsyncObservableCollection<AllowedValue>>();
            }

            if (!m_subtypeAllowedValues.ContainsKey(itemType))
            {
                //try
                {
                    AsyncObservableCollection<AllowedValue> allowedValues = item.Store.GetFieldAllowedValues(item, Datastore.PropNameSubtype);
                    AsyncObservableCollection<AllowedValue> cachedValues = new AsyncObservableCollection<AllowedValue>();
                    foreach (AllowedValue allowedValue in allowedValues)
                    {
                        cachedValues.Add(allowedValue);
                    }

                    m_subtypeAllowedValues.Add(itemType, cachedValues);
                    return cachedValues;
                }

                //catch (Exception exception)
                //{
                //    Planner.ApplicationManager.HandleException(exception);
                //    m_subtypeAllowedValues.Add(itemType, new AsyncObservableCollection<AllowedValue>());
                //    return m_subtypeAllowedValues[itemType];
                //}
            }
            else
            {
                return m_subtypeAllowedValues[itemType];
            }
        }

    }
}
