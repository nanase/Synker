using System;
using System.Threading;
using NUnit.Framework;
using Synker;

namespace UnitTest
{
    [TestFixture]
    public class IntervalTest
    {
        [Test]
        public void StartTest()
        {
            using (var interval = new Interval())
            {
                Assert.IsFalse(interval.Running);
                Assert.AreEqual(0, interval.TickCount);
                Assert.Greater(interval.IntervalMilliseconds, 0);

                interval.Start();

                Assert.IsTrue(interval.Running);

                interval.Stop();

                Assert.IsFalse(interval.Running);
            }
        }

        [Test]
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

        [Test]
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
                    Assert.Greater(args.Frequency, 0);

                    elapsed = true;
                };

                interval.Start();

                while (!elapsed)
                    Thread.Sleep(1);

                interval.Stop();

                Assert.IsFalse(interval.Running);
                Assert.GreaterOrEqual(interval.TickCount, 1);
            }
        }

        [Test]
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

        [Test]
        public void IntervalMillisecondsTest()
        {
            using (var interval = new Interval())
            {
                interval.IntervalMilliseconds = 42;
                Assert.AreEqual(42, interval.IntervalMilliseconds);
            }
        }

        [Test]
        public void IntervalMillisecondsError()
        {
            using (var interval = new Interval())
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => interval.IntervalMilliseconds = 0);
            }
        }

        [Test]
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

        [Test]
        public void FinalizeTest()
        {
            {
                // ReSharper disable once ObjectCreationAsStatement
                new Interval();
            }

            GC.WaitForPendingFinalizers();
        }

        [Test]
        public void DisposeErrorTest()
        {
            var interval = new Interval();
            interval.Dispose();

            Assert.Throws<ObjectDisposedException>(interval.Start);
            Assert.Throws<ObjectDisposedException>(interval.Stop);
            Assert.Throws<ObjectDisposedException>(interval.Restart);
            Assert.Throws<ObjectDisposedException>(interval.Reset);
            Assert.Throws<ObjectDisposedException>(() => interval.IntervalMilliseconds = 10);
        }

        [Test]
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

        [Test]
        public void StartNewError()
        {
            Assert.Throws<ArgumentNullException>(() => Interval.StartNew(10, null));
        }
    }
}
