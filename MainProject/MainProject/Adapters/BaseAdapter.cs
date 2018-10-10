using System;

namespace hs071_cs
{
    public abstract class BaseAdapter:IDisposable
    {
        /// <summary>
        /// amount of variables in vector
        /// </summary>
        public int _n;

        /// <summary>
        /// amount of restrictions
        /// </summary>
        public int _m;

        /// <summary>
        /// amount of non-zero elements in jacobian
        /// </summary>
        public int _nele_jac;

        public int _nele_hess;

        /// <summary>
        /// array with lower bound of variables
        /// </summary>
        public double[] _x_L;

        /// <summary>
        /// array with upper bound of variables
        /// </summary>
        public double[] _x_U;

        /// <summary>
        /// array with lower bound for restrictions
        /// </summary>
        public double[] _g_L;

        /// <summary>
        /// array with upper bound for restrictions
        /// </summary>
        public double[] _g_U;

        protected Random random;

        public BaseAdapter()
        {
            _x_L = null;
            _x_U = null;
            _g_L = null;
            _g_U = null;

            random = new Random();
            _n = 0;
            _m = 0;
            _nele_jac = 0;
            _nele_hess = 0;
        }

        /// <summary>
        /// Evaluation of object function
        /// </summary>
        /// <param name="n"></param>
        /// <param name="x"></param>
        /// <param name="new_x"></param>
        /// <param name="obj_value"></param>
        /// <returns></returns>
        public abstract bool Eval_f(int n, double[] x, bool new_x, out double obj_value);

        /// <summary>
        /// Gradient for object function 
        /// </summary>
        /// <param name="n"></param>
        /// <param name="x"></param>
        /// <param name="new_x"></param>
        /// <param name="grad_f"></param>
        /// <returns></returns>
        public abstract bool Eval_grad_f(int n, double[] x, bool new_x, double[] grad_f);

        /// <summary>
        /// Evaluation of restriction function (compute on each iteration)
        /// </summary>
        /// <param name="n"></param>
        /// <param name="x"></param>
        /// <param name="new_x"></param>
        /// <param name="m"></param>
        /// <param name="g"></param>
        /// <returns></returns>
        public abstract bool Eval_g(int n, double[] x, bool new_x, int m, double[] g);

        /// <summary>
        /// Jacobian for each restriction function
        /// </summary>
        /// <param name="n"></param>
        /// <param name="x"></param>
        /// <param name="new_x"></param>
        /// <param name="m"></param>
        /// <param name="nele_jac"></param>
        /// <param name="iRow"></param>
        /// <param name="jCol"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public abstract bool Eval_jac_g(int n, double[] x, bool new_x, int m, int nele_jac, int[] iRow, int[] jCol, double[] values);

        public abstract bool Eval_h(int n, double[] x, bool new_x, double obj_factor, int m, double[] lambda, bool new_lambda, int nele_hess, int[] iRow, int[] jCol, double[] values);

        public void Dispose()
        {
            _m = 0;
        }
    }
}