using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LuYao;

[TestClass]
public class GuardTests
{
    [TestMethod]
    public void TryRun_WhenActionSucceeds_ReturnsTrue()
    {
        bool result = Guard.TryRun(() => { });

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void TryRun_WhenActionThrows_ReturnsFalseAndInvokesOnError()
    {
        Exception? captured = null;

        bool result = Guard.TryRun(() => throw new InvalidOperationException("boom"), ex => captured = ex);

        Assert.IsFalse(result);
        Assert.IsNotNull(captured);
        Assert.IsInstanceOfType<InvalidOperationException>(captured);
    }

    [TestMethod]
    public void TryRun_WhenRethrowIsTrue_RethrowsHandledException()
    {
        Assert.Throws<InvalidOperationException>(() =>
            Guard.TryRun(() => throw new InvalidOperationException("boom"), rethrow: true));
    }

    [TestMethod]
    public void TryRun_WhenFilterReturnsFalse_DoesNotHandleException()
    {
        Assert.Throws<InvalidOperationException>(() =>
            Guard.TryRun(() => throw new InvalidOperationException("boom"), when: _ => false));
    }

    [TestMethod]
    public void TryRun_WhenOperationCanceledExceptionThrown_DoesNotHandleException()
    {
        Assert.Throws<OperationCanceledException>(() =>
            Guard.TryRun(() => throw new OperationCanceledException()));
    }

    [TestMethod]
    public void TryGet_WhenFuncThrows_ReturnsFallback()
    {
        int value = Guard.TryGet(() => throw new InvalidOperationException("boom"), fallback: 42);

        Assert.AreEqual(42, value);
    }

    [TestMethod]
    public async Task TryRunAsync_WhenActionThrows_ReturnsFalseAndInvokesOnError()
    {
        Exception? captured = null;

        bool result = await Guard.TryRunAsync(
            async () =>
            {
                await Task.Yield();
                throw new InvalidOperationException("boom");
            },
            ex => captured = ex);

        Assert.IsFalse(result);
        Assert.IsNotNull(captured);
        Assert.IsInstanceOfType<InvalidOperationException>(captured);
    }

    [TestMethod]
    public async Task TryRunAsync_WhenOperationCanceledExceptionThrown_DoesNotHandleException()
    {
        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            await Guard.TryRunAsync(async () =>
            {
                await Task.Yield();
                throw new OperationCanceledException();
            }));
    }

    [TestMethod]
    public async Task TryGetAsync_WhenFuncThrows_ReturnsFallback()
    {
        int value = await Guard.TryGetAsync(
            () => Task.FromException<int>(new InvalidOperationException("boom")),
            fallback: 64);

        Assert.AreEqual(64, value);
    }
}