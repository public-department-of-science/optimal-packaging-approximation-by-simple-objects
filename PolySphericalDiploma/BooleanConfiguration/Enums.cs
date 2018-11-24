namespace BooleanConfiguration
{
    public class Enums
    {
        public enum TypeOfSet
        {
            BooleanSet = 0, // 01_00_11_11_10_00_00..... Bn
            BnSet = 1, // 00000 (n-m of 0)_111111....111111... (m of 1) Bn(m)
            SphericalLocatedBnSet = 2 // 00000 (n-m1 of 0)_111111....111111... (m2 of 1) Bn(m1, m2)
        }
    }
}
