using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace NodeGraph.Controls
{
    public interface ICanvasObject
    {
        void UpdateScale(double scale, Point focus);
        void UpdateOffset(Point offset);
    }
}
