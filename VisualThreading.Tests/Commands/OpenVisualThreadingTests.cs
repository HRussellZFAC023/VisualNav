using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using VisualThreading.Commands;

namespace VisualThreading.Tests.Commands
{
    [TestClass]
    public class OpenVisualThreadingTests
    {
        private MockRepository mockRepository;



        [TestInitialize]
        public void TestInitialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);


        }

        private OpenVisualThreading CreateOpenVisualThreading()
        {
            return new OpenVisualThreading();
        }

        [TestMethod]
        public void TestMethod1()
        {
            // Arrange
            var openVisualThreading = this.CreateOpenVisualThreading();

            // Act


            // Assert
            Assert.Fail();
            this.mockRepository.VerifyAll();
        }
    }
}
