using System;
using System.Globalization;

namespace LuYao;

partial class Valid
{
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
    public static string ToString(DateTime? value, Date format) => ToString(value ?? default, format);

    /// <inheritdoc/>
    public static string ToString(DateTime value) => value.ToString("O");

    /// <inheritdoc/>
    public static bool ToBoolean(DateTime value) => false;

    /// <inheritdoc/>
    public static char ToChar(DateTime value) => default;

    /// <inheritdoc/>
    public static sbyte ToSByte(DateTime value) => default;

    /// <inheritdoc/>
    public static byte ToByte(DateTime value) => default;

    /// <inheritdoc/>
    public static short ToInt16(DateTime value) => default;

    /// <inheritdoc/>
    public static ushort ToUInt16(DateTime value) => default;

    /// <inheritdoc/>
    public static int ToInt32(DateTime value) => default;

    /// <inheritdoc/>
    public static uint ToUInt32(DateTime value) => default;

    /// <inheritdoc/>
    public static long ToInt64(DateTime value) => default;

    /// <inheritdoc/>
    public static ulong ToUInt64(DateTime value) => default;

    /// <inheritdoc/>
    public static float ToSingle(DateTime value) => default;

    /// <inheritdoc/>
    public static double ToDouble(DateTime value) => default;

    /// <inheritdoc/>
    public static decimal ToDecimal(DateTime value) => default;

    /// <inheritdoc/>
    public static DateTime ToDateTime(DateTime value) => value;
}
