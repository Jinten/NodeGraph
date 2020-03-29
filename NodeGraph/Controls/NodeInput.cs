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
    public class NodeInputContent : NodeConnectorContent
    {
        static NodeInputContent()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NodeInputContent), new FrameworkPropertyMetadata(typeof(NodeInputContent)));
        }
    }

	public class NodeInput : NodeConnector<NodeInputContent>
	{
		protected override string ConnectorCanvasName => "__NodeInputCanvas__";

		protected override ControlTemplate NodeConnectorContentTemplate
		{
			get
			{
				if(_NodeInputTemplate == null)
				{
					_NodeInputTemplate = Application.Current.TryFindResource("__NodeInputContentTemplate__") as ControlTemplate;
				}
				return _NodeInputTemplate;
			}
		}
		ControlTemplate _NodeInputTemplate = null;

		static NodeInput()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(NodeInput), new FrameworkPropertyMetadata(typeof(NodeInput)));
		}
	}
}
