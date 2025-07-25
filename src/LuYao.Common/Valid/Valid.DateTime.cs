using System;
using System.Globalization;

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
    public static DateTime ToDateTime(bool value) => default;

    /// <inheritdoc/>
    public static DateTime ToDateTime(char value) => default;

    /// <inheritdoc/>
    public static DateTime ToDateTime(sbyte value) => default;

    /// <inheritdoc/>
    public static DateTime ToDateTime(byte value) => default;

    /// <inheritdoc/>
    public static DateTime ToDateTime(short value) => default;

    /// <inheritdoc/>
    public static DateTime ToDateTime(ushort value) => default;

    /// <inheritdoc/>
    public static DateTime ToDateTime(int value)
    {
        long ticks = value;
        if (ticks < DateTime.MinValue.Ticks || ticks > DateTime.MaxValue.Ticks) return default;
        return new DateTime(ticks);
    }

    /// <inheritdoc/>
    public static DateTime ToDateTime(uint value)
    {
        long ticks = value;
        if (ticks < DateTime.MinValue.Ticks || ticks > DateTime.MaxValue.Ticks) return default;
        return new DateTime(ticks);
    }

    /// <inheritdoc/>
    public static DateTime ToDateTime(ulong value)
    {
        // ulong 可能超出 long 范围
        if (value > (ulong)DateTime.MaxValue.Ticks) return default;
        long ticks = (long)value;
        if (ticks < DateTime.MinValue.Ticks || ticks > DateTime.MaxValue.Ticks) return default;
        return new DateTime(ticks);
    }

    /// <inheritdoc/>
    public static DateTime ToDateTime(float value)
    {
        // System.Convert.ToDateTime(float) 先转 double，再转 ticks
        if (float.IsNaN(value) || float.IsInfinity(value)) return default;
        double d = value;
        if (d < DateTime.MinValue.Ticks || d > DateTime.MaxValue.Ticks) return default;
        return new DateTime((long)d);
    }

    /// <inheritdoc/>
    public static DateTime ToDateTime(double value)
    {
        if (double.IsNaN(value) || double.IsInfinity(value)) return default;
        if (value < DateTime.MinValue.Ticks || value > DateTime.MaxValue.Ticks) return default;
        return new DateTime((long)value);
    }

    /// <inheritdoc/>
    public static DateTime ToDateTime(decimal value)
    {
        if (value < DateTime.MinValue.Ticks || value > DateTime.MaxValue.Ticks) return default;
        return new DateTime((long)value);
    }

    /// <inheritdoc/>
    public static DateTime ToDateTime(DateTime value) => value;
}
