using System;
using BooleanConfiguration.Helper;
using BooleanConfiguration.Model;

namespace BooleanConfiguration.IO
{
    internal static class Input
    {
        public static void SelectTypeOfStartSet(ref Data data)
        {
            try
            {
                switch (Console.ReadLine())
                {
                    case "1":
                        data = new Data(Enums.TypeOfSet.BooleanSet);
                        break;

                    case "2":
                        data = new Data(Enums.TypeOfSet.BnSet);
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
                Output.ConsolePrint(ex.Message);
            }
        }
    }
}
