using System.Collections.Generic;
using System.Diagnostics;

namespace BooleanConfiguration
{
    internal class ResultOfResearching
    {
        /// <summary>
        /// Key => labda;
        /// Value => optional point
        /// </summary>
        private Dictionary<double, double[]> Result { get; set; }

        /// <summary>
        /// Time measurment per iteration
        /// </summary>
        private List<Stopwatch> TaskTimeList { get; set; }

        public ResultOfResearching()
        {
            Result = new Dictionary<double, double[]>();
            TaskTimeList = new List<Stopwatch>();
        }

        public void AddNewResult(KeyValuePair<double, double[]> keyValues, Stopwatch time)
        {
            Result.Add(keyValues.Key, keyValues.Value);
            TaskTimeList.Add(time);
        }

        public void ShowAllResults()
        {
            int i = 0;
            foreach (KeyValuePair<double, double[]> item in Result)
            {
                Output.ConsolePrint($"Lambda = {item.Key}, Array {item.Value}, Time = {TaskTimeList[i]}");
                ++i;
            }
        }

        public (KeyValuePair<double, double[]> keyValuePair, Stopwatch spentTime) GetResultById(double lambda, int i)
        {
            bool res = Result.TryGetValue(lambda, out double[] array);
            if (res)
            {
                return (keyValuePair: new KeyValuePair<double, double[]>(lambda, array), spentTime: TaskTimeList[i]);
            }
            else
            {
                return (keyValuePair: new KeyValuePair<double, double[]>(), spentTime: new Stopwatch());
            }
        }
    }
}
