
using System;
using BooleanConfiguration.Helper;
using BooleanConfiguration.Interfaces;
using static BooleanConfiguration.Helper.Enums;

namespace BooleanConfiguration.Model
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
        public double[][] ConstraintsMatrix { get; private set; }

        /// <summary>
        /// Dimension of space
        /// </summary>
        public int N { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int[] Lambda { get; private set; }

        public double MainLambda { get; set; }

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
            OptimizationHelper = new OptimizationHelper();

            Console.Write("N = ");
            N = int.Parse(Console.ReadLine()); // OptimizationHelper.GerIntegerValueInlcudingUpperBound(leftBound, rightBound);

            Console.Write("Set data range:");
            Console.Write("Left bound = ");
            int leftBound = int.Parse(Console.ReadLine());
            Console.Write("Right bound = ");
            int rightBound = int.Parse(Console.ReadLine());

            Set = SelectSet(typeOfSet);
            SetType = typeOfSet;

            n = N;

            // arrays allocation
            Lambda = new int[N];
            g_L = new double[N];
            g_U = new double[N];
            x_L = new double[N];
            x_U = new double[N];
            MatrixCOrRightPart = new double[N];
            MatrixX1 = new double[N];
            MatrixA = new double[N][];
            ConstraintsMatrix = new double[N][];
            AllocateArrayMemory(MatrixA, N);
            AllocateArrayMemory(ConstraintsMatrix, N);
            //

            // Setting random values
            OptimizationHelper.RandomizeMatrixA(MatrixA); // N*N
            OptimizationHelper.RandomizeMatrixA(ConstraintsMatrix); // N*N
            OptimizationHelper.RandomizeMatrixC(MatrixCOrRightPart); // N*1
            OptimizationHelper.RandomizeMatrixX1(MatrixX1); // N*1
            //

            // Restrictions
            RestrictionHelper.SetXBounds(x_L, x_U);
            m = RestrictionHelper.SetM(N);
            RestrictionHelper.SetRestrictionsBounds(g_L, g_U, ConstraintsMatrix, Set.MatrixX0, MatrixX1, N);
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
