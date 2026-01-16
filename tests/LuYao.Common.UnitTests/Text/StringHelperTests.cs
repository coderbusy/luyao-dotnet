using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace LuYao.Text;

[TestClass]
public class StringHelperTests
{
    #region Truncate 方法测试

    [TestMethod]
    public void Truncate_ThrowsArgumentNullException_WhenTextIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => StringHelper.Truncate(null, 10, out _));
    }

    [TestMethod]
    public void Truncate_ThrowsArgumentOutOfRangeException_WhenMaxLengthIsLessThanOne()
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => StringHelper.Truncate("test", 0, out _));
        Assert.Throws<ArgumentOutOfRangeException>(() => StringHelper.Truncate("test", -1, out _));
    }

    [TestMethod]
    public void Truncate_ReturnsOriginalText_WhenTextLengthLessThanOrEqualMaxLength()
    {
        // Arrange
        var text = "Hello World";

        // Act
        var truncated = StringHelper.Truncate(text, 20, out string remaining);

        // Assert
        Assert.AreEqual(text, truncated);
        Assert.AreEqual(string.Empty, remaining);
    }

    [TestMethod]
    public void Truncate_ReturnsOriginalText_WhenTextLengthEqualsMaxLength()
    {
        // Arrange
        var text = "Hello";

        // Act
        var truncated = StringHelper.Truncate(text, 5, out string remaining);

        // Assert
        Assert.AreEqual(text, truncated);
        Assert.AreEqual(string.Empty, remaining);
    }

    [TestMethod]
    public void Truncate_TruncatesAtWordBoundary_ForEnglishText()
    {
        // Arrange
        var text = "Hello World Test";

        // Act
        var truncated = StringHelper.Truncate(text, 12, out string remaining);

        // Assert
        Assert.AreEqual("Hello World", truncated);
        Assert.AreEqual(" Test", remaining);
    }

    [TestMethod]
    public void Truncate_TruncatesAtWordBoundary_WithMultipleSpaces()
    {
        // Arrange
        var text = "Hello   World   Test";

        // Act
        var truncated = StringHelper.Truncate(text, 12, out string remaining);

        // Assert
        Assert.AreEqual("Hello", truncated);
        Assert.AreEqual("   World   Test", remaining);
    }

    [TestMethod]
    public void Truncate_HandlesChineseCharacters()
    {
        // Arrange
        var text = "你好世界，这是一个测试";

        // Act
        var truncated = StringHelper.Truncate(text, 5, out string remaining);

        // Assert
        Assert.AreEqual("你好世界，", truncated);
        Assert.AreEqual("这是一个测试", remaining);
    }

    [TestMethod]
    public void Truncate_HandlesJapaneseCharacters()
    {
        // Arrange
        var text = "こんにちは世界です";

        // Act
        var truncated = StringHelper.Truncate(text, 5, out string remaining);

        // Assert
        Assert.AreEqual("こんにちは", truncated);
        Assert.AreEqual("世界です", remaining);
    }

    [TestMethod]
    public void Truncate_HandlesKoreanCharacters()
    {
        // Arrange
        var text = "안녕하세요세계입니다";

        // Act
        var truncated = StringHelper.Truncate(text, 5, out string remaining);

        // Assert
        Assert.AreEqual("안녕하세요", truncated);
        Assert.AreEqual("세계입니다", remaining);
    }

    [TestMethod]
    public void Truncate_HandlesMixedEnglishAndChinese()
    {
        // Arrange
        var text = "Hello 世界 Test 测试";

        // Act
        var truncated = StringHelper.Truncate(text, 10, out string remaining);

        // Assert
        Assert.AreEqual("Hello 世界", truncated);
        Assert.AreEqual(" Test 测试", remaining);
    }

    [TestMethod]
    public void Truncate_HandlesMixedEnglishChineseAndNumbers()
    {
        // Arrange
        var text = "Product123 产品456 Item789";

        // Act
        var truncated = StringHelper.Truncate(text, 15, out string remaining);

        // Assert
        Assert.AreEqual("Product123 产品", truncated);
        Assert.AreEqual("456 Item789", remaining);
    }

    [TestMethod]
    public void Truncate_HandlesEmptyString()
    {
        // Arrange
        var text = "";

        // Act
        var truncated = StringHelper.Truncate(text, 10, out string remaining);

        // Assert
        Assert.AreEqual(string.Empty, truncated);
        Assert.AreEqual(string.Empty, remaining);
    }

    [TestMethod]
    public void Truncate_HandlesSingleWord()
    {
        // Arrange
        var text = "Hello";

        // Act
        var truncated = StringHelper.Truncate(text, 3, out string remaining);

        // Assert
        Assert.AreEqual("Hel", truncated);
        Assert.AreEqual("lo", remaining);
    }

    [TestMethod]
    public void Truncate_HandlesPunctuationAsBoundary()
    {
        // Arrange
        var text = "Hello, World! Test.";

        // Act
        var truncated = StringHelper.Truncate(text, 14, out string remaining);

        // Assert
        Assert.AreEqual("Hello, World!", truncated);
        Assert.AreEqual(" Test.", remaining);
    }

    [TestMethod]
    public void Truncate_HandlesChinesePunctuationAsBoundary()
    {
        // Arrange
        var text = "你好，世界！测试。";

        // Act
        var truncated = StringHelper.Truncate(text, 3, out string remaining);

        // Assert
        Assert.AreEqual("你好，", truncated);
        Assert.AreEqual("世界！测试。", remaining);
    }

    [TestMethod]
    public void Truncate_HandlesTextWithSpaces()
    {
        // Arrange
        var text = "hello world test";

        // Act
        var truncated = StringHelper.Truncate(text, 13, out string remaining);

        // Assert
        Assert.AreEqual("hello world", truncated);
        Assert.AreEqual(" test", remaining);
    }

    [TestMethod]
    public void Truncate_HandlesVeryLongText()
    {
        // Arrange
        var text = new string('A', 10000) + " " + new string('B', 10000);

        // Act
        var truncated = StringHelper.Truncate(text, 5000, out string remaining);

        // Assert
        Assert.IsTrue(truncated.Length <= 5000);
        Assert.IsTrue(remaining.Length > 0);
    }

    [TestMethod]
    public void Truncate_HandlesNumbersAndLetters()
    {
        // Arrange
        var text = "Test123 Hello456";

        // Act
        var truncated = StringHelper.Truncate(text, 10, out string remaining);

        // Assert
        Assert.AreEqual("Test123", truncated);
        Assert.AreEqual(" Hello456", remaining);
    }

    [TestMethod]
    public void Truncate_HandlesSpecialCharacters()
    {
        // Arrange
        var text = "Hello@World#Test$End";

        // Act
        var truncated = StringHelper.Truncate(text, 12, out string remaining);

        // Assert
        Assert.AreEqual("Hello@World#", truncated);
        Assert.AreEqual("Test$End", remaining);
    }

    [TestMethod]
    public void Truncate_HandlesFullWidthCharacters()
    {
        // Arrange
        var text = "ＨｅｌｌｏＷｏｒｌｄ";

        // Act
        var truncated = StringHelper.Truncate(text, 5, out string remaining);

        // Assert
        Assert.AreEqual("Ｈｅｌｌｏ", truncated);
        Assert.AreEqual("Ｗｏｒｌｄ", remaining);
    }

    [TestMethod]
    public void Truncate_MaxLengthOne_ReturnsFirstCharacter()
    {
        // Arrange
        var text = "Hello World";

        // Act
        var truncated = StringHelper.Truncate(text, 1, out string remaining);

        // Assert
        Assert.AreEqual("H", truncated);
        Assert.AreEqual("ello World", remaining);
    }

    [TestMethod]
    public void Truncate_ComplexMixedText()
    {
        // Arrange
        var text = "Product名称：iPhone14 Pro 价格：$999";

        // Act
        var truncated = StringHelper.Truncate(text, 20, out string remaining);

        // Assert
        Assert.IsTrue(truncated.Length <= 20);
        Assert.IsTrue(remaining.Length > 0);
    }

    #endregion
}
