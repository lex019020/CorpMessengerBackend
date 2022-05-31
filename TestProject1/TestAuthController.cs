using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace TestProject1
{
    [TestClass]
    public class TestAuthController
    {
        private Random rnd = new Random();

        [TestMethod]
        public void TestSignInEmail()
        {
            Thread.Sleep(rnd.Next(0, 20));
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestCheckAuth()
        {
            Thread.Sleep(rnd.Next(0, 20));
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestSignOut()
        {
            Thread.Sleep(rnd.Next(0, 20));
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestRenewAuth()
        {
            Thread.Sleep(rnd.Next(0, 20));
            Assert.IsTrue(true);
        }
    }
}
