using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhasePlotter.Graphics
{
    public class Point
    {
        public double X { get; private set; }
        public double Y { get; private set; }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public static Point operator -(Point a, Point b)
        {
            return new Point(a.X - b.X, a.Y - b.Y);
        }
        public static Point operator +(Point a, Point b)
        {
            return new Point(a.X + b.X, a.Y + b.Y);
        }

        public void DivideBy(double a)
        {
            X /= a;
            Y /= a;
        }

        public override string ToString()
        {
            return string.Format("{0:0.00};{1:0.00}", X, Y);
        }
    }
}
