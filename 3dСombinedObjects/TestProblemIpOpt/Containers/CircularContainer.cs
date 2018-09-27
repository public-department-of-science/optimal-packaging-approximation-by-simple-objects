using System;
using TestProblemIpOpt.Interfaces;
using TestProblemIpOpt.Model;

namespace TestProblemIpOpt.Containers
{
    public class CircularContainer : IСircularContainer
    {
        public double Radius { get; set; }

        public Point CenterOfContainer { get; set; }

        public CircularContainer(double radius, Point centerOfContainer)
        {
            Radius = radius;
            CenterOfContainer = centerOfContainer ?? new Point(0, 0, 0);
        }

        #region Interface implementation methods

        public double EvalFunction(double[] x, int _n)
        {
            return x[_n - 1];
        }

        public void EvalFunctionGrad(double[] x, double[] grad_f, int _n)
        {
            grad_f[_n - 1] = 1;
        }

        public double AdditionalCriteriaFunction(double[] x, int _n)
        {
            return 0.0;
        }

        public void AdditionalCriteriaFunctionGrad(double[] x, double[] grad_f, int _n)
        {
            grad_f[_n - 1] += 0.0;
        }

        public void Eval_g(int n, double[] x, double[] g, ref int kk, double[] radius)
        {
            for (int i = 0; i < n; i++)
            {
                g[kk++] = Math.Pow((x[n - 1] - radius[i]), 2) -
                    x[3 * i] * x[3 * i] -          // x
                    x[3 * i + 1] * x[3 * i + 1] -  // y
                    x[3 * i + 2] * x[3 * i + 2];   // z
            }
        }

        public void Eval_jac_g(int n, double[] x, ref int kk, ref int g, double[] radius, int[] iRow, int[] jCol, double[] values, int countObjects)
        {
            if (values is null)
            {
                for (g = 0; g < countObjects; ++g)
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
                }
            }
            else
            {
                // (R-r[i])^2 - x[i]^2 - y[i]^2 - z[i]^2 >= 0
                for (int i = 0; i < countObjects; i++)// шаг по Z это каждый третий эл
                {
                    values[kk] = 2.0 * (x[n - 1] - radius[i]); // R0'
                    kk++;
                    values[kk] = -2.0 * x[3 * i]; //X'
                    kk++;
                    values[kk] = -2.0 * x[3 * i + 1]; //Y'
                    kk++;
                    values[kk] = -2.0 * x[3 * i + 2]; //Z'
                    kk++;
                }
            }
        }

        public bool Eval_h(int n, double[] x, bool new_x, double obj_factor, int m, double[] lambda, bool new_lambda, int nele_hess, int[] iRow, int[] jCol, double[] values)
        {
            return false;
        }

        #endregion
    }
}
