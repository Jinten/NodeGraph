using NodeGraph.Utilities;
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
    public enum NodeLinkType
    {
        Line,
        Curve
    }

    public class NodeLink : Shape, ICanvasObject, IDisposable
    {
        double EndPointX => _EndPoint.X / _Scale;
        double EndPointY => _EndPoint.Y / _Scale;
        Point _EndPoint = new Point(0, 0);

        double StartPointX => _StartPoint.X / _Scale;
        double StartPointY => _StartPoint.Y / _Scale;
        Point _StartPoint = new Point(0, 0);

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

        public NodeLinkType LinkType
        {
            get => (NodeLinkType)GetValue(LinkTypeProperty);
            set => SetValue(LinkTypeProperty, value);
        }
        public static readonly DependencyProperty LinkTypeProperty =
            DependencyProperty.Register(nameof(LinkType), typeof(NodeLinkType), typeof(NodeLink), new FrameworkPropertyMetadata(NodeLinkType.Curve, LinkTypePropertyChanged));

        static void DashOffsetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var nodeLink = d as NodeLink;
            nodeLink.InvalidateVisual();
        }

        static void LinkTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var nodeLink = d as NodeLink;
            nodeLink.InvalidateVisual();
        }

        public NodeInputContent Input { get; private set; } = null;
        public NodeOutputContent Output { get; private set; } = null;

        protected override Geometry DefiningGeometry => Geometry.Empty;

        bool IsConnecting => Input != null && Output != null;

        Canvas Canvas { get; } = null;

        double _Scale = 1.0f;
        Point _RestoreEndPoint = new Point();

        public NodeLink(Canvas canvas, double x, double y, double scale, NodeInputContent input) : this(canvas, x, y, scale)
        {
            Input = input;
        }

        public NodeLink(Canvas canvas, double x, double y, double scale, NodeOutputContent output) : this(canvas, x, y, scale)
        {
            Output = output;
        }

        NodeLink(Canvas canvas, double x, double y, double scale)
        {
            Canvas = canvas;

            IsHitTestVisible = false; // no need to hit until connected

            var point = new Point(x, y);
            _StartPoint = point;
            _EndPoint = point;

            _Scale = scale;

            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(new ScaleTransform() { ScaleX = scale, ScaleY = scale });
            RenderTransform = transformGroup;
            RenderTransformOrigin = new Point(0.5, 0.5);

            Canvas.SetZIndex(this, -1);
        }

        public void Dispose()
        {
            Input = null;
            Output = null;
        }

        public void Connect(NodeInputContent input)
        {
            if(Input != null)
            {
                Input.Disconnect(this);
            }
            Input = input;

            IsHitTestVisible = true;

            UpdateConnectPosition();

            InvalidateVisual();
        }

        public void Connect(NodeOutputContent output)
        {
            if(Output != null)
            {
                Output.Disconnect(this);
            }
            Output = output;

            IsHitTestVisible = true;

            UpdateConnectPosition();

            InvalidateVisual();
        }

        public void UpdateOffset(Point offset)
        {
            UpdateConnectPosition();

            InvalidateVisual();
        }

        public void UpdateEdgePoint(double x, double y)
        {
            if (Input == null)
            {
                // first connected output to input.
                _EndPoint = new Point(x, y);
            }
            else if (Output == null)
            {
                // first connected input to output.
                _StartPoint = new Point(x, y);
            }
            else
            {
                // reconnected point.
                _EndPoint = new Point(x, y);
            }

            InvalidateVisual();
        }

        public void ReleaseEndPoint()
        {
            IsHitTestVisible = false;
            _RestoreEndPoint = _EndPoint;
        }

        public void RestoreEndPoint()
        {
            IsHitTestVisible = true;
            UpdateEdgePoint(_RestoreEndPoint.X, _RestoreEndPoint.Y);
        }

        public void UpdateInputEdge(double x, double y)
        {
            _EndPoint = new Point(x, y);
            InvalidateVisual();
        }

        public void UpdateOutputEdge(double x, double y)
        {
            _StartPoint = new Point(x, y);
            InvalidateVisual();
        }

        public void Disconnect()
        {
            Input.Disconnect(this);
            Output.Disconnect(this);
        }

        void UpdateConnectPosition()
        {
            _EndPoint = Input.GetContentPosition(Canvas, 0.5, 0.5);
            _StartPoint = Output.GetContentPosition(Canvas, 0.5, 0.5);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            switch (LinkType)
            {
                case NodeLinkType.Line:
                    DrawLine(drawingContext);
                    break;
                case NodeLinkType.Curve:
                    DrawCurve(drawingContext);
                    break;
            }
        }

        void DrawLine(DrawingContext drawingContext)
        {
            var start = new Point(StartPointX, StartPointY);
            var end = new Point(EndPointX, EndPointY);

            double linkSize = LinkSize * (1.0f / _Scale);

            // collision
            if (IsHitTestVisible)
            {
                var hitVisiblePen = new Pen(Brushes.Transparent, linkSize + StrokeThickness);
                hitVisiblePen.Freeze();
                drawingContext.DrawLine(hitVisiblePen, start, end);
            }

            // actually visual
            var visualPen = new Pen(Fill, linkSize);
            if (IsMouseOver)
            {
                visualPen.DashStyle = new DashStyle(new double[] { 2 }, DashOffset);
            }
            visualPen.Freeze();

            drawingContext.DrawLine(visualPen, start, end);

            // arrow
            var vEnd = new Vector(EndPointX, EndPointY);
            var vStart = new Vector(StartPointX, StartPointY);
            var degree = Vector.AngleBetween(vStart - vEnd, vStart);

            if (IsConnecting == false)
            {
                return;
            }

            var segment = vStart - vEnd;
            segment.Normalize();

            Matrix rotMat = new Matrix();
            {
                rotMat.Rotate(30);

                var arrow = Vector.Multiply(segment, rotMat);
                drawingContext.DrawLine(visualPen, end, arrow * 12 + end);
            }
            {
                rotMat.Rotate(-60);

                var arrow = Vector.Multiply(segment, rotMat);
                drawingContext.DrawLine(visualPen, end, arrow * 12 + end);
            }
        }

        void DrawCurve(DrawingContext drawingContext)
        {
            var start = new Point(StartPointX, StartPointY);
            var end = new Point(EndPointX, EndPointY);
            var c0 = new Point((end.X - start.X) * 0.5 + start.X, start.Y);
            var c1 = new Point((start.X - end.X) * 0.5 + end.X, end.Y);

            var stream = new StreamGeometry();

            using (var context = stream.Open())
            {
                context.BeginFigure(start, true, false);
                context.BezierTo(c0, c1, end, true, false);
            }
            stream.Freeze();

            double linkSize = LinkSize * (1.0f / _Scale);

            // collision
            if (IsHitTestVisible)
            {
                var hitVisiblePen = new Pen(Brushes.Transparent, linkSize + StrokeThickness);
                hitVisiblePen.Freeze();
                drawingContext.DrawGeometry(null, hitVisiblePen, stream);
            }

            // actually visual
            var visualPen = new Pen(Fill, linkSize);
            if (IsMouseOver)
            {
                visualPen.DashStyle = new DashStyle(new double[] { 2 }, DashOffset);
            }

            visualPen.Freeze();

            drawingContext.DrawGeometry(null, visualPen, stream);
        }
    }
}
