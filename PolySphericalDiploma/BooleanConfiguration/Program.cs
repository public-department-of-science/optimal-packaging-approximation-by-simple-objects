using System;

namespace BooleanConfiguration
{
    public delegate void Print(string text);

    public class Program
    {
        private static void Main()
        {
            Print ConsolePrint = Output.ConsolePrint;
            Data data = null;

            Output.PrintToConsole(ConsolePrint);

            Input.SelectTypeOfStartSet(ref data);

            ResultOfResearching res = new RunTask().SolveTheProblem(data ?? throw new Exception("Data is null!"));

            Console.ReadLine();
        }
    }
}
