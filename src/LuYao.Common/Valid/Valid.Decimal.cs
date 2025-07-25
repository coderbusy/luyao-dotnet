using System;

namespace LuYao;

partial class Valid
{
    /// <inheritdoc/>
    public static Boolean ToBoolean(Decimal value) => value != 0;
    /// <inheritdoc/>
    public static Char ToChar(Decimal value) => Convert.ToChar(value);
    /// <inheritdoc/>
    public static SByte ToSByte(Decimal value) => Convert.ToSByte(value);
    /// <inheritdoc/>
    public static Byte ToByte(Decimal value) => Convert.ToByte(value);
    /// <inheritdoc/>
    public static Int16 ToInt16(Decimal value) => Convert.ToInt16(value);
    /// <inheritdoc/>
    public static UInt16 ToUInt16(Decimal value) => Convert.ToUInt16(value);
    /// <inheritdoc/>
    public static Int32 ToInt32(Decimal value) => Convert.ToInt32(value);
    /// <inheritdoc/>
    public static UInt32 ToUInt32(Decimal value) => Convert.ToUInt32(value);
    /// <inheritdoc/>
    public static Int64 ToInt64(Decimal value) => Convert.ToInt64(value);
    /// <inheritdoc/>
    public static UInt64 ToUInt64(Decimal value) => Convert.ToUInt64(value);
    /// <inheritdoc/>
    public static Single ToSingle(Decimal value) => Convert.ToSingle(value);
    /// <inheritdoc/>
    public static Double ToDouble(Decimal value) => Convert.ToDouble(value);
    /// <inheritdoc/>
    public static Decimal ToDecimal(Decimal value) => value;
    /// <inheritdoc/>
    public static DateTime ToDateTime(Decimal value) => Convert.ToDateTime(value);
    /// <inheritdoc/>
    public static String ToString(Decimal value) => Convert.ToString(value);
}
