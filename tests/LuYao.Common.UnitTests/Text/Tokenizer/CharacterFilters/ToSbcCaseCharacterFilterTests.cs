using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Text.Tokenizer.CharacterFilters;

[TestClass]
public class ToSbcCaseCharacterFilterTests
{
    [TestMethod]
    public void Filter_InputIsNull_ReturnsEmptyString()
    {
        // Arrange
        var filter = new ToSbcCaseCharacterFilter();

        // Act
        var result = filter.Filter(null);

        // Assert
        Assert.AreEqual(string.Empty, result);
    }

    [TestMethod]
    public void Filter_InputIsEmpty_ReturnsEmptyString()
    {
        // Arrange
        var filter = new ToSbcCaseCharacterFilter();

        // Act
        var result = filter.Filter(string.Empty);

        // Assert
        Assert.AreEqual(string.Empty, result);
    }

    [TestMethod]
    public void Filter_InputContainsHalfWidthSpace_ConvertsToFullWidthSpace()
    {
        // Arrange
        var filter = new ToSbcCaseCharacterFilter();

        // Act
        var result = filter.Filter(" ");

        // Assert
        Assert.AreEqual("　", result); // 全角空格
    }

    [TestMethod]
    public void Filter_InputContainsHalfWidthCharacters_ConvertsToFullWidthCharacters()
    {
        // Arrange
        var filter = new ToSbcCaseCharacterFilter();

        // Act
        var result = filter.Filter("!\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~");

        // Assert
        Assert.AreEqual("！＂＃＄％＆＇（）＊＋，－．／０１２３４５６７８９：；＜＝＞？＠ＡＢＣＤＥＦＧＨＩＪＫＬＭＮＯＰＱＲＳＴＵＶＷＸＹＺ［＼］＾＿｀ａｂｃｄｅｆｇｈｉｊｋｌｍｎｏｐｑｒｓｔｕｖｗｘｙｚ｛｜｝～", result);
    }

    [TestMethod]
    public void Filter_InputContainsMixedCharacters_ConvertsHalfWidthToFullWidthAndKeepsOthers()
    {
        // Arrange
        var filter = new ToSbcCaseCharacterFilter();

        // Act
        var result = filter.Filter("ABC123!@#中文");

        // Assert
        Assert.AreEqual("ＡＢＣ１２３！＠＃中文", result);
    }

    [TestMethod]
    public void Filter_InputContainsNoHalfWidthCharacters_ReturnsOriginalString()
    {
        // Arrange
        var filter = new ToSbcCaseCharacterFilter();

        // Act
        var result = filter.Filter("中文");

        // Assert
        Assert.AreEqual("中文", result);
    }
}