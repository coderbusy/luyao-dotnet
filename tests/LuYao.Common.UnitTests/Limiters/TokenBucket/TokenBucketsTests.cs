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
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Builder_WithCapacity_NonPositive_ThrowsArgumentOutOfRangeException()
    {
        TokenBuckets.Construct().WithCapacity(0);
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
    [ExpectedException(typeof(ArgumentNullException))]
    public void Builder_WithRefillStrategy_Null_ThrowsArgumentNullException()
    {
        TokenBuckets.Construct().WithRefillStrategy(null);
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
    [ExpectedException(typeof(ArgumentNullException))]
    public void Builder_WithSleepStrategy_Null_ThrowsArgumentNullException()
    {
        TokenBuckets.Construct().WithSleepStrategy(null);
    }

    [TestMethod]
    public void Builder_WithSleepStrategy_Valid_SetsSleepStrategy()
    {
        var builder = TokenBuckets.Construct().WithSleepStrategy(new DummySleepStrategy());
        Assert.IsNotNull(builder);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Builder_Build_WithoutCapacity_ThrowsInvalidOperationException()
    {
        TokenBuckets.Construct().Build();
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
}