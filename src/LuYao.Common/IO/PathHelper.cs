using System;
using System.Collections.Generic;
using System.Linq;

namespace LuYao.IO;

/// <summary>
/// 提供用于文件路径操作的辅助方法。
/// </summary>
public static class PathHelper
{
    private static readonly HashSet<char> InvalidFileNameChars = new HashSet<char>
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

        var safeFileName = string.Concat(FilterFileName(filename));

        return safeFileName;
    }
    private static IEnumerable<Char> FilterFileName(string str)
    {
        foreach (var c in str)
        {
            if (!InvalidFileNameChars.Contains(c)) yield return c;
        }
    }


    /// <summary>
    ///     根据文件后缀来获取MIME类型字符串
    /// </summary>
    public static string GetMimeType(string extension)
    {
        if (string.IsNullOrWhiteSpace(extension)) throw new ArgumentException(nameof(extension));
        string mime;
        extension = extension.ToLower();
        switch (extension)
        {
            case ".apk":
                mime = "application/vnd.android.package-archive";
                break;
            case ".avi":
                mime = "video/x-msvideo";
                break;
            case ".bin":
            case ".exe":
            case ".msi":
            case ".dll":
            case ".class":
                mime = "application/octet-stream";
                break;
            case ".csv":
                mime = "text/comma-separated-values";
                break;
            case ".html":
            case ".htm":
            case ".shtml":
                mime = "text/html";
                break;
            case ".css":
                mime = "text/css";
                break;
            case ".js":
                mime = "text/javascript";
                break;
            case ".doc":
            case ".dot":
            case ".docx":
                mime = "application/msword";
                break;
            case ".xla":
            case ".xls":
            case ".xlsx":
                mime = "application/msexcel";
                break;
            case ".ppt":
            case ".pptx":
                mime = "application/mspowerpoint";
                break;
            case ".gz":
                mime = "application/gzip";
                break;
            case ".gif":
                mime = "image/gif";
                break;
            case ".bmp":
                mime = "image/bmp";
                break;
            case ".jpeg":
            case ".jpg":
            case ".jpe":
            case ".png":
                mime = "image/jpeg";
                break;
            case ".mpeg":
            case ".mpg":
            case ".mpe":
            case ".wmv":
                mime = "video/mpeg";
                break;
            case ".mp3":
            case ".wma":
                mime = "audio/mpeg";
                break;
            case ".pdf":
                mime = "application/pdf";
                break;
            case ".rar":
                mime = "application/octet-stream";
                break;
            case ".txt":
                mime = "text/plain";
                break;
            case ".7z":
            case ".z":
                mime = "application/x-compress";
                break;
            case ".zip":
                mime = "application/x-zip-compressed";
                break;
            default:
                mime = "application/octet-stream";
                break;
        }
        return mime;
    }
}
