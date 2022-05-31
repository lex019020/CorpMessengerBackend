using Microsoft.VisualStudio.TestTools.UnitTesting;
using CorpMessengerBackend.Services;

namespace TestProject1
{
    [TestClass]
    public class TestCryptoService
    {
        [TestMethod]
        public void TestPassHash()
        {
            var password = "it387yfiukdjbflky";

            var secret = CryptographyService.HashPassword(password);

            Assert.IsTrue(CryptographyService.CheckPassword(password, secret));
        }

        [TestMethod]
        public void TestBadPassHash()
        {
            var password = "it387yfiukdjbflky";

            var secret = CryptographyService.HashPassword(password);

            Assert.IsFalse(CryptographyService.CheckPassword(password, "sdfdsdsbgdfsh"));
        }

        [TestMethod]
        public void TestBadInput()
        {
            Assert.IsFalse(CryptographyService.CheckPassword(null, ""));
        }

        [TestMethod]
        public void TestBadInput_2()
        {
            Assert.IsFalse(CryptographyService.CheckPassword("adasd", ""));
        }

        [TestMethod]
        public void TestTokenGen()
        {
            var token = CryptographyService.GenerateNewToken();

            Assert.IsTrue(!string.IsNullOrEmpty(token));   
            Assert.IsTrue(token.Length >= 96);
            Assert.IsTrue(token.Length <= 128);
        }
    }
}
