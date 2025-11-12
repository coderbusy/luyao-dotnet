using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace LuYao.Text.Json;

[TestClass]
public class JsonWriterTests
{
    private StringBuilder _stringBuilder;
    private JsonWriter _writer;

    [TestInitialize]
    public void Setup()
    {
        _stringBuilder = new StringBuilder();
        _writer = new JsonWriter(_stringBuilder);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _writer?.Dispose();
    }

    #region Constructor Tests

    [TestMethod]
    public void Constructor_WithTextWriter_ShouldCreateWriter()
    {
        // Arrange
        using var stringWriter = new StringWriter();

        // Act
        using var writer = new JsonWriter(stringWriter);

        // Assert
        Assert.IsNotNull(writer);
    }

    [TestMethod]
    public void Constructor_WithStringBuilder_ShouldCreateWriter()
    {
        // Arrange
        var sb = new StringBuilder();

        // Act
        using var writer = new JsonWriter(sb);

        // Assert
        Assert.IsNotNull(writer);
    }

    [TestMethod]
    public void Constructor_WithNullTextWriter_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => new JsonWriter((TextWriter)null));
    }

    [TestMethod]
    public void Constructor_WithIndentation_ShouldCreateIndentedWriter()
    {
        // Arrange
        var sb = new StringBuilder();

        // Act
        using var writer = new JsonWriter(sb, indented: true, indentString: "    ");

        // Assert
        Assert.IsNotNull(writer);
    }

    #endregion

    #region Object Tests

    [TestMethod]
    public void WriteStartObject_ShouldWriteOpeningBrace()
    {
        // Act
        _writer.WriteStartObject();
        _writer.Flush();

        // Assert
        Assert.AreEqual("{", _stringBuilder.ToString());
    }

    [TestMethod]
    public void WriteEndObject_ShouldWriteClosingBrace()
    {
        // Act
        _writer.WriteStartObject();
        _writer.WriteEndObject();
        _writer.Flush();

        // Assert
        Assert.AreEqual("{}", _stringBuilder.ToString());
    }

    [TestMethod]
    public void WriteObject_WithIndentation_ShouldFormatCorrectly()
    {
        // Arrange
        var sb = new StringBuilder();
        using var writer = new JsonWriter(sb, indented: true);

        // Act
        writer.WriteStartObject();
        writer.WritePropertyName("test");
        writer.WriteValue("value");
        writer.WriteEndObject();
        writer.Flush();

        // Assert
        var result = sb.ToString();
        Assert.IsTrue(result.Contains("{\n"));
        Assert.IsTrue(result.Contains("  \"test\": \"value\"\n"));
        Assert.IsTrue(result.Contains("}"));
    }

    [TestMethod]
    public void WriteEndObject_WithoutStartObject_ShouldThrowInvalidOperationException()
    {
        // Act & Assert
        Assert.ThrowsExactly<InvalidOperationException>(() => _writer.WriteEndObject());
    }

    #endregion

    #region Array Tests

    [TestMethod]
    public void WriteStartArray_ShouldWriteOpeningBracket()
    {
        // Act
        _writer.WriteStartArray();
        _writer.Flush();

        // Assert
        Assert.AreEqual("[", _stringBuilder.ToString());
    }

    [TestMethod]
    public void WriteEndArray_ShouldWriteClosingBracket()
    {
        // Act
        _writer.WriteStartArray();
        _writer.WriteEndArray();
        _writer.Flush();

        // Assert
        Assert.AreEqual("[]", _stringBuilder.ToString());
    }

    [TestMethod]
    public void WriteArray_WithIndentation_ShouldFormatCorrectly()
    {
        // Arrange
        var sb = new StringBuilder();
        using var writer = new JsonWriter(sb, indented: true);

        // Act
        writer.WriteStartArray();
        writer.WriteValue(1);
        writer.WriteValue(2);
        writer.WriteEndArray();
        writer.Flush();

        // Assert
        var result = sb.ToString();
        Assert.IsTrue(result.Contains("[\n"));
        Assert.IsTrue(result.Contains("1,\n"));
        Assert.IsTrue(result.Contains("2\n"));
        Assert.IsTrue(result.Contains("]"));
    }

    [TestMethod]
    public void WriteEndArray_WithoutStartArray_ShouldThrowInvalidOperationException()
    {
        // Act & Assert
        Assert.ThrowsExactly<InvalidOperationException>(() => _writer.WriteEndArray());
    }

    #endregion

    #region Property Tests

    [TestMethod]
    public void WritePropertyName_ValidName_ShouldWriteQuotedName()
    {
        // Act
        _writer.WriteStartObject();
        _writer.WritePropertyName("test");
        _writer.Flush();

        // Assert
        Assert.AreEqual("{\"test\":", _stringBuilder.ToString());
    }

    [TestMethod]
    public void WritePropertyName_WithIndentation_ShouldIncludeSpace()
    {
        // Arrange
        var sb = new StringBuilder();
        using var writer = new JsonWriter(sb, indented: true);

        // Act
        writer.WriteStartObject();
        writer.WritePropertyName("test");
        writer.Flush();

        // Assert
        var result = sb.ToString();
        Assert.IsTrue(result.Contains("\"test\": "));
    }

    [TestMethod]
    public void WritePropertyName_NullName_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => _writer.WritePropertyName(null));
    }

    #endregion

    #region String Value Tests

    [TestMethod]
    public void WriteValue_String_ShouldWriteQuotedString()
    {
        // Act
        _writer.WriteValue("test");
        _writer.Flush();

        // Assert
        Assert.AreEqual("\"test\"", _stringBuilder.ToString());
    }

    [TestMethod]
    public void WriteValue_NullString_ShouldWriteNull()
    {
        // Act
        _writer.WriteValue((string)null);
        _writer.Flush();

        // Assert
        Assert.AreEqual("null", _stringBuilder.ToString());
    }

    [TestMethod]
    public void WriteValue_StringWithEscapeCharacters_ShouldEscapeCorrectly()
    {
        // Arrange
        var testCases = new[]
        {
            ("\"", "\\\""),
            ("\\", "\\\\"),
            ("\b", "\\b"),
            ("\f", "\\f"),
            ("\n", "\\n"),
            ("\r", "\\r"),
            ("\t", "\\t")
        };

        foreach (var (input, expected) in testCases)
        {
            // Arrange
            var sb = new StringBuilder();
            using var writer = new JsonWriter(sb);

            // Act
            writer.WriteValue(input);
            writer.Flush();

            // Assert
            Assert.AreEqual($"\"{expected}\"", sb.ToString(), $"Failed for input: {input}");
        }
    }

    [TestMethod]
    public void WriteValue_StringWithControlCharacters_ShouldUnicodeEscape()
    {
        // Act
        _writer.WriteValue("\u0001");
        _writer.Flush();

        // Assert
        Assert.AreEqual("\"\\u0001\"", _stringBuilder.ToString());
    }

    #endregion

    #region Numeric Value Tests

    [TestMethod]
    public void WriteValue_Long_ShouldWriteNumber()
    {
        // Act
        _writer.WriteValue(12345L);
        _writer.Flush();

        // Assert
        Assert.AreEqual("12345", _stringBuilder.ToString());
    }

    [TestMethod]
    public void WriteValue_NegativeLong_ShouldWriteNegativeNumber()
    {
        // Act
        _writer.WriteValue(-12345L);
        _writer.Flush();

        // Assert
        Assert.AreEqual("-12345", _stringBuilder.ToString());
    }

    [TestMethod]
    public void WriteValue_Double_ShouldWriteNumber()
    {
        // Act
        _writer.WriteValue(123.45);
        _writer.Flush();

        // Assert
        Assert.AreEqual("123.45", _stringBuilder.ToString());
    }

    [TestMethod]
    public void WriteValue_DoubleNaN_ShouldWriteNull()
    {
        // Act
        _writer.WriteValue(double.NaN);
        _writer.Flush();

        // Assert
        Assert.AreEqual("null", _stringBuilder.ToString());
    }

    [TestMethod]
    public void WriteValue_DoubleInfinity_ShouldWriteNull()
    {
        // Act
        _writer.WriteValue(double.PositiveInfinity);
        _writer.Flush();

        // Assert
        Assert.AreEqual("null", _stringBuilder.ToString());
    }

    [TestMethod]
    public void WriteValue_DoubleNegativeInfinity_ShouldWriteNull()
    {
        // Act
        _writer.WriteValue(double.NegativeInfinity);
        _writer.Flush();

        // Assert
        Assert.AreEqual("null", _stringBuilder.ToString());
    }

    #endregion

    #region Boolean Value Tests

    [TestMethod]
    public void WriteValue_BooleanTrue_ShouldWriteTrue()
    {
        // Act
        _writer.WriteValue(true);
        _writer.Flush();

        // Assert
        Assert.AreEqual("true", _stringBuilder.ToString());
    }

    [TestMethod]
    public void WriteValue_BooleanFalse_ShouldWriteFalse()
    {
        // Act
        _writer.WriteValue(false);
        _writer.Flush();

        // Assert
        Assert.AreEqual("false", _stringBuilder.ToString());
    }

    #endregion

    #region Null Value Tests

    [TestMethod]
    public void WriteNull_ShouldWriteNull()
    {
        // Act
        _writer.WriteNull();
        _writer.Flush();

        // Assert
        Assert.AreEqual("null", _stringBuilder.ToString());
    }

    #endregion

    #region Raw Value Tests

    [TestMethod]
    public void WriteRaw_ValidJson_ShouldWriteRawValue()
    {
        // Act
        _writer.WriteRaw("{\"key\":\"value\"}");
        _writer.Flush();

        // Assert
        Assert.AreEqual("{\"key\":\"value\"}", _stringBuilder.ToString());
    }

    [TestMethod]
    public void WriteRaw_NullValue_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => _writer.WriteRaw(null));
    }

    #endregion

    #region Complex JSON Tests

    [TestMethod]
    public void WriteComplexObject_ShouldProduceValidJson()
    {
        // Act
        _writer.WriteStartObject();
        _writer.WritePropertyName("name");
        _writer.WriteValue("John");
        _writer.WritePropertyName("age");
        _writer.WriteValue(30L);
        _writer.WritePropertyName("active");
        _writer.WriteValue(true);
        _writer.WritePropertyName("address");
        _writer.WriteStartObject();
        _writer.WritePropertyName("city");
        _writer.WriteValue("New York");
        _writer.WriteEndObject();
        _writer.WriteEndObject();
        _writer.Flush();

        // Assert
        var expected = "{\"name\":\"John\",\"age\":30,\"active\":true,\"address\":{\"city\":\"New York\"}}";
        Assert.AreEqual(expected, _stringBuilder.ToString());
    }

    [TestMethod]
    public void WriteComplexArray_ShouldProduceValidJson()
    {
        // Act
        _writer.WriteStartArray();
        _writer.WriteValue("item1");
        _writer.WriteValue(42L);
        _writer.WriteStartObject();
        _writer.WritePropertyName("nested");
        _writer.WriteValue("value");
        _writer.WriteEndObject();
        _writer.WriteEndArray();
        _writer.Flush();

        // Assert
        var expected = "[\"item1\",42,{\"nested\":\"value\"}]";
        Assert.AreEqual(expected, _stringBuilder.ToString());
    }

    [TestMethod]
    public void WriteNestedArraysAndObjects_ShouldProduceValidJson()
    {
        // Act
        _writer.WriteStartObject();
        _writer.WritePropertyName("data");
        _writer.WriteStartArray();
        _writer.WriteStartObject();
        _writer.WritePropertyName("id");
        _writer.WriteValue(1L);
        _writer.WritePropertyName("tags");
        _writer.WriteStartArray();
        _writer.WriteValue("tag1");
        _writer.WriteValue("tag2");
        _writer.WriteEndArray();
        _writer.WriteEndObject();
        _writer.WriteEndArray();
        _writer.WriteEndObject();
        _writer.Flush();

        // Assert
        var expected = "{\"data\":[{\"id\":1,\"tags\":[\"tag1\",\"tag2\"]}]}";
        Assert.AreEqual(expected, _stringBuilder.ToString());
    }

    #endregion

    #region Indentation Tests

    [TestMethod]
    public void WriteIndentedJson_ShouldFormatWithNewlinesAndIndentation()
    {
        // Arrange
        var sb = new StringBuilder();
        using var writer = new JsonWriter(sb, indented: true, indentString: "  ");

        // Act
        writer.WriteStartObject();
        writer.WritePropertyName("name");
        writer.WriteValue("John");
        writer.WritePropertyName("items");
        writer.WriteStartArray();
        writer.WriteValue(1L);
        writer.WriteValue(2L);
        writer.WriteEndArray();
        writer.WriteEndObject();
        writer.Flush();

        // Assert
        var result = sb.ToString();
        Assert.IsTrue(result.Contains("{\n"));
        Assert.IsTrue(result.Contains("  \"name\": \"John\",\n"));
        Assert.IsTrue(result.Contains("  \"items\": [\n"));
        Assert.IsTrue(result.Contains("    1,\n"));
        Assert.IsTrue(result.Contains("    2\n"));
        Assert.IsTrue(result.Contains("  ]\n"));
        Assert.IsTrue(result.Contains("}"));
    }

    [TestMethod]
    public void WriteIndentedJson_CustomIndentString_ShouldUseCustomIndentation()
    {
        // Arrange
        var sb = new StringBuilder();
        using var writer = new JsonWriter(sb, indented: true, indentString: "\t");

        // Act
        writer.WriteStartObject();
        writer.WritePropertyName("key");
        writer.WriteValue("value");
        writer.WriteEndObject();
        writer.Flush();

        // Assert
        var result = sb.ToString();
        Assert.IsTrue(result.Contains("\t\"key\": \"value\""));
    }

    #endregion

    #region Buffer Management Tests

    [TestMethod]
    public void WriteValue_LongString_ShouldHandleBuffering()
    {
        // Arrange
        var longString = new string('a', 10000);

        // Act
        _writer.WriteValue(longString);
        _writer.Flush();

        // Assert
        Assert.AreEqual($"\"{longString}\"", _stringBuilder.ToString());
    }

    [TestMethod]
    public void WriteMultipleValues_ShouldHandleBufferFlushes()
    {
        // Act
        for (int i = 0; i < 1000; i++)
        {
            if (i == 0)
                _writer.WriteStartArray();

            _writer.WriteValue($"item{i}");

            if (i == 999)
                _writer.WriteEndArray();
        }
        _writer.Flush();

        // Assert
        var result = _stringBuilder.ToString();
        Assert.IsTrue(result.StartsWith("["));
        Assert.IsTrue(result.EndsWith("]"));
        Assert.IsTrue(result.Contains("\"item0\""));
        Assert.IsTrue(result.Contains("\"item999\""));
    }

    #endregion

    #region Error Handling Tests

    [TestMethod]
    public void WriteInvalidStructure_ShouldThrowInvalidOperationException()
    {
        // Act & Assert
        _writer.WriteStartObject();
        Assert.ThrowsExactly<InvalidOperationException>(() => _writer.WriteEndArray());
    }

    [TestMethod]
    public void WritePropertyNameOutsideObject_ShouldNotThrow()
    {
        // This test verifies current behavior - property names can be written at root level
        // Act
        _writer.WritePropertyName("test");
        _writer.WriteValue("value");
        _writer.Flush();

        // Assert
        Assert.AreEqual("\"test\":\"value\"", _stringBuilder.ToString());
    }

    #endregion

    #region Disposal Tests

    [TestMethod]
    public void Dispose_ShouldFlushAndCleanup()
    {
        // Arrange
        var sb = new StringBuilder();
        var writer = new JsonWriter(sb);

        // Act
        writer.WriteValue("test");
        writer.Dispose();

        // Assert
        Assert.AreEqual("\"test\"", sb.ToString());
    }

    [TestMethod]
    public void Dispose_CalledTwice_ShouldNotThrow()
    {
        // Arrange
        var sb = new StringBuilder();
        var writer = new JsonWriter(sb);

        // Act & Assert
        writer.Dispose();
        writer.Dispose(); // Should not throw
    }

    [TestMethod]
    public void Dispose_WithOwnedWriter_ShouldDisposeUnderlyingWriter()
    {
        // Arrange
        var stringWriter = new StringWriter();
        var writer = new JsonWriter(stringWriter, ownsWriter: true);

        // Act
        writer.WriteValue("test");
        writer.Dispose();

        // Assert
        Assert.AreEqual("\"test\"", stringWriter.ToString());
        // Note: We can't easily verify that stringWriter is disposed without reflection
    }

    #endregion

    #region Culture Tests

    [TestMethod]
    public void WriteValue_DoubleWithDifferentCulture_ShouldUseInvariantCulture()
    {
        // Arrange
        var originalCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("de-DE"); // Uses comma as decimal separator

            // Act
            _writer.WriteValue(123.45);
            _writer.Flush();

            // Assert
            Assert.AreEqual("123.45", _stringBuilder.ToString()); // Should use dot, not comma
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }

    [TestMethod]
    public void WriteValue_LongWithDifferentCulture_ShouldUseInvariantCulture()
    {
        // Arrange
        var originalCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-IN"); // Uses different number formatting

            // Act
            _writer.WriteValue(1234567L);
            _writer.Flush();

            // Assert
            Assert.AreEqual("1234567", _stringBuilder.ToString()); // No thousand separators
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }

    #endregion
}