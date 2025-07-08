using System;
using System.IO;

namespace LuYao.IO;

/// <summary>
/// 表示一个自动清理的临时文件，在对象释放时会自动删除对应的文件。
/// </summary>
public class AutoCleanTempFile : IDisposable
{
    /// <summary>
    /// 使用指定的文件名初始化 <see cref="AutoCleanTempFile"/> 类的新实例。
    /// </summary>
    /// <param name="fileName">临时文件的完整路径。</param>
    /// <exception cref="ArgumentException">当文件名为空或仅包含空白字符时抛出。</exception>
    public AutoCleanTempFile(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentException("File name cannot be null or whitespace.", nameof(fileName));
        this.FileName = fileName;
    }

    /// <summary>
    /// 创建一个新的自动清理的临时文件实例。
    /// </summary>
    /// <returns>返回 <see cref="AutoCleanTempFile"/> 的新实例。</returns>
    public static AutoCleanTempFile Create() => new AutoCleanTempFile(Path.GetTempFileName());

    /// <summary>
    /// 获取临时文件的完整路径。
    /// </summary>
    public string FileName { get; }

    /// <summary>
    /// 以写入方式打开临时文件。
    /// </summary>
    /// <returns>返回可写的 <see cref="FileStream"/> 实例。</returns>
    public FileStream OpenWrite() => File.OpenWrite(this.FileName);

    /// <summary>
    /// 以只读方式打开临时文件。
    /// </summary>
    /// <returns>返回只读的 <see cref="FileStream"/> 实例。</returns>
    public FileStream OpenRead() => File.OpenRead(this.FileName);

    private void Dispose(bool disposing)
    {
        if (disposing) GC.SuppressFinalize(this);
        if (File.Exists(FileName)) File.Delete(FileName);
    }

    /// <summary>
    /// 释放资源并删除临时文件。
    /// </summary>
    public void Dispose() => Dispose(true);

    /// <summary>
    /// 析构函数，用于释放资源并删除临时文件。
    /// </summary>
    ~AutoCleanTempFile() => Dispose(false);
}
