using ProductStudio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerNameSpace.Model
{
    public interface IRepository
    {
        void ReceiveDSItems(Datastore store, DatastoreItems items, ShouldRefresh isRefresh, bool isDefer);
    }
}
