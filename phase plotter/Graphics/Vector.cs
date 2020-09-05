using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhasePlotter.Graphics
{
    public class Vector
    {
        public Point Start { get; set; }
        public Point Finish { get; set; }
        public double Length
        {
            get
            {
                var p = Finish - Start;
                return Math.Sqrt(p.X * p.X + p.Y * p.Y);
            }
        }

        public Vector(Point start, Point finish)
        {
            Start = start;
            Finish = finish;
        }
        public Vector(Point finish)
        {
            Start = new Point(0, 0);
            Finish = finish;
        }

        public static Vector operator *(Vector a, double b)
        {
            var p = a.Finish - a.Start;
            return new Vector(a.Start, new Point(p.X * b, p.Y * b) + a.Start);
        }

        public Vector Normalize()
        {
            var zerobased = new Vector(this.Finish - this.Start);
            var length = Math.Sqrt(Math.Pow(zerobased.Finish.X, 2) + Math.Pow(zerobased.Finish.Y, 2));
            zerobased.Finish.DivideBy(length);
            return new Vector(this.Start, zerobased.Finish + this.Start);
        }
        public Vector RotateDeg(double angleDeg)
        {
            return this.Rotate(Math.PI * angleDeg / 180);
        }
        public Vector Rotate(double angle)
        {
            var v = new Vector(this.Finish - this.Start);
            var x = v.Finish.X * Math.Cos(angle) - v.Finish.Y * Math.Sin(angle);
            var y = v.Finish.X * Math.Sin(angle) + v.Finish.Y * Math.Cos(angle);
            return new Vector(this.Start, this.Start + new Point(x, y));
        }


        public override string ToString()
        {
            return "(" + Start + "," + Finish + ")";
        }
    }
}
