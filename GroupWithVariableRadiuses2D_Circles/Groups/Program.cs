using System;
using System.Diagnostics;
using System.Linq;
using Cureos.Numerics;
using Groups;
using hs071_cs.ObjectOptimazation;

namespace hs071_cs
{
    public delegate void Print(string text);

    public class Program
    {

        private static Random _rnd = new Random();
        private static int ballN;
        private static int counterOfCirclesWithVariableRadius = 0; // количество кругов с переменным радиусом

        public static Print Print { get; set; }
        static Program()
        {
            Print = new Print(OutPut.WriteLine);
        }

        public static void Main()
        {
            int circlesCount = 0; // количество кругов

            try
            {
                Console.Write("N = ");
                circlesCount = int.Parse(Console.ReadLine());
            }
            catch (Exception ex)
            {
                Print(ex.Message);
                circlesCount = 15;
            }

            const double maxRandRadius = 30; // максимальный радиус кругов r = 1..maxRandRadius

            #region Инициализация и обявление переменных

            DataHelper dataHelper = new DataHelper();

            double[] xStart = new double[circlesCount];
            double[] yStart = new double[circlesCount];
            double[] zNach = new double[circlesCount];
            double[] rStart = new double[circlesCount];
            int[] arrayWithGroups = null;
            double RNach = 0.0;

            double[] rIter = new double[circlesCount];
            double[] xFixedIter = new double[circlesCount];
            double[] yFixedIter = new double[circlesCount];
            double[] zStart = new double[circlesCount];

            double[] rBest = new double[circlesCount];
            double[] xBest = new double[circlesCount];
            double[] yBest = new double[circlesCount];
            double[] zBest = new double[circlesCount];
            double RIter = 0.0;

            #endregion

            ballN = circlesCount; // для использования вне Main (количество кругов)

            Print("\nSelect input method \n 1 --> Read from File \n 2 --> Random generate");

            switch (Console.ReadLine())
            {
                case "1":
                    Input.ReadFromFile(ref xStart, ref yStart, ref zNach, ref rStart, ref RNach, out arrayWithGroups, ref circlesCount, "");
                    break;
                case "2":
                {
                    Print("~~~ Randomize StartPoint ~~~");
                    Stopwatch stopWatch = new Stopwatch();

                    rStart = rRandomGenerate(maxRandRadius, circlesCount);

                    xyRRandomGenerateAvg(circlesCount, ref rStart, ref xStart, ref yStart, ref zNach, ref RNach);
                    Print("\n\t~~~ Генерируем точки с которых будем считать ~~~");

                    for (int i = 0; i < circlesCount; ++i)
                    {
                        xFixedIter[i] = xStart[i];
                        yFixedIter[i] = yStart[i];
                        zStart[i] = zNach[i];
                        rIter[i] = rStart[i];
                    }

                    RIter = RNach;
                }
                break;
                default:
                    return;
            }

            #region StartPoint

            Print("=== StartPoint ===");
            ShowData(xStart, yStart, zNach, rStart, RNach);
            Print("=== ================== ===");

            Data startPointData = new Data(xStart, yStart, zNach, rStart, RNach, circlesCount, 0, TaskClassification.FixedRadiusTask, type: null, Weight: null, C: null);
            OutPut.SaveToFile(startPointData, $"RandomPoint");

            //Fixed radius
            using (FixedRadius3dAdaptor adaptor = new FixedRadius3dAdaptor(startPointData))
            {
                XyzFixR(out double[] xyzFixR, xStart, yStart, zNach, RNach, circlesCount);

                RunTask(adaptor, xyzFixR, out xFixedIter, out yFixedIter, out zNach, circlesCount);
                RIter = xyzFixR[3 * circlesCount];
                rStart = adaptor.radius;
            }

            Data optionalPointFixedPoint = new Data(xFixedIter, yFixedIter, zStart, rStart, RIter, circlesCount, holeCount: 0, taskClassification: TaskClassification.FixedRadiusTask);
            OutPut.SaveToFile(optionalPointFixedPoint, $"FixedRad {RIter}");

            #endregion

            #region Groups

            Print("\n\t\t ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Print("\t\t ~~           Groups           ~~");
            Print("\t\t ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");

            Circle2D[] circles = new Circle2D[circlesCount];

            SetCirclesParameters(circlesCount, maxRandRadius, rStart, xFixedIter, yFixedIter, ref circles);

            //#region 5*10

            //for (int i = 0; i < 10; i++)
            //{
            //    Console.WriteLine($"\n5*10 ==> {i}");

            //    try
            //    {
            //        SetAndShowGroupsForEachCircle(ref circles, arrayWithGroups, ref counterOfCirclesWithVariableRadius);
            //        dataHelper.SetGroups(ref circles, ref counterOfCirclesWithVariableRadius, "2");
            //        dataHelper.RandomizeCoordinate(ref circles, xFixedIter, yFixedIter, zNach, circlesCount);
            //        dataHelper.RandomizeRadiuses(ref circles, rStart, circlesCount);
            //    }
            //    catch (Exception ex)
            //    {
            //        Print(ex.Message);
            //        return;
            //    }

            //    IpoptReturnCode status;
            //    double[] radius = rStart.OrderBy(a => a).ToArray();


            //    Stopwatch varRTaskTime = new Stopwatch();
            //    using (VariableRadiusAdapter vr = new VariableRadiusAdapter(circles, radius))
            //    {
            //        varRTaskTime.Start();
            //        status = RunTask(vr, circles, out xBest, out yBest, out zBest, out rIter, out RIter);
            //        varRTaskTime.Stop();
            //    }

            //    ShowData(xBest, yBest, zBest, rIter, RIter);

            //    Data optionalPoint = new Data(xBest, yBest, zBest, rIter, RIter, circlesCount, holeCount: 0, taskClassification: TaskClassification.FixedRadiusTask, type: null, Weight: null, C: null);
            //    OutPut.SaveToFile(optionalPoint, $"VariableRadius_5_by_10_{i}_R={RIter}");

            //    Print("RunTime: " + getElapsedTime(varRTaskTime));
            //    Print($"Norma Var = {Norma(xStart, xFixedIter, yStart, yFixedIter, zNach, zStart, rStart, rIter)}");
            //}

            //#endregion

            #region 10max var

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine($"\n 10max var {i}");

                try
                {
                    SetAndShowGroupsForEachCircle(ref circles, arrayWithGroups, ref counterOfCirclesWithVariableRadius);
                    dataHelper.SetGroups(ref circles, ref counterOfCirclesWithVariableRadius, "1");
                    dataHelper.RandomizeCoordinate(ref circles, xFixedIter, yFixedIter, zNach, circlesCount);
                    dataHelper.RandomizeRadiuses(ref circles, rStart, circlesCount);
                }
                catch (Exception ex)
                {
                    Print(ex.Message);
                    return;
                }

                IpoptReturnCode status;
                double[] radius = GetVariableRadiuses(circles);

                Stopwatch varRTaskTime = new Stopwatch();
                using (VariableRadiusAdapter vr = new VariableRadiusAdapter(circles, radius))
                {
                    varRTaskTime.Start();
                    status = RunTask(vr, circles, out xBest, out yBest, out zBest, out rIter, out RIter);
                    varRTaskTime.Stop();
                }

                ShowData(xBest, yBest, zBest, rIter, RIter);

                Data optionalPoint = new Data(xBest, yBest, zBest, rIter, RIter, circlesCount, holeCount: 0, taskClassification: TaskClassification.FixedRadiusTask, type: null, Weight: null, C: null);
                OutPut.SaveToFile(optionalPoint, $"VariableRadius_10MaxVar_OthersFixed_{i}_R={RIter}");

                Print("RunTime: " + getElapsedTime(varRTaskTime));
                Print($"Norma Var = {Norma(xStart, xFixedIter, yStart, yFixedIter, zNach, zStart, rStart, rIter)}");
            }

            #endregion


            #endregion


            Console.ReadLine();
        }

