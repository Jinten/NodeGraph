using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace NodeGraph.Controls
{
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

        public double InnerWidth
        {
            get => (double)GetValue(InnerWidthProperty);
            set => SetValue(InnerWidthProperty, value);
        }
        public static readonly DependencyProperty InnerWidthProperty = DependencyProperty.Register(
            nameof(InnerWidth),
            typeof(double),
            typeof(GroupNode),
            new FrameworkPropertyMetadata(100.0));

        public double InnerHeight
        {
            get => (double)GetValue(InnerHeightProperty);
            set => SetValue(InnerHeightProperty, value);
        }
        public static readonly DependencyProperty InnerHeightProperty = DependencyProperty.Register(
            nameof(InnerHeight),
            typeof(double),
            typeof(GroupNode),
            new FrameworkPropertyMetadata(100.0));

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

        bool _IsInside = false;
        bool _InternalPositionUpdating = false;
        Border _GroupNodeHeader = null;

        static GroupNode()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GroupNode), new FrameworkPropertyMetadata(typeof(GroupNode)));
        }

        public GroupNode(Canvas canvas, Point offset, double scale) : base(canvas, offset)
        {
            SizeChanged += Group_SizeChanged;
        }

        public void Initialize()
        {

        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _GroupNodeHeader = GetTemplateChild("__GroupNodeHeader__") as Border;
        }

        public void Expand(Rect rect)
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
            InnerWidth = Width - BorderSize * 2;
            InnerHeight = Height - BorderSize * 2 - _GroupNodeHeader.ActualHeight;
        }

        public void ChangeInnerColor(bool inside)
        {
            if (_IsInside != inside)
            {
                _IsInside = inside;
                InnerColor = inside ? new SolidColorBrush(Color.FromArgb(128, 0, 255, 0)) : Brushes.Transparent;
            }
        }

        public bool IsInsideCompletely(Rect rect)
        {
            return Position.X < rect.X && rect.Right < (Position.X + ActualWidth) && Position.Y < rect.Y && rect.Bottom < (Position.Y + ActualHeight);
        }

        protected override void OnUpdateTranslation()
        {
            _InternalPositionUpdating = true;

            InterlockPosition = Position;

            _InternalPositionUpdating = false;
        }

        protected override void OnDisposing()
        {
            SizeChanged -= Group_SizeChanged;
        }

        void Group_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SizeChangedCommand?.Execute(e.NewSize);
        }

        static void InterlockPositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var groupNode = d as GroupNode;
            if(groupNode._InternalPositionUpdating)
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
    }
}
