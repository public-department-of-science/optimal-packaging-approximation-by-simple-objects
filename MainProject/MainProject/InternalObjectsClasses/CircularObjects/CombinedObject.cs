using MainProject.Interfaces.InternalObjects.CircularObjects;
using PackegeProject.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using TestProblemIpOpt.Model;
using static MainProject.Enums.Enums;

namespace MainProject.InternalObjectsClasses.CircularObjects
{
    internal class CombinedObject : IInternalObject, ICombinedObject
    {
        public ObservableCollection<IInternalObject> InternalInCombineObject { get; }

        public double[] ArrayWithDistances { get; private set; }
        public int NumberOfVariableValues { get; }

        public double Weight { get; }

        public ObjectType ObjectType { get; set; }


        public CombinedObject() : this(combinedObjects: new ObservableCollection<IInternalObject>())
        {
            NumberOfVariableValues = ((Func<int>)(() =>
            {
                int count = 0;
                foreach (CombinedObject item in InternalInCombineObject)
                {
                    count += item.NumberOfVariableValues;
                }
                return count;
            })).Invoke();

            ObjectType = ObjectType.CombinedObject;
            Weight = CulculateWeightOfObject();

            InternalInCombineObject.CollectionChanged += ReCulculateDistances;
            ArrayWithDistances = new double[InternalInCombineObject.Count];

        }

        private void ReCulculateDistances(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                default:
                    ComputeDistanceWithObjects();
                    break;
            }
        }

        private CombinedObject(ObservableCollection<IInternalObject> combinedObjects)
        {
            InternalInCombineObject = combinedObjects ?? new ObservableCollection<IInternalObject>();
        }

        public void ComputeDistanceWithObjects()
        {
            foreach (IInternalObject item in InternalInCombineObject)
            {
                switch (item.ObjectType)
                {
                    case ObjectType.Circle:
                        break;
                    case ObjectType.Sphere:
                        break;
                    default:
                        throw new Exception($"Combined object must be {ObjectType.Circle} or {ObjectType.Sphere} structure!");
                        break;
                }
            }

            double[] arrayWithDistances = new double[InternalInCombineObject.Count];
            for (int i = 0; i < InternalInCombineObject.Count - 1; i++)
            {
                Point tempFirst = ((ICircularObject)InternalInCombineObject[i]).Center;
                for (int j = i + 1; j < InternalInCombineObject.Count; j++)
                {
                    Point tempSecond = ((ICircularObject)InternalInCombineObject[j]).Center;
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
            foreach (CombinedObject @object in InternalInCombineObject)
            {
                weight += @object.Weight;
            }
            return weight;
        }

    }
}
