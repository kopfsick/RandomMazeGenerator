using System;
using System.Threading.Tasks;

namespace RandomMazeGenerator.Skia.WPF
{
    public class SimpleGameLoop
    {
        private readonly Func<Task> _updateFunction;
        private int _targetFrameRate;
        private TimeSpan _targetUpdateTimeSpan;

        public SimpleGameLoop(int targetFrameRate, Func<Task> updateFunction)
        {
            _updateFunction = updateFunction;
            TargetFrameRate = targetFrameRate;
        }

        public int TargetFrameRate
        {
            get { return _targetFrameRate; }
            set
            {
                _targetFrameRate = value;
                _targetUpdateTimeSpan = TimeSpan.FromSeconds(1.0 / _targetFrameRate);
            }
        }

        public async Task Run()
        {
            while(true)
            {
                await _updateFunction();
                await Task.Delay(_targetUpdateTimeSpan);
            }
        }
    }
}
