﻿namespace MainProject.Model
{
    public class Point
    {
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }

        public Point(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Point(double[] x)
        {
            if (x.Length < 3)
            {
                throw new System.Exception("Cast array to Point error");
            }
            X = x[0];
            Y = x[1];
            Z = x[2];
        }

        public Point()
        {
            X = 0;
            Y = 0;
            Z = 0;
        }
    }
}