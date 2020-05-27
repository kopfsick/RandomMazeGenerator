using System.Threading.Tasks;

namespace RandomMazeGenerator.Core
{
    public interface IStepableAlgorithm
    {
        Task Run(int updateDelayMillis, int stepsPerUpdate);
        void Step(int steps = 1);
        bool IsFinished { get; }
    }
}