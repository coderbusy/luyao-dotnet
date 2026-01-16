using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LuYao.Encoders;

[TestClass]
public class Base62Tests
{
    [TestMethod]
    public void ToBase62_EmptyArray_ReturnsEmptyString()
    {
        // Arrange
        byte[] input = Array.Empty<byte>();

        // Act
        string result = Base62.ToBase62(input);

        // Assert
        Assert.AreEqual(string.Empty, result);
    }

    [TestMethod]
    public void ToBase62_SimpleBytes_ReturnsExpectedBase62String()
    {
        // Arrange
        byte[] input = { 1, 2, 3, 4, 5 };

        // Act
        string result = Base62.ToBase62(input);

        // Assert
        // 预期值可通过 FromBase62 验证
        CollectionAssert.AreEqual(input, Base62.FromBase62(result));
    }

    [TestMethod]
    public void ToBase62_UseInvertedCharset_EncodingAndDecodingAreConsistent()
    {
        // Arrange
        byte[] input = { 10, 20, 30, 40, 50 };

        // Act
        string encoded = Base62.ToBase62(input, inverted: true);
        byte[] decoded = Base62.FromBase62(encoded, inverted: true);

        // Assert
        CollectionAssert.AreEqual(input, decoded);
    }

    [TestMethod]
    public void FromBase62_EmptyString_ThrowsArgumentNullException()
    {
        // Arrange
        string input = "";

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => Base62.FromBase62(input));
    }

    [TestMethod]
    public void FromBase62_WhitespaceString_ThrowsArgumentNullException()
    {
        // Arrange
        string input = "   ";

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => Base62.FromBase62(input));
    }

    [TestMethod]
    public void ToBase62AndFromBase62_RandomBytes_RoundTripIsConsistent()
    {
        // Arrange
        var rng = new Random(42);
        byte[] input = new byte[128];
        rng.NextBytes(input);

        // Act
        string encoded = Base62.ToBase62(input);
        byte[] decoded = Base62.FromBase62(encoded);

        // Assert
        CollectionAssert.AreEqual(input, decoded);
    }
}