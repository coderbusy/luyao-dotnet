using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Limiters.TokenBucket;

public class FixedIntervalRefillStrategy : IRefillStrategy
{
    private readonly Ticker _ticker;

    private readonly long _numTokens;

    private readonly long _periodInTicks;

    private long _nextRefillTime;

    private readonly object _syncRoot = new object();

    public FixedIntervalRefillStrategy(Ticker ticker, long numTokens, TimeSpan period)
    {
        _ticker = ticker;
        _numTokens = numTokens;
        _periodInTicks = period.Ticks;
        _nextRefillTime = -1L;
    }

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
