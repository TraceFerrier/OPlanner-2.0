using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;

public enum Discipline
{
    Dev,
    Test,
    PM
}

namespace PlannerNameSpace
{
    public partial class WorkItem : StoreItem
    {
        public override ItemTypeID StoreItemType { get { return ItemTypeID.WorkItem; } }
        public override string DefaultItemPath { get { return ScheduleStore.Instance.DefaultTeamTreePath; } }

        public static bool IsPropNameTitle(string publicPropName) { return publicPropName == StringUtils.GetPropertyName((WorkItem p) => p.Title); }
        public static bool IsPropNameEstimate(string publicPropName) { return publicPropName == StringUtils.GetPropertyName((WorkItem p) => p.Estimate); }
        public static bool IsPropNameCompleted(string publicPropName) { return publicPropName == StringUtils.GetPropertyName((WorkItem p) => p.Completed); }
        public static bool IsPropNameWorkRemaining(string publicPropName) { return publicPropName == StringUtils.GetPropertyName((WorkItem p) => p.WorkRemaining); }

        public WorkItem()
        {
        }

        #region ParentItem implementation

        public BacklogItem ParentBacklogItem
        {
            get
            {
                return Planner.Instance.ItemRepository.GetItem<BacklogItem>(ParentBacklogItemKey);
            }
            set
            {
                ParentBacklogItemID = value.ID;

                TreeID = value.TreeID;
                ShipCycle = value.ShipCycle;
                FixBy = value.FixBy;
            }
        }

        public int ParentBacklogItemID
        {
            get
            {
                return GetIntValue(Datastore.PropNameParentBacklogItemID);
            }

            set
            {
                SetIntValue(Datastore.PropNameParentBacklogItemID, value);
            }
        }

        public string ParentBacklogItemKey
        {
            get
            {
                return GetParentBacklogItemKey(ParentBacklogItemID);
            }
        }

        public static string GetParentBacklogItemKey(int itemID)
        {
            if (itemID != 0)
            {
                return GetHostItemKey(itemID);
            }

            return null;
        }

        #endregion


        public static WorkItem CreateWorkItem(BacklogItem parentBacklogItem, string title = "New WorkItem", string subtype = SubtypeValues.ProductCoding,
            string assignedTo = null)
        {
            WorkItem newWorkItem = HostItemStore.Instance.CreateStoreItem<WorkItem>(ItemTypeID.WorkItem);
            if (assignedTo == null)
            {
                assignedTo = Planner.Instance.CurrentUserAlias;
            }

            newWorkItem.AssignedTo = assignedTo;
            newWorkItem.ParentBacklogItem = parentBacklogItem;
            newWorkItem.Title = title;
            newWorkItem.Subtype = subtype;
            return newWorkItem;
        }

        public virtual int WorkRemaining
        {
            get
            {
                int workRemaining = Estimate - Completed;
                if (workRemaining < 0) workRemaining = 0;
                return workRemaining;
            }
        }

        public int OriginalEstimate
        {
            get { return GetIntValue(Datastore.PropNameOriginalEstimate); }
            set { SetIntValue(Datastore.PropNameOriginalEstimate, value); }
        }

        public int Estimate
        {
            get
            {
                return GetIntValue(Datastore.PropNameEstimate);
            }

            set
            {
                SetIntValue(Datastore.PropNameEstimate, value);
                NotifyStatusChanged();
            }
        }

        public int Completed
        {
            get { return GetIntValue(Datastore.PropNameCompleted); }
            set
            {
                int completed = value;
                int estimate = Estimate;
                if (completed > estimate)
                {
                    completed = estimate;
                }

                SetIntValue(Datastore.PropNameCompleted, completed);
                NotifyStatusChanged();
            }
        }

        void NotifyStatusChanged()
        {
            NotifyPropertyChanged(() => WorkRemaining);
            NotifyPropertyChanged(() => ItemStatus);

            if (IsClosed && WorkRemaining > 0)
            {
                Status = StatusValues.Active;
            }
        }

        public string AssignedToDisplayName
        {
            get
            {
                GroupMemberItem member = AssignedToGroupMember;
                if (member != null)
                {
                    return member.DisplayName;
                }

                return null;
            }
        }

        public static List<string> DevWorkItemSubtypes
        {
            get 
            {
                if (m_devWorkItemSubtypes == null)
                {
                    m_devWorkItemSubtypes = new List<string>();
                    m_devWorkItemSubtypes.Add("Product Coding");
                    m_devWorkItemSubtypes.Add("Service Improvements");
                    m_devWorkItemSubtypes.Add("Feature");
                    m_devWorkItemSubtypes.Add("Assessment");
                }

                return m_devWorkItemSubtypes;
            }
        }

