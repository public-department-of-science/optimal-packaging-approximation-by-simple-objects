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
        /// Class with all restriction/values culculations
        /// </summary>
        private readonly OptimizationHelper OptimizationHelper;

        private ISet Set { get; set; }

        public Data(TypeOfSet typeOfSet)
        {
            Set = SelectSet(typeOfSet);
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

        private ISet SelectSet(TypeOfSet typeOfSet)
        {
            ISet currentSet = null;

            switch (typeOfSet)
            {
                case TypeOfSet.BooleanSet:
                    currentSet = new BooleanSet();
                    break;
                case TypeOfSet.BnSet:
                    currentSet = new BnSet();
                    break;
                case TypeOfSet.SphericalLocatedBnSet:
                    currentSet = new SphericalLocatedBnSet();
                    break;
                default:
                    break;
            }
            return currentSet;
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
