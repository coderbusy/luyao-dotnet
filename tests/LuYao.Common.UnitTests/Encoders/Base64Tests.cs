using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace LuYao.Encoders
{
    [TestClass]
    public class Base64Tests
    {
        [TestMethod]
        public void ToBase64_StandardEncoding_ReturnsCorrectBase64()
        {
            // Arrange
            var data = Encoding.UTF8.GetBytes("Hello, 世界!");
            var expected = Convert.ToBase64String(data);

            // Act
            var actual = Base64.ToBase64(data);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ToBase64_TrimPadding_ReturnsTrimmedBase64()
        {
            // Arrange
            var data = Encoding.UTF8.GetBytes("test"); // "dGVzdA=="
            var expected = "dGVzdA";

            // Act
            var actual = Base64.ToBase64(data, trim: true);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void FromBase64_StandardBase64_ReturnsOriginalBytes()
        {
            // Arrange
            var original = Encoding.UTF8.GetBytes("Hello, 世界!");
            var base64 = Convert.ToBase64String(original);

            // Act
            var result = Base64.FromBase64(base64);

            // Assert
            CollectionAssert.AreEqual(original, result);
        }

        [TestMethod]
        public void FromBase64_TrimmedBase64_ReturnsOriginalBytes()
        {
            // Arrange
            var original = Encoding.UTF8.GetBytes("test");
            var trimmedBase64 = "dGVzdA"; // 去掉了"=="

            // Act
            var result = Base64.FromBase64(trimmedBase64);

            // Assert
            CollectionAssert.AreEqual(original, result);
        }

        [TestMethod]
        public void FromBase64_EmptyString_ReturnsEmptyArray()
        {
            // Act
            var result = Base64.FromBase64(string.Empty);

            // Assert
            Assert.AreEqual(0, result.Length);
        }

        [TestMethod]
        public void FromBase64_InvalidString_ThrowsFormatException()
        {
            // Act & Assert
            Assert.Throws<FormatException>(() => Base64.FromBase64("!@#$"));
        }
    }
}