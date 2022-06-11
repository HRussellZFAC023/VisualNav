using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using VisualThreading.Commands;

namespace VisualThreading.Tests.Commands
{
    [TestClass]
    public class OpenRadialMenuTests
    {
        private MockRepository mockRepository;



        [TestInitialize]
        public void TestInitialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);


        }

        private OpenRadialMenu CreateOpenRadialMenu()
        {
            return new OpenRadialMenu();
        }

        [TestMethod]
        public void TestMethod1()
        {
            // Arrange
            var openRadialMenu = this.CreateOpenRadialMenu();

            // Act


            // Assert
            Assert.Fail();
            this.mockRepository.VerifyAll();
        }
    }
}
