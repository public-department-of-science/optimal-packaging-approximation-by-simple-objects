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
        private static int _countVarR = 0; // количество кругов с переменным радиусом

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

            const double maxRandRadius = 40; // максимальный радиус кругов r = 1..maxRandRadius

            #region Инициализация и обявление переменных

            DataHelper dataHelper = new DataHelper();

            double[] xStart = new double[circlesCount];
            double[] yStart = new double[circlesCount];
            double[] zNach = new double[circlesCount];
            double[] rStart = new double[circlesCount];
            int[] arrayWithGroups = null;
            double RNach = 0.0;

            double[] rIter = new double[circlesCount];
            double[] xIter = new double[circlesCount];
            double[] yIter = new double[circlesCount];
            double[] zIter = new double[circlesCount];

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
                        xIter[i] = xStart[i];
                        yIter[i] = yStart[i];
                        zIter[i] = zNach[i];
                        rIter[i] = rStart[i];
                    }

                    RIter = RNach;
                }
                break;
                default: return;
            }

            #region StartPoint

            Print("=== StartPoint ===");
            ShowData(xStart, yStart, zNach, rStart, RNach);
            Print("=== ================== ===");

            Data startPointData = new Data(xStart, yStart, zNach, rStart, RNach, circlesCount, 0, TaskClassification.FixedRadiusTask, type: null, Weight: null, C: null);
            OutPut.SaveToFile(startPointData, $"StartPoint");

            #endregion

            #region Groups

            Print("\n\t\t ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Print("\t\t ~~           Groups           ~~");
            Print("\t\t ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");

            Circle2D[] circles = new Circle2D[circlesCount];
            SetCirclesParameters(circlesCount, maxRandRadius, rStart, xStart, yStart, ref circles);

            ShowGroupsForEachCircle(circles, arrayWithGroups);

            try
            {
                dataHelper.SetGroups(ref circles, ref _countVarR);
                dataHelper.RandomizeCoordinate(ref circles, xIter, yIter, zIter, circlesCount);
                dataHelper.RandomizeRadiuses(ref circles, rStart, circlesCount);
            }
            catch (Exception ex)
            {
                Print(ex.Message);
                return;
            }

            IpoptReturnCode status;
            double[] radius = rStart.OrderBy(a => a).ToArray();

            Stopwatch varRTaskTime = new Stopwatch();
            using (VariableRadiusAdapter vr = new VariableRadiusAdapter(circles, radius))
            {
                varRTaskTime.Start();
                status = RunTask(vr, circles, out xIter, out yIter, out zIter, out rIter, out RIter);
                varRTaskTime.Stop();
            }

            ShowData(xIter, yIter, zIter, rIter, RIter);

            Data optionalPoint = new Data(xIter, yIter, zIter, rIter, RIter, circlesCount, holeCount: 0, taskClassification: TaskClassification.FixedRadiusTask, type: null, Weight: null, C: null);
            OutPut.SaveToFile(optionalPoint, "VariableRadius");

            #endregion

            Print("RunTime: " + getElapsedTime(varRTaskTime));
            Print($"Norma Var = {Norma(xStart, xIter, yStart, yIter, zNach, zIter, rStart, rIter)}");
            Console.ReadLine();
        }

        private static void ShowGroupsForEachCircle(Circle2D[] circles, int[] arrayWithGroups = null)
        {
            int i = 0;
            foreach (Circle2D item in circles)
            {
                if (arrayWithGroups is null)
                {
                    Print($"Circle[{i}].Group = {circles[i].Group}");
                }
                else
                {
                    Print($"Circle[{i}].Group = {arrayWithGroups[i]}");
                }
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

                circles[i].Odz.xL = xIter[i] - maxRandRadius;
                circles[i].Odz.xU = xIter[i] + maxRandRadius;
                circles[i].Odz.yL = yIter[i] - maxRandRadius;
                circles[i].Odz.yU = yIter[i] + maxRandRadius;
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
                    if (i < _countVarR)
                    {
                        x[2 * ballN + i] = c[i].Radius;
                    }
                }

                double coef = c.Sum(t => t.Radius) / 3;
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
                NewR[i] = (i < _countVarR) ? x[2 * ballN + i] : c[i].Radius;
            }

            Print($"Optimization return status: {status}");
            return status;
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

        public static double[] rRandomGenerate(double maxRandRadius, int cCount)
        {
            double[] arrR = new double[cCount];
            maxRandRadius--;

            for (int i = 0; i < cCount; ++i)
            {
                arrR[i] = 2 + Math.Round(_rnd.NextDouble() * maxRandRadius);
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
    }
}
