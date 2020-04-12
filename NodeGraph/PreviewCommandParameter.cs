using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeGraph
{
    public enum ConnectorType
    {
        Input,
        Output
    }

    public class PreviewConnectCommandParameter
    {
        public bool CanConnect { get; set; } = true;

        public ConnectorType ConnectTo { get; } = ConnectorType.Input;

        public Guid InputNodeGuid { get; } = Guid.Empty;
        public Guid InputConnectorGuid { get; } = Guid.Empty;
        public Guid OutputNodeGuid { get; } = Guid.Empty;
        public Guid OutputConnectorGuid { get; } = Guid.Empty;

        public PreviewConnectCommandParameter(ConnectorType connectTo, Guid inputNodeGuid, Guid inputGuid, Guid outputNodeGuid, Guid outputGuid)
        {
            ConnectTo = connectTo;

            InputNodeGuid = inputNodeGuid;
            InputConnectorGuid = inputGuid;
            OutputNodeGuid = outputNodeGuid;
            OutputConnectorGuid = outputGuid;
        }
    }
}
