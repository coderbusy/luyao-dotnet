using System;

namespace LuYao;

partial class Valid
{
    /// <inheritdoc/>
    public static Byte ToByte(Boolean value) => value ? (byte)1 : default;

    /// <inheritdoc/>
    public static Byte ToByte(Char value) => value <= (char)byte.MaxValue ? (byte)value : default;

    /// <inheritdoc/>
    public static Byte ToByte(SByte value) => value >= 0 ? (byte)value : default;

    /// <inheritdoc/>
    public static Byte ToByte(Byte value) => value;

    /// <inheritdoc/>
    public static Byte ToByte(Int16 value) => value >= byte.MinValue && value <= byte.MaxValue ? (byte)value : default;

    /// <inheritdoc/>
    public static Byte ToByte(UInt16 value) => value <= byte.MaxValue ? (byte)value : default;

    /// <inheritdoc/>
    public static Byte ToByte(Int32 value) => value >= byte.MinValue && value <= byte.MaxValue ? (byte)value : default;

    /// <inheritdoc/>
    public static Byte ToByte(UInt32 value) => value <= byte.MaxValue ? (byte)value : default;

    /// <inheritdoc/>
    public static Byte ToByte(Int64 value) => value >= byte.MinValue && value <= byte.MaxValue ? (byte)value : default;

    /// <inheritdoc/>
    public static Byte ToByte(UInt64 value) => value <= byte.MaxValue ? (byte)value : default;

    /// <inheritdoc/>
    public static Byte ToByte(Single value)
    {
        if (float.IsNaN(value) || float.IsInfinity(value)) return 0;
        if (value < byte.MinValue || value > byte.MaxValue) return 0;
        return (byte)value;
    }

    /// <inheritdoc/>
    public static Byte ToByte(Double value)
    {
        if (double.IsNaN(value) || double.IsInfinity(value)) return 0;
        if (value < byte.MinValue || value > byte.MaxValue) return 0;
        return (byte)value;
    }

    /// <inheritdoc/>
    public static Byte ToByte(Decimal value) => value >= byte.MinValue && value <= byte.MaxValue ? (byte)value : default;

    /// <inheritdoc/>
    public static Byte ToByte(DateTime value) => 0;
}
