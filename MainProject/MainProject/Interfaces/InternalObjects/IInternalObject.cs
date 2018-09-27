namespace PackegeProject.Interfaces
{
    internal interface IInternalObject
    {
        /// <summary>
        /// Amount of variables for one internal object (for fix radius sphere it's 3)
        /// </summary>
        int NumberOfVariables { get; set; }
    }
}
