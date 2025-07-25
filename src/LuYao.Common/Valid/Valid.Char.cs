using System;

namespace LuYao;

partial class Valid
{
    /// <inheritdoc/>
    public static Boolean ToBoolean(Char value) => default;

    /// <inheritdoc/>
    public static Char ToChar(Char value) => value;

    /// <inheritdoc/>
    public static SByte ToSByte(Char value) => value <= SByte.MaxValue ? (SByte)value : default;

    /// <inheritdoc/>
    public static Byte ToByte(Char value) => value <= Byte.MaxValue ? (Byte)value : default;

    /// <inheritdoc/>
    public static Int16 ToInt16(Char value) => value <= Int16.MaxValue ? (Int16)value : default;

    /// <inheritdoc/>
    public static UInt16 ToUInt16(Char value) => (UInt16)value;

    /// <inheritdoc/>
    public static Int32 ToInt32(Char value) => (Int32)value;

    /// <inheritdoc/>
    public static UInt32 ToUInt32(Char value) => (UInt32)value;

    /// <inheritdoc/>
    public static Int64 ToInt64(Char value) => (Int64)value;

    /// <inheritdoc/>
    public static UInt64 ToUInt64(Char value) => (UInt64)value;

    /// <inheritdoc/>
    public static Single ToSingle(Char value) => (Single)value;

    /// <inheritdoc/>
    public static Double ToDouble(Char value) => (Double)value;

    /// <inheritdoc/>
    public static Decimal ToDecimal(Char value) => (Decimal)value;

    /// <inheritdoc/>
    public static DateTime ToDateTime(Char value) => default;

    /// <inheritdoc/>
    public static String ToString(Char value) => value.ToString();
}
