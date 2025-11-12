namespace LuYao.Collections.Concurrent;

[TestClass]
public class AsyncQueueTests
{
    [TestMethod]
    public void Enqueue_ItemAdded_CountIncreases()
    {
        // Arrange
        var queue = new AsyncQueue<int>();

        // Act
        queue.Enqueue(1);

        // Assert
        Assert.AreEqual(1, queue.Count);
    }

    [TestMethod]
    public void EnqueueRange_ItemsAdded_CountMatches()
    {
        // Arrange
        var queue = new AsyncQueue<int>();
        var items = new List<int> { 1, 2, 3 };

        // Act
        queue.EnqueueRange(items);

        // Assert
        Assert.AreEqual(3, queue.Count);
    }

    [TestMethod]
    public async Task DequeueAsync_ItemAvailable_ReturnsItem()
    {
        // Arrange
        var queue = new AsyncQueue<int>();
        queue.Enqueue(1);

        // Act
        var result = await queue.DequeueAsync();

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public async Task DequeueAsync_NoItem_CancellationTokenTriggersException()
    {
        // Arrange
        var queue = new AsyncQueue<int>();
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsExactlyAsync<TaskCanceledException>(async () =>
        {
            await queue.DequeueAsync(cts.Token);
        });
    }

    [TestMethod]
    public async Task DequeueAsync_MultipleItems_ReturnsItemsInOrder()
    {
        // Arrange
        var queue = new AsyncQueue<int>();
        queue.EnqueueRange(new[] { 1, 2, 3 });

        // Act
        var first = await queue.DequeueAsync();
        var second = await queue.DequeueAsync();
        var third = await queue.DequeueAsync();

        // Assert
        Assert.AreEqual(1, first);
        Assert.AreEqual(2, second);
        Assert.AreEqual(3, third);
    }
}