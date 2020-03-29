using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace NodeGraph.Controls
{
    public class NodeInputContent : ContentControl
    {
        static NodeInputContent()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NodeInputContent), new FrameworkPropertyMetadata(typeof(NodeInputContent)));
        }

        NodeInput Owner { get; } = null;

        public NodeInputContent(NodeInput owner)
        {
            Owner = owner;
            SizeChanged += NodeInputContent_SizeChanged;
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            //TODO: implement if need anything.
        }

        void NodeInputContent_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Owner.UpdateInputsLayout();
        }
    }

    public class NodeInput : MultiSelector
    {
        public InputLayoutType InputLayout
        {
            get => (InputLayoutType)GetValue(InputLayoutProperty);
            set => SetValue(InputLayoutProperty, value);
        }
        public static readonly DependencyProperty InputLayoutProperty = DependencyProperty.Register(
            nameof(InputLayout),
            typeof(InputLayoutType),
            typeof(NodeInput),
            new FrameworkPropertyMetadata(InputLayoutType.Top, InputLayoutPropertyChanged));

        public double InputMargin
        {
            get => (double)GetValue(InputMarginProperty);
            set => SetValue(InputMarginProperty, value);
        }
        public static readonly DependencyProperty InputMarginProperty = DependencyProperty.Register(
            nameof(InputMargin),
            typeof(double),
            typeof(NodeInput),
            new FrameworkPropertyMetadata(2.0, InputMarginPropertyChanged));

        ControlTemplate NodeInputTemplate
        {
            get
            {
                if (_NodeInputTemplate == null)
                {
                    _NodeInputTemplate = Application.Current.TryFindResource("__NodeInputContentTemplate__") as ControlTemplate;
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
            (d as NodeInput).UpdateInputsLayout();
        }

        static void InputLayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as NodeInput).UpdateInputsLayout();
        }

        static NodeInput()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NodeInput), new FrameworkPropertyMetadata(typeof(NodeInput)));
        }

        public void UpdateInputsLayout()
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

            var inputs = _Canvas.Children.OfType<NodeInputContent>();

            Width = inputs.Max(arg => arg.ActualWidth);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _Canvas = GetTemplateChild("__NodeInputCanvas__") as Canvas;
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
                var node = new NodeInputContent(this) { DataContext = vm, Template = NodeInputTemplate, Style = ItemContainerStyle };

                _Canvas.Children.Add(node);
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
}
