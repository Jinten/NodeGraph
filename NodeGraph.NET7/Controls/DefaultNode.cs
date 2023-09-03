using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NodeGraph.NET7.Controls
{
    public class DefaultNode : NodeBase
    {
        public DataTemplate HeaderContentTemplate
        {
            get => (DataTemplate)GetValue(HeaderContentTemplateProperty);
            set => SetValue(HeaderContentTemplateProperty, value);
        }
        public static readonly DependencyProperty HeaderContentTemplateProperty = DependencyProperty.Register(
            nameof(HeaderContentTemplate),
            typeof(DataTemplate),
            typeof(DefaultNode),
            new FrameworkPropertyMetadata(null));

        public Thickness ContentMargin
        {
            get => (Thickness)GetValue(ContentMarginProperty);
            set => SetValue(ContentMarginProperty, value);
        }
        public static readonly DependencyProperty ContentMarginProperty = DependencyProperty.Register(
            nameof(ContentMargin),
            typeof(Thickness),
            typeof(DefaultNode),
            new FrameworkPropertyMetadata(new Thickness(4, 0, 4, 0), FrameworkPropertyMetadataOptions.AffectsArrange));

        public VerticalAlignment InputLayout
        {
            get => (VerticalAlignment)GetValue(InputLayoutProperty);
            set => SetValue(InputLayoutProperty, value);
        }
        public static readonly DependencyProperty InputLayoutProperty = DependencyProperty.Register(
            nameof(InputLayout),
            typeof(VerticalAlignment),
            typeof(DefaultNode),
            new FrameworkPropertyMetadata(VerticalAlignment.Top));

        public Thickness InputMargin
        {
            get => (Thickness)GetValue(InputMarginProperty);
            set => SetValue(InputMarginProperty, value);
        }
        public static readonly DependencyProperty InputMarginProperty = DependencyProperty.Register(
            nameof(InputMargin),
            typeof(Thickness),
            typeof(DefaultNode),
            new FrameworkPropertyMetadata(new Thickness(2.0)));

        public IEnumerable Inputs
        {
            get => (IEnumerable)GetValue(InputsProperty);
            set => SetValue(InputsProperty, value);
        }
        public static readonly DependencyProperty InputsProperty = DependencyProperty.Register(
            nameof(Inputs),
            typeof(IEnumerable),
            typeof(DefaultNode),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsArrange, InputsPropertyChanged));

        public Style InputStyle
        {
            get => (Style)GetValue(InputStyleProperty);
            set => SetValue(InputStyleProperty, value);
        }
        public static readonly DependencyProperty InputStyleProperty = DependencyProperty.Register(
            nameof(InputStyle),
            typeof(Style),
            typeof(DefaultNode),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public VerticalAlignment OutputLayout
        {
            get => (VerticalAlignment)GetValue(OutputLayoutProperty);
            set => SetValue(OutputLayoutProperty, value);
        }
        public static readonly DependencyProperty OutputLayoutProperty = DependencyProperty.Register(
            nameof(OutputLayout),
            typeof(VerticalAlignment),
            typeof(DefaultNode),
            new FrameworkPropertyMetadata(VerticalAlignment.Top));

        public Thickness OutputMargin
        {
            get => (Thickness)GetValue(OutputMarginProperty);
            set => SetValue(OutputMarginProperty, value);
        }
        public static readonly DependencyProperty OutputMarginProperty = DependencyProperty.Register(
            nameof(OutputMargin),
            typeof(Thickness),
            typeof(DefaultNode),
            new FrameworkPropertyMetadata(new Thickness(2.0)));

        public IEnumerable Outputs
        {
            get => (IEnumerable)GetValue(OutputsProperty);
            set => SetValue(OutputsProperty, value);
        }
        public static readonly DependencyProperty OutputsProperty = DependencyProperty.Register(
            nameof(Outputs),
            typeof(IEnumerable),
            typeof(DefaultNode),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsArrange, OutputsPropertyChanged));

        public Style OutputStyle
        {
            get => (Style)GetValue(OutputStyleProperty);
            set => SetValue(OutputStyleProperty, value);
        }
        public static readonly DependencyProperty OutputStyleProperty = DependencyProperty.Register(
            nameof(OutputStyle),
            typeof(Style),
            typeof(DefaultNode),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public ICommand SizeChangedCommand
        {
            get => (ICommand)GetValue(SizeChangedCommandProperty);
            set => SetValue(SizeChangedCommandProperty, value);
        }
        public static readonly DependencyProperty SizeChangedCommandProperty = DependencyProperty.Register(
            nameof(SizeChangedCommand),
            typeof(ICommand),
            typeof(DefaultNode),
            new FrameworkPropertyMetadata(null));

        NodeInput _NodeInput = null;
        NodeOutput _NodeOutput = null;

        ColumnDefinition _NodeInputGridColumnDefinition;
        ColumnDefinition _NodeContentTemplateGridColumnDefinition;
        ColumnDefinition _NodeOutputGridColumnDefinition;

        static void OutputsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var node = d as DefaultNode;

            if (e.OldValue is INotifyCollectionChanged oldCollection)
            {
                oldCollection.CollectionChanged -= node.OutputCollectionChanged;
            }
            if (e.NewValue is INotifyCollectionChanged newCollection)
            {
                newCollection.CollectionChanged += node.OutputCollectionChanged;
            }
        }

        static void InputsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var node = d as DefaultNode;

            if (e.OldValue != null && e.OldValue is INotifyCollectionChanged oldCollection)
            {
                oldCollection.CollectionChanged -= node.InputCollectionChanged;
            }
            if (e.NewValue != null && e.NewValue is INotifyCollectionChanged newCollection)
            {
                newCollection.CollectionChanged += node.InputCollectionChanged;
            }
        }

        static DefaultNode()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DefaultNode), new FrameworkPropertyMetadata(typeof(DefaultNode)));
        }

        internal DefaultNode(Canvas canvas, Point offset, double scale) : base(canvas, offset)
        {
            SizeChanged += Node_SizeChanged;
        }

        public override void OnApplyTemplate()
        {
            _NodeInput = (NodeInput)GetTemplateChild("__NodeInput__");
            _NodeInput.ApplyTemplate();

            _NodeOutput = (NodeOutput)GetTemplateChild("__NodeOutput__");
            _NodeOutput.ApplyTemplate();

            _NodeInputGridColumnDefinition = (ColumnDefinition)GetTemplateChild("__NodeInputGridColumnDefinition__");
            _NodeContentTemplateGridColumnDefinition = (ColumnDefinition)GetTemplateChild("__NodeContentTemplateGridColumnDefinition__");
            _NodeOutputGridColumnDefinition = (ColumnDefinition)GetTemplateChild("__NodeOutputGridColumnDefinition__");
        }

        internal void Initialize()
        {
            _NodeInput.Initialize();
            _NodeOutput.Initialize();
        }

        internal NodeLink[] EnumrateConnectedNodeLinks()
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

        protected override void OnDisposing()
        {
            _NodeInput.Dispose();
            _NodeOutput.Dispose();
            SizeChanged -= Node_SizeChanged;
        }

        protected override void OnUpdateTranslation()
        {
            _NodeInput.UpdateLinkPosition(Canvas);
            _NodeOutput.UpdateLinkPosition(Canvas);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
        }

        void Node_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _NodeInput.UpdateLinkPosition(Canvas);
            _NodeOutput.UpdateLinkPosition(Canvas);

            // Collapse input/output controls if not placement input/output connectors.            
            _NodeInputGridColumnDefinition.Width = _NodeInput.HasItems ? new GridLength(1.0f, GridUnitType.Star) : new GridLength(0);
            _NodeOutputGridColumnDefinition.Width = _NodeOutput.HasItems ? new GridLength(1.0f, GridUnitType.Star) : new GridLength(0);

            SizeChangedCommand?.Execute(e.NewSize);
        }

        void OutputCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Implement if need anything.
        }

        void InputCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Implement if need anything.
        }
    }
}
