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
    public class NodeOutputContent : NodeConnectorContent
    {
        protected override FrameworkElement ConnectorControl => _ConnectorControl;
        FrameworkElement _ConnectorControl = null;

        static NodeOutputContent()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NodeOutputContent), new FrameworkPropertyMetadata(typeof(NodeOutputContent)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _ConnectorControl = GetTemplateChild("__OutputConnector__") as FrameworkElement;
        }

        public override void UpdateLinkPosition(Canvas canvas)
        {
            var transformer = ConnectorControl.TransformToVisual(canvas);
            var posOnCanvas = transformer.Transform(new Point(ConnectorControl.ActualWidth * 0.5, ConnectorControl.ActualHeight * 0.5));

            foreach (var nodeLink in NodeLinks)
            {
                nodeLink.UpdateOutputEdge(posOnCanvas.X, posOnCanvas.Y);
            }
        }

        public override bool CanConnectTo(NodeConnectorContent connector)
        {
            return connector is NodeInputContent;
        }
    }

    public class NodeOutput : NodeConnector<NodeOutputContent>
    {
		protected override string ConnectorCanvasName => "__NodeOutputCanvas__";

        protected override ControlTemplate NodeConnectorContentTemplate => _NodeOutputTemplate.Get("__NodeOutputContentTemplate__");
		ResourceInstance<ControlTemplate> _NodeOutputTemplate = new ResourceInstance<ControlTemplate>();

        static NodeOutput()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(NodeOutput), new FrameworkPropertyMetadata(typeof(NodeOutput)));
		}
    }
}
