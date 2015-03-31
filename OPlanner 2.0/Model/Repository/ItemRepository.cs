using ProductStudio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Threading;

namespace PlannerNameSpace.Model
{
    public partial class ItemRepository : DispatcherObject, IRepository
    {
        private string m_currentProductGroupKey { get; set; }
        public bool IsQueryInProgress { get; set; }
        public bool IsRefreshInProgress { get; set; }
        public ShouldRefresh ShouldRefresh { get; set; }
        private RefreshType RefreshType;
        BackgroundTask QueryTask { get; set; }

        public ItemRepository()
        {
            ShouldRefresh = PlannerNameSpace.ShouldRefresh.No;
            InitializeGroupMembers();
            InitializeItemManager();
        }

        #region // Implementation of IRepository

        void IRepository.ReceiveDSItems(Datastore store, DatastoreItems items, ShouldRefresh isRefresh, bool isDefer)
        {
            if (isRefresh == ShouldRefresh.Yes)
            {
                ReceiveRefreshStoreItemsToDefer(store, items);
            }
            else if (isDefer)
            {
                ReceiveStoreItemsToDefer(store, items);
            }
            else
            {
                foreach (DatastoreItem dsItem in items)
                {
                    StoreItem storeItem = store.CreateAndInitializeItemFromDS(dsItem);

                    // Every item loaded from the back-end store is placed into
                    // the global item cache for immediate access.
                    AddItemFromStore(storeItem);

                }
            }
        }

        #endregion

        #region CurrentProductGroup
        public string GetCurrentProductGroupKey()
        {
            return m_currentProductGroupKey;
        }

        public ProductGroupItem GetCurrentProductGroup()
        {
            return CurrentProductGroup;
        }

        private ProductGroupItem m_currentProductGroup;
        public ProductGroupItem CurrentProductGroup
        {
            get
            {
                if (m_currentProductGroup == null && m_currentProductGroupKey != null)
                {
                    m_currentProductGroup = GetItem<ProductGroupItem>(m_currentProductGroupKey);
                }

                return m_currentProductGroup;
            }

            set
            {
                m_currentProductGroup = value;
            }
        }

        public string CurrentProductGroupName { get { return CurrentProductGroup != null ? CurrentProductGroup.Title : null; } }

        #endregion

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Fired when the basic scheduling metadata items for the current product group 
        /// (OffTimeItems, GroupMemberItems, etc.) have been loaded and cached (before any 
        /// backlog or work items have been queried for).
        /// </summary>
        //------------------------------------------------------------------------------------
        public event EventHandler ScheduleMetadataReady;

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Begins the process of loading the current's team's planner schedule from the
        /// repository.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void InitializeRepository(bool shouldClearProductGroup)
        {
            ExperienceItem.InitializeFilters();
            BacklogItem.InitializeFilters();

            if (shouldClearProductGroup)
            {
                BeginProductGroupQuery(null);
            }
            else
            {
                BeginOpenProductGroup();
            }
        }

