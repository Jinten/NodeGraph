using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NodeGraph.OperationEventArgs
{
    public class NodesMovedOperationEventArgs : EventArgs
    {
        public Guid[] NodeGuids { get; } = null;

        public NodesMovedOperationEventArgs(Guid[] nodeGuids)
        {
            NodeGuids = nodeGuids;
        }
    }
}
