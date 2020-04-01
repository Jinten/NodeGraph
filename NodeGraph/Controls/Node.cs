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

        public ConnectorLayoutType InputLayout
		{
			get => (ConnectorLayoutType)GetValue(InputLayoutProperty);
			set => SetValue(InputLayoutProperty, value);
		}
		public static readonly DependencyProperty InputLayoutProperty = DependencyProperty.Register(
			nameof(InputLayout),
			typeof(ConnectorLayoutType),
			typeof(Node),
			new FrameworkPropertyMetadata(ConnectorLayoutType.Top));

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

		public ConnectorLayoutType OutputLayout
		{
			get => (ConnectorLayoutType)GetValue(OutputLayoutProperty);
			set => SetValue(OutputLayoutProperty, value);
		}
		public static readonly DependencyProperty OutputLayoutProperty = DependencyProperty.Register(
			nameof(OutputLayout),
			typeof(ConnectorLayoutType),
			typeof(Node),
			new FrameworkPropertyMetadata(ConnectorLayoutType.Top));

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
			set => SetValue(IsSelectedProperty, value);
		}
		public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
			nameof(IsSelected),
			typeof(bool),
			typeof(Node),
			new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public Point DragStartPosition { get; private set; } = new Point(0, 0);
        
		Canvas Canvas { get; } = null;
		NodeInput _NodeInput = null;
        NodeOutput _NodeOutput = null;
        Point _Offset = new Point(0, 0);
        Point _Position = new Point(0, 0);
        ScaleTransform _Scale = new ScaleTransform(1, 1);
        TranslateTransform _Translate = new TranslateTransform(0, 0);

        static void OutputsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var node = d as Node;

            if (e.OldValue != null && e.OldValue is INotifyCollectionChanged oldCollection)
            {
                oldCollection.CollectionChanged -= node.OutputCollectionChanged;
            }
            if (e.NewValue != null && e.NewValue is INotifyCollectionChanged newCollection)
            {
                newCollection.CollectionChanged += node.OutputCollectionChanged;
            }
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

		public Node(Canvas canvas, double scale)
		{
            Canvas = canvas;
            SizeChanged += Node_SizeChanged;

            var transformGroup = new TransformGroup();
            _Scale.ScaleX = scale;
            _Scale.ScaleY = scale;
            transformGroup.Children.Add(_Scale);
            transformGroup.Children.Add(_Translate);
            RenderTransform = transformGroup;
            RenderTransformOrigin = new Point(0.5, 0.5);
		}

        public override void OnApplyTemplate()
		{
			_NodeInput = GetTemplateChild("__NodeInput__") as NodeInput;
            _NodeOutput = GetTemplateChild("__NodeOutput__") as NodeOutput;
        }

        public void UpdateScale(double scale, Point focus)
        {
            Vector n = new Vector(focus.X, focus.Y);
            n.Normalize();

            RenderTransformOrigin = new Point(n.X, n.Y);

            _Scale.ScaleX = scale;
            _Scale.ScaleY = scale;

            UpdatePosition();

            InvalidateVisual();
        }

        public void UpdateOffset(Point offset)
        {
            _Offset = offset;

            UpdatePosition();

            InvalidateVisual();
        }

        public void UpdatePosition(double x, double y)
        {
            _Position.X = x;
            _Position.Y = y;

            UpdatePosition();

            _NodeInput.UpdateLinkPosition(Canvas);
            _NodeOutput.UpdateLinkPosition(Canvas);
        }

        public void CaptureDragStartPosition()
        {
            DragStartPosition = _Position;
        }

		public void Dispose()
		{
            SizeChanged -= Node_SizeChanged;
		}

        void UpdatePosition()
        {
            _Translate.X = (_Position.X + _Offset.X) * _Scale.ScaleX;
            _Translate.Y = (_Position.Y + _Offset.Y) * _Scale.ScaleY;
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
