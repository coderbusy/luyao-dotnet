using System;

namespace LuYao.Data.Internal;

/// <summary>
/// 数据层的内部类型转换助手。
/// </summary>
internal static class DataConvert
{
    /// <summary>
    /// 将指定值转换为指定类型，并支持 Nullable。
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
