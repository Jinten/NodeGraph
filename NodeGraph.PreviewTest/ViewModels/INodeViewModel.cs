using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NodeGraph.PreviewTest.ViewModels
{
    public interface INodeViewModel
    {
        Guid Guid { get; set; }
        Point Position { get; set; }
        bool IsSelected { get; set; }
        NodeConnectorViewModel FindConnector(Guid guid);
    }
}
