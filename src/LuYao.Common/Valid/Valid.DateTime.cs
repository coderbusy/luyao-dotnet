using System;

namespace LuYao;

partial class Valid
{
    /// <inheritdoc/>
    public static DateTime ToDateTime(long value) => DateTime.FromBinary(value);

    /// <inheritdoc/>
    public static DateTime ToDateTime(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return default;
        if (DateTime.TryParse(value, out var dt)) return dt;
        return default;
    }

    /// <inheritdoc/>
    public static DateTime ToDateTime(Boolean value) => Convert.ToDateTime(value);
    /// <inheritdoc/>
    public static DateTime ToDateTime(Char value) => Convert.ToDateTime(value);
    /// <inheritdoc/>
    public static DateTime ToDateTime(SByte value) => Convert.ToDateTime(value);
    /// <inheritdoc/>
    public static DateTime ToDateTime(Byte value) => Convert.ToDateTime(value);
    /// <inheritdoc/>
    public static DateTime ToDateTime(Int16 value) => Convert.ToDateTime(value);
    /// <inheritdoc/>
    public static DateTime ToDateTime(UInt16 value) => Convert.ToDateTime(value);
    /// <inheritdoc/>
    public static DateTime ToDateTime(Int32 value) => Convert.ToDateTime(value);
    /// <inheritdoc/>
    public static DateTime ToDateTime(UInt32 value) => Convert.ToDateTime(value);
    /// <inheritdoc/>
    public static DateTime ToDateTime(UInt64 value) => Convert.ToDateTime(value);
    /// <inheritdoc/>
    public static DateTime ToDateTime(Single value) => Convert.ToDateTime(value);
    /// <inheritdoc/>
    public static DateTime ToDateTime(Double value) => Convert.ToDateTime(value);
    /// <inheritdoc/>
    public static DateTime ToDateTime(Decimal value) => Convert.ToDateTime(value);
    /// <inheritdoc/>
    public static DateTime ToDateTime(DateTime value) => value;
}
