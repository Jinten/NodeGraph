using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace NodeGraph.Controls
{
    public class NodeOutputContent : NodeConnectorContent
    {
        static NodeOutputContent()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NodeOutputContent), new FrameworkPropertyMetadata(typeof(NodeOutputContent)));
        }
    }

    public class NodeOutput : NodeConnector<NodeOutputContent>
    {
		protected override string ConnectorCanvasName => "__NodeOutputCanvas__";

		protected override ControlTemplate NodeConnectorContentTemplate
		{
			get
			{
				if(_NodeOutputTemplate == null)
				{
					_NodeOutputTemplate = Application.Current.TryFindResource("__NodeOutputContentTemplate__") as ControlTemplate;
				}
				return _NodeOutputTemplate;
			}
		}
		ControlTemplate _NodeOutputTemplate = null;

		static NodeOutput()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(NodeOutput), new FrameworkPropertyMetadata(typeof(NodeOutput)));
		}
	}
}
