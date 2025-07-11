using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LuYao.Threading.Tasks;

[TestClass]
public class TaskExtensionsTests
{
    [TestMethod]
    public void IsCompletedSuccessfully_WhenTaskCompleted_ShouldReturnTrue()
    {
        // Arrange
        var task = Task.FromResult(true);

        // Act
        var result = task.IsCompletedSuccessfully();

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task IsCompletedSuccessfully_WhenTaskFaulted_ShouldReturnFalse()
    {
        // Arrange
        var task = Task.Run(() => throw new Exception("Test Exception"));

        // Act & Assert
        await Assert.ThrowsExceptionAsync<Exception>(() => task);
        Assert.IsFalse(task.IsCompletedSuccessfully());
    }

    [TestMethod]
    public void IsCompletedSuccessfully_WhenTaskNotCompleted_ShouldReturnFalse()
    {
        // Arrange
        var tcs = new TaskCompletionSource<bool>();
        var task = tcs.Task;

        // Act
        var result = task.IsCompletedSuccessfully();

        // Assert
        Assert.IsFalse(result);
    }
}