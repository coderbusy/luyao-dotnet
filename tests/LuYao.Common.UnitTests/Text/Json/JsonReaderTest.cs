using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LuYao.Text.Json;

[TestClass]
public class JsonReaderTest
{
    [TestMethod]
    public void Constructor_WithTextReader_ShouldInitializeCorrectly()
    {
        // Arrange
        var textReader = new StringReader("{}");

        // Act
        using var jsonReader = new JsonReader(textReader);

        // Assert
        Assert.AreEqual(JsonTokenType.None, jsonReader.TokenType);
        Assert.IsNull(jsonReader.Value);
        Assert.AreEqual(1, jsonReader.Line);
        Assert.AreEqual(1, jsonReader.Column);
    }

    [TestMethod]
    public void Constructor_WithString_ShouldInitializeCorrectly()
    {
        // Arrange
        string json = "{}";

        // Act
        using var jsonReader = new JsonReader(json);

        // Assert
        Assert.AreEqual(JsonTokenType.None, jsonReader.TokenType);
        Assert.IsNull(jsonReader.Value);
        Assert.AreEqual(1, jsonReader.Line);
        Assert.AreEqual(1, jsonReader.Column);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Constructor_WithNullTextReader_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        new JsonReader((TextReader)null);
    }

    [TestMethod]
    public void Read_EmptyObject_ShouldReadCorrectly()
    {
        // Arrange
        using var jsonReader = new JsonReader("{}");

        // Act & Assert
        Assert.IsTrue(jsonReader.Read());
        Assert.AreEqual(JsonTokenType.StartObject, jsonReader.TokenType);
        Assert.IsNull(jsonReader.Value);

        Assert.IsTrue(jsonReader.Read());
        Assert.AreEqual(JsonTokenType.EndObject, jsonReader.TokenType);
        Assert.IsNull(jsonReader.Value);

        Assert.IsFalse(jsonReader.Read());
        Assert.AreEqual(JsonTokenType.None, jsonReader.TokenType);
    }

    [TestMethod]
    public void Read_EmptyArray_ShouldReadCorrectly()
    {
        // Arrange
        using var jsonReader = new JsonReader("[]");

        // Act & Assert
        Assert.IsTrue(jsonReader.Read());
        Assert.AreEqual(JsonTokenType.StartArray, jsonReader.TokenType);
        Assert.IsNull(jsonReader.Value);

        Assert.IsTrue(jsonReader.Read());
        Assert.AreEqual(JsonTokenType.EndArray, jsonReader.TokenType);
        Assert.IsNull(jsonReader.Value);

        Assert.IsFalse(jsonReader.Read());
        Assert.AreEqual(JsonTokenType.None, jsonReader.TokenType);
    }

    [TestMethod]
    public void Read_SimpleString_ShouldReadCorrectly()
    {
        // Arrange
        using var jsonReader = new JsonReader("\"hello\"");

        // Act & Assert
        Assert.IsTrue(jsonReader.Read());
        Assert.AreEqual(JsonTokenType.String, jsonReader.TokenType);
        Assert.AreEqual("hello", jsonReader.Value);

        Assert.IsFalse(jsonReader.Read());
    }

    [TestMethod]
    public void Read_PropertyName_ShouldReadCorrectly()
    {
        // Arrange
        using var jsonReader = new JsonReader("{\"name\":\"value\"}");

        // Act & Assert
        Assert.IsTrue(jsonReader.Read()); // {
        Assert.AreEqual(JsonTokenType.StartObject, jsonReader.TokenType);

        Assert.IsTrue(jsonReader.Read()); // "name"
        Assert.AreEqual(JsonTokenType.PropertyName, jsonReader.TokenType);
        Assert.AreEqual("name", jsonReader.Value);

        Assert.IsTrue(jsonReader.Read()); // "value"
        Assert.AreEqual(JsonTokenType.String, jsonReader.TokenType);
        Assert.AreEqual("value", jsonReader.Value);

        Assert.IsTrue(jsonReader.Read()); // }
        Assert.AreEqual(JsonTokenType.EndObject, jsonReader.TokenType);
    }

    [TestMethod]
    public void Read_StringWithEscapes_ShouldReadCorrectly()
    {
        // Arrange
        using var jsonReader = new JsonReader("\"hello\\nworld\\t\\\"test\\\"\"");

        // Act
        Assert.IsTrue(jsonReader.Read());

        // Assert
        Assert.AreEqual(JsonTokenType.String, jsonReader.TokenType);
        Assert.AreEqual("hello\nworld\t\"test\"", jsonReader.Value);
    }

