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
        if (underlyingType.IsEnum)
        {
            if (value is string str)
            {
                return Enum.Parse(underlyingType, str);
            }
            else
            {
                return Enum.ToObject(underlyingType, value);
            }
        }
        return Convert.ChangeType(value, underlyingType);
    }
}
