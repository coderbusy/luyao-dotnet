using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace LuYao;

/// <summary>
/// Unit tests for the Check validation utility class.
/// </summary>
[TestClass]
public class CheckTests
{
    #region NotNull Method Tests

    [TestMethod]
    public void NotNull_WithNonNullValue_ReturnsValue()
    {
        // Arrange
        var value = "test";

        // Act
        var result = Check.NotNull(value, nameof(value));

        // Assert
        Assert.AreEqual(value, result);
    }

    [TestMethod]
    public void NotNull_WithNullValue_ThrowsArgumentNullException()
    {
        // Arrange
        string? value = null;

        // Act & Assert
        try
        {
            Check.NotNull(value, nameof(value));
            Assert.Fail("Expected ArgumentNullException");
        }
        catch (ArgumentNullException)
        {
            // Expected
        }
    }

    [TestMethod]
    public void NotNull_WithNullValueAndMessage_ThrowsArgumentNullException()
    {
        // Arrange
        string? value = null;

        // Act & Assert
        try
        {
            Check.NotNull(value, nameof(value), "Custom error message");
            Assert.Fail("Expected ArgumentNullException");
        }
        catch (ArgumentNullException)
        {
            // Expected
        }
    }

    [TestMethod]
    public void NotNull_WithNullValueAndMessage_ContainsCustomMessage()
    {
        // Arrange
        string? value = null;
        var customMessage = "Custom error message";

        // Act & Assert
        try
        {
            Check.NotNull(value, nameof(value), customMessage);
            Assert.Fail("Expected ArgumentNullException");
        }
        catch (ArgumentNullException ex)
        {
            Assert.IsTrue(ex.Message.Contains(customMessage));
        }
    }

    #endregion

    #region NotNull String Method Tests

    [TestMethod]
    public void NotNull_String_WithValidValue_ReturnsValue()
    {
        // Arrange
        var value = "test string";

        // Act
        var result = Check.NotNull(value, nameof(value));

        // Assert
        Assert.AreEqual(value, result);
    }

    [TestMethod]
    public void NotNull_String_WithNullValue_ThrowsArgumentException()
    {
        // Arrange
        string? value = null;

        // Act & Assert
        try
        {
            Check.NotNull(value, nameof(value));
            Assert.Fail("Expected ArgumentException");
        }
        catch (ArgumentException)
        {
            // Expected
        }
    }

    [TestMethod]
    public void NotNull_String_WithExceedingMaxLength_ThrowsArgumentException()
    {
        // Arrange
        var value = "test string";

        // Act & Assert
        try
        {
            Check.NotNull(value, nameof(value), maxLength: 5);
            Assert.Fail("Expected ArgumentException");
        }
        catch (ArgumentException)
        {
            // Expected
        }
    }

    [TestMethod]
    public void NotNull_String_WithBelowMinLength_ThrowsArgumentException()
    {
        // Arrange
        var value = "test";

        // Act & Assert
        try
        {
            Check.NotNull(value, nameof(value), minLength: 10);
            Assert.Fail("Expected ArgumentException");
        }
        catch (ArgumentException)
        {
            // Expected
        }
    }

    [TestMethod]
    public void NotNull_String_WithValidLength_ReturnsValue()
    {
        // Arrange
        var value = "test";

        // Act
        var result = Check.NotNull(value, nameof(value), maxLength: 10, minLength: 2);

        // Assert
        Assert.AreEqual(value, result);
    }

    #endregion

    #region NotNullOrWhiteSpace Method Tests

    [TestMethod]
    public void NotNullOrWhiteSpace_WithValidValue_ReturnsValue()
    {
        // Arrange
        var value = "test";

        // Act
        var result = Check.NotNullOrWhiteSpace(value, nameof(value));

        // Assert
        Assert.AreEqual(value, result);
    }

    [TestMethod]
    public void NotNullOrWhiteSpace_WithNullValue_ThrowsArgumentException()
    {
        // Arrange
        string? value = null;

        // Act & Assert
        try
        {
            Check.NotNullOrWhiteSpace(value, nameof(value));
            Assert.Fail("Expected ArgumentException");
        }
        catch (ArgumentException)
        {
            // Expected
        }
    }

    [TestMethod]
    public void NotNullOrWhiteSpace_WithEmptyValue_ThrowsArgumentException()
    {
        // Arrange
        var value = "";

        // Act & Assert
        try
        {
            Check.NotNullOrWhiteSpace(value, nameof(value));
            Assert.Fail("Expected ArgumentException");
        }
        catch (ArgumentException)
        {
            // Expected
        }
    }

