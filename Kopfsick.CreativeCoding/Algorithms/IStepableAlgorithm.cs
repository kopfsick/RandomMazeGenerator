using System.Threading.Tasks;

namespace Kopfsick.CreativeCoding.Algorithms
{
    public interface IStepableAlgorithm
    {
        Task Run(int updateDelayMillis, int stepsPerUpdate);
        void Step(int steps = 1);
        bool IsFinished { get; }
    }
}