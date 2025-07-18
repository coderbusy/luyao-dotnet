using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao;


[TestClass]
public class ValidBooleanTests
{
    [TestMethod]
    public void ToBoolean_StringFalseValues_ReturnsFalse()
    {
        Assert.IsFalse(Valid.ToBoolean("0"));
        Assert.IsFalse(Valid.ToBoolean("false"));
        Assert.IsFalse(Valid.ToBoolean("no"));
        Assert.IsFalse(Valid.ToBoolean("off"));
        Assert.IsFalse(Valid.ToBoolean("n"));
        Assert.IsFalse(Valid.ToBoolean("f"));
        Assert.IsFalse(Valid.ToBoolean("null"));
        Assert.IsFalse(Valid.ToBoolean(" FALSE "));
        Assert.IsFalse(Valid.ToBoolean((string?)null));
        Assert.IsFalse(Valid.ToBoolean(""));
        Assert.IsFalse(Valid.ToBoolean("   "));
    }

    [TestMethod]
    public void ToBoolean_StringTrueValues_ReturnsTrue()
    {
        Assert.IsTrue(Valid.ToBoolean("1"));
        Assert.IsTrue(Valid.ToBoolean("true"));
        Assert.IsTrue(Valid.ToBoolean("yes"));
        Assert.IsTrue(Valid.ToBoolean("on"));
        Assert.IsTrue(Valid.ToBoolean("y"));
        Assert.IsTrue(Valid.ToBoolean("t"));
        Assert.IsTrue(Valid.ToBoolean("abc"));
        Assert.IsTrue(Valid.ToBoolean("True"));
    }

    [TestMethod]
    public void ToBoolean_ObjectNullOrDBNull_ReturnsFalse()
    {
        Assert.IsFalse(Valid.ToBoolean((object?)null));
        Assert.IsFalse(Valid.ToBoolean(System.DBNull.Value));
    }

    [TestMethod]
    public void ToBoolean_ObjectBool_ReturnsBoolValue()
    {
        Assert.IsTrue(Valid.ToBoolean(true));
        Assert.IsFalse(Valid.ToBoolean(false));
    }

    [TestMethod]
    public void ToBoolean_ObjectString_ReturnsExpected()
    {
        Assert.IsFalse(Valid.ToBoolean((object)"false"));
        Assert.IsTrue(Valid.ToBoolean((object)"yes"));
    }

    [TestMethod]
    public void ToBoolean_CharValues_ReturnsExpected()
    {
        Assert.IsFalse(Valid.ToBoolean((char)0));
        Assert.IsTrue(Valid.ToBoolean((char)1));
        Assert.IsTrue(Valid.ToBoolean('A'));
    }

    [TestMethod]
    public void ToBoolean_NullableCharValues_ReturnsExpected()
    {
        Assert.IsFalse(Valid.ToBoolean((char?)null));
        Assert.IsFalse(Valid.ToBoolean((char?)0));
        Assert.IsTrue(Valid.ToBoolean((char?)'B'));
    }

    [TestMethod]
    public void ToBoolean_NullableBoolValues_ReturnsExpected()
    {
        Assert.IsFalse(Valid.ToBoolean((bool?)null));
        Assert.IsTrue(Valid.ToBoolean((bool?)true));
        Assert.IsFalse(Valid.ToBoolean((bool?)false));
    }

    [TestMethod]
    public void ToBoolean_ByteValues_ReturnsExpected()
    {
        Assert.IsFalse(Valid.ToBoolean((byte)0));
        Assert.IsTrue(Valid.ToBoolean((byte)1));
    }

    [TestMethod]
    public void ToBoolean_NullableByteValues_ReturnsExpected()
    {
        Assert.IsFalse(Valid.ToBoolean((byte?)null));
        Assert.IsFalse(Valid.ToBoolean((byte?)0));
        Assert.IsTrue(Valid.ToBoolean((byte?)2));
    }

    [TestMethod]
    public void ToBoolean_SByteValues_ReturnsExpected()
    {
        Assert.IsFalse(Valid.ToBoolean((sbyte)0));
        Assert.IsTrue(Valid.ToBoolean((sbyte)1));
        Assert.IsFalse(Valid.ToBoolean((sbyte)-1));
    }

    [TestMethod]
    public void ToBoolean_NullableSByteValues_ReturnsExpected()
    {
        Assert.IsFalse(Valid.ToBoolean((sbyte?)null));
        Assert.IsFalse(Valid.ToBoolean((sbyte?)0));
        Assert.IsTrue(Valid.ToBoolean((sbyte?)3));
    }

    [TestMethod]
    public void ToBoolean_ShortValues_ReturnsExpected()
    {
        Assert.IsFalse(Valid.ToBoolean((short)0));
        Assert.IsTrue(Valid.ToBoolean((short)1));
        Assert.IsFalse(Valid.ToBoolean((short)-1));
    }

    [TestMethod]
    public void ToBoolean_NullableShortValues_ReturnsExpected()
    {
        Assert.IsFalse(Valid.ToBoolean((short?)null));
        Assert.IsFalse(Valid.ToBoolean((short?)0));
        Assert.IsTrue(Valid.ToBoolean((short?)4));
    }

