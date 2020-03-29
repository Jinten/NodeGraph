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
    public enum InputLayoutType
    {
        Top,
        Center,
        Bottom,
    }

    public class NodeInputContent : ContentControl
    {
        static NodeInputContent()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NodeInputContent), new FrameworkPropertyMetadata(typeof(NodeInputContent)));
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            //TODO: implement if need anything.
        }
    }

    internal class NodeInputs : MultiSelector
    {
        public InputLayoutType InputLayout
        {
            get => (InputLayoutType)GetValue(InputLayoutProperty);
            set => SetValue(InputLayoutProperty, value);
        }
        public static readonly DependencyProperty InputLayoutProperty = DependencyProperty.Register(
            nameof(InputLayout),
            typeof(InputLayoutType),
            typeof(NodeInputs),
            new FrameworkPropertyMetadata(InputLayoutType.Top, InputLayoutPropertyChanged));

        public double InputMargin
        {
            get => (double)GetValue(InputMarginProperty);
            set => SetValue(InputMarginProperty, value);
        }
        public static readonly DependencyProperty InputMarginProperty = DependencyProperty.Register(
            nameof(InputMargin),
            typeof(double),
            typeof(NodeInputs),
            new FrameworkPropertyMetadata(2.0, InputMarginPropertyChanged));

        ControlTemplate NodeInputTemplate
        {
            get
            {
                if (_NodeInputTemplate == null)
                {
                    _NodeInputTemplate = Application.Current.TryFindResource("__NodeInputTemplate__") as ControlTemplate;
                }
                return _NodeInputTemplate;
            }
        }
        ControlTemplate _NodeInputTemplate = null;

        Canvas _Canvas = null;
        bool _IsStyleInitialized = false;
        List<object> _DelayToBindVMs = new List<object>();


        static void InputMarginPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as NodeInputs).UpdateInputsLayout();
        }

        static void InputLayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as NodeInputs).UpdateInputsLayout();
        }

        static NodeInputs()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NodeInputs), new FrameworkPropertyMetadata(typeof(NodeInputs)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _Canvas = GetTemplateChild("__NodeInputsCanvas__") as Canvas;
        }

        protected override void OnItemContainerStyleChanged(Style oldItemContainerStyle, Style newItemContainerStyle)
        {
            base.OnItemContainerStyleChanged(oldItemContainerStyle, newItemContainerStyle);

            _IsStyleInitialized = true;

            if (_DelayToBindVMs.Count > 0 && _Canvas != null)
            {
                AddNodesToCanvas(_DelayToBindVMs.OfType<object>());

                UpdateInputsLayout();

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
                RemoveInputsFromCanvas(oldValue.OfType<object>());
                AddNodesToCanvas(newValue.OfType<object>());

                UpdateInputsLayout();
            }
        }

        void RemoveInputsFromCanvas(IEnumerable<object> removeVMs)
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

        void AddNodesToCanvas(IEnumerable<object> addVMs)
        {
            foreach (var vm in addVMs)
            {
                var node = new NodeInputContent() { DataContext = vm, Template = NodeInputTemplate, Style = ItemContainerStyle };

                _Canvas.Children.Add(node);
            }
        }

        void UpdateInputsLayout()
        {
            switch (InputLayout)
            {
                case InputLayoutType.Top:
                    ReplaceInputElements(0, FontSize);
                    break;
                case InputLayoutType.Center:
                    {
                        var halfSize = _Canvas.Children.Count * FontSize * 0.5;
                        ReplaceInputElements(ActualHeight * 0.5 - halfSize, FontSize);
                        break;
                    }
                case InputLayoutType.Bottom:
                    {
                        var totalSize = _Canvas.Children.Count * FontSize;
                        ReplaceInputElements(ActualHeight - totalSize, FontSize);
                        break;
                    }
            }
        }

        void ReplaceInputElements(double offset, double margin)
        {
            int index = 0;
            foreach (var element in _Canvas.Children.OfType<FrameworkElement>())
            {
                element.Margin = new Thickness(0, offset + margin * index * InputMargin, 0, 0);
                ++index;
            }
        }
    }

    public class Node : Control, INode
    {
        public InputLayoutType InputLayout
        {
            get => (InputLayoutType)GetValue(InputLayoutProperty);
            set => SetValue(InputLayoutProperty, value);
        }
        public static readonly DependencyProperty InputLayoutProperty = DependencyProperty.Register(
            nameof(InputLayout),
            typeof(InputLayoutType),
            typeof(Node),
            new FrameworkPropertyMetadata(InputLayoutType.Top));

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

        bool _IsDragging = false;
        Point _DragOffset = new Point(0, 0);

        NodeGraph _Owner = null;
        NodeInputs _NodeInputs = null;


        static Node()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Node), new FrameworkPropertyMetadata(typeof(Node)));
        }

        public Node(NodeGraph owner)
        {
            _Owner = owner;
        }

        public override void OnApplyTemplate()
        {
            _NodeInputs = GetTemplateChild("__NodeInputs__") as NodeInputs;
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

        void InputCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Implement if need anything.
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            _IsDragging = true;

            _DragOffset = e.GetPosition(this);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if(_IsDragging == false)
            {
                return;
            }

            var point = _Owner.GetDragNodePosition(e);
            Margin = new Thickness(point.X - _DragOffset.X, point.Y - _DragOffset.Y, 0, 0);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            _IsDragging = false;
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            _IsDragging = false;
        }
    }
}
