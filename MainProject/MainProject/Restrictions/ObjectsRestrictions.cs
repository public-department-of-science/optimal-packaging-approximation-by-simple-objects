using Cureos.Numerics;
using hs071_cs;
using MainProject.Interfaces;
using MainProject.Interfaces.InternalObjects.CircularObjects;
using MainProject.InternalObjectsClasses.CircularObjects;
using PackageProject.Interfaces;
using PackageProject.InternalObjectsClasses.CircularObjects;
using System;
using System.Collections.Generic;

namespace MainProject.Restrictions
{
    public class ObjectsRestrictions : IRestrictions
    {
        public void AmountOfIntersectionElement(Data data, out int amountOfElementThoseMustNotIntersect, out int _nele_jacAmountOfElementThoseMustNotIntersect)
        {
            amountOfElementThoseMustNotIntersect = 0;
            _nele_jacAmountOfElementThoseMustNotIntersect = 0;
            for (int i = 0; i < data.Objects.Count - 1; i++)
            {
                if (!((data.Objects[i] is ISphere) || (data.Objects[i] is ICombinedObject)))
                {
                    throw new Exception($"Program didn't configure for using polypoint objects. Only spheres or combined object.{data.Objects[i].GetType().ToString()}");
                }

                List<IInternalObject> firstInternalObject = new List<IInternalObject>();
                if (data.Objects[i] is CombinedObject)
                {
                    firstInternalObject.AddRange(((CombinedObject)data.Objects[i]).InternalInCombineObjects);
                }
                else
                {
                    firstInternalObject.Add(data.Objects[i]);
                }

                for (int j = i + 1; j < data.Objects.Count; j++)
                {
                    if (!((data.Objects[j] is ISphere) || (data.Objects[j] is ICombinedObject)))
                    {
                        throw new Exception($"Program didn't configure for using polypoint objects. Only spheres or combined object.{data.Objects[j].GetType().ToString()}");
                    }

                    List<IInternalObject> secondInternalObject = new List<IInternalObject>();
                    if (data.Objects[j] is CombinedObject)
                    {
                        secondInternalObject.AddRange(((CombinedObject)data.Objects[j]).InternalInCombineObjects);
                    }
                    else
                    {
                        secondInternalObject.Add(data.Objects[j]);
                    }

                    // Cycle for not intersection restriction
                    for (int k = 0; k < firstInternalObject.Count; k++)
                    {
                        Sphere first = (Sphere)firstInternalObject[k];
                        for (int z = 0; z < secondInternalObject.Count; z++)
                        {
                            Sphere second = (Sphere)secondInternalObject[z];

                            _nele_jacAmountOfElementThoseMustNotIntersect += second.NumberOfVariableValues;
                            ++amountOfElementThoseMustNotIntersect;
                        }
                    }
                }
            }
        }

        public int ValueRestriction(Data data, ref int countObjects, double[] _x_L, double[] _x_U)
        {
            int i = 0; // amount of variables(not fixed values) in full system
            foreach (IInternalObject item in data.Objects)
            {
                if (item is ICombinedObject)
                {
                    foreach (IInternalObject item1 in ((CombinedObject)item).InternalInCombineObjects)
                    {
                        int varInOneInternalObject = 0;
                        for (; varInOneInternalObject < item1.NumberOfVariableValues; ++varInOneInternalObject, ++i)
                        {
                            _x_L[i] = Ipopt.NegativeInfinity;
                            _x_U[i] = Ipopt.PositiveInfinity;
                        }
                        ++countObjects;
                    }
                }
                else
                {
                    int varInInternalObject = 0;
                    for (; varInInternalObject < item.NumberOfVariableValues; ++varInInternalObject, ++i)
                    {
                        _x_L[i] = Ipopt.NegativeInfinity;
                        _x_U[i] = Ipopt.PositiveInfinity;
                    }
                    ++countObjects;
                }
            }

            for (int j = 0; j < data.Container.AmountOfVariables; ++j, ++i)
            {
                _x_L[i] = Ipopt.NegativeInfinity;
                _x_U[i] = Ipopt.PositiveInfinity;
            }
            return i;
        }
    }
}
