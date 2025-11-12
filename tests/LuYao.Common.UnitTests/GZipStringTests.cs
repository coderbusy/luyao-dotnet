using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace LuYao;

[TestClass]
public class GZipStringTests
{
    private const string TestString = "����һ�������ַ��� This is a test string 12345!@#$%";

    [TestMethod]
    public void Compress_WithValidInputAndGzipBase64_ReturnsCompressedString()
    {
        // Arrange
        string input = TestString;
        string compressor = "gzip";
        string encoder = "base64";

        // Act
        string result = GZipString.Compress(input, compressor, encoder);

        // Assert
        Assert.IsTrue(result.StartsWith($"data:text/x-{compressor};{encoder},"));
        Assert.AreNotEqual(input, result);
    }

    [TestMethod]
    public void Compress_WithEmptyString_ReturnsEmptyString()
    {
        // Arrange
        string input = string.Empty;
        string compressor = "gzip";
        string encoder = "base64";

        // Act
        string result = GZipString.Compress(input, compressor, encoder);

        // Assert
        Assert.AreEqual(string.Empty, result);
    }

    [TestMethod]
    public void Compress_WithAlreadyCompressedString_ReturnsSameString()
    {
        // Arrange
        string originalString = TestString;
        string compressedString = GZipString.Compress(originalString, "gzip", "base64");

        // Act
        string result = GZipString.Compress(compressedString, "gzip", "base64");

        // Assert
        Assert.AreEqual(compressedString, result);
    }

    [TestMethod]
    public void Compress_WithNullCompressor_ThrowsArgumentNullException()
    {
        // Arrange
        string input = TestString;
        string compressor = null;
        string encoder = "base64";

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => GZipString.Compress(input, compressor, encoder));
    }

    [TestMethod]
    public void Compress_WithNullEncoder_ThrowsArgumentNullException()
    {
        // Arrange
        string input = TestString;
        string compressor = "gzip";
        string encoder = null;

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => GZipString.Compress(input, compressor, encoder));
    }

    [TestMethod]
    public void Compress_WithInvalidCompressor_ThrowsKeyNotFoundException()
    {
        // Arrange
        string input = TestString;
        string compressor = "invalid";
        string encoder = "base64";

        // Act & Assert
        Assert.ThrowsExactly<KeyNotFoundException>(() => GZipString.Compress(input, compressor, encoder));
    }

    [TestMethod]
    public void Compress_WithInvalidEncoder_ThrowsKeyNotFoundException()
    {
        // Arrange
        string input = TestString;
        string compressor = "gzip";
        string encoder = "invalid";

        // Act & Assert
        Assert.ThrowsExactly<KeyNotFoundException>(() => GZipString.Compress(input, compressor, encoder));
    }

    [TestMethod]
    public void Compress_WithInterfaceImplementation_ReturnsCompressedString()
    {
        // Arrange
        string input = TestString;
        var compressor = GZipString.GZip;
        var encoder = GZipString.Base64;

        // Act
        string result = GZipString.Compress(input, compressor, encoder);

        // Assert
        Assert.IsTrue(result.StartsWith($"data:text/x-{compressor.Identifier};{encoder.Identifier},"));
        Assert.AreNotEqual(input, result);
    }

    [TestMethod]
    public void Compress_WithNullCompressorInterface_ThrowsArgumentNullException()
    {
        // Arrange
        string input = TestString;
        GZipString.ICompressor compressor = null;
        var encoder = GZipString.Base64;

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => GZipString.Compress(input, compressor, encoder));
    }

    [TestMethod]
    public void Compress_WithNullEncoderInterface_ThrowsArgumentNullException()
    {
        // Arrange
        string input = TestString;
        var compressor = GZipString.GZip;
        GZipString.IEncoder encoder = null;

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => GZipString.Compress(input, compressor, encoder));
    }

    [TestMethod]
    public void Decompress_WithCompressedString_ReturnsOriginalString()
    {
        // Arrange
        string originalString = TestString;
        string compressedString = GZipString.Compress(originalString, "gzip", "base64");

        // Act
        string result = GZipString.Decompress(compressedString);

        // Assert
        Assert.AreEqual(originalString, result);
    }

    [TestMethod]
    public void Decompress_WithEmptyString_ReturnsEmptyString()
    {
        // Act
        string result = GZipString.Decompress(string.Empty);

        // Assert
        Assert.AreEqual(string.Empty, result);
    }

    [TestMethod]
    public void Decompress_WithNonCompressedString_ReturnsSameString()
    {
        // Arrange
        string input = TestString;

        // Act
        string result = GZipString.Decompress(input);

        // Assert
        Assert.AreEqual(input, result);
    }

    [TestMethod]
    public void Decompress_WithDifferentCompressorAndEncoder_ReturnsOriginalString()
    {
        // Arrange - Test different compressor/encoder combinations
        string originalString = TestString;
        
        // Test with LzString and Base62
        string compressed1 = GZipString.Compress(originalString, "lzstring", "base62");
        
        // Test with Deflate and Base32
        string compressed2 = GZipString.Compress(originalString, "deflate", "base32");
        
        // Test with GZip and Ascii85
        string compressed3 = GZipString.Compress(originalString, "gzip", "ascii85");

        // Act
        string result1 = GZipString.Decompress(compressed1);
        string result2 = GZipString.Decompress(compressed2);
        string result3 = GZipString.Decompress(compressed3);

        // Assert
        Assert.AreEqual(originalString, result1);
        Assert.AreEqual(originalString, result2);
        Assert.AreEqual(originalString, result3);
    }

    [TestMethod]
    public void Decompress_WithInvalidCompressorIdentifier_ThrowsKeyNotFoundException()
    {
        // Arrange
        string invalidCompressedString = "data:text/x-invalid;base64,ABCDEF";

        // Act & Assert
        Assert.ThrowsExactly<KeyNotFoundException>(() => GZipString.Decompress(invalidCompressedString));
    }

    [TestMethod]
    public void Decompress_WithInvalidEncoderIdentifier_ThrowsKeyNotFoundException()
    {
        // Arrange
        string invalidCompressedString = "data:text/x-gzip;invalid,ABCDEF";

        // Act & Assert
        Assert.ThrowsExactly<KeyNotFoundException>(() => GZipString.Decompress(invalidCompressedString));
    }
}