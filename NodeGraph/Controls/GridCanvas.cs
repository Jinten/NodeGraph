using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NodeGraph.Controls
{
    public class GridCanvas : Canvas
    {
        public Point Offset
        {
            get => (Point)GetValue(OffsetProperty);
            set => SetValue(OffsetProperty, value);
        }
        public static readonly DependencyProperty OffsetProperty =
            DependencyProperty.Register(nameof(Offset), typeof(Point), typeof(GridCanvas), new FrameworkPropertyMetadata(new Point(0, 0), OffsetPropertyChanged));

        public double Scale
        {
            get => (double)GetValue(ScaleProperty);
            set => SetValue(ScaleProperty, value);
        }
        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register(nameof(Scale), typeof(double), typeof(GridCanvas), new FrameworkPropertyMetadata(1.0, ScalePropertyChanged));

        public double Spacing
        {
            get => (double)GetValue(SpacingProperty);
            set => SetValue(SpacingProperty, value);
        }
        public static readonly DependencyProperty SpacingProperty =
            DependencyProperty.Register(nameof(Spacing), typeof(double), typeof(GridCanvas), new FrameworkPropertyMetadata(128.0));

        public int SubDivisionCount
        {
            get => (int)GetValue(SubDivisionCountProperty);
            set => SetValue(SubDivisionCountProperty, value);
        }
        public static readonly DependencyProperty SubDivisionCountProperty =
            DependencyProperty.Register(nameof(SubDivisionCount), typeof(int), typeof(GridCanvas), new FrameworkPropertyMetadata(3));

        public Brush GridMainBrush
        {
            get => (Brush)GetValue(GridMainBrushProperty);
            set => SetValue(GridMainBrushProperty, value);
        }
        public static readonly DependencyProperty GridMainBrushProperty =
            DependencyProperty.Register(nameof(GridMainBrush), typeof(Brush), typeof(GridCanvas), new FrameworkPropertyMetadata(Brushes.Gray, GridMainBrushPropertyChanged));

        public Brush GridSubBrush
        {
            get => (Brush)GetValue(GridSubBrushProperty);
            set => SetValue(GridSubBrushProperty, value);
        }
        public static readonly DependencyProperty GridSubBrushProperty =
            DependencyProperty.Register(nameof(GridSubBrush), typeof(Brush), typeof(GridCanvas), new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 64, 64, 64)), GridSubBrushPropertyChanged));

        public double GridMainThickness
        {
            get => (double)GetValue(GridMainThicknessProperty);
            set => SetValue(GridMainThicknessProperty, value);
        }
        public static readonly DependencyProperty GridMainThicknessProperty =
            DependencyProperty.Register(nameof(GridMainThickness), typeof(double), typeof(GridCanvas), new FrameworkPropertyMetadata(1.0, GridMainThicknessPropertyChanged));

        public double GridSubThickness
        {
            get => (double)GetValue(GridSubThicknessProperty);
            set => SetValue(GridSubThicknessProperty, value);
        }
        public static readonly DependencyProperty GridSubThicknessProperty =
            DependencyProperty.Register(nameof(GridSubThickness), typeof(double), typeof(GridCanvas), new FrameworkPropertyMetadata(1.0, GridSubThicknessPropertyChanged));

        Pen _GridMainPen = null;
        Pen _GridSubPen = null;
        ScaleTransform _Scale = new ScaleTransform(1.0, 1.0);

        static void OffsetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as GridCanvas).InvalidateVisual();
        }

        static void ScalePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var canvas = (d as GridCanvas);
            canvas.UpdateScale();
            canvas.InvalidateVisual();
        }

        static void GridMainBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as GridCanvas).UpdateMainGridPen();
        }

        static void GridSubBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as GridCanvas).UpdateMainGridPen();
        }

        static void GridMainThicknessPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as GridCanvas).UpdateSubGridPen();
        }

        static void GridSubThicknessPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as GridCanvas).UpdateSubGridPen();
        }

        static GridCanvas()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GridCanvas), new FrameworkPropertyMetadata(typeof(GridCanvas)));
        }

        public GridCanvas()
        {
            var transfromGroup = new TransformGroup();
            transfromGroup.Children.Add(_Scale);
            RenderTransform = transfromGroup;
            RenderTransformOrigin = new Point(0.5, 0.5);

            UpdateMainGridPen();
            UpdateSubGridPen();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            int subGridCount = SubDivisionCount + 1;
            double space = Spacing / subGridCount;
            double offset = space * subGridCount;

            // main horizon
            int hCount = (int)(ActualHeight / Spacing) + 1;
            for (int i = 0; i < hCount; ++i)
            {
                // sub horizon
                for (int sub = 0; sub < subGridCount; ++sub)
                {
                    double vOfs = CalcOffset(Offset.Y, space);
                    double hSub = sub * space + i * offset + vOfs;
                    dc.DrawLine(_GridSubPen, new Point(0, hSub), new Point(ActualWidth, hSub));
                }

                double h = i * Spacing + CalcOffset(Offset.Y, Spacing);
                dc.DrawLine(_GridMainPen, new Point(0, h), new Point(ActualWidth, h));
            }

            // main vertical
            int vCount = (int)(ActualWidth / Spacing) + 1;
            for (uint i = 0; i < vCount; ++i)
            {
                // sub vertical
                for (uint sub = 0; sub < subGridCount; ++sub)
                {
                    double hOfs = CalcOffset(Offset.X, space);
                    double vSub = sub * space + i * offset + hOfs;
                    dc.DrawLine(_GridSubPen, new Point(vSub, 0), new Point(vSub, ActualHeight));
                }

                double v = i * Spacing + CalcOffset(Offset.X, Spacing);
                dc.DrawLine(_GridMainPen, new Point(v, 0), new Point(v, ActualHeight));
            }
        }

        void UpdateMainGridPen()
        {
            _GridMainPen = new Pen(GridMainBrush, GridMainThickness);
            _GridMainPen.Freeze();
        }

        void UpdateSubGridPen()
        {
            _GridSubPen = new Pen(GridSubBrush, GridSubThickness);
            _GridSubPen.DashStyle = DashStyles.Dash;
            _GridSubPen.Freeze();
        }

        void UpdateScale()
        {
            _Scale.ScaleX = Scale;
            _Scale.ScaleY = Scale;
        }

        double CalcOffset(double offset, double repeat)
        {
            var sign = Math.Sign(offset);
            var absOffset = Math.Abs(offset);

            var result = absOffset > repeat ? absOffset % repeat : absOffset;

            return result * sign;
        }
    }
}
