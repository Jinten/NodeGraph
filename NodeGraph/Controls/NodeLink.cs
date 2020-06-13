using NodeGraph.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
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
        public Guid Guid
        {
            get => (Guid)GetValue(GuidProperty);
            set => SetValue(GuidProperty, value);
        }
        public static readonly DependencyProperty GuidProperty = DependencyProperty.Register(
            nameof(Guid),
            typeof(Guid),
            typeof(NodeLink),
            new FrameworkPropertyMetadata(Guid.NewGuid()));

        public Guid InputGuid
        {
            get => (Guid)GetValue(InputGuidProperty);
            set => SetValue(InputGuidProperty, value);
        }
        public static readonly DependencyProperty InputGuidProperty = DependencyProperty.Register(
            nameof(InputGuid),
            typeof(Guid),
            typeof(NodeLink),
            new FrameworkPropertyMetadata(Guid.NewGuid()));

        public Guid OutputGuid
        {
            get => (Guid)GetValue(OutputGuidProperty);
            set => SetValue(OutputGuidProperty, value);
        }
        public static readonly DependencyProperty OutputGuidProperty = DependencyProperty.Register(
            nameof(OutputGuid),
            typeof(Guid),
            typeof(NodeLink),
            new FrameworkPropertyMetadata(Guid.NewGuid()));

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

        public bool IsLocked
        {
            get => (bool)GetValue(IsLockedProperty);
            set => SetValue(IsLockedProperty, value);
        }
        public static readonly DependencyProperty IsLockedProperty =
            DependencyProperty.Register(nameof(IsLocked), typeof(bool), typeof(NodeLink), new FrameworkPropertyMetadata(false));

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

        public bool IsConnecting => Input != null && Output != null;

        public NodeInputContent Input { get; private set; } = null;
        public NodeOutputContent Output { get; private set; } = null;

        public NodeConnectorContent StartConnector { get; private set; } = null;
        public NodeConnectorContent ToEndConnector { get; private set; } = null;

        protected override Geometry DefiningGeometry => Geometry.Empty;

        double EndPointX => _EndPoint.X / _Scale;
        double EndPointY => _EndPoint.Y / _Scale;
        Point _EndPoint = new Point(0, 0);

        double StartPointX => _StartPoint.X / _Scale;
        double StartPointY => _StartPoint.Y / _Scale;
        Point _StartPoint = new Point(0, 0);

        Canvas Canvas { get; } = null;

        double _Scale = 1.0f;
        Point _RestoreEndPoint = new Point();

        static NodeLink()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NodeLink), new FrameworkPropertyMetadata(typeof(NodeLink)));
        }

        public NodeLink(Canvas canvas, double scale) : this(canvas, 0, 0, scale)
        {
            // constructed from view model.
        }

        public NodeLink(Canvas canvas, double x, double y, double scale, NodeConnectorContent connector) : this(canvas, x, y, scale)
        {
            // constructed from view for previewing node link. 

            switch (connector)
            {
                case NodeInputContent input:
                    Input = input;
                    StartConnector = input;
                    break;
                case NodeOutputContent output:
                    Output = output;
                    StartConnector = output;
                    break;
                default:
                    throw new InvalidCastException($"Cannot cast this connector. {connector}");
            }
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

        public NodeLink CreateGhost()
        {
            var ghost = new NodeLink(Canvas, _Scale);
            ghost._StartPoint = _StartPoint;
            ghost._EndPoint = _EndPoint;
            ghost.IsHitTestVisible = false;
            ghost.Fill = new SolidColorBrush(Color.FromArgb(128, 128, 128, 128));

            return ghost;
        }

        public void Validate()
        {
            if (Guid == Guid.Empty || InputGuid == Guid.Empty || OutputGuid == Guid.Empty)
            {
                throw new DataException("Does not assigned guid");
            }
        }

        public void Dispose()
        {
            Input?.Disconnect(this);
            Input = null;
            Output?.Disconnect(this);
            Output = null;
        }

        public void Connect(NodeInputContent input, NodeOutputContent output)
        {
            Input?.Disconnect(this);
            Output?.Disconnect(this);

            Input = input;
            Output = output;

            StartConnector = Output;
            ToEndConnector = Input;

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


        void UpdateConnectPosition()
        {
            _EndPoint = Input.GetContentPosition(Canvas, 0, 0.5);
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

            Point c0 = new Point();
            Point c1 = new Point();
            double power = 100;

            var pen = new Pen(Brushes.Green, 2);

            var axis = new Vector(1, 0);
            var startToEnd = (end.ToVector() - start.ToVector()).NormalizeTo();

            var k = 1 - Math.Pow(Math.Max(0, axis.DotProduct(startToEnd)), 10.0);
            var bias = start.X > end.X ? Math.Abs(start.X - end.X) * 0.25 : 0;

            c0 = new Point(+(power + bias) * k + start.X, start.Y);
            c1 = new Point(-(power + bias) * k + end.X, end.Y);

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
