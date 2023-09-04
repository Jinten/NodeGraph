using System.Windows;

namespace NodeGraph.NET6.Controls
{
    internal interface ISelectableObject
    {
        bool IsSelected { get; set; }
        object DataContext { get; set; }

        bool Contains(Rect rect);
        bool IntersectsWith(Rect rect);
    }
}
