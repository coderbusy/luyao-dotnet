using System;

namespace LuYao;

partial class Valid
{
    /// <inheritdoc/>
    public static Boolean ToBoolean(UInt16 value) => value != 0;
    /// <inheritdoc/>
    public static Char ToChar(UInt16 value) => Convert.ToChar(value);
    /// <inheritdoc/>
    public static SByte ToSByte(UInt16 value) => Convert.ToSByte(value);
    /// <inheritdoc/>
    public static Byte ToByte(UInt16 value) => Convert.ToByte(value);
    /// <inheritdoc/>
    public static Int16 ToInt16(UInt16 value) => Convert.ToInt16(value);
    /// <inheritdoc/>
    public static UInt16 ToUInt16(UInt16 value) => value;
    /// <inheritdoc/>
    public static Int32 ToInt32(UInt16 value) => Convert.ToInt32(value);
    /// <inheritdoc/>
    public static UInt32 ToUInt32(UInt16 value) => Convert.ToUInt32(value);
    /// <inheritdoc/>
    public static Int64 ToInt64(UInt16 value) => Convert.ToInt64(value);
    /// <inheritdoc/>
    public static UInt64 ToUInt64(UInt16 value) => Convert.ToUInt64(value);
    /// <inheritdoc/>
    public static Single ToSingle(UInt16 value) => Convert.ToSingle(value);
    /// <inheritdoc/>
    public static Double ToDouble(UInt16 value) => Convert.ToDouble(value);
    /// <inheritdoc/>
    public static Decimal ToDecimal(UInt16 value) => Convert.ToDecimal(value);
    /// <inheritdoc/>
    public static DateTime ToDateTime(UInt16 value) => Convert.ToDateTime(value);
    /// <inheritdoc/>
    public static String ToString(UInt16 value) => Convert.ToString(value);
}
