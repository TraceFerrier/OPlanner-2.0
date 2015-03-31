using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerNameSpace
{
    public class InvalidHierarchicalItemEvent : EventArgs
    {
        public StoreItem InvalidItem { get; set; }

        public InvalidHierarchicalItemEvent(StoreItem invalidItem)
        {
            InvalidItem = invalidItem;
        }
    }
}
