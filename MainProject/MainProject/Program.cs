using Cureos.Numerics;
using hs071_cs;
using System;
using System.Diagnostics;
using System.Threading;
using TestProblemIpOpt.Helpers;
using TestProblemIpOpt.Interfaces;

namespace MainProject
{
    #region Delegates

    // outPut Delegates
    public delegate void PrintTextDel(string message); // делегат для вывода информации
    public delegate void PrintErrorMessageDel(string message); // делегат для вывода информации об ошибке
    public delegate void ShowResultDel(Data data); // делегат для вывода информации об ошибке
    public delegate void WriteResultToFileDel(Data data, string fileName); // делегат для вывода информации в файл
    public delegate void PrintResultCodeDel(string codeResult); // делегат для вывода информации о решении

    // Input Delegates
    public delegate void ReadResultFromFileDel(ref double[] xNach, ref double[] yNach, ref double[] zNach, ref double[] rNach,
        ref double RNach, ref int TotalBallCount, ref int holesCount, string filePath);
    public delegate double[] RadiusSumGenerateDel(double[] radius);
    public delegate double[] RadiusRandomGenerate(double maxRandRadius, int cCount);
    public delegate void XYZRGenerateDel(int cCount, double[] r, out double[] x, out double[] y, out double[] z, out double R);

    #endregion

