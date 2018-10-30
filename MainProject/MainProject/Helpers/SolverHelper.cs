﻿using Cureos.Numerics;
using System;
using System.Diagnostics;

namespace hs071_cs
{
    public class SolverHelper
    {
        #region 

        public Data SolveTheProblem(Adapter dataAdapter, Data data)
        {
            Data result = null;
            Stopwatch taskWatch = new Stopwatch();
            taskWatch.Start();

            using (Ipopt problem = new Ipopt(dataAdapter._n, dataAdapter._x_L, dataAdapter._x_U, dataAdapter._m, dataAdapter._g_L, dataAdapter._g_U,
                dataAdapter._nele_jac, dataAdapter._nele_hess, dataAdapter.Eval_f, dataAdapter.Eval_g, dataAdapter.Eval_grad_f, dataAdapter.Eval_jac_g, dataAdapter.Eval_h))
            {
                problem.AddOption("tol", 1e-4);
                problem.AddOption("mu_strategy", "adaptive");
                problem.AddOption("hessian_approximation", "limited-memory");
                problem.AddOption("max_iter", 3000);
                problem.AddOption("print_level", 3); // 0 <= value <= 12, default is 5

                double resultVector;
                double[] t = data.DataToArray();

                IpoptReturnCode status = problem.SolveProblem(t, out resultVector, null, null, null, null);

                result = data.ArrayToData(t);
                OutPut.ReturnCodeMessage("\nTask status: " + status);
            }

            taskWatch.Stop();
            Program.Print("\n RunTime: " + GetElapsedTime(taskWatch));
            return result;
        }

        private void XyzFixR(out double[] xyzFixR, double[] xNach, double[] yNach, double[] zNach, double RNach, int ballCount)
        {
            xyzFixR = new double[3 * ballCount + 1];

            int j = 0;
            for (int i = 0; i < ballCount; i++)
            {
                xyzFixR[j] = xNach[i];
                xyzFixR[++j] = yNach[i];
                xyzFixR[++j] = zNach[i];
                j++;
            }
            xyzFixR[3 * ballCount] = RNach;
        }

        #endregion

        #region Measurement methods 

        public double Density(Data data)
        {
            return 0.0;
        }

        public static string GetElapsedTime(Stopwatch Watch)
        {
            TimeSpan ts = Watch.Elapsed;
            return string.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
        }

        #endregion
    }
}