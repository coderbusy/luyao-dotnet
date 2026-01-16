using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LuYao.Globalization;

/// <summary>
/// SizeHelper 的单元测试类
/// </summary>
[TestClass]
public class SizeHelperTests
{
    /// <summary>
    /// 测试单一单位格式：10x10x10cm，期望返回 [10, 10, 10]
    /// </summary>
    [TestMethod]
    public void ExtractSize_SingleUnitCm_ReturnsCorrectArray()
    {
        var result = SizeHelper.ExtractSize("10x10x10cm", out decimal[] arr);

        Assert.IsTrue(result);
        Assert.AreEqual(3, arr.Length);
        Assert.AreEqual(10m, arr[0]);
        Assert.AreEqual(10m, arr[1]);
        Assert.AreEqual(10m, arr[2]);
    }

    /// <summary>
    /// 测试多个统一单位格式：10cmx10cmx10cm，期望返回 [10, 10, 10]
    /// </summary>
    [TestMethod]
    public void ExtractSize_MultipleUniformUnitsCm_ReturnsCorrectArray()
    {
        var result = SizeHelper.ExtractSize("10cmx10cmx10cm", out decimal[] arr);

        Assert.IsTrue(result);
        Assert.AreEqual(3, arr.Length);
        Assert.AreEqual(10m, arr[0]);
        Assert.AreEqual(10m, arr[1]);
        Assert.AreEqual(10m, arr[2]);
    }

    /// <summary>
    /// 测试多个不统一单位：10cmx5inx10m，期望返回转换为厘米的值
    /// </summary>
    [TestMethod]
    public void ExtractSize_MultipleNonUniformUnits_ReturnsCorrectArray()
    {
        var result = SizeHelper.ExtractSize("10cmx5inx10m", out decimal[] arr);

        Assert.IsTrue(result);
        Assert.AreEqual(3, arr.Length);
        Assert.AreEqual(10m, arr[0]); // 10 cm
        Assert.AreEqual(12.7m, arr[1]); // 5 inch = 12.7 cm
        Assert.AreEqual(1000m, arr[2]); // 10 m = 1000 cm
    }

    /// <summary>
    /// 测试多组不同单位：10cmx10cmx10cm(3.94x3.94x3.94in)
    /// 期望返回 [10, 10, 10, 10.0076, 10.0076, 10.0076]
    /// </summary>
    [TestMethod]
    public void ExtractSize_MultipleGroupsWithDifferentUnits_ReturnsCorrectArray()
    {
        var result = SizeHelper.ExtractSize("10cmx10cmx10cm(3.94x3.94x3.94in)", out decimal[] arr);

        Assert.IsTrue(result);
        Assert.AreEqual(6, arr.Length);
        Assert.AreEqual(10m, arr[0]);
        Assert.AreEqual(10m, arr[1]);
        Assert.AreEqual(10m, arr[2]);
        Assert.AreEqual(10.0076m, arr[3]); // 3.94 inch = 10.0076 cm
        Assert.AreEqual(10.0076m, arr[4]);
        Assert.AreEqual(10.0076m, arr[5]);
    }

    /// <summary>
    /// 测试忽略不支持的文字：尺寸(cm)：10x10x10，期望返回 [10, 10, 10]
    /// </summary>
    [TestMethod]
    public void ExtractSize_IgnoreUnsupportedText_ReturnsCorrectArray()
    {
        var result = SizeHelper.ExtractSize("尺寸(cm)：10x10x10", out decimal[] arr);

        Assert.IsTrue(result);
        Assert.AreEqual(3, arr.Length);
        Assert.AreEqual(10m, arr[0]);
        Assert.AreEqual(10m, arr[1]);
        Assert.AreEqual(10m, arr[2]);
    }

    /// <summary>
    /// 测试默认单位为厘米：10x10x10（无单位），期望返回 [10, 10, 10]
    /// </summary>
    [TestMethod]
    public void ExtractSize_NoUnitSpecified_DefaultsToCm()
    {
        var result = SizeHelper.ExtractSize("10x10x10", out decimal[] arr);

        Assert.IsTrue(result);
        Assert.AreEqual(3, arr.Length);
        Assert.AreEqual(10m, arr[0]);
        Assert.AreEqual(10m, arr[1]);
        Assert.AreEqual(10m, arr[2]);
    }

