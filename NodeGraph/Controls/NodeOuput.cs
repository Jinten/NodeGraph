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
    public class NodeOutputContent : ContentControl
    {
        static NodeOutputContent()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NodeOutputContent), new FrameworkPropertyMetadata(typeof(NodeOutputContent)));
        }

        NodeOutput Owner { get; } = null;

        public NodeOutputContent(NodeOutput owner)
        {
            Owner = owner;
            SizeChanged += NodeOutputContent_SizeChanged;
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            //TODO: implement if need anything.
        }

        void NodeOutputContent_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Owner.UpdateOutputsLayout();
        }
    }

    public class NodeOutput : MultiSelector
    {
        public OutputLayoutType OutputLayout
        {
            get => (OutputLayoutType)GetValue(OutputLayoutProperty);
            set => SetValue(OutputLayoutProperty, value);
        }
        public static readonly DependencyProperty OutputLayoutProperty = DependencyProperty.Register(
            nameof(OutputLayout),
            typeof(OutputLayoutType),
            typeof(NodeOutput),
            new FrameworkPropertyMetadata(OutputLayoutType.Top, OutputLayoutPropertyChanged));

        public double OutputMargin
        {
            get => (double)GetValue(OutputMarginProperty);
            set => SetValue(OutputMarginProperty, value);
        }
        public static readonly DependencyProperty OutputMarginProperty = DependencyProperty.Register(
            nameof(OutputMargin),
            typeof(double),
            typeof(NodeOutput),
            new FrameworkPropertyMetadata(2.0, OutputMarginPropertyChanged));

        ControlTemplate NodeOutputTemplate
        {
            get
            {
                if (_NodeOutputTemplate == null)
                {
                    _NodeOutputTemplate = Application.Current.TryFindResource("__NodeOutputContentTemplate__") as ControlTemplate;
                }
                return _NodeOutputTemplate;
            }
        }
        ControlTemplate _NodeOutputTemplate = null;

        Canvas _Canvas = null;
        bool _IsStyleInitialized = false;
        List<object> _DelayToBindVMs = new List<object>();


        static void OutputMarginPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as NodeOutput).UpdateOutputsLayout();
        }

        static void OutputLayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as NodeOutput).UpdateOutputsLayout();
        }

        static NodeOutput()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NodeOutput), new FrameworkPropertyMetadata(typeof(NodeOutput)));
        }

        public void UpdateOutputsLayout()
        {
            switch (OutputLayout)
            {
                case OutputLayoutType.Top:
                    ReplaceOutputElements(0, FontSize);
                    break;
                case OutputLayoutType.Center:
                    {
                        var halfSize = _Canvas.Children.Count * FontSize * 0.5;
                        ReplaceOutputElements(ActualHeight * 0.5 - halfSize, FontSize);
                        break;
                    }
                case OutputLayoutType.Bottom:
                    {
                        var totalSize = _Canvas.Children.Count * FontSize;
                        ReplaceOutputElements(ActualHeight - totalSize, FontSize);
                        break;
                    }
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _Canvas = GetTemplateChild("__NodeOutputCanvas__") as Canvas;
        }

        protected override void OnItemContainerStyleChanged(Style oldItemContainerStyle, Style newItemContainerStyle)
        {
            base.OnItemContainerStyleChanged(oldItemContainerStyle, newItemContainerStyle);

            _IsStyleInitialized = true;

            if (_DelayToBindVMs.Count > 0 && _Canvas != null)
            {
                AddNodesToCanvas(_DelayToBindVMs.OfType<object>());

                UpdateOutputsLayout();

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
                RemoveOutputsFromCanvas(oldValue.OfType<object>());
                AddNodesToCanvas(newValue.OfType<object>());

                UpdateOutputsLayout();
            }
        }

        void RemoveOutputsFromCanvas(IEnumerable<object> removeVMs)
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
                var node = new NodeOutputContent(this) { DataContext = vm, Template = NodeOutputTemplate, Style = ItemContainerStyle };

                _Canvas.Children.Add(node);
            }
        }

        void ReplaceOutputElements(double offset, double margin)
        {
            int index = 0;
            foreach (var element in _Canvas.Children.OfType<FrameworkElement>())
            {
                element.Margin = new Thickness(0, offset + margin * index * OutputMargin, 0, 0);
                ++index;
            }
        }
    }
}
