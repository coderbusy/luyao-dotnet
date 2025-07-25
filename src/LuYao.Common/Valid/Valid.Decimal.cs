using System;

namespace LuYao;

partial class Valid
{
    /// <inheritdoc/>
    public static Boolean ToBoolean(Decimal value) => value != 0;

    /// <inheritdoc/>
    public static Char ToChar(Decimal value)
    {
        // 使用 Math.Truncate 丢弃小数部分，然后检查范围：0 到 65535
        var truncated = Math.Truncate(value);
        if (truncated < 0 || truncated > Char.MaxValue) return default;
        return (Char)truncated;
    }

    /// <inheritdoc/>
    public static SByte ToSByte(Decimal value)
    {
        // 使用 Math.Truncate 丢弃小数部分，然后检查范围：-128 到 127
        var truncated = Math.Truncate(value);
        if (truncated < SByte.MinValue || truncated > SByte.MaxValue) return default;
        return (SByte)truncated;
    }

    /// <inheritdoc/>
    public static Byte ToByte(Decimal value)
    {
        // 使用 Math.Truncate 丢弃小数部分，然后检查范围：0 到 255
        var truncated = Math.Truncate(value);
        if (truncated < Byte.MinValue || truncated > Byte.MaxValue) return default;
        return (Byte)truncated;
    }

    /// <inheritdoc/>
    public static Int16 ToInt16(Decimal value)
    {
        // 使用 Math.Truncate 丢弃小数部分，然后检查范围：-32768 到 32767
        var truncated = Math.Truncate(value);
        if (truncated < Int16.MinValue || truncated > Int16.MaxValue) return default;
        return (Int16)truncated;
    }

    /// <inheritdoc/>
    public static UInt16 ToUInt16(Decimal value)
    {
        // 使用 Math.Truncate 丢弃小数部分，然后检查范围：0 到 65535
        var truncated = Math.Truncate(value);
        if (truncated < UInt16.MinValue || truncated > UInt16.MaxValue) return default;
        return (UInt16)truncated;
    }

    /// <inheritdoc/>
    public static Int32 ToInt32(Decimal value)
    {
        // 使用 Math.Truncate 丢弃小数部分，然后检查范围：-2147483648 到 2147483647
        var truncated = Math.Truncate(value);
        if (truncated < Int32.MinValue || truncated > Int32.MaxValue) return default;
        return (Int32)truncated;
    }

    /// <inheritdoc/>
    public static UInt32 ToUInt32(Decimal value)
    {
        // 使用 Math.Truncate 丢弃小数部分，然后检查范围：0 到 4294967295
        var truncated = Math.Truncate(value);
        if (truncated < UInt32.MinValue || truncated > UInt32.MaxValue) return default;
        return (UInt32)truncated;
    }

    /// <inheritdoc/>
    public static Int64 ToInt64(Decimal value)
    {
        // 使用 Math.Truncate 丢弃小数部分，然后检查范围：-9223372036854775808 到 9223372036854775807
        var truncated = Math.Truncate(value);
        if (truncated < Int64.MinValue || truncated > Int64.MaxValue) return default;
        return (Int64)truncated;
    }

    /// <inheritdoc/>
    public static UInt64 ToUInt64(Decimal value)
    {
        // 使用 Math.Truncate 丢弃小数部分，然后检查范围：0 到 18446744073709551615
        var truncated = Math.Truncate(value);
        if (truncated < UInt64.MinValue || truncated > UInt64.MaxValue) return default;
        return (UInt64)truncated;
    }

    /// <inheritdoc/>
    public static Single ToSingle(Decimal value)
    {
        // Decimal 到 Single 的转换可能损失精度但不会溢出，使用直接转换
        return (Single)value;
    }

    /// <inheritdoc/>
    public static Double ToDouble(Decimal value)
    {
        // Decimal 到 Double 的转换可能损失精度但不会溢出，使用直接转换
        return (Double)value;
    }

    /// <inheritdoc/>
    public static Decimal ToDecimal(Decimal value) => value;

    /// <inheritdoc/>
    public static DateTime ToDateTime(Decimal value) => default;

    /// <inheritdoc/>
    public static String ToString(Decimal value) => value.ToString();
}