    /// <summary>
    /// 测试小数输入：10.5x20.3x30.7cm，期望返回小数值
    /// </summary>
    [TestMethod]
    public void ExtractSize_DecimalInput_ReturnsCorrectArray()
    {
        var result = SizeHelper.ExtractSize("10.5x20.3x30.7cm", out decimal[] arr);

        Assert.IsTrue(result);
        Assert.AreEqual(3, arr.Length);
        Assert.AreEqual(10.5m, arr[0]);
        Assert.AreEqual(20.3m, arr[1]);
        Assert.AreEqual(30.7m, arr[2]);
    }

    /// <summary>
    /// 测试使用 * 作为分隔符：10*20*30cm，期望返回 [10, 20, 30]
    /// </summary>
    [TestMethod]
    public void ExtractSize_AsteriskSeparator_ReturnsCorrectArray()
    {
        var result = SizeHelper.ExtractSize("10*20*30cm", out decimal[] arr);

        Assert.IsTrue(result);
        Assert.AreEqual(3, arr.Length);
        Assert.AreEqual(10m, arr[0]);
        Assert.AreEqual(20m, arr[1]);
        Assert.AreEqual(30m, arr[2]);
    }

    /// <summary>
    /// 测试英寸单位（IN）：10x10x10in，期望返回 [25.4, 25.4, 25.4]
    /// </summary>
    [TestMethod]
    public void ExtractSize_InchUnit_ReturnsCorrectArray()
    {
        var result = SizeHelper.ExtractSize("10x10x10in", out decimal[] arr);

        Assert.IsTrue(result);
        Assert.AreEqual(3, arr.Length);
        Assert.AreEqual(25.4m, arr[0]); // 10 inch = 25.4 cm
        Assert.AreEqual(25.4m, arr[1]);
        Assert.AreEqual(25.4m, arr[2]);
    }

    /// <summary>
    /// 测试英寸单位（INCH）：10x10x10inch，期望返回 [25.4, 25.4, 25.4]
    /// </summary>
    [TestMethod]
    public void ExtractSize_InchUnitLongForm_ReturnsCorrectArray()
    {
        var result = SizeHelper.ExtractSize("10x10x10inch", out decimal[] arr);

        Assert.IsTrue(result);
        Assert.AreEqual(3, arr.Length);
        Assert.AreEqual(25.4m, arr[0]); // 10 inch = 25.4 cm
        Assert.AreEqual(25.4m, arr[1]);
        Assert.AreEqual(25.4m, arr[2]);
    }

    /// <summary>
    /// 测试毫米单位：100x100x100mm，期望返回 [10, 10, 10]
    /// </summary>
    [TestMethod]
    public void ExtractSize_MillimeterUnit_ReturnsCorrectArray()
    {
        var result = SizeHelper.ExtractSize("100x100x100mm", out decimal[] arr);

        Assert.IsTrue(result);
        Assert.AreEqual(3, arr.Length);
        Assert.AreEqual(10m, arr[0]); // 100 mm = 10 cm
        Assert.AreEqual(10m, arr[1]);
        Assert.AreEqual(10m, arr[2]);
    }

    /// <summary>
    /// 测试分米单位：1x2x3dm，期望返回 [10, 20, 30]
    /// </summary>
    [TestMethod]
    public void ExtractSize_DecimeterUnit_ReturnsCorrectArray()
    {
        var result = SizeHelper.ExtractSize("1x2x3dm", out decimal[] arr);

        Assert.IsTrue(result);
        Assert.AreEqual(3, arr.Length);
        Assert.AreEqual(10m, arr[0]); // 1 dm = 10 cm
        Assert.AreEqual(20m, arr[1]); // 2 dm = 20 cm
        Assert.AreEqual(30m, arr[2]); // 3 dm = 30 cm
    }

    /// <summary>
    /// 测试米单位：1x2x3m，期望返回 [100, 200, 300]
    /// </summary>
    [TestMethod]
    public void ExtractSize_MeterUnit_ReturnsCorrectArray()
    {
        var result = SizeHelper.ExtractSize("1x2x3m", out decimal[] arr);

        Assert.IsTrue(result);
        Assert.AreEqual(3, arr.Length);
        Assert.AreEqual(100m, arr[0]); // 1 m = 100 cm
        Assert.AreEqual(200m, arr[1]); // 2 m = 200 cm
        Assert.AreEqual(300m, arr[2]); // 3 m = 300 cm
    }

