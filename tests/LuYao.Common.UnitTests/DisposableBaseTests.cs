using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LuYao;

[TestClass]
public class DisposableBaseTests
{
    private class TestDisposable : DisposableBase
    {
        public bool IsDisposed => Disposed;
        public bool CustomResourceDisposed { get; private set; }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                CustomResourceDisposed = true;
            }
            base.Dispose(disposing);
        }
    }

    [TestMethod]
    public void NewInstance_Disposed_ShouldBeFalse()
    {
        using var sut = new TestDisposable();
        Assert.IsFalse(sut.IsDisposed);
    }

    [TestMethod]
    public void AfterDispose_Disposed_ShouldBeTrue()
    {
        var sut = new TestDisposable();
        sut.Dispose();
        Assert.IsTrue(sut.IsDisposed);
    }

    [TestMethod]
    public void MultipleDispose_ShouldBeAllowed()
    {
        var sut = new TestDisposable();
        sut.Dispose();
        sut.Dispose(); // 不应抛出异常
        Assert.IsTrue(sut.IsDisposed);
    }

    [TestMethod]
    public void Dispose_ShouldReleaseCustomResources()
    {
        var sut = new TestDisposable();
        sut.Dispose();
        Assert.IsTrue(sut.CustomResourceDisposed);
    }

    [TestMethod]
    public void UsingStatement_ShouldDisposeCorrectly()
    {
        TestDisposable sut;
        using (sut = new TestDisposable())
        {
            Assert.IsFalse(sut.IsDisposed);
        }
        Assert.IsTrue(sut.IsDisposed);
    }
}