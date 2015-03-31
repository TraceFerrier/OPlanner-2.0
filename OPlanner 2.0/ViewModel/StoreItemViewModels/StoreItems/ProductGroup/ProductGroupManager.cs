using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace PlannerNameSpace
{
    public sealed class ProductGroupManager
    {
        private static readonly ProductGroupManager m_instance = new ProductGroupManager();

        public static ProductGroupManager Instance
        {
            get { return m_instance; }
        }

        public ProductGroupManager()
        {
        }

    }
}