    /// <summary>
    /// 测试空字符串输入，期望返回 false 和空数组
    /// </summary>
    [TestMethod]
    public void ExtractSize_EmptyString_ReturnsFalse()
    {
        var result = SizeHelper.ExtractSize("", out decimal[] arr);

        Assert.IsFalse(result);
        Assert.AreEqual(0, arr.Length);
    }

    /// <summary>
    /// 测试 null 输入，期望返回 false 和空数组
    /// </summary>
    [TestMethod]
    public void ExtractSize_NullString_ReturnsFalse()
    {
        var result = SizeHelper.ExtractSize(null, out decimal[] arr);

        Assert.IsFalse(result);
        Assert.AreEqual(0, arr.Length);
    }

    /// <summary>
    /// 测试单一数值输入（带厘米单位），期望返回单个元素的数组
    /// </summary>
    [TestMethod]
    public void ExtractSize_SingleValueWithCm_ReturnsCorrectArray()
    {
        var result = SizeHelper.ExtractSize("50cm", out decimal[] arr);

        Assert.IsTrue(result);
        Assert.AreEqual(1, arr.Length);
        Assert.AreEqual(50m, arr[0]);
    }

    /// <summary>
    /// 测试单一数值输入（带英寸单位），期望返回转换为厘米的值
    /// </summary>
    [TestMethod]
    public void ExtractSize_SingleValueWithInch_ReturnsCorrectArray()
    {
        var result = SizeHelper.ExtractSize("10in", out decimal[] arr);

        Assert.IsTrue(result);
        Assert.AreEqual(1, arr.Length);
        Assert.AreEqual(25.4m, arr[0]); // 10 inch = 25.4 cm
    }

    /// <summary>
    /// 测试单一数值输入（带毫米单位），期望返回转换为厘米的值
    /// </summary>
    [TestMethod]
    public void ExtractSize_SingleValueWithMm_ReturnsCorrectArray()
    {
        var result = SizeHelper.ExtractSize("100mm", out decimal[] arr);

        Assert.IsTrue(result);
        Assert.AreEqual(1, arr.Length);
        Assert.AreEqual(10m, arr[0]); // 100 mm = 10 cm
    }

    /// <summary>
    /// 测试单一数值输入（不带单位），期望返回 false（必须带单位）
    /// </summary>
    [TestMethod]
    public void ExtractSize_SingleValueWithoutUnit_ReturnsFalse()
    {
        var result = SizeHelper.ExtractSize("30", out decimal[] arr);

        Assert.IsFalse(result);
        Assert.AreEqual(0, arr.Length);
    }

    /// <summary>
    /// 测试单一小数数值输入，期望返回小数值
    /// </summary>
    [TestMethod]
    public void ExtractSize_SingleDecimalValue_ReturnsCorrectArray()
    {
        var result = SizeHelper.ExtractSize("25.5cm", out decimal[] arr);

        Assert.IsTrue(result);
        Assert.AreEqual(1, arr.Length);
        Assert.AreEqual(25.5m, arr[0]);
    }

    /// <summary>
    /// 测试仅包含文字没有数字的字符串，期望返回 false 或空数组
    /// </summary>
    [TestMethod]
    public void ExtractSize_OnlyText_ReturnsFalseOrEmpty()
    {
        var result = SizeHelper.ExtractSize("abc x def", out decimal[] arr);

        // Either returns false or returns true with empty array
        if (result)
        {
            Assert.AreEqual(0, arr.Length);
        }
        else
        {
            Assert.IsFalse(result);
        }
    }

    /// <summary>
    /// 测试大小写不敏感：10X10X10CM，期望返回 [10, 10, 10]
    /// </summary>
    [TestMethod]
    public void ExtractSize_CaseInsensitive_ReturnsCorrectArray()
    {
        var result = SizeHelper.ExtractSize("10X10X10CM", out decimal[] arr);

        Assert.IsTrue(result);
        Assert.AreEqual(3, arr.Length);
        Assert.AreEqual(10m, arr[0]);
        Assert.AreEqual(10m, arr[1]);
        Assert.AreEqual(10m, arr[2]);
    }

