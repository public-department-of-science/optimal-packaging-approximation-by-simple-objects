using TestProblemIpOpt.Model;

namespace TestProblemIpOpt.Interfaces
{
    public interface IСircularContainer : IContainer
    {
        double Radius { get; }

        Point CenterOfTheContainer { get; }
    }
}
