using Cureos.Numerics;
using hs071_cs;
using MainProject.Containers;
using MainProject.Interfaces.InternalObjects.CircularObjects;
using MainProject.InternalObjectsClasses.CircularObjects;
using PackageProject.Interfaces;
using PackageProject.InternalObjectsClasses.CircularObjects;
using System;
using System.Collections.Generic;

namespace MainProject.Restrictions
{
    public class ObjectsRestrictions //: IRestrictions
    {
        #region Calculation restriction

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
                        for (int z = 0; z < secondInternalObject.Count; z++)
                        {
                            _nele_jacAmountOfElementThoseMustNotIntersect += (((Sphere)secondInternalObject[z]).NumberOfVariableValues - 1) * 2;
                            ++amountOfElementThoseMustNotIntersect;
                        }
                    }
                }
            }
            _nele_jac += _nele_jacAmountOfElementThoseMustNotIntersect;
            _m += amountOfElementThoseMustNotIntersect;
        }

        public void CalculationAmountOfIntersectionCombinedObjectsRestriction(Data data, ref int _nele_jac, ref int _m)
        {
            foreach (IInternalObject item in data.Objects)
            {
                CombinedObject combinedObject = item as CombinedObject;
                if (combinedObject is null)
                {
                    continue;
                }
                _m += combinedObject.AmountOfElementsInTheDistanceArray;
                _nele_jac += 2 * 3 * combinedObject.AmountOfElementsInTheDistanceArray;
            }
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
            amountOfNonZeroElementInFirstDerivatives += (systemVariables - data.Container.AmountOfVariables); // x, y, z , (R - external) for Sphere and minus amount of variables for container
            restrictions += objectsCount;
        }

        public void CalculationFlourAndCeilingValuesRestrictionForVariablesVector(Data data, ref int countObjects, double[] _x_L, double[] _x_U, out int systemVariables)
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
                Console.WriteLine(_g_U[op-1].ToString());
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
                        throw new Exception($"Program didn't configure for using polypoint objects. Only spheres or combined objects.{data.Objects[j].GetType().ToString()}");
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

            foreach (IInternalObject item in data.Objects)
            {
                if (item as CombinedObject is null)
                {
                    continue;
                }

                double[][] arrayWithDistances = ((CombinedObject)item).ArrayWithDistances;
                for (int i = 0; i < arrayWithDistances.Length; i++)
                {
                    for (int j = 0; j < arrayWithDistances[i].Length; j++)
                    {
                        _g_L[op] = _g_U[op] = arrayWithDistances[i][j];
                        ++op;
                    }
                }
            }
            Console.WriteLine(op.ToString());
            #endregion
        }

        #endregion

        #region Eval

        public void Evaluation_g(Data data, int n, double[] x, bool new_x, int m, double[] restrictions)
        {
            int gCount = 0;

            // (R-r[i])^2 - x[i]^2 - y[i]^2 - z^2 >= 0
            foreach (IInternalObject @object in data.Objects)
            {
                if (@object is ISphere)
                {
                    restrictions[gCount++] = EquationKeepingSphereInTheContainer((CircularContainer)data.Container, (Sphere)@object);
                    continue;
                }

                if (@object is ICombinedObject)
                {
                    foreach (IInternalObject item in ((CombinedObject)@object).InternalInCombineObjects)
                    {
                        restrictions[gCount++] = EquationKeepingSphereInTheContainer((CircularContainer)data.Container, (Sphere)item);
                        continue;
                    }
                }
            }

            // (x[i]-x[j])^2 + (y[i]-y[j])^2 + (z[i]-z[j])^2 - (r[i]-r[j])^2 >=0

            for (int i = 0; i < data.Objects.Count - 1; i++)
            {
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

                            restrictions[gCount++] = EquationNotIntersactionTwoSpheres(first, second);
                            continue;
                        }
                    }
                }
            }

            //
            foreach (IInternalObject item in data.Objects)
            {
                CombinedObject combinedObject = item as CombinedObject;
                if (combinedObject is null)
                {
                    continue;
                }

                double[][] arrayWithDistances = combinedObject.ArrayWithDistances;
                for (int i = 0; i < arrayWithDistances.Length; i++)
                {
                    for (int j = 0; j < arrayWithDistances[i].Length; j++)
                    {
                        restrictions[gCount++] = arrayWithDistances[i][j];
                    }
                }
            }
        }

        internal void Evaluation_jacobian_g(Data data, int n, double[] x, bool new_x, int m, int nele_jac, int[] iRow, int[] jCol, double[] values)
        {
            //if (values == null)
            //{
            //    int kk = 0,
            //        g = 0;

            //    // (R-r[i])^2 - x[i]^2 - y[i]^2 - z[i]^2 >= 0
            //    // позиции R, Х и У, Z
            //    for (g = 0; g < countCircles; ++g)
            //    {
            //        //R0 -> внешний шар 
            //        iRow[kk] = g;
            //        jCol[kk++] = _n - 1;

            //        //X
            //        iRow[kk] = g;
            //        jCol[kk++] = 3 * g;

            //        //Y
            //        iRow[kk] = g;
            //        jCol[kk++] = 3 * g + 1;

            //        //Z
            //        iRow[kk] = g;
            //        jCol[kk++] = 3 * g + 2;
            //    }

            //    // (x[i]-x[j])^2 + (y[i]-y[j])^2 + (z[i]-z[j])^2 - (r[i]-r[j])^2 >=0
            //    for (int i = 0; i < countCircles - 1; ++i)
            //    {
            //        for (int j = i + 1; j < countCircles; ++j)
            //        {
            //            // -------  X[i], X[j] ------- 
            //            iRow[kk] = g;
            //            jCol[kk++] = 3 * i;
            //            iRow[kk] = g;
            //            jCol[kk++] = 3 * j;

            //            // -------  Y[i], Y[j] ------- 
            //            iRow[kk] = g; ;
            //            jCol[kk++] = 3 * i + 1;
            //            iRow[kk] = g;
            //            jCol[kk++] = 3 * j + 1;

            //            // -------  Z[i], Z[j] ------- 
            //            iRow[kk] = g;
            //            jCol[kk++] = 3 * i + 2;
            //            iRow[kk] = g;
            //            jCol[kk++] = 3 * j + 2;

            //            ++g;
            //        }
            //    }
            //}
            //else
            //{
            //    // (R-r[i])^2 - x[i]^2 - y[i]^2 - z[i]^2 >= 0
            //    int kk = 0;
            //    for (int i = 0; i < countCircles; i++)// шаг по Z это каждый третий эл
            //    {
            //        values[kk] = 2.0 * (x[_n - 1] - radius[i]); // R0'
            //        kk++;
            //        values[kk] = -2.0 * x[3 * i]; //X'
            //        kk++;
            //        values[kk] = -2.0 * x[3 * i + 1]; //Y'
            //        kk++;
            //        values[kk] = -2.0 * x[3 * i + 2]; //Z'
            //        kk++;
            //    }
            //    // (x[i]-x[j])^2 + (y[i]-y[j])^2 + (z[i]-z[j])^2 - (r[i]-r[j])^2 >=0
            //    //  Console.WriteLine("---------------------------------------");

            //    for (int i = 0; i < countCircles - 1; i++)
            //    {
            //        for (int j = i + 1; j < countCircles; j++)
            //        {
            //            values[kk++] = 2.0 * (x[3 * i] - x[3 * j]); //X[i]'
            //            values[kk++] = -2.0 * (x[3 * i] - x[3 * j]); //X[j]'

            //            values[kk++] = 2.0 * (x[3 * i + 1] - x[3 * j + 1]); //Y[i]'
            //            values[kk++] = -2.0 * (x[3 * i + 1] - x[3 * j + 1]); //Y[j]'

            //            values[kk++] = 2.0 * (x[3 * i + 2] - x[3 * j + 2]); //Z[i]'
            //            values[kk++] = -2.0 * (x[3 * i + 2] - x[3 * j + 2]); //Z[j]'
            //        }
            //    }
            //}
        }

        private double EquationNotIntersactionTwoSpheres(Sphere first, Sphere second)
        {
            return Math.Pow(first.Center.X - second.Center.X, 2.0) + Math.Pow(first.Center.Y - second.Center.Y, 2.0) + Math.Pow(first.Center.Z - second.Center.Z, 2.0)
                            - Math.Pow(first.Radius + second.Radius, 2.0);
        }

        private double EquationKeepingSphereInTheContainer(CircularContainer container, Sphere @object)
        {
            return Math.Pow(container.Radius - @object.Radius, 2.0) -
                   Math.Pow(@object.Center.X, 2.0) - Math.Pow(@object.Center.Y, 2.0) - Math.Pow(@object.Center.Z, 2.0);
        }

        #endregion
    }
}
