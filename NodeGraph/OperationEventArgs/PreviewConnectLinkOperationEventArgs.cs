using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeGraph.OperationEventArgs
{
    public class PreviewConnectLinkOperationEventArgs
    {
        public bool CanConnect { get; set; } = true;

        public Guid ConnectStartNodeGuid { get; } = Guid.Empty;
        public Guid ConnectStartConnectorGuid { get; } = Guid.Empty;
        public Guid ConnectToEndNodeGuid { get; } = Guid.Empty;
        public Guid ConnectToEndConnectorGuid { get; } = Guid.Empty;

        public PreviewConnectLinkOperationEventArgs(
            Guid connectStartNodeGuid,
            Guid connectStartConnectorGuid,
            Guid connectToEndNodeGuid,
            Guid connectToEndConnectorGuid)
        {
            ConnectStartNodeGuid = connectStartNodeGuid;
            ConnectStartConnectorGuid = connectStartConnectorGuid;
            ConnectToEndNodeGuid = connectToEndNodeGuid;
            ConnectToEndConnectorGuid = connectToEndConnectorGuid;
        }
    }
}