    /// <summary>
    /// 测试混合大小写和空格：10 x 20 X 30 Cm，期望返回 [10, 20, 30]
    /// </summary>
    [TestMethod]
    public void ExtractSize_MixedCaseWithSpaces_ReturnsCorrectArray()
    {
        var result = SizeHelper.ExtractSize("10 x 20 X 30 Cm", out decimal[] arr);

        Assert.IsTrue(result);
        Assert.AreEqual(3, arr.Length);
        Assert.AreEqual(10m, arr[0]);
        Assert.AreEqual(20m, arr[1]);
        Assert.AreEqual(30m, arr[2]);
    }

    /// <summary>
    /// 测试多个括号组：10x20cm(5x6in)(7x8mm)
    /// </summary>
    [TestMethod]
    public void ExtractSize_MultipleParenthesesGroups_ReturnsCorrectArray()
    {
        var result = SizeHelper.ExtractSize("10x20cm(5x6in)(7x8mm)", out decimal[] arr);

        Assert.IsTrue(result);
        Assert.AreEqual(6, arr.Length);
        Assert.AreEqual(10m, arr[0]);
        Assert.AreEqual(20m, arr[1]);
        Assert.AreEqual(12.7m, arr[2]); // 5 inch = 12.7 cm
        Assert.AreEqual(15.24m, arr[3]); // 6 inch = 15.24 cm
        Assert.AreEqual(0.7m, arr[4]); // 7 mm = 0.7 cm
        Assert.AreEqual(0.8m, arr[5]); // 8 mm = 0.8 cm
    }

    /// <summary>
    /// 测试每个值带单位的情况：10mm x 5cm x 2in
    /// </summary>
    [TestMethod]
    public void ExtractSize_EachValueWithUnit_ReturnsCorrectArray()
    {
        var result = SizeHelper.ExtractSize("10mmx5cmx2in", out decimal[] arr);

        Assert.IsTrue(result);
        Assert.AreEqual(3, arr.Length);
        Assert.AreEqual(1m, arr[0]); // 10 mm = 1 cm
        Assert.AreEqual(5m, arr[1]); // 5 cm = 5 cm
        Assert.AreEqual(5.08m, arr[2]); // 2 inch = 5.08 cm
    }

    /// <summary>
    /// 测试单一数值带米单位，期望成功解析
    /// </summary>
    [TestMethod]
    public void ExtractSize_SingleValueWithMeterUnit_ReturnsCorrectValue()
    {
        var result = SizeHelper.ExtractSize("1m", out decimal[] arr);

        Assert.IsTrue(result);
        Assert.AreEqual(1, arr.Length);
        Assert.AreEqual(100m, arr[0]); // 1 m = 100 cm
    }

    /// <summary>
    /// 测试单一小数数值带英寸单位（in），期望成功解析
    /// </summary>
    [TestMethod]
    public void ExtractSize_SingleDecimalValueWithInchUnit_ReturnsCorrectValue()
    {
        var result = SizeHelper.ExtractSize("1.1in", out decimal[] arr);

        Assert.IsTrue(result);
        Assert.AreEqual(1, arr.Length);
        Assert.AreEqual(2.794m, arr[0]); // 1.1 inch = 2.794 cm
    }

    /// <summary>
    /// 测试单一小数数值带英寸单位（inch），期望成功解析
    /// </summary>
    [TestMethod]
    public void ExtractSize_SingleDecimalValueWithInchUnitLongForm_ReturnsCorrectValue()
    {
        var result = SizeHelper.ExtractSize("1.1 inch", out decimal[] arr);

        Assert.IsTrue(result);
        Assert.AreEqual(1, arr.Length);
        Assert.AreEqual(2.794m, arr[0]); // 1.1 inch = 2.794 cm
    }

    /// <summary>
    /// 测试单一小数数值不带单位，期望返回 false
    /// </summary>
    [TestMethod]
    public void ExtractSize_SingleDecimalValueWithoutUnit_ReturnsFalse()
    {
        var result = SizeHelper.ExtractSize("1.1", out decimal[] arr);

        Assert.IsFalse(result);
        Assert.AreEqual(0, arr.Length);
    }

