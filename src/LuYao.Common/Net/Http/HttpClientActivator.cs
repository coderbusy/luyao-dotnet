using LuYao.Threading;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;

namespace LuYao.Net.Http;

/// <summary>
/// HttpClient 激活器，用于根据名称创建和管理 HttpClient 实例，
/// 支持处理程序的生命周期管理与自动清理，避免 Socket 耗尽问题。
/// </summary>
public class HttpClientActivator
{
    private static readonly TimerCallback CleanupCallback = (s) =>
        ((HttpClientActivator)s!).CleanupTimer_Tick();
    private static readonly TimeSpan DefaultCleanupInterval = TimeSpan.FromSeconds(10);

    private readonly ConcurrentQueue<ExpiredHandlerTrackingEntry> _expiredHandlers;
    private readonly TimerCallback _expiryCallback;

    private readonly HttpClientActivatorOption _option;
    private readonly Func<string, Lazy<ActiveHandlerTrackingEntry>> _entryFactory;
    private readonly ConcurrentDictionary<string, Lazy<ActiveHandlerTrackingEntry>> _activeHandlers;

    private Timer _cleanupTimer;
    private readonly object _cleanupTimerLock;
    private readonly object _cleanupActiveLock;

    /// <summary>
    /// 初始化 <see cref="HttpClientActivator"/> 类的新实例。
    /// </summary>
    /// <param name="option">HttpClient 激活器的配置选项。</param>
    public HttpClientActivator(HttpClientActivatorOption option)
    {
        _option = option ?? throw new ArgumentNullException(nameof(option));
        _activeHandlers = new ConcurrentDictionary<string, Lazy<ActiveHandlerTrackingEntry>>(
            StringComparer.Ordinal
        );
        _entryFactory = (name) =>
        {
            return new Lazy<ActiveHandlerTrackingEntry>(
                () => CreateHandlerEntry(name),
                LazyThreadSafetyMode.ExecutionAndPublication
            );
        };
        _expiredHandlers = new ConcurrentQueue<ExpiredHandlerTrackingEntry>();
        _expiryCallback = ExpiryTimer_Tick;

        _cleanupTimerLock = new object();
        _cleanupActiveLock = new object();
    }

    private void ExpiryTimer_Tick(object state)
    {
        var active = (ActiveHandlerTrackingEntry)state!;
        var removed = _activeHandlers.TryRemove(active.Name, out var found);
        var expired = new ExpiredHandlerTrackingEntry(active);
        _expiredHandlers.Enqueue(expired);

        StartCleanupTimer();
    }

    private ActiveHandlerTrackingEntry CreateHandlerEntry(string name)
    {
        var handler = new LifetimeTrackingHttpMessageHandler(_option.Factory(name));
        return new ActiveHandlerTrackingEntry(name, handler, _option.HandlerLifetime);
    }

    /// <summary>
    /// 启动清理定时器，用于定期处理已过期的处理程序，释放相关资源，防止资源泄漏。
    /// 如果定时器尚未启动，则创建并启动定时器。
    /// </summary>
    protected virtual void StartCleanupTimer()
    {
        lock (_cleanupTimerLock)
        {
            _cleanupTimer ??= NonCapturingTimer.Create(
                CleanupCallback,
                this,
                DefaultCleanupInterval,
                Timeout.InfiniteTimeSpan
            );
        }
    }

    /// <summary>
    /// 停止清理定时器，释放定时器资源。
    /// </summary>
    protected virtual void StopCleanupTimer()
    {
        lock (_cleanupTimerLock)
        {
            _cleanupTimer!.Dispose();
            _cleanupTimer = null;
        }
    }

    private void CleanupTimer_Tick()
    {
        StopCleanupTimer();

        if (!Monitor.TryEnter(_cleanupActiveLock))
        {
            StartCleanupTimer();
            return;
        }

        try
        {
            var initialCount = _expiredHandlers.Count;

            var disposedCount = 0;
            for (var i = 0; i < initialCount; i++)
            {
                _expiredHandlers.TryDequeue(out var entry);
                Debug.Assert(
                    entry != null,
                    "Entry was null, we should always get an entry back from TryDequeue"
                // 条目为 null，我们应该总是能从 TryDequeue 得到一个条目
                );

                if (entry.CanDispose)
                {
                    try
                    {
                        entry.InnerHandler.Dispose();
                        disposedCount++;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error disposing handler: {ex}");
                        // 释放处理程序时出错
                    }
                }
                else
                {
                    // 如果条目仍然存活，则将其重新放回队列，以便下一个清理周期处理。
                    _expiredHandlers.Enqueue(entry);
                }
            }
        }
        finally
        {
            Monitor.Exit(_cleanupActiveLock);
        }

        // 如果清理队列未完全清空，则稍后重试。
        if (!_expiredHandlers.IsEmpty)
            StartCleanupTimer();
    }

    /// <summary>
    /// 启动处理程序条目的过期定时器，到期后自动触发处理程序的回收。
    /// </summary>
    /// <param name="entry">要启动定时器的处理程序条目。</param>
    internal virtual void StartHandlerEntryTimer(ActiveHandlerTrackingEntry entry)
    {
        entry.StartExpiryTimer(_expiryCallback);
    }

    private HttpMessageHandler CreateHandler(string name)
    {
        if (name == null) throw new ArgumentNullException(nameof(name));

        var entry = _activeHandlers.GetOrAdd(name, _entryFactory).Value;

        StartHandlerEntryTimer(entry);

        return entry.Handler;
    }

    /// <summary>
    /// 创建一个带有指定名称的 <see cref="HttpClient"/> 实例。
    /// 每个名称对应独立的处理程序生命周期。
    /// </summary>
    /// <param name="name">HttpClient 实例的名称。</param>
    /// <returns>新建或复用的 <see cref="HttpClient"/> 实例。</returns>
    public HttpClient Create(string name)
    {
        if (name == null) throw new ArgumentNullException(nameof(name));
        var handler = CreateHandler(name);
        var client = new HttpClient(handler, false);

        for (var i = 0; i < _option.HttpClientActions.Count; i++) _option.HttpClientActions[i](client);

        return client;
    }

    /// <summary>
    /// 创建一个默认名称（空字符串）的 <see cref="HttpClient"/> 实例。
    /// </summary>
    /// <returns>新建或复用的 <see cref="HttpClient"/> 实例。</returns>
    public HttpClient Create() => Create(string.Empty);
}
