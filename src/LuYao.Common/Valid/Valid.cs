using System;

namespace LuYao;

/// <summary>
/// 数据类型转换
/// </summary>
public static partial class Valid
{
    /// <inheritdoc/>
    public static Char ToChar(object value) => Convert.ToChar(value);

    /// <inheritdoc/>
    public static Decimal ToDecimal(object value) => Convert.ToDecimal(value);

    /// <inheritdoc/>
    public static Double ToDouble(object value) => Convert.ToDouble(value);

    /// <inheritdoc/>
    public static Int16 ToInt16(object value) => Convert.ToInt16(value);

    /// <inheritdoc/>
    public static SByte ToSByte(object value) => Convert.ToSByte(value);

    /// <inheritdoc/>
    public static Single ToSingle(object value) => Convert.ToSingle(value);

    /// <inheritdoc/>
    public static UInt16 ToUInt16(object value) => Convert.ToUInt16(value);

    /// <inheritdoc/>
    public static UInt32 ToUInt32(object value) => Convert.ToUInt32(value);

    /// <inheritdoc/>
    public static UInt64 ToUInt64(object value) => Convert.ToUInt64(value);
}
