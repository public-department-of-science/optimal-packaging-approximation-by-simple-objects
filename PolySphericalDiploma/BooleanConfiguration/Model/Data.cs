using static BooleanConfiguration.Enums;

namespace BooleanConfiguration
{
    /// <summary>
    /// Class with optimization parameters
    /// </summary>
    public class Data
    {
        /// <summary>
        /// 
        /// </summary>
        public double[][] MatrixA { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public double[][] RestrictionsMatrix { get; private set; }

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
        public double[] MatrixCOrRightPart { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public double[] MatrixX1 { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public TypeOfSet SetType { get; set; }

        /// <summary>
        /// Class with all restriction/values culculations
        /// </summary>
        private readonly OptimizationHelper OptimizationHelper;

        public ISet Set { get; set; }


        public double[] g_L { get; set; }

        public double[] g_U { get; set; }

        public double[] x_L { get; set; }

        public double[] x_U { get; set; }

        public int m { get; set; } // restrictions amount

        public int n { get; set; } // variables amount

        public Data(TypeOfSet typeOfSet)
        {
            const int leftBound = 3;
            const int rightBound = 10;
            OptimizationHelper = new OptimizationHelper();

            N = OptimizationHelper.GerIntegerValueInlcudingUpperBound(leftBound, rightBound);

            Set = SelectSet(typeOfSet);
            SetType = typeOfSet;

            n = N;

            // arrays allocation
            Lamda = new int[N];
            g_L = new double[N];
            g_U = new double[N];
            x_L = new double[N];
            x_U = new double[N];
            MatrixCOrRightPart = new double[N];
            MatrixX1 = new double[N];
            MatrixA = new double[N][];
            RestrictionsMatrix = new double[N][];
            AllocateArrayMemory(MatrixA, N);
            AllocateArrayMemory(RestrictionsMatrix, N);
            //

            // Setting random values
            OptimizationHelper.RandomizeMatrixA(MatrixA); // N*N
            OptimizationHelper.RandomizeMatrixA(RestrictionsMatrix); // N*N
            OptimizationHelper.RandomizeMatrixC(MatrixCOrRightPart); // N*1
            OptimizationHelper.RandomizeMatrixX1(MatrixX1); // N*1
            //

            // Restrictions
            RestrictionHelper.SetXBounds(x_L, x_U);
            m = RestrictionHelper.SetM(N);
            RestrictionHelper.SetRestrictionsBounds(g_L, g_U, RestrictionsMatrix, Set.MatrixX0, MatrixX1, N);
            //
        }

        private ISet SelectSet(TypeOfSet typeOfSet)
        {
            ISet currentSet = null;

            switch (typeOfSet)
            {
                case TypeOfSet.BooleanSet:
                    currentSet = new BooleanSet(N);
                    break;
                case TypeOfSet.BnSet:
                    currentSet = new BnSet(N);
                    break;
                case TypeOfSet.SphericalLocatedBnSet:
                    currentSet = new SphericalLocatedBnSet(N);
                    break;
                default:
                    break;
            }
            return currentSet;
        }

        private void AllocateArrayMemory(double[][] array, int n)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = new double[n];
            }
        }
    }
}