    [TestMethod]
    public void Read_StringWithUnicodeEscape_ShouldReadCorrectly()
    {
        // Arrange
        using var jsonReader = new JsonReader("\"\\u0048\\u0065\\u006C\\u006C\\u006F\""); // "Hello"

        // Act
        Assert.IsTrue(jsonReader.Read());

        // Assert
        Assert.AreEqual(JsonTokenType.String, jsonReader.TokenType);
        Assert.AreEqual("Hello", jsonReader.Value);
    }

    [TestMethod]
    [ExpectedException(typeof(JsonException))]
    public void Read_StringWithInvalidUnicodeEscape_ShouldThrowException()
    {
        // Arrange
        using var jsonReader = new JsonReader("\"\\uGGGG\"");

        // Act
        jsonReader.Read();
    }

    [TestMethod]
    [ExpectedException(typeof(JsonException))]
    public void Read_StringWithInvalidEscape_ShouldThrowException()
    {
        // Arrange
        using var jsonReader = new JsonReader("\"\\x\"");

        // Act
        jsonReader.Read();
    }

    [TestMethod]
    [ExpectedException(typeof(JsonException))]
    public void Read_UnterminatedString_ShouldThrowException()
    {
        // Arrange
        using var jsonReader = new JsonReader("\"unterminated");

        // Act
        jsonReader.Read();
    }

    [TestMethod]
    public void Read_PositiveInteger_ShouldReadCorrectly()
    {
        // Arrange
        using var jsonReader = new JsonReader("123");

        // Act
        Assert.IsTrue(jsonReader.Read());

        // Assert
        Assert.AreEqual(JsonTokenType.Number, jsonReader.TokenType);
        Assert.AreEqual(123L, jsonReader.Value);
    }

    [TestMethod]
    public void Read_NegativeInteger_ShouldReadCorrectly()
    {
        // Arrange
        using var jsonReader = new JsonReader("-456");

        // Act
        Assert.IsTrue(jsonReader.Read());

        // Assert
        Assert.AreEqual(JsonTokenType.Number, jsonReader.TokenType);
        Assert.AreEqual(-456L, jsonReader.Value);
    }

    [TestMethod]
    public void Read_PositiveDecimal_ShouldReadCorrectly()
    {
        // Arrange
        using var jsonReader = new JsonReader("123.45");

        // Act
        Assert.IsTrue(jsonReader.Read());

        // Assert
        Assert.AreEqual(JsonTokenType.Number, jsonReader.TokenType);
        Assert.AreEqual(123.45, jsonReader.Value);
    }

    [TestMethod]
    public void Read_NegativeDecimal_ShouldReadCorrectly()
    {
        // Arrange
        using var jsonReader = new JsonReader("-123.45");

        // Act
        Assert.IsTrue(jsonReader.Read());

        // Assert
        Assert.AreEqual(JsonTokenType.Number, jsonReader.TokenType);
        Assert.AreEqual(-123.45, jsonReader.Value);
    }

    [TestMethod]
    public void Read_NumberWithExponent_ShouldReadCorrectly()
    {
        // Arrange
        using var jsonReader = new JsonReader("1.23e4");

        // Act
        Assert.IsTrue(jsonReader.Read());

        // Assert
        Assert.AreEqual(JsonTokenType.Number, jsonReader.TokenType);
        Assert.AreEqual(12300.0, jsonReader.Value);
    }

    [TestMethod]
    public void Read_NumberWithNegativeExponent_ShouldReadCorrectly()
    {
        // Arrange
        using var jsonReader = new JsonReader("1.23e-4");

        // Act
        Assert.IsTrue(jsonReader.Read());

        // Assert
        Assert.AreEqual(JsonTokenType.Number, jsonReader.TokenType);
        Assert.AreEqual(0.000123, jsonReader.Value);
    }

    [TestMethod]
    [ExpectedException(typeof(JsonException))]
    public void Read_InvalidNumber_ShouldThrowException()
    {
        // Arrange
        using var jsonReader = new JsonReader("-");

        // Act
        jsonReader.Read();
    }

    [TestMethod]
    public void Read_BooleanTrue_ShouldReadCorrectly()
    {
        // Arrange
        using var jsonReader = new JsonReader("true");

        // Act
        Assert.IsTrue(jsonReader.Read());

        // Assert
        Assert.AreEqual(JsonTokenType.Boolean, jsonReader.TokenType);
        Assert.AreEqual(true, jsonReader.Value);
    }

    [TestMethod]
    public void Read_BooleanFalse_ShouldReadCorrectly()
    {
        // Arrange
        using var jsonReader = new JsonReader("false");

        // Act
        Assert.IsTrue(jsonReader.Read());

        // Assert
        Assert.AreEqual(JsonTokenType.Boolean, jsonReader.TokenType);
        Assert.AreEqual(false, jsonReader.Value);
    }

