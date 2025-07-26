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

    /// <inheritdoc/>
    public static string ToString(DateTime value) => value.ToString("O");
}
