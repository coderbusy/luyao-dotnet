using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace LuYao;

[TestClass]
public class ValidStringTests
{
    [TestMethod]
    public void ToString_ObjectIsNull_ReturnsEmpty()
    {
        Assert.AreEqual(string.Empty, Valid.ToString((object)null));
    }

    [TestMethod]
    public void ToString_ObjectIsDBNull_ReturnsEmpty()
    {
        Assert.AreEqual(string.Empty, Valid.ToString(DBNull.Value));
    }

    [TestMethod]
    public void ToString_ObjectIsString_ReturnsStringValue()
    {
        Assert.AreEqual("abc", Valid.ToString((object)"abc"));
    }

    [TestMethod]
    public void ToString_BoolTrue_Returns1()
    {
        Assert.AreEqual("1", Valid.ToString(true));
    }

    [TestMethod]
    public void ToString_BoolFalse_Returns0()
    {
        Assert.AreEqual("0", Valid.ToString(false));
    }

    [TestMethod]
    public void ToString_NullableBoolTrue_Returns1()
    {
        Assert.AreEqual("1", Valid.ToString((bool?)true));
    }

    [TestMethod]
    public void ToString_NullableBoolFalse_Returns0()
    {
        Assert.AreEqual("0", Valid.ToString((bool?)false));
    }

    [TestMethod]
    public void ToString_NullableBoolNull_Returns0()
    {
        Assert.AreEqual("0", Valid.ToString((bool?)null));
    }

    [TestMethod]
    public void ToString_Char_ReturnsCharString()
    {
        Assert.AreEqual("A", Valid.ToString('A'));
    }

    [TestMethod]
    public void ToString_NullableCharNull_ReturnsEmpty()
    {
        Assert.AreEqual(string.Empty, Valid.ToString((char?)null));
    }

    [TestMethod]
    public void ToString_NullableCharValue_ReturnsCharString()
    {
        Assert.AreEqual("B", Valid.ToString((char?)'B'));
    }

    [TestMethod]
    public void ToString_Int32_ReturnsIntString()
    {
        Assert.AreEqual("123", Valid.ToString(123));
    }

    [TestMethod]
    public void ToString_NullableInt32Null_ReturnsEmpty()
    {
        Assert.AreEqual(string.Empty, Valid.ToString((int?)null));
    }

    [TestMethod]
    public void ToString_NullableInt32Value_ReturnsIntString()
    {
        Assert.AreEqual("456", Valid.ToString((int?)456));
    }

    [TestMethod]
    public void ToString_StringIsNull_ReturnsEmpty()
    {
        Assert.AreEqual(string.Empty, Valid.ToString((string)null));
    }

    [TestMethod]
    public void ToString_StringIsValue_ReturnsValue()
    {
        Assert.AreEqual("hello", Valid.ToString("hello"));
    }

    [TestMethod]
    public void ToString_DateTimeDefault_ReturnsEmpty()
    {
        Assert.AreEqual(string.Empty, Valid.ToString(default(DateTime), Date.年月日));
    }

    [TestMethod]
    public void ToString_DateTime年月日_ReturnsFormattedDate()
    {
        var dt = new DateTime(2024, 7, 18, 15, 30, 45);
        Assert.AreEqual("2024/07/18", Valid.ToString(dt, Date.年月日));
    }

    [TestMethod]
    public void ToString_DateTime全部显示_ReturnsFullFormattedDate()
    {
        var dt = new DateTime(2024, 7, 18, 15, 30, 45);
        Assert.AreEqual("2024/07/18 15:30:45", Valid.ToString(dt, Date.全部显示));
    }

    [TestMethod]
    public void ToString_DateTimeOnly_Returns年月日Format()
    {
        var dt = new DateTime(2024, 7, 18, 15, 30, 45);
        Assert.AreEqual("2024/07/18", Valid.ToString(dt));
    }

    [TestMethod]
    public void ToString_NullableDateTimeNull_ReturnsEmpty()
    {
        Assert.AreEqual(string.Empty, Valid.ToString((DateTime?)null));
    }

    [TestMethod]
    public void ToString_NullableDateTimeValue全部显示_ReturnsFullFormat()
    {
        var dt = new DateTime(2024, 7, 18, 15, 30, 45);
        Assert.AreEqual("2024/07/18 15:30:45", Valid.ToString((DateTime?)dt, Date.全部显示));
    }
}