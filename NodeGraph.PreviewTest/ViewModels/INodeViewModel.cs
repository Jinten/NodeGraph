using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeGraph.PreviewTest.ViewModels
{
    public interface INodeViewModel
    {
        Guid Guid { get; set; }

        NodeConnectorViewModel FindConnector(Guid guid);
    }
}
