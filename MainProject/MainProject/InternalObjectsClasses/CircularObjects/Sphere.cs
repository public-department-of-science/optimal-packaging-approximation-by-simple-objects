using MainProject.Enums;
using PackegeProject.Interfaces;
using System;
using TestProblemIpOpt.Model;

namespace PackegeProject.InternalObjectsClasses.CircularObjects
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
    }
}
