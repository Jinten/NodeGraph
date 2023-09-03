using System;

namespace NodeGraph.NET6.Operation
{
    public class ConnectedLinkOperationEventArgs : EventArgs
    {
        public Guid OutputConnectorGuid { get; } = Guid.Empty;
        public Guid OutputConnectorNodeGuid { get; } = Guid.Empty;
        public Guid InputConnectorGuid { get; } = Guid.Empty;
        public Guid InputConnectorNodeGuid { get; } = Guid.Empty;

        public ConnectedLinkOperationEventArgs(
            Guid outputConnectorGuid,
            Guid outputConnectorNodeGuid,
            Guid inputConnectorGuid,
            Guid inputConnectorNodeGuid)
        {
            InputConnectorGuid = inputConnectorGuid;
            InputConnectorNodeGuid = inputConnectorNodeGuid;
            OutputConnectorGuid = outputConnectorGuid;
            OutputConnectorNodeGuid = outputConnectorNodeGuid;
        }
    }
}
