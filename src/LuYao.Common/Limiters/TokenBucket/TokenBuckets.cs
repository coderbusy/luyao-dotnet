using System;
using System.Threading;

namespace LuYao.Limiters.TokenBucket;

/// <summary>
/// 提供令牌桶限流器的构建与相关策略配置的静态辅助类。
/// </summary>
public static class TokenBuckets
{
    /// <summary>
    /// 令牌桶构建器，用于配置并创建 <see cref="ITokenBucket"/> 实例。
    /// </summary>
    public class Builder
    {
        private long? _capacity;

        private IRefillStrategy _refillStrategy;

        private ISleepStrategy _sleepStrategy = YieldingSleepStrategyInstance;

        private readonly Ticker _ticker = Ticker.Default();

        /// <summary>
        /// 设置令牌桶的容量（最大令牌数）。
        /// </summary>
        /// <param name="numTokens">令牌桶容量，必须为正数。</param>
        /// <returns>返回当前 <see cref="Builder"/> 实例以便链式调用。</returns>
        public Builder WithCapacity(long numTokens)
        {
            if (numTokens <= 0) throw new ArgumentOutOfRangeException(
                nameof(numTokens),
                "Must specify a positive number of tokens"
            );
            _capacity = numTokens;
            return this;
        }

        /// <summary>
        /// 使用固定间隔补充策略配置令牌桶。
        /// </summary>
        /// <param name="refillTokens">每次补充的令牌数量。</param>
        /// <param name="period">补充间隔时间。</param>
        /// <returns>返回当前 <see cref="Builder"/> 实例以便链式调用。</returns>
        public Builder WithFixedIntervalRefillStrategy(long refillTokens, TimeSpan period)
        {
            return WithRefillStrategy(
                new FixedIntervalRefillStrategy(_ticker, refillTokens, period)
            );
        }

        /// <summary>
        /// 使用自定义补充策略配置令牌桶。
        /// </summary>
        /// <param name="refillStrategy">补充策略实例。</param>
        /// <returns>返回当前 <see cref="Builder"/> 实例以便链式调用。</returns>
        public Builder WithRefillStrategy(IRefillStrategy refillStrategy)
        {
            if (refillStrategy == null)
                throw new ArgumentNullException(nameof(refillStrategy));
            _refillStrategy = refillStrategy;
            return this;
        }

        /// <summary>
        /// 使用让出线程的休眠策略（Thread.Sleep(0)）。
        /// </summary>
        /// <returns>返回当前 <see cref="Builder"/> 实例以便链式调用。</returns>
        public Builder WithYieldingSleepStrategy()
        {
            return WithSleepStrategy(YieldingSleepStrategyInstance);
        }

        /// <summary>
        /// 使用忙等待休眠策略（Thread.SpinWait）。
        /// </summary>
        /// <returns>返回当前 <see cref="Builder"/> 实例以便链式调用。</returns>
        public Builder WithBusyWaitSleepStrategy()
        {
            return WithSleepStrategy(BusyWaitSleepStrategyInstance);
        }

        /// <summary>
        /// 使用自定义休眠策略配置令牌桶。
        /// </summary>
        /// <param name="sleepStrategy">休眠策略实例。</param>
        /// <returns>返回当前 <see cref="Builder"/> 实例以便链式调用。</returns>
        public Builder WithSleepStrategy(ISleepStrategy sleepStrategy)
        {
            if (sleepStrategy == null) throw new ArgumentNullException(nameof(sleepStrategy));
            _sleepStrategy = sleepStrategy;
            return this;
        }

        /// <summary>
        /// 构建并返回 <see cref="ITokenBucket"/> 实例。
        /// </summary>
        /// <returns>配置完成的 <see cref="ITokenBucket"/> 实例。</returns>
        public ITokenBucket Build()
        {
            if (!_capacity.HasValue) throw new InvalidOperationException("Must specify a capacity");
            return new TokenBucket(_capacity.Value, _refillStrategy, _sleepStrategy);
        }
    }

    private class YieldingSleepStrategy : ISleepStrategy
    {
        public void Sleep() => Thread.Sleep(0);
    }

    private class BusyWaitSleepStrategy : ISleepStrategy
    {
        public void Sleep() => Thread.SpinWait(1);
    }

    private static readonly ISleepStrategy YieldingSleepStrategyInstance = new YieldingSleepStrategy();

    private static readonly ISleepStrategy BusyWaitSleepStrategyInstance = new BusyWaitSleepStrategy();

    /// <summary>
    /// 创建并返回一个新的 <see cref="Builder"/> 实例，用于配置和构建令牌桶。
    /// </summary>
    /// <returns>新的 <see cref="Builder"/> 实例。</returns>
    public static Builder Construct()
    {
        return new Builder();
    }
}