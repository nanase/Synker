using System;

namespace Synker
{
    public class TimerElapsedEventArgs : EventArgs
    {
        #region -- Public Properties --

        public long Frequency { get; }

        public long Count { get; }

        public long IntervalGapTick { get; }

        public long LastEventProcessTick { get; }

        public double AccualIntervalTime => (double)IntervalGapTick / Frequency;

        public double LastEventProcessTime => (double)LastEventProcessTick / Frequency;

        #endregion

        #region -- Constructors --

        public TimerElapsedEventArgs(
            long frequency, long count, long intervalGapTick, long lastEventProcessTick)
        {
            Frequency = frequency;
            Count = count;
            IntervalGapTick = intervalGapTick;
            LastEventProcessTick = lastEventProcessTick;
        }

        #endregion
    }
}
