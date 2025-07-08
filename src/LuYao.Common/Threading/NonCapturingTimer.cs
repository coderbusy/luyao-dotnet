using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LuYao.Threading;

/// <summary>
/// 提供与 <see cref="System.Threading.Timer"/> 交互的便捷 API，
/// 并避免捕获 <see cref="ExecutionContext"/>，
/// 防止异步上下文中的 AsyncLocal 数据被意外保留。
/// </summary>
internal static class NonCapturingTimer
{
    /// <summary>
    /// 创建一个不会捕获 <see cref="ExecutionContext"/> 的定时器。
    /// 推荐在需要使用定时器的场景下调用本方法，以避免异步上下文数据泄漏。
    /// </summary>
    /// <param name="callback">定时器回调方法。</param>
    /// <param name="state">传递给回调方法的状态对象。</param>
    /// <param name="dueTime">定时器首次触发的延迟时间。</param>
    /// <param name="period">定时器后续触发的周期时间。</param>
    /// <returns>新创建的 <see cref="Timer"/> 实例。</returns>
    /// <exception cref="ArgumentNullException">callback 参数为 null 时抛出。</exception>
    public static Timer Create(
        TimerCallback callback,
        object state,
        TimeSpan dueTime,
        TimeSpan period
    )
    {
        if (callback is null)
        {
            throw new ArgumentNullException(nameof(callback));
        }

        // 不要将当前的 ExecutionContext 及其 AsyncLocal 捕获到定时器上
        bool restoreFlow = false;
        try
        {
            if (!ExecutionContext.IsFlowSuppressed())
            {
                ExecutionContext.SuppressFlow();
                restoreFlow = true;
            }

            return new Timer(callback, state, dueTime, period);
        }
        finally
        {
            // 恢复当前的 ExecutionContext
            if (restoreFlow)
            {
                ExecutionContext.RestoreFlow();
            }
        }
    }
}
