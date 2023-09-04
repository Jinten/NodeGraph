using System;

namespace NodeGraph.NET6.Operation
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
