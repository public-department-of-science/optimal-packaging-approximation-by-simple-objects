namespace hs071_cs
{
    /// <summary>
    /// Classic adaptor with spheres, combined objects
    /// </summary>
    internal class Adapter : BaseAdapter
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data"></param>
        public Adapter(Data data)
        {
            container = data.Container;
            Restrictions.CalculationAmountOfVariablesForTask(data, ref _n);

            _x_L = new double[_n];
            _x_U = new double[_n];

            Restrictions.CalculationFlourAndCeilingValuesRestrictionForVariables(data, ref objectsCont, _x_L, _x_U, out int systemVariables);

            #region Restrictions

            // 1) (R-r[i])^2-x[i]^2-y[i]^2 -z[i]^2 >= 0
            Restrictions.CalculationKeepingObjectsIntoContainerRestriction(data: data, amountOfNonZeroElementInFirstDerivatives: ref _nele_jac, restrictions: ref _m, systemVariables: systemVariables, objectsCount: objectsCont);
            // 2) (x[i] - x[j]) ^ 2 + (y[i] - y[j]) ^ 2 + (z[i] - z[j]) ^ 2 - (r[i] - r[j]) ^ 2 >= 0
            Restrictions.CalculationAmountOf_Not_IntersectionElementRestriction(data, ref _nele_jac, ref _m, out int amountOfElementThoseMustNotIntersect, out int _nele_jacAmountOfElementThoseMustNotIntersect);
            // 3) Intersection restriction for combinedObjects
            Restrictions.CalculationAmountOfIntersectionCombinedObjectsRestriction(data, ref _nele_jac, ref _m);

            _g_L = new double[_m];
            _g_U = new double[_m];

            //g
            Restrictions.CalculationFlourAndCeilingValuesForAllRestrictions_g(data, _g_L, _g_U, objectsCont);

            #endregion

            C = data.C ?? null;
        }

        public override bool Eval_f(int n, double[] x, bool new_x, out double obj_value)
        {
            // R -> min
            obj_value = K2 * container.EvalFunction(x, _n);
            return true;
        }

        public override bool Eval_grad_f(int n, double[] x, bool new_x, double[] grad_f)
        {
            container.EvalFunctionGrad(x, grad_f, _n);
            return true;
        }

        //public override bool Eval_g(int n, double[] x, bool new_x, int m, double[] g)
        //{
        //    int kk = 0;
        //    // (R-r[i])^2 - x[i]^2 - y[i]^2 - z^2 >= 0
        //    // from 0 to count-1
        //    container.Eval_g(_n, x, g, ref kk, radius);
        //    // kk = count
        //    // (x[i]-x[j])^2 + (y[i]-y[j])^2 + (z[i]-z[j])^2 - (r[i]-r[j])^2 >=0
        //    // from count to count*(count-1)/2 - 1

        //    for (int i = 0; i < countObjects - 1; i++) // на каждой итерации увеличиваем на 3 счетч. по Z
        //    {
        //        for (int j = i + 1; j < countObjects; j++)
        //        {
        //            g[kk++] = Math.Pow((x[3 * i] - x[3 * j]), 2.0)
        //                          + Math.Pow((x[3 * i + 1] - x[3 * j + 1]), 2.0)
        //                          + Math.Pow((x[3 * i + 2] - x[3 * j + 2]), 2.0)
        //                          - Math.Pow((radius[i] - radius[j]), 2.0);
        //        }
        //    }
        //    return true;
        //}

        //public override bool Eval_jac_g(int n, double[] x, bool new_x, int m, int nele_jac, int[] iRow, int[] jCol, double[] values)
        //{
        //    if (values == null)
        //    {
        //        int kk = 0,
        //            g = 0;

        //        // (R-r[i])^2 - x[i]^2 - y[i]^2 - z[i]^2 >= 0
        //        // позиции R, Х и У, Z
        //        container.Eval_jac_g(n, x, ref kk, ref g, radius, iRow, jCol, values, countObjects);

        //        // (x[i]-x[j])^2 + (y[i]-y[j])^2 + (z[i]-z[j])^2 - (r[i]-r[j])^2 >=0
        //        for (int i = 0; i < countObjects - 1; ++i)
        //        {
        //            for (int j = i + 1; j < countObjects; ++j)
        //            {
        //                // -------  X[i], X[j] ------- 
        //                iRow[kk] = g;
        //                jCol[kk++] = 3 * i;
        //                iRow[kk] = g;
        //                jCol[kk++] = 3 * j;

        //                // -------  Y[i], Y[j] ------- 
        //                iRow[kk] = g; ;
        //                jCol[kk++] = 3 * i + 1;
        //                iRow[kk] = g;
        //                jCol[kk++] = 3 * j + 1;

        //                // -------  Z[i], Z[j] ------- 
        //                iRow[kk] = g;
        //                jCol[kk++] = 3 * i + 2;
        //                iRow[kk] = g;
        //                jCol[kk++] = 3 * j + 2;

        //                ++g;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        // (R-r[i])^2 - x[i]^2 - y[i]^2 - z[i]^2 >= 0
        //        int kk = 0;
        //        for (int i = 0; i < countObjects; i++)// шаг по Z это каждый третий эл
        //        {
        //            values[kk] = 2.0 * (x[_n - 1] - radius[i]); // R0'
        //            kk++;
        //            values[kk] = -2.0 * x[3 * i]; //X'
        //            kk++;
        //            values[kk] = -2.0 * x[3 * i + 1]; //Y'
        //            kk++;
        //            values[kk] = -2.0 * x[3 * i + 2]; //Z'
        //            kk++;
        //        }
        //        // (x[i]-x[j])^2 + (y[i]-y[j])^2 + (z[i]-z[j])^2 - (r[i]-r[j])^2 >=0
        //        //  Console.WriteLine("---------------------------------------");

        //        for (int i = 0; i < countObjects - 1; i++)
        //        {
        //            for (int j = i + 1; j < countObjects; j++)
        //            {
        //                values[kk++] = 2.0 * (x[3 * i] - x[3 * j]); //X[i]'
        //                values[kk++] = -2.0 * (x[3 * i] - x[3 * j]); //X[j]'

        //                values[kk++] = 2.0 * (x[3 * i + 1] - x[3 * j + 1]); //Y[i]'
        //                values[kk++] = -2.0 * (x[3 * i + 1] - x[3 * j + 1]); //Y[j]'

        //                values[kk++] = 2.0 * (x[3 * i + 2] - x[3 * j + 2]); //Z[i]'
        //                values[kk++] = -2.0 * (x[3 * i + 2] - x[3 * j + 2]); //Z[j]'
        //            }
        //        }
        //    }
        //    return true;
        //}

        public override bool Eval_h(int n, double[] x, bool new_x, double obj_factor, int m, double[] lambda, bool new_lambda, int nele_hess, int[] iRow, int[] jCol, double[] values)
        {
            return false;
        }

        private void AddNewIteration(object element)
        {
            AllIteration.Add((double[])element);
        }
    }
}