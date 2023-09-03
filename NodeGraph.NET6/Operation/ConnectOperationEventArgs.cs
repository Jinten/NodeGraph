using System;

namespace NodeGraph.NET6.Operation
{
    public class ConnectedOperationEventArgs : EventArgs
    {
        public Guid InputNodeGuid { get; } = Guid.Empty;
        public Guid InputConnectorGuid { get; } = Guid.Empty;
        public Guid OutputNodeGuid { get; } = Guid.Empty;
        public Guid OutputConnectorGuid { get; } = Guid.Empty;

        public ConnectedOperationEventArgs(
            Guid inputNodeGuid,
            Guid inputConnectorGuid,
            Guid outputNodeGuid,
            Guid outputConnectorGuid)
        {
            InputNodeGuid = inputNodeGuid;
            InputConnectorGuid = inputConnectorGuid;
            OutputNodeGuid = outputNodeGuid;
            OutputConnectorGuid = outputConnectorGuid;
        }
    }
}
