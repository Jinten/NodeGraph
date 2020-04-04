using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NodeGraph.Utilities
{
    public class CatmullRomSpline
    {
        static public Vector Calc(double t, Vector p0, Vector p1, Vector p2, Vector p3)
        {
            Vector result;

            if (t == 0)
            {
                result = CalcFirst(t, p0, p1, p2);
            }
            else if (t == 1.0)
            {
                result = CalcLast(t, p1, p2, p3);
            }
            else
            {
                result = CalcMiddle(t, p0, p1, p2, p3);
            }

            return result;
        }

        static public Point Calc(double t, Point p0, Point p1, Point p2, Point p3)
        {
            var result = Calc(
                t,
                new Vector(p0.X, p0.Y),
                new Vector(p1.X, p1.Y),
                new Vector(p2.X, p2.Y),
                new Vector(p3.X, p3.Y));

            return new Point(result.X, result.Y);
        }

        static public Vector CalcFirst(double t, Vector p0, Vector p1, Vector p2)
        {
            Vector b = p0 - 2.0 * p1 + p2;
            Vector c = -3.0 * p0 + 4.0 * p1 - p2;
            Vector d = 2.0 * p0;

            return 0.5 * ((b * t * t) + (c * t) + d);
        }

        static public Vector CalcMiddle(double t, Vector p0, Vector p1, Vector p2, Vector p3)
        {
            Vector a = -p0 + 3.0 * p1 - 3.0 * p2 + p3;
            Vector b = 2.0 * p0 - 5.0 * p1 + 4.0 * p2 - p3;
            Vector c = -p0 + p2;
            Vector d = 2.0 * p1;

            return 0.5 * ((a * t * t * t) + (b * t * t) + (c * t) + d);
        }

        static public Vector CalcLast(double t, Vector p0, Vector p1, Vector p2)
        {
            Vector b = p0 - 2.0 * p1 + p2;
            Vector c = -p0 + p2;
            Vector d = 2.0 * p1;

            return 0.5 * ((b * t * t) + (c * t) + d);
        }
    }
}
