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
    /// 测试没有分隔符的字符串，期望返回 false
    /// </summary>
    [TestMethod]
    public void ExtractSize_NoSeparator_ReturnsFalse()
    {
        var result = SizeHelper.ExtractSize("10cm", out decimal[] arr);
        
        Assert.IsFalse(result);
        Assert.AreEqual(0, arr.Length);
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
}
