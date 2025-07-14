using System;
using System.Runtime.CompilerServices;

namespace LuYao.Logging.Loggers;

/// <summary>
/// 一个空实现的日志记录器，所有方法均未实现。
/// </summary>
public sealed class NullLogger : ILogger
{
    /// <summary>  
    /// 获取 NullLogger 的单例实例。  
    /// </summary>  
    public static NullLogger Instance { get; } = new NullLogger();

    /// <inheritdoc />
    public void Error(string message, Exception? exception = null, [CallerMemberName] string? callerMemberName = null)
    {
    }

    /// <inheritdoc />
    public void Error(Exception exception, string? message = null, [CallerMemberName] string? callerMemberName = null)
    {
    }

    /// <inheritdoc />
    public void Fatal(Exception exception, string message, [CallerMemberName] string? callerMemberName = null)
    {
    }

    /// <inheritdoc />
    public void Message(string text, [CallerMemberName] string? callerMemberName = null)
    {
    }

    /// <inheritdoc />
    public void Trace(string text, [CallerMemberName] string? callerMemberName = null)
    {
    }

    /// <inheritdoc />
    public void Warning(string message, [CallerMemberName] string? callerMemberName = null)
    {
    }
}
