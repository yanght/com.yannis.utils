using System;
using NUnit.Framework;
using com.yannis.utils;
using System.IO;

namespace com.yannis.test.com.yannis.utils
{
    [TestFixture]
    public class TestVerifyCode
    {
        [Test]
        public void TestMethod1()
        {
            string code = string.Empty;

            VerifyCodeHelper.VerifyCode(10, 10, 10, out code);

        }
    }
}
