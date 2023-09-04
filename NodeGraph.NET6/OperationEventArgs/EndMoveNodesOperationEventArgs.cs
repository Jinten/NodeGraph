using System;

namespace NodeGraph.NET6.OperationEventArgs
{
    public class EndMoveNodesOperationEventArgs : EventArgs
    {
        public Guid[] NodeGuids { get; } = null!;

        public EndMoveNodesOperationEventArgs(Guid[] nodeGuids)
        {
            NodeGuids = nodeGuids;
        }
    }
}
