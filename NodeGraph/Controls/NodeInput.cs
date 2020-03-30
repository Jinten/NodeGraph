using NodeGraph.Utilities;
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
        FrameworkElement _ConnectorControl = null;

        static NodeInputContent()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NodeInputContent), new FrameworkPropertyMetadata(typeof(NodeInputContent)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _ConnectorControl = GetTemplateChild("__InputConnector__") as FrameworkElement;
        }

        public override void UpdatePosition(Canvas canvas)
        {
            var transformer = _ConnectorControl.TransformToVisual(canvas);
            var posOnCanvas = transformer.Transform(new Point(_ConnectorControl.ActualWidth * 0.5, _ConnectorControl.ActualHeight * 0.5));

            foreach (var nodeLink in NodeLinks)
            {
                nodeLink.UpdateInputEdge(posOnCanvas.X, posOnCanvas.Y);
            }
        }

        public override bool CanConnectTo(NodeConnectorContent connector)
        {
            return connector is NodeOutputContent;
        }
    }

	public class NodeInput : NodeConnector<NodeInputContent>
	{
		protected override string ConnectorCanvasName => "__NodeInputCanvas__";

        protected override ControlTemplate NodeConnectorContentTemplate => _NodeInputTemplate.Get("__NodeInputContentTemplate__");
		ResourceInstance<ControlTemplate> _NodeInputTemplate = new ResourceInstance<ControlTemplate>();

        static NodeInput()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(NodeInput), new FrameworkPropertyMetadata(typeof(NodeInput)));
		}
    }
}
