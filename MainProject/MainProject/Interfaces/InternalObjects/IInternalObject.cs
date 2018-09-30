using static TestProblemIpOpt.Enums;

namespace PackegeProject.Interfaces
{
    public interface IInternalObject
    {
        /// <summary>
        /// Amount of variables for one internal object (for fix radius sphere it's 3)
        /// </summary>
        int NumberOfVariables { get; set; }

        /// <summary>
        /// value of Object Weight 
        /// </summary>
        double Weight { get; set; }

        /// <summary>
        /// Type of object selected according to ObjectType enum
        /// </summary>
        ObjectType ObjectType { get; set; }
    }
}
