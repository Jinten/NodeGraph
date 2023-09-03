using NodeGraph.NET6.Utilities;
using System.Windows;
using System.Windows.Controls;

namespace NodeGraph.NET6.Controls
{
    public class NodeInputContent : NodeConnectorContent
    {
        public bool AllowToConnectMultiple
        {
            get => (bool)GetValue(AllowToConnectMultipleProperty);
            set => SetValue(AllowToConnectMultipleProperty, value);
        }
        public static readonly DependencyProperty AllowToConnectMultipleProperty = DependencyProperty.Register(
            nameof(AllowToConnectMultiple),
            typeof(bool),
            typeof(NodeInputContent),
            new PropertyMetadata(false));

        public static bool AllowToOverrideConnection { get; set; } = false;

        protected override FrameworkElement ConnectorControl => _ConnectorControl;
        FrameworkElement _ConnectorControl = null;


        static NodeInputContent()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NodeInputContent), new FrameworkPropertyMetadata(typeof(NodeInputContent)));
        }

        public override void OnApplyTemplate()
        {
            _ConnectorControl = GetTemplateChild("__InputConnector__") as FrameworkElement;

            base.OnApplyTemplate();
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
            if (AllowToOverrideConnection == false && ConnectedCount > 0 && AllowToConnectMultiple == false)
            {
                // already connected to other node link.
                return false;
            }
            if ((CanConnect && connector is NodeOutputContent && Node != connector.Node) == false)
            {
                return false;
            }

            // check for circulation connecting.
            var nodeLinks = connector.Node.EnumrateConnectedNodeLinks();
            foreach (var nodeLink in nodeLinks)
            {
                if (nodeLink.Output?.Node == Node)
                {
                    return false;
                }
            }

            return true;
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
