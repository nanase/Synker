using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Synker;

namespace UnitTest
{
    [TestClass]
    public class TimerElapsedEventArgsTest
    {
        [TestMethod]
        public void IntervalGapTest()
        {
            var args = new TimerElapsedEventArgs(1, 0, 0, 0);
            Assert.AreEqual(0L, args.IntervalGapTick);
            Assert.AreEqual(0.0, args.IntervalGapTime);
        }

        [TestMethod]
        public void LastEventProcessTest()
        {
            var args = new TimerElapsedEventArgs(1, 0, 0, 0);
            Assert.AreEqual(0L, args.LastEventProcessTick);
            Assert.AreEqual(0.0, args.LastEventProcessTime);
        }
    }
}