        private static double[] GetVariableRadiuses(Circle2D[] circles)
        {
            double[] arrayWithSortedRadiuses = null;
            int amountOfVariableRad = circles.Where(x => x.Group != 0).Count();

            if (amountOfVariableRad != 0)
            {
                arrayWithSortedRadiuses = new double[amountOfVariableRad];

                for (int i = 0, j = 0; i < circles.Length; i++)
                {
                    if (circles[i].Group != 0)
                    {
                        arrayWithSortedRadiuses[j] = circles[i].Radius;
                    }
                }
            }

            return arrayWithSortedRadiuses.OrderBy(x => x).ToArray();
        }

        private static void SetAndShowGroupsForEachCircle(ref Circle2D[] circles, int[] arrayWithGroups, ref int counterOfCirclesWithVariableRadius)
        {
            counterOfCirclesWithVariableRadius = 0;
            int i = 0;
            if (arrayWithGroups is null)
            {
                Print("All elements in fixedRadius group!!");
                return;
            }

            foreach (Circle2D item in circles)
            {
                if (arrayWithGroups[i] != 0)
                {
                    item.Group = arrayWithGroups[i];
                    ++counterOfCirclesWithVariableRadius;
                }
                ++i;
            }

            i = 0;
            foreach (Circle2D item in circles)
            {
                Print($"Circle[{i}].Group = {circles[i].Group}");
                ++i;
            }
        }