        public static List<string> TestWorkItemSubtypes
        {
            get 
            {
                if (m_testWorkItemSubtypes == null)
                {
                    m_testWorkItemSubtypes = new List<string>();
                    m_testWorkItemSubtypes.Add("Automation");
                    m_testWorkItemSubtypes.Add("Automation Coding");
                    m_testWorkItemSubtypes.Add("Automated Testing");
                    m_testWorkItemSubtypes.Add("Manual Testing");
                }

                return m_testWorkItemSubtypes;
            }
        }

        private static List<string> m_devWorkItemSubtypes;
        private static List<string> m_testWorkItemSubtypes;

        public override string Subtype
        {
            get
            {
                return base.Subtype;
            }
            set
            {
                base.Subtype = value;
                NotifyPropertyChanged(() => WorkItemDiscipline);
            }
        }

        public Discipline WorkItemDiscipline
        {
            get
            {
                foreach (string devSubType in DevWorkItemSubtypes)
                {
                    if (StringUtils.StringsMatch(devSubType, Subtype))
                    {
                        return Discipline.Dev;
                    }
                }

                foreach (string testSubType in TestWorkItemSubtypes)
                {
                    if (StringUtils.StringsMatch(testSubType, Subtype))
                    {
                        return Discipline.Test;
                    }
                }

                return Discipline.Dev;
            }
        }

        static AsyncObservableCollection<string> s_statusValues;
        public AsyncObservableCollection<string> ItemStatusValues
        {
            get
            {
                if (s_statusValues == null)
                {
                    s_statusValues = new AsyncObservableCollection<string>();
                    s_statusValues.Add(WorkItemDisplayStates.NotStarted);
                    s_statusValues.Add(WorkItemDisplayStates.InProgress);
                    s_statusValues.Add(WorkItemDisplayStates.Completed);
                    s_statusValues.Add(WorkItemDisplayStates.CompletedAndResolved);
                    s_statusValues.Add(WorkItemDisplayStates.CompletedAndClosed);
                    s_statusValues.Add(WorkItemDisplayStates.Delete);
                }

                return s_statusValues;
            }
        }

        // The actual purpose of this method is only to force the ItemStatus property to re-evaluate
        // (and send a PropertyUpdated notification if appropriate).
        void ForceItemStatusEvaluation(WorkItemStates itemStatus)
        {
            Planner.Instance.WriteToEventLog("WorkItem status updated: " + itemStatus.ToString());
        }

        public WorkItemStates ItemStatus
        {
            get
            {
                WorkItemStates itemStatus;

                // Handle the case where the work item is currently closed, but then the user edited
                // the estimate or completed values to bring back work remaining.
                if (IsClosed && WorkRemaining > 0)
                {
                    if (Completed > 0)
                    {
                        itemStatus = WorkItemStates.InProgress;
                    }
                    else
                    {
                        itemStatus = WorkItemStates.NotStarted;
                    }
                }
                else if (IsClosed || IsResolved || WorkRemaining == 0 && Completed > 0)
                {
                    itemStatus = WorkItemStates.Completed;
                }
                else if (Completed > 0 || SubStatus == SubStatusValues.Investigating || SubStatus == SubStatusValues.WorkingOnFix)
                {
                    itemStatus = WorkItemStates.InProgress;
                }
                else
                {
                    itemStatus = WorkItemStates.NotStarted;
                }

                return itemStatus;
            }

            set
            {
                switch (value)
                {
                    case WorkItemStates.Completed:
                        Completed = Estimate;
                        break;

                    case WorkItemStates.InProgress:
                        SubStatus = SubStatusValues.WorkingOnFix;
                        break;

                    case WorkItemStates.Delete:
                        break;

                    case WorkItemStates.NotStarted:
                        SubStatus = "";
                        break;
                }

                ForceItemStatusEvaluation(ItemStatus);
            }
        }

        public override string AssignedTo
        {
            get
            {
                if (IsResolved || IsClosed)
                {
                    string workAssignedTo = WorkAssignedTo;
                    if (!string.IsNullOrWhiteSpace(workAssignedTo))
                    {
                        if (Planner.Instance.ItemRepository.GroupMemberExists(workAssignedTo))
                        {
                            return workAssignedTo;
                        }
                    }
                }
                
                return base.AssignedTo;
            }
            set
            {
                base.AssignedTo = value;

                if (IsActive)
                {
                    WorkAssignedTo = value;
                }
            }
        }

        public string WorkAssignedTo
        {
            get { return GetStringValue(Datastore.PropNameWorkItemWorkAssignedToKey); }
            set { SetStringValue(Datastore.PropNameWorkItemWorkAssignedToKey, value); }
        }

        public override void OnResolution()
        {
            base.OnResolution();
            WorkAssignedTo = AssignedTo;
        }
    }
}
