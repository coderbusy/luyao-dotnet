using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LuYao;

[TestClass]
public class CachedObjectTests
{
    [TestMethod]
    public void Constructor_WithValue_NoExpirationDate_ObjectNotExpired()
    {
        // Arrange
        var value = "TestValue";

        // Act
        var cachedObject = new CachedObject<string>(value);

        // Assert
        Assert.AreEqual(value, cachedObject.Value);
        Assert.AreEqual(DateTimeOffset.MaxValue, cachedObject.ExpirationDate);
        Assert.IsFalse(cachedObject.IsExpired());
    }

    [TestMethod]
    public void Constructor_WithValueAndExpirationDate_ObjectExpiresCorrectly()
    {
        // Arrange
        var value = "TestValue";
        var expirationDate = DateTimeOffset.UtcNow.AddMinutes(5);

        // Act
        var cachedObject = new CachedObject<string>(value, expirationDate);

        // Assert
        Assert.AreEqual(value, cachedObject.Value);
        Assert.AreEqual(expirationDate, cachedObject.ExpirationDate);
        Assert.IsFalse(cachedObject.IsExpired());
    }

    [TestMethod]
    public void Constructor_WithValueAndDuration_ObjectExpiresAfterDuration()
    {
        // Arrange
        var value = "TestValue";
        var duration = TimeSpan.FromMinutes(5);

        // Act
        var cachedObject = new CachedObject<string>(value, duration);

        // Assert
        Assert.AreEqual(value, cachedObject.Value);
        Assert.AreEqual(cachedObject.CachedDate.Add(duration), cachedObject.ExpirationDate);
        Assert.IsFalse(cachedObject.IsExpired());
    }

    [TestMethod]
    public void IsExpired_AfterExpirationDate_ReturnsTrue()
    {
        // Arrange
        var value = "TestValue";
        var expirationDate = DateTimeOffset.UtcNow.AddSeconds(-1); // Already expired

        // Act
        var cachedObject = new CachedObject<string>(value, expirationDate);

        // Assert
        Assert.IsTrue(cachedObject.IsExpired());
    }

    [TestMethod]
    public void ToString_WithValidObject_ReturnsCorrectStringRepresentation()
    {
        // Arrange
        var value = "TestValue";
        var cachedObject = new CachedObject<string>(value);

        // Act
        var result = cachedObject.ToString();

        // Assert
        Assert.IsTrue(result.Contains($"Value: {value}"));
        Assert.IsTrue(result.Contains($"IsExpired: {cachedObject.IsExpired()}"));
    }
}