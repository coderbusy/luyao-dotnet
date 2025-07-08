using System;
using System.Net.Http;

namespace LuYao.Net.Http;

// 线程安全性：此类是不可变的
internal sealed class ExpiredHandlerTrackingEntry
{
    private readonly WeakReference _livenessTracker;

    // 重要：不要在这里缓存对 `other` 或 `other.Handler` 的引用。
    // 我们需要允许它被 GC 回收。
    public ExpiredHandlerTrackingEntry(ActiveHandlerTrackingEntry other)
    {
        Name = other.Name;

        _livenessTracker = new WeakReference(other.Handler);
        InnerHandler = other.Handler.InnerHandler!;
    }

    public bool CanDispose => !_livenessTracker.IsAlive;

    public HttpMessageHandler InnerHandler { get; }

    public string Name { get; }
}
