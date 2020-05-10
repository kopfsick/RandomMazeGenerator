using System.Threading.Tasks;

namespace RandomMazeGenerator.WPF
{
    public abstract class StepableAlgorithmBase : IStepableAlgorithm
    {
        private bool _finished;

        public async Task Run(int updateDelayMillis, int stepsPerUpdate)
        {
            while(!_finished)
            {
                Step(stepsPerUpdate);
                await Task.Delay(updateDelayMillis);
            }
        }

        public void Step(int steps = 1)
        {
            for(int i = 0;i < steps;i++)
                Step();
        }

        protected void Finish() => _finished = true;

        protected abstract void Step();
    }
}
