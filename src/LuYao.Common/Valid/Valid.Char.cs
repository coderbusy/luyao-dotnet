using System;

namespace LuYao;

partial class Valid
{
    /// <inheritdoc/>
    public static Boolean ToBoolean(Char value) => false;
    /// <inheritdoc/>
    public static Char ToChar(Char value) => value;
    /// <inheritdoc/>
    public static SByte ToSByte(Char value) => Convert.ToSByte(value);
    /// <inheritdoc/>
    public static Byte ToByte(Char value) => Convert.ToByte(value);
    /// <inheritdoc/>
    public static Int16 ToInt16(Char value) => Convert.ToInt16(value);
    /// <inheritdoc/>
    public static UInt16 ToUInt16(Char value) => Convert.ToUInt16(value);
    /// <inheritdoc/>
    public static Int32 ToInt32(Char value) => Convert.ToInt32(value);
    /// <inheritdoc/>
    public static UInt32 ToUInt32(Char value) => Convert.ToUInt32(value);
    /// <inheritdoc/>
    public static Int64 ToInt64(Char value) => Convert.ToInt64(value);
    /// <inheritdoc/>
    public static UInt64 ToUInt64(Char value) => Convert.ToUInt64(value);
    /// <inheritdoc/>
    public static Single ToSingle(Char value) => Convert.ToSingle(value);
    /// <inheritdoc/>
    public static Double ToDouble(Char value) => Convert.ToDouble(value);
    /// <inheritdoc/>
    public static Decimal ToDecimal(Char value) => Convert.ToDecimal(value);
    /// <inheritdoc/>
    public static DateTime ToDateTime(Char value) => Convert.ToDateTime(value);
    /// <inheritdoc/>
    public static String ToString(Char value) => Convert.ToString(value);
}
