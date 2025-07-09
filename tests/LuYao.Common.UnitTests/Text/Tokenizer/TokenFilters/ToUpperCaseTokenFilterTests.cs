using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace LuYao.Text.Tokenizer.TokenFilters;

[TestClass]
public class ToUpperCaseTokenFilterTests
{
    [TestMethod]
    public void Filter_InputIsNull_ReturnsEmptyEnumerable()
    {
        // Arrange
        var filter = new ToUpperCaseTokenFilter();

        // Act
        var result = filter.Filter(null);

        // Assert
        Assert.IsFalse(result.Any());
    }

    [TestMethod]
    public void Filter_InputIsEmpty_ReturnsEmptyEnumerable()
    {
        // Arrange
        var filter = new ToUpperCaseTokenFilter();

        // Act
        var result = filter.Filter(string.Empty);

        // Assert
        Assert.IsFalse(result.Any());
    }

    [TestMethod]
    public void Filter_InputIsWhitespace_ReturnsEmptyEnumerable()
    {
        // Arrange
        var filter = new ToUpperCaseTokenFilter();

        // Act
        var result = filter.Filter("   ");

        // Assert
        Assert.IsFalse(result.Any());
    }

    [TestMethod]
    public void Filter_InputIsLowercase_ReturnsUppercaseEnumerable()
    {
        // Arrange
        var filter = new ToUpperCaseTokenFilter();

        // Act
        var result = filter.Filter("hello");

        // Assert
        Assert.AreEqual(1, result.Count());
        Assert.AreEqual("HELLO", result.First());
    }

    [TestMethod]
    public void Filter_InputIsMixedCase_ReturnsUppercaseEnumerable()
    {
        // Arrange
        var filter = new ToUpperCaseTokenFilter();

        // Act
        var result = filter.Filter("HeLLo");

        // Assert
        Assert.AreEqual(1, result.Count());
        Assert.AreEqual("HELLO", result.First());
    }

    [TestMethod]
    public void Filter_InputIsUppercase_ReturnsSameUppercaseEnumerable()
    {
        // Arrange
        var filter = new ToUpperCaseTokenFilter();

        // Act
        var result = filter.Filter("HELLO");

        // Assert
        Assert.AreEqual(1, result.Count());
        Assert.AreEqual("HELLO", result.First());
    }
}