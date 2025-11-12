using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LuYao.Limiters.TokenBucket;

[TestClass]
public class TokenBucketsTests
{
    [TestMethod]
    public void Construct_Always_ReturnsNewBuilder()
    {
        var builder1 = TokenBuckets.Construct();
        var builder2 = TokenBuckets.Construct();
        Assert.IsNotNull(builder1);
        Assert.IsNotNull(builder2);
        Assert.AreNotSame(builder1, builder2);
    }

    [TestMethod]
    public void Builder_WithCapacity_PositiveValue_SetsCapacity()
    {
        var builder = TokenBuckets.Construct().WithCapacity(10);
        Assert.IsNotNull(builder);
    }

    [TestMethod]
    public void Builder_WithCapacity_NonPositive_ThrowsArgumentOutOfRangeException()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => TokenBuckets.Construct().WithCapacity(0));
    }

    [TestMethod]
    public void Builder_WithFixedIntervalRefillStrategy_ValidArgs_SetsStrategy()
    {
        var builder = TokenBuckets.Construct()
            .WithCapacity(10)
            .WithFixedIntervalRefillStrategy(1, TimeSpan.FromSeconds(1));
        Assert.IsNotNull(builder);
    }

    [TestMethod]
    public void Builder_WithRefillStrategy_Null_ThrowsArgumentNullException()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => TokenBuckets.Construct().WithRefillStrategy(null));
    }

    [TestMethod]
    public void Builder_WithRefillStrategy_Valid_SetsStrategy()
    {
        var builder = TokenBuckets.Construct()
            .WithRefillStrategy(new DummyRefillStrategy());
        Assert.IsNotNull(builder);
    }

    [TestMethod]
    public void Builder_WithYieldingSleepStrategy_Always_SetsYieldingSleepStrategy()
    {
        var builder = TokenBuckets.Construct().WithYieldingSleepStrategy();
        Assert.IsNotNull(builder);
    }

    [TestMethod]
    public void Builder_WithBusyWaitSleepStrategy_Always_SetsBusyWaitSleepStrategy()
    {
        var builder = TokenBuckets.Construct().WithBusyWaitSleepStrategy();
        Assert.IsNotNull(builder);
    }

    [TestMethod]
    public void Builder_WithSleepStrategy_Null_ThrowsArgumentNullException()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => TokenBuckets.Construct().WithSleepStrategy(null));
    }

    [TestMethod]
    public void Builder_WithSleepStrategy_Valid_SetsSleepStrategy()
    {
        var builder = TokenBuckets.Construct().WithSleepStrategy(new DummySleepStrategy());
        Assert.IsNotNull(builder);
    }

    [TestMethod]
    public void Builder_Build_WithoutCapacity_ThrowsInvalidOperationException()
    {
        Assert.ThrowsExactly<InvalidOperationException>(() => TokenBuckets.Construct().Build());
    }

    // Dummy implementations for test
    private class DummyRefillStrategy : IRefillStrategy
    {
        public long Refill() => 1;
    }

    private class DummySleepStrategy : ISleepStrategy
    {
        public void Sleep() { }
    }

    private ITokenBucket Create()
    {
        return TokenBuckets.Construct()
                    .WithCapacity(10) // Burst capacity
                    .WithFixedIntervalRefillStrategy(5, TimeSpan.FromSeconds(1)) // Rate limit
                    .Build();
    }

    [TestMethod]
    public void Should_Allow_Requests_Within_Limit()
    {
        var limiter = Create();
        // 尝试发送 5 个请求，应该都能成功
        for (int i = 0; i < 5; i++)
        {
            Assert.IsTrue(limiter.TryConsume(1), "Request should be allowed.");
        }
    }

    [TestMethod]
    public void Should_Be_Allowed_To_Burst()
    {
        var limiter = Create();
        // 允许突发请求，尝试发送 10 个请求
        for (int i = 0; i < 10; i++)
        {
            Assert.IsTrue(limiter.TryConsume(1), "Burst request should be allowed.");
        }
    }

    [TestMethod]
    public void Should_Reject_Requests_When_Empty()
    {
        var limiter = Create();
        // 消耗 10 次以耗尽令牌
        for (int i = 0; i <= 10; i++)
        {
            limiter.TryConsume(1);
        }
        // 尝试发送第 11 个请求，应该被拒绝
        Assert.IsFalse(limiter.TryConsume(1), "Request should be rejected when no tokens are available.");
    }

    [TestMethod]
    public void Should_Refill_Tokens_Over_Time()
    {
        var limiter = Create();
        // 消耗 5 次以耗尽部分令牌
        for (int i = 0; i < 5; i++)
        {
            limiter.TryConsume(1);
        }

        // 等待一段时间，令牌应该补充
        Thread.Sleep(1000); // 等待 1 秒
                            // 应该能够成功消费 1 个令牌
        Assert.IsTrue(limiter.TryConsume(1), "Request should be allowed after refill.");
    }
}