        private static void SetCirclesParameters(int ballsCount, double maxRandRadius, double[] rNach, double[] xIter, double[] yIter, ref Circle2D[] circles)
        {
            for (int i = 0; i < ballsCount; ++i)
            {
                circles[i] = new Circle2D
                {
                    Group = 0
                };

                circles[i].Odz.xL = Ipopt.NegativeInfinity;// xIter[i] - maxRandRadius;
                circles[i].Odz.xU = Ipopt.PositiveInfinity;// xIter[i] + maxRandRadius;
                circles[i].Odz.yL = Ipopt.NegativeInfinity;// yIter[i] - maxRandRadius;
                circles[i].Odz.yU = Ipopt.PositiveInfinity;// yIter[i] + maxRandRadius;
                circles[i].Odz.rL = 0;
                circles[i].Odz.rU = rNach.Max();
                circles[i].Radius = rNach[i];
            }
        }

        private static double Norma(double[] xNach, double[] xIter, double[] yNach, double[] yIter, double[] zNach, double[] zIter, double[] rNach, double[] rIter)
        {
            double norma = 0.0;
            for (int i = 0; i < xNach.Length; i++)
            {
                norma += Math.Pow(xNach[i] - xIter[i], 2);
                norma += Math.Pow(yNach[i] - yIter[i], 2);
            }

            return norma;
        }

        private static IpoptReturnCode RunTask(VariableRadiusAdapter op, Circle2D[] c, out double[] NewX, out double[] NewY, out double[] NewZ, out double[] NewR, out double R0)
        {
            Stopwatch timer = new Stopwatch();

            IpoptReturnCode status;
            double[] x = new double[op._n];
            timer.Start();
            /* allocate space for the initial point and set the values */

            using (Ipopt problem = new Ipopt(op._n, op._x_L, op._x_U, op._m, op._g_L, op._g_U, op._nele_jac, op._nele_hess, op.eval_f, op.eval_g, op.eval_grad_f, op.eval_jac_g, op.eval_h))
            {
                problem.AddOption("tol", 1e-3);
                problem.AddOption("mu_strategy", "adaptive");
                problem.AddOption("hessian_approximation", "limited-memory");
                problem.AddOption("output_file", op.ToString() + "_" + DateTime.Now.ToShortDateString() + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + ".txt");
                problem.AddOption("file_print_level", 0);
                problem.AddOption("max_iter", 4000);
                problem.AddOption("print_level", 3); // 0<= value <= 12, default value is 5

                for (int i = 0; i < ballN; ++i)
                {
                    x[2 * i] = c[i].Coordinate.X;
                    x[2 * i + 1] = c[i].Coordinate.Y;

                    if (c[i].Group != 0)
                    {
                        x[2 * ballN + i] = c[i].Radius;
                    }
                }

                double coef = c.OrderByDescending(t => t.Radius).Take(4).Sum(t => t.Radius) * 0.4;
                x[x.Length - 1] = coef;
                status = problem.SolveProblem(x, out double obj, null, null, null, null);
            }
            timer.Stop();

            NewX = new double[ballN];
            NewY = new double[ballN];
            NewZ = new double[ballN];
            NewR = new double[ballN];
            R0 = x[x.Length - 1];

            for (int i = 0; i < ballN; ++i)
            {
                NewX[i] = x[2 * i];
                NewY[i] = x[2 * i + 1];
                NewR[i] = c[i].Group != 0 ? x[2 * ballN + i] : c[i].Radius;
            }

            Print($"Optimization return status: {status}");
            return status;
        }

