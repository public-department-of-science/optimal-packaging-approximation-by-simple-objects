using BooleanConfiguration.Helper;
using BooleanConfiguration.Model;
using System;

namespace BooleanConfiguration.IO
{
    internal static class Input
    {
        public static void SelectTypeOfStartSet(ref Data data, string setType, Print outStream)
        {
            try
            {
                switch (setType)
                {
                    case "1":
                        data = new Data(Enums.TypeOfSet.BooleanSet, outStream);
                        break;

                    case "2":
                        data = new Data(Enums.TypeOfSet.BnSet, outStream);
                        break;

                    case "3":
                        data = new Data(Enums.TypeOfSet.SphericalLocatedBnSet, outStream);
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
