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

        protected override Geometry DefiningGeometry => _LinkGeometry;

        enum PointPlace
        {
            Start,
            End,
        }

        NodeInputContent _Input;
        NodeOutputContent _Output;
        PointPlace _InputPointPlace;
        PointPlace _OutputPointPlace;
        LineGeometry _LinkGeometry = null;

        public NodeLink(double x, double y, NodeInputContent input) : this(x, y)
        {
            _Input = input;
            _InputPointPlace = PointPlace.Start;
        }

        public NodeLink(double x, double y, NodeOutputContent output) : this(x, y)
        {
            _Output = output;
            _OutputPointPlace = PointPlace.Start;
        }

        NodeLink(double x, double y)
        {
            _LinkGeometry = new LineGeometry(new Point(x, y), new Point(x, y));

            StrokeThickness = 2;
            Stroke = Brushes.Black;

            Canvas.SetZIndex(this, -1);
        }

        public void Dispose()
        {
            _Input = null;
            _Output = null;
        }

        public void Connect(NodeInputContent input)
        {
            _Input = input;
            _InputPointPlace = PointPlace.End;
        }

        public void Connect(NodeOutputContent output)
        {
            _Output = output;
            _OutputPointPlace = PointPlace.End;
        }

        public void UpdateInputEdge(double x, double y)
        {
            switch(_InputPointPlace)
            {
                case PointPlace.Start:
                    StartPoint = new Point(x, y);
                    break;
                case PointPlace.End:
                    EndPoint = new Point(x, y);
                    break;
            }
        }

        public void UpdateOutputEdge(double x, double y)
        {
            switch (_OutputPointPlace)
            {
                case PointPlace.Start:
                    StartPoint = new Point(x, y);
                    break;
                case PointPlace.End:
                    EndPoint = new Point(x, y);
                    break;
            }
        }

        public void Disconnect()
        {
            _Input.Disconnect(this);
            _Output.Disconnect(this);
        }
    }
}
