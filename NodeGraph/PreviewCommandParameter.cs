using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeGraph
{
    public class PreviewConnectCommandParameter
    {
        public bool CanConnect { get; set; } = true;

        public Guid ConnectStartNodeGuid { get; } = Guid.Empty;
        public Guid ConnectStartConnectorGuid { get; } = Guid.Empty;
        public Guid ConnectToNodeGuid { get; } = Guid.Empty;
        public Guid ConnectToConnectorGuid { get; } = Guid.Empty;

        public PreviewConnectCommandParameter(
            Guid connectStartNodeGuid,
            Guid connectStartConnectorGuid,
            Guid connectToNodeGuid,
            Guid connectToConnectorGuid)
        {
            ConnectStartNodeGuid = connectStartNodeGuid;
            ConnectStartConnectorGuid = connectStartConnectorGuid;
            ConnectToNodeGuid = connectToNodeGuid;
            ConnectToConnectorGuid = connectToConnectorGuid;
        }
    }
}
