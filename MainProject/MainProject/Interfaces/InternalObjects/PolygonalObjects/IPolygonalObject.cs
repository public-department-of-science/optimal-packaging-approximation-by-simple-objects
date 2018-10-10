using TestProblemIpOpt.Model;

namespace PackageProject.Interfaces.InternalObjects.PolygonalObjects
{
    internal interface IPolygonalObject
    {
        /// <summary>
        /// array with points polygonal object 
        /// </summary>
        Point[] Points { get; set; }
    }
}
