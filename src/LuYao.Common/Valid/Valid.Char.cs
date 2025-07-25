using System;

namespace LuYao;

partial class Valid
{
    /// <inheritdoc/>
    public static Char ToChar(Boolean value) => default;

    /// <inheritdoc/>
    public static Char ToChar(Char value) => value;

    /// <inheritdoc/>
    public static Char ToChar(SByte value)
    {
        // System.Convert: Only 0~65535 valid
        return value >= 0 ? (char)value : (char)0;
    }

    /// <inheritdoc/>
    public static Char ToChar(Byte value)
    {
        // System.Convert: 0~255 always valid
        return (char)value;
    }

    /// <inheritdoc/>
    public static Char ToChar(Int16 value)
    {
        // System.Convert: Only 0~65535 valid
        return value >= 0 ? (char)value : (char)0;
    }

    /// <inheritdoc/>
    public static Char ToChar(UInt16 value)
    {
        // System.Convert: 0~65535 always valid
        return (char)value;
    }

    /// <inheritdoc/>
    public static Char ToChar(Int32 value)
    {
        // System.Convert: Only 0~65535 valid
        return value >= Char.MinValue && value <= Char.MaxValue ? (char)value : (char)0;
    }

    /// <inheritdoc/>
    public static Char ToChar(UInt32 value)
    {
        // System.Convert: Only 0~65535 valid
        return value <= Char.MaxValue ? (char)value : (char)0;
    }

    /// <inheritdoc/>
    public static Char ToChar(Int64 value)
    {
        // System.Convert: Only 0~65535 valid
        return value >= Char.MinValue && value <= Char.MaxValue ? (char)value : (char)0;
    }

    /// <inheritdoc/>
    public static Char ToChar(UInt64 value)
    {
        // System.Convert: Only 0~65535 valid
        return value <= Char.MaxValue ? (char)value : (char)0;
    }

    /// <inheritdoc/>
    public static Char ToChar(Single value)
    {
        // System.Convert: Only 0~65535, not NaN/Infinity, must be whole number
        if (Single.IsNaN(value) || Single.IsInfinity(value))
            return (char)0;
        if (value < Char.MinValue || value > Char.MaxValue)
            return (char)0;
        var intValue = (int)value;
        if (intValue != value)
            return (char)0;
        return (char)intValue;
    }

    /// <inheritdoc/>
    public static Char ToChar(Double value)
    {
        // System.Convert: Only 0~65535, not NaN/Infinity, must be whole number
        if (Double.IsNaN(value) || Double.IsInfinity(value))
            return (char)0;
        if (value < Char.MinValue || value > Char.MaxValue)
            return (char)0;
        var intValue = (int)value;
        if (intValue != value)
            return (char)0;
        return (char)intValue;
    }

    /// <inheritdoc/>
    public static Char ToChar(Decimal value)
    {
        // System.Convert: Only 0~65535, must be whole number
        if (value < Char.MinValue || value > Char.MaxValue)
            return (char)0;
        if (Decimal.Truncate(value) != value)
            return (char)0;
        return (char)value;
    }

    /// <inheritdoc/>
    public static Char ToChar(DateTime value)
    {
        // System.Convert: Not supported, always throws
        return (char)0;
    }
}
