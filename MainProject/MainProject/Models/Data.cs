using PackageProject.Interfaces;
using System.Collections.Generic;
using MainProject.Containers;
using MainProject.Helpers;
using MainProject.Interfaces;

namespace hs071_cs
{
    public class Data
    {
        public Dimension dimension = new Dimension();

        public List<IInternalObject> Objects { get; }
        public double[,] C { get; } // матрица связей

        public IContainer Container { get; }

        public int[] AmountOfObjectsInEachComplexObject { get; private set; }

        public Data(IContainer container)
        {
            Objects = new List<IInternalObject>();
            Container = container ?? new CircularContainer(0.0, new MainProject.Model.Point());
            C = null;
        }
    }
}
