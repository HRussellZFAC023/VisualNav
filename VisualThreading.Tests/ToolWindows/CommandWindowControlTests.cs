using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using VisualThreading.ToolWindows;

namespace VisualThreading.Tests.ToolWindows
{
    [TestClass]
    public class CommandWindowControlTests
    {
        private MockRepository mockRepository;



        [TestInitialize]
        public void TestInitialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);


        }

        private CommandWindowControl CreateCommandWindowControl()
        {
            return new CommandWindowControl(
                TODO,
                TODO);
        }

        [TestMethod]
        public void TestMethod1()
        {
            // Arrange
            var commandWindowControl = this.CreateCommandWindowControl();

            // Act


            // Assert
            Assert.Fail();
            this.mockRepository.VerifyAll();
        }
    }
}
