using System;

namespace LuYao.Data.Internal;

/// <summary>
/// Internal type conversion helper for the Data layer.
/// </summary>
internal static class DataConvert
{
    /// <summary>
    /// Converts the specified value to the specified type, with Nullable support.
    /// </summary>
    public static object ChangeType(object value, Type type)
    {
        Type underlyingType = Nullable.GetUnderlyingType(type) ?? type;
        return Convert.ChangeType(value, underlyingType);
    }
}
