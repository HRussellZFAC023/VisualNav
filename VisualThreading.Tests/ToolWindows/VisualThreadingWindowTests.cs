using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;
using VisualThreading.ToolWindows;

namespace VisualThreading.Tests.ToolWindows
{
    [TestClass]
    public class VisualThreadingWindowTests
    {
        private MockRepository mockRepository;



        [TestInitialize]
        public void TestInitialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);


        }

        private VisualThreadingWindow CreateVisualThreadingWindow()
        {
            return new VisualThreadingWindow();
        }

        [TestMethod]
        public void GetTitle_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var visualThreadingWindow = this.CreateVisualThreadingWindow();
            int toolWindowId = 0;

            // Act
            var result = visualThreadingWindow.GetTitle(
                toolWindowId);

            // Assert
            Assert.Fail();
            this.mockRepository.VerifyAll();
        }

        [TestMethod]
        public async Task CreateAsync_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var visualThreadingWindow = this.CreateVisualThreadingWindow();
            int toolWindowId = 0;
            CancellationToken cancellationToken = default(global::System.Threading.CancellationToken);

            // Act
            var result = await visualThreadingWindow.CreateAsync(
                toolWindowId,
                cancellationToken);

            // Assert
            Assert.Fail();
            this.mockRepository.VerifyAll();
        }
    }
}
