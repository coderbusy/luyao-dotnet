using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LuYao.Encoders;


[TestClass]
public class LzStringTests
{
    [TestMethod]
    public void CompressToBase64_InputIsNull_ReturnsEmptyString()
    {
        var result = LzString.CompressToBase64(null);
        Assert.AreEqual(string.Empty, result);
    }

    [TestMethod]
    public void CompressToBase64_And_DecompressFromBase64_NormalString_RoundTrip()
    {
        string input = "Hello, 世界! 123";
        var compressed = LzString.CompressToBase64(input);
        var decompressed = LzString.DecompressFromBase64(compressed);
        Assert.AreEqual(input, decompressed);
    }

    [TestMethod]
    public void CompressToUTF16_InputIsNull_ReturnsEmptyString()
    {
        var result = LzString.CompressToUTF16(null);
        Assert.AreEqual(string.Empty, result);
    }

    [TestMethod]
    public void CompressToUTF16_And_DecompressFromUTF16_NormalString_RoundTrip()
    {
        string input = "测试UTF16压缩";
        var compressed = LzString.CompressToUTF16(input);
        var decompressed = LzString.DecompressFromUTF16(compressed);
        Assert.AreEqual(input, decompressed);
    }

    [TestMethod]
    public void CompressToUint8Array_And_DecompressFromUint8Array_NormalString_RoundTrip()
    {
        string input = "Uint8Array测试";
        var compressed = LzString.CompressToUint8Array(input);
        var decompressed = LzString.DecompressFromUint8Array(compressed);
        Assert.AreEqual(input, decompressed);
    }

    [TestMethod]
    public void CompressToEncodedURIComponent_And_DecompressFromEncodedURIComponent_NormalString_RoundTrip()
    {
        string input = "URI组件安全测试";
        var compressed = LzString.CompressToEncodedURIComponent(input);
        var decompressed = LzString.DecompressFromEncodedURIComponent(compressed);
        Assert.AreEqual(input, decompressed);
    }

    [TestMethod]
    public void Compress_InputIsNull_ReturnsEmptyString()
    {
        var result = LzString.Compress(null);
        Assert.AreEqual(string.Empty, result);
    }

    [TestMethod]
    public void Compress_And_Decompress_NormalString_RoundTrip()
    {
        string input = "LZ压缩算法测试";
        var compressed = LzString.Compress(input);
        var decompressed = LzString.Decompress(compressed);
        Assert.AreEqual(input, decompressed);
    }

    [TestMethod]
    public void Decompress_InputIsNullOrWhitespace_ReturnsEmptyString()
    {
        Assert.AreEqual(string.Empty, LzString.Decompress(null));
        Assert.AreEqual(string.Empty, LzString.Decompress(""));
        Assert.AreEqual(string.Empty, LzString.Decompress("   "));
    }

    [TestMethod]
    public void DecompressFromBase64_InputIsNullOrEmpty_ReturnsEmptyString()
    {
        Assert.AreEqual(string.Empty, LzString.DecompressFromBase64(null));
        Assert.AreEqual(string.Empty, LzString.DecompressFromBase64(""));
    }

    [TestMethod]
    public void DecompressFromUTF16_InputIsNullOrWhitespace_ReturnsEmptyString()
    {
        Assert.AreEqual(string.Empty, LzString.DecompressFromUTF16(null));
        Assert.AreEqual(string.Empty, LzString.DecompressFromUTF16(""));
        Assert.AreEqual(string.Empty, LzString.DecompressFromUTF16("   "));
    }

    [TestMethod]
    public void DecompressFromUint8Array_InputIsNull_ReturnsEmptyString()
    {
        Assert.AreEqual(string.Empty, LzString.DecompressFromUint8Array(null));
    }

    [TestMethod]
    public void DecompressFromEncodedURIComponent_InputIsNullOrWhitespace_ReturnsEmptyString()
    {
        Assert.AreEqual(string.Empty, LzString.DecompressFromEncodedURIComponent(null));
        Assert.AreEqual(string.Empty, LzString.DecompressFromEncodedURIComponent(""));
        Assert.AreEqual(string.Empty, LzString.DecompressFromEncodedURIComponent("   "));
    }
}