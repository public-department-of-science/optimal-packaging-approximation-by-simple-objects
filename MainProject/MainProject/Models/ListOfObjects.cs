using System;
using System.Collections.Generic;

namespace hs071_cs
{
    public class ListOfObjects
    {
        public List<Object> ListWithObjects { get; set; }
        public double[] ArrayWithDistance { get; set; }

        public ListOfObjects() => ListWithObjects = new List<Object>();

        private ListOfObjects(List<Object> list)
        {
            ListWithObjects = list ?? throw new ArgumentNullException(nameof(list));
            //ArrayWithDistance =
            ComputeDistance();// ?? throw new ArgumentNullException(nameof(ArrayWithDistance), "Computing of distances undefined!");
        }

        public void ComputeDistance()
        {
            double[] arrayWithDistances = new double[ListWithObjects.Count];
            for (int i = 0; i < ListWithObjects.Count - 1; i++)
            {
                Object tempFirst = ListWithObjects[i];
                for (int j = i + 1; j < ListWithObjects.Count; j++)
                {
                    Object tempSecond = ListWithObjects[j];
                    arrayWithDistances[j] = DistanceBetweenTwoObjects(tempFirst, tempSecond);
                }
            }
            ArrayWithDistance = arrayWithDistances;
        }

        private double DistanceBetweenTwoObjects(Object tempFirst, Object tempSecond) =>
            Math.Sqrt(Math.Pow((tempFirst.X - tempSecond.X), 2.0) + Math.Pow((tempFirst.Y - tempSecond.Y), 2.0) + Math.Pow((tempFirst.Z - tempSecond.Z), 2.0));
    }
}