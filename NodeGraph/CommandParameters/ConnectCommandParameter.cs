using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeGraph.CommandParameters
{
    public class ConnectCommandParameter
    {
        public Guid InputNodeGuid { get; } = Guid.Empty;
        public Guid InputConnectorGuid { get; } = Guid.Empty;
        public Guid OutputNodeGuid { get; } = Guid.Empty;
        public Guid OutputConnectorGuid { get; } = Guid.Empty;

        public ConnectCommandParameter(
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
