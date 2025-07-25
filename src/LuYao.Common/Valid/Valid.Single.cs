using System;

namespace LuYao;

partial class Valid
{
    /// <inheritdoc/>
    public static Boolean ToBoolean(Single value) => value != 0;
    /// <inheritdoc/>
    public static Char ToChar(Single value) => Convert.ToChar(value);
    /// <inheritdoc/>
    public static SByte ToSByte(Single value) => Convert.ToSByte(value);
    /// <inheritdoc/>
    public static Byte ToByte(Single value) => Convert.ToByte(value);
    /// <inheritdoc/>
    public static Int16 ToInt16(Single value) => Convert.ToInt16(value);
    /// <inheritdoc/>
    public static UInt16 ToUInt16(Single value) => Convert.ToUInt16(value);
    /// <inheritdoc/>
    public static Int32 ToInt32(Single value) => Convert.ToInt32(value);
    /// <inheritdoc/>
    public static UInt32 ToUInt32(Single value) => Convert.ToUInt32(value);
    /// <inheritdoc/>
    public static Int64 ToInt64(Single value) => Convert.ToInt64(value);
    /// <inheritdoc/>
    public static UInt64 ToUInt64(Single value) => Convert.ToUInt64(value);
    /// <inheritdoc/>
    public static Single ToSingle(Single value) => value;
    /// <inheritdoc/>
    public static Double ToDouble(Single value) => Convert.ToDouble(value);
    /// <inheritdoc/>
    public static Decimal ToDecimal(Single value) => Convert.ToDecimal(value);
    /// <inheritdoc/>
    public static DateTime ToDateTime(Single value) => Convert.ToDateTime(value);
    /// <inheritdoc/>
    public static String ToString(Single value) => Convert.ToString(value);
}
