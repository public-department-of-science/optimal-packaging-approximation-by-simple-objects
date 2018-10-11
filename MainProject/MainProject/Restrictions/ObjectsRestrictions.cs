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
        public void CalculationAmountOf_Not_IntersectionElementRestriction(Data data, ref int _nele_jac, ref int _m, out int amountOfElementThoseMustNotIntersect, out int _nele_jacAmountOfElementThoseMustNotIntersect)
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
            _nele_jac += _nele_jacAmountOfElementThoseMustNotIntersect;
            _m += amountOfElementThoseMustNotIntersect;
        }

        public void CalculationAmountOfIntersectionCombinedObjectsRestriction(Data data, ref int nele_jac, ref int m)
        {
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
                    continue;
                }

                // Cycle for not intersection restriction
                for (int k = 0; k < firstInternalObject.Count - 1; k++)
                {
                    for (int z = 1; z < firstInternalObject.Count; z++)
                    {

                    }
                }
            }
            _nele_jac += ;
            _m +=;
        }

        public void CalculationAmountOfVariablesForTask(Data data, ref int _n)
        {
            foreach (IInternalObject @object in data.Objects)
            {
                _n += @object.NumberOfVariableValues;
            }
            _n += data.Container.AmountOfVariables; // container variables
        }

        public void CalculationKeepingObjectsIntoContainerRestriction(Data data, ref int amountOfNonZeroElementInFirstDerivatives, ref int restrictions, int systemVariables, int objectsCount)
        {
            amountOfNonZeroElementInFirstDerivatives += (systemVariables - data.Container.AmountOfVariables); // x, y, z , R for Sphere and minus amount of variables for container
            restrictions += objectsCount;
        }

        public void CalculationFlourAndCeilingValuesRestrictionForVariables(Data data, ref int countObjects, double[] _x_L, double[] _x_U, out int systemVariables)
        {
            countObjects = 0;
            systemVariables = 0; // amount of variables(not fixed values) in system

            foreach (IInternalObject item in data.Objects)
            {
                if (item is ICombinedObject)
                {
                    foreach (IInternalObject item1 in ((CombinedObject)item).InternalInCombineObjects)
                    {
                        int varInOneInternalObject = 0;
                        for (; varInOneInternalObject < item1.NumberOfVariableValues; ++varInOneInternalObject, ++systemVariables)
                        {
                            _x_L[systemVariables] = Ipopt.NegativeInfinity;
                            _x_U[systemVariables] = Ipopt.PositiveInfinity;
                        }
                        ++countObjects;
                    }
                }
                else
                {
                    int varInInternalObject = 0;
                    for (; varInInternalObject < item.NumberOfVariableValues; ++varInInternalObject, ++systemVariables)
                    {
                        _x_L[systemVariables] = Ipopt.NegativeInfinity;
                        _x_U[systemVariables] = Ipopt.PositiveInfinity;
                    }
                    ++countObjects;
                }
            }

            for (int j = 0; j < data.Container.AmountOfVariables; ++j, ++systemVariables)
            {
                _x_L[systemVariables] = Ipopt.NegativeInfinity;
                _x_U[systemVariables] = Ipopt.PositiveInfinity;
            }
        }

        public void CalculationFlourAndCeilingValuesForAllRestrictions_g(Data data, double[] _g_L, double[] _g_U, int objectsCont)
        {
            int op = 0;

            #region Keeping objects into container

            for (int j = 0; j < objectsCont; j++) // радиусы от 0 до MAX
            {
                _g_L[op] = 0;
                _g_U[op++] = Ipopt.PositiveInfinity;
            }

            #endregion

            #region Not intersection Objects

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

                            _g_L[op] = Math.Pow((first.Radius + second.Radius), 2);
                            _g_U[op++] = Ipopt.PositiveInfinity;
                        }
                    }
                }
            }

            #endregion

            #region Combined objects intersections

            //radius = new double[countObjects];

            //int q = 0;
            //for (int i = 0; i < data.Objects.Length; i++)
            //{
            //    for (int j = 0; j < data.Objects[i].ListWithObjects.Count; j++)
            //    {
            //        radius[q++] = data.Objects[i].ListWithObjects[j].R;
            //    }
            //}

            #endregion
        }
    }
}
