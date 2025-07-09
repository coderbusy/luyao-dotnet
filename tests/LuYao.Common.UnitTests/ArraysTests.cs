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
}