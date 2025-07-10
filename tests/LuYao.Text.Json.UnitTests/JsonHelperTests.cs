using Microsoft.VisualStudio.TestTools.UnitTesting;
using LuYao.Text.Json;
using Newtonsoft.Json;
using System.Collections.Specialized;

namespace LuYao.Text.Json.UnitTests;

/// <summary>
/// 测试 JsonHelper 类的功能。
/// </summary>
[TestClass]
public class JsonHelperTests
{
    /// <summary>
    /// 测试 ExtractJson 方法，输入有效的 JSON 字符串，期望返回格式化后的 JSON。
    /// </summary>
    [TestMethod]
    public void ExtractJson_ValidJsonString_ReturnsFormattedJson()
    {
        // Arrange
        string input = "{\"name\":\"John\",\"age\":30,\"city\":\"New York\"}</script>";
        string expected = "{\"name\":\"John\",\"age\":30,\"city\":\"New York\"}";

        // Act
        string result = JsonHelper.ExtractJson(input, Formatting.None);

        // Assert
        Assert.AreEqual(expected, result);
    }

    /// <summary>
    /// 测试 ExtractJson 方法，输入空字符串，期望返回空字符串。
    /// </summary>
    [TestMethod]
    public void ExtractJson_EmptyString_ReturnsEmptyString()
    {
        // Arrange
        string input = string.Empty;

        // Act
        string result = JsonHelper.ExtractJson(input);

        // Assert
        Assert.AreEqual(string.Empty, result);
    }

    /// <summary>
    /// 测试 ExtractValueByNames 方法，输入包含目标属性的 JSON 字符串，期望提取目标属性值。
    /// </summary>
    [TestMethod]
    public void ExtractValueByNames_JsonWithTargetProperties_ReturnsCorrectValues()
    {
        // Arrange
        string input = "{\"name\":\"John\",\"age\":30,\"city\":\"New York\"}";
        string[] targetProperties = { "name", "city" };
        NameValueCollection expected = new NameValueCollection
        {
            { "name", "John" },
            { "city", "New York" }
        };

        // Act
        NameValueCollection result = JsonHelper.ExtractValueByNames(input, targetProperties);

        // Assert
        Assert.AreEqual(expected["name"], result["name"]);
        Assert.AreEqual(expected["city"], result["city"]);
    }

    /// <summary>
    /// 测试 ExtractValueByNames 方法，输入不包含目标属性的 JSON 字符串，期望返回空集合。
    /// </summary>
    [TestMethod]
    public void ExtractValueByNames_JsonWithoutTargetProperties_ReturnsEmptyCollection()
    {
        // Arrange
        string input = "{\"name\":\"John\",\"age\":30,\"city\":\"New York\"}";
        string[] targetProperties = { "country", "state" };

        // Act
        NameValueCollection result = JsonHelper.ExtractValueByNames(input, targetProperties);

        // Assert
        Assert.AreEqual(0, result.Count);
    }

    /// <summary>
    /// 测试 ExtractValueByNames 方法，输入空字符串，期望返回空集合。
    /// </summary>
    [TestMethod]
    public void ExtractValueByNames_EmptyString_ReturnsEmptyCollection()
    {
        // Arrange
        string input = string.Empty;
        string[] targetProperties = { "name", "city" };

        // Act
        NameValueCollection result = JsonHelper.ExtractValueByNames(input, targetProperties);

        // Assert
        Assert.AreEqual(0, result.Count);
    }
}