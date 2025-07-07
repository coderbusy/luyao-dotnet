using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LuYao;

[TestClass]
public class DisposeActionTests
{
    [TestMethod]
    public void Constructor_WhenActionIsNull_ShouldThrowArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() => new DisposeAction(null!));
    }

    [TestMethod]
    public void Dispose_ShouldExecuteAction()
    {
        // Arrange
        bool actionExecuted = false;
        var sut = new DisposeAction(() => actionExecuted = true);

        // Act
        sut.Dispose();

        // Assert
        Assert.IsTrue(actionExecuted);
    }

    [TestMethod]
    public void UsingStatement_ShouldExecuteActionOnDispose()
    {
        // Arrange
        bool actionExecuted = false;

        // Act
        using (new DisposeAction(() => actionExecuted = true))
        {
            Assert.IsFalse(actionExecuted);
        }

        // Assert
        Assert.IsTrue(actionExecuted);
    }
}