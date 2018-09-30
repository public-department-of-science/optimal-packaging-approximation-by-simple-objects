using PackegeProject.Interfaces;
using PackegeProject.InternalObjectsClasses.CircularObjects;
using static TestProblemIpOpt.Enums;

namespace hs071_cs
{
    public class Object
    {
        public IInternalObject InternalObject { get; set; }

        public Object()
        {
            InternalObject = new Sphere();
        }

        public Object(IInternalObject internalObject, ObjectType objectType)
        {
            InternalObject = internalObject;
            InternalObject.ObjectType = objectType;
        }
    }
}