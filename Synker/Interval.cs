/* LICENSE

The MIT License (MIT)

Copyright (c) 2016 Tomona Nanase

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

*/

using System;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using Synker.Resource;

namespace Synker
{
    /// <summary>
    /// 一定の間隔でイベントを発生させるための機能を提供します。
    /// </summary>
    public class Interval : IDisposable
    {
        #region -- Private Fields --

        private volatile bool requestedStop;
        private Task tickerTask;
        private int intervalMilliseconds = 10;

        #endregion

        #region -- Public Properties --

        /// <summary>
        /// イベントを発生する間隔値をミリ秒で設定または取得します。
        /// 設定値は 1 ミリ秒以上の値でなくてはなりません。
        /// </summary>
        /// <exception cref="ObjectDisposedException">オブジェクトが破棄された後に呼び出されると発生します。</exception>
        public int IntervalMilliseconds
        {
            get { return intervalMilliseconds; }
            set
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(GetType().FullName, Language.Interval_ObjectDisposed);

                if (value < 1)
                    throw new ArgumentOutOfRangeException(nameof(value), value, Language.Interval_ArgumentOutOfRange_1);

                intervalMilliseconds = value;
            }
        }

        /// <summary>
        /// タイマが動作しているかを表す真偽値を取得します。
        /// </summary>
        public bool Running { get; private set; }

        /// <summary>
        /// イベントが発生した回数を取得します。
        /// </summary>
        public long TickCount { get; private set; }

        /// <summary>
        /// オブジェクトが破棄されたかを表す真偽値を取得します。
        /// </summary>
        public bool IsDisposed { get; private set; }

        #endregion

        #region -- Public Events --

        /// <summary>
        /// 指定された間隔が経過し、イベントを発生させようとしたときに発生します。
        /// </summary>
        public event EventHandler<TimerElapsedEventArgs> Elapsed;

        #endregion

        #region -- Constructors --

        /// <summary>
        /// パラメータを指定せずに新しい <see cref="Interval"/> クラスのインスタンスを指定します。
        /// </summary>
        public Interval()
        {
            Reset();
        }

        #endregion

        #region -- Public Methods --

        /// <summary>
        /// タイマを動作させ、イベントを発生できる状態にします。
        /// </summary>
        /// <exception cref="ObjectDisposedException">オブジェクトが破棄された後に呼び出されると発生します。</exception>
        public void Start()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().FullName, Language.Interval_ObjectDisposed);

            if (Running)
                return;

            requestedStop = false;
            tickerTask = Task.Factory.StartNew(Tick);
            Running = true;
        }

        /// <summary>
        /// タイマを停止し、イベントの発生を止めます。
        /// </summary>
        public void Stop()
        {
            Stop(TimeSpan.FromMilliseconds(-1));
        }

        /// <summary>
        /// タイマを停止し、イベントの発生を止め、指定された時間だけ停止を待ちます。
        /// </summary>
        /// <param name="timeout">停止待機時間を表す <see cref="TimeSpan"/> 型のインスタンス。</param>
        /// <exception cref="ObjectDisposedException">オブジェクトが破棄された後に呼び出されると発生します。</exception>
        public void Stop(TimeSpan timeout)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().FullName, Language.Interval_ObjectDisposed);

            if (!Running)
                return;

            requestedStop = true;
            tickerTask.Wait(timeout);
            tickerTask.Dispose();
            tickerTask = null;
            Running = false;
        }

        /// <summary>
        /// イベント発生カウントを初期化し、再び発生ができる状態にリセットします。
        /// </summary>
        /// <exception cref="ObjectDisposedException">オブジェクトが破棄された後に呼び出されると発生します。</exception>
        public void Reset()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().FullName, Language.Interval_ObjectDisposed);

            TickCount = 0L;
            requestedStop = false;
        }

        /// <summary>
        /// タイマを停止し、再び開始します。
        /// </summary>
        public void Restart()
        {
            Restart(TimeSpan.FromMilliseconds(-1));
        }

        /// <summary>
        /// 指定された時間だけ停止を待ち、タイマを再び開始します。
        /// </summary>
        /// <param name="timeout">停止待機時間を表す <see cref="TimeSpan"/> 型のインスタンス。</param>
        /// <exception cref="ObjectDisposedException">オブジェクトが破棄された後に呼び出されると発生します。</exception>
        public void Restart(TimeSpan timeout)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().FullName, Language.Interval_ObjectDisposed);

            Stop(timeout);
            Start();
        }

        /// <summary>
        /// このオブジェクトで使用されているリソースを破棄します。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

        #region -- Public Static Methods --

        /// <summary>
        /// 間隔と呼び出されるメソッドを指定して新しい <see cref="Interval"/> クラスのインスタンスを初期化し、
        /// タイマを開始させます。
        /// </summary>
        /// <param name="milliseconds">イベントを発生するミリ秒単位の間隔値。</param>
        /// <param name="callback">呼び出されるメソッド。</param>
        /// <returns>新しい <see cref="Interval"/> クラスのインスタンス。</returns>
        public static Interval StartNew(int milliseconds, Action<object, TimerElapsedEventArgs> callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            var interval = new Interval { IntervalMilliseconds = milliseconds };
            interval.Elapsed += new EventHandler<TimerElapsedEventArgs>(callback);
            interval.Start();

            return interval;
        }

        #endregion

        #region -- Protected Methods --

        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed) return;

            Stop();

            IsDisposed = true;

            if (disposing)
                GC.SuppressFinalize(this);
        }

        ~Interval()
        {
            Dispose(false);
        }

        #endregion

        #region -- Private Methods --

        private void Tick()
        {
            var processTime = 0L;
            var oldTick = Stopwatch.GetTimestamp();
            var frequency = Stopwatch.Frequency;
            var milliFrequency = (long)(frequency * 0.001);
            var gap = 0.0;
            var nowTick = oldTick;

            while (!requestedStop)
            {
                TickCount++;

                var deltaTickDouble = intervalMilliseconds * 0.001 * frequency;
                var deltaTick = (long)deltaTickDouble;
                gap += deltaTickDouble - deltaTick;
                var gapDelta = (long)gap;
                gap -= gapDelta;

                var targetTick = oldTick + deltaTick + gapDelta;

                {
                    var beforeProcessTick = Stopwatch.GetTimestamp();

                    Elapsed?.Invoke(
                        this,
                        new TimerElapsedEventArgs(frequency, TickCount, oldTick - nowTick, processTime));

                    nowTick = Stopwatch.GetTimestamp();
                    processTime = nowTick - beforeProcessTick;
                }

                if (requestedStop)
                    break;

                while (!requestedStop && Stopwatch.GetTimestamp() + milliFrequency < targetTick)
                    Thread.Sleep(1);

                oldTick = targetTick;
            }
        }

        #endregion
    }
}
