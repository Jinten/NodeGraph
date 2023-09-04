using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace NodeGraph.Controls
{
    enum DragResizeType
    {
        None,
        LeftTop,
        RightTop,
        LeftBottom,
        RightBottom,
        Top,
        Bottom,
        Left,
        Right,
    }

    public class GroupNode : NodeBase
    {
        public DataTemplate HeaderContentTemplate
        {
            get => (DataTemplate)GetValue(HeaderContentTemplateProperty);
            set => SetValue(HeaderContentTemplateProperty, value);
        }
        public static readonly DependencyProperty HeaderContentTemplateProperty = DependencyProperty.Register(
            nameof(HeaderContentTemplate),
            typeof(DataTemplate),
            typeof(GroupNode),
            new FrameworkPropertyMetadata(null));

        /// <summary>
        /// This property is set only where script.
        /// Do not use at UI code.
        /// </summary>
        public Point InterlockPosition
        {
            get => (Point)GetValue(InterlockPositionProperty);
            set => SetValue(InterlockPositionProperty, value);
        }
        public static readonly DependencyProperty InterlockPositionProperty = DependencyProperty.Register(
            nameof(InterlockPosition),
            typeof(Point),
            typeof(GroupNode),
            new FrameworkPropertyMetadata(new Point(0, 0), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, InterlockPositionPropertyChanged));

        public double BorderSize
        {
            get => (double)GetValue(BorderSizeProperty);
            set => SetValue(BorderSizeProperty, value);
        }
        public static readonly DependencyProperty BorderSizeProperty = DependencyProperty.Register(
            nameof(BorderSize),
            typeof(double),
            typeof(GroupNode),
            new FrameworkPropertyMetadata(4.0));

        public Point InnerPosition
        {
            get => (Point)GetValue(InnerPositionProperty);
            set => SetValue(InnerPositionProperty, value);
        }
        public static readonly DependencyProperty InnerPositionProperty = DependencyProperty.Register(
            nameof(InnerPosition),
            typeof(Point),
            typeof(GroupNode),
            new FrameworkPropertyMetadata(new Point(0, 0), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, InnerPositionPropertyChanged));

        public double InnerWidth
        {
            get => (double)GetValue(InnerWidthProperty);
            set => SetValue(InnerWidthProperty, value);
        }
        public static readonly DependencyProperty InnerWidthProperty = DependencyProperty.Register(
            nameof(InnerWidth),
            typeof(double),
            typeof(GroupNode),
            new FrameworkPropertyMetadata(100.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, InnerWidthPropertyChanged));

        public double InnerHeight
        {
            get => (double)GetValue(InnerHeightProperty);
            set => SetValue(InnerHeightProperty, value);
        }
        public static readonly DependencyProperty InnerHeightProperty = DependencyProperty.Register(
            nameof(InnerHeight),
            typeof(double),
            typeof(GroupNode),
            new FrameworkPropertyMetadata(100.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, InnerHeightPropertyChanged));

        public SolidColorBrush InnerColor
        {
            get => (SolidColorBrush)GetValue(InnerColorProperty);
            set => SetValue(InnerColorProperty, value);
        }
        public static readonly DependencyProperty InnerColorProperty = DependencyProperty.Register(
            nameof(InnerColor),
            typeof(SolidColorBrush),
            typeof(GroupNode),
            new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromArgb(0, 0, 0, 0))));

        public string Comment
        {
            get => (string)GetValue(CommentProperty);
            set => SetValue(CommentProperty, value);
        }
        public static readonly DependencyProperty CommentProperty = DependencyProperty.Register(
            nameof(Comment),
            typeof(string),
            typeof(GroupNode),
            new FrameworkPropertyMetadata(string.Empty));

        public double CommentSize
        {
            get => (double)GetValue(CommentSizeProperty);
            set => SetValue(CommentSizeProperty, value);
        }
        public static readonly DependencyProperty CommentSizeProperty = DependencyProperty.Register(
            nameof(CommentSize),
            typeof(double),
            typeof(GroupNode),
            new FrameworkPropertyMetadata(12.0));

        public SolidColorBrush CommentForeground
        {
            get => (SolidColorBrush)GetValue(CommentForegroundProperty);
            set => SetValue(CommentForegroundProperty, value);
        }
        public static readonly DependencyProperty CommentForegroundProperty = DependencyProperty.Register(
            nameof(CommentForeground),
            typeof(SolidColorBrush),
            typeof(GroupNode),
            new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromArgb(64, 255, 255, 255))));

        public Thickness CommentMargin
        {
            get => (Thickness)GetValue(CommentMarginProperty);
            set => SetValue(CommentMarginProperty, value);
        }
        public static readonly DependencyProperty CommentMarginProperty = DependencyProperty.Register(
            nameof(CommentMargin),
            typeof(Thickness),
            typeof(GroupNode),
            new FrameworkPropertyMetadata(new Thickness(8)));

        public ICommand SizeChangedCommand
        {
            get => GetValue(SizeChangedCommandProperty) as ICommand;
            set => SetValue(SizeChangedCommandProperty, value);
        }
        public static readonly DependencyProperty SizeChangedCommandProperty = DependencyProperty.Register(
            nameof(SizeChangedCommand),
            typeof(ICommand),
            typeof(GroupNode),
            new FrameworkPropertyMetadata(null));

        public bool IsDraggingToResize { get; private set; } = false;

        bool _IsInside = false;
        bool _InternalPositionUpdating = false;

        Rect _CapturedNodeRect;
        DragResizeType _IsDraggingToResizeType = DragResizeType.None;
        Border _GroupNodeHeader = null;

        static GroupNode()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GroupNode), new FrameworkPropertyMetadata(typeof(GroupNode)));
        }

        internal GroupNode(Canvas canvas, Point offset, double scale) : base(canvas, offset)
        {
            SizeChanged += Group_SizeChanged;
        }

        internal void Initialize()
        {
            // Implement if anything.
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _GroupNodeHeader = (Border)GetTemplateChild("__GroupNodeHeader__");
            _GroupNodeHeader.SizeChanged += GroupNodeHeader_SizeChanged;
        }

        internal void ExpandSize(Rect rect)
        {
            var oldPosition = Position;
            var maxX = Math.Max(oldPosition.X + Width, rect.Right + BorderSize);
            var maxY = Math.Max(oldPosition.Y + Height, rect.Bottom + BorderSize);

            var x = rect.X - BorderSize;
            var y = rect.Y - _GroupNodeHeader.ActualHeight - BorderSize;
            UpdatePosition(Math.Min(x, Position.X), Math.Min(y, Position.Y));

            var w = rect.Width + BorderSize * 2;
            var h = rect.Height + BorderSize * 2 + _GroupNodeHeader.ActualHeight;
            Width = Math.Max(w, maxX - Position.X);
            Height = Math.Max(h, maxY - Position.Y);

            UpdateInnerSize();
            UpdateInnerPosition();
        }

        internal void CaptureToResizeDragging()
        {
            if (_IsDraggingToResizeType != DragResizeType.None)
            {
                IsDraggingToResize = true;
                _CapturedNodeRect = new Rect(Position, new Size(ActualWidth, ActualHeight));
            }
        }

        internal void ReleaseToResizeDragging()
        {
            IsDraggingToResize = false;
        }

        internal void Resize(in Point pos)
        {
            switch (_IsDraggingToResizeType)
            {
                case DragResizeType.LeftTop:
                {
                    var x = (_CapturedNodeRect.Right - pos.X > MinHeight) ? pos.X : Position.X;
                    var y = (_CapturedNodeRect.Bottom - pos.Y > MinHeight) ? pos.Y : Position.Y;
                    Position = new Point(x, y);
                    Width = Math.Max(MinWidth, _CapturedNodeRect.Right - Position.X);
                    Height = Math.Max(MinWidth, _CapturedNodeRect.Bottom - Position.Y);
                }
                Mouse.SetCursor(Cursors.SizeNWSE);
                break;
                case DragResizeType.RightTop:
                {
                    var h = _CapturedNodeRect.Bottom - pos.Y;
                    if (h > MinHeight)
                    {
                        Position = new Point(Position.X, pos.Y);
                        Height = Math.Max(MinHeight, h);
                    }
                }
                Width = Math.Max(MinWidth, pos.X - _CapturedNodeRect.X);
                Mouse.SetCursor(Cursors.SizeNESW);
                break;
                case DragResizeType.LeftBottom:
                {
                    var w = _CapturedNodeRect.Right - pos.X;
                    if (w > MinWidth)
                    {
                        Position = new Point(pos.X, Position.Y);
                        Width = Math.Max(MinWidth, w);
                    }
                }
                Height = Math.Max(MinHeight, pos.Y - _CapturedNodeRect.Y);
                Mouse.SetCursor(Cursors.SizeNESW);
                break;
                case DragResizeType.RightBottom:
                    Width = Math.Max(MinWidth, pos.X - _CapturedNodeRect.X);
                    Height = Math.Max(MinHeight, pos.Y - _CapturedNodeRect.Y);
                    Mouse.SetCursor(Cursors.SizeNWSE);
                    break;
                case DragResizeType.Top:
                {
                    var h = _CapturedNodeRect.Bottom - pos.Y;
                    if (h > MinHeight)
                    {
                        Position = new Point(Position.X, pos.Y);
                        Height = Math.Max(MinHeight, h);
                    }
                }
                Mouse.SetCursor(Cursors.SizeNS);
                break;
                case DragResizeType.Bottom:
                    Height = Math.Max(MinHeight, pos.Y - _CapturedNodeRect.Y);
                    Mouse.SetCursor(Cursors.SizeNS);
                    break;
                case DragResizeType.Left:
                {
                    var w = _CapturedNodeRect.Right - pos.X;
                    if (w > MinWidth)
                    {
                        Position = new Point(pos.X, Position.Y);
                        Width = Math.Max(MinWidth, w);
                    }
                }
                Mouse.SetCursor(Cursors.SizeWE);
                break;
                case DragResizeType.Right:
                    Width = Math.Max(MinWidth, pos.X - _CapturedNodeRect.X);
                    Mouse.SetCursor(Cursors.SizeWE);
                    break;
            }

            UpdateInnerPosition();
        }

        internal void ChangeInnerColor(bool inside)
        {
            if (_IsInside != inside)
            {
                _IsInside = inside;
                InnerColor = inside ? new SolidColorBrush(Color.FromArgb(128, 0, 255, 0)) : Brushes.Transparent;
            }
        }

        internal bool IsInsideCompletely(in Rect rect)
        {
            return InnerPosition.X <= rect.X && rect.Right <= (InnerPosition.X + InnerWidth)
                && InnerPosition.Y <= rect.Y && rect.Bottom <= (InnerPosition.Y + InnerHeight);
        }

        internal bool IsInsideCompletely(Point pos)
        {
            return InnerPosition.X <= pos.X && pos.X <= (InnerPosition.X + InnerWidth)
                && InnerPosition.Y <= pos.Y && pos.Y <= (InnerPosition.Y + InnerHeight);
        }

        internal Rect GetInnerBoundingBox()
        {
            return new Rect(InnerPosition, new Size(InnerWidth, InnerHeight));
        }

        protected override void OnUpdateTranslation()
        {
            _InternalPositionUpdating = true;

            InterlockPosition = Position;
            UpdateInnerPosition();

            _InternalPositionUpdating = false;
        }

        protected override void OnDisposing()
        {
            SizeChanged -= Group_SizeChanged;
            _GroupNodeHeader.SizeChanged -= GroupNodeHeader_SizeChanged;
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            if (IsDraggingToResize == false)
            {
                UpdateMouseCursor(e);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (IsDraggingToResize == false)
            {
                UpdateMouseCursor(e);
            }
        }

        void UpdateMouseCursor(MouseEventArgs e)
        {
            var pos = e.GetPosition(this);
            if (pos.X < BorderSize)
            {
                // left

                // cursor is inside left vertical border.
                if (pos.Y < BorderSize)
                {
                    // top
                    _IsDraggingToResizeType = DragResizeType.LeftTop;
                    Mouse.SetCursor(Cursors.SizeNWSE);
                }
                else if (pos.Y > ActualHeight - BorderSize)
                {
                    // bottom
                    _IsDraggingToResizeType = DragResizeType.LeftBottom;
                    Mouse.SetCursor(Cursors.SizeNESW);
                }
                else
                {
                    // left
                    _IsDraggingToResizeType = DragResizeType.Left;
                    Mouse.SetCursor(Cursors.SizeWE);
                }
            }
            else if (pos.X > (ActualWidth - BorderSize))
            {
                // right

                // cursor is inside right vertical border.
                if (pos.Y < BorderSize)
                {
                    // top
                    _IsDraggingToResizeType = DragResizeType.RightTop;
                    Mouse.SetCursor(Cursors.SizeNESW);
                }
                else if (pos.Y > ActualHeight - BorderSize)
                {
                    // bottom
                    _IsDraggingToResizeType = DragResizeType.RightBottom;
                    Mouse.SetCursor(Cursors.SizeNWSE);
                }
                else
                {
                    // right
                    _IsDraggingToResizeType = DragResizeType.Right;
                    Mouse.SetCursor(Cursors.SizeWE);
                }
            }
            else
            {
                // middle
                if (pos.Y < BorderSize)
                {
                    _IsDraggingToResizeType = DragResizeType.Top;
                    Mouse.SetCursor(Cursors.SizeNS);
                }
                else if (pos.Y > (ActualHeight - BorderSize))
                {
                    _IsDraggingToResizeType = DragResizeType.Bottom;
                    Mouse.SetCursor(Cursors.SizeNS);
                }
                else
                {
                    _IsDraggingToResizeType = DragResizeType.None;
                }
            }
        }

        void Group_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateInnerSize();
            UpdateInnerPosition();
            SizeChangedCommand?.Execute(e.NewSize);
        }

        void GroupNodeHeader_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateInnerSize();
            UpdateInnerPosition();

            Width = InnerWidth + BorderSize * 2;
            Height = InnerHeight + _GroupNodeHeader.ActualHeight + BorderSize * 2;
            UpdateLayout();
        }

        void UpdateInnerSize()
        {
            InnerWidth = Width - BorderSize * 2;
            InnerHeight = Height - BorderSize * 2 - _GroupNodeHeader.ActualHeight;

            // for notifying Width/Height immediately.
            UpdateLayout();
        }

        void UpdateWidthFromInnerWidth()
        {
            Width = InnerWidth + BorderSize * 2;

            // for notifying InnerWidth/InnerHeight immediately.
            UpdateLayout();
        }

        void UpdateHeightFromInnerHeight()
        {
            Height = InnerHeight + _GroupNodeHeader.ActualHeight + BorderSize * 2;

            // for notifying InnerWidth/InnerHeight immediately.
            UpdateLayout();
        }

        void UpdateInnerPosition()
        {
            var diff_x = BorderSize;
            var diff_y = _GroupNodeHeader.ActualHeight + BorderSize;
            InnerPosition = new Point(Position.X + diff_x, Position.Y + diff_y);

            // for notifying Position immediately.
            UpdateLayout();
        }

        static void InterlockPositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var groupNode = d as GroupNode;
            if (groupNode._InternalPositionUpdating)
            {
                return;
            }

            var move = groupNode.InterlockPosition - groupNode.Position;

            var otherNodes = groupNode.Canvas.Children.OfType<NodeBase>().Where(arg => arg != groupNode).ToArray();
            foreach (var node in otherNodes)
            {
                if (groupNode.IsInsideCompletely(node.GetBoundingBox()))
                {
                    node.Position = node.Position + move;
                }
            }

            groupNode.Position = groupNode.InterlockPosition;
        }

        static void InnerPositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var groupNode = d as GroupNode;
            if (groupNode._InternalPositionUpdating)
            {
                return;
            }

            var x = groupNode.InnerPosition.X - groupNode.BorderSize;
            var y = groupNode.InnerPosition.Y - (groupNode._GroupNodeHeader.ActualHeight + groupNode.BorderSize);
            groupNode.UpdatePosition(x, y);
        }

        static void InnerWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var groupNode = d as GroupNode;

            groupNode.UpdateWidthFromInnerWidth();
        }

        static void InnerHeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var groupNode = d as GroupNode;

            groupNode.UpdateHeightFromInnerHeight();
        }
    }
}
