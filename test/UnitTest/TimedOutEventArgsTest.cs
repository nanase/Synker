using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Synker;

namespace UnitTest
{
    [TestClass]
    public class TimedOutEventArgsTest
    {
        [TestMethod]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
        public void CtorError()
        {
            ExceptionAssert.Expect(typeof(ArgumentOutOfRangeException), () => new TimedOutEventArgs(0, 42L));
        }

        [TestMethod]
        public void IntervalGapTest()
        {
            var args = new TimedOutEventArgs(1, 42);
            Assert.AreEqual(42L, args.IntervalGapTick);
            Assert.AreEqual(42.0, args.IntervalGapTime);
        }
    }
}
