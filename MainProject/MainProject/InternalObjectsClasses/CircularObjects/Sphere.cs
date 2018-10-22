using MainProject.Enums;
using MainProject.Model;
using PackageProject.Interfaces;
using System;

namespace PackageProject.InternalObjectsClasses.CircularObjects
{
    internal class Sphere : ISphere
    {
        public double Radius { get; private set; }

        /// <summary>
        /// x, y, z, R
        /// </summary>
        public int NumberOfVariableValues { get; private set; }

        /// <summary>
        /// 4/3* Pi* R^3
        /// </summary>
        public double Weight { get; private set; }

        public Enums.ObjectType ObjectType { get; private set; }

        public Point Center { get; private set; }

        public Sphere(Point center, double radius)
        {
            Center = center ?? new Point();
            NumberOfVariableValues = 4;
            Radius = radius;
            ObjectType = Enums.ObjectType.Sphere;

            Weight = ((Func<double>)(() =>
            {
                return 4.0 / 3.0 * Math.PI * Math.Pow(Radius, 3.0);
            })).Invoke();
        }

        public Sphere(double[] data)
        {
            Center = new Point(data);
            NumberOfVariableValues = 4;
            Radius = data[3];
            ObjectType = Enums.ObjectType.Sphere;

            Weight = ((Func<double>)(() =>
            {
                return 4.0 / 3.0 * Math.PI * Math.Pow(Radius, 3.0);
            })).Invoke();
        }
    }
}
