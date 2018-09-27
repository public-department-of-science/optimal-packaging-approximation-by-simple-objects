namespace TestProblemIpOpt
{
    public class Enums
    {
        public enum CircularObjectType
        {
            Undefined = 0, // type not undefined
            ProhibitionZone = 1, // prohibition zone which not moving
            Sphere = 2, // usual ball
            CompositeObject = 3, //a few ball united in one composite object
            Cone = 4,
            Cylender = 5
        }

        public enum PoligonalObject
        {
            Undefined = 0, // type not undefined
            Cube = 1,
            Parallepiped = 2,
            Prism = 3
        }

        public enum TaskClassification
        {
            FixedRadiusTask = 0, // task type
            VariableRadius = 1 // task type
        }

        #region 

        public enum SelectBallsWithVariableRadiusScheme
        {
            Random = 0,
            EvenBalls = 1,
            OddBalls = 2
        }

        #endregion
    }
}