    [TestMethod]
    public void NotNullOrWhiteSpace_WithWhitespaceValue_ThrowsArgumentException()
    {
        // Arrange
        var value = "   ";

        // Act & Assert
        try
        {
            Check.NotNullOrWhiteSpace(value, nameof(value));
            Assert.Fail("Expected ArgumentException");
        }
        catch (ArgumentException)
        {
            // Expected
        }
    }

    [TestMethod]
    public void NotNullOrWhiteSpace_WithExceedingMaxLength_ThrowsArgumentException()
    {
        // Arrange
        var value = "test string";

        // Act & Assert
        try
        {
            Check.NotNullOrWhiteSpace(value, nameof(value), maxLength: 5);
            Assert.Fail("Expected ArgumentException");
        }
        catch (ArgumentException)
        {
            // Expected
        }
    }

    [TestMethod]
    public void NotNullOrWhiteSpace_WithBelowMinLength_ThrowsArgumentException()
    {
        // Arrange
        var value = "test";

        // Act & Assert
        try
        {
            Check.NotNullOrWhiteSpace(value, nameof(value), minLength: 10);
            Assert.Fail("Expected ArgumentException");
        }
        catch (ArgumentException)
        {
            // Expected
        }
    }

    #endregion

    #region NotNullOrEmpty Method Tests

    [TestMethod]
    public void NotNullOrEmpty_WithValidValue_ReturnsValue()
    {
        // Arrange
        var value = "test";

        // Act
        var result = Check.NotNullOrEmpty(value, nameof(value));

        // Assert
        Assert.AreEqual(value, result);
    }

    [TestMethod]
    public void NotNullOrEmpty_WithNullValue_ThrowsArgumentException()
    {
        // Arrange
        string? value = null;

        // Act & Assert
        try
        {
            Check.NotNullOrEmpty(value, nameof(value));
            Assert.Fail("Expected ArgumentException");
        }
        catch (ArgumentException)
        {
            // Expected
        }
    }

    [TestMethod]
    public void NotNullOrEmpty_WithEmptyValue_ThrowsArgumentException()
    {
        // Arrange
        var value = "";

        // Act & Assert
        try
        {
            Check.NotNullOrEmpty(value, nameof(value));
            Assert.Fail("Expected ArgumentException");
        }
        catch (ArgumentException)
        {
            // Expected
        }
    }

    [TestMethod]
    public void NotNullOrEmpty_WithWhitespaceValue_ReturnsValue()
    {
        // Arrange
        var value = "   ";

        // Act
        var result = Check.NotNullOrEmpty(value, nameof(value));

        // Assert
        Assert.AreEqual(value, result);
    }

    #endregion

    #region NotNullOrEmpty Collection Method Tests

    [TestMethod]
    public void NotNullOrEmpty_Collection_WithNonEmptyCollection_ReturnsCollection()
    {
        // Arrange
        var collection = new List<int> { 1, 2, 3 };

        // Act
        var result = Check.NotNullOrEmpty(collection, nameof(collection));

        // Assert
        Assert.AreEqual(collection, result);
    }

    [TestMethod]
    public void NotNullOrEmpty_Collection_WithNullCollection_ThrowsArgumentException()
    {
        // Arrange
        List<int>? collection = null;

        // Act & Assert
        try
        {
            Check.NotNullOrEmpty(collection, nameof(collection));
            Assert.Fail("Expected ArgumentException");
        }
        catch (ArgumentException)
        {
            // Expected
        }
    }

    [TestMethod]
    public void NotNullOrEmpty_Collection_WithEmptyCollection_ThrowsArgumentException()
    {
        // Arrange
        var collection = new List<int>();

        // Act & Assert
        try
        {
            Check.NotNullOrEmpty(collection, nameof(collection));
            Assert.Fail("Expected ArgumentException");
        }
        catch (ArgumentException)
        {
            // Expected
        }
    }

    #endregion

    #region AssignableTo Method Tests

    [TestMethod]
    public void AssignableTo_WithValidAssignable_ReturnsType()
    {
        // Arrange
        var type = typeof(ArgumentException);

        // Act
        var result = Check.AssignableTo<Exception>(type, nameof(type));

        // Assert
        Assert.AreEqual(type, result);
    }

