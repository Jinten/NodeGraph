using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

namespace NodeGraph.NET7.Controls
{
    public abstract class NodeConnectorContent : ContentControl, IDisposable
    {
        public Guid Guid
        {
            get => (Guid)GetValue(GuidProperty);
            set => SetValue(GuidProperty, value);
        }
        public static readonly DependencyProperty GuidProperty = DependencyProperty.Register(
            nameof(Guid),
            typeof(Guid),
            typeof(NodeConnectorContent),
            new PropertyMetadata(Guid.Empty));

        public int ConnectedCount
        {
            get => (int)GetValue(ConnectedCountProperty);
            private set => SetValue(ConnectedCountPropertyKey, value);
        }
        public static readonly DependencyPropertyKey ConnectedCountPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(ConnectedCount),
            typeof(int),
            typeof(NodeConnectorContent),
            new PropertyMetadata(0));

        public static readonly DependencyProperty ConnectedCountProperty = ConnectedCountPropertyKey.DependencyProperty;

        public bool IsConnected
        {
            get => (bool)GetValue(IsConnectedProperty);
            private set => SetValue(IsConnectedPropertyKey, value);
        }
        public static readonly DependencyPropertyKey IsConnectedPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(IsConnected),
            typeof(bool),
            typeof(NodeConnectorContent),
            new PropertyMetadata(false));

        public static readonly DependencyProperty IsConnectedProperty = IsConnectedPropertyKey.DependencyProperty;

        public Brush Stroke
        {
            get => (Brush)GetValue(StrokeProperty);
            set => SetValue(StrokeProperty, value);
        }
        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(
            nameof(Stroke),
            typeof(Brush),
            typeof(NodeConnectorContent),
            new FrameworkPropertyMetadata(Brushes.Blue));

        public double StrokeThickness
        {
            get => (double)GetValue(StrokeThicknessProperty);
            set => SetValue(StrokeThicknessProperty, value);
        }
        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(
            nameof(StrokeThickness),
            typeof(double),
            typeof(NodeConnectorContent),
            new FrameworkPropertyMetadata(1.0));

        public Brush Fill
        {
            get => (Brush)GetValue(FillProperty);
            set => SetValue(FillProperty, value);
        }
        public static readonly DependencyProperty FillProperty = DependencyProperty.Register(
            nameof(Fill),
            typeof(Brush),
            typeof(NodeConnectorContent),
            new FrameworkPropertyMetadata(Brushes.Gray));

        public bool CanConnect
        {
            get => (bool)GetValue(CanConnectProperty);
            set => SetValue(CanConnectProperty, value);
        }
        public static readonly DependencyProperty CanConnectProperty = DependencyProperty.Register(
            nameof(CanConnect),
            typeof(bool),
            typeof(NodeConnectorContent),
            new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

        public Point Position
        {
            get => _Position;
            set => UpdatePosition(value);
        }

        public DefaultNode Node { get; private set; } = null;

        /// <summary>
        /// connecting node links.
        /// </summary>
        public IEnumerable<NodeLink> NodeLinks => _NodeLinks;
        List<NodeLink> _NodeLinks = new List<NodeLink>();

        protected abstract FrameworkElement ConnectorControl { get; }

        Point _Position = new Point();
        TranslateTransform _Translate = new TranslateTransform();

        static public bool CanConnectEachOther(NodeConnectorContent start, NodeConnectorContent toEnd)
        {
            return start.CanConnectTo(toEnd) && toEnd.CanConnectTo(start);
        }

        public NodeConnectorContent()
        {
            var transfromGroup = new TransformGroup();
            transfromGroup.Children.Add(_Translate);
            RenderTransform = transfromGroup;
        }

        public void Initialize()
        {
            var parent = VisualTreeHelper.GetParent(this);
            while (parent.GetType() != typeof(DefaultNode))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            Node = parent as DefaultNode;
        }

        public void Dispose()
        {
            // You need to clear Style.
            // Because implemented on style for binding.
            Style = null;

            // Clear binding for subscribing source changed event from old control.
            // throw exception about visual tree ancestor different if you not clear binding.
            BindingOperations.ClearAllBindings(this);

            var nodeLinks = _NodeLinks.ToArray();

            // it must instance to nodeLinks because change node link collection in NodeLink Dispose.
            foreach (var nodeLink in nodeLinks)
            {
                nodeLink.Dispose();
            }
        }

        public void Connect(NodeLink nodeLink)
        {
            _NodeLinks.Add(nodeLink);
            ConnectedCount = _NodeLinks.Count;
            IsConnected = ConnectedCount > 0;
        }

        public void Disconnect(NodeLink nodeLink)
        {
            _NodeLinks.Remove(nodeLink);
            ConnectedCount = _NodeLinks.Count;
            IsConnected = ConnectedCount > 0;
        }

        public Point GetContentPosition(Canvas canvas, double xScaleOffset = 0.5, double yScaleOffset = 0.5)
        {
            // it will be shifted Control position if not called UpdateLayout().
            ConnectorControl.UpdateLayout();
            var transformer = ConnectorControl.TransformToVisual(canvas);

            var x = ConnectorControl.ActualWidth * xScaleOffset;
            var y = ConnectorControl.ActualHeight * yScaleOffset;
            return transformer.Transform(new Point(x, y));
        }

        public abstract void UpdateLinkPosition(Canvas canvas);
        public abstract bool CanConnectTo(NodeConnectorContent connector);

        void UpdatePosition(Point pos)
        {
            _Position = pos;
            _Translate.X = _Position.X;
            _Translate.Y = _Position.Y;

            InvalidateVisual();
        }
    }

    public abstract class NodeConnector<T> : MultiSelector, IDisposable where T : NodeConnectorContent, new()
    {
        public Thickness ConnectorMargin
        {
            get => (Thickness)GetValue(ConnectorMarginProperty);
            set => SetValue(ConnectorMarginProperty, value);
        }
        public static readonly DependencyProperty ConnectorMarginProperty = DependencyProperty.Register(
            nameof(ConnectorMargin),
            typeof(Thickness),
            typeof(NodeConnector<T>),
            new FrameworkPropertyMetadata(new Thickness(2.0), ConnectorMarginPropertyChanged));

        /// <summary>
        /// must implement connector canvas name.
        /// </summary>
        protected abstract string ConnectorCanvasName { get; }

        /// <summary>
        /// must implement connector content template.
        /// </summary>
        protected abstract ControlTemplate NodeConnectorContentTemplate { get; }

        /// <summary>
        /// must implement connector content style.
        /// </summary>
        protected abstract Style NodeConnectorContentBaseStyle { get; }

        Canvas _Canvas = null;


        static void ConnectorMarginPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as NodeConnector<T>).UpdateConnectorsLayout();
        }

        public void Initialize()
        {
            foreach (var connector in _Canvas.Children.OfType<NodeConnectorContent>())
            {
                // ItemContainerStyle is still null during applying template, so need to apply at initialize.(ApplyTemplate later)
                connector.Style = ItemContainerStyle;
                connector.Initialize();
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

            var connectors = _Canvas.Children.OfType<NodeConnectorContent>().ToArray();

            foreach (var connector in connectors)
            {
                connector.Dispose();
            }
        }

        public NodeLink[] EnumerateConnectedNodeLinks()
        {
            var connectors = _Canvas.Children.OfType<NodeConnectorContent>().ToArray();
            return connectors.SelectMany(arg => arg.NodeLinks).ToArray();
        }

        public T FindNodeConnector(Guid guid)
        {
            var connectors = _Canvas.Children.OfType<T>().ToArray();
            return connectors.FirstOrDefault(arg => arg.Guid == guid);
        }

        public void UpdateLinkPosition(Canvas canvas)
        {
            if (_Canvas == null)
            {
                return;
            }

            foreach (var connector in _Canvas.Children.OfType<NodeConnectorContent>())
            {
                connector.UpdateLinkPosition(canvas);
            }
        }

        public void UpdateConnectorsLayout()
        {
            if (_Canvas == null)
            {
                return;
            }

            ReplaceConnectElements(0);

            var connectorContents = _Canvas.Children.OfType<NodeConnectorContent>();

            if (connectorContents.Count() > 0)
            {
                var numElement = connectorContents.Count();
                Width = connectorContents.Max(arg => arg.ActualWidth);
                Height = connectorContents.Sum(arg => arg.ActualHeight) + numElement * (ConnectorMargin.Top + ConnectorMargin.Bottom) + numElement; // last adding numElement is 1px border each other.
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _Canvas = GetTemplateChild(ConnectorCanvasName) as Canvas;
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);

            if (oldValue != null)
            {
                RemoveConnectorFromCanvas(oldValue.OfType<object>());
            }
            if (newValue != null)
            {
                AddConnectorsToCanvas(newValue.OfType<object>());
            }

            UpdateConnectorsLayout();
        }

        void RemoveConnectorFromCanvas(IEnumerable<object> removeVMs)
        {
            var removeElements = new List<T>();
            var children = _Canvas.Children.OfType<T>();

            foreach (var removeVM in removeVMs)
            {
                var removeElement = children.First(arg => arg.DataContext == removeVM);
                removeElements.Add(removeElement);
            }

            foreach (var element in removeElements)
            {
                element.SizeChanged -= Connector_SizeChanged;
                element.Dispose();
                _Canvas.Children.Remove(element);
            }
        }

        void AddConnectorsToCanvas(IEnumerable<object> addVMs)
        {
            foreach (var vm in addVMs)
            {
                var connector = new T()
                {
                    DataContext = vm,
                    Template = NodeConnectorContentTemplate,
                };
                connector.ApplyTemplate();

                connector.SizeChanged += Connector_SizeChanged;
                _Canvas.Children.Add(connector);
            }
        }

        void Connector_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateConnectorsLayout();
        }

        void ReplaceConnectElements(double offset)
        {
            int index = 0;
            foreach (var element in _Canvas.Children.OfType<NodeConnectorContent>())
            {
                double margin = index * (ConnectorMargin.Top + ConnectorMargin.Bottom) + ConnectorMargin.Top;
                element.Padding = new Thickness(ConnectorMargin.Left, 0, ConnectorMargin.Right, 0);
                element.Position = new Point(0, offset + element.ActualHeight * index + margin + index); // last adding index for non overlap other connector.
                ++index;
            }
        }
    }
}