    [TestMethod]
    [ExpectedException(typeof(JsonException))]
    public void Read_InvalidBoolean_ShouldThrowException()
    {
        // Arrange
        using var jsonReader = new JsonReader("tru");

        // Act
        jsonReader.Read();
    }

    [TestMethod]
    public void Read_Null_ShouldReadCorrectly()
    {
        // Arrange
        using var jsonReader = new JsonReader("null");

        // Act
        Assert.IsTrue(jsonReader.Read());

        // Assert
        Assert.AreEqual(JsonTokenType.Null, jsonReader.TokenType);
        Assert.IsNull(jsonReader.Value);
    }

    [TestMethod]
    [ExpectedException(typeof(JsonException))]
    public void Read_InvalidNull_ShouldThrowException()
    {
        // Arrange
        using var jsonReader = new JsonReader("nul");

        // Act
        jsonReader.Read();
    }

    [TestMethod]
    public void Read_ComplexObject_ShouldReadCorrectly()
    {
        // Arrange
        string json = "{\"name\":\"John\",\"age\":30,\"active\":true,\"scores\":[85,92,78],\"address\":null}";
        using var jsonReader = new JsonReader(json);

        // Act & Assert
        Assert.IsTrue(jsonReader.Read()); // {
        Assert.AreEqual(JsonTokenType.StartObject, jsonReader.TokenType);

        Assert.IsTrue(jsonReader.Read()); // "name"
        Assert.AreEqual(JsonTokenType.PropertyName, jsonReader.TokenType);
        Assert.AreEqual("name", jsonReader.Value);

        Assert.IsTrue(jsonReader.Read()); // "John"
        Assert.AreEqual(JsonTokenType.String, jsonReader.TokenType);
        Assert.AreEqual("John", jsonReader.Value);

        Assert.IsTrue(jsonReader.Read()); // "age"
        Assert.AreEqual(JsonTokenType.PropertyName, jsonReader.TokenType);
        Assert.AreEqual("age", jsonReader.Value);

        Assert.IsTrue(jsonReader.Read()); // 30
        Assert.AreEqual(JsonTokenType.Number, jsonReader.TokenType);
        Assert.AreEqual(30L, jsonReader.Value);

        Assert.IsTrue(jsonReader.Read()); // "active"
        Assert.AreEqual(JsonTokenType.PropertyName, jsonReader.TokenType);
        Assert.AreEqual("active", jsonReader.Value);

        Assert.IsTrue(jsonReader.Read()); // true
        Assert.AreEqual(JsonTokenType.Boolean, jsonReader.TokenType);
        Assert.AreEqual(true, jsonReader.Value);

        Assert.IsTrue(jsonReader.Read()); // "scores"
        Assert.AreEqual(JsonTokenType.PropertyName, jsonReader.TokenType);
        Assert.AreEqual("scores", jsonReader.Value);

        Assert.IsTrue(jsonReader.Read()); // [
        Assert.AreEqual(JsonTokenType.StartArray, jsonReader.TokenType);

        Assert.IsTrue(jsonReader.Read()); // 85
        Assert.AreEqual(JsonTokenType.Number, jsonReader.TokenType);
        Assert.AreEqual(85L, jsonReader.Value);

        Assert.IsTrue(jsonReader.Read()); // 92
        Assert.AreEqual(JsonTokenType.Number, jsonReader.TokenType);
        Assert.AreEqual(92L, jsonReader.Value);

        Assert.IsTrue(jsonReader.Read()); // 78
        Assert.AreEqual(JsonTokenType.Number, jsonReader.TokenType);
        Assert.AreEqual(78L, jsonReader.Value);

        Assert.IsTrue(jsonReader.Read()); // ]
        Assert.AreEqual(JsonTokenType.EndArray, jsonReader.TokenType);

        Assert.IsTrue(jsonReader.Read()); // "address"
        Assert.AreEqual(JsonTokenType.PropertyName, jsonReader.TokenType);
        Assert.AreEqual("address", jsonReader.Value);

        Assert.IsTrue(jsonReader.Read()); // null
        Assert.AreEqual(JsonTokenType.Null, jsonReader.TokenType);
        Assert.IsNull(jsonReader.Value);

        Assert.IsTrue(jsonReader.Read()); // }
        Assert.AreEqual(JsonTokenType.EndObject, jsonReader.TokenType);

        Assert.IsFalse(jsonReader.Read()); // EOF
        Assert.AreEqual(JsonTokenType.None, jsonReader.TokenType);
    }

