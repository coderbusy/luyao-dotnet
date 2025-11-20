using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Text.Json;

[TestClass]
public class TranslatableJsonModelTests
{
    /// <summary>
    /// 测试 Transform 方法，输入有效的 JSON 字符串，期望返回正确的模型对象。
    /// </summary>
    [TestMethod]
    public void Transform_ValidJsonString_ReturnsCorrectModel()
    {
        // Arrange
        string json = "{\"Name\":\"John\",\"Age\":30}";

        // Act
        var result = TestJsonModel.Transform(json);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("John", result.Name);
        Assert.AreEqual(30, result.Age);
    }

    /// <summary>
    /// 测试 Transform 方法，输入空字符串，期望引发 ArgumentException。
    /// </summary>
    [TestMethod]
    public void Transform_EmptyJsonString_ThrowsArgumentException()
    {
        // Arrange
        string json = "";

        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => TestJsonModel.Transform(json));
    }

    /// <summary>
    /// 测试 Transform 方法，输入有效的对象，期望返回正确的模型对象。
    /// </summary>
    [TestMethod]
    public void Transform_ValidObject_ReturnsCorrectModel()
    {
        // Arrange
        var model = new { Name = "Jane", Age = 25 };

        // Act
        var result = TestJsonModel.Transform(model);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("Jane", result.Name);
        Assert.AreEqual(25, result.Age);
    }

    /// <summary>
    /// 测试 Transform 方法，输入 null 对象，期望引发 ArgumentNullException。
    /// </summary>
    [TestMethod]
    public void Transform_NullObject_ThrowsArgumentNullException()
    {
        // Arrange
        object? model = null;

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => TestJsonModel.Transform(model));
    }
}
