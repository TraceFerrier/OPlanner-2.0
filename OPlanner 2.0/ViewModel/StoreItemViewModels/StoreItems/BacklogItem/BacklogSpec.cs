using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerNameSpace
{
    public partial class BacklogItem
    {
        public string ParentSpec
        {
            get
            {
                string spec = null;
                if (IsSpecdInBacklog)
                {
                    spec = Constants.c_SpecdInBacklog;
                }
                else
                {
                    spec = GetStringValue(Datastore.PropNameParentSpec);
                }

                if (!string.IsNullOrWhiteSpace(spec))
                {
                    if (spec.StartsWith("(No Spec"))
                    {
                        spec = Constants.c_SpecTBD;
                    }
                }
                else
                {
                    spec = Constants.c_SpecTBD;
                }

                return spec;
            }

            set
            {
                if (value == Constants.c_SpecdInBacklog)
                {
                    IsSpecdInBacklog = true;
                }
                else if (value == Constants.c_SpecTBD)
                {
                    IsSpecdInBacklog = false;
                    if (IsSpecSet)
                    {
                        SetStringValue(Datastore.PropNameParentSpec, null);
                    }
                }
                else
                {
                    SetStringValue(Datastore.PropNameParentSpec, value);
                }

                NotifyPropertyChangedByName();
            }
        }

        public string StoreSpecStatusText
        {
            get
            {
                string specStatus = GetStringValue(Datastore.PropNameParentSpecStatus);
                if (string.IsNullOrWhiteSpace(specStatus))
                {
                    return Constants.c_NotSet;
                }
                else
                {
                    return specStatus;
                }
            }

            set
            {
                if (value != null)
                {
                    StoreSpecStatusValue specStatusValue = StoreSpecStatus.GetStoreSpecStatus(value);
                    if (specStatusValue == StoreSpecStatusValue.ReadyForCoding || specStatusValue == StoreSpecStatusValue.SpecFinalized)
                    {
                        if (!IsSpecSet)
                        {
                            //UserMessage.Show("The Spec Status of a Backlog Item cannot be set to 'Ready for Coding' or 'Spec Finalized' unless the Spec is specified as either a valid spec for your product group, set to 'No Spec Required', or set to 'Spec'd in Backlog'.");
                            //NotifyPropertyChanged();
                            //return;
                        }
                    }

                    if (value == Constants.c_noSpecRequired)
                    {
                        value = Constants.c_SpecTBD;
                    }

                    if (value == Constants.c_NotSet)
                    {
                        value = null;
                    }

                    SetStringValue(Datastore.PropNameParentSpecStatus, value);
                }
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the value that selects for which team you want to picks specs from.
        /// This value determines what set of values is returned from TeamSpecValues.
        /// </summary>
        //------------------------------------------------------------------------------------
        public string SpecTeam
        {
            get
            {
                string specTeam = GetStringValue(Datastore.PropNameBacklogSpecTeam);
                if (string.IsNullOrWhiteSpace(specTeam))
                {
                    return Planner.Instance.DefaultSpecTeamName;
                }

                return specTeam;
            }

            set
            {
                SetStringValue(Datastore.PropNameBacklogSpecTeam, value);
                NotifyPropertyChanged(() => TeamSpecValues);
                NotifyPropertyChanged(() => ParentSpec);
            }
        }

        public static AsyncObservableCollection<AllowedValue> m_SpecTeamListWithNone;
        public static AsyncObservableCollection<AllowedValue> m_SpecTeamListWithoutNone;

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the list of team names that allow selection of which team to return a
        /// collection of specs for.
        /// </summary>
        //------------------------------------------------------------------------------------
        public AsyncObservableCollection<AllowedValue> SpecTeamList
        {
            get
            {
                if (Planner.Instance.DefaultSpecTeamName == Constants.c_NoneSpecTeamName)
                {
                    if (m_SpecTeamListWithNone == null)
                    {
                        BuildSpecTeamLists();
                    }

                    return m_SpecTeamListWithNone;
                }
                else
                {

                }

                if (m_SpecTeamListWithoutNone == null)
                {
                    BuildSpecTeamLists();
                }

                return m_SpecTeamListWithoutNone;
            }
        }

        private void BuildSpecTeamLists()
        {
            m_SpecTeamListWithNone = HostItemStore.Instance.GetFieldAllowedValues(this, Datastore.PropNameBacklogSpecTeam);
            m_SpecTeamListWithoutNone = m_SpecTeamListWithNone.ToCollection();
            foreach (AllowedValue value in m_SpecTeamListWithoutNone)
            {
                if ((string)value.Value == Constants.c_NoneSpecTeamName)
                {
                    m_SpecTeamListWithoutNone.Remove(value);
                    break;
                }
            }

        }

        private static Dictionary<string, AsyncObservableCollection<AllowedValue>> m_specsByTeam;

        public AsyncObservableCollection<AllowedValue> TeamSpecValues
        {
            get
            {
                if (m_specsByTeam == null)
                {
                    m_specsByTeam = new Dictionary<string, AsyncObservableCollection<AllowedValue>>();
                }

                string team = SpecTeam;

                if (!m_specsByTeam.ContainsKey(team))
                {
                    AsyncObservableCollection<AllowedValue> specs;
                    if (team != Constants.c_None)
                    {
                        specs = HostItemStore.Instance.GetDependentFieldAllowedValues(Datastore.PropNameParentSpec, Datastore.PropNameBacklogSpecTeam, team);
                    }
                    else
                    {
                        specs = new AsyncObservableCollection<AllowedValue>();
                    }

                    specs.Insert(0, new AllowedValue(Constants.c_SpecdInBacklog));
                    specs.Insert(1, new AllowedValue(Constants.c_SpecTBD));

                    foreach (AllowedValue specValue in specs)
                    {
                        string spec = specValue.Value as string;
                        if (!string.IsNullOrWhiteSpace(spec))
                        {
                            if (spec.StartsWith("(No Spec"))
                            {
                                specs.Remove(specValue);
                                break;
                            }
                        }
                    }

                    m_specsByTeam.Add(team, specs);

                }

                return m_specsByTeam[team];
            }
        }

        public static void RefreshCommonValues()
        {
            m_specStatusValues = null;
            m_postMortemStatusValues = null;
        }

        static AsyncObservableCollection<AllowedValue> m_specStatusValues;
        public AsyncObservableCollection<AllowedValue> SpecStatusValues
        {
            get
            {
                if (IsSpecTBD)
                {
                    AsyncObservableCollection<AllowedValue> specStatusValues = new AsyncObservableCollection<AllowedValue>();
                    specStatusValues.Add(new AllowedValue(Constants.c_NotSet));
                    return specStatusValues;
                }

                else if (m_specStatusValues == null)
                {
                    //try
                    {
                        m_specStatusValues = new AsyncObservableCollection<AllowedValue>();
                        m_specStatusValues = HostItemStore.Instance.GetFieldAllowedValues(this, Datastore.PropNameParentSpecStatus);
                        m_specStatusValues.Insert(0, new AllowedValue(Constants.c_NotSet));
                        foreach (AllowedValue statusValue in m_specStatusValues)
                        {
                            if (statusValue.Value.ToString().Contains(StoreSpecStatusTextValues.SpecNotNeeded))
                            {
                                m_specStatusValues.Remove(statusValue);
                                break;
                            }
                        }
                    }

                    //catch (Exception exception)
                    //{
                    //    Planner.ApplicationManager.HandleException(exception);
                    //    m_specStatusValues = new AsyncObservableCollection<AllowedValue>();
                    //}
                }

                return m_specStatusValues;
            }
        }

        public static bool IsSpecValid(string spec)
        {
            if (string.IsNullOrWhiteSpace(spec) || spec.StartsWith("(No Spec"))
            {
                return false;
            }

            return true;
        }

        public bool IsSpecSet
        {
            get
            {
                string parentSpec = ParentSpec;
                return IsSpecValid(parentSpec);
            }
        }

        public bool IsSpecReadyForCoding
        {
            get
            {
                if (IsSpecSet)
                {
                    StoreSpecStatusValue specStatusValue = StoreSpecStatus.GetStoreSpecStatus(StoreSpecStatusText);
                    if (specStatusValue == StoreSpecStatusValue.ReadyForCoding || specStatusValue == StoreSpecStatusValue.SpecFinalized)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        static AsyncObservableCollection<AllowedValue> m_postMortemStatusValues;
        public AsyncObservableCollection<AllowedValue> PostMortemStatusValues
        {
            get
            {
                if (m_postMortemStatusValues == null)
                {
                    m_postMortemStatusValues = HostItemStore.Instance.GetFieldAllowedValues(this, Datastore.PropNamePostMortemStatus);
                    m_postMortemStatusValues.Insert(0, new AllowedValue(Constants.c_NotSet));
                }

                return m_postMortemStatusValues;
            }
        }

        public bool IsPostMortemSpecAttached
        {
            get
            {
                try
                {
                    if (AttachedFilesCount == 0)
                    {
                        return false;
                    }
                }
                catch (Exception exception)
                {
                    Planner.Instance.HandleException(exception);
                    return false;
                }

                return GetBoolValue(Datastore.PropNameIsPostMortemSpecAttached);
            }

            set { SetBoolValue(Datastore.PropNameIsPostMortemSpecAttached, value); }
        }

        public string PostMortemStatus
        {
            get { return GetNotSetStringValue(Datastore.PropNamePostMortemStatus); }
            set
            {
                SetNotSetStringValue(Datastore.PropNamePostMortemStatus, value);
                NotifyPropertyChanged(() => IsPostMortemIssue);
            }
        }

        public string SpecStatusComments
        {
            get { return GetStringValue(Datastore.PropNameParentSpecStatusComments); }
            set { SetStringValue(Datastore.PropNameParentSpecStatusComments, value); }
        }

        public string SpecLink
        {
            get { return GetStringValue(Datastore.PropNameParentSpecLink); }
            set { SetStringValue(Datastore.PropNameParentSpecLink, value); }
        }

        public bool IsSpecTBD
        {
            get
            {
                return StringUtils.StringsMatch(ParentSpec, Constants.c_SpecTBD);
            }
        }

        public bool IsSpecdInBacklog
        {
            get { return GetBoolValue(Datastore.PropNameSpecdInBacklog); }
            set { SetBoolValue(Datastore.PropNameSpecdInBacklog, value); }
        }


    }
}
