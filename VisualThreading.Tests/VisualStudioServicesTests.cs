using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using VisualThreading;

namespace VisualThreading.Tests
{
    [TestClass]
    public class VisualStudioServicesTests
    {
        private MockRepository mockRepository;



        [TestInitialize]
        public void TestInitialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);


        }

        private VisualStudioServices CreateVisualStudioServices()
        {
            return new VisualStudioServices();
        }

        [TestMethod]
        public void TestMethod1()
        {
            // Arrange
            var visualStudioServices = this.CreateVisualStudioServices();

            // Act


            // Assert
            Assert.Fail();
            this.mockRepository.VerifyAll();
        }
    }
}
