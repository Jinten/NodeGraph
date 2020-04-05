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
        protected override FrameworkElement ConnectorControl => _ConnectorControl;
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

        public override void UpdateLinkPosition(Canvas canvas)
        {
            var transformer = _ConnectorControl.TransformToVisual(canvas);
            var posOnCanvas = transformer.Transform(new Point(0, _ConnectorControl.ActualHeight * 0.5));

            foreach (var nodeLink in NodeLinks)
            {
                nodeLink.UpdateInputEdge(posOnCanvas.X, posOnCanvas.Y);
            }
        }

        public override bool CanConnectTo(NodeConnectorContent connector)
        {
            return ConnectedCount == 0 && connector is NodeOutputContent;
        }
    }

	public class NodeInput : NodeConnector<NodeInputContent>
	{
		protected override string ConnectorCanvasName => "__NodeInputCanvas__";

        protected override ControlTemplate NodeConnectorContentTemplate => _NodeInputTemplate.Get("__NodeInputContentTemplate__");
		ResourceInstance<ControlTemplate> _NodeInputTemplate = new ResourceInstance<ControlTemplate>();

        protected override Style NodeConnectorContentBaseStyle => _NodeConnectorContentBaseStyle.Get("__NodeInputBaseStyle__");
        ResourceInstance<Style> _NodeConnectorContentBaseStyle = new ResourceInstance<Style>();


        static NodeInput()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(NodeInput), new FrameworkPropertyMetadata(typeof(NodeInput)));
		}
    }
}
