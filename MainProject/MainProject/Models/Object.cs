using System;
using static TestProblemIpOpt.Enums;

namespace hs071_cs
{
    public class Object
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double R { get; set; }
        public double Weight { get => Weight; private set => Weight = (4.0 / 3 * Math.PI * Math.Pow(R, 3.0)); }
        public CircularObjectType ObjectType { get; set; }

        public Object()
        {
            ObjectType = 0;
        }

        public Object(double x, double y, double z, double r, CircularObjectType objectType)
        {
            X = x;
            Y = y;
            Z = z;
            R = r;
            ObjectType = objectType;
        }
    }
}