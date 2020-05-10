using System.Threading.Tasks;

namespace RandomMazeGenerator.WPF
{
    public interface IStepableAlgorithm
    {
        Task Run(int updateDelayMillis, int stepsPerUpdate);
        void Step(int steps = 1);
    }
}