﻿using MainProject;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using TestProblemIpOpt.Containers;
using TestProblemIpOpt.Interfaces;

namespace hs071_cs
{
    internal static class Input
    {
        private static Random rnd = new Random();
        private static readonly PrintErrorMessageDel PrintError = new PrintErrorMessageDel(OutPut.ErrorMessage); // Print error and ErrorMessage

        // Methods (Properties) for access to private field circle
        public static int ObjectsCount { get; set; } = 0;

        // Generate Value for Extern Ball which include others less balls
        public static double[] RadiusRandomGenerate(double maxRandRadius, int cCount)
        {
            return ExternalRadius(ref maxRandRadius, cCount);
        }

        private static double[] ExternalRadius(ref double maxRandRadius, int cCount)
        {
            double[] arrR = new double[cCount];
            maxRandRadius--;
            for (int i = 0; i < cCount; ++i)
            {//arrR[i] = 1 + _rnd.NextDouble() * maxRandRadius; // 1..maxRadius
                arrR[i] = 2 + Math.Round(rnd.NextDouble() * maxRandRadius); // 1..maxRadius
                Thread.Sleep(14);
            }
            arrR = arrR.OrderBy(a => a).ToArray();
            return arrR;
        }

        //генерирование произвольной матрицы связей
        public static void SetC(out double[,] matrixC)
        {
            matrixC = RandGenerateBondMatrix();
        }

        private static double[,] RandGenerateBondMatrix()
        {
            double[,] matrixC = new double[ObjectsCount, ObjectsCount];
            for (int i = 0; i < ObjectsCount; i++)
            {
                for (int j = i; j < ObjectsCount; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    matrixC[i, j] = 0;// rnd.NextDouble();
                    //if (matrixC[i, j] < 0.3)
                    //{
                    //    matrixC[i, j] = Math.Round(matrixC[i, j] * 100);
                    //}
                    //else
                    //{
                    //    matrixC[i, j] = 0;
                    //}
                    matrixC[j, i] = matrixC[i, j];
                }
            }
            for (int i = 0; i < ObjectsCount; i++)
            {
                for (int j = 0; j < ObjectsCount; j++)
                {
                    OutPut.Write(matrixC[i, j] + " ");
                }
                OutPut.WriteLine();
            }
            return matrixC;
        }

        //Type Reading data
        public static void ChooseTypeReadingData(out int[] amountOfObjectsInEachComplexObject, out int TotalBallCount, out double[] xNach, out double[] yNach, out double[] zNach, out double[] rNach, out double RNach, out double maxRandRadius, out double[] rSortSum)
        {
            xNach = new double[ObjectsCount];
            yNach = new double[ObjectsCount];
            zNach = new double[ObjectsCount];
            rNach = new double[ObjectsCount];
            TotalBallCount = 0;
            RNach = 0;
            maxRandRadius = 0;
            rSortSum = new double[ObjectsCount];

            TypeOFReadingData(out amountOfObjectsInEachComplexObject, ref TotalBallCount, ref xNach, ref yNach, ref zNach, ref rNach, ref RNach, ref maxRandRadius, ref rSortSum);
        }

        public static void ChooseTypeOfContainer(out IContainer container, double rNach)
        {
            switch (Console.ReadLine())
            {
                case "1":
                    container = new CircularContainer(rNach, new TestProblemIpOpt.Model.Point());
                    break;
                case "2":
                    container = new ParallelogramContainer();
                    break;
                default:
                    container = new CircularContainer(rNach, new TestProblemIpOpt.Model.Point());
                    break;
            }
        }

