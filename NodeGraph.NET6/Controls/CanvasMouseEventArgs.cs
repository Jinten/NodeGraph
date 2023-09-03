using System;
using System.Windows;

namespace NodeGraph.NET6.Controls
{
    public class CanvasMouseEventArgs : EventArgs
    {
        // This position has taken scale and offset into account.
        public Point TransformedPosition { get; }

        public CanvasMouseEventArgs(Point transformedPosition)
        {
            TransformedPosition = transformedPosition;
        }
    }
}
