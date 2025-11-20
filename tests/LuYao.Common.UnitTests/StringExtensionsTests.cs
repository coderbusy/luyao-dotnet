using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Text.RegularExpressions;

namespace LuYao;

[TestClass]
public class StringExtensionsTests
{
    #region FindSlice 结构测试

    [TestMethod]
    public void FindSlice_Constructor_SetsStartAndEnd()
    {
        // Arrange & Act
        var slice = new StringExtensions.FindSlice(5, 10);

        // Assert
        Assert.AreEqual(5, slice.Start);
        Assert.AreEqual(10, slice.End);
    }

    [TestMethod]
    public void FindSlice_Success_ReturnsTrueForValidSlice()
    {
        // Arrange
        var validSlice = new StringExtensions.FindSlice(5, 10);
        var invalidSlice1 = new StringExtensions.FindSlice(5, 5);
        var invalidSlice2 = new StringExtensions.FindSlice(5, 0);

        // Act & Assert
        Assert.IsTrue(validSlice.Success);
        Assert.IsFalse(invalidSlice1.Success);
        Assert.IsFalse(invalidSlice2.Success);
    }

    [TestMethod]
    public void FindSlice_Contains_ChecksIfPositionIsInRange()
    {
        // Arrange
        var slice = new StringExtensions.FindSlice(5, 10);

        // Act & Assert
        Assert.IsTrue(slice.Contains(5));  // 边界值（开始）
        Assert.IsTrue(slice.Contains(7));  // 中间值
        Assert.IsTrue(slice.Contains(10)); // 边界值（结束）
        Assert.IsFalse(slice.Contains(4)); // 边界值之外
        Assert.IsFalse(slice.Contains(11)); // 边界值之外
    }

    #endregion

    #region FindAll 方法测试

    [TestMethod]
    public void FindAll_ReturnsCorrectSlices_WhenPatternExists()
    {
        // Arrange
        var html = "<div>内容1</div><div>内容2</div>";

        // Act
        var slices = html.FindAll("<div>", "</div>").ToArray();

        // Assert
        Assert.AreEqual(2, slices.Length);
        Assert.AreEqual(0, slices[0].Start);
        Assert.AreEqual(13, slices[0].End);
        Assert.AreEqual(14, slices[1].Start);
        Assert.AreEqual(27, slices[1].End);
    }

    [TestMethod]
    public void FindAll_ReturnsEmptyCollection_WhenPatternNotFound()
    {
        // Arrange
        var text = "没有匹配的内容";

        // Act
        var slices = text.FindAll("<div>", "</div>").ToArray();

        // Assert
        Assert.IsEmpty(slices);
    }

    [TestMethod]
    public void FindAll_ReturnsCorrectSlices_WithDifferentComparison()
    {
        // Arrange
        var html = "<DIV>内容1</DIV><div>内容2</div>";

        // Act - 默认忽略大小写
        var slicesIgnoreCase = html.FindAll("<div>", "</div>").ToArray();
        // 区分大小写
        var slicesCaseSensitive = html.FindAll("<div>", "</div>", StringComparison.Ordinal).ToArray();

        // Assert
        Assert.AreEqual(2, slicesIgnoreCase.Length);
        Assert.AreEqual(1, slicesCaseSensitive.Length);
        Assert.AreEqual(14, slicesCaseSensitive[0].Start);
    }

    #endregion

    #region Find 方法测试

    [TestMethod]
    public void Find_ReturnsFirstMatchingSlice_WhenPatternExists()
    {
        // Arrange
        var html = "<div>内容1</div><div>内容2</div>";

        // Act
        var slice = html.Find("<div>", "</div>");

        // Assert
        Assert.IsTrue(slice.Success);
        Assert.AreEqual(0, slice.Start);
        Assert.AreEqual(13, slice.End);
    }

    [TestMethod]
    public void Find_ReturnsDefaultSlice_WhenPatternNotFound()
    {
        // Arrange
        var text = "没有匹配的内容";

        // Act
        var slice = text.Find("<div>", "</div>");

        // Assert
        Assert.IsFalse(slice.Success);
        Assert.AreEqual(0, slice.Start);
        Assert.AreEqual(0, slice.End);
    }

    #endregion

    #region GetDeterministicHashCode 方法测试

    [TestMethod]
    public void GetDeterministicHashCode_ReturnsSameHashForSameString()
    {
        // Arrange
        var str1 = "测试字符串";
        var str2 = "测试字符串";

        // Act
        var hash1 = str1.GetDeterministicHashCode();
        var hash2 = str2.GetDeterministicHashCode();

        // Assert
        Assert.AreEqual(hash1, hash2);
    }

    [TestMethod]
    public void GetDeterministicHashCode_ReturnsDifferentHashForDifferentStrings()
    {
        // Arrange
        var str1 = "测试字符串1";
        var str2 = "测试字符串2";

        // Act
        var hash1 = str1.GetDeterministicHashCode();
        var hash2 = str2.GetDeterministicHashCode();

        // Assert
        Assert.AreNotEqual(hash1, hash2);
    }

    [TestMethod]
    public void GetDeterministicHashCode_HandlesEmptyString()
    {
        // Arrange
        var emptyStr = string.Empty;

        // Act
        var hash = emptyStr.GetDeterministicHashCode();

        // Assert
        Assert.AreEqual(757602046, hash); // 根据算法，空字符串哈希值为 5381 * 2
    }

    #endregion

    #region Slice 方法测试

    [TestMethod]
    public void Slice_ReturnsCorrectSubstring_WithPositiveIndices()
    {
        // Arrange
        var str = "Hello, world!";

        // Act
        var result = str.Slice(0, 5);

        // Assert
        Assert.AreEqual("Hello", result);
    }

    [TestMethod]
    public void Slice_ReturnsCorrectSubstring_WithNegativeStartIndex()
    {
        // Arrange
        var str = "Hello, world!";

        // Act
        var result = str.Slice(-6);

        // Assert
        Assert.AreEqual("world!", result);
    }

    [TestMethod]
    public void Slice_ReturnsCorrectSubstring_WithNegativeEndIndex()
    {
        // Arrange
        var str = "Hello, world!";

        // Act
        var result = str.Slice(7, -1);

        // Assert
        Assert.AreEqual("world", result);
    }

    [TestMethod]
    public void Slice_ReturnsEntireString_WhenEndIndexIsZero()
    {
        // Arrange
        var str = "Hello, world!";

        // Act
        var result = str.Slice(0);

        // Assert
        Assert.AreEqual("Hello, world!", result);
    }

    [TestMethod]
    public void Slice_HandlesBoundaryConditions()
    {
        // Arrange
        var str = "Hello";

        // Act
        var result1 = str.Slice(0, str.Length); // 全部
        var result2 = str.Slice(-str.Length); // 从头开始
        var result3 = str.Slice(str.Length - 1, str.Length); // 最后一个字符

        // Assert
        Assert.AreEqual("Hello", result1);
        Assert.AreEqual("Hello", result2);
        Assert.AreEqual("o", result3);
    }

    #endregion
}