        private TrainItem m_currentTrain;
        DateTime m_currentTrainDate;
        public TrainItem CurrentTrain
        {
            get
            {
                if (m_currentTrain == null || m_currentTrainDate != null && m_currentTrainDate.Day != DateTime.Now.Day)
                {
                    AsyncObservableCollection<TrainItem> items = TrainItems;
                    foreach (TrainItem train in items)
                    {
                        if (train.TimeFrame == TrainTimeFrame.Current && StringUtils.StringsMatch(train.TrainShipCycle, "Gemini"))
                        {
                            if (train.EndDate > train.StartDate)
                            {
                                m_currentTrain = train;
                                m_currentTrainDate = DateTime.Now;
                                break;
                            }
                        }
                    }
                }

                return m_currentTrain;
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Given shipCycle and fixBy values, returns the TrainItem object that represents
        /// the corresponding train.
        /// </summary>
        //------------------------------------------------------------------------------------
        public TrainItem FindTrain(string shipCycle, string fixBy)
        {
            if (StringUtils.StringsMatch(TrainItem.BacklogTrainItem.TrainShipCycle, shipCycle) && StringUtils.StringsMatch(TrainItem.BacklogTrainItem.TrainFixBy, fixBy))
            {
                return TrainItem.BacklogTrainItem;
            }

            foreach (TrainItem train in TrainItems)
            {
                if (StringUtils.StringsMatch(train.TrainShipCycle, shipCycle) && StringUtils.StringsMatch(train.TrainFixBy, fixBy))
                {
                    return train;
                }
            }

            return null;
        }

        #region Commands

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Sends a refresh query to the back-end store, and syncs the in-memory cache to
        /// any resulting changes.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void Refresh()
        {
            if (!IsRefreshInProgress)
            {
                if (!IsDiscoveryComplete)
                {
                    UserMessage.Show("Please wait for item discovery to be completed before performing a Refresh.");
                }
                else if (ChangeCount > 0)
                {
                    UserMessage.Show("Refresh is not available while changes are pending - save your changes, and then try Refresh again.");
                }
                else
                {
                    PlannerOld.EventManager.OnPlannerRefreshStarting();
                    BeginRefreshQuery(m_currentProductGroupKey);
                }
            }
        }

        #endregion

        #region Queries
        //------------------------------------------------------------------------------------
        /// <summary>
        ///  Kicks off a background query to retrieve the data needed to build burndown
        ///  statistics for the currently loaded product group.
        /// </summary>
        //------------------------------------------------------------------------------------
        private void BeginProductGroupQuery(BackgroundTask continuingTask)
        {
            BackgroundTask queryTask = new BackgroundTask(continuingTask);
            queryTask.DoWork += ProductGroupQuery_DoWork;
            queryTask.TaskCompleted += ProductGroupQuery_TaskCompleted;
            queryTask.IsProgressDialogIndeterminate = true;
            queryTask.RunTaskAsync();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        ///  Background worker for the ProductGroup query.
        /// </summary>
        //------------------------------------------------------------------------------------
        void ProductGroupQuery_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            BackgroundTask taskWorker = e.Argument as BackgroundTask;
            taskWorker.ReportProgress(0, "", "Loading Product Groups...");

            try
            {
                List<ItemTypeID> typeList = new List<ItemTypeID>();
                typeList.Add(ItemTypeID.ProductGroup);
                typeList.Add(ItemTypeID.HelpContent);
                ActiveItemTypeQuery query = new ActiveItemTypeQuery(ScheduleStore.Instance, null, typeList, ShouldRefresh.No);

                BackgroundTaskResult queryResult = ScheduleStore.Instance.ExecuteQuery(this, query, ShouldRefresh, taskWorker, false);
                if (!CheckResults(taskWorker, e, queryResult))
                {
                    return;
                }

                e.Result = new BackgroundTaskResult { ResultType = ResultType.Completed };
            }

            catch (Exception exception)
            {
                Planner.Instance.WriteToEventLog(exception.Message);
                Planner.Instance.WriteToEventLog(exception.StackTrace);
                e.Result = new BackgroundTaskResult { ResultType = ResultType.Failed, ResultMessage = exception.Message };
                return;
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Called when product group query is completed.
        /// </summary>
        //------------------------------------------------------------------------------------
        void ProductGroupQuery_TaskCompleted(object TaskArgs, BackgroundTaskResult result)
        {
            ApplicationWelcome welcome = new ApplicationWelcome();
            welcome.BeginWelcome();

            if (welcome.Result == DialogResult.Cancel)
            {
                Planner.Shutdown();
            }

            else if (welcome.Result == DialogResult.Open)
            {
                string productGroupItemKey = welcome.SelectedProductGroupItem.StoreKey;
                Planner.Restart(productGroupItemKey);
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Begins the process of discovering and opening the last-opened product group.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void BeginOpenProductGroup()
        {
            BackgroundTask openProductGroupTask = new BackgroundTask(ProgressDialogOption.StandardProgressNoClose);
            openProductGroupTask.DoWork += openProductGroupTask_DoWork;
            openProductGroupTask.TaskCompleted += openProductGroupTask_TaskCompleted;
            openProductGroupTask.IsProgressDialogIndeterminate = true;
            openProductGroupTask.RunTaskAsync();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Background worker for BeginOpenProductGroup - this routine attempts to read the
        /// item from the backing store that represents the last-opened product group.
        /// </summary>
        //------------------------------------------------------------------------------------
        void openProductGroupTask_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            BackgroundTask taskWorker = e.Argument as BackgroundTask;
            taskWorker.ReportProgress(0, "", "Starting up...");

            //try
            {
                m_currentProductGroupKey = Planner.Instance.UserPreferences.GetGlobalPreference<string>(GlobalPreference.LastOpenProductGroupKey);
                CurrentProductGroup = Datastore.GetStoreItem(m_currentProductGroupKey) as ProductGroupItem;
                if (CurrentProductGroup == null || !CurrentProductGroup.IsActive)
                {
                    m_currentProductGroupKey = null;
                }

                e.Result = new BackgroundTaskResult { ResultType = ResultType.Completed };

            }

            //catch (Exception exception)
            //{
            //    e.Result = new BackgroundTaskResult { ResultType = ResultType.Failed, ResultMessage = exception.Message };
            //}
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Will be called when the task kicked off by BeginOpenProductGroup completes.
        /// </summary>
        //------------------------------------------------------------------------------------
        void openProductGroupTask_TaskCompleted(object TaskArgs, BackgroundTaskResult result)
        {
            if (result.ResultType == ResultType.Failed)
            {
                UserMessage.ShowTwoLines("OPlanner encountered an error during startup.", result.ResultMessage);
                Planner.Shutdown();
                return;
            }

            bool IsProductGroupCompatible = true;
            if (CurrentProductGroup != null && !CurrentProductGroup.IsCompatibleWithCurrentStore)
            {
                IsProductGroupCompatible = false;
                UserMessage.Show("The Product Group you last opened (" + CurrentProductGroup.Title + ") is not compatible with the current Store (" + HostItemStore.Instance.StoreName + "). Click 'OK' to pick a different group.");
            }

            // If the user hasn't already selected a product group on a previous run of the app, kick off a product group
            // query so that we can present the user with the current product groups available to be opened.
            if (m_currentProductGroupKey == null || !IsProductGroupCompatible)
            {
                BeginProductGroupQuery(result.Task);
            }
            else
            {
                BeginPlannerQuery(ShouldRefresh.No, m_currentProductGroupKey, result.Task);
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Handle the event fired when the attempt to open the current product group has 
        /// completed.
        /// </summary>
        //------------------------------------------------------------------------------------
        void Handle_OpenProductGroupComplete(object sender, ProductGroupOpenedEvent e)
        {
        }

        bool CheckResults(BackgroundTask taskWorker, DoWorkEventArgs e, BackgroundTaskResult result)
        {
            if (IsCancelled(taskWorker, e))
            {
                IsRefreshInProgress = false;
                return false;
            }

            if (result.ResultType != ResultType.Completed)
            {
                e.Result = result;
                return false;
            }

            return true;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        ///  Kicks off a background query to retrieve the entire planner schedule for the
        ///  specified product group.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void BeginPlannerQuery(ShouldRefresh shouldRefresh, string productGroupKey, BackgroundTask continuingTask = null, RefreshType refreshType = RefreshType.QueryForChangedItems)
        {
            IsQueryInProgress = true;
            IsRefreshInProgress = shouldRefresh == PlannerNameSpace.ShouldRefresh.Yes;

            ShouldRefresh = shouldRefresh;
            RefreshType = refreshType;

            if (continuingTask != null)
            {
                QueryTask = new BackgroundTask(continuingTask);
            }
            else
            {
                QueryTask = new BackgroundTask(ShouldRefresh == ShouldRefresh.No);
            }

            QueryTask.DoWork += PlannerQueryTask_DoWork;
            QueryTask.TaskCompleted += PlannerQueryTask_Completed;
            QueryTask.IsProgressDialogIndeterminate = true;
            QueryTask.RunTaskAsync();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        ///  Background worker that executes all planner store queries.
        /// </summary>
        //------------------------------------------------------------------------------------
        void PlannerQueryTask_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundTask taskWorker = e.Argument as BackgroundTask;

            const int totalTasks = 4;
            int currentTask = 1;

            {

                // First query for the items that are global to all product groups.
                taskWorker.ReportProgress((currentTask * 100) / totalTasks, "", "Looking for your product group...");
                List<ItemTypeID> typeList = new List<ItemTypeID>();
                typeList.Add(ItemTypeID.ProductGroup);
                typeList.Add(ItemTypeID.Train);
                typeList.Add(ItemTypeID.Persona);
                typeList.Add(ItemTypeID.HelpContent);
                ////

                ActiveItemTypeQuery query = new ActiveItemTypeQuery(ScheduleStore.Instance, null, typeList, ShouldRefresh);

                bool deferItemCreationforScheduleItems = ShouldRefresh == ShouldRefresh.Yes ? true : false;
                BackgroundTaskResult queryResult = ScheduleStore.Instance.ExecuteQuery(this, query, ShouldRefresh, taskWorker, deferItemCreationforScheduleItems);
                if (!CheckResults(taskWorker, e, queryResult))
                {
                    return;
                }

                // Query for bugs that users have filed against OPlanner
                //OPlannerBugsQuery bugsQuery = new OPlannerBugsQuery(ScheduleStore.Instance, ShouldRefresh);
                //queryResult = ScheduleStore.Instance.ExecuteQuery(this, bugsQuery, ShouldRefresh, taskWorker, deferItemCreationforScheduleItems);
                //if (!CheckResults(taskWorker, e, queryResult))
                //{
                //    return;
                //}

                CurrentProductGroup = GetItem<ProductGroupItem>(m_currentProductGroupKey);
                if (CurrentProductGroup != null)
                {
                    CurrentProductGroup.EnsureProductGroupMembers();
                }

                currentTask++;

                // Query for all pillars first
                taskWorker.ReportProgress((currentTask * 100) / totalTasks, "", "Opening the plan for your product group...");
                typeList.Clear();
                typeList.Add(ItemTypeID.Pillar);
                query = new ActiveItemTypeQuery(ScheduleStore.Instance, m_currentProductGroupKey, typeList, ShouldRefresh);
                queryResult = ScheduleStore.Instance.ExecuteQuery(this, query, ShouldRefresh, taskWorker, deferItemCreationforScheduleItems);
                if (!CheckResults(taskWorker, e, queryResult))
                {
                    return;
                }

                // Next query for all the planner items associated with this product group
                taskWorker.ReportProgress((currentTask * 100) / totalTasks, "", "Loading team member information...");
                typeList.Clear();
                typeList.Add(ItemTypeID.GroupMember);
                typeList.Add(ItemTypeID.ScrumTeam);
                typeList.Add(ItemTypeID.OffTime);

                query = new ActiveItemTypeQuery(ScheduleStore.Instance, m_currentProductGroupKey, typeList, ShouldRefresh);
                queryResult = ScheduleStore.Instance.ExecuteQuery(this, query, ShouldRefresh, taskWorker, deferItemCreationforScheduleItems);
                if (!CheckResults(taskWorker, e, queryResult))
                {
                    return;
                }

                currentTask++;
                OnScheduleMetadataReady();

                // Next, query for all Experiences, Features, and workItems owned by the product group
                List<string> groupMembers = Planner.Instance.GetMemberAliases();
                if (groupMembers.Count > 0)
                {
                    AsyncObservableCollection<TrainItem> currentTrains = GetQueryableTrains();
                    List<int> pillarPathIDs = GetAllPillarPathIDs();

                    // Get all experience items first, so they'll all be available once we query for features and 
                    // attempt to associate them with their experiences.
                    taskWorker.ReportProgress((currentTask * 100) / totalTasks, "", "Loading your team's experiences...");
                    typeList.Clear();
                    typeList.Add(ItemTypeID.Experience);

                    List<int> treeIds = new List<int>();
                    foreach (int pathID in pillarPathIDs)
                    {
                        treeIds.Add(pathID);
                    }

                    bool deferItemCreationForHostItems = DeferItemCreationForHostItems;
                    HostItemQuery hostQuery = new HostItemQuery(HostItemStore.Instance, typeList, groupMembers, treeIds, currentTrains, ShouldRefresh, RefreshType);
                    queryResult = HostItemStore.Instance.ExecuteQuery(this, hostQuery, ShouldRefresh, taskWorker, deferItemCreationForHostItems);
                    if (!CheckResults(taskWorker, e, queryResult))
                    {
                        return;
                    }

                    // Get all features next, so they'll all be available once we query for work items and 
                    // attempt to associate them with their parent features.
                    taskWorker.ReportProgress((currentTask * 100) / totalTasks, "", "Loading your team's features...");
                    typeList.Clear();
                    typeList.Add(ItemTypeID.BacklogItem);

                    hostQuery = new HostItemQuery(HostItemStore.Instance, typeList, groupMembers, treeIds, currentTrains, ShouldRefresh, RefreshType);
                    queryResult = HostItemStore.Instance.ExecuteQuery(this, hostQuery, ShouldRefresh, taskWorker, deferItemCreationForHostItems);
                    if (!CheckResults(taskWorker, e, queryResult))
                    {
                        return;
                    }

                    // Then get the work items.
                    taskWorker.ReportProgress((currentTask * 100) / totalTasks, "", "Loading your team's work items...");
                    typeList.Clear();
                    typeList.Add(ItemTypeID.WorkItem);

                    hostQuery = new HostItemQuery(HostItemStore.Instance, typeList, groupMembers, treeIds, currentTrains, ShouldRefresh, RefreshType);
                    queryResult = HostItemStore.Instance.ExecuteQuery(this, hostQuery, ShouldRefresh, taskWorker, deferItemCreationForHostItems);
                    if (!CheckResults(taskWorker, e, queryResult))
                    {
                        return;
                    }

                }

                e.Result = new BackgroundTaskResult { ResultType = ResultType.Completed };

            }

        }

        public AsyncObservableCollection<TrainItem> GetQueryableTrains()
        {
            AsyncObservableCollection<TrainItem> trainItems = TrainItems.ToCollection();
            trainItems.Insert(0, TrainItem.BacklogTrainItem);
            return trainItems;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        ///  Called when the PlannerQuery task kicked off by BeginPlannerQuery completes.
        /// </summary>
        //------------------------------------------------------------------------------------
        void PlannerQueryTask_Completed(object TaskArgs, BackgroundTaskResult result)
        {
            IsQueryInProgress = false;
            IsRefreshInProgress = false;

            if (result.ResultType == ResultType.Cancelled || result.ResultType == ResultType.Failed)
            {
                if (!Planner.Instance.IsStartupComplete)
                {
                    Planner.Shutdown();
                }
            }

            if (result.ResultType == ResultType.Completed)
            {
                if (ShouldRefresh == ShouldRefresh.Yes)
                {
                    Planner.Instance.LastRefreshTime = DateTime.Now;
                }
            }

            Planner.OnPlannerQueryCompleted(this, new PlannerQueryCompletedEventArgs(result, ShouldRefresh));
        }

        public void CancelQuery()
        {
            if (IsQueryInProgress && QueryTask != null)
            {
                QueryTask.CancelTask();
            }
        }

        public void BeginRefreshQuery(string productGroupKey)
        {
            RefreshType refreshType = PlannerNameSpace.RefreshType.QueryForChangedItems;
            BeginPlannerQuery(ShouldRefresh.Yes, productGroupKey, null, refreshType);
        }

        bool IsCancelled(BackgroundTask taskWorker, System.ComponentModel.DoWorkEventArgs e)
        {
            if (taskWorker.CancellationPending)
            {
                e.Cancel = true;
                e.Result = new BackgroundTaskResult { ResultType = ResultType.Cancelled };
                return true;
            }

            return false;
        }

        #endregion

        void OnScheduleMetadataReady()
        {
            if (ScheduleMetadataReady != null)
            {
                ScheduleMetadataReady(this, EventArgs.Empty);
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a list of all the defined Product Studio paths for all pillars associated
        /// with the currently open ProductGroup.
        /// </summary>
        //------------------------------------------------------------------------------------
        List<int> GetAllPillarPathIDs()
        {
            List<int> pillarPathIDs = new List<int>();
            AsyncObservableCollection<PillarItem> pillars = PillarItem.Items;
            foreach (PillarItem pillar in pillars)
            {
                pillarPathIDs.Add(pillar.PillarPathID);
            }

            return pillarPathIDs;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Given the ID of a node in the host store feature tree, return the PillarItem (if
        /// any) that owns that node (via of it's declared paths).  If no owner is found, null
        /// will be returned.
        /// </summary>
        //------------------------------------------------------------------------------------
        public PillarItem FindOwnerPillar(int treeID)
        {
            AsyncObservableCollection<PillarItem> pillars = PillarItem.Items;
            foreach (PillarItem pillar in pillars)
            {
                if (pillar.IsTreeIDUnderPillar(treeID))
                {
                    return pillar;
                }
            }

            return null;
        }

    }
}
