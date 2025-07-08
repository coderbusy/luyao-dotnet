using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LuYao.Limiters.TokenBucket;

public static class TokenBuckets
{
    public class Builder
    {
        private long? _capacity;

        private IRefillStrategy _refillStrategy;

        private ISleepStrategy _sleepStrategy = YieldingSleepStrategyInstance;

        private readonly Ticker _ticker = Ticker.Default();

        public Builder WithCapacity(long numTokens)
        {
            if (numTokens <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(numTokens),
                    "Must specify a positive number of tokens"
                );
            _capacity = numTokens;
            return this;
        }

        public Builder WithFixedIntervalRefillStrategy(long refillTokens, TimeSpan period)
        {
            return WithRefillStrategy(
                new FixedIntervalRefillStrategy(_ticker, refillTokens, period)
            );
        }

        public Builder WithRefillStrategy(IRefillStrategy refillStrategy)
        {
            if (refillStrategy == null)
                throw new ArgumentNullException(nameof(refillStrategy));
            _refillStrategy = refillStrategy;
            return this;
        }

        public Builder WithYieldingSleepStrategy()
        {
            return WithSleepStrategy(YieldingSleepStrategyInstance);
        }

        public Builder WithBusyWaitSleepStrategy()
        {
            return WithSleepStrategy(BusyWaitSleepStrategyInstance);
        }

        public Builder WithSleepStrategy(ISleepStrategy sleepStrategy)
        {
            if (sleepStrategy == null)
                throw new ArgumentNullException(nameof(sleepStrategy));
            _sleepStrategy = sleepStrategy;
            return this;
        }

        public ITokenBucket Build()
        {
            if (!_capacity.HasValue)
                throw new InvalidOperationException("Must specify a capacity");
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

    private static readonly ISleepStrategy YieldingSleepStrategyInstance =
        new YieldingSleepStrategy();

    private static readonly ISleepStrategy BusyWaitSleepStrategyInstance =
        new BusyWaitSleepStrategy();

    public static Builder Construct()
    {
        return new Builder();
    }
}
