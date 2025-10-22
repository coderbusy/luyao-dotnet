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
        Assert.ThrowsException<ArgumentNullException>(() => StringHelper.Truncate(null, 10));
    }

    [TestMethod]
    public void Truncate_ThrowsArgumentOutOfRangeException_WhenMaxLengthIsLessThanOne()
    {
        // Act & Assert
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => StringHelper.Truncate("test", 0));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => StringHelper.Truncate("test", -1));
    }

    [TestMethod]
    public void Truncate_ReturnsOriginalText_WhenTextLengthLessThanOrEqualMaxLength()
    {
        // Arrange
        var text = "Hello World";

        // Act
        var result = StringHelper.Truncate(text, 20);

        // Assert
        Assert.AreEqual(text, result.Truncated);
        Assert.AreEqual(string.Empty, result.Remaining);
        Assert.IsFalse(result.WasTruncated);
    }

    [TestMethod]
    public void Truncate_ReturnsOriginalText_WhenTextLengthEqualsMaxLength()
    {
        // Arrange
        var text = "Hello";

        // Act
        var result = StringHelper.Truncate(text, 5);

        // Assert
        Assert.AreEqual(text, result.Truncated);
        Assert.AreEqual(string.Empty, result.Remaining);
        Assert.IsFalse(result.WasTruncated);
    }

    [TestMethod]
    public void Truncate_TruncatesAtWordBoundary_ForEnglishText()
    {
        // Arrange
        var text = "Hello World Test";

        // Act
        var result = StringHelper.Truncate(text, 12);

        // Assert
        Assert.AreEqual("Hello World", result.Truncated);
        Assert.AreEqual(" Test", result.Remaining);
        Assert.IsTrue(result.WasTruncated);
    }

    [TestMethod]
    public void Truncate_TruncatesAtWordBoundary_WithMultipleSpaces()
    {
        // Arrange
        var text = "Hello   World   Test";

        // Act
        var result = StringHelper.Truncate(text, 12);

        // Assert
        Assert.AreEqual("Hello", result.Truncated);
        Assert.AreEqual("   World   Test", result.Remaining);
        Assert.IsTrue(result.WasTruncated);
    }

    [TestMethod]
    public void Truncate_HandlesChineseCharacters()
    {
        // Arrange
        var text = "你好世界，这是一个测试";

        // Act
        var result = StringHelper.Truncate(text, 5);

        // Assert
        Assert.AreEqual("你好世界，", result.Truncated);
        Assert.AreEqual("这是一个测试", result.Remaining);
        Assert.IsTrue(result.WasTruncated);
    }

    [TestMethod]
    public void Truncate_HandlesJapaneseCharacters()
    {
        // Arrange
        var text = "こんにちは世界です";

        // Act
        var result = StringHelper.Truncate(text, 5);

        // Assert
        Assert.AreEqual("こんにちは", result.Truncated);
        Assert.AreEqual("世界です", result.Remaining);
        Assert.IsTrue(result.WasTruncated);
    }

    [TestMethod]
    public void Truncate_HandlesKoreanCharacters()
    {
        // Arrange
        var text = "안녕하세요세계입니다";

        // Act
        var result = StringHelper.Truncate(text, 5);

        // Assert
        Assert.AreEqual("안녕하세요", result.Truncated);
        Assert.AreEqual("세계입니다", result.Remaining);
        Assert.IsTrue(result.WasTruncated);
    }

    [TestMethod]
    public void Truncate_HandlesMixedEnglishAndChinese()
    {
        // Arrange
        var text = "Hello 世界 Test 测试";

        // Act
        var result = StringHelper.Truncate(text, 10);

        // Assert
        Assert.AreEqual("Hello 世界", result.Truncated);
        Assert.AreEqual(" Test 测试", result.Remaining);
        Assert.IsTrue(result.WasTruncated);
    }

    [TestMethod]
    public void Truncate_HandlesMixedEnglishChineseAndNumbers()
    {
        // Arrange
        var text = "Product123 产品456 Item789";

        // Act
        var result = StringHelper.Truncate(text, 15);

        // Assert
        Assert.AreEqual("Product123 产品", result.Truncated);
        Assert.AreEqual("456 Item789", result.Remaining);
        Assert.IsTrue(result.WasTruncated);
    }

    [TestMethod]
    public void Truncate_HandlesEmptyString()
    {
        // Arrange
        var text = "";

        // Act
        var result = StringHelper.Truncate(text, 10);

        // Assert
        Assert.AreEqual(string.Empty, result.Truncated);
        Assert.AreEqual(string.Empty, result.Remaining);
        Assert.IsFalse(result.WasTruncated);
    }

    [TestMethod]
    public void Truncate_HandlesSingleWord()
    {
        // Arrange
        var text = "Hello";

        // Act
        var result = StringHelper.Truncate(text, 3);

        // Assert
        Assert.AreEqual("Hel", result.Truncated);
        Assert.AreEqual("lo", result.Remaining);
        Assert.IsTrue(result.WasTruncated);
    }

    [TestMethod]
    public void Truncate_HandlesPunctuationAsBoundary()
    {
        // Arrange
        var text = "Hello, World! Test.";

        // Act
        var result = StringHelper.Truncate(text, 14);

        // Assert
        Assert.AreEqual("Hello, World!", result.Truncated);
        Assert.AreEqual(" Test.", result.Remaining);
        Assert.IsTrue(result.WasTruncated);
    }

    [TestMethod]
    public void Truncate_HandlesChinesePunctuationAsBoundary()
    {
        // Arrange
        var text = "你好，世界！测试。";

        // Act
        var result = StringHelper.Truncate(text, 3);

        // Assert
        Assert.AreEqual("你好，", result.Truncated);
        Assert.AreEqual("世界！测试。", result.Remaining);
        Assert.IsTrue(result.WasTruncated);
    }

    [TestMethod]
    public void Truncate_WithSuffix_AppendsSuffixToTruncatedText()
    {
        // Arrange
        var text = "Hello World Test";

        // Act
        var result = StringHelper.Truncate(text, 9, "...");

        // Assert
        Assert.AreEqual("Hello...", result.Truncated);
        Assert.AreEqual(" World Test", result.Remaining);
        Assert.IsTrue(result.WasTruncated);
    }

    [TestMethod]
    public void Truncate_WithSuffix_AccountsForSuffixLength()
    {
        // Arrange
        var text = "你好世界测试";

        // Act
        var result = StringHelper.Truncate(text, 5, "...");

        // Assert
        Assert.AreEqual("你好...", result.Truncated);
        Assert.AreEqual("世界测试", result.Remaining);
        Assert.IsTrue(result.WasTruncated);
    }

    [TestMethod]
    public void Truncate_WithSuffix_HandlesNullSuffix()
    {
        // Arrange
        var text = "Hello World";

        // Act
        var result = StringHelper.Truncate(text, 8, null);

        // Assert
        Assert.AreEqual("Hello", result.Truncated);
        Assert.AreEqual(" World", result.Remaining);
        Assert.IsTrue(result.WasTruncated);
    }

    [TestMethod]
    public void Truncate_HandlesVeryLongText()
    {
        // Arrange
        var text = new string('A', 10000) + " " + new string('B', 10000);

        // Act
        var result = StringHelper.Truncate(text, 5000);

        // Assert
        Assert.IsTrue(result.Truncated.Length <= 5000);
        Assert.IsTrue(result.WasTruncated);
        Assert.IsTrue(result.Remaining.Length > 0);
    }

    [TestMethod]
    public void Truncate_HandlesTextWithSpaces()
    {
        // Arrange
        var text = "hello world test";

        // Act
        var result = StringHelper.Truncate(text, 13);

        // Assert
        Assert.AreEqual("hello world", result.Truncated);
        Assert.AreEqual(" test", result.Remaining);
        Assert.IsTrue(result.WasTruncated);
    }

    [TestMethod]
    public void Truncate_HandlesNumbersAndLetters()
    {
        // Arrange
        var text = "Test123 Hello456";

        // Act
        var result = StringHelper.Truncate(text, 10);

        // Assert
        Assert.AreEqual("Test123", result.Truncated);
        Assert.AreEqual(" Hello456", result.Remaining);
        Assert.IsTrue(result.WasTruncated);
    }

    [TestMethod]
    public void Truncate_HandlesSpecialCharacters()
    {
        // Arrange
        var text = "Hello@World#Test$End";

        // Act
        var result = StringHelper.Truncate(text, 12);

        // Assert
        Assert.AreEqual("Hello@World#", result.Truncated);
        Assert.AreEqual("Test$End", result.Remaining);
        Assert.IsTrue(result.WasTruncated);
    }

    [TestMethod]
    public void Truncate_HandlesFullWidthCharacters()
    {
        // Arrange
        var text = "ＨｅｌｌｏＷｏｒｌｄ";

        // Act
        var result = StringHelper.Truncate(text, 5);

        // Assert
        Assert.AreEqual("Ｈｅｌｌｏ", result.Truncated);
        Assert.AreEqual("Ｗｏｒｌｄ", result.Remaining);
        Assert.IsTrue(result.WasTruncated);
    }

    [TestMethod]
    public void Truncate_MaxLengthOne_ReturnsFirstCharacter()
    {
        // Arrange
        var text = "Hello World";

        // Act
        var result = StringHelper.Truncate(text, 1);

        // Assert
        Assert.AreEqual("H", result.Truncated);
        Assert.AreEqual("ello World", result.Remaining);
        Assert.IsTrue(result.WasTruncated);
    }

    [TestMethod]
    public void Truncate_WithLargeSuffix_StillTruncates()
    {
        // Arrange
        var text = "Hello World";

        // Act
        var result = StringHelper.Truncate(text, 8, ".....");

        // Assert
        Assert.AreEqual("Hel.....", result.Truncated);
        Assert.IsTrue(result.WasTruncated);
        Assert.IsTrue(result.Remaining.Length > 0);
    }

    [TestMethod]
    public void Truncate_ComplexMixedText()
    {
        // Arrange
        var text = "Product名称：iPhone14 Pro 价格：$999";

        // Act
        var result = StringHelper.Truncate(text, 20);

        // Assert
        Assert.IsTrue(result.Truncated.Length <= 20);
        Assert.IsTrue(result.WasTruncated);
        Assert.IsTrue(result.Remaining.Length > 0);
    }

    #endregion

    #region TruncateResult 结构测试

    [TestMethod]
    public void TruncateResult_Constructor_SetsProperties()
    {
        // Arrange & Act
        var result = new StringHelper.TruncateResult("truncated", "remaining", true);

        // Assert
        Assert.AreEqual("truncated", result.Truncated);
        Assert.AreEqual("remaining", result.Remaining);
        Assert.IsTrue(result.WasTruncated);
    }

    [TestMethod]
    public void TruncateResult_Constructor_HandlesEmptyStrings()
    {
        // Arrange & Act
        var result = new StringHelper.TruncateResult("", "", false);

        // Assert
        Assert.AreEqual(string.Empty, result.Truncated);
        Assert.AreEqual(string.Empty, result.Remaining);
        Assert.IsFalse(result.WasTruncated);
    }

    #endregion
}
