using System;

namespace LuYao;

partial class Valid
{
    /// <inheritdoc/>
    public static Boolean ToBoolean(SByte value) => value != 0;
    /// <inheritdoc/>
    public static Char ToChar(SByte value) => Convert.ToChar(value);
    /// <inheritdoc/>
    public static SByte ToSByte(SByte value) => value;
    /// <inheritdoc/>
    public static Byte ToByte(SByte value) => Convert.ToByte(value);
    /// <inheritdoc/>
    public static Int16 ToInt16(SByte value) => Convert.ToInt16(value);
    /// <inheritdoc/>
    public static UInt16 ToUInt16(SByte value) => Convert.ToUInt16(value);
    /// <inheritdoc/>
    public static Int32 ToInt32(SByte value) => Convert.ToInt32(value);
    /// <inheritdoc/>
    public static UInt32 ToUInt32(SByte value) => Convert.ToUInt32(value);
    /// <inheritdoc/>
    public static Int64 ToInt64(SByte value) => Convert.ToInt64(value);
    /// <inheritdoc/>
    public static UInt64 ToUInt64(SByte value) => Convert.ToUInt64(value);
    /// <inheritdoc/>
    public static Single ToSingle(SByte value) => Convert.ToSingle(value);
    /// <inheritdoc/>
    public static Double ToDouble(SByte value) => Convert.ToDouble(value);
    /// <inheritdoc/>
    public static Decimal ToDecimal(SByte value) => Convert.ToDecimal(value);
    /// <inheritdoc/>
    public static DateTime ToDateTime(SByte value) => Convert.ToDateTime(value);
    /// <inheritdoc/>
    public static String ToString(SByte value) => Convert.ToString(value);
}
