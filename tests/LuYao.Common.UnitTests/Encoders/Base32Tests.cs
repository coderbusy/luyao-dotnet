using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Encoders;

[TestClass]
public class Base32Tests
{
    [TestMethod]
    public void ToBase32_EmptyArray_ReturnsEmptyString()
    {
        var result = Base32.ToBase32(Array.Empty<byte>());
        Assert.AreEqual(string.Empty, result);
    }

    [TestMethod]
    public void ToBase32_SampleBytes_ReturnsExpectedBase32()
    {
        byte[] data = { 0x01, 0x23, 0x45, 0x67, 0x89 };
        var result = Base32.ToBase32(data);
        // 0x01 0x23 0x45 0x67 0x89 => "04HMASW9"
        Assert.AreEqual("04HMASW9", result);
    }

    [TestMethod]
    public void FromBase32_EmptyString_ReturnsEmptyArray()
    {
        var result = Base32.FromBase32(string.Empty);
        CollectionAssert.AreEqual(Array.Empty<byte>(), result);
    }

    [TestMethod]
    public void FromBase32_SampleString_ReturnsExpectedBytes()
    {
        string base32 = "04HMASW9";
        byte[] expected = { 0x01, 0x23, 0x45, 0x67, 0x89 };
        var result = Base32.FromBase32(base32);
        CollectionAssert.AreEqual(expected, result);
    }

    [TestMethod]
    public void FromBase32_InvalidCharacter_ThrowsArgumentException()
    {
        Assert.ThrowsExactly<ArgumentException>(() => Base32.FromBase32("INVALID*"));
    }

    [TestMethod]
    public void ToBase32_And_FromBase32_RoundTrip()
    {
        byte[] original = { 0xDE, 0xAD, 0xBE, 0xEF };
        var encoded = Base32.ToBase32(original);
        var decoded = Base32.FromBase32(encoded);
        CollectionAssert.AreEqual(original, decoded);
    }
}