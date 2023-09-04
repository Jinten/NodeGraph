using System;

namespace NodeGraph.NET7.OperationEventArgs
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
