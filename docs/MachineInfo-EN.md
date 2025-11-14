# MachineInfo Class Documentation

## Overview

The `MachineInfo` class is used to retrieve and store basic machine hardware and system information. It provides cross-platform functionality for gathering machine information, supporting Windows, Linux, and macOS operating systems.

## Key Features

### System Information
- **Operating System Info**: OS name and version
- **Hardware Info**: Processor model, manufacturer, product name
- **Identification Info**: Computer serial number, motherboard info, disk serial number

## Usage Examples

### Basic Usage

```csharp
using LuYao.Devices;

// Get machine information instance
var machineInfo = MachineInfo.Get();

// Access basic information
Console.WriteLine($"OS: {machineInfo.OSName}");
Console.WriteLine($"Version: {machineInfo.OSVersion}");
Console.WriteLine($"Processor: {machineInfo.Processor}");
Console.WriteLine($"Vendor: {machineInfo.Vendor}");
Console.WriteLine($"Product: {machineInfo.Product}");
Console.WriteLine($"Serial: {machineInfo.Serial}");
Console.WriteLine($"Board: {machineInfo.Board}");
Console.WriteLine($"Disk ID: {machineInfo.DiskID}");
```

### Reload Information

```csharp
var machineInfo = MachineInfo.Get();

// Reload machine information
machineInfo.Reload();

// Access updated information
Console.WriteLine($"OS: {machineInfo.OSName}");
```

### Complete Example: Display System Information

```csharp
using System;
using LuYao.Devices;

class Program
{
    static void Main()
    {
        var machineInfo = MachineInfo.Get();
        
        Console.WriteLine("=== System Information ===");
        Console.WriteLine($"OS: {machineInfo.OSName}");
        Console.WriteLine($"Version: {machineInfo.OSVersion}");
        Console.WriteLine($"Processor: {machineInfo.Processor}");
        Console.WriteLine($"Vendor: {machineInfo.Vendor}");
        Console.WriteLine($"Product: {machineInfo.Product}");
        Console.WriteLine($"Serial: {machineInfo.Serial}");
        Console.WriteLine($"Board: {machineInfo.Board}");
        Console.WriteLine($"Disk ID: {machineInfo.DiskID}");
    }
}
```

## Property Reference

| Property | Type | Description |
|----------|------|-------------|
| `OSName` | `string?` | Operating system name (e.g., "Windows 11", "Ubuntu 22.04") |
| `OSVersion` | `string?` | Operating system version number |
| `Product` | `string?` | Product name (e.g., "ThinkPad X1 Carbon") |
| `Vendor` | `string?` | Manufacturer (e.g., "Lenovo", "Dell", "Apple") |
| `Processor` | `string?` | Processor model (e.g., "Intel Core i7-10750H") |
| `Serial` | `string?` | Computer serial number, suitable for branded machines |
| `Board` | `string?` | Motherboard serial number or family information |
| `DiskID` | `string?` | Disk serial number |

## Method Reference

### Get()

Static method to get and initialize a new `MachineInfo` instance.

```csharp
var machineInfo = MachineInfo.Get();
```

### Reload()

Reload machine information.

```csharp
machineInfo.Reload();
```

## Platform Support

### Windows
- Supports .NET Framework 4.5+ and .NET Core 3.0+
- Gets hardware info via registry and WMIC
- Retrieves: OSName, OSVersion, Product, Vendor, Processor, Serial, Board, DiskID

### Linux
- Supports .NET Core 3.0+
- Reads `/proc/cpuinfo` for processor information
- Reads DMI information from `/sys/class/dmi/id/`
- Reads disk information from `/sys/block/`
- Retrieves: OSName, OSVersion, Product, Vendor, Processor, Serial, Board, DiskID

### macOS
- Supports .NET Core 3.0+
- Uses `system_profiler` for hardware information
- Retrieves: OSName, OSVersion, Product, Processor, Serial
- Vendor defaults to "Apple"

## Important Notes

1. **Performance Impact**: Some operations (like WMIC queries on Windows) may be time-consuming. Consider caching the `MachineInfo` instance.

2. **Permission Requirements**:
   - Windows: Some registry keys may require administrator privileges
   - Linux: Reading some system files may require root permissions
   - macOS: Some system commands may require appropriate permissions

3. **Cross-platform Compatibility**: Not all properties are available on all platforms. Check for `null` or empty strings before use. For example:
   - `Serial`, `Board`, `DiskID` may be empty on some VMs or specific hardware
   - `Board` and `DiskID` may not be available on macOS

## Best Practices

1. **Singleton Pattern**: Use singleton pattern to cache `MachineInfo` instance and avoid repeated initialization.

```csharp
public class SystemInfo
{
    private static readonly Lazy<MachineInfo> _instance = 
        new Lazy<MachineInfo>(() => MachineInfo.Get());
    
    public static MachineInfo Instance => _instance.Value;
}
```

2. **Exception Handling**: Some operations may fail, handle exceptions appropriately.

```csharp
try
{
    var machineInfo = MachineInfo.Get();
    Console.WriteLine($"OS: {machineInfo.OSName}");
}
catch (Exception ex)
{
    Console.WriteLine($"Failed to get machine info: {ex.Message}");
}
```

3. **Null Checks**: Some properties may be null, check before use.

```csharp
var machineInfo = MachineInfo.Get();

if (!string.IsNullOrEmpty(machineInfo.Serial))
{
    Console.WriteLine($"Serial: {machineInfo.Serial}");
}
else
{
    Console.WriteLine("Serial number not available");
}
```

## Reference

This implementation is based on the MachineInfo implementation from the [NewLifeX](https://github.com/NewLifeX/X) project.
