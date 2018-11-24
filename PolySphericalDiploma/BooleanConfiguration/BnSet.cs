namespace BooleanConfiguration
{
    internal class BnSet : IBnSet
    {
        public int M1 { get; set; }
        public int M2 { get; set; }
        public string MatrixX0 { get; set; }

        public BnSet(int N)
        {
            MatrixX0 = "0000000001111111111111111111111111111111111111111".Substring(0, N);
            M1 = MatrixX0.Contains("1").ToString().Length;
            M2 = 5;
        }
    }
}
