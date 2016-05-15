using System;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace Synker
{
    public class Interval
    {
        #region -- Private Fields --

        private volatile bool requestedStop;
        private Task tickerTask;
        private int intervalMilliseconds = 10;
        #endregion

        #region -- Public Properties --

        public int IntervalMilliseconds
        {
            get { return intervalMilliseconds; }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException(nameof(value));

                intervalMilliseconds = value;
            }
        }

        public bool Running { get; private set; }

        public long TickCount { get; private set; }

        #endregion

        #region -- Public Events --

        public event EventHandler<IntervalTickEventArgs> Elapsed;

        #endregion

        #region -- Constructors --

        public Interval()
        {
            Reset();
        }

        #endregion

        #region -- Public Methods --

        public void Start()
        {
            if (Running)
                return;

            requestedStop = false;
            tickerTask = Task.Factory.StartNew(Tick);
            Running = true;
        }

        public void Stop()
        {
            Stop(TimeSpan.FromMilliseconds(-1));
        }

        public void Stop(TimeSpan timeout)
        {
            if (!Running)
                return;

            requestedStop = true;
            tickerTask.Wait(timeout);
            tickerTask.Dispose();
            tickerTask = null;
            Running = false;
        }

        public void Reset()
        {
            TickCount = 0L;
            requestedStop = false;
        }

        public void Restart()
        {
            Restart(TimeSpan.FromMilliseconds(-1));
        }

        public void Restart(TimeSpan timeout)
        {
            Stop(timeout);
            Start();
        }

        #endregion

        #region -- Private Methods --

        private void Tick()
        {
            var processTime = 0L;
            var oldTick = Stopwatch.GetTimestamp();
            var frequency = Stopwatch.Frequency;
            var milliFrequency = (long)(frequency * 0.001);
            var gap = 0.0;
            var nowTick = oldTick;

            while (!requestedStop)
            {
                TickCount++;
                
                var deltaTickDouble = intervalMilliseconds * 0.001 * frequency;
                var deltaTick = (long)deltaTickDouble;
                gap += deltaTickDouble - deltaTick;
                var gapDelta = (long) gap;
                gap -= gapDelta;

                var targetTick = oldTick + deltaTick + gapDelta;

                var args = new IntervalTickEventArgs(frequency, TickCount, oldTick - nowTick, processTime);
                var beforeProcessTick = Stopwatch.GetTimestamp();

                Elapsed?.Invoke(this, args);

                nowTick = Stopwatch.GetTimestamp();
                processTime = nowTick - beforeProcessTick;
                
                if (requestedStop)
                    break;
                
                while (!requestedStop && Stopwatch.GetTimestamp() + milliFrequency < targetTick)
                    Thread.Sleep(1);
                
                oldTick = targetTick;
            }
        }

        #endregion
    }
}
