using TestProblemIpOpt.Model;

namespace PackegeProject.Interfaces
{
    internal interface ICone : IObjectHasCircleInStructure, IInternalObject
    {
        double Height { get; set; }

        Point LowerBaseCenter { get; set; }
    }
}
