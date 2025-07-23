using System;
using System.Globalization;

namespace LuYao;

partial class Valid
{
    /// <summary>
    /// 将值转换为字符串。
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ToString(object? value)
    {
        switch (value)
        {
            case Boolean v1: return ToString(v1);
            case Byte v2: return ToString(v2);
            case Char v3: return ToString(v3);
            case DateTime v4: return ToString(v4);
            case Decimal v5: return ToString(v5);
            case Double v6: return ToString(v6);
            case Int16 v7: return ToString(v7);
            case Int32 v8: return ToString(v8);
            case Int64 v9: return ToString(v9);
            case SByte v10: return ToString(v10);
            case Single v11: return ToString(v11);
            case String v12: return ToString(v12);
            case UInt16 v13: return ToString(v13);
            case UInt32 v14: return ToString(v14);
            case UInt64 v15: return ToString(v15);
            default: return value?.ToString() ?? string.Empty;
        }
    }

    /// <summary>
    /// 将值转换为字符串。
    /// </summary>
    public static string ToString(bool? value) => value == true ? "1" : "0";

    /// <summary>
    /// 将值转换为字符串。
    /// </summary>
    public static string ToString(bool value) => value ? "1" : "0";

    /// <summary>
    /// 将值转换为字符串。
    /// </summary>
    public static string ToString(Char? value) => value?.ToString() ?? string.Empty;
    /// <summary>
    /// 将值转换为字符串。
    /// </summary>
    public static string ToString(Char value) => value.ToString();

    /// <summary>
    /// 将值转换为字符串。
    /// </summary>
    public static string ToString(SByte? value) => value?.ToString() ?? string.Empty;
    /// <summary>
    /// 将值转换为字符串。
    /// </summary>
    public static string ToString(SByte value) => value.ToString();

    /// <summary>
    /// 将值转换为字符串。
    /// </summary>
    public static string ToString(Byte? value) => value?.ToString() ?? string.Empty;
    /// <summary>
    /// 将值转换为字符串。
    /// </summary>
    public static string ToString(Byte value) => value.ToString();

    /// <summary>
    /// 将值转换为字符串。
    /// </summary>
    public static string ToString(Int16? value) => value?.ToString() ?? string.Empty;
    /// <summary>
    /// 将值转换为字符串。
    /// </summary>
    public static string ToString(Int16 value) => value.ToString();

    /// <summary>
    /// 将值转换为字符串。
    /// </summary>
    public static string ToString(UInt16? value) => value?.ToString() ?? string.Empty;
    /// <summary>
    /// 将值转换为字符串。
    /// </summary>
    public static string ToString(UInt16 value) => value.ToString();

    /// <summary>
    /// 将值转换为字符串。
    /// </summary>
    public static string ToString(Int32? value) => value?.ToString() ?? string.Empty;
    /// <summary>
    /// 将值转换为字符串。
    /// </summary>
    public static string ToString(Int32 value) => value.ToString();

    /// <summary>
    /// 将值转换为字符串。
    /// </summary>
    public static string ToString(UInt32? value) => value?.ToString() ?? string.Empty;
    /// <summary>
    /// 将值转换为字符串。
    /// </summary>
    public static string ToString(UInt32 value) => value.ToString();

    /// <summary>
    /// 将值转换为字符串。
    /// </summary>
    public static string ToString(Int64? value) => value?.ToString() ?? string.Empty;
    /// <summary>
    /// 将值转换为字符串。
    /// </summary>
    public static string ToString(Int64 value) => value.ToString();

    /// <summary>
    /// 将值转换为字符串。
    /// </summary>
    public static string ToString(UInt64? value) => value?.ToString() ?? string.Empty;
    /// <summary>
    /// 将值转换为字符串。
    /// </summary>
    public static string ToString(UInt64 value) => value.ToString();

    /// <summary>
    /// 将值转换为字符串。
    /// </summary>
    public static string ToString(Single? value) => value?.ToString() ?? string.Empty;
    /// <summary>
    /// 将值转换为字符串。
    /// </summary>
    public static string ToString(Single value) => value.ToString();

    /// <summary>
    /// 将值转换为字符串。
    /// </summary>
    public static string ToString(Double? value) => value?.ToString() ?? string.Empty;
    /// <summary>
    /// 将值转换为字符串。
    /// </summary>
    public static string ToString(Double value) => value.ToString();

    /// <summary>
    /// 将值转换为字符串。
    /// </summary>
    public static string ToString(Decimal? value) => value != null ? ToString(value.Value) : string.Empty;
    /// <summary>
    /// 将值转换为字符串。
    /// </summary>
    public static string ToString(Decimal value) => value.ToString("0.00##", CultureInfo.InvariantCulture);


    /// <summary>
    /// 将值转换为字符串。
    /// </summary>
    public static string ToString(string value) => value ?? string.Empty;

    /// <summary>
    /// 将值转换为字符串。
    /// </summary>
    public static string ToString(DateTime dt, Date format)
    {
        if (dt == default(DateTime)) return string.Empty;
        switch (format)
        {
            case Date.月日: return dt.ToString("MM/dd", DateTimeFormatInfo.InvariantInfo);
            case Date.小时分钟秒: return dt.ToString("HH:mm:ss", DateTimeFormatInfo.InvariantInfo);
            case Date.小时分钟: return dt.ToString("HH:mm", DateTimeFormatInfo.InvariantInfo);
            case Date.年月日小时分钟: return dt.ToString("yyyy/MM/dd HH:mm", DateTimeFormatInfo.InvariantInfo);
            case Date.月日年小时分钟: return dt.ToString("MM/dd/yyyy HH:mm", DateTimeFormatInfo.InvariantInfo);
            case Date.月日小时分钟: return dt.ToString("MM/dd HH:mm", DateTimeFormatInfo.InvariantInfo);
            case Date.全部显示: return dt.ToString("yyyy/MM/dd HH:mm:ss", DateTimeFormatInfo.InvariantInfo);
            default: return dt.ToString("yyyy/MM/dd", DateTimeFormatInfo.InvariantInfo);
        }
    }

    /// <summary>
    /// 将值转换为字符串。
    /// </summary>
    public static string ToString(DateTime value) => ToString(value, Date.年月日);
    /// <summary>
    /// 将值转换为字符串。
    /// </summary>
    public static string ToString(DateTime? value) => ToString(value ?? DateTime.MinValue, Date.年月日);
    /// <summary>
    /// 将值转换为字符串。
    /// </summary>
    public static string ToString(DateTime? value, Date format) => ToString(value ?? DateTime.MinValue, format);
}
