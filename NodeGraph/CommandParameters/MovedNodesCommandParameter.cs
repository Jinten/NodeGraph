using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NodeGraph.CommandParameters
{
    public class MovedNodesCommandParameter
    {
        public Guid[] NodeGuids { get; } = null;

        public MovedNodesCommandParameter(Guid[] nodeGuids)
        {
            NodeGuids = nodeGuids;
        }
    }
}
