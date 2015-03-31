using System;
using System.Collections.ObjectModel;

namespace PlannerNameSpace.Model
{
    public partial class ItemRepository
    {
        public GroupMemberItemCache GroupMemberItems;
        public AsyncObservableCollection<PersonaItem> PersonaItems;
        public AsyncObservableCollection<PlannerBugItem> PlannerBugItems;
        public StoreItemCollection<ProductGroupItem> ProductGroupItems;
        public AsyncObservableCollection<TrainItem> TrainItems;


        public void InitializeCaches()
        {
            GroupMemberItems = new GroupMemberItemCache();
            TrainItems = new AsyncObservableCollection<TrainItem>();

            // Item Caches
            ProductGroupItems = new StoreItemCollection<ProductGroupItem>();
            PersonaItems = new AsyncObservableCollection<PersonaItem>();
            PlannerBugItems = new AsyncObservableCollection<PlannerBugItem>();
        }

        void AddToItemTypeCache(StoreItem item)
        {
            switch (item.StoreItemType)
            {
                case ItemTypeID.Train:
                    TrainItem trainItem = (TrainItem)item;
                    if (trainItem.EndDate.AddDays(45) >= DateTime.Today)
                    {
                        TrainItems.Add((TrainItem)item);
                    }
                    break;
                case ItemTypeID.ProductGroup:
                    ProductGroupItems.Add((ProductGroupItem)item);
                    break;
                case ItemTypeID.Persona:
                    PersonaItems.Add((PersonaItem)item);
                    break;
                case ItemTypeID.PlannerBug:
                    PlannerBugItems.Add((PlannerBugItem)item);
                    break;
                case ItemTypeID.GroupMember:
                    GroupMemberItems.Add((GroupMemberItem)item);
                    break;
            }
        }

        void RemoveFromItemTypeCache(StoreItem item)
        {
            switch (item.StoreItemType)
            {
                case ItemTypeID.Train:
                    TrainItems.Remove((TrainItem)item);
                    break;
                case ItemTypeID.ProductGroup:
                    ProductGroupItems.Remove((ProductGroupItem)item);
                    break;
                case ItemTypeID.Persona:
                    PersonaItems.Remove((PersonaItem)item);
                    break;
                case ItemTypeID.PlannerBug:
                    PlannerBugItems.Remove((PlannerBugItem)item);
                    break;
                case ItemTypeID.GroupMember:
                    GroupMemberItems.Remove((GroupMemberItem)item);
                    break;
            }
        }

    }

}
