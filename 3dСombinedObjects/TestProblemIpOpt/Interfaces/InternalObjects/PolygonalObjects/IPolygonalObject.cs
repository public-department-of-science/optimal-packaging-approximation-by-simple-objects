using TestProblemIpOpt.Model;

namespace PackegeProject.Interfaces.InternalObjects.PolygonalObjects
{
    internal interface IPolygonalObject
    {
        /// <summary>
        /// array with points polygonal object 
        /// </summary>
        Point[] Points { get; set; }
    }
}
