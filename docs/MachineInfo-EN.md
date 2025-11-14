# MachineInfo Class Documentation

## Overview

The `MachineInfo` class is used to retrieve and store machine hardware and system information. It provides cross-platform functionality for gathering machine information, supporting Windows, Linux, and macOS operating systems.

## Key Features

### Static Information
- **Operating System Info**: OS name and version
- **Hardware Identifiers**: UUID, GUID, serial numbers, and other unique identifiers
- **Hardware Info**: Processor model, manufacturer, product name, etc.
- **Storage Info**: Disk serial number, motherboard information

### Dynamic Information
- **Memory Status**: Total memory, available memory, free memory
- **CPU Usage**: Real-time CPU utilization
- **Network Speed**: Uplink and downlink network speeds
- **Other Metrics**: Temperature, battery level (for supported devices)

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
```

### View Hardware Identifiers

```csharp
var machineInfo = MachineInfo.Get();

// Hardware unique identifier (motherboard UUID)
Console.WriteLine($"UUID: {machineInfo.UUID}");

// Software unique identifier (system GUID)
Console.WriteLine($"GUID: {machineInfo.Guid}");

// Computer serial number
Console.WriteLine($"Serial: {machineInfo.Serial}");

// Disk serial number
Console.WriteLine($"Disk ID: {machineInfo.DiskID}");
```

### Monitor System Performance

```csharp
using LuYao.Devices;
using LuYao.Globalization;

var machineInfo = MachineInfo.Get();

// Refresh dynamic information
machineInfo.Refresh();

// Memory information
Console.WriteLine($"Total Memory: {SizeHelper.ToReadable(machineInfo.Memory)}");
Console.WriteLine($"Available: {SizeHelper.ToReadable(machineInfo.AvailableMemory)}");
Console.WriteLine($"Free: {SizeHelper.ToReadable(machineInfo.FreeMemory)}");

// CPU usage (requires two refreshes for accurate value)
System.Threading.Thread.Sleep(1000);
machineInfo.Refresh();
Console.WriteLine($"CPU Usage: {machineInfo.CpuRate:P2}");
```

### Monitor Network Speed

```csharp
var machineInfo = MachineInfo.Get();

// First call establishes baseline
machineInfo.RefreshSpeed();

// Wait for a period
System.Threading.Thread.Sleep(1000);

// Call again to get speed
machineInfo.RefreshSpeed();

Console.WriteLine($"Upload: {SizeHelper.ToReadable(machineInfo.UplinkSpeed)}/s");
Console.WriteLine($"Download: {SizeHelper.ToReadable(machineInfo.DownlinkSpeed)}/s");
```

### Using Extension Properties

```csharp
var machineInfo = MachineInfo.Get();

// Set custom properties
machineInfo["ApplicationVersion"] = "1.0.0";
machineInfo["DeploymentDate"] = DateTime.Now;

// Read custom properties
var version = machineInfo["ApplicationVersion"];
var deployDate = machineInfo["DeploymentDate"];

Console.WriteLine($"App Version: {version}");
Console.WriteLine($"Deploy Date: {deployDate}");
```

### Complete Example: System Monitoring

```csharp
using System;
using System.Threading;
using LuYao.Devices;
using LuYao.Globalization;

