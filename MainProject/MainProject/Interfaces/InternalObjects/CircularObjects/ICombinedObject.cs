using PackegeProject.Interfaces;
using System.Collections.ObjectModel;

namespace MainProject.Interfaces.InternalObjects.CircularObjects
{
    public interface ICombinedObject
    {
        ObservableCollection<IInternalObject> InternalInCombineObject { get; }

        double[] ArrayWithDistances { get; }

        void ComputeDistanceWithObjects();
    }
}
