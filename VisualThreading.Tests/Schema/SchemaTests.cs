using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;

namespace VisualThreading.Tests.Schema
{
    [TestClass]
    public class SchemaTests
    {
        private MockRepository mockRepository;



        [TestInitialize]
        public void TestInitialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);


        }


        [TestMethod]
        public async Task LoadAsync_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var schema = this.CreateSchema();

            // Act
            var result = await VisualThreading.Schema.Schema.LoadAsync();

            // Assert
            Assert.Fail();
            this.mockRepository.VerifyAll();
        }
    }
}
