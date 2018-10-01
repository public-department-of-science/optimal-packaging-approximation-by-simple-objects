using MainProject.Enums;
using PackegeProject.Interfaces.InternalObjects.PolygonalObjects;
using System;
using TestProblemIpOpt.Model;

namespace PackegeProject.InternalObjectsClasses.PolygonalObjects
{
    internal class Cube : ICube
    {
        public double FaceOfTheCube { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int NumberOfVariableValues { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Point[] Points { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public double Weight => throw new NotImplementedException();

        public Enums.ObjectType ObjectType => throw new NotImplementedException();
    }
}
