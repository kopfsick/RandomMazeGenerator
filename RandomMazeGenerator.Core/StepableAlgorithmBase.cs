using System.Threading.Tasks;

namespace RandomMazeGenerator.WPF
{
    public abstract class StepableAlgorithmBase : IStepableAlgorithm
    {
        public bool IsFinished { get; private set; }

        public async Task Run(int updateDelayMillis, int stepsPerUpdate)
        {
            while(!IsFinished)
            {
                Step(stepsPerUpdate);
                await Task.Delay(updateDelayMillis);
            }
        }

        public void Step(int steps = 1)
        {

            for(int i = 0;i < steps;i++)
                if(!IsFinished)
                    Step();
        }

        protected void Finish() => IsFinished = true;

        protected abstract void Step();
    }
}
