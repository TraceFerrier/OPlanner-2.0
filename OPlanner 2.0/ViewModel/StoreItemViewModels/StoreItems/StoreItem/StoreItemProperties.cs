using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerNameSpace
{
    public partial class StoreItem
    {
        public int ID
        {
            get
            {
                m_id = GetIntValue(Datastore.PropNameID);
                return m_id;
            }
            set
            {
                int oldValue = ID;
                SetIntValue(Datastore.PropNameID, value);

                if (oldValue != value)
                {
                    Planner.Instance.ItemRepository.OnStoreItemIDChange(this, oldValue, value);
                }
            }
        }

        public string Title
        {
            get
            {
                m_title = GetStringValue(Datastore.PropNameTitle);
                return m_title;
            }
            set
            {
                m_title = value;
                SetStringValue(Datastore.PropNameTitle, m_title);
                NotifyPropertyChanged(() => IDQualifiedTitle);
            }
        }

        string m_treePath;
        public string TreePath
        {
            get
            {
                if (m_treePath == null)
                {
                    m_treePath = HostItemStore.Instance.GetTreePath(TreeID, ProductTreeFormat.ExcludeProduct);
                }

                return m_treePath;
            }
        }

        public virtual int TreeID
        {
            get { return GetIntValue(Datastore.PropNameTreeID); }
            set
            {
                SetIntValue(Datastore.PropNameTreeID, value);

                m_treePath = null;
                NotifyPropertyChanged(() => TreePath);
            }
        }

        public virtual string AssignedTo
        {
            get
            {
                return GetStringValue(Datastore.PropNameAssignedTo);
            }

            set
            {
                SetStringValue(Datastore.PropNameAssignedTo, value);

                m_productTeamAssignedTo = null;
                NotifyPropertyChanged(() => ProductTeamAssignedTo);
            }
        }

        public bool IsActive
        {
            get { return StringUtils.StringsMatch(Status, StatusValues.Active); }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Refreshes the status and resolution from the backing store, and then returns 
        /// true if this item is in the deleted state.
        /// </summary>
        //------------------------------------------------------------------------------------
        public bool IsDeletedOnRefresh
        {
            get
            {
                string backingStatus = GetRefreshedBackingStringValue(() => Status);

                // Force Resolution into memory if it's not already there, then read the latest
                // value from the store
                string currentResolution = Resolution;
                string backingResolution = GetRefreshedBackingStringValue(() => Resolution);
                if (!StringUtils.StringsMatch(backingStatus, StatusValues.Active) && !StringUtils.StringsMatch(backingResolution, ResolutionType.Fixed))
                {
                    return true;
                }

                return false;
            }
        }

        public bool IsFixed
        {
            get { return IsResolved || IsClosed; }
        }

        public bool IsResolved
        {
            get { return StringUtils.StringsMatch(Status, StatusValues.Resolved) && StringUtils.StringsMatch(Resolution, ResolutionType.Fixed); }
        }

        public bool IsResolvedAnyResolution
        {
            get { return StringUtils.StringsMatch(Status, StatusValues.Resolved); }
        }

        public bool IsClosed
        {
            get { return StringUtils.StringsMatch(Status, StatusValues.Closed) && StringUtils.StringsMatch(Resolution, ResolutionType.Fixed); }
        }

        public bool IsClosedAnyResolution
        {
            get { return StringUtils.StringsMatch(Status, StatusValues.Closed); }
        }

        public virtual string Status
        {
            get
            {
                return GetStringValue(Datastore.PropNameStatus);
            }

            set
            {
                SetStringValue(Datastore.PropNameStatus, value);
            }
        }

        public string ParentProductGroupKey
        {
            get { return GetStringValue(Datastore.PropNameParentProductGroupKey); }
            set { SetStringValue(Datastore.PropNameParentProductGroupKey, value); }
        }

        public string SubStatus
        {
            get { return GetStringValue(Datastore.PropNameSubStatus); }
            set { SetStringValue(Datastore.PropNameSubStatus, value); }
        }

        public string Resolution
        {
            get { return GetStringValue(Datastore.PropNameResolution); }
            set { SetStringValue(Datastore.PropNameResolution, value); }
        }

        public string Severity
        {
            get { return GetStringValue(Datastore.PropNameSeverity); }
            set { SetStringValue(Datastore.PropNameSeverity, value); }
        }

        public string ShipCycle
        {
            get { return GetStringValue(Datastore.PropNameShipCycle); }
            set { SetStringValue(Datastore.PropNameShipCycle, value); }
        }

        public virtual string FixBy
        {
            get { return GetStringValue(Datastore.PropNameFixBy); }
            set { SetStringValue(Datastore.PropNameFixBy, value); }
        }

        public virtual string Subtype
        {
            get { return GetStringValue(Datastore.PropNameSubtype); }
            set
            {
                PreviousSubtype = Subtype;
                SetStringValue(Datastore.PropNameSubtype, value);
            }
        }

        public DateTime OpenedDate
        {
            get { return TypeUtils.GetValueAsLocalTime(GetDateValue(Datastore.PropNameOpenedDate)); }
            set { SetDateValue(Datastore.PropNameOpenedDate, value); }
        }

        public string OpenedDateShort
        {
            get { return OpenedDate.ToShortDateString(); }
        }

        public DateTime LastChangedDate
        {
            get { return TypeUtils.GetValueAsLocalTime(GetDateValue(Datastore.PropNameChangedDate)); }
            set { SetDateValue(Datastore.PropNameChangedDate, value); }
        }

        public string OpenedBy
        {
            get { return GetStringValue(Datastore.PropNameOpenedBy); }
            set { SetStringValue(Datastore.PropNameOpenedBy, value); }
        }

        public virtual int BusinessRank
        {
            get { return GetIntValue(Datastore.PropNameBusiness_Rank); }
            set { SetIntValue(Datastore.PropNameBusiness_Rank, value); }
        }

        public virtual TrainItem ParentTrainItem
        {
            get { return null; }
            set { ;}
        }

    }
}
