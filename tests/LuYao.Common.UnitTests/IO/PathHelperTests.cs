using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LuYao.IO;

/// <summary>
/// 测试 PathHelper 类的功能。
/// </summary>
[TestClass]
public class PathHelperTests
{
    [TestMethod]
    public void SafeFileName_ShouldRemoveInvalidCharacters()
    {
        // Arrange
        string input = "test|file:name?.txt";
        string expected = "testfilename.txt";

        // Act
        string result = PathHelper.SafeFileName(input);

        // Assert
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void SafeFileName_ShouldReturnEmptyForNullOrWhitespace()
    {
        // Arrange
        string input = "   ";
        string expected = string.Empty;

        // Act
        string result = PathHelper.SafeFileName(input);

        // Assert
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void SafeFileName_ShouldHandleValidFileName()
    {
        // Arrange
        string input = "valid_filename.txt";
        string expected = "valid_filename.txt";

        // Act
        string result = PathHelper.SafeFileName(input);

        // Assert
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void SafeFileName_ShouldHandleEmptyString()
    {
        // Arrange
        string input = string.Empty;
        string expected = string.Empty;

        // Act
        string result = PathHelper.SafeFileName(input);

        // Assert
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void GetMimeType_ShouldReturnCorrectMimeType_ForKnownExtensions()
    {
        // Arrange
        string extension = ".txt";
        string expectedMimeType = "text/plain";
        // Act
        string result = PathHelper.GetMimeType(extension);
        // Assert
        Assert.AreEqual(expectedMimeType, result);
    }

    [TestMethod]
    public void GetMimeType_ShouldThrowArgumentException_ForNullOrEmptyExtension()
    {
        // Arrange
        string extension = string.Empty;
        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() => PathHelper.GetMimeType(extension));
    }

    [TestMethod]
    public void GetMimeType_ShouldReturnDefaultMimeType_ForUnknownExtensions()
    {
        // Arrange
        string extension = ".unknown";
        string expectedMimeType = "application/octet-stream";
        // Act
        string result = PathHelper.GetMimeType(extension);
        // Assert
        Assert.AreEqual(expectedMimeType, result);
    }
}