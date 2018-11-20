using System;

namespace BooleanConfiguration
{
    internal class OptimizationHelper
    {
        private readonly Random random;

        public OptimizationHelper()
        {
            random = new Random();
        }

        private void RandomizeMatrix(double[][] maxtrix)
        {
            for (int i = 0; i < maxtrix.Length; i++)
            {
                double[] row = maxtrix[i];

                for (int j = 0; j < row.Length; j++)
                {
                    maxtrix[i][j] = random.Next(0, 26);
                }
            }
        }

        public int GerIntegerValueInlcudingUpperBound(int v1, int v2)
        {
            return random.Next(v1, v2 + 1);
        }

        public void RandomizeMatrixC(double[][] maxtrixC)
        {
            RandomizeMatrix(maxtrixC);
        }

        public void RandomizeMatrixA(double[][] maxtrixA)
        {
            RandomizeMatrix(maxtrixA);
        }
    }
}
