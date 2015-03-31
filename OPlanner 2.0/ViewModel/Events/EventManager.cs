using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Runtime.CompilerServices;

namespace PlannerNameSpace
{
    public delegate void RibbonButtonEventHandler();
    public delegate void StoreEventHandler();
    public delegate void UpdateProductGroupMembersEventHandler();
    public delegate void StoreItemEventHandler(StoreItem item);
    public delegate void PropertyEventHandler(object source, string propName);
    public delegate void GeneralUpdateHandler();

    public sealed class MyEventManager : DispatcherObject
    {
        // Events that any listener can suscribe to
        public event GeneralUpdateHandler CommitmentStatusComputationComplete;
        public event GeneralUpdateHandler ForecastingComputationComplete;
        public event ScrumTeamItemEventHandler ScrumTeamViewTeamSelectionChanged;

        public event RibbonButtonEventHandler CreateWorkItem;
        public event RibbonButtonEventHandler DeleteWorkItem;
        public event RibbonButtonEventHandler CreateBacklogItem;

        public event StoreEventHandler PlannerRefreshStarting;

        public event UpdateProductGroupMembersEventHandler ProductGroupMembersUpdateComplete;
        public event PropertyEventHandler PropertyChangedCanceled;
        public event GeneralUpdateHandler GotoItemCommand;
        public event GeneralUpdateHandler TabLoadStarting;

        public event EventHandler ScrumTeamCollectionChanged;
        public event EventHandler<StoreCommitCompleteEventArgs> StoreCommitComplete;

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Will be fired when start-up is fully complete, and the MainWindow is ready.
        /// </summary>
        //------------------------------------------------------------------------------------
        public event EventHandler ApplicationStartupComplete;

        public void OnApplicationStartupComplete()
        {
            if (ApplicationStartupComplete != null)
            {
                ApplicationStartupComplete(this, EventArgs.Empty);
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Fired when a planner query requested by a call to BeginPlannerQuery has completed.
        /// </summary>
        //------------------------------------------------------------------------------------
        public event EventHandler<PlannerQueryCompletedEventArgs> PlannerQueryCompleted;

        public void OnPlannerQueryCompleted(object sender, PlannerQueryCompletedEventArgs args)
        {
            if (PlannerQueryCompleted != null)
            {
                PlannerQueryCompleted(sender, args);
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Event fired if the default spec team name changes.
        /// </summary>
        //------------------------------------------------------------------------------------
        public event EventHandler DefaultSpecTeamNameChanged;

        public void OnDefaultSpecTeamNameChanged()
        {
            if (DefaultSpecTeamNameChanged != null)
            {
                DefaultSpecTeamNameChanged(this, EventArgs.Empty);
            }
        }

        public void OnScrumTeamCollectionChanged()
        {
            if (ScrumTeamCollectionChanged != null)
            {
                ScrumTeamCollectionChanged(this, EventArgs.Empty);
            }
        }

        public void OnTabLoadStarting()
        {
            if (TabLoadStarting != null)
            {
                TabLoadStarting();
            }
        }

        private void DispatchOnStoreCommitComplete(object sender, StoreCommitCompleteEventArgs e)
        {
            if (StoreCommitComplete != null)
            {
                StoreCommitComplete(sender, e);
            }
        }

        // A refresh of all planner data is starting
        public void OnPlannerRefreshStarting()
        {
            if (PlannerRefreshStarting != null)
            {
                PlannerRefreshStarting();
            }
        }

        // An async update of the current product group's members has been completed
        public void OnProductGroupMembersUpdateCompleted()
        {
            if (ProductGroupMembersUpdateComplete != null)
            {
                ProductGroupMembersUpdateComplete();
            }
        }

        // Create new work item
        public void OnCreateNewWorkItem()
        {
            if (CreateWorkItem != null)
            {
                CreateWorkItem();
            }
        }

        public void OnDeleteWorkItem()
        {
            if (DeleteWorkItem != null)
            {
                DeleteWorkItem();
            }
        }

        public void OnCreateBacklogItem()
        {
            if (CreateBacklogItem != null)
            {
                CreateBacklogItem();
            }
        }

        public void OnForecastingComputationComplete()
        {
            if (ForecastingComputationComplete != null)
            {
                ForecastingComputationComplete();
            }
        }

        public void OnCommitmentStatusComputationComplete()
        {
            if (CommitmentStatusComputationComplete != null)
            {
                CommitmentStatusComputationComplete();
            }
        }

        public void OnScrumTeamViewTeamSelectionChanged(object sender, ScrumTeamItem currentItem)
        {
            if (ScrumTeamViewTeamSelectionChanged != null)
            {
                ScrumTeamViewTeamSelectionChanged(sender, new ScrumTeamChangedEventArgs(currentItem));
            }
        }

        public void OnPropertyChangedCanceled(object source, [CallerMemberName] String publicPropName = "")
        {
            if (PropertyChangedCanceled != null)
            {
                PropertyChangedCanceled(source, publicPropName);
            }
        }

        public void OnGotoItemCommand()
        {
            if (GotoItemCommand != null)
            {
                GotoItemCommand();
            }
        }

    }
}
