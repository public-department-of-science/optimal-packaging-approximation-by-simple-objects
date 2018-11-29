using System.Collections.Generic;
using System.Diagnostics;
using BooleanConfiguration.IO;

namespace BooleanConfiguration.Model
{
    public class ResultOfResearching
    {
        /// <summary>
        /// Key => labda;
        /// Value => optional point with time
        /// </summary>
        private Dictionary<double, KeyValuePair<KeyValuePair<double[], Stopwatch>, double>> Result { get; set; }


        public ResultOfResearching()
        {
            Result = new Dictionary<double, KeyValuePair<KeyValuePair<double[], Stopwatch>, double>>();
        }

        public void AddNewResult(double lambda, KeyValuePair<double[], Stopwatch> keyValues, double functionValue)
        {
            if (Result.ContainsKey(lambda))
            {
                do
                {
                    lambda += 0.01;
                }
                while (Result.ContainsKey(lambda));
            }
            double[] temp = new double[keyValues.Key.Length];
            for (int i = 0; i < temp.Length; i++)
            {
                temp[i] = keyValues.Key[i];
            }

            Result.Add(lambda, new KeyValuePair<KeyValuePair<double[], Stopwatch>, double>(keyValues, functionValue));
        }

        public void ShowAllResults()
        {
            int i = 0;
            foreach (KeyValuePair<double, KeyValuePair<KeyValuePair<double[], Stopwatch>, double>> item in Result)
            {
                Output.ConsolePrint($"Lambda = {item.Key}, FunctValue = {item.Value.Value}, Array {item.Value.Key}, Time = {item.Value.Value}");
                ++i;
            }
        }

        public KeyValuePair<KeyValuePair<double[], Stopwatch>, double> GetResultById(double lambda)
        {
            if (Result.ContainsKey(lambda))
            {
                return Result[lambda];
            }
            else
            {
                return new KeyValuePair<KeyValuePair<double[], Stopwatch>, double>();
            }
        }
    }
}
