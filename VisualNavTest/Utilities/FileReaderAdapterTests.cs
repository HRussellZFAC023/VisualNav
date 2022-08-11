using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using VisualNav.Utilities;

namespace VisualNavTest.Utilities
{
    [TestClass]
    public class FileReaderAdapterTests
    {
        [TestMethod]
        public async Task ReadFileAsync_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var fileReaderAdapter = new FileReaderAdapter();
            string file = null;

            // Act
            var result = await fileReaderAdapter.ReadFileAsync(file);

            // Assert
            Assert.Fail();
        }
    }
}
