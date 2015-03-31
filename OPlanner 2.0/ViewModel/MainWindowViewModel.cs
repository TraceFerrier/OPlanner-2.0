using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerNameSpace.ViewModel
{
    public class MainWindowViewModel : BasePropertyChanged
    {
        private int m_changesToSave;

        public MainWindowViewModel()
        {
            ChangesToSave = 0;
            Planner.ChangeListUpdated += Planner_ChangeListUpdated;
        }

        public int ChangesToSave
        {
            get { return m_changesToSave; }
            set { m_changesToSave = value; NotifyPropertyChangedByName(); }
        }

        public string CurrentProductGroup
        {
            get { return Planner.ProductGroupName; }
        }

        void Planner_ChangeListUpdated(object sender, ChangeListUpdatedEventArgs e)
        {
            ChangesToSave = e.ChangesToSave;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// SaveChanges command
        /// </summary>
        //------------------------------------------------------------------------------------
        public void SaveChanges()
        {
            Planner.Instance.ItemRepository.CommitChanges();
        }

        public bool CanSaveChanges(object parameter)
        {
            return ChangesToSave > 0;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Refresh command
        /// </summary>
        //------------------------------------------------------------------------------------
        public void RefreshModelData()
        {
            Planner.Instance.ItemRepository.Refresh();
        }

        public bool CanRefreshModelData(object parameter)
        {
            return true;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// UndoChanges command
        /// </summary>
        //------------------------------------------------------------------------------------
        public void UndoChanges()
        {
            Planner.Instance.ItemRepository.UndoChanges();
        }

        public bool CanUndoChanges(object parameter)
        {
            return CanSaveChanges(parameter);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// ShowChangeListViewer command
        /// </summary>
        //------------------------------------------------------------------------------------
        static ChangeListWindow ChangeListWindow = null;
        public void ShowChangeListViewer()
        {
            if (ChangeListWindow != null)
            {
                if (!ChangeListWindow.IsVisible)
                {
                    ChangeListWindow.Show();
                }
                ChangeListWindow.BringIntoView();
            }
            else
            {
                ChangeListWindow = new ChangeListWindow();
                ChangeListWindow.Closed += ChangeListWindow_Closed;
                ChangeListWindow.Show();
            }
        }

        public bool CanShowChangeListViewer(object parameter)
        {
            return true;
        }

        void ChangeListWindow_Closed(object sender, EventArgs e)
        {
            ChangeListWindow = null;
        }

    }
}
