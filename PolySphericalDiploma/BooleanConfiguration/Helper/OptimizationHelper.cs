using System;
using System.Linq;
using BooleanConfiguration.Model;

namespace BooleanConfiguration.Helper
{
    internal class OptimizationHelper
    {
        private static readonly Random random;

        static OptimizationHelper()
        {
            random = new Random();
        }

        public void RandomizeMatrix(double[][] maxtrix)
        {
            for (int i = 0; i < maxtrix.Length; i++)
            {
                double[] row = maxtrix[i];

                for (int j = 0; j < row.Length; j++)
                {
                    maxtrix[i][j] = random.Next(0, 10);
                }
            }
        }

        public static void GettingArrayWithLabda(double[][] matrixA, ref double[] labdaArray, out double mainLambda)
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

        public void RandomizeMatrixC(double[] maxtrixC)
        {
            for (int i = 0; i < maxtrixC.Length; i++)
            {
                maxtrixC[i] = random.Next(-10, 10);
            }
        }

        public static double[] GettingVariablesVector(Data data)
        {
            double[] randomStartValues = new double[data.N];
            for (int i = 0; i < randomStartValues.Length; i++)
            {
                randomStartValues[i] = random.NextDouble();
            }

            return randomStartValues;
        }

        public void RandomizeMatrixA(double[][] maxtrixA)
        {
            RandomizeMatrix(maxtrixA);
        }

        public void RandomizeMatrixX1(double[] matrixX1)
        {
            for (int j = 0; j < matrixX1.Length; j++)
            {
                matrixX1[j] = random.NextDouble();
            }
        }
    }
}
