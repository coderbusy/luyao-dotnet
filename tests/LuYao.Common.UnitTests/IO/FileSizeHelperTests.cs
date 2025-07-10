using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LuYao.IO;

/// <summary>
/// FileSizeHelper 的单元测试类。
/// </summary>
[TestClass]
public class FileSizeHelperTests
{
    /// <summary>
    /// 测试文件大小为 0 时的显示结果。
    /// </summary>
    [TestMethod]
    public void GetDisplayName_ShouldReturnZeroByte_WhenFileSizeIsZero()
    {
        // Arrange
        long fileSize = 0;

        // Act
        string result = FileSizeHelper.GetDisplayName(fileSize);

        // Assert
        Assert.AreEqual("0 Byte", result);
    }

    /// <summary>
    /// 测试文件大小为 1024 时的显示结果。
    /// </summary>
    [TestMethod]
    public void GetDisplayName_ShouldReturn1KB_WhenFileSizeIs1024()
    {
        // Arrange
        long fileSize = 1024;

        // Act
        string result = FileSizeHelper.GetDisplayName(fileSize);

        // Assert
        Assert.AreEqual("1.00 KB", result);
    }

    /// <summary>
    /// 测试文件大小为 1048576 时的显示结果。
    /// </summary>
    [TestMethod]
    public void GetDisplayName_ShouldReturn1MB_WhenFileSizeIs1048576()
    {
        // Arrange
        long fileSize = 1048576;

        // Act
        string result = FileSizeHelper.GetDisplayName(fileSize);

        // Assert
        Assert.AreEqual("1.00 MB", result);
    }

    /// <summary>
    /// 测试文件大小为负数时的显示结果。
    /// </summary>
    [TestMethod]
    public void GetDisplayName_ShouldReturnZeroByte_WhenFileSizeIsNegative()
    {
        // Arrange
        long fileSize = -1;

        // Act
        string result = FileSizeHelper.GetDisplayName(fileSize);

        // Assert
        Assert.AreEqual("0 Byte", result);
    }

    /// <summary>
    /// 测试文件大小为 1125899906842624 时的显示结果。
    /// </summary>
    [TestMethod]
    public void GetDisplayName_ShouldReturn1PB_WhenFileSizeIs1125899906842624()
    {
        // Arrange
        long fileSize = 1125899906842624;

        // Act
        string result = FileSizeHelper.GetDisplayName(fileSize);

        // Assert
        Assert.AreEqual("1.00 PB", result);
    }
}