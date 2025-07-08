using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Limiters.TokenBucket;

internal class TokenBucket : ITokenBucket
{
    private readonly long _capacity;

    private readonly IRefillStrategy _refillStrategy;

    private readonly ISleepStrategy _sleepStrategy;

    private long _size;

    private readonly object _syncRoot = new object();

    public TokenBucket(long capacity, IRefillStrategy refillStrategy, ISleepStrategy sleepStrategy)
    {
        _capacity = capacity;
        _refillStrategy = refillStrategy;
        _sleepStrategy = sleepStrategy;
        _size = 0L;
    }

    public bool TryConsume() => TryConsume(1L);

    public bool TryConsume(long numTokens)
    {
        if (numTokens <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(numTokens),
                "Number of tokens to consume must be positive"
            );
        if (numTokens > _capacity)
            throw new ArgumentOutOfRangeException(
                nameof(numTokens),
                "Number of tokens to consume must be less than the capacity of the bucket."
            );
        lock (_syncRoot)
        {
            long num = Math.Min(_capacity, Math.Max(0L, _refillStrategy.Refill()));
            _size = Math.Max(0L, Math.Min(_size + num, _capacity));
            if (numTokens > _size)
            {
                return false;
            }
            _size -= numTokens;
            return true;
        }
    }

    public void Consume()
    {
        Consume(1L);
    }

    public void Consume(long numTokens)
    {
        while (!TryConsume(numTokens))
        {
            _sleepStrategy.Sleep();
        }
    }
}
