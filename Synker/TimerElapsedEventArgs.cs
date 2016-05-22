﻿/* LICENSE

The MIT License (MIT)

Copyright (c) 2016 Tomona Nanase

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

*/

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
