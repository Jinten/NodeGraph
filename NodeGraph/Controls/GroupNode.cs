using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace NodeGraph.Controls
{
    public class GroupNode : NodeBase
    {
        public DataTemplate HeaderContentTemplate
        {
            get => (DataTemplate)GetValue(HeaderContentTemplateProperty);
            set => SetValue(HeaderContentTemplateProperty, value);
        }
        public static readonly DependencyProperty HeaderContentTemplateProperty = DependencyProperty.Register(
            nameof(HeaderContentTemplate),
            typeof(DataTemplate),
            typeof(GroupNode),
            new FrameworkPropertyMetadata(null));

        public double InnerWidth
        {
            get => (double)GetValue(InnerWidthProperty);
            set => SetValue(InnerWidthProperty, value);
        }
        public static readonly DependencyProperty InnerWidthProperty = DependencyProperty.Register(
            nameof(InnerWidth),
            typeof(double),
            typeof(GroupNode),
            new FrameworkPropertyMetadata(100.0));

        public double InnerHeight
        {
            get => (double)GetValue(InnerHeightProperty);
            set => SetValue(InnerHeightProperty, value);
        }
        public static readonly DependencyProperty InnerHeightProperty = DependencyProperty.Register(
            nameof(InnerHeight),
            typeof(double),
            typeof(GroupNode),
            new FrameworkPropertyMetadata(100.0));

        public ICommand SizeChangedCommand
        {
            get => GetValue(SizeChangedCommandProperty) as ICommand;
            set => SetValue(SizeChangedCommandProperty, value);
        }
        public static readonly DependencyProperty SizeChangedCommandProperty = DependencyProperty.Register(
            nameof(SizeChangedCommand),
            typeof(ICommand),
            typeof(GroupNode),
            new FrameworkPropertyMetadata(null));

        static GroupNode()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GroupNode), new FrameworkPropertyMetadata(typeof(GroupNode)));
        }

        public GroupNode(Canvas canvas, Point offset, double scale) : base(canvas, offset)
        {
            SizeChanged += Group_SizeChanged;
        }

        public void Initialize()
        {

        }

        protected override void OnUpdateTranslation()
        {
            // TODO:
        }

        protected override void OnDisposing()
        {
            SizeChanged -= Group_SizeChanged;
        }

        void Group_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SizeChangedCommand?.Execute(e.NewSize);
        }
    }
}
