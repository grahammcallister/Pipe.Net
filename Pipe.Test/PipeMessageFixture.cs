using NUnit.Framework;

namespace Pipe.Test
{
    [TestFixture]
    public class PipeMessageFixture
    {
        [TestCase]
        public void GetMessageFromBuffer_WithNoCarriageReturnInitialiser_ReturnsMessageStringWithOnlyOneCarriageReturn()
        {
            // Arrange
            var sut = new PipeMessage("Hello");
            // Act
            var message = sut.GetMessageFromBuffer();
            // Assert
            Assert.AreEqual("Hello\r\n", message);
        }

        [TestCase]
        public void GetMessageFromBuffer_WithCarriageReturnInitialiser_ReturnsMessageStringWithOnlyOneCarriageReturn()
        {
            // Arrange
            var sut = new PipeMessage("Hello\r\n");
            // Act
            var message = sut.GetMessageFromBuffer();
            // Assert
            Assert.AreEqual("Hello\r\n", message);
        }

        [TestCase]
        public void
            GetMessageFromBuffer_WithTwoCarriageReturnInitialiser_ReturnsMessageStringWithOnlyOneCarriageReturn()
        {
            // Arrange
            var sut = new PipeMessage("Hello\r\n\r\n");
            // Act
            var message = sut.GetMessageFromBuffer();
            // Assert
            Assert.AreEqual("Hello\r\n", message);
        }

        [TestCase]
        public void
            GetMessageFromBuffer_WithTwoCarriageReturnsAndEmptyInInitialiser_ReturnsMessageStringWithOneCarriageReturn()
        {
            // Arrange
            var sut = new PipeMessage("Hello\r\n\0\r\n");
            // Act
            var message = sut.GetMessageFromBuffer();
            // Assert
            Assert.AreEqual("Hello\r\n", message);
        }

        [TestCase]
        public void
            GetMessageFromBuffer_WithInterspersedCarriageReturnsAndEmptyCharsInInitialiser_ReturnsMessageStringWithOneCarriageReturn()
        {
            // Arrange
            var sut = new PipeMessage("Hello\r\n\0World!\0\r\n");
            // Act
            var message = sut.GetMessageFromBuffer();
            // Assert
            Assert.AreEqual("Hello\r\n\0World!\r\n", message);
        }
    }
}