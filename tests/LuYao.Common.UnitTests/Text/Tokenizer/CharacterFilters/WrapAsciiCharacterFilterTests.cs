using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LuYao.Text.Tokenizer.CharacterFilters;

[TestClass]
public class WrapAsciiCharacterFilterTests
{
    [TestMethod]
    public void Filter_InputIsNull_ReturnsEmptyString()
    {
        // Arrange
        var filter = new WrapAsciiCharacterFilter();

        // Act
        var result = filter.Filter(null);

        // Assert
        Assert.AreEqual(string.Empty, result);
    }

    [TestMethod]
    public void Filter_InputIsEmpty_ReturnsEmptyString()
    {
        // Arrange
        var filter = new WrapAsciiCharacterFilter();

        // Act
        var result = filter.Filter(string.Empty);

        // Assert
        Assert.AreEqual(string.Empty, result);
    }

    [TestMethod]
    public void Filter_InputContainsAsciiCharacters_WrapsAsciiCharactersWithSpaces()
    {
        // Arrange
        var filter = new WrapAsciiCharacterFilter();

        // Act
        var result = filter.Filter("Hello123");

        // Assert
        Assert.AreEqual(" Hello123 ", result);
    }

    [TestMethod]
    public void Filter_InputContainsNonAsciiCharacters_KeepsNonAsciiCharactersUnchanged()
    {
        // Arrange
        var filter = new WrapAsciiCharacterFilter();

        // Act
        var result = filter.Filter("你好");

        // Assert
        Assert.AreEqual("你好", result);
    }

    [TestMethod]
    public void Filter_InputContainsMixedCharacters_WrapsAsciiCharactersAndKeepsNonAsciiCharacters()
    {
        // Arrange
        var filter = new WrapAsciiCharacterFilter();

        // Act
        var result = filter.Filter("Hello你好123");

        // Assert
        Assert.AreEqual(" Hello 你好 123 ", result);
    }

    [TestMethod]
    public void Filter_InputContainsWhitespaceOnly_ReturnsEmptyString()
    {
        // Arrange
        var filter = new WrapAsciiCharacterFilter();

        // Act
        var result = filter.Filter("   ");

        // Assert
        Assert.AreEqual(string.Empty, result);
    }
}