using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Text.Tokenizer.CharacterFilters;

[TestClass]
public class ToDbcCaseCharacterFilterTests
{
    [TestMethod]
    public void Filter_InputIsNull_ReturnsEmptyString()
    {
        // Arrange
        var filter = new ToDbcCaseCharacterFilter();

        // Act
        var result = filter.Filter(null);

        // Assert
        Assert.AreEqual(string.Empty, result);
    }

    [TestMethod]
    public void Filter_InputIsEmpty_ReturnsEmptyString()
    {
        // Arrange
        var filter = new ToDbcCaseCharacterFilter();

        // Act
        var result = filter.Filter(string.Empty);

        // Assert
        Assert.AreEqual(string.Empty, result);
    }

    [TestMethod]
    public void Filter_InputContainsFullWidthSpace_ConvertsToHalfWidthSpace()
    {
        // Arrange
        var filter = new ToDbcCaseCharacterFilter();

        // Act
        var result = filter.Filter("　");

        // Assert
        Assert.AreEqual(" ", result);
    }

    [TestMethod]
    public void Filter_InputContainsFullWidthCharacters_ConvertsToHalfWidthCharacters()
    {
        // Arrange
        var filter = new ToDbcCaseCharacterFilter();

        // Act
        var result = filter.Filter("！＂＃＄％＆＇（）＊＋，－．／０１２３４５６７８９：；＜＝＞？＠ＡＢＣＤＥＦＧＨＩＪＫＬＭＮＯＰＱＲＳＴＵＶＷＸＹＺ［＼］＾＿｀ａｂｃｄｅｆｇｈｉｊｋｌｍｎｏｐｑｒｓｔｕｖｗｘｙｚ｛｜｝～");

        // Assert
        Assert.AreEqual("!\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~", result);
    }

    [TestMethod]
    public void Filter_InputContainsMixedCharacters_ConvertsFullWidthToHalfWidthAndKeepsOthers()
    {
        // Arrange
        var filter = new ToDbcCaseCharacterFilter();

        // Act
        var result = filter.Filter("ＡＢＣ123！＠＃");

        // Assert
        Assert.AreEqual("ABC123!@#", result);
    }

    [TestMethod]
    public void Filter_InputContainsNoFullWidthCharacters_ReturnsOriginalString()
    {
        // Arrange
        var filter = new ToDbcCaseCharacterFilter();

        // Act
        var result = filter.Filter("ABC123!@#");

        // Assert
        Assert.AreEqual("ABC123!@#", result);
    }
}