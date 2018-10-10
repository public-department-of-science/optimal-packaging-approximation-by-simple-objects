using TestProblemIpOpt.Model;

namespace PackageProject.Interfaces
{
    internal interface ICylinder : IObjectHasCircleInStructure, IInternalObject
    {
        Point LowerBaseCenter { get; set; }

        Point UpperBaseCenter { get; set; }

        double Height { get; set; }
    }
}
