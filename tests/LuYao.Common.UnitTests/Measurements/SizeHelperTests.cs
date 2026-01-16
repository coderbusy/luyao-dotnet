using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace LuYao.Measurements;

/// <summary>
/// SizeHelper 的单元测试类。
/// </summary>
[TestClass]
public class SizeHelperTests
{
    /// <summary>
    /// 测试解析简单的无单位格式 "1x2x3"。
    /// </summary>
    [TestMethod]
    public void Extract_ShouldParseSimpleDimensions_WhenInputIs1x2x3()
    {
        // Arrange
        string input = "1x2x3";

        // Act
        var results = SizeHelper.Extract(input);

        // Assert
        Assert.AreEqual(1, results.Count);
        var result = results[0];
        Assert.AreEqual(DimensionUnit.Centimeter, result.Unit); // 默认为厘米
        Assert.AreEqual(3, result.Dimensions.Count);
        Assert.AreEqual(1m, result.Dimensions[0].Value);
        Assert.AreEqual(2m, result.Dimensions[1].Value);
        Assert.AreEqual(3m, result.Dimensions[2].Value);
        Assert.AreEqual(DimensionKind.Unspecified, result.Dimensions[0].Kind);
        Assert.AreEqual(DimensionKind.Unspecified, result.Dimensions[1].Kind);
        Assert.AreEqual(DimensionKind.Unspecified, result.Dimensions[2].Kind);
    }

    /// <summary>
    /// 测试解析带厘米单位的格式 "10cmx20cmx30cm"。
    /// </summary>
    [TestMethod]
    public void Extract_ShouldParseCentimeters_WhenInputIs10cmx20cmx30cm()
    {
        // Arrange
        string input = "10cmx20cmx30cm";

        // Act
        var results = SizeHelper.Extract(input);

        // Assert
        Assert.AreEqual(1, results.Count);
        var result = results[0];
        Assert.AreEqual(DimensionUnit.Centimeter, result.Unit);
        Assert.AreEqual(3, result.Dimensions.Count);
        Assert.AreEqual(10m, result.Dimensions[0].Value);
        Assert.AreEqual(20m, result.Dimensions[1].Value);
        Assert.AreEqual(30m, result.Dimensions[2].Value);
    }

    /// <summary>
    /// 测试解析带英寸标记的格式 "10''W X 36''H"。
    /// </summary>
    [TestMethod]
    public void Extract_ShouldParseInchesWithDimensionKinds_WhenInputIs10InchWx36InchH()
    {
        // Arrange
        string input = "10''W X 36''H";

        // Act
        var results = SizeHelper.Extract(input);

        // Assert
        Assert.AreEqual(1, results.Count);
        var result = results[0];
        Assert.AreEqual(DimensionUnit.Inch, result.Unit);
        Assert.AreEqual(2, result.Dimensions.Count);
        Assert.AreEqual(10m, result.Dimensions[0].Value);
        Assert.AreEqual(DimensionKind.Width, result.Dimensions[0].Kind);
        Assert.AreEqual(36m, result.Dimensions[1].Value);
        Assert.AreEqual(DimensionKind.Height, result.Dimensions[1].Kind);
    }

    /// <summary>
    /// 测试解析带括号的复合格式 "10cmx10cmx10cm(3.94x3.94x3.94in)"。
    /// </summary>
    [TestMethod]
    public void Extract_ShouldParseTwoResults_WhenInputHasParentheses()
    {
        // Arrange
        string input = "10cmx10cmx10cm(3.94x3.94x3.94in)";

        // Act
        var results = SizeHelper.Extract(input);

        // Assert
        Assert.AreEqual(2, results.Count);

        // 第一个结果：厘米
        var firstResult = results[0];
        Assert.AreEqual(DimensionUnit.Centimeter, firstResult.Unit);
        Assert.AreEqual(3, firstResult.Dimensions.Count);
        Assert.AreEqual(10m, firstResult.Dimensions[0].Value);
        Assert.AreEqual(10m, firstResult.Dimensions[1].Value);
        Assert.AreEqual(10m, firstResult.Dimensions[2].Value);

        // 第二个结果：英寸
        var secondResult = results[1];
        Assert.AreEqual(DimensionUnit.Inch, secondResult.Unit);
        Assert.AreEqual(3, secondResult.Dimensions.Count);
        Assert.AreEqual(3.94m, secondResult.Dimensions[0].Value);
        Assert.AreEqual(3.94m, secondResult.Dimensions[1].Value);
        Assert.AreEqual(3.94m, secondResult.Dimensions[2].Value);
    }

    /// <summary>
    /// 测试解析空字符串。
    /// </summary>
    [TestMethod]
    public void Extract_ShouldReturnEmptyList_WhenInputIsEmpty()
    {
        // Arrange
        string input = "";

        // Act
        var results = SizeHelper.Extract(input);

        // Assert
        Assert.AreEqual(0, results.Count);
    }

    /// <summary>
    /// 测试解析 null 字符串。
    /// </summary>
    [TestMethod]
    public void Extract_ShouldReturnEmptyList_WhenInputIsNull()
    {
        // Arrange
        string? input = null;

        // Act
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var results = SizeHelper.Extract(input);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        // Assert
        Assert.AreEqual(0, results.Count);
    }

