using TestProblemIpOpt.Model;

namespace PackegeProject.Interfaces
{
    internal interface ICylinder : IObjectHasCircleInStructure, IInternalObject
    {
        Point LowerBaseCenter { get; set; }

        Point UpperBaseCenter { get; set; }

        double Height { get; set; }
    }
}
