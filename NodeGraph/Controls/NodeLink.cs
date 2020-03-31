using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace NodeGraph.Controls
{
    public class NodeLink : Shape, IDisposable
    {
        public Point EndPoint { get; private set; }
        public Point StartPoint { get; private set; }

        public double BorderThickness
        {
            get => (double)GetValue(BorderThicknessProperty);
            set => SetValue(BorderThicknessProperty, value);
        }
        public static readonly DependencyProperty BorderThicknessProperty = 
            DependencyProperty.Register(nameof(BorderThickness), typeof(double), typeof(NodeLink), new FrameworkPropertyMetadata(2.0));

        public NodeInputContent Input { get; private set; } = null;
        public NodeOutputContent Output { get; private set; } = null;

        protected override Geometry DefiningGeometry => Geometry.Empty;

        Pen _HitVisiblePen = null;

        public NodeLink(double x, double y, NodeInputContent input) : this(x, y)
        {
            Input = input;
        }

        public NodeLink(double x, double y, NodeOutputContent output) : this(x, y)
        {
            Output = output;
        }

        NodeLink(double x, double y)
        {
            IsHitTestVisible = false; // no need to hit until connected

            var point = new Point(x, y);
            StartPoint = point;
            EndPoint = point;

            Canvas.SetZIndex(this, -1);
        }

        public void Dispose()
        {
            Input = null;
            Output = null;
        }

        public void Connect(NodeInputContent input)
        {
            Input = input;
            IsHitTestVisible = true;
        }

        public void Connect(NodeOutputContent output)
        {
            Output = output;
            IsHitTestVisible = true;
        }

        public void UpdateEdgePoint(double x, double y)
        {
            if(Input == null)
            {
                // first connecting output to input.
                EndPoint = new Point(x, y);
            }
            else if(Output == null)
            {
                // first connecting input to output.
                StartPoint = new Point(x, y);
            }
            else
            {
                // reconnecting.
                EndPoint = new Point(x, y);
            }

            InvalidateVisual();
        }

        public void ReleaseEndPoint()
        {
            IsHitTestVisible = false;
        }

        public void RestoreEndPoint()
        {
            IsHitTestVisible = true;
        }

        public void UpdateInputEdge(double x, double y)
        {
            EndPoint = new Point(x, y);
            InvalidateVisual();
        }

        public void UpdateOutputEdge(double x, double y)
        {
            StartPoint = new Point(x, y);
            InvalidateVisual();
        }

        public void Disconnect()
        {
            Input.Disconnect(this);
            Output.Disconnect(this);
        }

        protected override void OnStyleChanged(Style oldStyle, Style newStyle)
        {
            base.OnStyleChanged(oldStyle, newStyle);

            _HitVisiblePen = new Pen(Brushes.Transparent, StrokeThickness + BorderThickness);
            _HitVisiblePen.Freeze();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            // collision
            if (IsHitTestVisible)
            {
                drawingContext.DrawLine(_HitVisiblePen, StartPoint, EndPoint);
            }

            // actually visual
            drawingContext.DrawLine(new Pen(Stroke, StrokeThickness), StartPoint, EndPoint);

            base.OnRender(drawingContext);
        }
    }
}
