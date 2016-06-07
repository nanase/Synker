using System;
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
        public void ElapsedTest1()
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

        [TestMethod]
        public void ElapsedTest2()
        {
            using (var interval = new Interval() { IntervalMilliseconds = 1 })
            {
                interval.Elapsed += (sender, args) => Thread.Sleep(10);

                interval.Start();

                Thread.Sleep(5);

                interval.Stop();

                Assert.IsFalse(interval.Running);
            }
        }

        [TestMethod]
        public void IntervalMillisecondsTest()
        {
            using (var interval = new Interval())
            {
                interval.IntervalMilliseconds = 42;
                Assert.AreEqual(42, interval.IntervalMilliseconds);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void IntervalMillisecondsError()
        {
            using (var interval = new Interval())
            {
                interval.IntervalMilliseconds = 0;
            }
        }

        [TestMethod]
        public void StartTestDuplex()
        {
            var interval = new Interval();

            interval.Start();
            Assert.IsTrue(interval.Running);

            interval.Start();
            Assert.IsTrue(interval.Running);

            interval.Stop();
            Assert.IsFalse(interval.Running);

            interval.Stop();
            Assert.IsFalse(interval.Running);

            interval.Dispose();
            Assert.IsTrue(interval.IsDisposed);

            interval.Dispose();
            Assert.IsTrue(interval.IsDisposed);
        }

        [TestMethod]
        public void FinalizeTest()
        {
            {
                // ReSharper disable once ObjectCreationAsStatement
                new Interval();
            }

            GC.WaitForPendingFinalizers();
        }

        [TestMethod]
        public void DisposeErrorTest()
        {
            var interval = new Interval();
            interval.Dispose();

            ExceptionAssert.Expect(typeof (ObjectDisposedException), interval.Start);
            ExceptionAssert.Expect(typeof(ObjectDisposedException), interval.Stop);
            ExceptionAssert.Expect(typeof(ObjectDisposedException), interval.Restart);
            ExceptionAssert.Expect(typeof(ObjectDisposedException), interval.Reset);
            ExceptionAssert.Expect(typeof(ObjectDisposedException), () => interval.IntervalMilliseconds = 10);
        }

        [TestMethod]
        public void StartNewTest()
        {
            var processed = false;
            using (var interval = Interval.StartNew(10, (s, e) => processed = true))
            {
                Assert.IsNotNull(interval);
                Assert.IsTrue(interval.Running);
                Thread.Sleep(20);

                interval.Stop();
                Assert.IsFalse(interval.Running);
                Assert.IsTrue(processed);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void StartNewError()
        {
            Interval.StartNew(10, null);
        }
    }
}
