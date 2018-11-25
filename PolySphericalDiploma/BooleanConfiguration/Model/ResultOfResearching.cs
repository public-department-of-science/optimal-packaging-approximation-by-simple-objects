using System.Collections.Generic;
using System.Diagnostics;

namespace BooleanConfiguration
{
    public class ResultOfResearching
    {
        /// <summary>
        /// Key => labda;
        /// Value => optional point with time
        /// </summary>
        private Dictionary<double, KeyValuePair<double[], Stopwatch>> Result { get; set; }

        public ResultOfResearching()
        {
            Result = new Dictionary<double, KeyValuePair<double[], Stopwatch>>();
        }

        public void AddNewResult(double lambda, KeyValuePair<double[], Stopwatch> keyValues)
        {
            if (Result.ContainsKey(lambda))
            {
                do
                {
                    lambda += 0.01;
                }
                while (Result.ContainsKey(lambda));
            }

            Result.Add(lambda, keyValues);
        }

        public void ShowAllResults()
        {
            int i = 0;
            foreach (KeyValuePair<double, KeyValuePair<double[], Stopwatch>> item in Result)
            {
                Output.ConsolePrint($"Lambda = {item.Key}, Array {item.Value.Key}, Time = {item.Value.Value}");
                ++i;
            }
        }

        public KeyValuePair<double[], Stopwatch> GetResultById(double lambda)
        {
            if (Result.ContainsKey(lambda))
            {
                return Result[lambda];
            }
            else
            {
                return new KeyValuePair<double[], Stopwatch>();
            }
        }
    }
}
