using System;

namespace LuYao;

/// <summary>
/// 此类用于在调用 Dispose 方法时
/// 执行指定的操作。
/// </summary>
public class DisposeAction : IDisposable
{
    private readonly Action _action;

    /// <summary>
    /// 创建一个新的 <see cref="DisposeAction"/> 对象。
    /// </summary>
    /// <param name="action">当此对象被释放时要执行的操作。</param>
    public DisposeAction(Action action)
    {
        if (action == null) throw new ArgumentNullException(nameof(action));

        _action = action;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _action();
    }
}