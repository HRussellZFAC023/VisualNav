using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using VisualThreading.Commands;

namespace VisualThreading.Tests.Commands
{
    [TestClass]
    public class OpenCommandPreviewTests
    {
        private MockRepository mockRepository;



        [TestInitialize]
        public void TestInitialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);


        }

        private OpenCommandPreview CreateOpenCommandPreview()
        {
            return new OpenCommandPreview();
        }

        [TestMethod]
        public void TestMethod1()
        {
            // Arrange
            var openCommandPreview = this.CreateOpenCommandPreview();

            // Act


            // Assert
            Assert.Fail();
            this.mockRepository.VerifyAll();
        }
    }
}
