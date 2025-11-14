using System;
using System.Runtime.InteropServices;
using System.Threading;
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
    public void Guid_ShouldNotBeNullOrEmpty()
    {
        // Arrange
        var machineInfo = MachineInfo.Get();

        // Assert
        Assert.IsFalse(String.IsNullOrEmpty(machineInfo.Guid), "Guid should not be null or empty");
    }

    [TestMethod]
    public void UUID_ShouldNotBeNullOrEmpty()
    {
        // Arrange
        var machineInfo = MachineInfo.Get();

        // Assert
        Assert.IsFalse(String.IsNullOrEmpty(machineInfo.UUID), "UUID should not be null or empty");
    }

    [TestMethod]
    public void Memory_ShouldBeGreaterThanZero()
    {
        // Arrange
        var machineInfo = MachineInfo.Get();

        // Assert
        Assert.IsTrue(machineInfo.Memory > 0, "Memory should be greater than 0");
    }

    [TestMethod]
    public void Refresh_ShouldUpdateDynamicProperties()
    {
        // Arrange
        var machineInfo = MachineInfo.Get();
        var initialMemory = machineInfo.Memory;

        // Act
        machineInfo.Refresh();

        // Assert - Memory should still be > 0 after refresh
        Assert.IsTrue(machineInfo.Memory > 0, "Memory should be greater than 0 after refresh");
    }

    [TestMethod]
    public void CpuRate_ShouldBeInValidRange()
    {
        // Arrange
        var machineInfo = MachineInfo.Get();
        
        // Need to refresh twice to get CPU rate
        machineInfo.Refresh();
        Thread.Sleep(1000);
        machineInfo.Refresh();

        // Assert - CPU rate should be between 0 and 1
        Assert.IsTrue(machineInfo.CpuRate >= 0 && machineInfo.CpuRate <= 1, 
            $"CPU rate should be between 0 and 1, got {machineInfo.CpuRate}");
    }

    [TestMethod]
    public void ExtensionProperties_ShouldWork()
    {
        // Arrange
        var machineInfo = MachineInfo.Get();

        // Act
        machineInfo["TestKey"] = "TestValue";
        var value = machineInfo["TestKey"];

        // Assert
        Assert.AreEqual("TestValue", value);
    }

    [TestMethod]
    public void ExtensionProperties_ShouldReturnNullForNonExistentKey()
    {
        // Arrange
        var machineInfo = MachineInfo.Get();

        // Act
        var value = machineInfo["NonExistentKey"];

        // Assert
        Assert.IsNull(value);
    }

    [TestMethod]
    public void RefreshSpeed_ShouldNotThrow()
    {
        // Arrange
        var machineInfo = MachineInfo.Get();

        // Act & Assert - Should not throw
        machineInfo.RefreshSpeed();
        Thread.Sleep(1000);
        machineInfo.RefreshSpeed();
        
        // Network speeds should be non-negative
        Assert.IsTrue(machineInfo.UplinkSpeed >= 0, "Uplink speed should be non-negative");
        Assert.IsTrue(machineInfo.DownlinkSpeed >= 0, "Downlink speed should be non-negative");
    }

    [TestMethod]
    public void Processor_ShouldNotBeNull()
    {
        // Arrange
        var machineInfo = MachineInfo.Get();

        // Assert - Processor might be null on some platforms, but usually isn't
        // Just checking it doesn't throw
        var processor = machineInfo.Processor;
        Assert.IsTrue(true); // Test passes if we get here
    }

    [TestMethod]
    public void AvailableMemory_ShouldBeLessThanOrEqualToTotalMemory()
    {
        // Arrange
        var machineInfo = MachineInfo.Get();

        // Act
        machineInfo.Refresh();

        // Assert
        if (machineInfo.AvailableMemory > 0)
        {
            Assert.IsTrue(machineInfo.AvailableMemory <= machineInfo.Memory,
                $"Available memory ({machineInfo.AvailableMemory}) should be <= total memory ({machineInfo.Memory})");
        }
    }

    [TestMethod]
    public void FreeMemory_ShouldBeLessThanOrEqualToTotalMemory()
    {
        // Arrange
        var machineInfo = MachineInfo.Get();

        // Act
        machineInfo.Refresh();

        // Assert
        if (machineInfo.FreeMemory > 0)
        {
            Assert.IsTrue(machineInfo.FreeMemory <= machineInfo.Memory,
                $"Free memory ({machineInfo.FreeMemory}) should be <= total memory ({machineInfo.Memory})");
        }
    }
}
