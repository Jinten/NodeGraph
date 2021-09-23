using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NodeGraph.Controls
{
    public abstract class NodeBase : ContentControl, ICanvasObject, IDisposable
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

        public EventHandler BeginSelectionChanged { get; set; } = null;
        public EventHandler EndSelectionChanged { get; set; } = null;

        protected Canvas Canvas { get; } = null;
        protected Point Offset { get; private set; } = new Point(0, 0);
        protected TranslateTransform Translate { get; private set; } = new TranslateTransform(0, 0);


        public NodeBase(Canvas canvas, Point offset)
        {
            Focusable = true;

            Canvas = canvas;
            Offset = offset;

            Translate.X = Position.X + Offset.X;
            Translate.Y = Position.Y + Offset.Y;

            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(Translate);

            RenderTransform = transformGroup;
        }

        public Rect GetBoundingBox()
        {
            return new Rect(Position.X, Position.Y, ActualWidth, ActualHeight);
        }

        public void CaptureDragStartPosition()
        {
            DragStartPosition = Position;
        }

        public void UpdatePosition(double x, double y)
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

        public void Dispose()
        {
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
            var node = d as NodeBase;

            node.BeginSelectionChanged?.Invoke(node, EventArgs.Empty);

            node.Focus();

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
