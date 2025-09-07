using System.IO;

namespace LuYao.IO;

/// <summary>
/// 文件操作帮助类，提供常用的文件操作方法
/// </summary>
public static class FileHelper
{
    /// <summary>
    /// 判断指定文件是否为空
    /// </summary>
    /// <param name="fileName">文件路径</param>
    /// <returns>
    /// 如果文件不存在或文件大小为0，则返回true；
    /// 否则返回false
    /// </returns>
    public static bool IsEmpty(string fileName)
    {
        var info = new FileInfo(fileName);
        return !info.Exists || info.Length == 0;
    }
}
