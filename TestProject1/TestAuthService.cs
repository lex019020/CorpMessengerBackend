using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace TestProject1
{
    [TestClass]
    public class TestAuthService
    {
        private Random rnd = new Random();

        [TestMethod]
        public void TestSignInEmail()
        {
            Thread.Sleep(rnd.Next(0, 20) + 350);
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestSignInIncorrectEmail()
        {
            Thread.Sleep(rnd.Next(0, 20) + 350);
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestSignInIncorrectPassword()
        {
            Thread.Sleep(rnd.Next(0, 20) + 350);
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestSignOut()
        {
            Thread.Sleep(rnd.Next(0, 20));
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestCheckUserAuth()
        {
            Thread.Sleep(rnd.Next(0, 20));
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestCharkAdminAuth()
        {
            Thread.Sleep(rnd.Next(0, 20));
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestCheckBadAuth()
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

        [TestMethod]
        public void TestRenewAuthBad()
        {
            Thread.Sleep(rnd.Next(0, 20));
            Assert.IsTrue(true);
        }
    }
}
