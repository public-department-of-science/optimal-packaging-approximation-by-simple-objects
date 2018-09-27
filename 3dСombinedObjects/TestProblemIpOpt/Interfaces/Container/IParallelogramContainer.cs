using TestProblemIpOpt.Model;

namespace TestProblemIpOpt.Interfaces
{
    public interface IParallelogramContainer : IContainer
    {
        Point[] ParallelogramPoints { get; set; }
    }
}