        private static void TypeOFReadingData(out int[] amountOfObjectsInEachComplexObject, ref int TotalBallCount, ref double[] xNach, ref double[] yNach,
            ref double[] zNach, ref double[] rNach, ref double RNach, ref double maxRandRadius, ref double[] rSortSum)
        {
            amountOfObjectsInEachComplexObject = null;
            int keyCode;
            try
            {
                OutPut.Write("\nInput 1 or 2 ==>");
                keyCode = int.Parse(Console.ReadLine());
            }
            catch (Exception ex)
            {
                PrintError("\nInput Error --> {0}" + ex.Message);
                keyCode = 2;
            }
            switch (keyCode)
            {
                case 1:
                    try
                    {
                        ReadFromFile(out amountOfObjectsInEachComplexObject, ref xNach, ref yNach, ref zNach, ref rNach, ref RNach, ref TotalBallCount, "ChangedCoordinateWithHoles");
                        rSortSum = raSumGenerate(rNach); // отсортированные радиусы r[0]; r[0] + r[1];
                    }
                    catch (IOException exIO)
                    {
                        OutPut.WriteLine(string.Format(" Объект вызвавший ошибку {0}, вознuкшая ошибка - {1}  ", exIO.Source, exIO.Message));
                        TotalBallCount = 1;
                        maxRandRadius = 0;
                    }
                    break;
                case 2:
                    try
                    {

                        SetIntegerValue(ref TotalBallCount, "Total balls count"); // кол шаров всего
                        SetDoubleValue(ref maxRandRadius, "Max radius"); // радиус

                        NumberLessOrEqualZero(TotalBallCount);
                        NumberLessOrEqualZero(maxRandRadius);
                    }
                    catch (Exception ex)
                    {
                        PrintError("\nError -> {0}" + ex.Message);
                        TotalBallCount = 1;
                        maxRandRadius = 0;
                    }
                    rNach = RadiusRandomGenerate(maxRandRadius, TotalBallCount); //"~~~ Генерирования случайными числами начальных радиусов ~~~
                    rSortSum = raSumGenerate(rNach); // отсортированные радиусы r[0]; r[0] + r[1];
                    XyzRRandomGenerateAvg(TotalBallCount, rNach, out xNach, out yNach, out zNach, out RNach); // генерируем начальные точки x,y,r,R
                    break;
                default:
                    break;
            }
            ObjectsCount = TotalBallCount; // для использования в классе
        }

        // установка целочисленного значения в переменную
        public static void SetIntegerValue(ref int value, string printedText)
        {
            value = TrySetIntegerValue(printedText);
        }

        private static int TrySetIntegerValue(string printedText)
        {
            int val; // setted integer value
            try
            {
                OutPut.Write(string.Format("{0} = ", printedText));
                val = Convert.ToInt32(Console.ReadLine()); // количество кругов
            }
            catch (Exception ex)
            {
                PrintError("\nInput Error --> " + ex.Message);
                val = -1;
            }
            return val;
        }

        // установка вещественного значения в переменную
        public static void SetDoubleValue(ref double value, string printedText)
        {
            value = TrySetDoubleValue(printedText);
        }

        private static double TrySetDoubleValue(string printedText)
        {
            double val; // setted double value
            try
            {
                OutPut.Write(string.Format("{0} = ", printedText));
                val = Convert.ToDouble(Console.ReadLine()); // количество кругов
            }
            catch (Exception ex)
            {
                PrintError("\nInput Error --> " + ex.Message);
                val = -1;
            }
            return val;
        }

        private static void NumberLessOrEqualZero<T>(T value)
        {
            if (Convert.ToInt32(value) <= 0)
            {
                throw new Exception("Value less 0!!");
            }
        }

        // Генератор начальных r - радиусов внутренних кругов
        // r[i] = r[i] + rnd()*min{r}
        private static double[] rRandomGenerate(double[] radius)
        {
            return GenerateRadius(radius);
        }

        private static double[] GenerateRadius(double[] radius)
        {
            int cCount = radius.Length;
            double minR = radius[0];
            double maxR = radius[0];
            for (int i = 1; i < cCount; ++i)
            {
                if (minR > radius[i])
                {
                    minR = radius[i];
                }

                if (maxR < radius[i])
                {
                    maxR = radius[i];
                }
            }
            double[] arrR = new double[cCount];
            for (int i = 0; i < cCount; ++i)
            {
                //arrR[i] = radiuses[i] + (_rnd.NextDouble() - 0.5) * minR;
                arrR[i] = radius[i] + 0.5 * (rnd.NextDouble() - 0.5) * (maxR - minR);
                if (arrR[i] < minR)
                {
                    arrR[i] = minR;
                }

                if (arrR[i] > maxR)
                {
                    arrR[i] = maxR;
                }
            }
            return arrR;
        }

        // Reading from file while end (x, y, z, r, R)
        public static void ReadFromFile(out int[] amountOfObjectsInEachComplexObject, ref double[] x, ref double[] y, ref double[] z, ref double[] r, ref double R, ref int TotalBallCount, string fileName)
        {
            ReadDataFromFile(out amountOfObjectsInEachComplexObject, ref x, ref y, ref z, ref r, ref R, ref TotalBallCount, fileName);
        }

