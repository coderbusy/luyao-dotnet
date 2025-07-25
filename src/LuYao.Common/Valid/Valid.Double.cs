using System;

namespace LuYao;

partial class Valid
{
    /// <inheritdoc/>
    public static Boolean ToBoolean(Double value) => value != 0 && !Double.IsNaN(value);

    /// <inheritdoc/>
    public static Char ToChar(Double value)
    {
        // 检查特殊值
        if (Double.IsNaN(value) || Double.IsInfinity(value)) return default;

        // 丢弃小数部分，然后检查范围：0 到 65535
        var truncated = Math.Truncate(value);
        if (truncated < 0 || truncated > Char.MaxValue) return default;
        return (Char)truncated;
    }

    /// <inheritdoc/>
    public static SByte ToSByte(Double value)
    {
        // 检查特殊值
        if (Double.IsNaN(value) || Double.IsInfinity(value)) return default;

        // 丢弃小数部分，然后检查范围：-128 到 127
        var truncated = Math.Truncate(value);
        if (truncated < SByte.MinValue || truncated > SByte.MaxValue) return default;
        return (SByte)truncated;
    }

    /// <inheritdoc/>
    public static Byte ToByte(Double value)
    {
        // 检查特殊值
        if (Double.IsNaN(value) || Double.IsInfinity(value)) return default;

        // 丢弃小数部分，然后检查范围：0 到 255
        var truncated = Math.Truncate(value);
        if (truncated < Byte.MinValue || truncated > Byte.MaxValue) return default;
        return (Byte)truncated;
    }

    /// <inheritdoc/>
    public static Int16 ToInt16(Double value)
    {
        // 检查特殊值
        if (Double.IsNaN(value) || Double.IsInfinity(value)) return default;

        // 丢弃小数部分，然后检查范围：-32768 到 32767
        var truncated = Math.Truncate(value);
        if (truncated < Int16.MinValue || truncated > Int16.MaxValue) return default;
        return (Int16)truncated;
    }

    /// <inheritdoc/>
    public static UInt16 ToUInt16(Double value)
    {
        // 检查特殊值
        if (Double.IsNaN(value) || Double.IsInfinity(value)) return default;

        // 丢弃小数部分，然后检查范围：0 到 65535
        var truncated = Math.Truncate(value);
        if (truncated < UInt16.MinValue || truncated > UInt16.MaxValue) return default;
        return (UInt16)truncated;
    }

    /// <inheritdoc/>
    public static Int32 ToInt32(Double value)
    {
        // 检查特殊值
        if (Double.IsNaN(value) || Double.IsInfinity(value)) return default;

        // 丢弃小数部分，然后检查范围：-2147483648 到 2147483647
        var truncated = Math.Truncate(value);
        if (truncated < Int32.MinValue || truncated > Int32.MaxValue) return default;
        return (Int32)truncated;
    }

    /// <inheritdoc/>
    public static UInt32 ToUInt32(Double value)
    {
        // 检查特殊值
        if (Double.IsNaN(value) || Double.IsInfinity(value)) return default;

        // 丢弃小数部分，然后检查范围：0 到 4294967295
        var truncated = Math.Truncate(value);
        if (truncated < UInt32.MinValue || truncated > UInt32.MaxValue) return default;
        return (UInt32)truncated;
    }

    /// <inheritdoc/>
    public static Int64 ToInt64(Double value)
    {
        // 检查特殊值
        if (Double.IsNaN(value) || Double.IsInfinity(value)) return default;

        // 丢弃小数部分，然后检查范围：-9223372036854775808 到 9223372036854775807
        var truncated = Math.Truncate(value);
        if (truncated < Int64.MinValue || truncated > Int64.MaxValue) return default;
        return (Int64)truncated;
    }

    /// <inheritdoc/>
    public static UInt64 ToUInt64(Double value)
    {
        // 检查特殊值
        if (Double.IsNaN(value) || Double.IsInfinity(value)) return default;

        // 丢弃小数部分，然后检查范围：0 到 18446744073709551615
        var truncated = Math.Truncate(value);
        if (truncated < 0 || truncated > UInt64.MaxValue) return default;
        return (UInt64)truncated;
    }

    /// <inheritdoc/>
    public static Single ToSingle(Double value)
    {
        // 检查特殊值
        if (Double.IsNaN(value)) return Single.NaN;
        if (Double.IsPositiveInfinity(value)) return Single.PositiveInfinity;
        if (Double.IsNegativeInfinity(value)) return Single.NegativeInfinity;

        // 检查范围：Single.MinValue 到 Single.MaxValue
        if (value < Single.MinValue || value > Single.MaxValue) return default;
        return (Single)value;
    }

    /// <inheritdoc/>
    public static Double ToDouble(Double value) => value;

    /// <inheritdoc/>
    public static Decimal ToDecimal(Double value)
    {
        // 检查特殊值
        if (Double.IsNaN(value) || Double.IsInfinity(value)) return default;

        // Decimal 的范围比 Double 小，需要检查范围
        // Decimal 最大值约为 79,228,162,514,264,337,593,543,950,335
        if (value < (double)Decimal.MinValue || value > (double)Decimal.MaxValue) return default;
        return (Decimal)value;
    }

    /// <inheritdoc/>
    public static DateTime ToDateTime(Double value)
    {
        // Double 到 DateTime 的转换在标准库中会抛出 InvalidCastException
        // 我们直接返回默认值
        return default;
    }

    /// <inheritdoc/>
    public static String ToString(Double value)
    {
        // Double.ToString() 永远不会抛出异常，即使是特殊值也会返回相应的字符串
        return value.ToString();
    }
}
