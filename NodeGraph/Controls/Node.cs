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

    public enum OutputLayoutType
    {
        Top,
        Center,
        Bottom,
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

        public OutputLayoutType OutputLayout
        {
            get => (OutputLayoutType)GetValue(OutputLayoutProperty);
            set => SetValue(OutputLayoutProperty, value);
        }
        public static readonly DependencyProperty OutputLayoutProperty = DependencyProperty.Register(
            nameof(OutputLayout),
            typeof(OutputLayoutType),
            typeof(Node),
            new FrameworkPropertyMetadata(OutputLayoutType.Top));

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

        bool _IsDragging = false;
        Point _DragOffset = new Point(0, 0);

        NodeGraph Owner { get; } = null;
        NodeInput _NodeInputs = null;


        static Node()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Node), new FrameworkPropertyMetadata(typeof(Node)));
        }

        public Node(NodeGraph owner)
        {
            Owner = owner;
        }

        public override void OnApplyTemplate()
        {
            _NodeInputs = GetTemplateChild("__NodeInput__") as NodeInput;
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

        void OutputCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Implement if need anything.
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

            var point = Owner.GetDragNodePosition(e);
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
