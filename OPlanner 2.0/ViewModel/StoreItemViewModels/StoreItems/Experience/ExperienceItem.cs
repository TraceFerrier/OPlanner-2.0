using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Windows.Data;

namespace PlannerNameSpace
{
    public partial class ExperienceItem : StoreItem
    {
        public override ItemTypeID StoreItemType { get { return ItemTypeID.Experience; } }
        public override string DefaultItemPath { get { return ScheduleStore.Instance.DefaultTeamTreePath; } }

        public static ExperienceItem GetDummyAllItem()
        {
            return StoreItem.GetDummyItem<ExperienceItem>(DummyItemType.AllType);
        }
        private ICollectionView m_backlogItemsView;

        public ICollectionView BacklogItemsView
        {
            get
            {
                if (m_backlogItemsView == null)
                {
                    m_backlogItemsView = CollectionViewSource.GetDefaultView(BacklogItems);
                    SortUtils.SetCustomSorting(m_backlogItemsView, new ItemPropertySort<BacklogItem>(StringUtils.GetPropertyName((BacklogItem b) => b.BusinessRank)), ListSortDirection.Ascending);
                }

                return m_backlogItemsView;
            }
        }

        private BacklogItem m_selectedBacklogItem;

        public BacklogItem SelectedBacklogItem
        {
            get { return m_selectedBacklogItem; }
            set { m_selectedBacklogItem = value; }
        }


        public override int BusinessRank
        {
            get { return GetIntValue(Datastore.PropNameExperienceBusinessRank); }
            set { SetIntValue(Datastore.PropNameExperienceBusinessRank, value); }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// The primary owner for this scenario
        /// </summary>
        //------------------------------------------------------------------------------------
        public GroupMemberItem Owner
        {
            get
            {
                GroupMemberItem owner = Planner.Instance.ItemRepository.GetMemberByAlias(OwnerAlias);
                if (owner == null)
                {
                    return StoreItem.GetDummyItem<GroupMemberItem>(DummyItemType.NoneType);
                }
                else
                {
                    return owner;
                }
            }

            set
            {
                GroupMemberItem owner = value;
                if (!StoreItem.IsRealItem(owner))
                {
                    OwnerAlias = null;
                }
                else
                {
                    OwnerAlias = owner.Alias; 
                }

                NotifyPropertyChanged(() => OwnerPicture);
            }
        }

        public string OwnerAlias
        {
            get { return GetStringValue(Datastore.PropNameExperienceOwnerAlias); }
            set { SetStringValue(Datastore.PropNameExperienceOwnerAlias, value); }
        }

        public BitmapSource OwnerPicture
        {
            get
            {
                if (StoreItem.IsRealItem(Owner))
                {
                    return Owner.GetUserPicture(this);
                }

                return GetGenericProfileImage();
            }
        }


    }
}
