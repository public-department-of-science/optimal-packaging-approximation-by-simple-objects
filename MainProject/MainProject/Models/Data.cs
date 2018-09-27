using System;
using TestProblemIpOpt.Containers;
using TestProblemIpOpt.Helpers;
using TestProblemIpOpt.Interfaces;
using static TestProblemIpOpt.Enums;

namespace hs071_cs
{
    public class Data
    {
        public Dimension dimension = new Dimension();

        #region For Future
        public double[] xStartPoint;
        public double[] yStartPoint;
        public double[] zStartpoint;
        public double[] rStart;
        public double RNach;

        public double[] xIteration;
        public double[] yiteration;
        public double[] ziteration;
        public double[] riteration;
        public double Riteration;
        #endregion

        public int objectsCount { get; }
        public int holesCount { get; }
        public ListOfObjects[] Objects { get; }
        public double[,] C { get; } // матрица связей

        public IContainer Container { get; }
        // public TaskClassification TaskClassification;

        public int[] AmountOfObjectsInEachComplexObject { get; private set; }

        public Data() : this(new int[1] { 1 }, new double[1] { 0.1 }, new double[1] { 0.1 }, new double[1] { 0.1 }, new double[1] { 0.1 }, null, 1, 1)
        {
        }

        private Data(int objectCount)
        {
            Objects = new ListOfObjects[objectCount];
            for (int i = 0; i < Objects.Length; i++)
            {
                Objects[i] = new ListOfObjects();
            }

            C = new double[objectCount, objectCount];
        }

        public Data(int[] amountOfSpheresInEachComplexObject, double[] x, double[] y, double[] z, double[] r, IContainer container, int objectsCount, int holesCount, CircularObjectType[] type = null, double[,] C = null) : this(objectsCount)
        {
            AmountOfObjectsInEachComplexObject = amountOfSpheresInEachComplexObject ?? throw new ArgumentNullException("AmountOfSpheresInEachComplexObject", "Argument is null!!");
            this.objectsCount = objectsCount;
            if (amountOfSpheresInEachComplexObject.Length != objectsCount)
            {
                throw new Exception("Array dsmension unmutched!");
            }

            Container = container ?? new CircularContainer(0.0, new TestProblemIpOpt.Model.Point());
            this.holesCount = holesCount;
            // TaskClassification = taskClassification;

            int j = 0;
            for (int i = 0; i < Objects.Length; i++)
            {
                int v = (j + AmountOfObjectsInEachComplexObject[i]);
                for (; j < v; j++)
                {
                    Objects[i].ListWithObjects.Add(new Object(x[j], y[j], z[j], r[j], CircularObjectType.Sphere));
                }

                Objects[i].ComputeDistance();
            }
            this.C = C ?? null;
        }
    }
}