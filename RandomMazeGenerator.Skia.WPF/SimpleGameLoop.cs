using System;
using System.Diagnostics;
using System.Threading;
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

        public Task Run()
        {
            return Task.Run(async () =>
            {
                _lastFpsUpdate = DateTime.Now;
                var framewatch = new Stopwatch();

                while(true)
                {
                    framewatch.Restart();
                    await _updateFunction();

                    var alreadyElapsed = framewatch.Elapsed;
                    var shouldWait = _targetUpdateTimeSpan - alreadyElapsed;

                    //while(framewatch.Elapsed < _targetUpdateTimeSpan)
                    //{
                    //    Thread.Yield();
                    //}
                    if(shouldWait > TimeSpan.Zero)
                        await Task.Delay(shouldWait);

                    _fpsCounterFramesRendered++;

                    double secondsSinceLastFPSUpdate = (DateTime.Now - _lastFpsUpdate).TotalSeconds;
                    if(secondsSinceLastFPSUpdate >= 1)
                    {
                        CurrentAverageFPS = _fpsCounterFramesRendered/secondsSinceLastFPSUpdate;
                        _fpsCounterFramesRendered = 0;
                        _lastFpsUpdate = DateTime.Now;
                    }
                }

            });
        }
    }
}
