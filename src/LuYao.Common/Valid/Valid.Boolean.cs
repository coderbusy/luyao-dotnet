using System;
using System.Collections.Generic;

namespace LuYao;

partial class Valid
{
    private static readonly ISet<string> FALSE_STRINGS = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "0", "false", "no", "off", "n", "f", "null"
    };

    /// <inheritdoc/>
    public static bool ToBoolean(string? value)
    {
        if (value == null || string.IsNullOrWhiteSpace(value)) return false;
        if (FALSE_STRINGS.Contains(value)) return false;
        string str = value.Trim();
        return FALSE_STRINGS.Contains(str) ? false : true;
    }

    /// <inheritdoc/>
    public static bool ToBoolean(char value)
    {
        switch (value)
        {
            case 't':
            case 'T':
                return true;
            case 'f':
            case 'F':
                return false;
        }
        return false;
    }

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
