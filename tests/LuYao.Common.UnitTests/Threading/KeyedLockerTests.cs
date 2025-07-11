using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LuYao.Threading;

/// <summary>
/// 测试 KeyedLocker<T> 类型的功能。
/// </summary>
[TestClass]
public class KeyedLockerTests
{
    /// <summary>
    /// 测试使用有效键获取锁对象时，锁对象应正确返回。
    /// </summary>
    [TestMethod]
    public void GetLock_ValidKey_ReturnsLockObject()
    {
        // Arrange
        string key = "TestKey";

        // Act
        var lockObject = KeyedLocker<object>.GetLock(key);

        // Assert
        Assert.IsNotNull(lockObject, "锁对象不应为 null。");
    }

    /// <summary>
    /// 测试使用 null 键获取锁对象时，应抛出 ArgumentNullException。
    /// </summary>
    [TestMethod]
    public void GetLock_NullKey_ThrowsArgumentNullException()
    {
        // Arrange
        string? key = null;

        // Act & Assert
        Assert.ThrowsException<ArgumentNullException>(() => KeyedLocker<object>.GetLock(key), "应抛出 ArgumentNullException。");
    }

    /// <summary>
    /// 测试使用相同的键获取锁对象时，应返回相同的锁对象。
    /// </summary>
    [TestMethod]
    public void GetLock_SameKey_ReturnsSameLockObject()
    {
        // Arrange
        string key = "TestKey";

        // Act
        var lockObject1 = KeyedLocker<object>.GetLock(key);
        var lockObject2 = KeyedLocker<object>.GetLock(key);

        // Assert
        Assert.AreSame(lockObject1, lockObject2, "相同的键应返回相同的锁对象。");
    }
}