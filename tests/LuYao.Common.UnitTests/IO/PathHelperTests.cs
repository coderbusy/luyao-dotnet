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
}