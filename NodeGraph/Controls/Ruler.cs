using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NodeGraph.Controls
{
    internal class Ruler : ContentControl
    {
        public double Scale
        {
            get => (double)GetValue(ScaleProperty);
            set => SetValue(ScaleProperty, value);
        }
        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register(nameof(Scale), typeof(double), typeof(Ruler), new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public Point Offset
        {
            get => (Point)GetValue(OffsetProperty);
            set => SetValue(OffsetProperty, value);
        }
        public static readonly DependencyProperty OffsetProperty =
            DependencyProperty.Register(nameof(Offset), typeof(Point), typeof(Ruler), new FrameworkPropertyMetadata(new Point(0, 0), FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush Color
        {
            get => (Brush)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register(nameof(Color), typeof(Brush), typeof(Ruler), new FrameworkPropertyMetadata(Brushes.Black, ColorPropertyChanged));

        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(Ruler), new FrameworkPropertyMetadata(Orientation.Horizontal));

        Pen _Pen = null;
        static Typeface Typeface = new Typeface("Verdana");
        static double LineOffset = 3;
        static double LineLength = 5 + LineOffset;
        static double LineDistance = 100;
        static double SubLineOffset = 1;
        static double SubLineLength = 5 + SubLineOffset;
        static double SubLineDistance = LineDistance / 10;

        static void ColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Ruler).UpdatePen();
        }

        static Ruler()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Ruler), new FrameworkPropertyMetadata(typeof(Ruler)));
        }

        public Ruler()
        {

        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (_Pen == null)
            {
                UpdatePen();
            }

            switch (Orientation)
            {
                case Orientation.Horizontal:
                    OnRenderHorizontal(drawingContext);
                    break;
                case Orientation.Vertical:
                    OnRenderVertical(drawingContext);
                    break;
            }
        }

        void OnRenderHorizontal(DrawingContext dc)
        {
            double s = Math.Max(Scale, 1);
            double numScale = Math.Max(1.0 / Scale, 1);

            int init = (int)(-Offset.X / LineDistance) - 1;
            int count = (int)((-Offset.X + ActualWidth) / LineDistance) + 1;
            for (int i = init; i < count; ++i)
            {
                double num = i * LineDistance;
                double x = (num + Offset.X) * s;
                dc.DrawLine(_Pen, new Point(x, LineOffset), new Point(x, LineLength));

                for (int j = 1; j < 10; ++j)
                {
                    double sub_x = x + j * SubLineDistance * s;
                    dc.DrawLine(_Pen, new Point(sub_x, SubLineOffset), new Point(sub_x, SubLineLength));
                }

                int numText = (int)(num * numScale);
                var text = new FormattedText($"{numText}", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, Typeface, 8, Color, 1.0);
                dc.DrawText(text, new Point(x - text.Width * 0.5, LineLength));
            }
        }

        void OnRenderVertical(DrawingContext dc)
        {
            double s = Math.Max(Scale, 1);
            double numScale = Math.Max(1.0 / Scale, 1);

            int init = (int)(-Offset.Y / LineDistance) - 1;
            int count = (int)((-Offset.Y + ActualHeight) / LineDistance) + 1;
            for (int i = init; i < count; ++i)
            {
                double num = i * LineDistance;
                double y = (num + Offset.Y) * s;
                dc.DrawLine(_Pen, new Point(LineOffset, y), new Point(LineLength, y));

                for (int j = 1; j < 10; ++j)
                {
                    double sub_y = y + j * SubLineDistance * s;
                    dc.DrawLine(_Pen, new Point(SubLineOffset, sub_y), new Point(SubLineLength, sub_y));
                }

                int numText = (int)(num * numScale);
                var text = new FormattedText($"{numText}", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, Typeface, 8, Color, 1.0);
                dc.DrawText(text, new Point(LineLength + LineOffset, y - text.Height * 0.5));
            }
        }

        void UpdatePen()
        {
            _Pen = new Pen(Color, 1);
            _Pen.Freeze();
        }
    }
}
