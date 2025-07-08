using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Limiters.TokenBucket;

/// <summary>
/// 令牌桶实现，基于 ITokenBucket 接口。
/// </summary>
internal class TokenBucket : ITokenBucket
{
    private readonly long _capacity;
    private readonly IRefillStrategy _refillStrategy;
    private readonly ISleepStrategy _sleepStrategy;
    private long _size;
    private readonly object _syncRoot = new object();

    /// <summary>
    /// 初始化 TokenBucket 实例。
    /// </summary>
    /// <param name="capacity">令牌桶容量。</param>
    /// <param name="refillStrategy">补充策略。</param>
    /// <param name="sleepStrategy">休眠策略。</param>
    public TokenBucket(long capacity, IRefillStrategy refillStrategy, ISleepStrategy sleepStrategy)
    {
        _capacity = capacity;
        _refillStrategy = refillStrategy;
        _sleepStrategy = sleepStrategy;
        _size = 0L;
    }

    /// <summary>
    /// 尝试消耗一个令牌。
    /// </summary>
    /// <returns>如果成功消耗令牌，则返回 true；否则返回 false。</returns>
    public bool TryConsume() => TryConsume(1L);

    /// <summary>
    /// 尝试消耗指定数量的令牌。
    /// </summary>
    /// <param name="numTokens">要消耗的令牌数量。</param>
    /// <returns>如果成功消耗指定数量的令牌，则返回 true；否则返回 false。</returns>
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

    /// <summary>
    /// 消耗一个令牌（如果没有足够的令牌则阻塞直到可用）。
    /// </summary>
    public void Consume()
    {
        Consume(1L);
    }

    /// <summary>
    /// 消耗指定数量的令牌（如果没有足够的令牌则阻塞直到可用）。
    /// </summary>
    /// <param name="numTokens">要消耗的令牌数量。</param>
    public void Consume(long numTokens)
    {
        while (!TryConsume(numTokens))
        {
            _sleepStrategy.Sleep();
        }
    }
}
