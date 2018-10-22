using MainProject.Containers;
using MainProject.Helpers;
using MainProject.Interfaces;
using MainProject.Interfaces.InternalObjects.CircularObjects;
using MainProject.InternalObjectsClasses.CircularObjects;
using PackageProject.Interfaces;
using PackageProject.InternalObjectsClasses.CircularObjects;
using System.Collections.Generic;

namespace hs071_cs
{
    public class Data
    {
        public Dimension dimension = new Dimension();

        public List<IInternalObject> Objects { get; }
        public double[,] C { get; } // матрица связей

        public IContainer Container { get; }

        public Data(IContainer container)
        {
            Objects = new List<IInternalObject>();
            Container = container ?? new CircularContainer(0.0, new MainProject.Model.Point());
            C = null;
        }

        /// <summary>
        /// Return double array with all system variables
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public double[] DataToArray()
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public Data ArrayToData(double[] x)
        {
            int xCount = 0;
            Data deserializedArrayToData = new Data(Container);

            foreach (IInternalObject @object in Objects)
            {
                IInternalObject tempObject = null;

                if (@object is ISphere)
                {
                    double[] sphereData = new double[((Sphere)@object).NumberOfVariableValues];
                    for (int i = 0; i < ((Sphere)@object).NumberOfVariableValues; ++i, ++xCount)
                    {
                        sphereData[i] = x[xCount];
                    }
                    tempObject = new Sphere(sphereData);
                    deserializedArrayToData.Objects.Add(tempObject);
                    continue;
                }

                if (@object is ICombinedObject)
                {
                    CombinedObject combinedObject = new CombinedObject();
                    foreach (IInternalObject item in ((CombinedObject)@object).InternalInCombineObjects)
                    {
                        if (item is ISphere)
                        {
                            double[] sphereData = new double[((Sphere)tempObject).NumberOfVariableValues];
                            for (int i = 0; i < ((Sphere)tempObject).NumberOfVariableValues; ++i, ++xCount)
                            {
                                sphereData[i] = x[xCount];
                            }
                            tempObject = new Sphere(sphereData);
                            combinedObject.InternalInCombineObjects.Add(tempObject);
                            continue;
                        }
                    }
                    tempObject = combinedObject;
                    deserializedArrayToData.Objects.Add(tempObject);
                    continue;
                }
            }
            return deserializedArrayToData;
        }
    }
}
