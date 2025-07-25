using System;

namespace LuYao;

partial class Valid
{
    /// <inheritdoc/>
    public static Boolean ToBoolean(Int64 value) => value != 0;
    /// <inheritdoc/>
    public static Char ToChar(Int64 value) => Convert.ToChar(value);
    /// <inheritdoc/>
    public static SByte ToSByte(Int64 value) => Convert.ToSByte(value);
    /// <inheritdoc/>
    public static Byte ToByte(Int64 value) => Convert.ToByte(value);
    /// <inheritdoc/>
    public static Int16 ToInt16(Int64 value) => Convert.ToInt16(value);
    /// <inheritdoc/>
    public static UInt16 ToUInt16(Int64 value) => Convert.ToUInt16(value);
    /// <inheritdoc/>
    public static Int32 ToInt32(Int64 value) => Convert.ToInt32(value);
    /// <inheritdoc/>
    public static UInt32 ToUInt32(Int64 value) => Convert.ToUInt32(value);
    /// <inheritdoc/>
    public static Int64 ToInt64(Int64 value) => value;
    /// <inheritdoc/>
    public static UInt64 ToUInt64(Int64 value) => Convert.ToUInt64(value);
    /// <inheritdoc/>
    public static Single ToSingle(Int64 value) => Convert.ToSingle(value);
    /// <inheritdoc/>
    public static Double ToDouble(Int64 value) => Convert.ToDouble(value);
    /// <inheritdoc/>
    public static Decimal ToDecimal(Int64 value) => Convert.ToDecimal(value);
    /// <inheritdoc/>
    public static DateTime ToDateTime(Int64 value) => Convert.ToDateTime(value);
    /// <inheritdoc/>
    public static String ToString(Int64 value) => Convert.ToString(value);
}
