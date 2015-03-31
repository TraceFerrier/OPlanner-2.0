using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerNameSpace
{
    public class PlannerQueryCompletedEventArgs : EventArgs
    {
        public BackgroundTaskResult Result { get; set; }
        public ShouldRefresh ShouldRefresh { get; set; }

        public PlannerQueryCompletedEventArgs(BackgroundTaskResult result, ShouldRefresh shouldRefresh)
        {
            Result = result;
            ShouldRefresh = shouldRefresh;
        }
    }
}
