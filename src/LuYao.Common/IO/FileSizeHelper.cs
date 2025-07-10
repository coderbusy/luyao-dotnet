using System;
using System.IO;

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

    /// <summary>
    /// 从文件路径获取文件大小。
    /// </summary>
    /// <param name="path">文件路径。</param>
    /// <returns>文件大小（字节数）。如果文件不存在，则返回 0。</returns>
    public static long ForFile(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return 0;
        var info = new FileInfo(path);
        if (!info.Exists) return 0;
        return info.Length;
    }

    /// <summary>
    /// 从目录路径获取目录大小。
    /// </summary>
    /// <param name="path">目录路径。</param>
    /// <returns>目录大小（字节数）。如果目录不存在，则返回 0。</returns>
    public static long ForDirectory(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return 0;
        if (!Directory.Exists(path)) return 0;
        long totalSize = 0;
        var files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            var fileInfo = new FileInfo(file);
            totalSize += fileInfo.Length;
        }
        return totalSize;
    }
}
