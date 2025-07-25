using System;

namespace LuYao;

partial class Valid
{
    /// <inheritdoc/>
    public static Boolean ToBoolean(Int32 value) => value != 0;
    /// <inheritdoc/>
    public static Char ToChar(Int32 value) => Convert.ToChar(value);
    /// <inheritdoc/>
    public static SByte ToSByte(Int32 value) => Convert.ToSByte(value);
    /// <inheritdoc/>
    public static Byte ToByte(Int32 value) => Convert.ToByte(value);
    /// <inheritdoc/>
    public static Int16 ToInt16(Int32 value) => Convert.ToInt16(value);
    /// <inheritdoc/>
    public static UInt16 ToUInt16(Int32 value) => Convert.ToUInt16(value);
    /// <inheritdoc/>
    public static Int32 ToInt32(Int32 value) => value;
    /// <inheritdoc/>
    public static UInt32 ToUInt32(Int32 value) => Convert.ToUInt32(value);
    /// <inheritdoc/>
    public static Int64 ToInt64(Int32 value) => Convert.ToInt64(value);
    /// <inheritdoc/>
    public static UInt64 ToUInt64(Int32 value) => Convert.ToUInt64(value);
    /// <inheritdoc/>
    public static Single ToSingle(Int32 value) => Convert.ToSingle(value);
    /// <inheritdoc/>
    public static Double ToDouble(Int32 value) => Convert.ToDouble(value);
    /// <inheritdoc/>
    public static Decimal ToDecimal(Int32 value) => Convert.ToDecimal(value);
    /// <inheritdoc/>
    public static DateTime ToDateTime(Int32 value) => Convert.ToDateTime(value);
    /// <inheritdoc/>
    public static String ToString(Int32 value) => Convert.ToString(value);
}