    /// <summary>
    /// 测试数值带单位后面还有其他字符，期望返回 false
    /// </summary>
    [TestMethod]
    public void ExtractSize_ValueWithUnitFollowedByOtherCharacters_ReturnsFalse()
    {
        var result = SizeHelper.ExtractSize("5m1", out decimal[] arr);

        Assert.IsFalse(result);
        Assert.AreEqual(0, arr.Length);
    }

    /// <summary>
    /// 测试包含多个数字和字母的复杂字符串（无分隔符），期望返回 false
    /// </summary>
    [TestMethod]
    public void ExtractSize_ComplexStringWithoutSeparator_ReturnsFalse()
    {
        var result = SizeHelper.ExtractSize("1109020P3060", out decimal[] arr);

        Assert.IsFalse(result);
        Assert.AreEqual(0, arr.Length);
    }

    /// <summary>
    /// 测试使用斜杠分隔的多组尺寸：120x200cm/47x78.7in
    /// 期望返回 [120, 200, 119.38, 199.898]
    /// </summary>
    [TestMethod]
    public void ExtractSize_SlashSeparatedGroups_ReturnsCorrectArray()
    {
        var result = SizeHelper.ExtractSize("120x200cm/47x78.7in", out decimal[] arr);

        Assert.IsTrue(result);
        Assert.AreEqual(4, arr.Length);
        Assert.AreEqual(120m, arr[0]); // 120 cm
        Assert.AreEqual(200m, arr[1]); // 200 cm
        Assert.AreEqual(119.38m, arr[2]); // 47 inch = 119.38 cm
        Assert.AreEqual(199.898m, arr[3]); // 78.7 inch = 199.898 cm
    }

    /// <summary>
    /// 测试使用逗号分隔的多组尺寸：10x20cm,5x10in
    /// </summary>
    [TestMethod]
    public void ExtractSize_CommaSeparatedGroups_ReturnsCorrectArray()
    {
        var result = SizeHelper.ExtractSize("10x20cm,5x10in", out decimal[] arr);

        Assert.IsTrue(result);
        Assert.AreEqual(4, arr.Length);
        Assert.AreEqual(10m, arr[0]); // 10 cm
        Assert.AreEqual(20m, arr[1]); // 20 cm
        Assert.AreEqual(12.7m, arr[2]); // 5 inch = 12.7 cm
        Assert.AreEqual(25.4m, arr[3]); // 10 inch = 25.4 cm
    }

    /// <summary>
    /// 测试混合使用斜杠和括号：100x150cm/40x60in(10x15mm)
    /// </summary>
    [TestMethod]
    public void ExtractSize_MixedSlashAndParentheses_ReturnsCorrectArray()
    {
        var result = SizeHelper.ExtractSize("100x150cm/40x60in(10x15mm)", out decimal[] arr);

        Assert.IsTrue(result);
        Assert.AreEqual(6, arr.Length);
        Assert.AreEqual(100m, arr[0]); // 100 cm
        Assert.AreEqual(150m, arr[1]); // 150 cm
        Assert.AreEqual(101.6m, arr[2]); // 40 inch = 101.6 cm
        Assert.AreEqual(152.4m, arr[3]); // 60 inch = 152.4 cm
        Assert.AreEqual(1m, arr[4]); // 10 mm = 1 cm
        Assert.AreEqual(1.5m, arr[5]); // 15 mm = 1.5 cm
    }

    #region Extract Method Tests

    /// <summary>
    /// 测试无标记输入：1x2x3，期望 DimensionKind 为 Unspecified
    /// </summary>
    [TestMethod]
    public void Extract_UnmarkedInput_ReturnsUnspecifiedKind()
    {
        var result = SizeHelper.Extract("1x2x3");

        Assert.AreEqual(1, result.Count);
        var dimension = result[0];
        Assert.AreEqual(DimensionUnit.Centimeter, dimension.Unit);
        Assert.AreEqual(3, dimension.Items.Count);
        Assert.AreEqual(DimensionKind.Unspecified, dimension.Items[0].Kind);
        Assert.AreEqual(DimensionKind.Unspecified, dimension.Items[1].Kind);
        Assert.AreEqual(DimensionKind.Unspecified, dimension.Items[2].Kind);
        Assert.AreEqual(1m, dimension.Items[0].Value);
        Assert.AreEqual(2m, dimension.Items[1].Value);
        Assert.AreEqual(3m, dimension.Items[2].Value);
    }

