using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LuYao.Globalization;

/// <summary>
/// RmbHelper 的单元测试类
/// </summary>
[TestClass]
public class RmbHelperTests
{
    /// <summary>
    /// 测试金额为零时，期望返回“零元整”
    /// </summary>
    [TestMethod]
    public void ToRmbUpper_ZeroAmount_ReturnsZeroYuanWhole()
    {
        var result = RmbHelper.ToRmbUpper(0M);
        Assert.AreEqual("零元整", result);
    }

    /// <summary>
    /// 测试金额为小数时，期望返回正确的大写形式
    /// </summary>
    [TestMethod]
    public void ToRmbUpper_DecimalAmount_ReturnsCorrectUpperCase()
    {
        var result = RmbHelper.ToRmbUpper(1234.56M);
        Assert.AreEqual("壹仟贰佰叁拾肆元伍角陆分", result);
    }

    /// <summary>
    /// 测试金额为整数时，期望返回正确的大写形式
    /// </summary>
    [TestMethod]
    public void ToRmbUpper_IntegerAmount_ReturnsCorrectUpperCase()
    {
        var result = RmbHelper.ToRmbUpper(100000000M);
        Assert.AreEqual("壹亿元整", result);
    }

    /// <summary>
    /// 测试金额超出范围时，期望抛出 ArgumentOutOfRangeException
    /// </summary>
    [TestMethod]
    public void ToRmbUpper_OutOfRangeAmount_ThrowsArgumentOutOfRangeException()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => RmbHelper.ToRmbUpper(10000000000000000M));
    }

    /// <summary>
    /// 测试金额为负数时，期望抛出 ArgumentOutOfRangeException
    /// </summary>
    [TestMethod]
    public void ToRmbUpper_NegativeAmount_ThrowsArgumentOutOfRangeException()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => RmbHelper.ToRmbUpper(-1M));
    }

    /// <summary>
    /// 测试金额为最大值时，期望抛出 ArgumentOutOfRangeException
    /// </summary>
    [TestMethod]
    public void ToRmbUpper_MaxValidAmount_ThrowsArgumentOutOfRangeException()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => RmbHelper.ToRmbUpper(9999999999999999.99M));
    }
}