using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using VisualThreading.ToolWindows;

namespace VisualThreading.Tests.ToolWindows
{
    [TestClass]
    public class VisualThreadingWindowControlTests
    {
        private MockRepository mockRepository;



        [TestInitialize]
        public void TestInitialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);


        }

        private VisualThreadingWindowControl CreateVisualThreadingWindowControl()
        {
            return new VisualThreadingWindowControl();
        }

        [TestMethod]
        public void TestMethod1()
        {
            // Arrange
            var visualThreadingWindowControl = this.CreateVisualThreadingWindowControl();

            // Act


            // Assert
            Assert.Fail();
            this.mockRepository.VerifyAll();
        }
    }
}
