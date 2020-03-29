using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace NodeGraph.Controls
{
	public class NodeLink : Shape, IDisposable
	{
		protected override Geometry DefiningGeometry => _LinkGeometry;

        public Point StartPoint
        {
            get => _LinkGeometry.StartPoint;
            set => _LinkGeometry.StartPoint = value;
        }

        public Point EndPoint
        {
            get => _LinkGeometry.EndPoint;
            set => _LinkGeometry.EndPoint = value;
        }

        NodeInputContent _Input;
        NodeOutputContent _Output;
		LineGeometry _LinkGeometry = null;

		public NodeLink(double x, double y, NodeOutputContent output)
		{
            _Output = output;

            _LinkGeometry = new LineGeometry(new Point(x, y), new Point(x, y));

			StrokeThickness = 2;
			Stroke = Brushes.Black;

            Canvas.SetZIndex(this, ZIndex.NodeLink);
        }

        public void Dispose()
        {
            _Input = null;
            _Output = null;
        }

        public void Connect(NodeInputContent input)
        {
            _Input = input;
        }

        public void Disconnect()
        {
            _Input.Disconnect(this);
            _Output.Disconnect(this);
        }
	}
}