    /// <summary>
    /// 测试英寸标记和维度标签：10''W X 36''H
    /// </summary>
    [TestMethod]
    public void Extract_InchWithDimensionLabels_ParsesCorrectly()
    {
        var result = SizeHelper.Extract("10''W X 36''H");

        Assert.AreEqual(1, result.Count);
        var dimension = result[0];
        Assert.AreEqual(DimensionUnit.Inch, dimension.Unit);
        Assert.AreEqual(2, dimension.Items.Count);

        Assert.AreEqual(DimensionKind.Width, dimension.Items[0].Kind);
        Assert.AreEqual(10m, dimension.Items[0].Value);

        Assert.AreEqual(DimensionKind.Height, dimension.Items[1].Kind);
        Assert.AreEqual(36m, dimension.Items[1].Value);
    }

    /// <summary>
    /// 测试双组解析：10cmx10cmx10cm(3.94x3.94x3.94in)，期望返回两个Dimension对象
    /// </summary>
    [TestMethod]
    public void Extract_TwoGroups_ReturnsTwoDimensions()
    {
        var result = SizeHelper.Extract("10cmx10cmx10cm(3.94x3.94x3.94in)");

        Assert.AreEqual(2, result.Count);

        // 第一组：厘米
        var dim1 = result[0];
        Assert.AreEqual(DimensionUnit.Centimeter, dim1.Unit);
        Assert.AreEqual(3, dim1.Items.Count);
        Assert.AreEqual(10m, dim1.Items[0].Value);
        Assert.AreEqual(10m, dim1.Items[1].Value);
        Assert.AreEqual(10m, dim1.Items[2].Value);

        // 第二组：英寸
        var dim2 = result[1];
        Assert.AreEqual(DimensionUnit.Inch, dim2.Unit);
        Assert.AreEqual(3, dim2.Items.Count);
        Assert.AreEqual(3.94m, dim2.Items[0].Value);
        Assert.AreEqual(3.94m, dim2.Items[1].Value);
        Assert.AreEqual(3.94m, dim2.Items[2].Value);
    }

    /// <summary>
    /// 测试厘米输入保持原单位
    /// </summary>
    [TestMethod]
    public void Extract_CentimeterInput_PreservesUnit()
    {
        var result = SizeHelper.Extract("10x20x30cm");

        Assert.AreEqual(1, result.Count);
        var dimension = result[0];
        Assert.AreEqual(DimensionUnit.Centimeter, dimension.Unit);
        Assert.AreEqual(3, dimension.Items.Count);
        Assert.AreEqual(10m, dimension.Items[0].Value);
        Assert.AreEqual(20m, dimension.Items[1].Value);
        Assert.AreEqual(30m, dimension.Items[2].Value);
    }

    /// <summary>
    /// 测试英寸输入保持原单位
    /// </summary>
    [TestMethod]
    public void Extract_InchInput_PreservesUnit()
    {
        var result = SizeHelper.Extract("10x20x30in");

        Assert.AreEqual(1, result.Count);
        var dimension = result[0];
        Assert.AreEqual(DimensionUnit.Inch, dimension.Unit);
        Assert.AreEqual(3, dimension.Items.Count);
        Assert.AreEqual(10m, dimension.Items[0].Value);
        Assert.AreEqual(20m, dimension.Items[1].Value);
        Assert.AreEqual(30m, dimension.Items[2].Value);
    }

    /// <summary>
    /// 测试毫米转换为厘米
    /// </summary>
    [TestMethod]
    public void Extract_MillimeterInput_ConvertsToCentimeter()
    {
        var result = SizeHelper.Extract("100x200x300mm");

        Assert.AreEqual(1, result.Count);
        var dimension = result[0];
        Assert.AreEqual(DimensionUnit.Centimeter, dimension.Unit);
        Assert.AreEqual(3, dimension.Items.Count);
        Assert.AreEqual(10m, dimension.Items[0].Value);  // 100mm = 10cm
        Assert.AreEqual(20m, dimension.Items[1].Value);  // 200mm = 20cm
        Assert.AreEqual(30m, dimension.Items[2].Value);  // 300mm = 30cm
    }

