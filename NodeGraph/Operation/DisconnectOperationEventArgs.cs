using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeGraph.OperationEventArgs
{
    public class DisconnectedOperationEventArgs : EventArgs
    {
        public Guid NodeLinkGuid { get; } = Guid.Empty;
        public Guid OutputConnectorGuid { get; } = Guid.Empty;
        public Guid OutputConnectorNodeGuid { get; } = Guid.Empty;
        public Guid InputConnectorGuid { get; } = Guid.Empty;
        public Guid InputConnectorNodeGuid { get; } = Guid.Empty;

        public DisconnectedOperationEventArgs(
            Guid nodeLinkGuid,
            Guid outputConnectorGuid,
            Guid outputConnectorNodeGuid,
            Guid inputConnectorGuid,
            Guid inputConnectorNodeGuid)
        {
            NodeLinkGuid = nodeLinkGuid;
            OutputConnectorGuid = outputConnectorGuid;
            OutputConnectorNodeGuid = outputConnectorNodeGuid;
            InputConnectorGuid = inputConnectorGuid;
            InputConnectorNodeGuid = inputConnectorNodeGuid;
        }
    }
}
