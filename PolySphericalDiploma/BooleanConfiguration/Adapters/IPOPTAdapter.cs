﻿using System;
using BooleanConfiguration;
using BooleanConfiguration.Helper;
using BooleanConfiguration.Model;

namespace hs071_cs.Adapters
{
    // TODO: TO REALISE IPOPT_adapter
    internal class IPOPTAdapter : BaseAdaptor, IDisposable
    {
        /// <summary>
        /// Coefficients matrix for "current" objective function
        /// </summary>
        public double[][] MatrixOfCoef { get; set; }

        /// <summary>
        //// ""C"" * x <= b
        /// </summary>
        public double[][] ConstraintsMatrix { get; set; }

        public double[] MatrixX0 { get; set; }

        public IPOPTAdapter(Data data)
        {
            MatrixOfCoef = data.MatrixA;
            ConstraintsMatrix = data.ConstraintsMatrix;
            MatrixX0 = RestrictionHelper.GettingOneDemensionArrayFromString(data.N, data.Set.MatrixX0);
            _n = data.n;
            _m = data.m;

            _x_L = data.x_L;
            _x_U = data.x_U;

            _g_L = data.g_L;
            _g_U = data.g_U;

            _nele_jac = data.N * data.N;
            _nele_hess = 0;
        }

        public override bool Eval_f(int n, double[] x, bool new_x, out double obj_value)
        {
            // R -> min
            obj_value = IPOPTHelper.CalculateFunctionObjValue(MatrixOfCoef, x);// K2 * x[_n - 1] + K2 * C_Multiply_by_Length(x);
            return true;
        }

        public override bool Eval_grad_f(int n, double[] x, bool new_x, double[] grad_f)
        {
            IPOPTHelper.CulculateFunctionGrad(MatrixOfCoef, x, grad_f);
            return true;
        }

        public override bool Eval_g(int n, double[] x, bool new_x, int m, double[] g)
        {
            IPOPTHelper.CalculateEvalRestrictions(x, g, ConstraintsMatrix, MatrixX0);
            return true;
        }

        public override bool Eval_jac_g(int n, double[] x, bool new_x, int m, int nele_jac, int[] iRow, int[] jCol, double[] values)
        {
            if (values == null)
            {
                IPOPTHelper.SettingNonZeroElementsInJacPosition(iRow, jCol, ConstraintsMatrix);
            }
            else
            {
                IPOPTHelper.CalculateEvalJacRestrictions(x, values, ConstraintsMatrix, MatrixX0);
            }

            return true;
        }

        public override bool Eval_h(int n, double[] x, bool new_x, double obj_factor, int m, double[] lambda, bool new_lambda, int nele_hess, int[] iRow, int[] jCol, double[] values)
        {
            return false;
        }

        public void Dispose()
        {
            _m = 0;
        }
    }
}
