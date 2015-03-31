using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductStudio;
using PlannerNameSpace.Model;

namespace PlannerNameSpace
{
    public abstract class PSEvaluation2Store : Datastore
    {
        public static class GroupMemberFields
        {
            public static readonly int IdxJobTitle = 0;
            public static readonly int IdxPillarKey = 1;
            public static readonly int IdxAvgCapacity = 2;
        }

        public PSEvaluation2Store(StoreType storeType)
            : base(storeType)
        {
        }

        public override string StoreName { get { return "PSEvaluation2"; } }
        public override int TeamRootNode { get { return 0; } }
        public override int TeamRootDepth { get { return 1; } }

        public override string PropNameType { get { return "Source ID"; } }
        public override string PropSubTypeName { get { return null; } }
        protected override void InitializeProperties()
        {
            // Notes: Scenario field is bound to a list of allowed values if the Milestone field is set
            // so it can't be used as a freeform field if the milestone field is used.

            AddItemType(ItemTypeID.ProductGroup, "{81653854-355E-43e8-8F1E-89AC8165CF36}", Constants.c_Any);
            AddItemType(ItemTypeID.Train, "{426247A7-A844-45BB-8972-D332CFD2EE7C}", Constants.c_Any);
            AddItemType(ItemTypeID.GroupMember, "{4B06A238-EBB6-457f-A105-4A085CF5261A}", Constants.c_Any);
            AddItemType(ItemTypeID.OffTime, "{57BA61BD-D51E-4A92-B571-A2ABB08C5B79}", Constants.c_Any);
            AddItemType(ItemTypeID.Pillar, "{8676B0C9-F000-4a07-AB3E-578DCD36C876}", Constants.c_Any);
            AddItemType(ItemTypeID.ScrumTeam, "{8DA2B78D-A6C9-41c0-BFBE-A27F83F292B4}", Constants.c_Any);
            AddItemType(ItemTypeID.Persona, "{A7408746-F262-4876-A03F-858743A1A77E}", Constants.c_Any);
            AddItemType(ItemTypeID.PlannerBug, "{56E286FA-A624-4416-BBD9-A4058E04CAD7}", Constants.c_Any);
            AddItemType(ItemTypeID.HelpContent, "{BF1E0494-9F02-4C7D-B8C3-9C78481BC855}", Constants.c_Any);

            // Until we have a Schedule Database with a schema that defines the allowed
            // values for the following properties, set the proposed allowed values directly.
            SetFieldAllowedValues(Datastore.PropNameDiscipline, new AllowedValue { Value = DisciplineValues.Dev });
            SetFieldAllowedValues(Datastore.PropNameDiscipline, new AllowedValue { Value = DisciplineValues.Test });
            SetFieldAllowedValues(Datastore.PropNameDiscipline, new AllowedValue { Value = DisciplineValues.PM });
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Thanks to the small number of fields in this store, we persist some values for
        /// some StoreItems as composite values.
        /// </summary>
        //------------------------------------------------------------------------------------
        protected override void RegisterCompositeValues(CompositeValueRegistry registry)
        {
            // GroupMemberItem
            registry.RegisterCompositeValue(Datastore.PropNameJobTitlePillarAndAvgCapacity, StringUtils.GetPropertyName((GroupMemberItem s) => s.JobTitle), GroupMemberFields.IdxJobTitle);
            registry.RegisterCompositeValue(Datastore.PropNameJobTitlePillarAndAvgCapacity, StringUtils.GetPropertyName((GroupMemberItem s) => s.PillarItemKey), GroupMemberFields.IdxPillarKey);
            registry.RegisterCompositeValue(Datastore.PropNameJobTitlePillarAndAvgCapacity, StringUtils.GetPropertyName((GroupMemberItem s) => s.CapacityPerDay), GroupMemberFields.IdxAvgCapacity);

            // ProductGroupItem
            registry.RegisterCompositeValue(Datastore.PropNameProductGroupComposite, StringUtils.GetPropertyName((ProductGroupItem s) => s.DefaultSpecTeamName), 0);
            registry.RegisterCompositeValue(Datastore.PropNameProductGroupComposite, StringUtils.GetPropertyName((ProductGroupItem s) => s.GroupAdmin1), 1);
            registry.RegisterCompositeValue(Datastore.PropNameProductGroupComposite, StringUtils.GetPropertyName((ProductGroupItem s) => s.GroupAdmin2), 2);
            registry.RegisterCompositeValue(Datastore.PropNameProductGroupComposite, StringUtils.GetPropertyName((ProductGroupItem s) => s.GroupAdmin3), 3);
            registry.RegisterCompositeValue(Datastore.PropNameProductGroupComposite, StringUtils.GetPropertyName((ProductGroupItem s) => s.HostItemStoreName), 4);

        }

        public override string DefaultTeamTreePath { get { return "\\test\\"; } }
        public override string DefaultMemberListTreePath { get { return "\\test\\"; } }
        public override string DefaultMilestoneTreePath { get { return "\\test\\"; } }
        public override string DefaultSprintTreePath { get { return "\\test\\"; } }
        public override string DefaultTaskFolderTreePath { get { return "\\test\\"; } }

        public override void InitializeRequiredFieldValues(StoreItem item)
        {
            item.SetStringValue("Issue type", "Spec Issue", "Issue type");
            item.Severity = "2";
            item.OpenedBy = Planner.Instance.CurrentUserAlias;
            item.OpenedDate = DateTime.Now;
        }
    }
}
