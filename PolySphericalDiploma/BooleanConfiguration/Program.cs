using System;
using BooleanConfiguration.IO;
using BooleanConfiguration.Model;
using BooleanConfiguration.Solvers;

namespace BooleanConfiguration
{
    public delegate void Print(string text);

    public class Program
    {
        private static void Main()
        {
            Print ConsolePrint = Output.ConsolePrint;
            Data data = null;
            ResultOfResearching res = null;

            Output.PrintToConsole(ConsolePrint);
            Input.SelectTypeOfStartSet(ref data);

            try
            {
                res = new RunTask().SolveTheProblem(data ?? throw new Exception("Data is null!"));
            }
            catch (Exception ex)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                ConsolePrint($"Problem can't be solved! {ex.Message} Check previous message.");
                Console.BackgroundColor = ConsoleColor.White;
            }

            Output.SaveToFile(res ?? new ResultOfResearching(), RunTask.LamdaArray);
            Console.ReadLine();
        }
    }
}
