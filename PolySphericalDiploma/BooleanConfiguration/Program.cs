using System;

namespace BooleanConfiguration
{
    internal delegate void Print(string text);

    internal class Program
    {
        private static void Main()
        {
            Print ConxolePrint = Output.ConsolePrint;

            ConxolePrint("Select set type:");
            ConxolePrint("1 - > Bn (0101101011...00111010101");
            ConxolePrint("2 - > Bn(m) 00000 (n-m of 0)_111111....111111... (m of 1)");
            ConxolePrint("3 - > Bn(m1, m2) 00000 (n-m1 of 0)_111111....111111... (m2 of 1)");

            Data data = null;
            try
            {


                switch (Console.ReadLine())
                {
                    case "1":
                        data = new Data(Enums.TypeOfSet.BnSet);
                        break;

                    case "2":
                        data = new Data(Enums.TypeOfSet.BooleanSet);
                        break;

                    case "3":
                        data = new Data(Enums.TypeOfSet.SphericalLocatedBnSet);
                        break;

                    default:
                        return;
                }

            }
            catch (Exception ex)
            {
            }
            Console.ReadLine();
        }
    }
}
