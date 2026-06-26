using System;
using System.Threading.Tasks;

namespace LuYao;

/// <summary>
/// Provides guarded execution helpers to reduce try/catch boilerplate
/// while keeping behavior observable and configurable.
/// </summary>
public static class Guard
{
    /// <summary>
    /// Executes an action with guarded exception handling.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="onError">An optional callback invoked when a handled exception is caught.</param>
    /// <param name="when">An optional predicate to determine whether an exception should be handled.</param>
    /// <param name="rethrow">If <see langword="true"/>, rethrows handled exceptions after invoking <paramref name="onError"/>.</param>
    /// <returns><see langword="true"/> when execution succeeds; otherwise <see langword="false"/> for handled exceptions.</returns>
    public static bool TryRun(Action action, Action<Exception>? onError = null, Func<Exception, bool>? when = null, bool rethrow = false)
    {
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
    /// Executes an asynchronous action with guarded exception handling.
    /// </summary>
    /// <param name="action">The asynchronous action to execute.</param>
    /// <param name="onError">An optional callback invoked when a handled exception is caught.</param>
    /// <param name="when">An optional predicate to determine whether an exception should be handled.</param>
    /// <param name="rethrow">If <see langword="true"/>, rethrows handled exceptions after invoking <paramref name="onError"/>.</param>
    /// <returns>A task that returns <see langword="true"/> when execution succeeds; otherwise <see langword="false"/> for handled exceptions.</returns>
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
    /// Executes a function and returns its result, or a fallback value when a handled exception occurs.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <param name="func">The function to execute.</param>
    /// <param name="fallback">The fallback value returned when a handled exception occurs.</param>
    /// <param name="onError">An optional callback invoked when a handled exception is caught.</param>
    /// <param name="when">An optional predicate to determine whether an exception should be handled.</param>
    /// <param name="rethrow">If <see langword="true"/>, rethrows handled exceptions after invoking <paramref name="onError"/>.</param>
    /// <returns>The function result when successful; otherwise <paramref name="fallback"/> for handled exceptions.</returns>
    public static T TryGet<T>(Func<T> func, T fallback = default!, Action<Exception>? onError = null, Func<Exception, bool>? when = null, bool rethrow = false)
    {
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
    /// Executes an asynchronous function and returns its result,
    /// or a fallback value when a handled exception occurs.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <param name="func">The asynchronous function to execute.</param>
    /// <param name="fallback">The fallback value returned when a handled exception occurs.</param>
    /// <param name="onError">An optional callback invoked when a handled exception is caught.</param>
    /// <param name="when">An optional predicate to determine whether an exception should be handled.</param>
    /// <param name="rethrow">If <see langword="true"/>, rethrows handled exceptions after invoking <paramref name="onError"/>.</param>
    /// <returns>A task that returns the function result when successful; otherwise <paramref name="fallback"/> for handled exceptions.</returns>
    public static async Task<T> TryGetAsync<T>(Func<Task<T>> func, T fallback = default!, Action<Exception>? onError = null, Func<Exception, bool>? when = null, bool rethrow = false)
    {
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