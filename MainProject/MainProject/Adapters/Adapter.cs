using Cureos.Numerics;
using PackageProject.Interfaces;
using System;

namespace hs071_cs
{
    internal class Adapter : BaseAdapter
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data"></param>
        public Adapter(Data data)
        {
            container = data.Container;
            countObjects = 0;

            foreach (IInternalObject @object in data.Objects)
            {
                _n += @object.NumberOfVariableValues;
            }

            _n += data.Container.AmountOfVariables; // container variables

            _x_L = new double[_n];
            _x_U = new double[_n];

            #region value restriction

            int AmountOfVariablesWithoutContainer = Restrictions.ValueRestriction(data, ref countObjects, _x_L, _x_U);

            #endregion

            /*    Огрaничения
             **************************************************************************************/
            _nele_jac = 0;
            _m = 0; // => restrictions

            // (R-r[i])^2-x[i]^2-y[i]^2 -z[i]^2 >= 0
            _nele_jac += (AmountOfVariablesWithoutContainer - data.Container.AmountOfVariables); // x, y, z , R
            _m += countObjects;

            // (x[i] - x[j]) ^ 2 + (y[i] - y[j]) ^ 2 + (z[i] - z[j]) ^ 2 - (r[i] - r[j]) ^ 2 >= 0
            Restrictions.AmountOfIntersectionElement(data, out int amountOfElementThoseMustNotIntersect, out int _nele_jacAmountOfElementThoseMustNotIntersect);
            _nele_jac += _nele_jacAmountOfElementThoseMustNotIntersect;
            _m += amountOfElementThoseMustNotIntersect;

            //m[i]*x[i] + count
            // _nele_jac += 3 * countCircles;
            // _m += countCircles;

            _g_L = new double[_m];
            _g_U = new double[_m];
            int op = 0;
            for (int j = 0; j < countObjects; j++) // радиусы от 0 до MAX
            {
                _g_L[op] = 0;
                _g_U[op++] = Ipopt.PositiveInfinity;
            }
            //for (int i = 0; i < countObjects - 1; i++)
            //{
            //    for (int j = i + 1; j < countObjects; j++)
            //    {
            //       // _g_L[op] = Math.Pow((radius[i] + radius[j]), 2);
            //        _g_U[op++] = Ipopt.PositiveInfinity;
            //    }
            //}

            //radius = new double[countObjects];
            //int q = 0;
            //for (int i = 0; i < data.Objects.Length; i++)
            //{
            //    for (int j = 0; j < data.Objects[i].ListWithObjects.Count; j++)
            //    {
            //        //radius[q++] = data.Objects[i].ListWithObjects[j].R;
            //    }
            //}

            C = data.C ?? null;
            _nele_hess = 0;
        }



        public override bool Eval_f(int n, double[] x, bool new_x, out double obj_value)
        {
            throw new NotImplementedException();
        }

        public override bool Eval_grad_f(int n, double[] x, bool new_x, double[] grad_f)
        {
            throw new NotImplementedException();
        }

        public override bool Eval_g(int n, double[] x, bool new_x, int m, double[] g)
        {
            throw new NotImplementedException();
        }

        public override bool Eval_jac_g(int n, double[] x, bool new_x, int m, int nele_jac, int[] iRow, int[] jCol, double[] values)
        {
            throw new NotImplementedException();
        }

        public override bool Eval_h(int n, double[] x, bool new_x, double obj_factor, int m, double[] lambda, bool new_lambda, int nele_hess, int[] iRow, int[] jCol, double[] values)
        {
            throw new NotImplementedException();
        }

        //private int AmountOfIntersectionElement(int countObjects)
        //{
        //    int countOfElements = 0;

        //    for (int i = 0; i < countObjects; i++)
        //    {
        //        countOfElements += countObjects - i - 1;
        //    }
        //    return 3 * countOfElements; // 3 = (x + y + z)
        //}

        //private void AddNewIteration(object element)
        //{
        //    AllIteration.Add((double[])element);
        //}

        //public override bool Eval_f(int n, double[] x, bool new_x, out double obj_value)
        //{
        //    // R -> min
        //    obj_value = K2 * container.EvalFunction(x, _n); // + K2 * C_Multiply_by_Length(x);

        //    ThreadPool.GetMinThreads(out int minWorker, out int minIOC);
        //    ThreadPool.SetMaxThreads(minWorker, minIOC);
        //    ThreadPool.QueueUserWorkItem(new WaitCallback(AddNewIteration), x);
        //    return true;
        //}

