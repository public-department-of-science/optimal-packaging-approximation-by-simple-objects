using PackegeProject.Interfaces;
using System.Collections.Generic;

namespace MainProject.Interfaces.InternalObjects.CircularObjects
{
    public interface ICombinedObject<T> where T : IInternalObject
    {
        List<T> InternalCircularObjects { get; }

        double[] ArrayWithDistances { get; }

        void ComputeDistanceWithObjects();
    }
}
