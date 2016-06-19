using System;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using Synker;

namespace UnitTest
{
    [TestFixture]
    public class TimedOutEventArgsTest
    {
        [Test]
        [TestCase(0L, 42L, TestName = "frequency が 0 以下の場合")]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
        public void CtorError(long frequency, long intervalGapTick)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new TimedOutEventArgs(frequency, intervalGapTick));
        }

        [Test]
        public void IntervalGapTest()
        {
            var args = new TimedOutEventArgs(1, 42);
            Assert.AreEqual(42L, args.IntervalGapTick);
            Assert.AreEqual(42.0, args.IntervalGapTime);
        }
    }
}