        //public override bool Eval_grad_f(int n, double[] x, bool new_x, double[] grad_f)
        //{
        //    #region Length Bond
        //    //double[] Xderivative = new double[countCircles];
        //    //double[] Yderivative = new double[countCircles];
        //    //double[] Zderivative = new double[countCircles];
        //    //////
        //    //Parallel.For(0, countCircles - 1, i =>
        //    //{
        //    //    Parallel.For(i + 1, countCircles, body: j =>
        //    //    {
        //    //        Xderivative[i] += C[i, j] * (x[3 * i] - x[3 * j]) /
        //    //         (
        //    //         Math.Sqrt(
        //    //             Math.Pow(x[3 * i], 2) - 2 * x[3 * i] * x[3 * j] + Math.Pow(x[3 * j], 2) // x
        //    //             + Math.Pow(x[3 * i + 1] - x[3 * j + 1], 2) // y
        //    //             + Math.Pow(x[3 * i + 2] - x[3 * j + 2], 2) // z
        //    //                  )
        //    //         );

        //    //        Yderivative[i] += C[i, j] * (x[3 * i + 1] - x[3 * j + 1]) /
        //    //            (
        //    //            Math.Sqrt(
        //    //                Math.Pow(x[3 * i] - x[3 * j], 2) // x
        //    //                + Math.Pow(x[3 * i + 1], 2) - 2 * x[3 * i + 1] * x[3 * j + 1] + Math.Pow(x[3 * j + 1], 2) // y
        //    //                + Math.Pow(x[3 * i + 2] - x[3 * j + 2], 2) // z
        //    //                     )
        //    //            );

        //    //        Zderivative[i] += C[i, j] * (x[3 * i + 2] - x[3 * j + 2]) /
        //    //            (
        //    //            Math.Sqrt(
        //    //                Math.Pow(x[3 * i] - x[3 * j], 2) // x
        //    //                + Math.Pow(x[3 * i + 1] - x[3 * j + 1], 2) // y
        //    //                + Math.Pow(x[3 * i + 2], 2) - 2 * x[3 * i + 2] * x[3 * j + 2] + Math.Pow(x[3 * j + 2], 2) // z
        //    //                     )
        //    //            );
        //    //    });
        //    //});
        //    //Parallel.For(0, countCircles, i =>
        //    //{
        //    //    grad_f[3 * i] = K2 * Xderivative[i];
        //    //    grad_f[3 * i + 1] = K2 * Yderivative[i];
        //    //    grad_f[3 * i + 2] = K2 * Zderivative[i];
        //    //});
        //    #endregion

        //    container.EvalFunctionGrad(x, grad_f, _n);
        //    return true;
        //}

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
        //    ////Weight[i] * X[i] + Weight[i] * Y[i] + Weight[i] * Z[i]
        //    //Parallel.For(0, countCircles, i =>
        //    //{
        //    //    g[kk++] = Math.Pow(Weight[i] * x[3 * i], 2) + Math.Pow(Weight[i] * x[3 * i + 1], 2) + Math.Pow(Weight[i] * x[3 * i + 2], 2);
        //    //});
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

        //        //// Weight[i] * X[i] + Weight[i] * Y[i] + Weight[i] * Z[i]
        //        //Parallel.For(0, countCircles, i =>
        //        //{
        //        //    // Weight[i] * X[i]
        //        //    iRow[kk] = g;
        //        //    jCol[kk++] = 0;

        //        //    // Weight[i] * Y[i] 
        //        //    iRow[kk] = g;
        //        //    jCol[kk++] = 1;

        //        //    // Weight[i] * Z[i]
        //        //    iRow[kk] = g;
        //        //    jCol[kk++] = 2;
        //        //    ++g;
        //        //});
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

        //        //////mx;my;mz
        //        //Parallel.For(0, countCircles, i =>
        //        //{
        //        //    values[kk] = 2 * Math.Pow(Weight[i], 2) * x[3 * i];
        //        //    kk++;
        //        //    values[kk] = 2 * Math.Pow(Weight[i], 2) * x[3 * i + 1];
        //        //    kk++;
        //        //    values[kk] = 2 * Math.Pow(Weight[i], 2) * x[3 * i + 2];
        //        //    kk++;
        //        //});
        //    }
        //    return true;
        //}

        //public override bool Eval_h(int n, double[] x, bool new_x, double obj_factor, int m, double[] lambda, bool new_lambda, int nele_hess, int[] iRow, int[] jCol, double[] values)
        //{
        //    return false;
        //}

        //// Вычисляем диаметр как сумму всех радиусов
        //private double DiametrSum(double[] radius)
        //{
        //    double sum = 0.0;
        //    foreach (double rad in radius)
        //    {
        //        sum += 2 * rad;
        //    }
        //    return sum;
        //}

        ////private double C_Multiply_by_Length(double[] x)
        ////{
        ////    double Sum = 0;
        ////    Parallel.For(0, countCircles - 1, i =>
        ////        {
        ////            Parallel.For(i + 1, countCircles, j =>
        ////               {
        ////                   Sum += C[i, j] * Math.Sqrt(Math.Pow(x[3 * i] - x[3 * j], 2)
        ////                + Math.Pow(x[3 * i + 1] - x[3 * j + 1], 2)
        ////                + Math.Pow(x[3 * i + 2] - x[3 * j + 2], 2));
        ////               });
        ////        });
        ////    return Sum;
        ////}


    }
}