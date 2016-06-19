using System;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using Synker;

namespace UnitTest
{
    [TestFixture]
    public class TimerElapsedEventArgsTest
    {
        [Test]
        [TestCase(0L, 0L, 0L, 0L, TestName = "frequency が 0 以下の場合")]
        [TestCase(1L, -1L, 0L, 0L, TestName = "count が負の場合")]
        [TestCase(1L, 0L, 0L, -1L, TestName = "lastEventProcessTick が負の場合")]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
        public void CtorError(long frequency, long count, long intervalGapTick, long lastEventProcessTick)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new TimerElapsedEventArgs(frequency, count, intervalGapTick, lastEventProcessTick));
        }

        [Test]
        public void IntervalGapTest()
        {
            var args = new TimerElapsedEventArgs(1, 0, 0, 0);
            Assert.AreEqual(0L, args.IntervalGapTick);
            Assert.AreEqual(0.0, args.IntervalGapTime);
        }

        [Test]
        public void LastEventProcessTest()
        {
            var args = new TimerElapsedEventArgs(1, 0, 0, 0);
            Assert.AreEqual(0L, args.LastEventProcessTick);
            Assert.AreEqual(0.0, args.LastEventProcessTime);
        }
    }
}
