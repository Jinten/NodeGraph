using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NodeGraph.Extensions
{
    public static class RectExtension
    {
        public static bool IsInclude(this Rect me, Rect target)
        {
            return me.Left <= target.Left && target.Right <= me.Right && me.Top <= target.Top && target.Bottom <= me.Bottom;
        }
    }
}
