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

        public override void UpdatePosition(Canvas canvas)
        {
            var transformer = _ConnectorControl.TransformToVisual(canvas);
            var posOnCanvas = transformer.Transform(new Point(_ConnectorControl.ActualWidth * 0.5, _ConnectorControl.ActualHeight * 0.5));

            foreach (var nodeLink in NodeLinks)
            {
                nodeLink.StartPoint = new Point(posOnCanvas.X, posOnCanvas.Y);
            }
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