    [TestMethod]
    public void ToBoolean_UShortValues_ReturnsExpected()
    {
        Assert.IsFalse(Valid.ToBoolean((ushort)0));
        Assert.IsTrue(Valid.ToBoolean((ushort)1));
    }

    [TestMethod]
    public void ToBoolean_NullableUShortValues_ReturnsExpected()
    {
        Assert.IsFalse(Valid.ToBoolean((ushort?)null));
        Assert.IsFalse(Valid.ToBoolean((ushort?)0));
        Assert.IsTrue(Valid.ToBoolean((ushort?)5));
    }

    [TestMethod]
    public void ToBoolean_IntValues_ReturnsExpected()
    {
        Assert.IsFalse(Valid.ToBoolean(0));
        Assert.IsTrue(Valid.ToBoolean(1));
        Assert.IsFalse(Valid.ToBoolean(-1));
    }

    [TestMethod]
    public void ToBoolean_NullableIntValues_ReturnsExpected()
    {
        Assert.IsFalse(Valid.ToBoolean((int?)null));
        Assert.IsFalse(Valid.ToBoolean((int?)0));
        Assert.IsTrue(Valid.ToBoolean((int?)6));
    }

    [TestMethod]
    public void ToBoolean_UIntValues_ReturnsExpected()
    {
        Assert.IsFalse(Valid.ToBoolean((uint)0));
        Assert.IsTrue(Valid.ToBoolean((uint)1));
    }

    [TestMethod]
    public void ToBoolean_NullableUIntValues_ReturnsExpected()
    {
        Assert.IsFalse(Valid.ToBoolean((uint?)null));
        Assert.IsFalse(Valid.ToBoolean((uint?)0));
        Assert.IsTrue(Valid.ToBoolean((uint?)7));
    }

    [TestMethod]
    public void ToBoolean_LongValues_ReturnsExpected()
    {
        Assert.IsFalse(Valid.ToBoolean((long)0));
        Assert.IsTrue(Valid.ToBoolean((long)1));
        Assert.IsFalse(Valid.ToBoolean((long)-1));
    }

    [TestMethod]
    public void ToBoolean_NullableLongValues_ReturnsExpected()
    {
        Assert.IsFalse(Valid.ToBoolean((long?)null));
        Assert.IsFalse(Valid.ToBoolean((long?)0));
        Assert.IsTrue(Valid.ToBoolean((long?)8));
    }

    [TestMethod]
    public void ToBoolean_ULongValues_ReturnsExpected()
    {
        Assert.IsFalse(Valid.ToBoolean((ulong)0));
        Assert.IsTrue(Valid.ToBoolean((ulong)1));
    }

    [TestMethod]
    public void ToBoolean_NullableULongValues_ReturnsExpected()
    {
        Assert.IsFalse(Valid.ToBoolean((ulong?)null));
        Assert.IsFalse(Valid.ToBoolean((ulong?)0));
        Assert.IsTrue(Valid.ToBoolean((ulong?)9));
    }

    [TestMethod]
    public void ToBoolean_FloatValues_ReturnsExpected()
    {
        Assert.IsFalse(Valid.ToBoolean(0f));
        Assert.IsTrue(Valid.ToBoolean(1.1f));
        Assert.IsFalse(Valid.ToBoolean(-1.1f));
    }

    [TestMethod]
    public void ToBoolean_NullableFloatValues_ReturnsExpected()
    {
        Assert.IsFalse(Valid.ToBoolean((float?)null));
        Assert.IsFalse(Valid.ToBoolean((float?)0f));
        Assert.IsTrue(Valid.ToBoolean((float?)10.1f));
    }

    [TestMethod]
    public void ToBoolean_DoubleValues_ReturnsExpected()
    {
        Assert.IsFalse(Valid.ToBoolean(0d));
        Assert.IsTrue(Valid.ToBoolean(1.2d));
        Assert.IsFalse(Valid.ToBoolean(-1.2d));
    }

    [TestMethod]
    public void ToBoolean_NullableDoubleValues_ReturnsExpected()
    {
        Assert.IsFalse(Valid.ToBoolean((double?)null));
        Assert.IsFalse(Valid.ToBoolean((double?)0d));
        Assert.IsTrue(Valid.ToBoolean((double?)11.2d));
    }

    [TestMethod]
    public void ToBoolean_DecimalValues_ReturnsExpected()
    {
        Assert.IsFalse(Valid.ToBoolean(0m));
        Assert.IsTrue(Valid.ToBoolean(1.3m));
        Assert.IsFalse(Valid.ToBoolean(-1.3m));
    }

    [TestMethod]
    public void ToBoolean_NullableDecimalValues_ReturnsExpected()
    {
        Assert.IsFalse(Valid.ToBoolean((decimal?)null));
        Assert.IsFalse(Valid.ToBoolean((decimal?)0m));
        Assert.IsTrue(Valid.ToBoolean((decimal?)12.3m));
    }
}