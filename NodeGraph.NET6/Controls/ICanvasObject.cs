﻿using System;
using System.Windows;

namespace NodeGraph.NET6.Controls
{
    public interface ICanvasObject : IDisposable
    {
        Guid Guid { get; set; }
        void UpdateOffset(Point offset);
    }
}
