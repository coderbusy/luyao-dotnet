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
    public static string ToString(DateTime? value, Date format) => ToString(value ?? DateTime.MinValue, format);

    /// <inheritdoc/>
    public static string ToString(DateTime value) => value.ToString("O");
    /// <inheritdoc/>
    public static Boolean ToBoolean(DateTime value) => Convert.ToBoolean(value);
    /// <inheritdoc/>
    public static Char ToChar(DateTime value) => Convert.ToChar(value);
    /// <inheritdoc/>
    public static SByte ToSByte(DateTime value) => Convert.ToSByte(value);
    /// <inheritdoc/>
    public static Byte ToByte(DateTime value) => Convert.ToByte(value);
    /// <inheritdoc/>
    public static Int16 ToInt16(DateTime value) => Convert.ToInt16(value);
    /// <inheritdoc/>
    public static UInt16 ToUInt16(DateTime value) => Convert.ToUInt16(value);
    /// <inheritdoc/>
    public static Int32 ToInt32(DateTime value) => Convert.ToInt32(value);
    /// <inheritdoc/>
    public static UInt32 ToUInt32(DateTime value) => Convert.ToUInt32(value);
    /// <inheritdoc/>
    public static Int64 ToInt64(DateTime value) => Convert.ToInt64(value);
    /// <inheritdoc/>
    public static UInt64 ToUInt64(DateTime value) => Convert.ToUInt64(value);
    /// <inheritdoc/>
    public static Single ToSingle(DateTime value) => Convert.ToSingle(value);
    /// <inheritdoc/>
    public static Double ToDouble(DateTime value) => Convert.ToDouble(value);
    /// <inheritdoc/>
    public static Decimal ToDecimal(DateTime value) => Convert.ToDecimal(value);
    /// <inheritdoc/>
    public static DateTime ToDateTime(DateTime value) => value;
}
