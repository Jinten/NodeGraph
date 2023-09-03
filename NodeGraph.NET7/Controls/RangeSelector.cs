using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace NodeGraph.NET7.Controls
{
    public class RangeSelector : Shape
    {
        public Rect RangeRect
        {
            get => (Rect)GetValue(RangeRectProperty);
            set => SetValue(RangeRectProperty, value);
        }
        public static readonly DependencyProperty RangeRectProperty = DependencyProperty.Register(
            nameof(RangeRect),
            typeof(Rect),
            typeof(RangeSelector),
            new FrameworkPropertyMetadata(new Rect(), FrameworkPropertyMetadataOptions.AffectsRender));

        public bool IsIntersects
        {
            get => (bool)GetValue(IsIntersectsProperty);
            set => SetValue(IsIntersectsProperty, value);
        }
        public static readonly DependencyProperty IsIntersectsProperty = DependencyProperty.Register(
            nameof(IsIntersects),
            typeof(bool),
            typeof(RangeSelector),
            new FrameworkPropertyMetadata(false));

        protected override Geometry DefiningGeometry => Geometry.Empty;

        Pen _StrokePen = null;

        static RangeSelector()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RangeSelector), new FrameworkPropertyMetadata(typeof(RangeSelector)));
        }

        public void Reset(Point pos)
        {
            RangeRect = new Rect(pos, Size.Empty);
            IsIntersects = false;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            _StrokePen = new Pen(Stroke, StrokeThickness);
            _StrokePen.Freeze();

            drawingContext.DrawRectangle(Fill, _StrokePen, RangeRect);
        }
    }
}
