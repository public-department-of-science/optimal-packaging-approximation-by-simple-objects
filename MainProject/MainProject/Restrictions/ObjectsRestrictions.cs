using Cureos.Numerics;
using hs071_cs;
using MainProject.Containers;
using MainProject.Interfaces.InternalObjects.CircularObjects;
using MainProject.InternalObjectsClasses.CircularObjects;
using PackageProject.Interfaces;
using PackageProject.InternalObjectsClasses.CircularObjects;
using System;
using System.Collections.Generic;

namespace hs071_cs
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
                _x_L[systemVariables] = 0;
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

            // objects intersection in combined object 
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

        public void Evaluation_jacobian_g(Data data, int n, double[] x, bool new_x, int m, int nele_jac, int[] iRow, int[] jCol, double[] values)
        {
            if (values == null)
            {
                int kk = 0, g = 0;
                // (R-r[i])^2 - x[i]^2 - y[i]^2 - z[i]^2 >= 0
                // позиции R, Х и У, Z
                foreach (IInternalObject @object in data.Objects)
                {
                    if (@object is ISphere)
                    {
                        kk = ElementsPositionCalculationForKeepingObjectsInArea_Jacobian_G(n, iRow, jCol, kk, g);
                        ++g;
                        continue;
                    }

                    if (@object is ICombinedObject)
                    {
                        foreach (IInternalObject item in ((CombinedObject)@object).InternalInCombineObjects)
                        {
                            kk = ElementsPositionCalculationForKeepingObjectsInArea_Jacobian_G(n, iRow, jCol, kk, g);
                        }
                        continue;
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
                            for (int z = 0; z < secondInternalObject.Count; z++)
                            {
                                kk = ElementsPositionCalculationForNotIntersectionObjects_Jacobian_G(iRow, jCol, kk, g, k, z);
                                ++g;
                            }
                        }
                    }
                }

                // попарное пересечение 
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
                            kk = ElementsPositionCalculationForNotIntersectionObjects_Jacobian_G(iRow, jCol, kk, g, i, j);
                            ++g;
                        }
                    }
                }
            }
            else
            {
                int kk = 0;
                // (R-r[i])^2 - x[i]^2 - y[i]^2 - z[i]^2 >= 0
                foreach (IInternalObject @object in data.Objects)
                {
                    if (@object is ISphere)
                    {
                        Sphere sphere = (Sphere)@object;
                        kk = ValuesCalculationKeepingInArea_Jacobian_G(n, x, values, kk, sphere);
                        continue;
                    }

                    if (@object is ICombinedObject)
                    {
                        foreach (IInternalObject item in ((CombinedObject)@object).InternalInCombineObjects)
                        {
                            Sphere sphere = (Sphere)item;
                            kk = ValuesCalculationKeepingInArea_Jacobian_G(n, x, values, kk, sphere);
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

                                kk = ValuesCalculationObjectsNotIntersection_Jacobian_G(values, kk, first, second);
                                continue;
                            }
                        }
                    }
                }
                //попарное пересечение комб объетов
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
                            kk = ValuesCalculationObjectsIntersectionInCombinedObjects_Jacobian_G(values, kk);
                            continue;
                        }
                    }
                }
            }
        }

        #endregion

        #region Restriction helper methods

        private static int ValuesCalculationObjectsIntersectionInCombinedObjects_Jacobian_G(double[] values, int kk)
        {
            values[kk++] = 2.0;//* (first.Center.X - second.Center.X); //X[i]'
            values[kk++] = -2.0;// * (first.Center.X - second.Center.X); //X[j]'

            values[kk++] = 2.0;// * (first.Center.Y - second.Center.Y); //Y[i]'
            values[kk++] = -2.0;// * (first.Center.Y - second.Center.Y); //Y[j]'

            values[kk++] = 2.0;// * (first.Center.Z - second.Center.Z); //Z[i]'
            values[kk++] = -2.0;// * (first.Center.Z - second.Center.Z); //Z[j]'
            return kk;
        }

        private static int ValuesCalculationObjectsNotIntersection_Jacobian_G(double[] values, int kk, Sphere first, Sphere second)
        {
            values[kk++] = 2.0 * (first.Center.X - second.Center.X); //X[i]'
            values[kk++] = -2.0 * (first.Center.X - second.Center.X); //X[j]'

            values[kk++] = 2.0 * (first.Center.Y - second.Center.Y); //Y[i]'
            values[kk++] = -2.0 * (first.Center.Y - second.Center.Y); //Y[j]'

            values[kk++] = 2.0 * (first.Center.Z - second.Center.Z); //Z[i]'
            values[kk++] = -2.0 * (first.Center.Z - second.Center.Z); //Z[j]'
            return kk;
        }

        private static int ValuesCalculationKeepingInArea_Jacobian_G(int n, double[] x, double[] values, int kk, Sphere sphere)
        {
            values[kk] = 2.0 * (x[n - 1] - sphere.Radius); // R0'
            kk++;
            values[kk] = -2.0 * sphere.Center.X; //X'
            kk++;
            values[kk] = -2.0 * sphere.Center.Y; //Y'
            kk++;
            values[kk] = -2.0 * sphere.Center.Z; //Z'
            kk++;
            return kk;
        }

        private static int ElementsPositionCalculationForNotIntersectionObjects_Jacobian_G(int[] iRow, int[] jCol, int kk, int g, int k, int z)
        {
            // -------  X[i], X[j] ------- 
            iRow[kk] = g;
            jCol[kk++] = 3 * k;
            iRow[kk] = g;
            jCol[kk++] = 3 * z + 1;

            // -------  Y[i], Y[j] ------- 
            iRow[kk] = g;
            jCol[kk++] = 3 * k + 2;
            iRow[kk] = g;
            jCol[kk++] = 3 * z + 3;

            // -------  Z[i], Z[j] ------- 
            iRow[kk] = g;
            jCol[kk++] = 3 * k + 4;
            iRow[kk] = g;
            jCol[kk++] = 3 * z + 5;
            return kk;
        }

        private static int ElementsPositionCalculationForKeepingObjectsInArea_Jacobian_G(int n, int[] iRow, int[] jCol, int kk, int g)
        {
            //R0 -> внешний шар 
            iRow[kk] = g;
            jCol[kk++] = n - 1;

            //X
            iRow[kk] = g;
            jCol[kk++] = 3 * g;

            //Y
            iRow[kk] = g;
            jCol[kk++] = 3 * g + 1;

            //Z
            iRow[kk] = g;
            jCol[kk++] = 3 * g + 2;
            return kk;
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
