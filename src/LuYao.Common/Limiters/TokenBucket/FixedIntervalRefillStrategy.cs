using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Limiters.TokenBucket;

/// <summary>
/// 固定间隔补充令牌的策略实现。
/// </summary>
public class FixedIntervalRefillStrategy : IRefillStrategy
{
    private readonly Ticker _ticker;

    private readonly long _numTokens;

    private readonly long _periodInTicks;

    private long _nextRefillTime;

    private readonly object _syncRoot = new object();

    /// <summary>
    /// 初始化 <see cref="FixedIntervalRefillStrategy"/> 类的新实例。
    /// </summary>
    /// <param name="ticker">用于获取当前时间的 Ticker 实例。</param>
    /// <param name="numTokens">每次补充的令牌数量。</param>
    /// <param name="period">补充令牌的时间间隔。</param>
    public FixedIntervalRefillStrategy(Ticker ticker, long numTokens, TimeSpan period)
    {
        _ticker = ticker;
        _numTokens = numTokens;
        _periodInTicks = period.Ticks;
        _nextRefillTime = -1L;
    }

    /// <summary>
    /// 根据固定时间间隔补充令牌。
    /// </summary>
    /// <returns>本次应补充的令牌数量。</returns>
    public long Refill()
    {
        lock (_syncRoot)
        {
            long num = _ticker.Read();
            if (num < _nextRefillTime)
            {
                return 0L;
            }
            long num2 = Math.Max((num - _nextRefillTime) / _periodInTicks, 1L);
            _nextRefillTime += _periodInTicks * num2;
            return _numTokens * num2;
        }
    }
}
