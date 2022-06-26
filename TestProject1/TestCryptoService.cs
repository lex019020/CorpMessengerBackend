using Microsoft.VisualStudio.TestTools.UnitTesting;
using CorpMessengerBackend.Services;

namespace TestProject1
{
    
    [TestClass]
    public class TestCryptoService
    {
        private readonly ICriptographyProvider _cryptographyProvider;

        public TestCryptoService()
        {
            _cryptographyProvider = new CryptographyService();
        }


        [TestMethod]
        public void TestPassHash()
        {
            var password = "it387yfiukdjbflky";

            var secret = _cryptographyProvider.HashPassword(password);

            Assert.IsTrue(_cryptographyProvider.CheckPassword(password, secret));
        }

        [TestMethod]
        public void TestBadPassHash()
        {
            var password = "it387yfiukdjbflky";

            var secret = _cryptographyProvider.HashPassword(password);

            Assert.IsFalse(_cryptographyProvider.CheckPassword(password, "sdfdsdsbgdfsh"));
        }

        [TestMethod]
        public void TestBadInput()
        {
            Assert.IsFalse(_cryptographyProvider.CheckPassword(null, ""));
        }

        [TestMethod]
        public void TestBadInput_2()
        {
            Assert.IsFalse(_cryptographyProvider.CheckPassword("adasd", ""));
        }

        [TestMethod]
        public void TestTokenGen()
        {
            var token = _cryptographyProvider.GenerateNewToken();

            Assert.IsTrue(!string.IsNullOrEmpty(token));   
            Assert.IsTrue(token.Length >= 96);
            Assert.IsTrue(token.Length <= 128);
        }
    }
}
