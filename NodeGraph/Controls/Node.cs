using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NodeGraph.Controls
{
    public class Node : ContentControl, ICanvasObject, IDisposable
    {
        public Guid Guid
        {
            get => (Guid)GetValue(GuidProperty);
            set => SetValue(GuidProperty, value);
        }
        public static readonly DependencyProperty GuidProperty = DependencyProperty.Register(
            nameof(Guid),
            typeof(Guid),
            typeof(Node),
            new PropertyMetadata(Guid.NewGuid()));

        public DataTemplate HeaderContentTemplate
        {
            get => (DataTemplate)GetValue(HeaderContentTemplateProperty);
            set => SetValue(HeaderContentTemplateProperty, value);
        }
        public static readonly DependencyProperty HeaderContentTemplateProperty = DependencyProperty.Register(
            nameof(HeaderContentTemplate),
            typeof(DataTemplate),
            typeof(Node),
            new FrameworkPropertyMetadata(null));

        public VerticalAlignment InputLayout
        {
            get => (VerticalAlignment)GetValue(InputLayoutProperty);
            set => SetValue(InputLayoutProperty, value);
        }
        public static readonly DependencyProperty InputLayoutProperty = DependencyProperty.Register(
            nameof(InputLayout),
            typeof(VerticalAlignment),
            typeof(Node),
            new FrameworkPropertyMetadata(VerticalAlignment.Top));

        public double InputMargin
        {
            get => (double)GetValue(InputMarginProperty);
            set => SetValue(InputMarginProperty, value);
        }
        public static readonly DependencyProperty InputMarginProperty = DependencyProperty.Register(
            nameof(InputMargin),
            typeof(double),
            typeof(Node),
            new FrameworkPropertyMetadata(2.0));

        public IEnumerable Inputs
        {
            get => GetValue(InputsProperty) as IEnumerable;
            set => SetValue(InputsProperty, value);
        }
        public static readonly DependencyProperty InputsProperty = DependencyProperty.Register(
            nameof(Inputs),
            typeof(IEnumerable),
            typeof(Node),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsArrange, InputsPropertyChanged));

        public Style InputStyle
        {
            get => GetValue(InputStyleProperty) as Style;
            set => SetValue(InputStyleProperty, value);
        }
        public static readonly DependencyProperty InputStyleProperty = DependencyProperty.Register(
            nameof(InputStyle),
            typeof(Style),
            typeof(Node),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public VerticalAlignment OutputLayout
        {
            get => (VerticalAlignment)GetValue(OutputLayoutProperty);
            set => SetValue(OutputLayoutProperty, value);
        }
        public static readonly DependencyProperty OutputLayoutProperty = DependencyProperty.Register(
            nameof(OutputLayout),
            typeof(VerticalAlignment),
            typeof(Node),
            new FrameworkPropertyMetadata(VerticalAlignment.Top));

        public double OutputMargin
        {
            get => (double)GetValue(OutputMarginProperty);
            set => SetValue(OutputMarginProperty, value);
        }
        public static readonly DependencyProperty OutputMarginProperty = DependencyProperty.Register(
            nameof(OutputMargin),
            typeof(double),
            typeof(Node),
            new FrameworkPropertyMetadata(2.0));

        public IEnumerable Outputs
        {
            get => GetValue(OutputsProperty) as IEnumerable;
            set => SetValue(OutputsProperty, value);
        }
        public static readonly DependencyProperty OutputsProperty = DependencyProperty.Register(
            nameof(Outputs),
            typeof(IEnumerable),
            typeof(Node),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsArrange, OutputsPropertyChanged));

        public Style OutputStyle
        {
            get => GetValue(OutputStyleProperty) as Style;
            set => SetValue(OutputStyleProperty, value);
        }
        public static readonly DependencyProperty OutputStyleProperty = DependencyProperty.Register(
            nameof(OutputStyle),
            typeof(Style),
            typeof(Node),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => UpdateSelectedState(value);
        }
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            nameof(IsSelected),
            typeof(bool),
            typeof(Node),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public Point Position
        {
            get => (Point)GetValue(PositionProperty);
            set => SetValue(PositionProperty, value);
        }
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(
            nameof(Position),
            typeof(Point),
            typeof(Node),
            new FrameworkPropertyMetadata(new Point(0, 0), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, PositionPropertyChanged));

        public Point DragStartPosition { get; private set; } = new Point(0, 0);

        Canvas Canvas { get; } = null;
        NodeInput _NodeInput = null;
        NodeOutput _NodeOutput = null;
        Point _Offset = new Point(0, 0);
        TranslateTransform _Translate = new TranslateTransform(0, 0);

        static void OutputsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var node = d as Node;

            if (e.OldValue is INotifyCollectionChanged oldCollection)
            {
                oldCollection.CollectionChanged -= node.OutputCollectionChanged;
            }
            if (e.NewValue is INotifyCollectionChanged newCollection)
            {
                newCollection.CollectionChanged += node.OutputCollectionChanged;
            }
        }

        static void PositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var node = d as Node;

            node.UpdatePosition();

            node._NodeInput.UpdateLinkPosition(node.Canvas);
            node._NodeOutput.UpdateLinkPosition(node.Canvas);
        }

        static void InputsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var node = d as Node;

            if (e.OldValue != null && e.OldValue is INotifyCollectionChanged oldCollection)
            {
                oldCollection.CollectionChanged -= node.InputCollectionChanged;
            }
            if (e.NewValue != null && e.NewValue is INotifyCollectionChanged newCollection)
            {
                newCollection.CollectionChanged += node.InputCollectionChanged;
            }
        }

        static Node()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Node), new FrameworkPropertyMetadata(typeof(Node)));
        }

        public Node(Canvas canvas, Point offset, double scale)
        {
            Canvas = canvas;
            SizeChanged += Node_SizeChanged;

            var transformGroup = new TransformGroup();

            _Offset = offset;

            UpdatePosition();

            transformGroup.Children.Add(_Translate);
            RenderTransform = transformGroup;
        }

        public override void OnApplyTemplate()
        {
            _NodeInput = GetTemplateChild("__NodeInput__") as NodeInput;
            _NodeOutput = GetTemplateChild("__NodeOutput__") as NodeOutput;
        }

        public NodeLink[] EnumrateConnectedNodeLinks()
        {
            var inputNodeLinks = _NodeInput.EnumerateConnectedNodeLinks();
            var outputNodeLinks = _NodeOutput.EnumerateConnectedNodeLinks();

            return inputNodeLinks.Concat(outputNodeLinks).ToArray();
        }

        public NodeConnectorContent FindNodeConnectorContent(Guid guid)
        {
            var input = _NodeInput.FindNodeConnector(guid);
            if (input != null)
            {
                return input;
            }

            return _NodeOutput.FindNodeConnector(guid);
        }

        public void UpdateOffset(Point offset)
        {
            _Offset = offset;

            UpdatePosition();

            InvalidateVisual();
        }

        public void UpdatePosition(double x, double y)
        {
            Position = new Point(x, y);

            UpdatePosition();

            _NodeInput.UpdateLinkPosition(Canvas);
            _NodeOutput.UpdateLinkPosition(Canvas);
        }

        public void CaptureDragStartPosition()
        {
            DragStartPosition = Position;
        }

        public void Dispose()
        {
            _NodeInput.Dispose();
            _NodeOutput.Dispose();
            SizeChanged -= Node_SizeChanged;
        }

        void UpdateSelectedState(bool value)
        {
            SetValue(IsSelectedProperty, value);
            Panel.SetZIndex(this, value ? 1 : 0);
        }

        void UpdatePosition()
        {
            _Translate.X = Position.X + _Offset.X;
            _Translate.Y = Position.Y + _Offset.Y;
        }

        void Node_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _NodeInput.UpdateLinkPosition(Canvas);
            _NodeOutput.UpdateLinkPosition(Canvas);
        }

        void OutputCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Implement if need anything.
        }

        void InputCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Implement if need anything.
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
        }
    }
}
