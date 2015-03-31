using System.ComponentModel;
using System.Windows.Data;

namespace PlannerNameSpace
{
    public abstract partial class StoreItem
    {
        private static ICollectionView m_pillarItemsView;
        private static ICollectionView m_trainItemsView;
        private static ICollectionView m_pmMembersView;

        public string PillarFriendlyName
        {
            get { return Constants.PillarFriendlyName; }
        }

        public string TrainFriendlyName
        {
            get { return Constants.TrainFriendlyName; }
        }

        public static ICollectionView AvailablePMMembers
        {
            get
            {
                if (m_pmMembersView == null)
                {
                    m_pmMembersView = CollectionViewSource.GetDefaultView(Planner.Instance.ItemRepository.GetPMMembers());
                }

                return m_pmMembersView;
            }
        }

        public static ICollectionView PillarItemsView
        {
            get
            {
                if (m_pillarItemsView == null)
                {
                    m_pillarItemsView = CollectionViewSource.GetDefaultView(PillarItem.Items);
                    SortUtils.SetCustomSorting(m_pillarItemsView, new ItemPropertySort<StoreItem>(StringUtils.GetPropertyName((StoreItem b) => b.Title)), ListSortDirection.Ascending);
                }

                return m_pillarItemsView;
            }

            private set { StoreItem.m_pillarItemsView = value; }
        }

        public static ICollectionView TrainItemsView
        {
            get
            {
                if (m_trainItemsView == null)
                {
                    m_trainItemsView = CollectionViewSource.GetDefaultView(Planner.Instance.ItemRepository.TrainItems);
                    SortUtils.SetCustomSorting(m_trainItemsView, new ItemPropertySort<StoreItem>(StringUtils.GetPropertyName((StoreItem b) => b.Title)), ListSortDirection.Ascending);
                }

                return m_trainItemsView;
            }

            private set { StoreItem.m_trainItemsView = value; }
        }

    }
}
