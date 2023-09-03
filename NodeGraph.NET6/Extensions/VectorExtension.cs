using System.Windows;

namespace NodeGraph.NET6.Extensions
{
    public static class VectorExtension
    {
        public static double DotProduct(this Vector a, Vector b)
        {
            return a.X * b.X + a.Y * b.Y;
        }

        public static Vector NormalizeTo(this Vector v)
        {
            var temp = v;
            temp.Normalize();

            return temp;
        }
    }
}