    /// <summary>
    /// 测试米转换为厘米
    /// </summary>
    [TestMethod]
    public void Extract_MeterInput_ConvertsToCentimeter()
    {
        var result = SizeHelper.Extract("1x2x3m");

        Assert.AreEqual(1, result.Count);
        var dimension = result[0];
        Assert.AreEqual(DimensionUnit.Centimeter, dimension.Unit);
        Assert.AreEqual(3, dimension.Items.Count);
        Assert.AreEqual(100m, dimension.Items[0].Value);  // 1m = 100cm
        Assert.AreEqual(200m, dimension.Items[1].Value);  // 2m = 200cm
        Assert.AreEqual(300m, dimension.Items[2].Value);  // 3m = 300cm
    }

    /// <summary>
    /// 测试双引号标记的英寸
    /// </summary>
    [TestMethod]
    public void Extract_DoubleQuoteInch_ParsesCorrectly()
    {
        var result = SizeHelper.Extract("10\"x20\"x30\"");

        Assert.AreEqual(1, result.Count);
        var dimension = result[0];
        Assert.AreEqual(DimensionUnit.Inch, dimension.Unit);
        Assert.AreEqual(3, dimension.Items.Count);
        Assert.AreEqual(10m, dimension.Items[0].Value);
        Assert.AreEqual(20m, dimension.Items[1].Value);
        Assert.AreEqual(30m, dimension.Items[2].Value);
    }

    /// <summary>
    /// 测试空字符串输入
    /// </summary>
    [TestMethod]
    public void Extract_EmptyString_ReturnsEmptyList()
    {
        var result = SizeHelper.Extract("");

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }

    /// <summary>
    /// 测试null输入
    /// </summary>
    [TestMethod]
    public void Extract_NullString_ReturnsEmptyList()
    {
        var result = SizeHelper.Extract(null);

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }

    /// <summary>
    /// 测试维度标签：Width, Height, Length
    /// </summary>
    [TestMethod]
    public void Extract_DimensionLabels_ParsesCorrectly()
    {
        var result = SizeHelper.Extract("10cmW x 20cmH x 30cmL");

        Assert.AreEqual(1, result.Count);
        var dimension = result[0];
        Assert.AreEqual(DimensionUnit.Centimeter, dimension.Unit);
        Assert.AreEqual(3, dimension.Items.Count);

        Assert.AreEqual(DimensionKind.Width, dimension.Items[0].Kind);
        Assert.AreEqual(10m, dimension.Items[0].Value);

        Assert.AreEqual(DimensionKind.Height, dimension.Items[1].Kind);
        Assert.AreEqual(20m, dimension.Items[1].Value);

        Assert.AreEqual(DimensionKind.Length, dimension.Items[2].Kind);
        Assert.AreEqual(30m, dimension.Items[2].Value);
    }

    /// <summary>
    /// 测试完整维度标签词：Width, Height, Length
    /// </summary>
    [TestMethod]
    public void Extract_FullDimensionLabels_ParsesCorrectly()
    {
        var result = SizeHelper.Extract("10Width x 20Height x 30Length cm");

        Assert.AreEqual(1, result.Count);
        var dimension = result[0];
        Assert.AreEqual(DimensionUnit.Centimeter, dimension.Unit);
        Assert.AreEqual(3, dimension.Items.Count);

        Assert.AreEqual(DimensionKind.Width, dimension.Items[0].Kind);
        Assert.AreEqual(DimensionKind.Height, dimension.Items[1].Kind);
        Assert.AreEqual(DimensionKind.Length, dimension.Items[2].Kind);
    }

    /// <summary>
    /// 测试混合单位和维度标签
    /// </summary>
    [TestMethod]
    public void Extract_MixedUnitsWithLabels_ParsesCorrectly()
    {
        var result = SizeHelper.Extract("10cmW x 20''H");

        Assert.AreEqual(1, result.Count);
        var dimension = result[0];
        // 当有多种单位时，优先检测到的单位作为主单位
        Assert.AreEqual(2, dimension.Items.Count);

        Assert.AreEqual(DimensionKind.Width, dimension.Items[0].Kind);
        Assert.AreEqual(DimensionKind.Height, dimension.Items[1].Kind);
    }

