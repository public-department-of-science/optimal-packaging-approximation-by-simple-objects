﻿using Cureos.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace hs071_cs
{


    class FixedRadius3dAdaptor : BaseAdaptor, IDisposable
    {
        public readonly int countCircles; // количество шаров
        public readonly int countOfCombinedObjects; // количество шаров
        public double[] X { get; } //массив х (входят все координаты и радиусы)
        public readonly double[] radius;
        private double K1 = 1;
        private double K2 = 1;
        private List<double[]> _allIteration;

        public List<double[]> AllIteration { get => _allIteration; set => _allIteration = value; }
        private double[] Weight { get; set; }
        private double[,] C { get; set; }

        public FixedRadius3dAdaptor(Data data)
        {
            AllIteration = new List<double[]>();

            countCircles = data.ballCount; // задаём количетво кругов
            _n = countCircles * 3 + 1; // количество переменных в векторе
            radius = new double[countCircles];
            //вспомогательные счетчики
            double sumR = 0;
            int it = 0;

            foreach (var item in data.ball)
            {
                radius[it++] = item.R;
                sumR += item.R;
            }
            radius = radius.OrderBy(a => a).ToArray();

            /*    Ограничения переменных
            * *************************************************************************************/

            if (countCircles <= 10)
            {
                K1 = 1;
            }
            else
            {
                K1 = 0.4;
            }

            _x_L = new double[_n];
            _x_U = new double[_n];

            double max = radius[countCircles - 1];

            // обнуляем необходимые счетчики
            int countXYZR = 0,
                countCoordinate = 0;
            for (int i = 0; i < countCircles; i++)
            {
                _x_L[3 * i] = Ipopt.NegativeInfinity;
                _x_U[3 * i] = Ipopt.PositiveInfinity;

                _x_L[3 * i + 1] = Ipopt.NegativeInfinity;
                _x_U[3 * i + 1] = Ipopt.PositiveInfinity;

                _x_L[3 * i + 2] = Ipopt.NegativeInfinity;
                _x_U[3 * i + 2] = Ipopt.PositiveInfinity;

                if (max < radius[countCoordinate])
                {
                    max = radius[countCoordinate];
                }

                countCoordinate++;
                countXYZR++;
            }

            _x_L[_n - 1] = max;
            _x_U[_n - 1] = sumR * K1;

            countCircles = data.amountOfCombinedObjectsInEachObject[0];

            /*    Огрaничения
             **************************************************************************************/
            _nele_jac = 0;
            _m = 0;

            // (R-r[i])^2-x[i]^2-y[i]^2 -z[i]^2 >= 0
            _nele_jac += 4 * countCircles; // x, y, z , R
            _m += countCircles;

            // добавляем количество якобианов для комбинир. объектов
            for (int i = 1; i < data.amountOfCombinedObjectsInEachObject.Length; i++)
            {
                countOfCombinedObjects += data.amountOfCombinedObjectsInEachObject[i];
                _nele_jac += 4 * data.amountOfCombinedObjectsInEachObject[i];
            }
            _m += countOfCombinedObjects;// добавляем ограничений для попадания для комбинир. объектов в область

            // (x[i]-x[j])^2 + (y[i]-y[j])^2 + (z[i]-z[j])^2 - (r[i]-r[j])^2 >=0
            //var amountOfNormalObjects = data.amountOfCombinedObjectsInEachObject[0];

            int v = 3 * countCircles * (countCircles - 1); // amount of not intersations among normal objects
            _nele_jac += v;  // 2*3 два ограничения по 3 ненулевых частных производных

            //int y = 0;
            //for (int i = 1; i < data.amountOfCombinedObjectsInEachObject.Length; i++)
            //{
            //    y += 3 * data.amountOfCombinedObjectsInEachObject[0] * (data.amountOfCombinedObjectsInEachObject[i]);
            //}

            int v1 = countCircles * (countCircles - 1) / 2;
            _m += v1;

            //m[i]*x[i] + count
            // _nele_jac += 3 * countCircles;
            // _m += countCircles;

            _g_L = new double[_m];
            _g_U = new double[_m];
            int op = 0;
            for (int j = 0; j < countCircles; j++) // радиусы от 0 до MAX
            {
                _g_L[op] = 0;
                _g_U[op++] = Ipopt.PositiveInfinity;
            }

            // для комбинир объектов 
            for (int j = countCircles; j < countCircles + countOfCombinedObjects; j++) // радиусы от 0 до MAX
            {
                _g_L[op] = 0;
                _g_U[op++] = Ipopt.PositiveInfinity;
            }

            for (int i = 0; i < countCircles - 1; i++)
            {
                for (int j = i + 1; j < countCircles; j++)
                {
                    _g_L[op] = Math.Pow((radius[i] + radius[j]), 2);
                    _g_U[op++] = Ipopt.PositiveInfinity;
                }
            }



            double eps = Ipopt.PositiveInfinity;

            //Constraints of variebles For  Weight[i] * X[i] + Weight[i] * Y[i] + Weight[i] * Z[i]
            //for (int i = 0; i < countCircles; i++)
            //{
            //    _g_L[op] = 0;
            //    _g_U[op++] = eps;
            //}

            //Hesian
            Weight = new double[data.ballCount];
            for (int i = 0; i < data.ballCount; i++)
            {
                Weight[i] = data.ball[i].Weight;
            }
            C = data.C ?? null;
            _nele_hess = 0;

        } // End_Конструктор 

        private void AddNewIteration(object element)
        {
            AllIteration.Add((double[])element);
        }

        public override bool Eval_f(int n, double[] x, bool new_x, out double obj_value)
        {
            // R -> min
            int minWorker, minIOC;

            obj_value = K2 * x[_n - 1] + K2 * C_Multiply_by_Length(x);
            ThreadPool.GetMinThreads(out minWorker, out minIOC);
            ThreadPool.SetMaxThreads(minWorker, minIOC);
            ThreadPool.QueueUserWorkItem(new WaitCallback(AddNewIteration), x);
            return true;
        }

        public override bool Eval_grad_f(int n, double[] x, bool new_x, double[] grad_f)
        {
            #region Length Bond
            double[] Xderivative = new double[countCircles];
            double[] Yderivative = new double[countCircles];
            double[] Zderivative = new double[countCircles];
            ////
            Parallel.For(0, countCircles - 1, i =>
            {
                Parallel.For(i + 1, countCircles, body: j =>
                {
                    Xderivative[i] += C[i, j] * (x[3 * i] - x[3 * j]) /
                     (
                     Math.Sqrt(
                         Math.Pow(x[3 * i], 2) - 2 * x[3 * i] * x[3 * j] + Math.Pow(x[3 * j], 2) // x
                         + Math.Pow(x[3 * i + 1] - x[3 * j + 1], 2) // y
                         + Math.Pow(x[3 * i + 2] - x[3 * j + 2], 2) // z
                              )
                     );

                    Yderivative[i] += C[i, j] * (x[3 * i + 1] - x[3 * j + 1]) /
                        (
                        Math.Sqrt(
                            Math.Pow(x[3 * i] - x[3 * j], 2) // x
                            + Math.Pow(x[3 * i + 1], 2) - 2 * x[3 * i + 1] * x[3 * j + 1] + Math.Pow(x[3 * j + 1], 2) // y
                            + Math.Pow(x[3 * i + 2] - x[3 * j + 2], 2) // z
                                 )
                        );

                    Zderivative[i] += C[i, j] * (x[3 * i + 2] - x[3 * j + 2]) /
                        (
                        Math.Sqrt(
                            Math.Pow(x[3 * i] - x[3 * j], 2) // x
                            + Math.Pow(x[3 * i + 1] - x[3 * j + 1], 2) // y
                            + Math.Pow(x[3 * i + 2], 2) - 2 * x[3 * i + 2] * x[3 * j + 2] + Math.Pow(x[3 * j + 2], 2) // z
                                 )
                        );
                });
            });
            Parallel.For(0, countCircles, i =>
            {
                grad_f[3 * i] = K2 * Xderivative[i];
                grad_f[3 * i + 1] = K2 * Yderivative[i];
                grad_f[3 * i + 2] = K2 * Zderivative[i];
            });
            #endregion

            grad_f[_n - 1] = K1 * 1;
            return true;
        }

        public override bool Eval_g(int n, double[] x, bool new_x, int m, double[] g)
        {
            int kk = 0;
            // (R-r[i])^2 - x[i]^2 - y[i]^2 - z^2 >= 0
            // from 0 to count-1
            Parallel.For(0, countCircles, i =>
            {
                g[kk++] = Math.Pow((x[_n - 1] - radius[i]), 2) -
                    x[3 * i] * x[3 * i] -          // x
                    x[3 * i + 1] * x[3 * i + 1] -  // y
                    x[3 * i + 2] * x[3 * i + 2];   // z
            });

            // для комбинир. объектов
            Parallel.For(countCircles, countCircles + countOfCombinedObjects, i =>
             {
                 g[kk++] = Math.Pow((x[_n - 1] - radius[i]), 2) -
                     x[3 * i] * x[3 * i] -          // x
                     x[3 * i + 1] * x[3 * i + 1] -  // y
                     x[3 * i + 2] * x[3 * i + 2];   // z
             });

            // kk = count
            // (x[i]-x[j])^2 + (y[i]-y[j])^2 + (z[i]-z[j])^2 - (r[i]-r[j])^2 >=0
            // from count to count*(count-1)/2 - 1

            for (int i = 0; i < countCircles - 1; i++) // на каждой итерации увеличиваем на 3 счетч. по Z
            {
                for (int j = i + 1; j < countCircles; j++)
                {
                    g[kk++] = Math.Pow((x[3 * i] - x[3 * j]), 2.0)
                                  + Math.Pow((x[3 * i + 1] - x[3 * j + 1]), 2.0)
                                  + Math.Pow((x[3 * i + 2] - x[3 * j + 2]), 2.0)
                                  - Math.Pow((radius[i] - radius[j]), 2.0);
                }
            }
            ////Weight[i] * X[i] + Weight[i] * Y[i] + Weight[i] * Z[i]
            //Parallel.For(0, countCircles, i =>
            //{
            //    g[kk++] = Math.Pow(Weight[i] * x[3 * i], 2) + Math.Pow(Weight[i] * x[3 * i + 1], 2) + Math.Pow(Weight[i] * x[3 * i + 2], 2);
            //});
            return true;
        }

        public override bool Eval_jac_g(int n, double[] x, bool new_x, int m, int nele_jac, int[] iRow, int[] jCol, double[] values)
        {
            if (values == null)
            {
                int kk = 0,
                    g = 0;

                // (R-r[i])^2 - x[i]^2 - y[i]^2 - z[i]^2 >= 0
                // позиции R, Х и У, Z
                for (g = 0; g < countCircles; ++g)
                {
                    //R0 -> внешний шар 
                    iRow[kk] = g;
                    jCol[kk++] = _n - 1;

                    //X
                    iRow[kk] = g;
                    jCol[kk++] = 3 * g;

                    //Y
                    iRow[kk] = g;
                    jCol[kk++] = 3 * g + 1;

                    //Z
                    iRow[kk] = g;
                    jCol[kk++] = 3 * g + 2;
                }

                // для комбинированных объектов
                for (g = countCircles; g < countCircles + countOfCombinedObjects; ++g)
                {
                    //R0 -> внешний шар 
                    iRow[kk] = g;
                    jCol[kk++] = _n - 1;

                    //X
                    iRow[kk] = g;
                    jCol[kk++] = 3 * g;

                    //Y
                    iRow[kk] = g;
                    jCol[kk++] = 3 * g + 1;

                    //Z
                    iRow[kk] = g;
                    jCol[kk++] = 3 * g + 2;
                }

                // (x[i]-x[j])^2 + (y[i]-y[j])^2 + (z[i]-z[j])^2 - (r[i]-r[j])^2 >=0
                for (int i = 0; i < countCircles - 1; ++i)
                {
                    for (int j = i + 1; j < countCircles; ++j)
                    {
                        // -------  X[i], X[j] ------- 
                        iRow[kk] = g;
                        jCol[kk++] = 3 * i;
                        iRow[kk] = g;
                        jCol[kk++] = 3 * j;

                        // -------  Y[i], Y[j] ------- 
                        iRow[kk] = g; ;
                        jCol[kk++] = 3 * i + 1;
                        iRow[kk] = g;
                        jCol[kk++] = 3 * j + 1;

                        // -------  Z[i], Z[j] ------- 
                        iRow[kk] = g;
                        jCol[kk++] = 3 * i + 2;
                        iRow[kk] = g;
                        jCol[kk++] = 3 * j + 2;

                        ++g;
                    }
                }

                //// Weight[i] * X[i] + Weight[i] * Y[i] + Weight[i] * Z[i]
                //Parallel.For(0, countCircles, i =>
                //{
                //    // Weight[i] * X[i]
                //    iRow[kk] = g;
                //    jCol[kk++] = 0;

                //    // Weight[i] * Y[i] 
                //    iRow[kk] = g;
                //    jCol[kk++] = 1;

                //    // Weight[i] * Z[i]
                //    iRow[kk] = g;
                //    jCol[kk++] = 2;
                //    ++g;
                //});
            }
            else
            {
                // (R-r[i])^2 - x[i]^2 - y[i]^2 - z[i]^2 >= 0
                int kk = 0;
                for (int i = 0; i < countCircles; i++)
                {
                    values[kk] = 2.0 * (x[_n - 1] - radius[i]); // R0'
                    kk++;
                    values[kk] = -2.0 * x[3 * i]; //X'
                    kk++;
                    values[kk] = -2.0 * x[3 * i + 1]; //Y'
                    kk++;
                    values[kk] = -2.0 * x[3 * i + 2]; //Z'
                    kk++;
                }

                // для комбинированных объектов
                for (int i = countCircles; i < countCircles + countOfCombinedObjects; i++)
                {
                    values[kk] = 2.0 * (x[_n - 1] - radius[i]); // R0'
                    kk++;
                    values[kk] = -2.0 * x[3 * i]; //X'
                    kk++;
                    values[kk] = -2.0 * x[3 * i + 1]; //Y'
                    kk++;
                    values[kk] = -2.0 * x[3 * i + 2]; //Z'
                    kk++;
                }

                // (x[i]-x[j])^2 + (y[i]-y[j])^2 + (z[i]-z[j])^2 - (r[i]-r[j])^2 >=0
                //  Console.WriteLine("---------------------------------------");

                for (int i = 0; i < countCircles - 1; i++)
                {
                    for (int j = i + 1; j < countCircles; j++)
                    {
                        values[kk++] = 2.0 * (x[3 * i] - x[3 * j]); //X[i]'
                        values[kk++] = -2.0 * (x[3 * i] - x[3 * j]); //X[j]'

                        values[kk++] = 2.0 * (x[3 * i + 1] - x[3 * j + 1]); //Y[i]'
                        values[kk++] = -2.0 * (x[3 * i + 1] - x[3 * j + 1]); //Y[j]'

                        values[kk++] = 2.0 * (x[3 * i + 2] - x[3 * j + 2]); //Z[i]'
                        values[kk++] = -2.0 * (x[3 * i + 2] - x[3 * j + 2]); //Z[j]'
                    }
                }

                //////mx;my;mz
                //Parallel.For(0, countCircles, i =>
                //{
                //    values[kk] = 2 * Math.Pow(Weight[i], 2) * x[3 * i];
                //    kk++;
                //    values[kk] = 2 * Math.Pow(Weight[i], 2) * x[3 * i + 1];
                //    kk++;
                //    values[kk] = 2 * Math.Pow(Weight[i], 2) * x[3 * i + 2];
                //    kk++;
                //});
            }
            return true;
        }

        public override bool Eval_h(int n, double[] x, bool new_x, double obj_factor, int m, double[] lambda, bool new_lambda, int nele_hess, int[] iRow, int[] jCol, double[] values)
        {
            return false;
        }

        // Вычисляем диаметр как сумму всех радиусов
        private double DiametrSum(double[] radius)
        {
            var sum = 0.0;
            foreach (var rad in radius)
            {
                sum += 2 * rad;
            }
            return sum;
        }

        private double C_Multiply_by_Length(double[] x)
        {
            double Sum = 0;
            Parallel.For(0, countCircles - 1, i =>
                {
                    Parallel.For(i + 1, countCircles, j =>
                       {
                           Sum += C[i, j] * Math.Sqrt(Math.Pow(x[3 * i] - x[3 * j], 2)
                        + Math.Pow(x[3 * i + 1] - x[3 * j + 1], 2)
                        + Math.Pow(x[3 * i + 2] - x[3 * j + 2], 2));
                       });
                });
            return Sum;
        }

        public void Dispose()
        {
            _m = 0;
        }
    }
}
