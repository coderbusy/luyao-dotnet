using System;
using System.Collections.Generic;

namespace LuYao;

partial class Valid
{
    private static readonly ISet<string> TRUE_STRINGS = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "1", "true", "yes", "on", "y", "t"
    };

    /// <inheritdoc/>
    public static bool ToBoolean(string? value)
    {
        if (value == null || string.IsNullOrWhiteSpace(value)) return false;
        if (TRUE_STRINGS.Contains(value)) return true;
        string str = value.Trim();
        return TRUE_STRINGS.Contains(str) ? true : false;
    }

    /// <inheritdoc/>
    public static bool ToBoolean(char value) => false;

    /// <inheritdoc/>
    public static bool ToBoolean(byte value) => value != 0;

    /// <inheritdoc/>
    public static bool ToBoolean(sbyte value) => value != 0;

    /// <inheritdoc/>
    public static bool ToBoolean(short value) => value != 0;

    /// <inheritdoc/>
    public static bool ToBoolean(ushort value) => value != 0;

    /// <inheritdoc/>
    public static bool ToBoolean(int value) => value != 0;

    /// <inheritdoc/>
    public static bool ToBoolean(uint value) => value != 0;

    /// <inheritdoc/>
    public static bool ToBoolean(long value) => value != 0;

    /// <inheritdoc/>
    public static bool ToBoolean(ulong value) => value != 0;

    /// <inheritdoc/>
    public static bool ToBoolean(float value) => value != 0;

    /// <inheritdoc/>
    public static bool ToBoolean(double value) => value != 0;

    /// <inheritdoc/>
    public static bool ToBoolean(decimal value) => value != 0;

    /// <inheritdoc/>
    public static Boolean ToBoolean(DateTime value) => false;

    /// <inheritdoc/>
    public static Boolean ToBoolean(Boolean value) => value;
}
