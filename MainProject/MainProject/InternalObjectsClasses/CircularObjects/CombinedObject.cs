using MainProject.Interfaces.InternalObjects.CircularObjects;
using PackageProject.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using TestProblemIpOpt.Model;
using static MainProject.Enums.Enums;

namespace MainProject.InternalObjectsClasses.CircularObjects
{
    public class CombinedObject : IInternalObject, ICombinedObject
    {
        public ObservableCollection<IInternalObject> InternalInCombineObjects { get; }

        public double[] ArrayWithDistances { get; private set; }
        public int NumberOfVariableValues { get; private set; }

        public double Weight { get; }

        public ObjectType ObjectType { get; set; }


        public CombinedObject() : this(combinedObjects: new ObservableCollection<IInternalObject>())
        {
            NumberOfVariableValues = 0;

            ObjectType = ObjectType.CombinedObject;
            Weight = CulculateWeightOfObject();

            InternalInCombineObjects.CollectionChanged += ReCulculateDistances;
            ArrayWithDistances = new double[InternalInCombineObjects.Count];

        }

        private CombinedObject(ObservableCollection<IInternalObject> combinedObjects)
        {
            InternalInCombineObjects = combinedObjects ?? new ObservableCollection<IInternalObject>();
        }

        private void ReCulculateDistances(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                default:
                    NumberOfVariableValues = ((Func<int>)(() =>
                      {
                          int count = 0;
                          foreach (var item in InternalInCombineObjects)
                          {
                              count += item.NumberOfVariableValues;
                          }
                          return count;
                      })).Invoke();
                    ComputeDistanceWithObjects();
                    break;
            }
        }

        public void ComputeDistanceWithObjects()
        {
            foreach (IInternalObject item in InternalInCombineObjects)
            {
                switch (item.ObjectType)
                {
                    case ObjectType.Circle:
                        break;
                    case ObjectType.Sphere:
                        break;
                    default:
                        throw new Exception($"Combined object must be {ObjectType.Circle} or {ObjectType.Sphere} structure!");
                }
            }

            double[] arrayWithDistances = new double[InternalInCombineObjects.Count];
            for (int i = 0; i < InternalInCombineObjects.Count - 1; i++)
            {
                Point tempFirst = ((ICircularObject)InternalInCombineObjects[i]).Center;
                for (int j = i + 1; j < InternalInCombineObjects.Count; j++)
                {
                    Point tempSecond = ((ICircularObject)InternalInCombineObjects[j]).Center;
                    arrayWithDistances[j] = DistanceBetweenTwoObjects(tempFirst, tempSecond);
                }
            }
            ArrayWithDistances = arrayWithDistances;
        }

        private double DistanceBetweenTwoObjects(Point firstObjectCenter, Point secondObjectCenter) =>
            Math.Sqrt(Math.Pow((firstObjectCenter.X - secondObjectCenter.X), 2.0)
                + Math.Pow((firstObjectCenter.Y - secondObjectCenter.Y), 2.0)
                + Math.Pow((firstObjectCenter.Z - secondObjectCenter.Z), 2.0));

        private double CulculateWeightOfObject()
        {
            double weight = 0.0;
            foreach (CombinedObject @object in InternalInCombineObjects)
            {
                weight += @object.Weight;
            }
            return weight;
        }

    }
}
