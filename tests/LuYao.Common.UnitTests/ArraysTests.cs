using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao;

/// <summary>
/// 测试 Arrays 类型的功能。
/// </summary>
[TestClass]
public class ArraysTests
{
    /// <summary>
    /// 测试 Empty 方法在获取空数组时返回正确结果。
    /// </summary>
    [TestMethod]
    public void Empty_GetEmptyArray_ReturnsEmptyArray()
    {
        // Act
        var result = Arrays.Empty<int>();

        // Assert
        Assert.IsNotNull(result, "返回的数组不应为 null。");
        Assert.AreEqual(0, result.Length, "返回的数组长度应为 0。");
    }

    /// <summary>
    /// 测试 Empty 方法在多次调用时返回相同的实例。
    /// </summary>
    [TestMethod]
    public void Empty_MultipleCalls_ReturnsSameInstance()
    {
        // Act
        var result1 = Arrays.Empty<string>();
        var result2 = Arrays.Empty<string>();

        // Assert
        Assert.AreSame(result1, result2, "多次调用 Empty 方法应返回相同的数组实例。");
    }

    [TestMethod]
    public void Join_EmptyArray_EmptyString()
    {
        // Arrange
        var emptyArray = Array.Empty<int>();
        // Act
        var result = Arrays.Join(emptyArray, ",", 0);
        // Assert
        Assert.AreEqual(string.Empty, result, "空数组连接结果应为空字符串。");
    }

    [TestMethod]
    public void Join_NullElement_JoinEmptyString()
    {
        // Arrange
        var emptyArray = new[] { "A", string.Empty, "C", null, "D" };
        // Act
        var result = Arrays.Join(emptyArray, ",", 0);
        // Assert
        Assert.AreEqual("A,,C,,D", result, "包含空字符串的数组连接结果应为空字符串。");
    }

    [TestMethod]
    public void Join_WithRange_JoinCorrectly()
    {
        // Arrange
        var array = new[] { "A", "B", "C", "D", "E" };
        // Act
        var result = Arrays.Join(array, ",", 1, 3);
        // Assert
        Assert.AreEqual("B,C,D", result, "连接指定范围的数组元素应正确返回。");
    }
}