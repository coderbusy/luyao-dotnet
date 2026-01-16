using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace LuYao.Encoders;

[TestClass]
public class Ascii85Tests
{
    [TestMethod]
    public void EncodeDecode_InputIsHelloWorld_Reversible()
    {
        var ascii85 = new Ascii85();
        byte[] original = Encoding.UTF8.GetBytes("Hello, World!");
        string encoded = ascii85.Encode(original);
        byte[] decoded = ascii85.Decode(encoded);
        CollectionAssert.AreEqual(original, decoded);
    }

    [TestMethod]
    public void ToAscii85StringFromAscii85String_InputIsTest1234_Reversible()
    {
        byte[] original = Encoding.UTF8.GetBytes("Test1234");
        string encoded = Ascii85.ToAscii85String(original);
        byte[] decoded = Ascii85.FromAscii85String(encoded);
        CollectionAssert.AreEqual(original, decoded);
    }

    [TestMethod]
    public void Encode_EnforceMarksTrue_IncludesPrefixAndSuffix()
    {
        var ascii85 = new Ascii85 { EnforceMarks = true };
        byte[] data = Encoding.UTF8.GetBytes("abc");
        string encoded = ascii85.Encode(data);
        Assert.IsTrue(encoded.StartsWith(ascii85.PrefixMark));
        Assert.IsTrue(encoded.EndsWith(ascii85.SuffixMark));
    }

    [TestMethod]
    public void Encode_EnforceMarksFalse_DoesNotIncludePrefixAndSuffix()
    {
        var ascii85 = new Ascii85 { EnforceMarks = false };
        byte[] data = Encoding.UTF8.GetBytes("abc");
        string encoded = ascii85.Encode(data);
        Assert.IsFalse(encoded.StartsWith(ascii85.PrefixMark));
        Assert.IsFalse(encoded.EndsWith(ascii85.SuffixMark));
    }

    [TestMethod]
    public void Decode_EnforceMarksTrue_MissingMarks_ThrowsException()
    {
        var ascii85 = new Ascii85 { EnforceMarks = true };
        string encoded = "87cURD_*#4DfTZ";
        Assert.Throws<Exception>(() => ascii85.Decode(encoded));
    }

    [TestMethod]
    public void Decode_EnforceMarksFalse_WithoutMarks_Works()
    {
        var ascii85 = new Ascii85 { EnforceMarks = false };
        byte[] original = Encoding.UTF8.GetBytes("data");
        string encoded = ascii85.Encode(original);
        // È¥µôÇ°ºó×º
        encoded = encoded.TrimStart('<', '~').TrimEnd('~', '>');
        byte[] decoded = ascii85.Decode(encoded);
        CollectionAssert.AreEqual(original, decoded);
    }

    [TestMethod]
    public void Encode_EmptyArray_ReturnsPrefixAndSuffixOnly()
    {
        var ascii85 = new Ascii85();
        byte[] empty = Array.Empty<byte>();
        string encoded = ascii85.Encode(empty);
        Assert.IsTrue(encoded.Contains(ascii85.PrefixMark));
        Assert.IsTrue(encoded.Contains(ascii85.SuffixMark));
    }

    [TestMethod]
    public void Decode_EmptyString_ReturnsEmptyArray()
    {
        var ascii85 = new Ascii85();
        string encoded = ascii85.PrefixMark + ascii85.SuffixMark;
        byte[] decoded = ascii85.Decode(encoded);
        Assert.AreEqual(0, decoded.Length);
    }
}