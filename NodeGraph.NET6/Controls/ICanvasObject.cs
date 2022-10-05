using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace NodeGraph.NET6.Controls
{
    public interface ICanvasObject : IDisposable
    {
        Guid Guid { get; set; }
        void UpdateOffset(Point offset);
    }
}
