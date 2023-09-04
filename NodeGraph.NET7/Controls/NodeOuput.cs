using NodeGraph.NET7.Utilities;
using System.Windows;
using System.Windows.Controls;

namespace NodeGraph.NET7.Controls
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
            _ConnectorControl = GetTemplateChild("__OutputConnector__") as FrameworkElement;

            base.OnApplyTemplate();
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
            if ((CanConnect && connector is NodeInputContent && Node != connector.Node) == false)
            {
                return false;
            }

            // check for circulation connecting.
            var nodeLinks = connector.Node.EnumrateConnectedNodeLinks();
            foreach (var nodeLink in nodeLinks)
            {
                if (nodeLink.Input?.Node == Node)
                {
                    return false;
                }
            }

            return true;
        }
    }

    public class NodeOutput : NodeConnector<NodeOutputContent>
    {
        protected override string ConnectorCanvasName => "__NodeOutputCanvas__";

        protected override ControlTemplate NodeConnectorContentTemplate => _NodeOutputTemplate.Get("__NodeOutputContentTemplate__");

        protected override Style NodeConnectorContentBaseStyle => _NodeConnectorContentBaseStyle.Get("__NodeOutputBaseStyle__");
        ResourceInstance<Style> _NodeConnectorContentBaseStyle = new ResourceInstance<Style>();

        ResourceInstance<ControlTemplate> _NodeOutputTemplate = new ResourceInstance<ControlTemplate>();

        static NodeOutput()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NodeOutput), new FrameworkPropertyMetadata(typeof(NodeOutput)));
        }
    }
}
