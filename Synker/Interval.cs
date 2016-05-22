﻿using System;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace Synker
{
    /// <summary>
    /// 一定の間隔でイベントを発生させるための機能を提供します。
    /// </summary>
    public class Interval
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
        public int IntervalMilliseconds
        {
            get { return intervalMilliseconds; }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException(nameof(value));

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

        #endregion

        #region -- Public Events --

        /// <summary>
        /// 指定された間隔が経過し、イベントを発生させようとしたときに発生します。
        /// </summary>
        public event EventHandler<TimerElapsedEventArgs> Elapsed;

        #endregion

        #region -- Constructors --

        /// <summary>
        /// パラメータを指定せずに新しい Interval クラスのインスタンスを指定します。
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
        public void Start()
        {
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
        public void Stop(TimeSpan timeout)
        {
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
        public void Reset()
        {
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
        public void Restart(TimeSpan timeout)
        {
            Stop(timeout);
            Start();
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
                var gapDelta = (long) gap;
                gap -= gapDelta;

                var targetTick = oldTick + deltaTick + gapDelta;

                var args = new TimerElapsedEventArgs(frequency, TickCount, oldTick - nowTick, processTime);
                var beforeProcessTick = Stopwatch.GetTimestamp();

                Elapsed?.Invoke(this, args);

                nowTick = Stopwatch.GetTimestamp();
                processTime = nowTick - beforeProcessTick;
                
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
