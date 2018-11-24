using System;

namespace BooleanConfiguration
{
    public class BooleanSet : IBooleanSet
    {
        public int M { get; set; }
        public string MatrixX0 { get; set; }

        public BooleanSet(int N)
        {
            MatrixX0 = Convert.ToString(new Random().Next(1, 50), 2).Substring(0, N);
            M = MatrixX0.Length;
        }
    }
}
