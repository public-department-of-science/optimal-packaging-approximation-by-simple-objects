using static BooleanConfiguration.Enums;

namespace BooleanConfiguration
{
    /// <summary>
    /// Class with optimization parameters
    /// </summary>
    internal class Data
    {
        /// <summary>
        /// 
        /// </summary>
        public double[][] MatrixA { get; private set; }

        /// <summary>
        /// Dimension of space
        /// </summary>
        public int N { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int[] Lamda { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public double[][] MatrixC { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public TypeOfSet SetType { get; set; }

        /// <summary>
        /// Sum of 1 When TypeOfSet = 1
        /// </summary>
        public int M { get; private set; }

        /// <summary>
        /// Sum left bound of Bn => When TypeOfSet = 2
        /// </summary>
        public int M1 { get; private set; }

        /// <summary>
        /// Sum right bound of Bn => When TypeOfSet = 2
        /// </summary>
        public int M2 { get; private set; }

        /// <summary>
        /// Class with all restriction/values culculations
        /// </summary>
        private readonly OptimizationHelper OptimizationHelper;

        public Data(TypeOfSet typeOfSet)
        {
            SetType = typeOfSet;

            const int leftBound = 3;
            const int rightBound = 10;
            OptimizationHelper = new OptimizationHelper();
            N = OptimizationHelper.GerIntegerValueInlcudingUpperBound(leftBound, rightBound);

            Lamda = new int[N];

            AllocateArrayMemory(MatrixA, N);
            AllocateArrayMemory(MatrixC, N);

            OptimizationHelper.RandomizeMatrixA(MatrixA);
            OptimizationHelper.RandomizeMatrixC(MatrixC);
        }

        private void AllocateArrayMemory(double[][] array, int n)
        {
            array = new double[N][];

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = new double[n];
            }
        }
    }
}
