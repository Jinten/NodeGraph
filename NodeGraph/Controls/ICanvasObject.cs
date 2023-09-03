using System;
using System.Windows;

namespace NodeGraph.Controls
{
    public interface ICanvasObject : IDisposable
    {
        Guid Guid { get; set; }
        void UpdateOffset(Point offset);
    }
}
