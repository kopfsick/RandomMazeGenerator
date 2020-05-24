using System;
using System.Threading.Tasks;

namespace RandomMazeGenerator.Skia.WPF
{
    public class SimpleGameLoop
    {
        private readonly Func<Task> _updateFunction;
        private int _targetFrameRate;
        private TimeSpan _targetUpdateTimeSpan;
        private int _fpsCounterFramesRendered;
        private DateTime _lastFpsUpdate;
        private DateTime _lastUpdate;

        public SimpleGameLoop(int targetFrameRate, Func<Task> updateFunction)
        {
            _updateFunction = updateFunction;
            TargetFrameRate = targetFrameRate;
        }

        public double CurrentAverageFPS { get; private set; }

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
            _lastFpsUpdate = DateTime.Now;
            _lastUpdate = DateTime.Now;

            while(true)
            {
                _lastUpdate = DateTime.Now;
                await _updateFunction();

                _fpsCounterFramesRendered++;

                double secondsSinceLastFPSUpdate = (DateTime.Now - _lastFpsUpdate).TotalSeconds;
                if(secondsSinceLastFPSUpdate >= 1)
                {
                    CurrentAverageFPS = _fpsCounterFramesRendered/secondsSinceLastFPSUpdate;
                    _fpsCounterFramesRendered = 0;
                    _lastFpsUpdate = DateTime.Now;
                }


                var alreadyElapsed = DateTime.Now - _lastUpdate;
                var shouldWait = _targetUpdateTimeSpan - alreadyElapsed;
                await Task.Delay((int)Math.Max(shouldWait.TotalMilliseconds, 0));
            }
        }
    }
}
