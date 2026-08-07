using System;
using System.Threading.Tasks;

namespace LuYao;

/// <summary>
/// 提供受保护的执行帮助程序以减少 try/catch 样板文件
/// 同时保持行为可观察和可配置。
/// </summary>
public static class Guard
{
    /// <summary>
    /// 执行带有受保护异常处理的操作。
    /// </summary>
    /// <param name="action">要执行的操作。</param>
    /// <param name="onError">捕获已处理异常时调用的可选回调。</param>
    /// <param name="when">用于确定是否应处理异常的可选谓词。</param>
    /// <param name="rethrow">如果 <see langword="true"/>，则在调用 <paramref name="onError"/> 后重新抛出已处理的异常。</param>
    /// <returns><see langword="true"/> 执行成功时；否则 <see langword="false"/> 以了解已处理的异常。</returns>
    public static bool TryRun(Action action, Action<Exception>? onError = null, Func<Exception, bool>? when = null, bool rethrow = false)
    {
        if (action == null) throw new ArgumentNullException(nameof(action));

        try
        {
            action();
            return true;
        }
        catch (Exception ex) when (CanHandle(ex, when))
        {
            onError?.Invoke(ex);
            if (rethrow) throw;
            return false;
        }
    }

    /// <summary>
    /// 执行带有受保护异常处理的异步操作。
    /// </summary>
    /// <param name="action">要执行的异步操作。</param>
    /// <param name="onError">捕获已处理异常时调用的可选回调。</param>
    /// <param name="when">用于确定是否应处理异常的可选谓词。</param>
    /// <param name="rethrow">如果 <see langword="true"/>，则在调用 <paramref name="onError"/> 后重新抛出已处理的异常。</param>
    /// <returns>执行成功时返回<see langword="true"/>的任务；否则 <see langword="false"/> 以了解已处理的异常。</returns>
    public static async Task<bool> TryRunAsync(Func<Task> action, Action<Exception>? onError = null, Func<Exception, bool>? when = null, bool rethrow = false)
    {
        try
        {
            await action();
            return true;
        }
        catch (Exception ex) when (CanHandle(ex, when))
        {
            onError?.Invoke(ex);
            if (rethrow) throw;
            return false;
        }
    }

    /// <summary>
    /// 执行函数并返回其结果，或在发生已处理异常时返回回退值。
    /// </summary>
    /// <typeparam name="T">结果类型。</typeparam>
    /// <param name="func">要执行的函数。</param>
    /// <param name="fallback">发生已处理异常时返回的回退值。</param>
    /// <param name="onError">捕获已处理异常时调用的可选回调。</param>
    /// <param name="when">用于确定是否应处理异常的可选谓词。</param>
    /// <param name="rethrow">如果 <see langword="true"/>，则在调用 <paramref name="onError"/> 后重新抛出已处理的异常。</param>
    /// <returns>成功时的函数结果；否则 <paramref name="fallback"/> 用于处理异常。</returns>
    public static T TryGet<T>(Func<T> func, T fallback = default!, Action<Exception>? onError = null, Func<Exception, bool>? when = null, bool rethrow = false)
    {
        if (func == null) throw new ArgumentNullException(nameof(func));

        try
        {
            return func();
        }
        catch (Exception ex) when (CanHandle(ex, when))
        {
            onError?.Invoke(ex);
            if (rethrow) throw;
            return fallback;
        }
    }

    /// <summary>
    /// 执行异步函数并返回其结果，
    /// 或发生已处理异常时的后备值。
    /// </summary>
    /// <typeparam name="T">结果类型。</typeparam>
    /// <param name="func">要执行的异步函数。</param>
    /// <param name="fallback">发生已处理异常时返回的回退值。</param>
    /// <param name="onError">捕获已处理异常时调用的可选回调。</param>
    /// <param name="when">用于确定是否应处理异常的可选谓词。</param>
    /// <param name="rethrow">如果 <see langword="true"/>，则在调用 <paramref name="onError"/> 后重新抛出已处理的异常。</param>
    /// <returns>成功时返回函数结果的任务；否则 <paramref name="fallback"/> 用于处理异常。</returns>
    public static async Task<T> TryGetAsync<T>(Func<Task<T>> func, T fallback = default!, Action<Exception>? onError = null, Func<Exception, bool>? when = null, bool rethrow = false)
    {
        if (func == null) throw new ArgumentNullException(nameof(func));

        try
        {
            return await func();
        }
        catch (Exception ex) when (CanHandle(ex, when))
        {
            onError?.Invoke(ex);
            if (rethrow) throw;
            return fallback;
        }
    }

    private static bool CanHandle(Exception ex, Func<Exception, bool>? when)
    {
        if (IsCritical(ex)) return false;
        if (when == null) return true;
        return when(ex);
    }

    private static bool IsCritical(Exception ex)
    {
        return ex is OperationCanceledException
            || ex is OutOfMemoryException
            || ex is StackOverflowException
            || ex is AccessViolationException
            || ex is AppDomainUnloadedException
            || ex is BadImageFormatException
            || ex is CannotUnloadAppDomainException
            || ex is InvalidProgramException;
    }
}