    /// <summary>
    /// 测试小数值
    /// </summary>
    [TestMethod]
    public void Extract_DecimalValues_ParsesCorrectly()
    {
        var result = SizeHelper.Extract("10.5x20.3x30.7cm");

        Assert.AreEqual(1, result.Count);
        var dimension = result[0];
        Assert.AreEqual(DimensionUnit.Centimeter, dimension.Unit);
        Assert.AreEqual(3, dimension.Items.Count);
        Assert.AreEqual(10.5m, dimension.Items[0].Value);
        Assert.AreEqual(20.3m, dimension.Items[1].Value);
        Assert.AreEqual(30.7m, dimension.Items[2].Value);
    }

    /// <summary>
    /// 测试分米转换为厘米
    /// </summary>
    [TestMethod]
    public void Extract_DecimeterInput_ConvertsToCentimeter()
    {
        var result = SizeHelper.Extract("1x2x3dm");

        Assert.AreEqual(1, result.Count);
        var dimension = result[0];
        Assert.AreEqual(DimensionUnit.Centimeter, dimension.Unit);
        Assert.AreEqual(3, dimension.Items.Count);
        Assert.AreEqual(10m, dimension.Items[0].Value);  // 1dm = 10cm
        Assert.AreEqual(20m, dimension.Items[1].Value);  // 2dm = 20cm
        Assert.AreEqual(30m, dimension.Items[2].Value);  // 3dm = 30cm
    }

    /// <summary>
    /// 测试多括号组
    /// </summary>
    [TestMethod]
    public void Extract_MultipleParenthesesGroups_ParsesCorrectly()
    {
        var result = SizeHelper.Extract("10x20cm(5x6in)(7x8mm)");

        Assert.AreEqual(3, result.Count);

        // 第一组：厘米
        Assert.AreEqual(DimensionUnit.Centimeter, result[0].Unit);
        Assert.AreEqual(2, result[0].Items.Count);

        // 第二组：英寸
        Assert.AreEqual(DimensionUnit.Inch, result[1].Unit);
        Assert.AreEqual(2, result[1].Items.Count);

        // 第三组：毫米转厘米
        Assert.AreEqual(DimensionUnit.Centimeter, result[2].Unit);
        Assert.AreEqual(2, result[2].Items.Count);
    }


    /// <summary>
    /// 测试带维度标签和斜杠分隔的多组尺寸：W180xH100cm/71x39in
    /// 期望返回两个Dimension对象，第一组为厘米单位，第二组为英寸单位
    /// </summary>
    [TestMethod]
    public void Extract_WidthHeightLabelsWithSlashSeparator_ParsesCorrectly()
    {
        var result = SizeHelper.Extract("W180xH100cm/71x39in");

        Assert.HasCount(2, result);

        // 第一组：厘米，带W和H标签
        var dim1 = result[0];
        Assert.AreEqual(DimensionUnit.Centimeter, dim1.Unit);
        Assert.HasCount(2, dim1.Items);
        Assert.AreEqual(DimensionKind.Width, dim1.Items[0].Kind);
        Assert.AreEqual(180m, dim1.Items[0].Value);
        Assert.AreEqual(DimensionKind.Height, dim1.Items[1].Kind);
        Assert.AreEqual(100m, dim1.Items[1].Value);

        // 第二组：英寸
        var dim2 = result[1];
        Assert.AreEqual(DimensionUnit.Inch, dim2.Unit);
        Assert.HasCount(2, dim2.Items);
        Assert.AreEqual(71m, dim2.Items[0].Value);
        Assert.AreEqual(39m, dim2.Items[1].Value);
    }

    /// <summary>
    /// 测试双引号标记的英寸带维度标签（后置）：10''W X 36''H
    /// 期望正确解析出宽度和高度，单位为英寸
    /// </summary>
    [TestMethod]
    public void Extract_DoubleQuoteInchWithPostfixDimensionLabels_ParsesCorrectly()
    {
        var result = SizeHelper.Extract("10''W X 36''H");

        Assert.HasCount(1, result);
        var dimension = result[0];
        Assert.AreEqual(DimensionUnit.Inch, dimension.Unit);
        Assert.HasCount(2, dimension.Items);

        Assert.AreEqual(DimensionKind.Width, dimension.Items[0].Kind);
        Assert.AreEqual(10m, dimension.Items[0].Value);

        Assert.AreEqual(DimensionKind.Height, dimension.Items[1].Kind);
        Assert.AreEqual(36m, dimension.Items[1].Value);
    }
    #endregion
}