    [TestMethod]
    public void AssignableTo_WithNonAssignable_ThrowsArgumentException()
    {
        // Arrange
        var type = typeof(string);

        // Act & Assert
        try
        {
            Check.AssignableTo<Exception>(type, nameof(type));
            Assert.Fail("Expected ArgumentException");
        }
        catch (ArgumentException)
        {
            // Expected
        }
    }

    [TestMethod]
    public void AssignableTo_WithNullType_ThrowsArgumentNullException()
    {
        // Arrange
        Type? type = null;

        // Act & Assert
        try
        {
            Check.AssignableTo<Exception>(type, nameof(type));
            Assert.Fail("Expected ArgumentNullException");
        }
        catch (ArgumentNullException)
        {
            // Expected
        }
    }

    #endregion

    #region Length Method Tests

    [TestMethod]
    public void Length_WithValidLength_ReturnsValue()
    {
        // Arrange
        var value = "test";

        // Act
        var result = Check.Length(value, nameof(value), maxLength: 10);

        // Assert
        Assert.AreEqual(value, result);
    }

    [TestMethod]
    public void Length_WithNullValue_ReturnsNull()
    {
        // Arrange
        string? value = null;

        // Act
        var result = Check.Length(value, nameof(value), maxLength: 10);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Length_WithMinLengthAndNullValue_ThrowsArgumentException()
    {
        // Arrange
        string? value = null;

        // Act & Assert
        try
        {
            Check.Length(value, nameof(value), maxLength: 10, minLength: 1);
            Assert.Fail("Expected ArgumentException");
        }
        catch (ArgumentException)
        {
            // Expected
        }
    }

    [TestMethod]
    public void Length_WithExceedingMaxLength_ThrowsArgumentException()
    {
        // Arrange
        var value = "test string";

        // Act & Assert
        try
        {
            Check.Length(value, nameof(value), maxLength: 5);
            Assert.Fail("Expected ArgumentException");
        }
        catch (ArgumentException)
        {
            // Expected
        }
    }

    #endregion

    #region Positive Method Tests

    [TestMethod]
    public void Positive_Int32_WithPositiveValue_ReturnsValue()
    {
        // Arrange
        int value = 10;

        // Act
        var result = Check.Positive(value, nameof(value));

        // Assert
        Assert.AreEqual(value, result);
    }

    [TestMethod]
    public void Positive_Int32_WithZero_ThrowsArgumentException()
    {
        // Arrange
        int value = 0;

        // Act & Assert
        try
        {
            Check.Positive(value, nameof(value));
            Assert.Fail("Expected ArgumentException");
        }
        catch (ArgumentException)
        {
            // Expected
        }
    }

    [TestMethod]
    public void Positive_Int32_WithNegativeValue_ThrowsArgumentException()
    {
        // Arrange
        int value = -10;

        // Act & Assert
        try
        {
            Check.Positive(value, nameof(value));
            Assert.Fail("Expected ArgumentException");
        }
        catch (ArgumentException)
        {
            // Expected
        }
    }

    [TestMethod]
    public void Positive_Int64_WithPositiveValue_ReturnsValue()
    {
        // Arrange
        long value = 100L;

        // Act
        var result = Check.Positive(value, nameof(value));

        // Assert
        Assert.AreEqual(value, result);
    }

    [TestMethod]
    public void Positive_Int64_WithNegativeValue_ThrowsArgumentException()
    {
        // Arrange
        long value = -100L;

        // Act & Assert
        try
        {
            Check.Positive(value, nameof(value));
            Assert.Fail("Expected ArgumentException");
        }
        catch (ArgumentException)
        {
            // Expected
        }
    }

    [TestMethod]
    public void Positive_Double_WithPositiveValue_ReturnsValue()
    {
        // Arrange
        double value = 3.14;

        // Act
        var result = Check.Positive(value, nameof(value));

        // Assert
        Assert.AreEqual(value, result);
    }

    [TestMethod]
    public void Positive_Decimal_WithPositiveValue_ReturnsValue()
    {
        // Arrange
        decimal value = 99.99m;

        // Act
        var result = Check.Positive(value, nameof(value));

        // Assert
        Assert.AreEqual(value, result);
    }

    #endregion

    #region Range Method Tests

    [TestMethod]
    public void Range_Int32_WithValueInRange_ReturnsValue()
    {
        // Arrange
        int value = 5;

        // Act
        var result = Check.Range(value, nameof(value), minimumValue: 0, maximumValue: 10);

        // Assert
        Assert.AreEqual(value, result);
    }

    [TestMethod]
    public void Range_Int32_WithValueBelowMinimum_ThrowsArgumentException()
    {
        // Arrange
        int value = -5;

        // Act & Assert
        try
        {
            Check.Range(value, nameof(value), minimumValue: 0, maximumValue: 10);
            Assert.Fail("Expected ArgumentException");
        }
        catch (ArgumentException)
        {
            // Expected
        }
    }

    [TestMethod]
    public void Range_Int32_WithValueAboveMaximum_ThrowsArgumentException()
    {
        // Arrange
        int value = 15;

        // Act & Assert
        try
        {
            Check.Range(value, nameof(value), minimumValue: 0, maximumValue: 10);
            Assert.Fail("Expected ArgumentException");
        }
        catch (ArgumentException)
        {
            // Expected
        }
    }

    [TestMethod]
    public void Range_Int32_WithValueAtBoundaries_ReturnsValue()
    {
        // Arrange
        int minValue = 0;
        int maxValue = 10;

        // Act
        var resultMin = Check.Range(minValue, nameof(minValue), minimumValue: 0, maximumValue: 10);
        var resultMax = Check.Range(maxValue, nameof(maxValue), minimumValue: 0, maximumValue: 10);

        // Assert
        Assert.AreEqual(minValue, resultMin);
        Assert.AreEqual(maxValue, resultMax);
    }

    [TestMethod]
    public void Range_Double_WithValueInRange_ReturnsValue()
    {
        // Arrange
        double value = 5.5;

        // Act
        var result = Check.Range(value, nameof(value), minimumValue: 0.0, maximumValue: 10.0);

        // Assert
        Assert.AreEqual(value, result);
    }

    [TestMethod]
    public void Range_Double_WithValueBelowMinimum_ThrowsArgumentException()
    {
        // Arrange
        double value = -5.5;

        // Act & Assert
        try
        {
            Check.Range(value, nameof(value), minimumValue: 0.0, maximumValue: 10.0);
            Assert.Fail("Expected ArgumentException");
        }
        catch (ArgumentException)
        {
            // Expected
        }
    }

    [TestMethod]
    public void Range_Decimal_WithValueInRange_ReturnsValue()
    {
        // Arrange
        decimal value = 5.5m;

        // Act
        var result = Check.Range(value, nameof(value), minimumValue: 0m, maximumValue: 10m);

        // Assert
        Assert.AreEqual(value, result);
    }

    #endregion

    #region NotDefaultOrNull Method Tests

    [TestMethod]
    public void NotDefaultOrNull_WithNonDefaultValue_ReturnsValue()
    {
        // Arrange
        int? value = 42;

        // Act
        var result = Check.NotDefaultOrNull(value, nameof(value));

        // Assert
        Assert.AreEqual(42, result);
    }

    [TestMethod]
    public void NotDefaultOrNull_WithNullValue_ThrowsArgumentException()
    {
        // Arrange
        int? value = null;

        // Act & Assert
        try
        {
            Check.NotDefaultOrNull(value, nameof(value));
            Assert.Fail("Expected ArgumentException");
        }
        catch (ArgumentException)
        {
            // Expected
        }
    }

    [TestMethod]
    public void NotDefaultOrNull_WithDefaultValue_ThrowsArgumentException()
    {
        // Arrange
        int? value = 0;

        // Act & Assert
        try
        {
            Check.NotDefaultOrNull(value, nameof(value));
            Assert.Fail("Expected ArgumentException");
        }
        catch (ArgumentException)
        {
            // Expected
        }
    }

    [TestMethod]
    public void NotDefaultOrNull_WithGuidValue_ReturnsValue()
    {
        // Arrange
        Guid? value = Guid.NewGuid();

        // Act
        var result = Check.NotDefaultOrNull(value, nameof(value));

        // Assert
        Assert.AreEqual(value.Value, result);
    }

    [TestMethod]
    public void NotDefaultOrNull_WithDefaultGuid_ThrowsArgumentException()
    {
        // Arrange
        Guid? value = Guid.Empty;

        // Act & Assert
        try
        {
            Check.NotDefaultOrNull(value, nameof(value));
            Assert.Fail("Expected ArgumentException");
        }
        catch (ArgumentException)
        {
            // Expected
        }
    }

    #endregion
}
