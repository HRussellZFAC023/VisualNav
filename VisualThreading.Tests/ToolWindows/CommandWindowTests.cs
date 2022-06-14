using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;
using VisualThreading.ToolWindows;

namespace VisualThreading.Tests.ToolWindows
{
    [TestClass]
    public class CommandWindowTests
    {
        private MockRepository mockRepository;



        [TestInitialize]
        public void TestInitialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);


        }

        private CommandWindow CreateCommandWindow()
        {
            return new CommandWindow();
        }

        [TestMethod]
        public void GetTitle_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var commandWindow = this.CreateCommandWindow();
            int toolWindowId = 0;

            // Act
            var result = commandWindow.GetTitle(
                toolWindowId);

            // Assert
            Assert.Fail();
            this.mockRepository.VerifyAll();
        }

        [TestMethod]
        public async Task CreateAsync_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var commandWindow = this.CreateCommandWindow();
            int toolWindowId = 0;
            CancellationToken cancellationToken = default(global::System.Threading.CancellationToken);

            // Act
            var result = await commandWindow.CreateAsync(
                toolWindowId,
                cancellationToken);

            // Assert
            Assert.Fail();
            this.mockRepository.VerifyAll();
        }
    }
}
