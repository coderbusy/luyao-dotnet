using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LuYao.Text.Tokenizer.CharacterFilters;

/// <summary>
/// 测试类，用于验证 <see cref="MergeMultipleSpacesCharacterFilter"/> 的行为。
/// </summary>
[TestClass]
public class MergeMultipleSpacesCharacterFilterTests
{
    /// <summary>
    /// 验证输入包含多个连续空格时，过滤器能够正确合并为单个空格。
    /// </summary>
    [TestMethod]
    public void Filter_InputWithMultipleSpaces_MergesToSingleSpace()
    {
        // Arrange
        var filter = new MergeMultipleSpacesCharacterFilter();
        var input = "This    is  a   test.";

        // Act
        var result = filter.Filter(input);

        // Assert
        Assert.AreEqual("This is a test.", result);
    }

    /// <summary>
    /// 验证输入为空字符串时，过滤器返回空字符串。
    /// </summary>
    [TestMethod]
    public void Filter_EmptyInput_ReturnsEmptyString()
    {
        // Arrange
        var filter = new MergeMultipleSpacesCharacterFilter();
        var input = string.Empty;

        // Act
        var result = filter.Filter(input);

        // Assert
        Assert.AreEqual(string.Empty, result);
    }

    /// <summary>
    /// 验证输入为 null 时，过滤器返回空字符串。
    /// </summary>
    [TestMethod]
    public void Filter_NullInput_ReturnsEmptyString()
    {
        // Arrange
        var filter = new MergeMultipleSpacesCharacterFilter();
        string input = null;

        // Act
        var result = filter.Filter(input);

        // Assert
        Assert.AreEqual(string.Empty, result);
    }

    /// <summary>
    /// 验证输入中没有连续空格时，过滤器返回原始文本。
    /// </summary>
    [TestMethod]
    public void Filter_InputWithoutMultipleSpaces_ReturnsOriginalText()
    {
        // Arrange
        var filter = new MergeMultipleSpacesCharacterFilter();
        var input = "This is a test.";

        // Act
        var result = filter.Filter(input);

        // Assert
        Assert.AreEqual("This is a test.", result);
    }

    /// <summary>
    /// 验证输入中包含各种空白字符时，过滤器能够正确合并为单个空格。
    /// </summary>
    [TestMethod]
    public void Filter_InputWithVariousWhitespace_MergesToSingleSpace()
    {
        // Arrange
        var filter = new MergeMultipleSpacesCharacterFilter();
        var input = "This\tis\n  a\r\n   test.";

        // Act
        var result = filter.Filter(input);

        // Assert
        Assert.AreEqual("This is a test.", result);
    }
}
