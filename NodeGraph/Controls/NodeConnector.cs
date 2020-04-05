using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace NodeGraph.Controls
{
    public enum ConnectorLayoutType
    {
        Top,
        Center,
        Bottom,
    }

    public abstract class NodeConnectorContent : ContentControl
    {
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

        /// <summary>
        /// connecting node links.
        /// </summary>
        protected IEnumerable<NodeLink> NodeLinks => _NodeLinks;
        List<NodeLink> _NodeLinks = new List<NodeLink>();

        protected abstract FrameworkElement ConnectorControl { get; }

        public void Connect(NodeLink nodeLink)
        {
            _NodeLinks.Add(nodeLink);
        }

        public void Disconnect(NodeLink nodeLink)
        {
            _NodeLinks.Remove(nodeLink);
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
    }

    public abstract class NodeConnector<T> : MultiSelector where T : NodeConnectorContent, new()
    {
        public ConnectorLayoutType ConnectorLayout
        {
            get => (ConnectorLayoutType)GetValue(ConnectorLayoutProperty);
            set => SetValue(ConnectorLayoutProperty, value);
        }
        public static readonly DependencyProperty ConnectorLayoutProperty = DependencyProperty.Register(
            nameof(ConnectorLayout),
            typeof(ConnectorLayoutType),
            typeof(NodeConnector<T>),
            new FrameworkPropertyMetadata(ConnectorLayoutType.Top, ConnectorLayoutPropertyChanged));

        public double ConnectorMargin
        {
            get => (double)GetValue(ConnectorMarginProperty);
            set => SetValue(ConnectorMarginProperty, value);
        }
        public static readonly DependencyProperty ConnectorMarginProperty = DependencyProperty.Register(
            nameof(ConnectorMargin),
            typeof(double),
            typeof(NodeConnector<T>),
            new FrameworkPropertyMetadata(2.0, ConnectorMarginPropertyChanged));

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
        bool _IsStyleInitialized = false;
        List<object> _DelayToBindVMs = new List<object>();


        static void ConnectorMarginPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as NodeConnector<T>).UpdateConnectorsLayout();
        }

        static void ConnectorLayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as NodeConnector<T>).UpdateConnectorsLayout();
        }

        public void UpdateLinkPosition(Canvas canvas)
        {
            foreach (var connector in _Canvas.Children.OfType<NodeConnectorContent>())
            {
                connector.UpdateLinkPosition(canvas);
            }
        }

        public void UpdateConnectorsLayout()
        {
            switch (ConnectorLayout)
            {
                case ConnectorLayoutType.Top:
                    ReplaceConnectElements(0, FontSize);
                    break;
                case ConnectorLayoutType.Center:
                    {
                        var halfSize = _Canvas.Children.Count * FontSize * 0.5;
                        ReplaceConnectElements(ActualHeight * 0.5 - halfSize, FontSize);
                        break;
                    }
                case ConnectorLayoutType.Bottom:
                    {
                        var totalSize = _Canvas.Children.Count * FontSize;
                        ReplaceConnectElements(ActualHeight - totalSize, FontSize);
                        break;
                    }
            }

            var connectorContents = _Canvas.Children.OfType<NodeConnectorContent>();

            if (connectorContents.Count() > 0)
            {
                Width = connectorContents.Max(arg => arg.ActualWidth);
                Height = connectorContents.Sum(arg => arg.ActualHeight) + connectorContents.Count() * ConnectorMargin;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _Canvas = GetTemplateChild(ConnectorCanvasName) as Canvas;
        }

        protected override void OnItemContainerStyleChanged(Style oldItemContainerStyle, Style newItemContainerStyle)
        {
            base.OnItemContainerStyleChanged(oldItemContainerStyle, newItemContainerStyle);

            _IsStyleInitialized = true;

            if (_DelayToBindVMs.Count > 0 && _Canvas != null)
            {
                AddConnectorsToCanvas(_DelayToBindVMs.OfType<object>());

                UpdateConnectorsLayout();

                _DelayToBindVMs.Clear();
            }
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);
            if (_Canvas == null || _IsStyleInitialized == false)
            {
                _DelayToBindVMs.AddRange(newValue.OfType<object>());
            }
            else
            {
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
        }

        void RemoveConnectorFromCanvas(IEnumerable<object> removeVMs)
        {
            var removeElements = new List<UIElement>();
            var children = _Canvas.Children.OfType<FrameworkElement>();

            foreach (var removeVM in removeVMs)
            {
                var removeElement = children.First(arg => arg.DataContext == removeVM);
                removeElements.Add(removeElement);
            }

            removeElements.ForEach(arg => _Canvas.Children.Remove(arg));
        }

        void AddConnectorsToCanvas(IEnumerable<object> addVMs)
        {
            foreach (var vm in addVMs)
            {
                var connector = new T()
                {
                    DataContext = vm,
                    Template = NodeConnectorContentTemplate,
                    Style = ItemContainerStyle
                };

                connector.SizeChanged += ConnectorSizeChanged;
                _Canvas.Children.Add(connector);
            }
        }

        void ConnectorSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateConnectorsLayout();
        }

        void ReplaceConnectElements(double offset, double margin)
        {
            int index = 0;
            foreach (var element in _Canvas.Children.OfType<FrameworkElement>())
            {
                element.Margin = new Thickness(0, offset + margin * index * ConnectorMargin, 0, 0);
                ++index;
            }
        }
    }
}
