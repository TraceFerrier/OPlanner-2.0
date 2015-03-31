using System;
using System.Collections;
using System.ComponentModel;

namespace PlannerNameSpace
{
    public class BusinessRankSort : BaseSort
    {
        public override string SortTitle
        {
            get
            {
                return "Business Rank";
            }
            set
            {
                base.SortTitle = value;
            }
        }

        public override int Compare(object x, object y)
        {
            StoreItem storeItemX = (StoreItem)x;
            StoreItem storeItemY = (StoreItem)y;
            bool isXLater = false;

            // Any item assigned to a train ranks higher than an item 'on the backlog'
            if (StoreItem.IsRealItem(storeItemX.ParentTrainItem) && !StoreItem.IsRealItem(storeItemY.ParentTrainItem))
            {
                isXLater = false;
            }
            else if (!StoreItem.IsRealItem(storeItemX.ParentTrainItem) && StoreItem.IsRealItem(storeItemY.ParentTrainItem))
            {
                isXLater = true;
            }

            // The item assigned to the earlier train ranks earlier
            else if (StoreItem.IsRealItem(storeItemX.ParentTrainItem) && StoreItem.IsRealItem(storeItemY.ParentTrainItem) && storeItemX.ParentTrainItem.EndDate > storeItemY.ParentTrainItem.EndDate)
            {
                isXLater = true;
            }
            else if (StoreItem.IsRealItem(storeItemX.ParentTrainItem) && StoreItem.IsRealItem(storeItemY.ParentTrainItem) && storeItemX.ParentTrainItem.EndDate < storeItemY.ParentTrainItem.EndDate)
            {
                isXLater = false;
            }
            else
            {
                // If both items are assigned to the same train, then rank according to BusinessRank
                if (storeItemX.BusinessRank == storeItemY.BusinessRank)
                {
                    return 0;
                }

                isXLater = storeItemX.BusinessRank > storeItemY.BusinessRank;
            }

            if (m_direction == ListSortDirection.Ascending)
            {
                return isXLater ? 1 : -1;
            }
            else
            {
                return isXLater ? -1 : 1;
            }
        }
    }
}
