using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{
    internal static class ExceptionAssert
    {
        public static void Expect(Type type, Action action)
        {
            try
            {
                action();
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual(type, e.GetType());
            }
        }
    }
}