    /// <summary>
    /// 测试解析带空格的格式 "10 cm x 20 cm x 30 cm"。
    /// </summary>
    [TestMethod]
    public void Extract_ShouldHandleSpaces_WhenInputHasSpaces()
    {
        // Arrange
        string input = "10 cm x 20 cm x 30 cm";

        // Act
        var results = SizeHelper.Extract(input);

        // Assert
        Assert.AreEqual(1, results.Count);
        var result = results[0];
        Assert.AreEqual(DimensionUnit.Centimeter, result.Unit);
        Assert.AreEqual(3, result.Dimensions.Count);
        Assert.AreEqual(10m, result.Dimensions[0].Value);
        Assert.AreEqual(20m, result.Dimensions[1].Value);
        Assert.AreEqual(30m, result.Dimensions[2].Value);
    }

    /// <summary>
    /// 测试解析带双引号的英寸格式 "10\"x20\"x30\""。
    /// </summary>
    [TestMethod]
    public void Extract_ShouldParseDoubleQuoteInches_WhenInputHasDoubleQuotes()
    {
        // Arrange
        string input = "10\"x20\"x30\"";

        // Act
        var results = SizeHelper.Extract(input);

        // Assert
        Assert.AreEqual(1, results.Count);
        var result = results[0];
        Assert.AreEqual(DimensionUnit.Inch, result.Unit);
        Assert.AreEqual(3, result.Dimensions.Count);
        Assert.AreEqual(10m, result.Dimensions[0].Value);
        Assert.AreEqual(20m, result.Dimensions[1].Value);
        Assert.AreEqual(30m, result.Dimensions[2].Value);
    }

    /// <summary>
    /// 测试解析小数尺寸 "10.5cmx20.5cmx30.5cm"。
    /// </summary>
    [TestMethod]
    public void Extract_ShouldParseDecimalValues_WhenInputHasDecimals()
    {
        // Arrange
        string input = "10.5cmx20.5cmx30.5cm";

        // Act
        var results = SizeHelper.Extract(input);

        // Assert
        Assert.AreEqual(1, results.Count);
        var result = results[0];
        Assert.AreEqual(DimensionUnit.Centimeter, result.Unit);
        Assert.AreEqual(3, result.Dimensions.Count);
        Assert.AreEqual(10.5m, result.Dimensions[0].Value);
        Assert.AreEqual(20.5m, result.Dimensions[1].Value);
        Assert.AreEqual(30.5m, result.Dimensions[2].Value);
    }

    /// <summary>
    /// 测试解析带有大写X的格式 "10cmX20cmX30cm"。
    /// </summary>
    [TestMethod]
    public void Extract_ShouldHandleUppercaseX_WhenInputHasUppercaseX()
    {
        // Arrange
        string input = "10cmX20cmX30cm";

        // Act
        var results = SizeHelper.Extract(input);

        // Assert
        Assert.AreEqual(1, results.Count);
        var result = results[0];
        Assert.AreEqual(DimensionUnit.Centimeter, result.Unit);
        Assert.AreEqual(3, result.Dimensions.Count);
        Assert.AreEqual(10m, result.Dimensions[0].Value);
        Assert.AreEqual(20m, result.Dimensions[1].Value);
        Assert.AreEqual(30m, result.Dimensions[2].Value);
    }

    /// <summary>
    /// 测试解析带单引号的英寸格式 "10'x20'x30'"。
    /// </summary>
    [TestMethod]
    public void Extract_ShouldParseSingleQuoteInches_WhenInputHasSingleQuotes()
    {
        // Arrange
        string input = "10'x20'x30'";

        // Act
        var results = SizeHelper.Extract(input);

        // Assert
        Assert.AreEqual(1, results.Count);
        var result = results[0];
        Assert.AreEqual(DimensionUnit.Inch, result.Unit);
        Assert.AreEqual(3, result.Dimensions.Count);
        Assert.AreEqual(10m, result.Dimensions[0].Value);
        Assert.AreEqual(20m, result.Dimensions[1].Value);
        Assert.AreEqual(30m, result.Dimensions[2].Value);
    }

    /// <summary>
    /// 测试解析带深度标记的格式 "10cmW x 20cmH x 30cmD"。
    /// </summary>
    [TestMethod]
    public void Extract_ShouldParseWidthHeightDepth_WhenInputHasWHDMarkers()
    {
        // Arrange
        string input = "10cmW x 20cmH x 30cmD";

        // Act
        var results = SizeHelper.Extract(input);

        // Assert
        Assert.AreEqual(1, results.Count);
        var result = results[0];
        Assert.AreEqual(DimensionUnit.Centimeter, result.Unit);
        Assert.AreEqual(3, result.Dimensions.Count);
        Assert.AreEqual(10m, result.Dimensions[0].Value);
        Assert.AreEqual(DimensionKind.Width, result.Dimensions[0].Kind);
        Assert.AreEqual(20m, result.Dimensions[1].Value);
        Assert.AreEqual(DimensionKind.Height, result.Dimensions[1].Kind);
        Assert.AreEqual(30m, result.Dimensions[2].Value);
        Assert.AreEqual(DimensionKind.Depth, result.Dimensions[2].Kind);
    }

    /// <summary>
    /// 测试解析两个尺寸的格式 "10cmx20cm"。
    /// </summary>
    [TestMethod]
    public void Extract_ShouldParseTwoDimensions_WhenInputHasTwoValues()
    {
        // Arrange
        string input = "10cmx20cm";

        // Act
        var results = SizeHelper.Extract(input);

        // Assert
        Assert.AreEqual(1, results.Count);
        var result = results[0];
        Assert.AreEqual(DimensionUnit.Centimeter, result.Unit);
        Assert.AreEqual(2, result.Dimensions.Count);
        Assert.AreEqual(10m, result.Dimensions[0].Value);
        Assert.AreEqual(20m, result.Dimensions[1].Value);
    }
}
