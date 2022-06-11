using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using VisualThreading.Options;

namespace VisualThreading.Tests.Options
{
    [TestClass]
    public class OptionsProviderTests
    {
        private MockRepository mockRepository;



        [TestInitialize]
        public void TestInitialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);


        }

        private OptionsProvider CreateProvider()
        {
            return new OptionsProvider();
        }

        [TestMethod]
        public void TestMethod1()
        {
            // Arrange
            var provider = this.CreateProvider();

            // Act


            // Assert
            Assert.Fail();
            this.mockRepository.VerifyAll();
        }
    }
}
