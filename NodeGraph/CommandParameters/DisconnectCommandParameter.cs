using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeGraph.CommandParameters
{
    public class DisconnectCommandParameter
    {
        public Guid NodeLinkGuid { get; } = Guid.Empty;
        public Guid InputNodeGuid { get; } = Guid.Empty;
        public Guid InputConnectorGuid { get; } = Guid.Empty;
        public Guid OutputNodeGuid { get; } = Guid.Empty;
        public Guid OutputConnectorGuid { get; } = Guid.Empty;

        public DisconnectCommandParameter(
            Guid nodeLinkGuid,
            Guid inputNodeGuid,
            Guid inputConnectorGuid,
            Guid outputNodeGuid,
            Guid outputConnectorGuid)
        {
            NodeLinkGuid = nodeLinkGuid;
            InputNodeGuid = inputNodeGuid;
            InputConnectorGuid = inputConnectorGuid;
            OutputNodeGuid = outputNodeGuid;
            OutputConnectorGuid = outputConnectorGuid;
        }
    }
}
