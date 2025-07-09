using System;
using System.Linq;

namespace LuYao.IO;

/// <summary>
/// 提供用于文件路径操作的辅助方法。
/// </summary>
public static class PathHelper
{
    private static readonly char[] InvalidFileNameChars = new char[]
    {
        '\"', '<', '>', '|', '\0',
        (char)1, (char)2, (char)3, (char)4, (char)5, (char)6, (char)7, (char)8, (char)9, (char)10,
        (char)11, (char)12, (char)13, (char)14, (char)15, (char)16, (char)17, (char)18, (char)19, (char)20,
        (char)21, (char)22, (char)23, (char)24, (char)25, (char)26, (char)27, (char)28, (char)29, (char)30,
        (char)31, ':', '*', '?', '\\', '/'
    };

    /// <summary>
    /// 返回一个安全的文件名，移除无效字符。
    /// </summary>
    /// <param name="filename">原始文件名。</param>
    /// <returns>移除无效字符后的安全文件名。</returns>
    public static string SafeFileName(string filename)
    {
        if (string.IsNullOrWhiteSpace(filename)) return string.Empty;

        var safeFileName = string.Concat(filename.Where(c => Array.IndexOf(InvalidFileNameChars, c) == -1));

        return safeFileName;
    }
}
