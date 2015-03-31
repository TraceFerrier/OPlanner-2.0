using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlannerNameSpace.Model;

namespace PlannerNameSpace
{
    public abstract class OfficeMainStore : Datastore
    {
        public OfficeMainStore(StoreType storeType)
            : base(storeType)
        {
        }

        protected enum LocalPropID
        {
            Fix_By,
            Custom1,
            Custom2,
            Ship_Cycle,
            SubFixBy,
            User_Scenario,
            Explain_Defect,
            Custom5,
        }

        public bool IsCloneStore
        {
            get
            {
                return Planner.Instance.UserPreferences.ShouldUseCloneHostStore;
            }
        }

        public override string StoreName
        {
            get
            {
                if (IsCloneStore)
                {
                    return "Office Main Clone";
                }
                else
                {
                    return "OfficeMain";
                }
            }
        }

        public override int TeamRootNode { get { return 1; } }
        public override int TeamRootDepth { get { return 2; } }
        public override string PropNameType { get { return "Type"; } }
        public override string PropSubTypeName { get { return "Subtype"; } }
        protected override void InitializeProperties()
        {
            AddItemType(ItemTypeID.Experience, "Feature Crew", "Experience");
            AddItemType(ItemTypeID.BacklogItem, "Feature Crew", "Feature");
            AddItemType(ItemTypeID.WorkItem, "Work Item", Constants.c_Any);
        }

        public override void InitializeRequiredFieldValues(StoreItem item)
        {
            item.Severity = "2";
            item.OpenedBy = Planner.Instance.CurrentUserAlias;
            item.OpenedDate = DateTime.Now;
        }

        public override string DefaultTeamTreePath { get { return "\\test\\"; } }
        public override string DefaultMemberListTreePath { get { return "\\test\\"; } }
        public override string DefaultMilestoneTreePath { get { return "\\test\\"; } }
        public override string DefaultSprintTreePath { get { return "\\test\\"; } }
        public override string DefaultTaskFolderTreePath { get { return "\\test\\"; } }
    }
}
