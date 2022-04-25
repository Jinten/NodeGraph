using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace NodeGraph.Controls
{
    public class CanvasMouseEventArgs : MouseEventArgs
    {
        // This position has taken scale and offset into account.
        public Point TransformedPosition { get; }

        public CanvasMouseEventArgs(Point transformedPosition, MouseDevice mouse, int timestamp) : base(mouse, timestamp)
        {
            TransformedPosition = transformedPosition;
        }
    }
}
