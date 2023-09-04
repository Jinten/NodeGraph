using System;

namespace NodeGraph.NET7.Operation
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
