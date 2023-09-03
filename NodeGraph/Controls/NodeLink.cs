using NodeGraph.Extensions;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace NodeGraph.Controls
{
    public enum NodeLinkType
    {
        Line,
        Curve
    }

    public class NodeLink : Shape, ICanvasObject, ISelectableObject
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

        public Guid InputConnectorGuid
        {
            get => (Guid)GetValue(InputConnectorGuidProperty);
            set => SetValue(InputConnectorGuidProperty, value);
        }
        public static readonly DependencyProperty InputConnectorGuidProperty = DependencyProperty.Register(
            nameof(InputConnectorGuid),
            typeof(Guid),
            typeof(NodeLink),
            new FrameworkPropertyMetadata(Guid.NewGuid()));

        public Guid OutputConnectorGuid
        {
            get => (Guid)GetValue(OutputConnectorGuidProperty);
            set => SetValue(OutputConnectorGuidProperty, value);
        }
        public static readonly DependencyProperty OutputConnectorGuidProperty = DependencyProperty.Register(
            nameof(OutputConnectorGuid),
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

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            nameof(IsSelected), typeof(bool), typeof(NodeLink), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        static void DashOffsetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var nodeLink = (NodeLink)d;
            nodeLink.InvalidateVisual();
        }

        static void LinkTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var nodeLink = (NodeLink)d;
            nodeLink.InvalidateVisual();
        }

        public bool IsConnecting => Input != null && Output != null;

        public NodeInputContent Input { get; private set; } = null;
        public NodeOutputContent Output { get; private set; } = null;

        public NodeConnectorContent StartConnector { get; private set; } = null;
        public NodeConnectorContent ToEndConnector { get; private set; } = null;

        protected override Geometry DefiningGeometry => StreamGeometry;

        double EndPointX => _EndPoint.X / _Scale;
        double EndPointY => _EndPoint.Y / _Scale;
        Point _EndPoint = new Point(0, 0);

        double StartPointX => _StartPoint.X / _Scale;
        double StartPointY => _StartPoint.Y / _Scale;
        Point _StartPoint = new Point(0, 0);

        Canvas Canvas { get; } = null;

        double _Scale = 1.0f;
        Point _Offset = new Point(0, 0);
        Point _RestoreEndPoint = new Point();

        readonly StreamGeometry StreamGeometry = new StreamGeometry();

        static NodeLink()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NodeLink), new FrameworkPropertyMetadata(typeof(NodeLink)));
        }

        internal NodeLink(Canvas canvas, double scale, Point offset)
        {
            // constructed from view model or other constructors.
            Canvas = canvas;

            IsHitTestVisible = false; // no need to hit until connected

            _Scale = scale;
            _Offset = offset;

            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(new ScaleTransform() { ScaleX = scale, ScaleY = scale });
            RenderTransform = transformGroup;
            RenderTransformOrigin = new Point(0.5, 0.5);

            Canvas.SetZIndex(this, -1);
        }

        internal NodeLink(NodeConnectorContent connector, double x, double y, Canvas canvas, double scale, Point offset) : this(x, y, canvas, scale, offset)
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

        NodeLink(double x, double y, Canvas canvas, double scale, Point offset) : this(canvas, scale, offset)
        {
            // create from view for preview reconnecting link.
            var point = new Point(x, y);
            _StartPoint = point;
            _EndPoint = point;
        }

        public bool Contains(Rect rect)
        {
            var test = Rect.Offset(RenderedGeometry.Bounds, -_Offset.X, -_Offset.Y);
            return rect.Contains(test);
        }

        public bool IntersectsWith(Rect rect)
        {
            var test = Rect.Offset(RenderedGeometry.Bounds, -_Offset.X, -_Offset.Y);
            return rect.IntersectsWith(test);
        }

        internal NodeLink CreateGhost()
        {
            var ghost = new NodeLink(Canvas, _Scale, _Offset);
            ghost.DataContext = null; // ghost link cannot binding connector properties. so you have to bind null to DataContext.(otherwise, occur binding errors)
            ghost._StartPoint = _StartPoint;
            ghost._EndPoint = _EndPoint;
            ghost.IsHitTestVisible = false;
            ghost.Fill = new SolidColorBrush(Color.FromArgb(128, 128, 128, 128));

            return ghost;
        }

        internal void Validate()
        {
            if (Guid == Guid.Empty || InputConnectorGuid == Guid.Empty || OutputConnectorGuid == Guid.Empty)
            {
                throw new DataException("Does not assigned guid");
            }
        }

        public void Dispose()
        {
            // You need to clear Style.
            // Because implemented on style for binding.
            Style = null;

            // Clear binding for subscribing source changed event from old control.
            // throw exception about visual tree ancestor different if you not clear binding.
            BindingOperations.ClearAllBindings(this);

            Input?.Disconnect(this);
            Input = null;
            Output?.Disconnect(this);
            Output = null;
        }

        internal void Connect(NodeInputContent input, NodeOutputContent output)
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
            _Offset = offset;

            UpdateConnectPosition();

            InvalidateVisual();
        }

        internal void UpdateEdgePoint(double x, double y)
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

        internal void ReleaseEndPoint()
        {
            IsHitTestVisible = false;
            _RestoreEndPoint = _EndPoint;
        }

        internal void RestoreEndPoint()
        {
            IsHitTestVisible = true;
            UpdateEdgePoint(_RestoreEndPoint.X, _RestoreEndPoint.Y);
        }

        internal void UpdateInputEdge(double x, double y)
        {
            _EndPoint = new Point(x, y);
            InvalidateVisual();
        }

        internal void UpdateOutputEdge(double x, double y)
        {
            _StartPoint = new Point(x, y);
            InvalidateVisual();
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

        void UpdateConnectPosition()
        {
            _EndPoint = Input.GetContentPosition(Canvas, 0, 0.5);
            _StartPoint = Output.GetContentPosition(Canvas, 0.5, 0.5);
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

            StreamGeometry.Clear();

            using (var context = StreamGeometry.Open())
            {
                context.BeginFigure(start, true, false);
                context.BezierTo(c0, c1, end, true, false);
            }

            double linkSize = LinkSize * (1.0f / _Scale);

            // collision
            if (IsHitTestVisible)
            {
                var hitVisiblePen = new Pen(Brushes.Transparent, linkSize + StrokeThickness);
                hitVisiblePen.Freeze();
                drawingContext.DrawGeometry(null, hitVisiblePen, StreamGeometry);
            }

            // actually visual
            var visualPen = new Pen(Fill, linkSize);
            if (IsMouseOver)
            {
                visualPen.DashStyle = new DashStyle(new double[] { 2 }, DashOffset);
            }

            visualPen.Freeze();

            drawingContext.DrawGeometry(null, visualPen, StreamGeometry);
        }
    }
}
