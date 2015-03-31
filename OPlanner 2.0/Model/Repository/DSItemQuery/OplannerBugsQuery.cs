using System.Collections.Generic;
using System;

namespace PlannerNameSpace
{
    public class OPlannerBugsQuery : BaseQuery
    {
        ShouldRefresh ShouldRefresh { get; set; }

        public OPlannerBugsQuery(Datastore store, ShouldRefresh isRefresh)
            : base(store)
        {
            ShouldRefresh = isRefresh;
        }

        protected override void BuildQueryXML()
        {
            BeginQuery();

            BeginAndGroup();
            //AddClause(Store.PropNameType, "Equals", Store.GetItemTypeKey(ItemTypeID.PlannerBug));

            if (ShouldRefresh == ShouldRefresh.Yes)
            {
                DateTime lastRefreshTime = Planner.Instance.LastRefreshTime;
                string lastRefreshDate = lastRefreshTime.ToShortDateString();
                AddClause(Datastore.PropNameChangedDate, "equalsGreater", lastRefreshDate);
            }
            
            EndGroup();

            EndQuery();
        }
    }
}
