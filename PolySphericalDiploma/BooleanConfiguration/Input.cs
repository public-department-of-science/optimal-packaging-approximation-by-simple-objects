using System;

namespace BooleanConfiguration
{
    internal static class Input
    {
        public static void SelectTypeOfStartSet(Data data)
        {
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
                Output.ConsolePrint(ex.Message);
            }
        }
    }
}