class Program
{
    static void Main()
    {
        var machineInfo = MachineInfo.Get();
        
        // Display static information
        Console.WriteLine("=== System Information ===");
        Console.WriteLine($"OS: {machineInfo.OSName}");
        Console.WriteLine($"Version: {machineInfo.OSVersion}");
        Console.WriteLine($"Processor: {machineInfo.Processor}");
        Console.WriteLine($"Vendor: {machineInfo.Vendor}");
        Console.WriteLine($"Product: {machineInfo.Product}");
        Console.WriteLine($"UUID: {machineInfo.UUID}");
        Console.WriteLine($"GUID: {machineInfo.Guid}");
        Console.WriteLine();
        
        // Monitor dynamic information
        Console.WriteLine("=== Performance Monitor (Press Ctrl+C to exit) ===");
        machineInfo.RefreshSpeed(); // Establish network speed baseline
        
        while (true)
        {
            machineInfo.Refresh();
            machineInfo.RefreshSpeed();
            
            Console.Clear();
            Console.WriteLine($"Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine();
            
            Console.WriteLine($"CPU Usage: {machineInfo.CpuRate:P2}");
            Console.WriteLine($"Total Memory: {SizeHelper.ToReadable(machineInfo.Memory)}");
            Console.WriteLine($"Available: {SizeHelper.ToReadable(machineInfo.AvailableMemory)}");
            Console.WriteLine($"Usage: {(1.0 - (double)machineInfo.AvailableMemory / machineInfo.Memory):P2}");
            Console.WriteLine();
            
            Console.WriteLine($"Upload: {SizeHelper.ToReadable(machineInfo.UplinkSpeed)}/s");
            Console.WriteLine($"Download: {SizeHelper.ToReadable(machineInfo.DownlinkSpeed)}/s");
            
            Thread.Sleep(1000);
        }
    }
}
```

## Property Reference

### System Properties

| Property | Type | Description |
|----------|------|-------------|
| `OSName` | `string?` | Operating system name (e.g., "Windows 11", "Ubuntu 22.04") |
| `OSVersion` | `string?` | Operating system version number |

### Hardware Identifiers

| Property | Type | Description |
|----------|------|-------------|
| `UUID` | `string?` | Hardware unique identifier, motherboard UUID (may duplicate on some brands) |
| `Guid` | `string?` | Software unique identifier, updates after OS reinstall |
| `Serial` | `string?` | Computer serial number, suitable for branded machines |
| `DiskID` | `string?` | Disk serial number |
| `Board` | `string?` | Motherboard serial number or family information |

### Hardware Information

| Property | Type | Description |
|----------|------|-------------|
| `Product` | `string?` | Product name (e.g., "ThinkPad X1 Carbon") |
| `Vendor` | `string?` | Manufacturer (e.g., "Lenovo", "Dell") |
| `Processor` | `string?` | Processor model (e.g., "Intel Core i7-10750H") |

### Memory Information

| Property | Type | Description |
|----------|------|-------------|
| `Memory` | `ulong` | Total memory in bytes |
| `AvailableMemory` | `ulong` | Available memory in bytes |
| `FreeMemory` | `ulong` | Free memory in bytes (may differ from available on Linux) |

### Performance Metrics

| Property | Type | Description |
|----------|------|-------------|
| `CpuRate` | `double` | CPU usage rate, range 0.0 ~ 1.0 |
| `UplinkSpeed` | `ulong` | Network upload speed in bytes per second |
| `DownlinkSpeed` | `ulong` | Network download speed in bytes per second |

### Other Properties

| Property | Type | Description |
|----------|------|-------------|
| `Temperature` | `double` | Temperature in degrees (hardware dependent) |
| `Battery` | `double` | Battery remaining, range 0.0 ~ 1.0 (device dependent) |

### Extension Properties

```csharp
// Indexer for storing and retrieving custom properties
object? this[string key] { get; set; }
```

## Method Reference

### Get()

Static method to get and initialize a new `MachineInfo` instance.

```csharp
var machineInfo = MachineInfo.Get();
```

### Init()

Initialize machine information and load static hardware information. This method is automatically called in `Get()`.

```csharp
machineInfo.Init();
```

### Refresh()

Refresh dynamic performance information (CPU, memory, etc.). Recommended to call periodically for latest performance data.

```csharp
machineInfo.Refresh();
```

### RefreshSpeed()

Refresh network speed information. Requires at least two calls to calculate speed.

```csharp
machineInfo.RefreshSpeed();
Thread.Sleep(1000);
machineInfo.RefreshSpeed(); // Now can get accurate speed
```

## Platform Support

### Windows
- Supports .NET Framework 4.5+ and .NET Core 3.0+
- Gets hardware info via registry and WMIC
- Uses Win32 API for memory and CPU information

### Linux
- Supports .NET Core 3.0+
- Reads system files like `/proc/cpuinfo`, `/proc/meminfo`, `/proc/stat`
- Reads DMI information from `/sys/class/dmi/id/`
- Reads `/etc/machine-id` for system GUID

### macOS
- Supports .NET Core 3.0+
- Uses `system_profiler` for hardware information
- Uses `vm_stat` for memory information
- Uses `top` for CPU usage

## Important Notes

1. **Performance Impact**: Some operations (like WMIC queries on Windows) may be time-consuming. Consider caching the `MachineInfo` instance.

2. **CPU Usage**: Requires two `Refresh()` calls (with at least 1 second interval) for accurate CPU usage.

3. **Network Speed**: Similarly requires two `RefreshSpeed()` calls to calculate speed.

4. **Permission Requirements**:
   - Windows: Some registry keys may require administrator privileges
   - Linux: Reading some system files may require root permissions
   - macOS: Some system commands may require appropriate permissions

5. **Unique Identifiers**:
   - `UUID` and `Guid` may not be obtainable or unique in some environments (VMs, Ghost systems)
   - Random GUIDs are automatically generated when unavailable

6. **Cross-platform Compatibility**: Not all properties are available on all platforms. Check for `null` or empty strings before use.

## Best Practices

1. **Singleton Pattern**: Use singleton pattern to cache `MachineInfo` instance and avoid repeated initialization.

```csharp
public class SystemMonitor
{
    private static readonly Lazy<MachineInfo> _instance = 
        new Lazy<MachineInfo>(() => MachineInfo.Get());
    
    public static MachineInfo Instance => _instance.Value;
}
```

2. **Periodic Refresh**: Use a timer to periodically refresh performance data.

```csharp
var timer = new System.Timers.Timer(1000); // 1 second
timer.Elapsed += (sender, e) =>
{
    machineInfo.Refresh();
    machineInfo.RefreshSpeed();
    UpdateUI(machineInfo);
};
timer.Start();
```

3. **Exception Handling**: Some operations may fail, handle exceptions appropriately.

```csharp
try
{
    var machineInfo = MachineInfo.Get();
    machineInfo.Refresh();
}
catch (Exception ex)
{
    Console.WriteLine($"Failed to get machine info: {ex.Message}");
}
```

## Reference

This implementation is based on the MachineInfo implementation from the [NewLifeX](https://github.com/NewLifeX/X) project.

## Related Documentation

- [SizeHelper Documentation](SizeHelper-EN.md) - For formatting byte sizes
- [UnitConverter Documentation](UnitConverter-EN.md) - Unit conversion tool
