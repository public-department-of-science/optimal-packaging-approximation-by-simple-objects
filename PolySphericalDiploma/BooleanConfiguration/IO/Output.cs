using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using BooleanConfiguration.Model;

namespace BooleanConfiguration.IO
{
    internal static class Output
    {
        public static void ConsolePrint(string text, bool needWriteLine = false)
        {
            Console.Write(text);

            if (needWriteLine)
            {
                ConsolePrintWriteLine();
            }
        }

        public static void ConsolePrintWriteLine()
        {
            Console.WriteLine();
        }

        public static void PrintToConsole(Print ConsolePrint)
        {
            ConsolePrint("Select set type:", needPrintNewLine: true);
            ConsolePrint("1 - > Bn (0101101011...00111010101", needPrintNewLine: true);
            ConsolePrint("2 - > Bn(m) 00000 (n-m of 0)_111111....111111... (m of 1)", needPrintNewLine: true);
            ConsolePrint("3 - > Bn(m1, m2) 00000 (n-m1 of 0)_111111....111111... (m2 of 1)", needPrintNewLine: true);
            Console.Write("Choose type of set: ");
        }

        public static void SaveToFile(ResultOfResearching res, double[] lambda)
        {
            string writePath = AppDomain.CurrentDomain.BaseDirectory + res.ToString() + ".txt";

            try
            {
                using (StreamWriter sw = new StreamWriter(writePath, false, System.Text.Encoding.Default))
                {
                    sw.WriteLine($"<<Table with results below>> N = {lambda.Length}");

                    foreach (double item in lambda)
                    {
                        var result = res.GetResultById(item);
                        var taskStatus = res.GetTaskStatusById(item);

                        sw.WriteLine($"Status = {taskStatus.ToString()} ");
                        sw.WriteLine($"_______Lambda = {item.ToString()}___________");
                        sw.WriteLine($"FunctionValue = {result.Value.ToString("0.00")} Time = {result.Key.Value.Elapsed.ToString()}");
                        sw.WriteLine($"Local optional point: ");

                        for (int i = 0; i < result.Key.Key.Length; i++)
                        {
                            double item1 = result.Key.Key[i];
                            //sw.Write($" X[{i + 1}] = " + Math.Round(item1).ToString() + ";");
                            sw.Write($" X[{i + 1}] = " + item1.ToString("0.00") + ";");
                        }

                        sw.WriteLine();
                        sw.WriteLine();
                    }

                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                ConsolePrint(ex.Message);
            }
        }
    }
}
