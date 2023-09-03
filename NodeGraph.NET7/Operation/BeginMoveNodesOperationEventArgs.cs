using System;

namespace NodeGraph.NET7.Operation
{
    public class BeginMoveNodesOperationEventArgs : EventArgs
    {
        public Guid[] NodeGuids { get; } = null!;

        public BeginMoveNodesOperationEventArgs(Guid[] nodeGuids)
        {
            NodeGuids = nodeGuids;
        }
    }
}
