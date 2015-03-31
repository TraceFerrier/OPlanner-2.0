using FirstFloor.ModernUI.Windows.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerNameSpace
{
    public partial class Planner
    {
        //------------------------------------------------------------------------------------
        /// <summary>
        /// Will be fired when start-up is fully complete, and the MainWindow is ready.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static event EventHandler ApplicationStartupComplete;

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Fired when a planner query requested by a call to BeginPlannerQuery has completed.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static event EventHandler<PlannerQueryCompletedEventArgs> PlannerQueryCompleted;

        public static void OnPlannerQueryCompleted(object sender, PlannerQueryCompletedEventArgs args)
        {
            if (PlannerQueryCompleted != null)
            {
                PlannerQueryCompleted(sender, args);
            }
        }

        public static event EventHandler<ChangeListUpdatedEventArgs> ChangeListUpdated;

        public static void OnChangeListUpdated(object sender, ChangeListUpdatedEventArgs args)
        {
            if (ChangeListUpdated != null)
            {
                ChangeListUpdated(sender, args);
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Event fired if the default spec team name changes.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static event EventHandler DefaultSpecTeamNameChanged;

        // The Store Commit operation has completed
        public static void OnStoreCommitComplete(object sender, StoreCommitCompleteEventArgs e)
        {
            Planner.Instance.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => DispatchOnStoreCommitComplete(sender, e)));
        }

        public static event EventHandler<StoreCommitCompleteEventArgs> StoreCommitComplete;

        private static void DispatchOnStoreCommitComplete(object sender, StoreCommitCompleteEventArgs e)
        {
            if (StoreCommitComplete != null)
            {
                StoreCommitComplete(sender, e);
            }
        }

        public static event EventHandler DiscoveryComplete;
        public static void OnDiscoveryComplete(object sender)
        {
            if (DiscoveryComplete != null)
            {
                DiscoveryComplete(sender, EventArgs.Empty);
            }
        }

        public static event EventHandler<InvalidHierarchicalItemEvent> InvalidHierarchicalItemDetected;
        public static void OnInvalidHierarchicalItemEvent(object sender, StoreItem invalidItem)
        {
            if (InvalidHierarchicalItemDetected != null)
            {
                InvalidHierarchicalItemDetected(sender, new InvalidHierarchicalItemEvent(invalidItem));
            }
        }
    }
}
