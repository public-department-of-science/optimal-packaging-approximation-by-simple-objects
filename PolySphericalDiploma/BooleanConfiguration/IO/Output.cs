using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using BooleanConfiguration.Model;

namespace BooleanConfiguration.IO
{
    internal static class Output
    {
        public static void ConsolePrint(string text)
        {
            Console.WriteLine(text);
        }
        public static void PrintToConsole(Print ConsolePrint)
        {
            ConsolePrint("Select set type:");
            ConsolePrint("1 - > Bn (0101101011...00111010101");
            ConsolePrint("2 - > Bn(m) 00000 (n-m of 0)_111111....111111... (m of 1)");
            ConsolePrint("3 - > Bn(m1, m2) 00000 (n-m1 of 0)_111111....111111... (m2 of 1)");
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
                        KeyValuePair<double[], Stopwatch> result = res.GetResultById(item);

                        sw.WriteLine($"_______Lambda = {item.ToString()}___________");
                        sw.WriteLine($"Time = {result.Value.Elapsed.ToString()}");
                        sw.WriteLine($"OPTIONAL POINT: ");

                        for (int i = 0; i < result.Key.Length; i++)
                        {
                            double item1 = result.Key[i];
                            sw.Write($" X[{i + 1}] = " + item1.ToString() + ";");
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
