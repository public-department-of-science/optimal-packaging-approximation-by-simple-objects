using MainProject.Enums;
using PackegeProject.Interfaces;
using System;
using TestProblemIpOpt.Model;

namespace PackegeProject.InternalObjectsClasses.CircularObjects
{
    internal class Cone : ICone
    {
        public double Height { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Point LowerBaseCenter { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double Radius { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int NumberOfVariableValues { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public double Weight => throw new NotImplementedException();

        public Enums.ObjectType ObjectType => throw new NotImplementedException();
    }
}
