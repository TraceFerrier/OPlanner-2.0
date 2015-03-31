using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;

namespace PlannerNameSpace
{
    public partial class BacklogItem : StoreItem
    {
        public BacklogItem()
        {
            m_workItems = new StoreItemCollection<WorkItem>();
            m_currentWorkItems = new StoreItemCollection<WorkItem>();
        }

        public override ItemTypeID StoreItemType { get { return ItemTypeID.BacklogItem; } }
        public override string DefaultItemPath { get { return ScheduleStore.Instance.DefaultTeamTreePath; } }

        private PillarItem m_parentPillarItem;
        private TrainItem m_parentTrainItem;
        private ICollectionView m_workItemsView;

        public ICollectionView WorkItemsView
        {
            get
            {
                if (m_workItemsView == null)
                {
                    m_workItemsView = CollectionViewSource.GetDefaultView(WorkItems);
                    SortUtils.SetCustomSorting(m_workItemsView, new ItemPropertySort<WorkItem>(StringUtils.GetPropertyName((WorkItem w) => w.BusinessRank)), ListSortDirection.Ascending);
                }

                return m_workItemsView;
            }
        }

        private WorkItem m_selectedWorkItem;

        public WorkItem SelectedWorkItem
        {
            get { return m_selectedWorkItem; }
            set { m_selectedWorkItem = value; }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the PillarItem that this backlog item is assigned to.
        /// </summary>
        //------------------------------------------------------------------------------------
        public PillarItem ParentPillarItem
        {
            get
            {
                if (m_parentPillarItem == null)
                {
                    m_parentPillarItem = Planner.Instance.ItemRepository.FindOwnerPillar(this.TreeID);
                    if (m_parentPillarItem == null)
                    {
                        m_parentPillarItem = GetDummyItem<PillarItem>(DummyItemType.NoneType);
                    }
                }

                return m_parentPillarItem;
            }

            set
            {
                if (value != null)
                {
                    PillarItem currentPillarItem = m_parentPillarItem;
                    if (currentPillarItem != null && currentPillarItem.IsDummyItem)
                    {
                        currentPillarItem = null;
                    }

                    PillarItem proposedPillarItem = value;
                    if (currentPillarItem == null || proposedPillarItem.StoreKey != currentPillarItem.StoreKey)
                    {
                        bool changeValidated = true;

                        if (changeValidated)
                        {
                            m_parentPillarItem = proposedPillarItem;
                            this.TreeID = m_parentPillarItem.PillarPathID;
                        }
                    }
                }
            }
        }

        public override int TreeID
        {
            get
            {
                return base.TreeID;
            }
            set
            {
                base.TreeID = value;
                m_parentPillarItem = null;
                NotifyPropertyChanged(() => ParentPillarItem);
            }
        }

        public override void DeleteItem()
        {
            List<WorkItem> workItems = WorkItems.ToList();
            foreach (WorkItem workItem in workItems)
            {
                if (workItem.IsResolvedAnyResolution)
                {
                    workItem.CloseItem();
                }
                else if (workItem.IsActive)
                {
                    workItem.DeleteItem();
                }
            }

            base.DeleteItem();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the TrainItem that this backlog item is assigned to.
        /// </summary>
        //------------------------------------------------------------------------------------
        public override TrainItem ParentTrainItem
        {
            get
            {
                if (m_parentTrainItem == null)
                {
                    m_parentTrainItem = Planner.Instance.ItemRepository.FindTrain(GetEffectiveShipCycle(ShipCycle), FixBy);
                    if (m_parentTrainItem == null)
                    {
                        m_parentTrainItem = GetDummyItem<TrainItem>(DummyItemType.NoneType);
                    }
                }

                return m_parentTrainItem;
            }

            set
            {
                if (value == null)
                {
                    return;
                }

                TrainItem currentTrain = m_parentTrainItem;
                if (currentTrain == null || currentTrain.IsNoneItem)
                {
                    currentTrain = null;
                }

                TrainItem proposedTrainItem = value;
                if (currentTrain == null || proposedTrainItem.StoreKey != currentTrain.StoreKey)
                {
                    ShipCycle = value.TrainShipCycle;
                    FixBy = value.TrainFixBy;
                }
            }
        }

        public override string FixBy
        {
            get
            {
                return base.FixBy;
            }
            set
            {
                base.FixBy = value;
                m_parentTrainItem = null;
                NotifyPropertyChanged(() => ParentTrainItem);
            }
        }

        public ScrumTeamItem ScrumTeamItem
        {
            get
            {
                ScrumTeamItem scrumTeamItem = Planner.Instance.ItemRepository.GetItem<ScrumTeamItem>(ScrumTeamKey);
                if (scrumTeamItem == null)
                {
                    return ScrumTeamItem.GetDummyNoneTeam();
                }

                return scrumTeamItem;
            }

            set
            {
                if (value == null || value.IsDummyItem)
                {
                    ScrumTeamKey = null;
                }

                else
                {
                    ScrumTeamItem scrumTeamItem = value;

                    if (scrumTeamItem != null)
                    {
                        ScrumTeamKey = scrumTeamItem.StoreKey;
                    }
                    else
                    {
                        ScrumTeamKey = null;
                    }

                    m_parentTrainItem = null;
                }
            }
        }
        public string ScrumTeamKey
        {
            get
            {
                return GetStringValue(Datastore.PropNameBacklogScrumTeamKey);
            }

            set
            {
                SetStringValue(Datastore.PropNameBacklogScrumTeamKey, value);
            }
        }

        public string ParentScenarioItemKey
        {
            get
            {
                return GetStringValue(Datastore.PropNameParentScenarioItemKey);
            }

            set
            {
                SetStringValue(Datastore.PropNameParentScenarioItemKey, value);
            }
        }

        public int StoryPoints
        {
            get { return GetIntValue(Datastore.PropNameStoryPoints); }
            set { SetIntValue(Datastore.PropNameStoryPoints, value); }
        }

        public string PMOwner
        {
            get 
            {
                string pmOwnerAlias = GetStringValue(Datastore.PropNamePM_Owner);
                if (string.IsNullOrWhiteSpace(pmOwnerAlias))
                {
                    pmOwnerAlias = Constants.c_None;
                }

                return pmOwnerAlias;
            }

            set 
            {
                string pmOwnerAlias = value;
                if (pmOwnerAlias == Constants.c_None)
                {
                    pmOwnerAlias = null;
                }

                SetStringValue(Datastore.PropNamePM_Owner, pmOwnerAlias);
            }
        }

        public string TestOwner
        {
            get { return GetStringValue(Datastore.PropNameTest_Owner); }
            set { SetStringValue(Datastore.PropNameTest_Owner, value); }
        }

        public string DevOwner
        {
            get { return GetStringValue(Datastore.PropNameDev_Owner); }
            set { SetStringValue(Datastore.PropNameDev_Owner, value); }
        }

        public bool IsPostMortemIssue
        {
            get
            {
                if (PostMortemStatus == Constants.c_NotSet)
                {
                    return false;
                }

                return true;
            }
        }

        private int m_totalWorkScheduled = -1;
        public int TotalWorkScheduled
        {
            get
            {
                if (m_totalWorkScheduled < 0)
                {
                    CalculateTotalWorkStatistics();
                }

                return m_totalWorkScheduled;
            }
        }

        private int m_totalWorkCompleted = -1;
        public int TotalWorkCompleted
        {
            get
            {
                if (m_totalWorkCompleted < 0)
                {
                    CalculateTotalWorkStatistics();
                }

                return m_totalWorkCompleted;
            }
        }

        private int m_totalWorkRemaining = -1;
        public int TotalWorkRemaining
        {
            get
            {
                if (m_totalWorkRemaining < 0)
                {
                    CalculateTotalWorkStatistics();
                }

                return m_totalWorkRemaining;
            }
        }

        private void CalculateTotalWorkStatistics()
        {
            m_totalWorkScheduled = 0;
            m_totalWorkCompleted = 0;
            m_totalWorkRemaining = 0;
            foreach (WorkItem workItem in WorkItems)
            {
                m_totalWorkScheduled += workItem.Estimate;
                m_totalWorkCompleted += workItem.Completed;
                m_totalWorkRemaining += workItem.WorkRemaining;
            }

        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the total work remaining for all this item's work items, without regard
        /// to commitment status.
        /// </summary>
        //------------------------------------------------------------------------------------
        public int TotalWorkAvailable
        {
            get
            {
                int totalWorkRemaining = 0;
                foreach (WorkItem workItem in WorkItems)
                {
                    totalWorkRemaining += workItem.WorkRemaining;
                }

                return totalWorkRemaining;
            }
        }
    }
}
