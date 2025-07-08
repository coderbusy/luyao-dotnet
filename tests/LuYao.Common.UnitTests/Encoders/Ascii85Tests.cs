using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace LuYao.Encoders;


[TestClass]
public class Ascii85Tests
{
    [TestMethod]
    public void Encode_And_Decode_Should_Be_Reversible()
    {
        var ascii85 = new Ascii85();
        byte[] original = Encoding.UTF8.GetBytes("Hello, World!");
        string encoded = ascii85.Encode(original);
        byte[] decoded = ascii85.Decode(encoded);
        CollectionAssert.AreEqual(original, decoded);
    }

    [TestMethod]
    public void Static_ToAscii85String_And_FromAscii85String_Should_Be_Reversible()
    {
        byte[] original = Encoding.UTF8.GetBytes("Test1234");
        string encoded = Ascii85.ToAscii85String(original);
        byte[] decoded = Ascii85.FromAscii85String(encoded);
        CollectionAssert.AreEqual(original, decoded);
    }

    [TestMethod]
    public void Encode_Should_Include_Prefix_And_Suffix_When_EnforceMarks_True()
    {
        var ascii85 = new Ascii85 { EnforceMarks = true };
        byte[] data = Encoding.UTF8.GetBytes("abc");
        string encoded = ascii85.Encode(data);
        Assert.IsTrue(encoded.StartsWith(ascii85.PrefixMark));
        Assert.IsTrue(encoded.EndsWith(ascii85.SuffixMark));
    }

    [TestMethod]
    public void Encode_Should_Not_Include_Prefix_And_Suffix_When_EnforceMarks_False()
    {
        var ascii85 = new Ascii85 { EnforceMarks = false };
        byte[] data = Encoding.UTF8.GetBytes("abc");
        string encoded = ascii85.Encode(data);
        Assert.IsFalse(encoded.StartsWith(ascii85.PrefixMark));
        Assert.IsFalse(encoded.EndsWith(ascii85.SuffixMark));
    }

    [TestMethod]
    public void Decode_Should_Throw_When_Missing_Marks_And_EnforceMarks_True()
    {
        var ascii85 = new Ascii85 { EnforceMarks = true };
        string encoded = "87cURD_*#4DfTZ";
        Assert.ThrowsException<Exception>(() => ascii85.Decode(encoded));
    }

    [TestMethod]
    public void Decode_Should_Work_Without_Marks_When_EnforceMarks_False()
    {
        var ascii85 = new Ascii85 { EnforceMarks = false };
        byte[] original = Encoding.UTF8.GetBytes("data");
        string encoded = ascii85.Encode(original);
        // 去掉前后缀
        encoded = encoded.TrimStart('<', '~').TrimEnd('~', '>');
        byte[] decoded = ascii85.Decode(encoded);
        CollectionAssert.AreEqual(original, decoded);
    }

    [TestMethod]
    public void Encode_Should_Handle_Empty_Array()
    {
        var ascii85 = new Ascii85();
        byte[] empty = Array.Empty<byte>();
        string encoded = ascii85.Encode(empty);
        Assert.IsTrue(encoded.Contains(ascii85.PrefixMark));
        Assert.IsTrue(encoded.Contains(ascii85.SuffixMark));
    }

    [TestMethod]
    public void Decode_Should_Handle_Empty_String()
    {
        var ascii85 = new Ascii85();
        string encoded = ascii85.PrefixMark + ascii85.SuffixMark;
        byte[] decoded = ascii85.Decode(encoded);
        Assert.AreEqual(0, decoded.Length);
    }
}