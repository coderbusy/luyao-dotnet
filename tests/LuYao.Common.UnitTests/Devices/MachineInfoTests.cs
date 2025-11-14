using System;
using LuYao.Devices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LuYao.Common.UnitTests.Devices;

/// <summary>
/// MachineInfo 类的单元测试
/// </summary>
[TestClass]
public class MachineInfoTests
{
    [TestMethod]
    public void Get_ShouldReturnMachineInfo()
    {
        // Act
        var machineInfo = MachineInfo.Get();

        // Assert
        Assert.IsNotNull(machineInfo);
    }

    [TestMethod]
    public void OSName_ShouldNotBeNullOrEmpty()
    {
        // Arrange
        var machineInfo = MachineInfo.Get();

        // Assert
        Assert.IsFalse(String.IsNullOrEmpty(machineInfo.OSName), "OSName should not be null or empty");
    }

    [TestMethod]
    public void OSVersion_ShouldNotBeNullOrEmpty()
    {
        // Arrange
        var machineInfo = MachineInfo.Get();

        // Assert
        Assert.IsFalse(String.IsNullOrEmpty(machineInfo.OSVersion), "OSVersion should not be null or empty");
    }

    [TestMethod]
    public void Processor_CanBeRead()
    {
        // Arrange
        var machineInfo = MachineInfo.Get();

        // Assert - Processor might be null on some platforms, just verify it doesn't throw
        var processor = machineInfo.Processor;
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void Reload_ShouldNotThrow()
    {
        // Arrange
        var machineInfo = MachineInfo.Get();
        var originalOSName = machineInfo.OSName;

        // Act
        machineInfo.Reload();

        // Assert - OSName should still be set after reload
        Assert.IsFalse(String.IsNullOrEmpty(machineInfo.OSName), "OSName should not be null or empty after reload");
    }

    [TestMethod]
    public void AllBasicProperties_CanBeAccessed()
    {
        // Arrange
        var machineInfo = MachineInfo.Get();

        // Act & Assert - All properties should be accessible
        var osName = machineInfo.OSName;
        var osVersion = machineInfo.OSVersion;
        var product = machineInfo.Product;
        var vendor = machineInfo.Vendor;
        var processor = machineInfo.Processor;
        var serial = machineInfo.Serial;
        var board = machineInfo.Board;
        var diskId = machineInfo.DiskID;

        // Test passes if we get here without exceptions
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void Product_MayBeNullOnSomePlatforms()
    {
        // Arrange
        var machineInfo = MachineInfo.Get();

        // Act
        var product = machineInfo.Product;

        // Assert - Product can be null on some platforms (e.g., virtual machines)
        // Just verify it doesn't throw
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void Serial_MayBeNullOnSomePlatforms()
    {
        // Arrange
        var machineInfo = MachineInfo.Get();

        // Act
        var serial = machineInfo.Serial;

        // Assert - Serial can be null on some platforms
        // Just verify it doesn't throw
        Assert.IsTrue(true);
    }
}
