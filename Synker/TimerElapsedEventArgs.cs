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
using Synker.Resource;

namespace Synker
{
    /// <summary>
    /// タイマが指定された間隔だけ経過してイベントが発生したときに使用される引数を格納します。
    /// </summary>
    public class TimerElapsedEventArgs : EventArgs
    {
        #region -- Public Properties --

        /// <summary>
        /// タイマで使われていた高精度タイマの周波数を取得します。
        /// </summary>
        public long Frequency { get; }

        /// <summary>
        /// このイベントが何回目に発生したかを表す数値を取得します。
        /// </summary>
        public long Count { get; }

        /// <summary>
        /// 本来発生すべき時刻と、このイベントの発生時刻のずれを表すティック値を取得します。
        /// </summary>
        public long IntervalGapTick { get; }

        /// <summary>
        /// 前回のイベントの処理時間を表すティック値を取得します。
        /// </summary>
        public long LastEventProcessTick { get; }

        /// <summary>
        /// 本来発生すべき時刻と、このイベントの発生時刻のずれを表す秒数を取得します。
        /// </summary>
        public double IntervalGapTime => (double)IntervalGapTick / Frequency;

        /// <summary>
        /// 前回のイベントの処理時間を表す秒数を取得します。
        /// </summary>
        public double LastEventProcessTime => (double)LastEventProcessTick / Frequency;

        #endregion

        #region -- Constructors --

        /// <summary>
        /// 指定されたパラメータを使って新しい <see cref="TimerElapsedEventArgs"/> クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="frequency">タイマで使われていた高精度タイマの周波数。</param>
        /// <param name="count">このイベントが何回目に発生したかを表す数値。</param>
        /// <param name="intervalGapTick">本来発生すべき時刻と、このイベントの発生時刻のずれを表すティック値。</param>
        /// <param name="lastEventProcessTick">前回のイベントの処理時間を表すティック値。</param>
        /// <exception cref="ArgumentOutOfRangeException">パラメータのいずれかの値が有効な範囲内にないときに発生します。</exception>
        public TimerElapsedEventArgs(
            long frequency, long count, long intervalGapTick, long lastEventProcessTick)
        {
            if (frequency < 1)
                throw new ArgumentOutOfRangeException(nameof(frequency), frequency, Language.TimerElapsedEventArgs_ArgumentOutOfRange_0);

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), count, Language.TimerElapsedEventArgs_ArgumentOutOfRange_0);

            if (lastEventProcessTick < 0)
                throw new ArgumentOutOfRangeException(nameof(lastEventProcessTick), lastEventProcessTick, Language.TimerElapsedEventArgs_ArgumentOutOfRange_0);

            Frequency = frequency;
            Count = count;
            IntervalGapTick = intervalGapTick;
            LastEventProcessTick = lastEventProcessTick;
        }

        #endregion
    }
}
