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

namespace Synker
{
    /// <summary>
    /// タイマの動作時のブロッキングモードを定義します。
    /// </summary>
    public enum BlockingMode
    {
        /// <summary>
        /// ノンブロッキングモード。
        /// タイマの開始から停止まで別スレッドでカウントが行われ、コールバックは非同期で行われます。
        /// </summary>
        Nonblocking,

        /// <summary>
        /// ブロッキングモード。
        /// タイマの開始から停止まで呼び出し元と同じスレッドで行われます。
        /// </summary>
        Blocking
    }
}