        private static void ReadDataFromFile(out int[] amountOfObjectsInEachComplexObject, ref double[] x, ref double[] y, ref double[] z, ref double[] r, ref double R, ref int TotalBallCount, string fileName)
        {
            try
            {
                amountOfObjectsInEachComplexObject = null;
                string readPath = @"D:\" + fileName + ".txt";
                FileInfo fileInfo = new FileInfo(readPath);
                if (fileInfo.Exists)
                {
                    StreamReader sr = new StreamReader(readPath);
                    string allReadedSymbols = "";
                    string[] xyzrString = new string[4];
                    string currentLine = "";
                    int i = 0;
                    while (currentLine != null)
                    {
                        currentLine = sr.ReadLine();
                        if (currentLine != null)
                        {
                            allReadedSymbols += currentLine + ";";
                            i++;
                        }
                    }

                    amountOfObjectsInEachComplexObject = new int[i - 1];

                    string[] arrayOfLines = allReadedSymbols.Split(';');
                    TotalBallCount = ObjectsCount = i - 1;// first
                    R = Convert.ToDouble(arrayOfLines[0].Split(' ')[3].Replace('.', ',').Trim()); // external radius

                    // check demention
                    if ((i - 1) != TotalBallCount) /// (i-2)
                    {
                        throw new Exception("Dimension do not match!");
                    }
                    else
                    {
                        int Elements = 0;

                        for (int j = 1; j < arrayOfLines.Length - 1; j++)
                        {
                            amountOfObjectsInEachComplexObject[j - 1] = arrayOfLines[j].Split(' ').Length / 4;
                            Elements += amountOfObjectsInEachComplexObject[j - 1];
                        }
                        x = new double[Elements];
                        y = new double[Elements];
                        z = new double[Elements];
                        r = new double[Elements];
                        int countOfElementsInLine = 0;

                        for (i = 1; i < arrayOfLines.Length - 1; i++) //// !!!!!! arrayOfLines.Length - 2
                        {
                            xyzrString = arrayOfLines[i].Split(' ');
                            for (int k = 0; k < xyzrString.Length; k++)
                            {
                                xyzrString[k] = xyzrString[k].Replace('.', ',').Trim();
                            }
                            for (int j = 0; j < xyzrString.Length; j++)
                            {
                                x[countOfElementsInLine] = Convert.ToDouble(xyzrString[j++]);
                                y[countOfElementsInLine] = Convert.ToDouble(xyzrString[j++]);
                                z[countOfElementsInLine] = Convert.ToDouble(xyzrString[j++]);
                                r[countOfElementsInLine] = Convert.ToDouble(xyzrString[j]);
                                ++countOfElementsInLine;
                            }
                        }
                        OutPut.WriteLine("Data has been read!");
                    }
                }
            }
            catch (Exception ex)
            {
                amountOfObjectsInEachComplexObject = null;
                PrintError(string.Format("Data not readed! Error --> {0}", ex.Message));
                OutPut.WriteLine("We generate random coordinate from 0 to 10");

                r = RadiusRandomGenerate(10, ObjectsCount); //"~~~ Генерирования случайными числами начальных радиусов ~~~
                XyzRRandomGenerateAvg(ObjectsCount, r, out x, out y, out z, out R); // генерируем начальные точки x,y,r,R
                return;
            }
        }

        // Генератор начальных x and y and R
        // в диапазоне от -max(r[i]) до max(r[i])
        public static void XyzRRandomGenerateAvg(int cCount, double[] r, out double[] x, out double[] y, out double[] z, out double R)
        {
            XYZRRandGenerateAVG(cCount, r, out x, out y, out z, out R);
        }

        private static void XYZRRandGenerateAVG(int cCount, double[] r, out double[] x, out double[] y, out double[] z, out double R)
        {
            x = new double[cCount];
            y = new double[cCount];
            z = new double[cCount];

            double avgCircle = r.Average();
            double maxCircle = r.Max();
            // генеририруем R из массивов Х и У
            double maxX = 0;
            double maxY = 0;
            double maxZ = 0;
            double maxR = 0;
            double maxRXYZ = 0;
            for (int i = 0; i < cCount; i++)
            {
                x[i] = 10 * avgCircle * (rnd.NextDouble() - 0.5);
                y[i] = 10 * avgCircle * (rnd.NextDouble() - 0.5);
                z[i] = 10 * avgCircle * (rnd.NextDouble() - 0.5);

                maxX = Math.Max(Math.Abs(x[i] + r[i]), Math.Abs(x[i] - r[i]));
                maxY = Math.Max(Math.Abs(y[i] + r[i]), Math.Abs(y[i] - r[i]));
                maxZ = Math.Max(Math.Abs(z[i] + r[i]), Math.Abs(z[i] - r[i]));

                maxR = Math.Max(maxX, maxY);
                maxR = Math.Max(maxR, maxZ);

                maxRXYZ = Math.Max(maxRXYZ, maxR);
            }
            R = maxRXYZ; // sumR;
        }

        // Формирование массива радиусов для ограничений и задач оптимизаций
        // r[i] = sum[1..i]{r[k]}
        public static double[] raSumGenerate(double[] radius)
        {
            return SummGenerate(ref radius);
        }

        private static double[] SummGenerate(ref double[] radius)
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