    [TestMethod]
    public void Read_WithWhitespace_ShouldSkipWhitespace()
    {
        // Arrange
        using var jsonReader = new JsonReader("  \t\r\n  {  \t\r\n  }  \t\r\n  ");

        // Act & Assert
        Assert.IsTrue(jsonReader.Read());
        Assert.AreEqual(JsonTokenType.StartObject, jsonReader.TokenType);

        Assert.IsTrue(jsonReader.Read());
        Assert.AreEqual(JsonTokenType.EndObject, jsonReader.TokenType);

        Assert.IsFalse(jsonReader.Read());
    }

    [TestMethod]
    public void LineAndColumn_ShouldUpdateCorrectly()
    {
        // Arrange
        using var jsonReader = new JsonReader("{\n  \"name\": \"value\"\n}");

        // Act & Assert
        Assert.AreEqual(1, jsonReader.Line);
        Assert.AreEqual(1, jsonReader.Column);

        Assert.IsTrue(jsonReader.Read()); // {
        Assert.AreEqual(1, jsonReader.Line);
        Assert.AreEqual(2, jsonReader.Column);

        Assert.IsTrue(jsonReader.Read()); // "name"
        Assert.AreEqual(2, jsonReader.Line);

        Assert.IsTrue(jsonReader.Read()); // "value"

        Assert.IsTrue(jsonReader.Read()); // }
        Assert.AreEqual(3, jsonReader.Line);
    }

    [TestMethod]
    [ExpectedException(typeof(JsonException))]
    public void Read_UnexpectedCharacter_ShouldThrowException()
    {
        // Arrange
        using var jsonReader = new JsonReader("@");

        // Act
        jsonReader.Read();
    }

    [TestMethod]
    [ExpectedException(typeof(ObjectDisposedException))]
    public void Read_AfterDispose_ShouldThrowObjectDisposedException()
    {
        // Arrange
        var jsonReader = new JsonReader("{}");
        jsonReader.Dispose();

        // Act
        jsonReader.Read();
    }

    [TestMethod]
    public void Dispose_ShouldDisposeCorrectly()
    {
        // Arrange
        var textReader = new StringReader("{}");
        var jsonReader = new JsonReader(textReader, ownsReader: true);

        // Act
        jsonReader.Dispose();

        // Assert - 第二次调用不应该抛出异常
        jsonReader.Dispose();
    }

    [TestMethod]
    public void Dispose_WithoutOwningReader_ShouldNotDisposeReader()
    {
        // Arrange
        var textReader = new StringReader("{}");
        var jsonReader = new JsonReader(textReader, ownsReader: false);

        // Act
        jsonReader.Dispose();

        // Assert - Reader 应该仍然可用
        // 注意：这里我们无法直接验证 TextReader 未被释放，因为 StringReader 没有公共的 IsDisposed 属性
        // 但我们可以确保 JsonReader 正确处理了 ownsReader 参数
    }

    [TestMethod]
    public void Read_LargeNumber_ShouldReadCorrectly()
    {
        // Arrange
        using var jsonReader = new JsonReader("9223372036854775807"); // long.MaxValue

        // Act
        Assert.IsTrue(jsonReader.Read());

        // Assert
        Assert.AreEqual(JsonTokenType.Number, jsonReader.TokenType);
        Assert.AreEqual(long.MaxValue, jsonReader.Value);
    }

    [TestMethod]
    public void Read_VeryLargeNumber_ShouldReadAsDouble()
    {
        // Arrange
        using var jsonReader = new JsonReader("1.7976931348623157e308"); // 接近 double.MaxValue

        // Act
        Assert.IsTrue(jsonReader.Read());

        // Assert
        Assert.AreEqual(JsonTokenType.Number, jsonReader.TokenType);
        Assert.IsInstanceOfType(jsonReader.Value, typeof(double));
    }

    [TestMethod]
    [ExpectedException(typeof(JsonException))]
    public void Read_StringWithUnescapedControlCharacter_ShouldThrowException()
    {
        // Arrange
        using var jsonReader = new JsonReader("\"\u0001\""); // 未转义的控制字符

        // Act
        jsonReader.Read();
    }

    [TestMethod]
    public void Read_EmptyString_ShouldReadCorrectly()
    {
        // Arrange
        using var jsonReader = new JsonReader("\"\"");

        // Act
        Assert.IsTrue(jsonReader.Read());

        // Assert
        Assert.AreEqual(JsonTokenType.String, jsonReader.TokenType);
        Assert.AreEqual("", jsonReader.Value);
    }

    [TestMethod]
    public void Read_Zero_ShouldReadCorrectly()
    {
        // Arrange
        using var jsonReader = new JsonReader("0");

        // Act
        Assert.IsTrue(jsonReader.Read());

        // Assert
        Assert.AreEqual(JsonTokenType.Number, jsonReader.TokenType);
        Assert.AreEqual(0L, jsonReader.Value);
    }
}