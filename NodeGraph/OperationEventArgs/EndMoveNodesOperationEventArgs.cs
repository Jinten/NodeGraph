using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NodeGraph.OperationEventArgs
{
    public class EndMoveNodesOperationEventArgs : EventArgs
    {
        public Guid[] NodeGuids { get; } = null;

        public EndMoveNodesOperationEventArgs(Guid[] nodeGuids)
        {
            NodeGuids = nodeGuids;
        }
    }
}
