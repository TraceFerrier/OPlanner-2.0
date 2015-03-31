using System.Reflection;

namespace PlannerNameSpace
{
    public partial class StoreItem
    {
        protected virtual IStoreItemList GetChildItems() { return null; }
        protected virtual IStoreItemList GetSkipLevelOwnedItems() { return null; }
        protected virtual IStoreItemList GetOwnedItems() { return null; }

        protected virtual bool IsLeafItem { get { return false; } }
        protected virtual IStoreItemList GetCache() { return null; }

        public virtual string ParentKeyProperty { get { return null; } }
        public virtual string OwnerKeyProperty { get { return null; } }
        public virtual string OwnerAssignedProperty { get { return null; } }

        #region Public cache interface

        public StoreItem ParentItem
        {
            get { return GetItemByParentKeyPropertyName(ParentKeyProperty); }
        }

        public StoreItem OwnerItem
        {
            get
            {
                if (OwnerKeyProperty != null)
                {
                    return GetItemByParentKeyPropertyName(OwnerKeyProperty);
                }
                else if (OwnerAssignedProperty != null)
                {
                    return AssignedToGroupMember;
                }

                return null;
            }
        }

        private StoreItem GetItemByParentKeyPropertyName(string propName)
        {
            if (propName != null)
            {
                PropertyInfo prop = this.GetType().GetProperty(propName);
                string parentKey = (string)prop.GetValue(this);
                return Planner.Instance.ItemRepository.GetItem(parentKey);
            }

            return null;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Will be called when this item comes into existence and has been added to the
        /// global cache.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void OnAdd()
        {
            // Maintain item cache
            AddToItemCache();

            // Maintain hierarchy
            if (StoreItem.IsRealItem(ParentItem))
            {
                ParentItem.ChildAdded(this);
            }

            // Maintain owners
            if (StoreItem.IsRealItem(OwnerItem))
            {
                OwnerItem.AddOwnedItem(this);
            }

            FinalizeItem();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Will be called when this item is being deleted, and removed from the global
        /// cache.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void OnRemove()
        {
            // Maintain item cache
            RemoveFromItemCache();

            if (StoreItem.IsRealItem(ParentItem))
            {
                ParentItem.ChildRemoved(this);
            }

            // Maintain owners
            if (StoreItem.IsRealItem(OwnerItem))
            {
                OwnerItem.RemoveOwnedItem(this);
            }

        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Will be called when the given property for this item has been updated.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void OnUpdate(ItemProperty itemProperty)
        {
            // Maintain hierarchy
            if (itemProperty.PublicPropName == ParentKeyProperty)
            {
                StoreItem previousParent = Planner.Instance.ItemRepository.GetItem((string)itemProperty.PreviousValue);
                StoreItem newParent = Planner.Instance.ItemRepository.GetItem((string)itemProperty.CurrentValue);

                if (StoreItem.IsRealItem(previousParent))
                {
                    previousParent.ChildRemoved(this);
                }

                if (StoreItem.IsRealItem(newParent))
                {
                    newParent.ChildAdded(this);
                }
            }

            // Maintain ownership
            else if (itemProperty.PublicPropName == OwnerKeyProperty)
            {
                StoreItem previousOwner = Planner.Instance.ItemRepository.GetItem((string)itemProperty.PreviousValue);
                StoreItem newOwner = Planner.Instance.ItemRepository.GetItem((string)itemProperty.CurrentValue);

                if (StoreItem.IsRealItem(previousOwner))
                {
                    previousOwner.RemoveOwnedItem(this);
                }

                if (StoreItem.IsRealItem(newOwner))
                {
                    newOwner.AddOwnedItem(this);
                }
            }
            else if (itemProperty.PublicPropName == OwnerAssignedProperty)
            {
                StoreItem previousOwner = Planner.Instance.ItemRepository.GetMemberByAlias((string)itemProperty.PreviousValue);
                StoreItem newOwner = Planner.Instance.ItemRepository.GetMemberByAlias((string)itemProperty.CurrentValue);
                if (StoreItem.IsRealItem(previousOwner))
                {
                    previousOwner.RemoveOwnedItem(this);
                }

                if (StoreItem.IsRealItem(newOwner))
                {
                    newOwner.AddOwnedItem(this);
                }
            }
            else
            {
                NotifyItemUpdated(itemProperty);
            }

            MaintainFilters(ChangeType.Updated);
        }

        #endregion

        private void AddToItemCache()
        {
            IStoreItemList itemCache = GetCache();
            if (itemCache != null)
            {
                itemCache.Add(this);
            }

            MaintainFilters(ChangeType.Added);
        }

        private void RemoveFromItemCache()
        {
            IStoreItemList itemCache = GetCache();
            if (itemCache != null)
            {
                itemCache.Remove(this);
            }

            MaintainFilters(ChangeType.Removed);
        }

        private void ChildAdded(StoreItem childItem)
        {
            IStoreItemList childItems = GetChildItems();
            if (childItems == null)
            {
                return;
            }

            // If this child item isn't the type that this parent expects,
            // don't add it to the list of children.
            if (!childItems.ItemMatchesListType(childItem))
            {
                Planner.OnInvalidHierarchicalItemEvent(this, childItem);
                return;
            }

            if (!childItems.Contains(childItem))
            {
                childItems.Add(childItem);

                if (Planner.Instance.IsStartupComplete)
                {
                    NotifyItemUpdatedWorker(new NotificationArgs(childItem, ChangeType.Added, HierarchicalChangeSource.ChildItem, null));
                }

                if (StoreItem.IsRealItem(ParentItem))
                {
                    ParentItem.ChildAdded(this);
                }
            }
        }

        private void ChildRemoved(StoreItem childItem)
        {
            IStoreItemList childItems = GetChildItems();
            if (childItems != null && childItems.Contains(childItem))
            {
                childItems.Remove(childItem);

                if (Planner.Instance.IsStartupComplete)
                {
                    NotifyItemUpdatedWorker(new NotificationArgs(childItem, ChangeType.Removed, HierarchicalChangeSource.ChildItem, null));
                }
            }
        }

        private void AddOwnedItem(StoreItem ownedItem)
        {
            IStoreItemList ownedItems = GetOwnedItems();
            if (ownedItems != null)
            {
                if (!ownedItems.Contains(ownedItem))
                {
                    ownedItems.Add(ownedItem);

                    if (Planner.Instance.IsStartupComplete)
                    {
                        NotifyItemUpdatedWorker(new NotificationArgs(ownedItem, ChangeType.Added, HierarchicalChangeSource.OwnedItem, null));
                    }
                }
            }
        }

        private void RemoveOwnedItem(StoreItem ownedItem)
        {
            IStoreItemList ownedItems = GetOwnedItems();
            if (ownedItems != null)
            {
                if (ownedItems.Contains(ownedItem))
                {
                    ownedItems.Remove(ownedItem);

                    if (Planner.Instance.IsStartupComplete)
                    {
                        NotifyItemUpdatedWorker(new NotificationArgs(ownedItem, ChangeType.Removed, HierarchicalChangeSource.OwnedItem, null));
                    }
                }
            }
        }

    }
}
