using System;
using System.Threading;
using NUnit.Framework;
using Synker;

namespace UnitTest
{
    [TestFixture]
    public class TimeoutTest
    {
        [Test]
        public void StartTest()
        {
            using (var timeout = new Synker.Timeout())
            {
                Assert.IsFalse(timeout.Running);
                Assert.Greater(timeout.TimeoutMilliseconds, 0);

                timeout.Start();

                Assert.IsTrue(timeout.Running);

                timeout.Stop();

                Assert.IsFalse(timeout.Running);
            }
        }

        [Test]
        public void RestartTest()
        {
            using (var timeout = new Synker.Timeout())
            {
                Assert.IsFalse(timeout.Running);

                timeout.Start();

                Assert.IsTrue(timeout.Running);

                timeout.Restart();

                Assert.IsTrue(timeout.Running);
            }

            using (var timeout = new Synker.Timeout())
            {
                Assert.IsFalse(timeout.Running);

                timeout.Restart();

                Assert.IsTrue(timeout.Running);
            }
        }

        [Test]
        public void TimedOutTest1()
        {
            using (var timeout = new Synker.Timeout())
            {
                var timedout = false;

                timeout.TimedOut += (sender, args) =>
                {
                    timedout = true;

                    Assert.IsNotNull(sender);
                    // ReSharper disable once AccessToDisposedClosure
                    Assert.AreSame(timeout, sender);

                    Assert.IsNotNull(args);
                    Assert.IsFalse(double.IsInfinity(args.IntervalGapTime));
                    Assert.IsFalse(double.IsNaN(args.IntervalGapTime));
                    Assert.Greater(args.Frequency, 0);
                };

                timeout.Start();

                while (timeout.Running)
                    Thread.Sleep(1);

                Thread.Sleep(10);

                Assert.IsTrue(timedout);
                Assert.IsFalse(timeout.Running);
            }
        }

        [Test]
        public void TimedOutTest2()
        {
            using (var timeout = new Synker.Timeout())
            {
                var timedout = false;

                timeout.Mode = BlockingMode.Blocking;
                timeout.TimedOut += (sender, args) =>
                {
                    Assert.IsNotNull(sender);
                    // ReSharper disable once AccessToDisposedClosure
                    Assert.AreSame(timeout, sender);

                    Assert.IsNotNull(args);
                    Assert.IsFalse(double.IsInfinity(args.IntervalGapTime));
                    Assert.IsFalse(double.IsNaN(args.IntervalGapTime));
                    Assert.Greater(args.Frequency, 0);

                    timedout = true;
                };

                timeout.Start();

                Assert.IsTrue(timedout);
                Assert.IsFalse(timeout.Running);
            }
        }

        [Test]
        public void IntervalMillisecondsTest()
        {
            using (var timeout = new Synker.Timeout())
            {
                timeout.TimeoutMilliseconds = 42;
                Assert.AreEqual(42, timeout.TimeoutMilliseconds);
            }
        }

        [Test]
        public void IntervalMillisecondsError()
        {
            using (var timeout = new Synker.Timeout())
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => timeout.TimeoutMilliseconds = 0);
            }
        }

        [Test]
        public void StartTestDuplex()
        {
            var timeout = new Synker.Timeout();

            timeout.Start();
            Assert.IsTrue(timeout.Running);

            timeout.Start();
            Assert.IsTrue(timeout.Running);

            timeout.Stop();
            Assert.IsFalse(timeout.Running);

            timeout.Stop();
            Assert.IsFalse(timeout.Running);

            timeout.Dispose();
            Assert.IsTrue(timeout.IsDisposed);

            timeout.Dispose();
            Assert.IsTrue(timeout.IsDisposed);
        }

        [Test]
        public void FinalizeTest()
        {
            {
                // ReSharper disable once ObjectCreationAsStatement
                new Synker.Timeout();
            }

            GC.WaitForPendingFinalizers();
        }

        [Test]
        public void DisposeErrorTest()
        {
            var timeout = new Synker.Timeout();
            timeout.Dispose();

            Assert.Throws<ObjectDisposedException>(timeout.Start);
            Assert.Throws<ObjectDisposedException>(timeout.Stop);
            Assert.Throws<ObjectDisposedException>(timeout.Restart);
            Assert.Throws<ObjectDisposedException>(timeout.Reset);
            Assert.Throws<ObjectDisposedException>(() => timeout.TimeoutMilliseconds = 10);
        }

        [Test]
        public void ModeTest()
        {
            using (var timeout = new Synker.Timeout())
            {
                Assert.AreEqual(BlockingMode.Nonblocking, timeout.Mode);

                timeout.Mode = BlockingMode.Blocking;
                Assert.AreEqual(BlockingMode.Blocking, timeout.Mode);
            }
        }

        [Test]
        public void ModeError()
        {
            using (var timeout = new Synker.Timeout())
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => timeout.Mode = (BlockingMode)int.MaxValue);
            }
        }

        [Test]
        public void StartNewTest()
        {
            var processed = false;
            using (var timeout = Synker.Timeout.StartNew(10, (s, e) => processed = true))
            {
                Assert.IsNotNull(timeout);
                Assert.IsTrue(timeout.Running);

                Thread.Sleep(20);

                Assert.IsFalse(timeout.Running);
                Assert.IsTrue(processed);
            }
        }

        [Test]
        public void StartNewError()
        {
            Assert.Throws<ArgumentNullException>(() => Synker.Timeout.StartNew(10, null));
        }
    }
}
