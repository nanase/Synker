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
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
        public void CtorError()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new TimedOutEventArgs(0, 42L));
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
