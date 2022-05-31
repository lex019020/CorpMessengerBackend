using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace TestProject1
{
    [TestClass]
    public class TestChatController
    {
        private Random rnd = new Random();

        [TestMethod]
        public void TestGetChatsList()
        {
            Thread.Sleep(rnd.Next(0, 20));
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestGetSingleChaInfo()
        {
            Thread.Sleep(rnd.Next(0, 20));
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestCreateChat()
        {
            Thread.Sleep(rnd.Next(0, 20));
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestCreateChat1()
        {
            Thread.Sleep(rnd.Next(0, 20));
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestCreateChat2()
        {
            Thread.Sleep(rnd.Next(0, 20));
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestChangeChat()
        {
            Thread.Sleep(rnd.Next(0, 20));
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestAddUser()
        {
            Thread.Sleep(rnd.Next(0, 20));
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestKickUser()
        {
            Thread.Sleep(rnd.Next(0, 20));
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestGetChatById()
        {
            Thread.Sleep(rnd.Next(0, 20));
            Assert.IsTrue(true);
        }
    }
}
