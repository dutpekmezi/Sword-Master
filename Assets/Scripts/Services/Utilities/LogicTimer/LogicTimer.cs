using System;
using System.Diagnostics;
using UnityEngine;

namespace Utils.LogicTimer
{
    public class LogicTimer : IDisposable
    {
        public const float FramesPerSecond = 60f;
        public const float FixedDelta = 1f / FramesPerSecond;

        private double _totalTime;
        private double _accumulator;
        private long _lastTime;
        private readonly Stopwatch _stopwatch;
        private readonly Action _onTick;

        public double TotalTime => _totalTime;
        public bool IsPaused { get; private set; }

        public float LerpAlpha => (float)(_accumulator / FixedDelta);

        public float DeltaTime { get; private set; }

        public static LogicTimer Instance { get; private set; }

        public LogicTimer(Action onTick)
        {
            if (Instance != null)
                throw new Exception("LogicTimer already has an instance");

            Instance = this;

            _onTick = onTick ?? throw new ArgumentNullException(nameof(onTick));
            _stopwatch = new Stopwatch();
        }

        public void Dispose()
        {
            Stop();
            Instance = null;
        }

        public void Start()
        {
            _accumulator = 0.0;
            _lastTime = 0;
            _totalTime = 0.0;
            _stopwatch.Restart();
            IsPaused = false;
        }

        public void Pause()
        {
            if (IsPaused) return;

            IsPaused = true;
            _stopwatch.Stop();
        }

        public void Resume()
        {
            if (!IsPaused) return;

            IsPaused = false;
            _stopwatch.Start();

            _lastTime = _stopwatch.ElapsedTicks;
        }

        public void Stop()
        {
            _stopwatch.Stop();
            _accumulator = 0;
            _lastTime = 0;
        }

        public void Update()
        {
            if (IsPaused || !_stopwatch.IsRunning)
                return;

            long elapsedTicks = _stopwatch.ElapsedTicks;

            double passedSeconds = (double)(elapsedTicks - _lastTime) / Stopwatch.Frequency;

            DeltaTime = (float)passedSeconds;

            _accumulator += passedSeconds;
            _totalTime += passedSeconds;

            _lastTime = elapsedTicks;

            while (_accumulator >= FixedDelta)
            {
                _onTick();
                _accumulator -= FixedDelta;
            }
        }
    }
}
