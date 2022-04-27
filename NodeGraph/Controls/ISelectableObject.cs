using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NodeGraph.Controls
{
    internal interface ISelectableObject
    {
        bool IsSelected { get; set; }
        object DataContext { get; set; }

        bool Contains(Rect rect);
        bool IntersectsWith(Rect rect);
    }
}
