using System;

namespace BooleanConfiguration
{
    public delegate void Print(string text);

    public class Program
    {
        private static void Main()
        {
            Print ConsolePrint = Output.ConsolePrint;

            ConsolePrint("Select set type:");
            ConsolePrint("1 - > Bn (0101101011...00111010101");
            ConsolePrint("2 - > Bn(m) 00000 (n-m of 0)_111111....111111... (m of 1)");
            ConsolePrint("3 - > Bn(m1, m2) 00000 (n-m1 of 0)_111111....111111... (m2 of 1)");

            Data data = null;
            Input.SelectTypeOfStartSet(data);
            ResultOfResearching res = new RunTask().SolveTheProblem(data);

            Console.ReadLine();
        }
    }
}
