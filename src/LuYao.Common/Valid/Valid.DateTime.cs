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
}
