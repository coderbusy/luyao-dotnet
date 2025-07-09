using System;

namespace LuYao.IO;

/// <summary>
/// 文件大小帮助类，用于将文件大小转换为可读的显示格式。
/// </summary>
public static class FileSizeHelper
{
    private static readonly long[] FileLengthLimit =
    {
        0L,
        1024L,
        1048576L,
        1073741824L,
        1099511627776L,
        1125899906842624L,
        1152921504606847000L
    };

    private static readonly string[] FileLengthUnit = { "Byte", "KB", "MB", "GB", "TB", "PB" };

    /// <summary>
    /// 获取文件大小的显示格式。
    /// </summary>
    /// <param name="count">文件大小（字节数）。</param>
    /// <returns>格式化后的文件大小字符串，例如 "1.23 MB"。</returns>
    public static string GetDisplayName(long count)
    {
        if (count <= 0) return "0 Byte";
        int x = 0;
        for (int i = 0; i < FileLengthLimit.Length; i++)
        {
            x = i;
            if (count < FileLengthLimit[i]) break;
        }
        var val = FileLengthLimit[x - 1] == 0 ? count : count / Convert.ToDecimal(FileLengthLimit[x - 1]);
        return $"{val:0.00} {FileLengthUnit[x - 1]}";
    }
}
