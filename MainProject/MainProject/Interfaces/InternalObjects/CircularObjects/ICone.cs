using TestProblemIpOpt.Model;

namespace PackegeProject.Interfaces
{
    internal interface ICone : IInternalCircularObject, IInternalObject
    {
        double Height { get; set; }

        Point LowerBaseCenter { get; set; }
    }
}