    public class Program
    {
        public static void Main()
        {
            Dimension dimension = new Dimension();
            dimension.CreateInstance(3);
            Console.WriteLine(Thread.CurrentThread.Name + dimension.EntityOfProblem.SettedDimension);


            #region declaration of variables

            //#region delegates exemplars
            PrintTextDel Print = new PrintTextDel(OutPut.Write); // Write To Console some string
            //var SaveToFile = new WriteResultToFileDel(OutPut.SaveToFile); //  Writing result
            //var Show = new ShowResultDel(OutPut.ShowData); // return StatusCode of Optimization Problem

            //#endregion

            ////Start Point
            double[] rSortSum; // отсортированный массив радиусов, для ограничений
            double RNach = 0;
            double[] xNach; double[] yNach; double[] zNach; double[] rNach;
            //List<double[]> IpoptIterationData = new List<double[]>();

            #region Reading Data
            Print("\nSelect input method \n 1 --> Read from File \n 2 --> Random generate");
            Input.ChooseTypeReadingData(out int[] amountOfObjectsInEachComplexObject, out int TotalBallCount, out int holesCount, out xNach, out yNach, out zNach, out rNach, out RNach, out double maxRandRadius, out rSortSum);
            Print("\nChoose  type of external container \n 1 --> Circular container \n 2 --> Parallelogram container\nSelect-->");
            Input.ChooseTypeOfContainer(out IContainer container, RNach);
            #endregion

            Data startPointData = new Data(amountOfObjectsInEachComplexObject, xNach, yNach, zNach, rNach, container, TotalBallCount, holesCount);
            int c = xNach.Length;
            double[] xIter = new double[c]; double[] yIter = new double[c];
            double[] zIter = new double[c]; double[] rIter = new double[c];

            #region Формирование xyzR массива  
            double[] xyzFixR;
            XyzFixR(out xyzFixR, xNach, yNach, zNach, RNach, TotalBallCount);
            #endregion

            for (int i = 0; i < c; i++)
            {
                xIter[i] = 0;// xNach[i];
                yIter[i] = 0;// yNach[i];
                zIter[i] = 0;// zNach[i];
                rIter[i] = 0;// rNach[i];
            }
            double RIter = RNach;

            #region Matrix of Bond
            double[,] lengthBond;
            Print("Matrix of bond:\n");
            Input.SetC(out lengthBond);
            OutPut.SaveToC(lengthBond, TotalBallCount, "MatrixC");
            #endregion

            Print("\n=== Начальные значения ===");
            #endregion

            #region Solving Problem and Time measurement
            Stopwatch fullTaskTime = new Stopwatch();
            fullTaskTime.Start();

            //Show(startPointData);
            //SaveToFile(startPointData, "StartPointWithHoles");
            //Print("\n=== ================== ===");

            //* Solving with fixed radius -- Start point random generated or reading from file
            // * *********************************************************/
            Print("\n\n\t ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Print("\n\t\t ~~~ Solve problem ~~~");
            Print("\n\t ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Stopwatch TaskTime = new Stopwatch();
            TaskTime.Start();// Start time

            using (Adapter adaptor = new Adapter(startPointData))
            {
                RunTask(adaptor, xyzFixR, out xIter, out yIter, out zIter, c);
                RIter = xyzFixR[3 * TotalBallCount];
                // IpoptIterationData = adaptor.AllIteration;
                rNach = adaptor.radius;
            }

            //fixRTaskTime.Stop(); // Stop time
            //Print("\nВыполенение задачи RunTime: " + OutPut.getElapsedTime(fixRTaskTime));

            //Print("\n=== Результат расчётов ===");
            //Data optionalPoint = new Data(xIter, yIter, zIter, rNach, RIter, TotalBallCount, holesCount: holesCount, taskClassification: TaskClassification.FixedRadiusTask, type: objectType, Weight: weight, C: lengthBond);
            //Show(optionalPoint);
            //Print("\nQuality of solution = " + Density(rNach, RIter));

            //OutPut.SaveToFileAllIteration(optionalPoint, IpoptIterationData, "IterationFixedRadius", true);
            //SaveToFile(optionalPoint, "CoordinateWithHoles"); // запись результата в файл

            ////
            //fullTaskTime.Stop();
            //Print("\nВыполенение всей задачи RunTime: " + OutPut.getElapsedTime(fullTaskTime));
            //Print("\nQuality of solution = " + Density(rNach, RIter));
            //Print("\n========= Press <RETURN> to exit... ========= \n");
            //Console.ReadLine();
            #endregion
        }

        //private static void SetObjectTypeForBallsWithUnfixedRadius(ObjectType[] objectType)
        //{
        //    Parallel.For(0, objectType.Length, i =>
        //     {
        //         objectType[i] = ObjectType.VariiableRadiusBall;
        //     });
        //}

        //private static void CreateArrayWithObjectType(Data data, out ObjectType[] objectType)
        //{
        //    objectType = new ObjectType[data.objects.Length];
        //    for (int i = 0, j = 0; i < data.objects.Length; i++)
        //    {
        //        if (i <= j)
        //        {
        //            for (; j < data.holesCount; ++j, ++i)
        //            {
        //                data.objects[j].ObjectType = objectType[j] = (ObjectType)2;
        //            }
        //        }
        //        data.objects[i].ObjectType = objectType[i] = (ObjectType)3;
        //    }
        //}

        //private static void RandomizeStartPoint(Data data, out double[] constantRadius)
        //{
        //    Random random = new Random();
        //    constantRadius = new double[data.objectsCount];
        //    for (int i = 0; i < data.objectsCount; i++)
        //    {
        //        constantRadius[i] = data.objects[i].R;
        //        data.objects[i].R *= random.NextDouble();
        //    };
        //}

        //private static void CalculateWeight(Data data, out double[] weight)
        //{
        //    weight = new double[data.objects.Length];
        //    for (int i = 0; i < weight.Length; i++)
        //    {
        //        data.objects[i].Weight = weight[i] = (4.0 / 3.0) * Math.PI * Math.Pow(data.objects[i].R, 3);
        //    }
        //    Array.Sort(weight);
        //}

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

        private static void RunTask(Adapter op, double[] xyz, out double[] NewX, out double[] NewY, out double[] NewZ, int ballN)
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
                double obj;
                status = problem.SolveProblem(xyz, out obj, null, null, null, null);
            }
            taskWatch.Stop();
            new PrintResultCodeDel(OutPut.ReturnCodeMessage)("\nOptimization return status: " + status);

            NewX = new double[ballN];
            NewY = new double[ballN];
            NewZ = new double[ballN];

            for (int i = 0; i < ballN; i++)
            {
                NewX[i] = xyz[3 * i];
                NewY[i] = xyz[3 * i + 1];
                NewZ[i] = xyz[3 * i + 2];
            }
            new PrintTextDel(OutPut.WriteLine)("RunTime: " + OutPut.getElapsedTime(taskWatch));
        }

        //объемное заполнение внешнего шара
        private static double Density(double[] r, double R)
        {
            double totalCapacity = (4.0 / 3.0) * Math.PI * Math.Pow(R, 3.0),
                   realCapacity = 0;

            for (int i = 0; i < r.Length; i++)
            {
                realCapacity += (4.0 / 3.0) * Math.PI * Math.Pow(r[i], 3.0);
            }
            return realCapacity / totalCapacity;
        }
    }
}
