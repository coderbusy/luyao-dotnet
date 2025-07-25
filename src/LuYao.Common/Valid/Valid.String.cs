using System;

namespace LuYao;

partial class Valid
{
    /// <inheritdoc/>
    public static Boolean ToBoolean(String? value)
    {
        if (value is null) return default;
        return Convert.ToBoolean(value);
    }
    /// <inheritdoc/>
    public static Char ToChar(String? value)
    {
        if (value is null) return default;
        return Convert.ToChar(value);
    }
    /// <inheritdoc/>
    public static SByte ToSByte(String? value)
    {
        if (value is null) return default;
        return Convert.ToSByte(value);
    }
    /// <inheritdoc/>
    public static Byte ToByte(String? value)
    {
        if (value is null) return default;
        return Convert.ToByte(value);
    }
    /// <inheritdoc/>
    public static Int16 ToInt16(String? value)
    {
        if (value is null) return default;
        return Convert.ToInt16(value);
    }
    /// <inheritdoc/>
    public static UInt16 ToUInt16(String? value)
    {
        if (value is null) return default;
        return Convert.ToUInt16(value);
    }
    /// <inheritdoc/>
    public static Int32 ToInt32(String? value)
    {
        if (value is null) return default;
        return Convert.ToInt32(value);
    }
    /// <inheritdoc/>
    public static UInt32 ToUInt32(String? value)
    {
        if (value is null) return default;
        return Convert.ToUInt32(value);
    }
    /// <inheritdoc/>
    public static Int64 ToInt64(String? value)
    {
        if (value is null) return default;
        return Convert.ToInt64(value);
    }
    /// <inheritdoc/>
    public static UInt64 ToUInt64(String? value)
    {
        if (value is null) return default;
        return Convert.ToUInt64(value);
    }
    /// <inheritdoc/>
    public static Single ToSingle(String? value)
    {
        if (value is null) return default;
        return Convert.ToSingle(value);
    }
    /// <inheritdoc/>
    public static Double ToDouble(String? value)
    {
        if (value is null) return default;
        return Convert.ToDouble(value);
    }
    /// <inheritdoc/>
    public static Decimal ToDecimal(String? value)
    {
        if (value is null) return default;
        return Convert.ToDecimal(value);
    }
    /// <inheritdoc/>
    public static DateTime ToDateTime(String? value)
    {
        if (value is null) return default;
        return Convert.ToDateTime(value);
    }
    /// <inheritdoc/>
    public static String ToString(String? value)
    {
        if (value is null) return String.Empty;
        return Convert.ToString(value);
    }
}
