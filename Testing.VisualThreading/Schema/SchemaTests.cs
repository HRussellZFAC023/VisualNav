using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using VisualThreading.Schema;

namespace Testing.VisualThreading.Schema
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

        private global::VisualThreading.Schema.Schema CreateSchema()
        {
            return new Schema();
        }

        [TestMethod]
        public async Task LoadAsync_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var schema = this.CreateSchema();

            // Act
            var result = await schema.LoadAsync();

            // Assert
            Assert.Fail();
            this.mockRepository.VerifyAll();
        }
    }
}
