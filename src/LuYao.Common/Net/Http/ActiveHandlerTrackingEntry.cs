using LuYao.Threading;
using System;
using System.Diagnostics;
using System.Threading;

namespace LuYao.Net.Http;

// 线程安全性：我们将此类视为不可变的，除了定时器以外。为“过期”池创建新对象大大简化了线程需求。
internal sealed class ActiveHandlerTrackingEntry
{
    private static readonly TimerCallback _timerCallback = (s) =>
        ((ActiveHandlerTrackingEntry)s!).Timer_Tick();
    private readonly object _lock;
    private bool _timerInitialized;
    private Timer _timer;
    private TimerCallback _callback;

    public ActiveHandlerTrackingEntry(
        string name,
        LifetimeTrackingHttpMessageHandler handler,
        TimeSpan lifetime
    )
    {
        Name = name;
        Handler = handler;
        Lifetime = lifetime;

        _lock = new object();
    }

    public LifetimeTrackingHttpMessageHandler Handler { get; private set; }

    public TimeSpan Lifetime { get; }

    public string Name { get; }

    public void StartExpiryTimer(TimerCallback callback)
    {
        if (Lifetime == Timeout.InfiniteTimeSpan)
            return; // 永不过期。

        if (Volatile.Read(ref _timerInitialized))
            return;

        StartExpiryTimerSlow(callback);
    }

    private void StartExpiryTimerSlow(TimerCallback callback)
    {
        Debug.Assert(Lifetime != Timeout.InfiniteTimeSpan);

        lock (_lock)
        {
            if (Volatile.Read(ref _timerInitialized))
                return;

            _callback = callback;
            _timer = NonCapturingTimer.Create(
                _timerCallback,
                this,
                Lifetime,
                Timeout.InfiniteTimeSpan
            );
            _timerInitialized = true;
        }
    }

    private void Timer_Tick()
    {
        Debug.Assert(_callback != null);
        Debug.Assert(_timer != null);

        lock (_lock)
        {
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;

                _callback(this);
            }
        }
    }
}
