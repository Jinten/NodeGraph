using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace NodeGraph.Controls
{
    public class NodeLink : Shape, IDisposable
    {
        public Point EndPoint { get; private set; }
        public Point StartPoint { get; private set; }

        public double LinkSize
        {
            get => (double)GetValue(LinkSizeProperty);
            set => SetValue(LinkSizeProperty, value);
        }
        public static readonly DependencyProperty LinkSizeProperty = 
            DependencyProperty.Register(nameof(LinkSize), typeof(double), typeof(NodeLink), new FrameworkPropertyMetadata(2.0));

        public double DashOffset
        {
            get => (double)GetValue(DashOffsetProperty);
            set => SetValue(DashOffsetProperty, value);
        }
        public static readonly DependencyProperty DashOffsetProperty =
            DependencyProperty.Register(nameof(DashOffset), typeof(double), typeof(NodeLink), new FrameworkPropertyMetadata(0.0, DashOffsetPropertyChanged));

        private static void DashOffsetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var nodeLink = d as NodeLink;
            nodeLink.InvalidateVisual();
        }

        public NodeInputContent Input { get; private set; } = null;
        public NodeOutputContent Output { get; private set; } = null;

        bool IsConnecting => Input != null && Output != null;

        protected override Geometry DefiningGeometry => Geometry.Empty;

        Pen _HitVisiblePen = null;

        public NodeLink(double x, double y, NodeInputContent input) : this(x, y)
        {
            Input = input;
        }

        public NodeLink(double x, double y, NodeOutputContent output) : this(x, y)
        {
            Output = output;
        }

        NodeLink(double x, double y)
        {
            IsHitTestVisible = false; // no need to hit until connected

            var point = new Point(x, y);
            StartPoint = point;
            EndPoint = point;

            Canvas.SetZIndex(this, -1);
        }

        public void Dispose()
        {
            Input = null;
            Output = null;
        }

        public void Connect(NodeInputContent input)
        {
            Input = input;
            IsHitTestVisible = true;

            InvalidateVisual();
        }

        public void Connect(NodeOutputContent output)
        {
            Output = output;
            IsHitTestVisible = true;

            InvalidateVisual();
        }

        public void UpdateEdgePoint(double x, double y)
        {
            if(Input == null)
            {
                // first connecting output to input.
                EndPoint = new Point(x, y);
            }
            else if(Output == null)
            {
                // first connecting input to output.
                StartPoint = new Point(x, y);
            }
            else
            {
                // reconnecting.
                EndPoint = new Point(x, y);
            }

            InvalidateVisual();
        }

        public void ReleaseEndPoint()
        {
            IsHitTestVisible = false;
        }

        public void RestoreEndPoint()
        {
            IsHitTestVisible = true;
        }

        public void UpdateInputEdge(double x, double y)
        {
            EndPoint = new Point(x, y);
            InvalidateVisual();
        }

        public void UpdateOutputEdge(double x, double y)
        {
            StartPoint = new Point(x, y);
            InvalidateVisual();
        }

        public void Disconnect()
        {
            Input.Disconnect(this);
            Output.Disconnect(this);
        }

        protected override void OnStyleChanged(Style oldStyle, Style newStyle)
        {
            base.OnStyleChanged(oldStyle, newStyle);

            _HitVisiblePen = new Pen(Brushes.Transparent, LinkSize + StrokeThickness);
            _HitVisiblePen.Freeze();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            // collision
            if (IsHitTestVisible)
            {
                drawingContext.DrawLine(_HitVisiblePen, StartPoint, EndPoint);
            }

            // actually visual
            var visualPen = new Pen(Fill, LinkSize);
            if (IsMouseOver)
            {
                visualPen.DashStyle = new DashStyle(new double[] { 2 }, DashOffset);
            }

            drawingContext.DrawLine(visualPen, StartPoint, EndPoint);

            // arrow
            var vEnd = new Vector(EndPoint.X, EndPoint.Y);
            var vStart = new Vector(StartPoint.X, StartPoint.Y);
            var degree = Vector.AngleBetween(vStart - vEnd, vStart);

            if(IsConnecting == false)
            {
                return;
            }

            var segment = vStart - vEnd;
            segment.Normalize();

            Matrix rotMat = new Matrix();
            {
                rotMat.Rotate(30);

                var arrow = Vector.Multiply(segment, rotMat);
                drawingContext.DrawLine(visualPen, EndPoint, arrow * 12 + EndPoint);
            }
            {
                rotMat.Rotate(-60);
                var arrow = Vector.Multiply(segment, rotMat);
                drawingContext.DrawLine(visualPen, EndPoint, arrow * 12 + EndPoint);
            }
        }
    }
}
