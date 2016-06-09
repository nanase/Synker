using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Synker;

namespace UnitTest
{
    [TestClass]
    public class TimeoutTest
    {
        [TestMethod]
        public void StartTest()
        {
            using (var timeout = new Synker.Timeout())
            {
                Assert.IsFalse(timeout.Running);
                Assert.IsTrue(timeout.TimeoutMilliseconds > 0);

                timeout.Start();

                Assert.IsTrue(timeout.Running);

                timeout.Stop();

                Assert.IsFalse(timeout.Running);
            }
        }

        [TestMethod]
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

        [TestMethod]
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
                    Assert.IsTrue(args.Frequency > 0);
                };

                timeout.Start();

                while (timeout.Running)
                    Thread.Sleep(1);

                Assert.IsTrue(timedout);
                Assert.IsFalse(timeout.Running);
            }
        }

        [TestMethod]
        public void TimedOutTest2()
        {
            using (var timeout = new Synker.Timeout())
            {
                var timedout = false;

                timeout.Mode = TimeoutMode.Blocking;
                timeout.TimedOut += (sender, args) =>
                {
                    Assert.IsNotNull(sender);
                    // ReSharper disable once AccessToDisposedClosure
                    Assert.AreSame(timeout, sender);

                    Assert.IsNotNull(args);
                    Assert.IsFalse(double.IsInfinity(args.IntervalGapTime));
                    Assert.IsFalse(double.IsNaN(args.IntervalGapTime));
                    Assert.IsTrue(args.Frequency > 0);

                    timedout = true;
                };

                timeout.Start();

                Assert.IsTrue(timedout);
                Assert.IsFalse(timeout.Running);
            }
        }

        [TestMethod]
        public void IntervalMillisecondsTest()
        {
            using (var timeout = new Synker.Timeout())
            {
                timeout.TimeoutMilliseconds = 42;
                Assert.AreEqual(42, timeout.TimeoutMilliseconds);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void IntervalMillisecondsError()
        {
            using (var timeout = new Synker.Timeout())
            {
                timeout.TimeoutMilliseconds = 0;
            }
        }

        [TestMethod]
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

        [TestMethod]
        public void FinalizeTest()
        {
            {
                // ReSharper disable once ObjectCreationAsStatement
                new Synker.Timeout();
            }

            GC.WaitForPendingFinalizers();
        }

        [TestMethod]
        public void DisposeErrorTest()
        {
            var timeout = new Synker.Timeout();
            timeout.Dispose();

            ExceptionAssert.Expect(typeof (ObjectDisposedException), timeout.Start);
            ExceptionAssert.Expect(typeof(ObjectDisposedException), timeout.Stop);
            ExceptionAssert.Expect(typeof(ObjectDisposedException), timeout.Restart);
            ExceptionAssert.Expect(typeof(ObjectDisposedException), timeout.Reset);
            ExceptionAssert.Expect(typeof(ObjectDisposedException), () => timeout.TimeoutMilliseconds = 10);
        }

        [TestMethod]
        public void ModeTest()
        {
            using (var timeout = new Synker.Timeout())
            {
                Assert.AreEqual(TimeoutMode.Nonblocking, timeout.Mode);

                timeout.Mode = TimeoutMode.Blocking;
                Assert.AreEqual(TimeoutMode.Blocking, timeout.Mode);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ModeError()
        {
            using (var timeout = new Synker.Timeout())
            {
                timeout.Mode = (TimeoutMode)int.MaxValue;
            }
        }

        [TestMethod]
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

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void StartNewError()
        {
            Synker.Timeout.StartNew(10, null);
        }
    }
}
