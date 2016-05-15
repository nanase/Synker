using System;

namespace Synker
{
    public class IntervalTickEventArgs : EventArgs
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

        public IntervalTickEventArgs(
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
