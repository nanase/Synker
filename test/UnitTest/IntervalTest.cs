using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Synker;

namespace UnitTest
{
    [TestClass]
    public class IntervalTest
    {
        [TestMethod]
        public void StartTest()
        {
            using (var interval = new Interval())
            {
                Assert.IsFalse(interval.Running);
                Assert.AreEqual(0, interval.TickCount);
                Assert.IsTrue(interval.IntervalMilliseconds > 0);

                interval.Start();

                Assert.IsTrue(interval.Running);

                interval.Stop();

                Assert.IsFalse(interval.Running);
            }
        }

        [TestMethod]
        public void RestartTest()
        {
            using (var interval = new Interval())
            {
                Assert.IsFalse(interval.Running);

                interval.Start();

                Assert.IsTrue(interval.Running);

                interval.Restart();

                Assert.IsTrue(interval.Running);
            }

            using (var interval = new Interval())
            {
                Assert.IsFalse(interval.Running);

                interval.Restart();

                Assert.IsTrue(interval.Running);
            }
        }

        [TestMethod]
        public void ElapsedTest()
        {
            using (var interval = new Interval())
            {
                var elapsed = false;

                interval.Elapsed += (sender, args) =>
                {
                    if (elapsed)
                        return;

                    Assert.IsNotNull(sender);
                    // ReSharper disable once AccessToDisposedClosure
                    Assert.AreSame(interval, sender);

                    Assert.IsNotNull(args);
                    Assert.AreEqual(1, args.Count);
                    Assert.IsTrue(args.Frequency > 0);

                    elapsed = true;
                };

                interval.Start();

                while (!elapsed)
                    Thread.Sleep(1);

                interval.Stop();

                Assert.IsFalse(interval.Running);
                Assert.IsTrue(interval.TickCount >= 1);
            }
        }

    }
}
