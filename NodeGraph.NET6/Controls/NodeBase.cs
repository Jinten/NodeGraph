using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace NodeGraph.NET6.Controls
{
    public abstract class NodeBase : ContentControl, ICanvasObject, ISelectableObject, IDisposable
    {
        public Guid Guid
        {
            get => (Guid)GetValue(GuidProperty);
            set => SetValue(GuidProperty, value);
        }
        public static readonly DependencyProperty GuidProperty = DependencyProperty.Register(
            nameof(Guid),
            typeof(Guid),
            typeof(NodeBase),
            new PropertyMetadata(Guid.NewGuid()));

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => UpdateSelectedState(value);
        }
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            nameof(IsSelected),
            typeof(bool),
            typeof(NodeBase),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, IsSelectedPropertyChanged));

        public Point Position
        {
            get => (Point)GetValue(PositionProperty);
            set => SetValue(PositionProperty, value);
        }
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(
            nameof(Position),
            typeof(Point),
            typeof(NodeBase),
            new FrameworkPropertyMetadata(new Point(0, 0), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, PositionPropertyChanged));

        public Point DragStartPosition { get; private set; } = new Point(0, 0);

        internal EventHandler BeginSelectionChanged { get; set; } = null;
        internal EventHandler EndSelectionChanged { get; set; } = null;

        protected Canvas Canvas { get; } = null;
        protected Point Offset { get; private set; } = new Point(0, 0);
        protected TranslateTransform Translate { get; private set; } = new TranslateTransform(0, 0);


        internal NodeBase(Canvas canvas, Point offset)
        {
            Canvas = canvas;
            Offset = offset;

            Translate.X = Position.X + Offset.X;
            Translate.Y = Position.Y + Offset.Y;

            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(Translate);

            RenderTransform = transformGroup;
        }

        internal Rect GetBoundingBox()
        {
            return new Rect(Position.X, Position.Y, ActualWidth, ActualHeight);
        }

        internal void CaptureDragStartPosition()
        {
            DragStartPosition = Position;
        }

        internal void UpdatePosition(double x, double y)
        {
            Position = new Point(x, y);

            UpdateTranslation();
        }

        public void UpdateOffset(Point offset)
        {
            Offset = offset;

            UpdateTranslation();

            InvalidateVisual();
        }

        public bool Contains(Rect rect)
        {
            return rect.Contains(GetBoundingBox());
        }

        public bool IntersectsWith(Rect rect)
        {
            return GetBoundingBox().IntersectsWith(rect);
        }

        public void Dispose()
        {
            // You need to clear Style.
            // Because implemented on style for binding.
            Style = null;

            // Clear binding for subscribing source changed event from old control.
            // throw exception about visual tree ancestor different if you not clear binding.
            BindingOperations.ClearAllBindings(this);

            // Clear binding myself first.
            // Because children throw exception about visual tree ancestor different when my Style to be null.
            OnDisposing();
        }

        void UpdateSelectedState(bool value)
        {
            SetValue(IsSelectedProperty, value);
            Panel.SetZIndex(this, value ? 1 : 0);
        }

        void UpdateTranslation()
        {
            Translate.X = Position.X + Offset.X;
            Translate.Y = Position.Y + Offset.Y;

            OnUpdateTranslation();
        }

        static void IsSelectedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var node = (NodeBase)d;

            node.BeginSelectionChanged?.Invoke(node, EventArgs.Empty);

            if (node.Focusable)
            {
                node.Focus();
            }

            node.EndSelectionChanged?.Invoke(node, EventArgs.Empty);
        }

        static void PositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as NodeBase).UpdateTranslation();
        }

        protected abstract void OnDisposing();
        protected abstract void OnUpdateTranslation();
    }
}
