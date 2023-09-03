using System.Windows;

namespace NodeGraph.NET6.Extensions
{
    public static class PointExtension
    {
        public static Point Add(this Point a, Point b)
        {
            return new Point(a.X + b.X, a.Y + b.Y);
        }

        public static Point Sub(this Point a, Point b)
        {
            return new Point(a.X - b.X, a.Y - b.Y);
        }

        public static Vector ToVector(this Point me)
        {
            return new Vector(me.X, me.Y);
        }
    }
}
