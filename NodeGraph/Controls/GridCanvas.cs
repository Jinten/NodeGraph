using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NodeGraph.Controls
{
    /// <summary>
    /// Infinity grid rendering canvas.
    /// </summary>
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
            DependencyProperty.Register(nameof(GridMainBrush), typeof(Brush), typeof(GridCanvas), new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromRgb(80,80,80)), GridMainBrushPropertyChanged));

        public Brush GridSubBrush
        {
            get => (Brush)GetValue(GridSubBrushProperty);
            set => SetValue(GridSubBrushProperty, value);
        }
        public static readonly DependencyProperty GridSubBrushProperty =
            DependencyProperty.Register(nameof(GridSubBrush), typeof(Brush), typeof(GridCanvas), new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromRgb(64, 64, 64)), GridSubBrushPropertyChanged));

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

            UpdateMainGridPen();
            UpdateSubGridPen();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            // render grid only visual area.
            int subGridCount = SubDivisionCount + 1;
            double subGridSpace = Spacing / subGridCount;
            double subGridOffset = subGridSpace * subGridCount;

            double MinHorizonOnCanvas = -ActualWidth * Math.Max(0.0, 1.0 - Scale) * 1.0 / Scale * 0.5;
            double MaxHorizonOnCanvas = +ActualWidth * (1 + Math.Max(0.0, 1.0 - Scale) * 1.0 / Scale * 0.5);

            double MinVerticalOnCanvas = -ActualHeight * Math.Max(0.0, 1.0 - Scale) * 1.0 / Scale * 0.5;
            double MaxVerticalOnCanvas = +ActualHeight * (1 + Math.Max(0.0, 1.0 - Scale) * 1.0 / Scale * 0.5);

            double invScale = 1.0 / Scale;

            int hCount = (int)(ActualHeight * invScale / Spacing + Math.Ceiling(Scale));
            int initHIndex = (int)(Math.Max(0, ActualHeight * invScale - ActualHeight) / Spacing);

            int vCount = (int)(ActualWidth * invScale / Spacing + Math.Ceiling(Scale)) + 2;
            int initVIndex = (int)(Math.Max(0, ActualWidth * invScale - ActualWidth) / Spacing);

            // sub horizon
            for (int i = -initHIndex; i < hCount; ++i)
            {
                for (int sub = 0; sub < subGridCount; ++sub)
                {
                    double hSub = sub * subGridSpace + i * subGridOffset + (Offset.Y % subGridSpace);
                    dc.DrawLine(_GridSubPen, new Point(MinHorizonOnCanvas, hSub), new Point(MaxHorizonOnCanvas, hSub));
                }
            }

            // sub vertical
            for (int i = -initVIndex; i < vCount; ++i)
            {
                for (uint sub = 0; sub < subGridCount; ++sub)
                {
                    double vSub = sub * subGridSpace + i * subGridOffset + (Offset.X % subGridSpace);
                    dc.DrawLine(_GridSubPen, new Point(vSub, MinVerticalOnCanvas), new Point(vSub, MaxVerticalOnCanvas));
                }
            }

            // main horizontal
            for (int i = -initHIndex; i < hCount; ++i)
            {
                double h = i * Spacing + Offset.Y % Spacing;
                dc.DrawLine(_GridMainPen, new Point(MinHorizonOnCanvas, h), new Point(MaxHorizonOnCanvas, h));
            }

            // main vertical
            for (int i = -initVIndex; i < vCount; ++i)
            {
                double v = i * Spacing + Offset.X % Spacing;
                dc.DrawLine(_GridMainPen, new Point(v, MinVerticalOnCanvas), new Point(v, MaxVerticalOnCanvas));
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
            _GridSubPen.Freeze();
        }

        void UpdateScale()
        {
            _Scale.ScaleX = Scale;
            _Scale.ScaleY = Scale;
        }
    }
}
