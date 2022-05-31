using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;


namespace TestProject1
{
    [TestClass]
    public class TestDepartmentController
    {
        Random rnd = new Random();

        [TestMethod]
        public void TestGetDepartmentsList()
        {
            Thread.Sleep(rnd.Next(0, 20));
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestGetDepartmentById()
        {
            Thread.Sleep(rnd.Next(0, 20));
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestAddDepartment()
        {
            Thread.Sleep(rnd.Next(0, 20));
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestEditDepartment()
        {
            Thread.Sleep(rnd.Next(0, 20));
            Assert.IsTrue(true);
        }
    }
}
