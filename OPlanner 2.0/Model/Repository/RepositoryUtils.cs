﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerNameSpace.Model
{
    public partial class ItemRepository
    {
        //------------------------------------------------------------------------------------
        /// <summary>
        /// Given a list of objects, return a store item collection of the specified type of
        /// StoreItems.  If none of the objects in the given list are of that type, an empty
        /// list will be returned.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static StoreItemCollection<T> GetStoreItemsFromList<T>(IList items) where T : StoreItem, new()
        {
            StoreItemCollection<T> storeItems = new StoreItemCollection<T>();
            foreach (object item in items)
            {
                T storeItem = item as T;
                if (storeItem != null)
                {
                    storeItems.Add(storeItem);
                }
            }

            return storeItems;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Given a collection of backlog items, return the total work remaining for all the
        /// work items associated with those backlog items.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static int GetTotalWorkRemaining(StoreItemCollection<BacklogItem> backlogItems)
        {
            int totalWorkRemaining = 0;
            foreach (BacklogItem backlogItem in backlogItems)
            {
                totalWorkRemaining += backlogItem.TotalWorkRemaining;
            }

            return totalWorkRemaining;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Deletes (closes as won't fix) all the store items on the given list, without UI
        /// prompting.  
        /// </summary>
        //------------------------------------------------------------------------------------
        public static void DeleteItems<T>(StoreItemCollection<T> items) where T : StoreItem
        {
            foreach (T item in items)
            {
                item.DeleteItem();
            }
        }

        public static AsyncObservableCollection<T> GetItems<T>(AsyncObservableCollection<T> collection, DummyItemType collectionType) where T : StoreItem, new()
        {
            AsyncObservableCollection<T> items = collection.ToCollection();
            items.Sort((x, y) => x.Title.CompareTo(y.Title));

            if (collectionType == DummyItemType.NoneType || collectionType == DummyItemType.AllNoneType)
            {
                T item = StoreItem.GetDummyItem<T>(DummyItemType.NoneType);
                items.Insert(0, item);
            }

            if (collectionType == DummyItemType.AllType || collectionType == DummyItemType.AllNoneType)
            {
                T item = StoreItem.GetDummyItem<T>(DummyItemType.AllType);
                items.Insert(0, item);
            }

            return items;
        }

        public static AsyncObservableCollection<T> GetItems<T>(AsyncObservableCollection<T> collection, DummyItemType collectionType, string sortPropName) where T : StoreItem, new()
        {
            AsyncObservableCollection<T> items = collection.ToCollection();

            if (collectionType == DummyItemType.NoneType || collectionType == DummyItemType.AllNoneType)
            {
                T item = StoreItem.GetDummyItem<T>(DummyItemType.NoneType);
                items.Add(item);
            }

            if (collectionType == DummyItemType.AllType || collectionType == DummyItemType.AllNoneType)
            {
                T item = StoreItem.GetDummyItem<T>(DummyItemType.AllType);
                items.Add(item);
            }

            ItemPropertySort<T> itemComparer = new ItemPropertySort<T>(sortPropName, System.ComponentModel.ListSortDirection.Ascending);
            items.Sort((x, y) => itemComparer.Compare(x, y));

            return items;
        }


    }
}
