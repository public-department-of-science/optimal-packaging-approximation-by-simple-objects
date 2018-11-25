﻿using System.Collections.Generic;
using System.Diagnostics;
using Cureos.Numerics;
using hs071_cs;

namespace BooleanConfiguration
{
    public class RunTask
    {
        public ResultOfResearching SolveTheProblem(Data data)
        {
            try
            {
                IPOPTAdapter dataAdapter = new IPOPTAdapter(data);
                ResultOfResearching resultOfResearching = new ResultOfResearching();

                // lamdaArray => labda for each line
                // mainLambda => lamdaArray.MaxFromLambdaArray();
                OptimizationHelper.GettingArrayWithLabda(data.MatrixA, out double[] lamdaArray, out double mainLambda);

                for (int i = 0; i < lamdaArray.Length; i++)
                {
                    using (Ipopt ipoptSolver = new Ipopt(dataAdapter._n, dataAdapter._x_L, dataAdapter._x_U, dataAdapter._m, dataAdapter._g_L, dataAdapter._g_U,
                        dataAdapter._nele_jac, dataAdapter._nele_hess, dataAdapter.Eval_f, dataAdapter.Eval_g, dataAdapter.Eval_grad_f, dataAdapter.Eval_jac_g, dataAdapter.Eval_h))
                    {
                        Stopwatch taskTime = new Stopwatch();

                        ipoptSolver.AddOption("tol", 1e-2);
                        ipoptSolver.AddOption("mu_strategy", "adaptive");
                        ipoptSolver.AddOption("hessian_approximation", "limited-memory");
                        ipoptSolver.AddOption("max_iter", 3000);
                        ipoptSolver.AddOption("print_level", 3); // 0 <= value <= 12, default is 5

                        taskTime.Start();
                        double[] x = OptimizationHelper.GettingVariablesVector(data); // TODO variables array need to be in this one-demension array
                        IpoptReturnCode t = ipoptSolver.SolveProblem(x, out double resultValue, null, null, null, null);
                        taskTime.Stop();
                        resultOfResearching.AddNewResult(lamdaArray[i], new KeyValuePair<double[], Stopwatch>(x, taskTime));
                        // taskTime; => spent time
                    }
                }

                return resultOfResearching;
            }
            catch (System.Exception ex)
            {
                Output.ConsolePrint(ex.Message);
                return null;
            }
        }
    }
}
