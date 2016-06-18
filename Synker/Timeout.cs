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
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Synker.Resource;

namespace Synker
{
    /// <summary>
    /// 一定の時間でイベントを発生させるための機能を提供します。
    /// </summary>
    public class Timeout : IDisposable
    {
        #region -- Private Fields --
        
        private volatile bool requestedStop;
        private Task tickerTask;
        private int timeoutMilliseconds = 10;
        private BlockingMode mode = BlockingMode.Nonblocking;
        private long targetTick;

        #endregion

        #region -- Public Properties --

        /// <summary>
        /// イベントを発生するタイムアウト値をミリ秒で設定または取得します。
        /// 設定値は 1 ミリ秒以上の値でなくてはなりません。
        /// </summary>
        /// <exception cref="ObjectDisposedException">オブジェクトが破棄された後に呼び出されると発生します。</exception>
        public int TimeoutMilliseconds
        {
            get { return timeoutMilliseconds; }
            set
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(GetType().FullName, Language.Timeout_ObjectDisposed);

                if (value < 1)
                    throw new ArgumentOutOfRangeException(nameof(value), value, Language.Timeout_ArgumentOutOfRange_1);

                timeoutMilliseconds = value;
            }
        }

        /// <summary>
        /// ブロッキングモードを表す <see cref="BlockingMode"/> 列挙体の値を取得または設定します。
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">与えられた引数が有効な <see cref="BlockingMode"/> 列挙体の値でないときに発生します。</exception>
        public BlockingMode Mode
        {
            get { return mode; }
            set
            {
                if (!Enum.IsDefined(typeof(BlockingMode), value))
                    throw new ArgumentOutOfRangeException(nameof(value), value, Language.Timeout_ArgumentOutOfRange_2);

                mode = value;
            }
        }

        /// <summary>
        /// オブジェクトが破棄されたかを表す真偽値を取得します。
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// タイマが動作しているかを表す真偽値を取得します。
        /// </summary>
        public bool Running { get; private set; }

        #endregion

        #region -- Public Events --

        /// <summary>
        /// 指定された時間が経過し、イベントを発生させようとしたときに発生します。
        /// </summary>
        public event EventHandler<TimedOutEventArgs> TimedOut;

        #endregion

        #region -- Constructors --

        /// <summary>
        /// パラメータを指定せずに新しい <see cref="Timeout"/> クラスのインスタンスを指定します。
        /// </summary>
        public Timeout()
        {
            this.Reset();
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
                throw new ObjectDisposedException(GetType().FullName, Language.Timeout_ObjectDisposed);

            if (Running)
                return;
            
            targetTick = Stopwatch.GetTimestamp() + (long)Math.Round(TimeoutMilliseconds * 0.001 * Stopwatch.Frequency);
            Running = true;
            requestedStop = false;

            if (mode == BlockingMode.Blocking)
                Tick();
            else
                tickerTask = Task.Factory.StartNew(Tick);
        }

        /// <summary>
        /// タイマを停止し、イベントの発生を止めます。
        /// </summary>
        public void Stop()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().FullName, Language.Timeout_ObjectDisposed);

            if (!Running)
                return;

            requestedStop = true;
            tickerTask?.Wait();
            tickerTask?.Dispose();
            tickerTask = null;
            Running = false;
        }

        /// <summary>
        /// イベント発生ティックを初期化し、再び発生ができる状態にリセットします。
        /// </summary>
        /// <exception cref="ObjectDisposedException">オブジェクトが破棄された後に呼び出されると発生します。</exception>
        public void Reset()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().FullName, Language.Timeout_ObjectDisposed);

            Stop();
            targetTick = 0L;
        }

        /// <summary>
        /// タイマを停止し、再び開始します。
        /// </summary>
        public void Restart()
        {
            Stop();
            Reset();
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
        /// 時間と呼び出されるメソッドを指定して新しい <see cref="Timeout"/> クラスのインスタンスを初期化し、
        /// タイマを開始させます。
        /// </summary>
        /// <param name="milliseconds">イベントを発生するミリ秒単位のタイムアウト値。</param>
        /// <param name="callback">呼び出されるメソッド。</param>
        /// <returns>新しい <see cref="Timeout"/> クラスのインスタンス。</returns>
        public static Timeout StartNew(int milliseconds, Action<object, TimedOutEventArgs> callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            var timeout = new Timeout { TimeoutMilliseconds = milliseconds };
            timeout.TimedOut += new EventHandler<TimedOutEventArgs>(callback);
            timeout.Start();

            return timeout;
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

        ~Timeout()
        {
            Dispose(false);
        }

        #endregion

        #region -- Private Methods --

        private void Tick()
        {
            var frequency = Stopwatch.Frequency;

            while (!requestedStop)
            {
                var nowTick = Stopwatch.GetTimestamp();

                if (nowTick >= targetTick)
                {
                    requestedStop = true;
                    Running = false;
                    TimedOut?.Invoke(
                        this,
                        new TimedOutEventArgs(frequency, targetTick - nowTick));
                    return;
                }

                Thread.Sleep(1);
            }
        }

        #endregion
    }

    
}
