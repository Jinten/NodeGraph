using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NodeGraph.CommandParameters
{
    public class NodesMovedCommandParameter
    {
        public Guid[] NodeGuids { get; } = null;

        public NodesMovedCommandParameter(Guid[] nodeGuids)
        {
            NodeGuids = nodeGuids;
        }
    }

    public class StartMoveNodesCommandParameter
    {
        public Guid[] NodeGuids { get; } = null;

        public StartMoveNodesCommandParameter(Guid[] nodeGuids)
        {
            NodeGuids = nodeGuids;
        }
    }
}
