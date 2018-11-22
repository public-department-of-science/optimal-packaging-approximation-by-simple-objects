using System;
using System.Linq;

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

        internal static void GettingArrayWithLabda(double[][] matrixA, out double[] labdaArray, out double mainLambda)
        {
            labdaArray = new double[matrixA.Length];
            for (int i = 0; i < matrixA.Length; i++)
            {
                // lambdaInThisLine = Sum of line - a[i][j]
                labdaArray[i] = matrixA[i].Sum(x => x) - matrixA[i][i];
            }

            mainLambda = labdaArray.Max();
            labdaArray = labdaArray.OrderBy(x => x).ToArray();
        }

        public int GerIntegerValueInlcudingUpperBound(int v1, int v2)
        {
            return random.Next(v1, v2 + 1);
        }

        public void RandomizeMatrixC(double[][] maxtrixC)
        {
            RandomizeMatrix(maxtrixC);
        }

        internal static double[] GettingVariablesVector(Data data)
        {
        }

        public void RandomizeMatrixA(double[][] maxtrixA)
        {
            RandomizeMatrix(maxtrixA);
        }
    }
}
