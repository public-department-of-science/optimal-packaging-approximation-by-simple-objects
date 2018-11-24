using System;

namespace BooleanConfiguration
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
        }
    }
}
