using System;

namespace LuYao;

partial class Valid
{
    /// <inheritdoc/>
    public static Boolean ToBoolean(UInt64 value) => value != 0;
    /// <inheritdoc/>
    public static Char ToChar(UInt64 value) => Convert.ToChar(value);
    /// <inheritdoc/>
    public static SByte ToSByte(UInt64 value) => Convert.ToSByte(value);
    /// <inheritdoc/>
    public static Byte ToByte(UInt64 value) => Convert.ToByte(value);
    /// <inheritdoc/>
    public static Int16 ToInt16(UInt64 value) => Convert.ToInt16(value);
    /// <inheritdoc/>
    public static UInt16 ToUInt16(UInt64 value) => Convert.ToUInt16(value);
    /// <inheritdoc/>
    public static Int32 ToInt32(UInt64 value) => Convert.ToInt32(value);
    /// <inheritdoc/>
    public static UInt32 ToUInt32(UInt64 value) => Convert.ToUInt32(value);
    /// <inheritdoc/>
    public static Int64 ToInt64(UInt64 value) => Convert.ToInt64(value);
    /// <inheritdoc/>
    public static UInt64 ToUInt64(UInt64 value) => value;
    /// <inheritdoc/>
    public static Single ToSingle(UInt64 value) => Convert.ToSingle(value);
    /// <inheritdoc/>
    public static Double ToDouble(UInt64 value) => Convert.ToDouble(value);
    /// <inheritdoc/>
    public static Decimal ToDecimal(UInt64 value) => Convert.ToDecimal(value);
    /// <inheritdoc/>
    public static DateTime ToDateTime(UInt64 value) => Convert.ToDateTime(value);
    /// <inheritdoc/>
    public static String ToString(UInt64 value) => Convert.ToString(value);
}