        private static void RunTask(FixedRadius3dAdaptor op, double[] xyz, out double[] NewX, out double[] NewY, out double[] NewZ, int ballN)
        {
            Stopwatch taskWatch = new Stopwatch();
            IpoptReturnCode status;
            taskWatch.Start();
            using (Ipopt problem = new Ipopt(op._n, op._x_L, op._x_U, op._m, op._g_L, op._g_U, op._nele_jac, op._nele_hess, op.Eval_f, op.Eval_g, op.Eval_grad_f, op.Eval_jac_g, op.Eval_h))
            {
                // https://www.coin-or.org/Ipopt/documentation/node41.html#opt:print_options_documentation
                problem.AddOption("tol", 1e-2);
                problem.AddOption("mu_strategy", "adaptive");
                problem.AddOption("hessian_approximation", "limited-memory");
                problem.AddOption("max_iter", 3000);
                problem.AddOption("print_level", 3); // 0 <= value <= 12, default is 5

                /* solve the problem */
                status = problem.SolveProblem(xyz, out double obj, null, null, null, null);
            }
            taskWatch.Stop();
            Print("\nOptimization return status: " + status);

            NewX = new double[ballN];
            NewY = new double[ballN];
            NewZ = new double[ballN];

            for (int i = 0; i < ballN; i++)
            {
                NewX[i] = xyz[3 * i];
                NewY[i] = xyz[3 * i + 1];
                NewZ[i] = xyz[3 * i + 2];
            }
            Print("RunTime: " + OutPut.getElapsedTime(taskWatch));
        }

        private static string getElapsedTime(Stopwatch Watch)
        {
            TimeSpan ts = Watch.Elapsed;
            return string.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
        }

        private static void ShowData(double[] dataX, double[] dataY, double[] dataZ, double[] radius, double R)
        {
            int cicleCount = radius.Length;
            int iterCount = 1;

            for (int i = 0; i < cicleCount; ++i)
            {
                Print($" x[{iterCount++}] = {dataX[i]}");
                Print($" y[{iterCount++}] = {dataY[i]}");
                Print($" z[{iterCount++}] = {dataZ[i]}");
                Print($" r[{iterCount++}] = {radius[i]}");
            }

            Print($" R_External[{iterCount++}] = {R}");
        }

        public static double[] rRandomGenerate(double maxRandRadius, int ciclesCount)
        {
            double[] arrR = new double[ciclesCount];
            maxRandRadius--;

            for (int i = 0; i < ciclesCount; ++i)
            {
                arrR[i] = Math.Round(ciclesCount * _rnd.NextDouble()) + 1;
            }

            return arrR;
        }

        public static void xyRRandomGenerateAvg(int cCount, ref double[] r, ref double[] x, ref double[] y, ref double[] z, ref double R)
        {
            x = new double[cCount];
            y = new double[cCount];
            z = new double[cCount];

            double avgCircle = r.Average();
            double maxCircle = r.Max();

            double maxX = 0;
            double maxY = 0;
            double maxZ = 0;
            double maxR = 0;
            double maxRXYZ = 0;
            for (int i = 0; i < cCount; ++i)
            {
                x[i] = 10 * avgCircle * (_rnd.NextDouble() - 0.5);
                y[i] = 10 * avgCircle * (_rnd.NextDouble() - 0.5);
                z[i] = 0;// 10 * avgCircle * (_rnd.NextDouble() - 0.5);

                maxX = Math.Max(Math.Abs(x[i] + r[i]), Math.Abs(x[i] - r[i]));
                maxY = Math.Max(Math.Abs(y[i] + r[i]), Math.Abs(y[i] - r[i]));
                maxZ = Math.Max(Math.Abs(z[i] + r[i]), Math.Abs(z[i] - r[i]));
                maxR = Math.Max(Math.Max(maxX, maxY), maxZ);
                maxRXYZ = Math.Max(maxRXYZ, maxR);
            }
            R = maxRXYZ;
        }

        public static double[] raSumGenerate(double[] radius)
        {
            int cCount = radius.Length;
            double[] arrSumR = new double[cCount];
            radius = radius.OrderBy(a => a).ToArray();
            for (int i = 0; i < cCount; ++i)
            {
                for (int k = 0; k <= i; ++k)
                {
                    arrSumR[i] += radius[k];
                }
            }

            return arrSumR;
        }

        private static void XyzFixR(out double[] xyzFixR, double[] xNach, double[] yNach, double[] zNach, double RNach, int ballCount)
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
    }
}
