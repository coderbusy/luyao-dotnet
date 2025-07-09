using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;

namespace LuYao.Text.Tokenizer.CharacterFilters;


[TestClass]
public class RegexCharacterFilterTests
{
    [TestMethod]
    public void Filter_InputIsNull_ReturnsEmptyString()
    {
        // Arrange
        var regex = new Regex("[a-z]");
        var filter = new RegexCharacterFilter(regex, "*");

        // Act
        var result = filter.Filter(null);

        // Assert
        Assert.AreEqual(string.Empty, result);
    }

    [TestMethod]
    public void Filter_InputIsEmpty_ReturnsEmptyString()
    {
        // Arrange
        var regex = new Regex("[a-z]");
        var filter = new RegexCharacterFilter(regex, "*");

        // Act
        var result = filter.Filter(string.Empty);

        // Assert
        Assert.AreEqual(string.Empty, result);
    }

    [TestMethod]
    public void Filter_InputMatchesRegex_ReplacesMatchedCharacters()
    {
        // Arrange
        var regex = new Regex("[a-z]");
        var filter = new RegexCharacterFilter(regex, "*");

        // Act
        var result = filter.Filter("abc123");

        // Assert
        Assert.AreEqual("***123", result);
    }

    [TestMethod]
    public void Filter_InputDoesNotMatchRegex_ReturnsOriginalString()
    {
        // Arrange
        var regex = new Regex("[a-z]");
        var filter = new RegexCharacterFilter(regex, "*");

        // Act
        var result = filter.Filter("123");

        // Assert
        Assert.AreEqual("123", result);
    }

    [TestMethod]
    public void Filter_InputContainsWhitespace_ReplacesMatchedCharacters()
    {
        // Arrange
        var regex = new Regex("[a-z]");
        var filter = new RegexCharacterFilter(regex, "*");

        // Act
        var result = filter.Filter("a b c");

        // Assert
        Assert.AreEqual("* * *", result);
    }
}