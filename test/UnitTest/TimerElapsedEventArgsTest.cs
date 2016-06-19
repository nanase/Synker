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
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
        public void CtorError()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new TimerElapsedEventArgs(0, 0, 0, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TimerElapsedEventArgs(1, -1, 0, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TimerElapsedEventArgs(1, 0, 0, -1));
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
