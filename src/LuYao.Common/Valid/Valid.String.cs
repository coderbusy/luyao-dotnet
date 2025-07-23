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
    public static string? ToString(object? value)
    {
        if (value is null) return null;
        if (Convert.IsDBNull(value)) return null;
        return value.ToString();
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
