# MachineInfo 类文档

## 概述

`MachineInfo` 类用于获取和存储机器的硬件和系统信息。该类提供了跨平台的机器信息获取功能，支持 Windows、Linux 和 macOS 操作系统。

## 主要功能

### 静态信息
- **操作系统信息**：操作系统名称和版本
- **硬件标识**：UUID、GUID、序列号等唯一标识
- **硬件信息**：处理器型号、制造商、产品名称等
- **存储信息**：磁盘序列号、主板信息

### 动态信息
- **内存状态**：总内存、可用内存、空闲内存
- **CPU 使用率**：实时 CPU 占用率
- **网络速度**：上行和下行网络速度
- **其他指标**：温度、电池剩余（适用于支持的设备）

## 使用示例

### 基本使用

```csharp
using LuYao.Devices;

// 获取机器信息实例
var machineInfo = MachineInfo.Get();

// 访问基本信息
Console.WriteLine($"操作系统: {machineInfo.OSName}");
Console.WriteLine($"系统版本: {machineInfo.OSVersion}");
Console.WriteLine($"处理器: {machineInfo.Processor}");
Console.WriteLine($"制造商: {machineInfo.Vendor}");
Console.WriteLine($"产品名称: {machineInfo.Product}");
```

### 查看硬件标识

```csharp
var machineInfo = MachineInfo.Get();

// 硬件唯一标识（主板UUID）
Console.WriteLine($"UUID: {machineInfo.UUID}");

// 软件唯一标识（系统GUID）
Console.WriteLine($"GUID: {machineInfo.Guid}");

// 计算机序列号
Console.WriteLine($"序列号: {machineInfo.Serial}");

// 磁盘序列号
Console.WriteLine($"磁盘ID: {machineInfo.DiskID}");
```

### 监控系统性能

```csharp
using LuYao.Devices;
using LuYao.Globalization;

var machineInfo = MachineInfo.Get();

// 刷新动态信息
machineInfo.Refresh();

// 内存信息
Console.WriteLine($"总内存: {SizeHelper.ToReadable(machineInfo.Memory)}");
Console.WriteLine($"可用内存: {SizeHelper.ToReadable(machineInfo.AvailableMemory)}");
Console.WriteLine($"空闲内存: {SizeHelper.ToReadable(machineInfo.FreeMemory)}");

// CPU 占用率（需要两次刷新才能获得准确值）
System.Threading.Thread.Sleep(1000);
machineInfo.Refresh();
Console.WriteLine($"CPU 占用率: {machineInfo.CpuRate:P2}");
```

### 监控网络速度

```csharp
var machineInfo = MachineInfo.Get();

// 首次调用建立基线
machineInfo.RefreshSpeed();

// 等待一段时间
System.Threading.Thread.Sleep(1000);

// 再次调用获取速度
machineInfo.RefreshSpeed();

Console.WriteLine($"上行速度: {SizeHelper.ToReadable(machineInfo.UplinkSpeed)}/s");
Console.WriteLine($"下行速度: {SizeHelper.ToReadable(machineInfo.DownlinkSpeed)}/s");
```

### 使用扩展属性

```csharp
var machineInfo = MachineInfo.Get();

// 设置自定义属性
machineInfo["ApplicationVersion"] = "1.0.0";
machineInfo["DeploymentDate"] = DateTime.Now;

// 读取自定义属性
var version = machineInfo["ApplicationVersion"];
var deployDate = machineInfo["DeploymentDate"];

Console.WriteLine($"应用版本: {version}");
Console.WriteLine($"部署日期: {deployDate}");
```

### 完整示例：系统信息监控

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
        
        // 显示静态信息
        Console.WriteLine("=== 系统信息 ===");
        Console.WriteLine($"操作系统: {machineInfo.OSName}");
        Console.WriteLine($"系统版本: {machineInfo.OSVersion}");
        Console.WriteLine($"处理器: {machineInfo.Processor}");
        Console.WriteLine($"制造商: {machineInfo.Vendor}");
        Console.WriteLine($"产品: {machineInfo.Product}");
        Console.WriteLine($"UUID: {machineInfo.UUID}");
        Console.WriteLine($"GUID: {machineInfo.Guid}");
        Console.WriteLine();
        
        // 监控动态信息
        Console.WriteLine("=== 性能监控 (按 Ctrl+C 退出) ===");
        machineInfo.RefreshSpeed(); // 建立网络速度基线
        
        while (true)
        {
            machineInfo.Refresh();
            machineInfo.RefreshSpeed();
            
            Console.Clear();
            Console.WriteLine($"时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine();
            
            Console.WriteLine($"CPU 占用率: {machineInfo.CpuRate:P2}");
            Console.WriteLine($"总内存: {SizeHelper.ToReadable(machineInfo.Memory)}");
            Console.WriteLine($"可用内存: {SizeHelper.ToReadable(machineInfo.AvailableMemory)}");
            Console.WriteLine($"内存使用率: {(1.0 - (double)machineInfo.AvailableMemory / machineInfo.Memory):P2}");
            Console.WriteLine();
            
            Console.WriteLine($"上行速度: {SizeHelper.ToReadable(machineInfo.UplinkSpeed)}/s");
            Console.WriteLine($"下行速度: {SizeHelper.ToReadable(machineInfo.DownlinkSpeed)}/s");
            
            Thread.Sleep(1000);
        }
    }
}
```

## 属性说明

### 系统属性

| 属性 | 类型 | 说明 |
|------|------|------|
| `OSName` | `string?` | 操作系统名称（如 "Windows 11", "Ubuntu 22.04"） |
| `OSVersion` | `string?` | 操作系统版本号 |

### 硬件标识

| 属性 | 类型 | 说明 |
|------|------|------|
| `UUID` | `string?` | 硬件唯一标识，取主板编码，部分品牌存在重复 |
| `Guid` | `string?` | 软件唯一标识，操作系统重装后更新 |
| `Serial` | `string?` | 计算机序列号，适用于品牌机 |
| `DiskID` | `string?` | 磁盘序列号 |
| `Board` | `string?` | 主板序列号或家族信息 |

### 硬件信息

| 属性 | 类型 | 说明 |
|------|------|------|
| `Product` | `string?` | 产品名称（如 "ThinkPad X1 Carbon"） |
| `Vendor` | `string?` | 制造商（如 "Lenovo", "Dell"） |
| `Processor` | `string?` | 处理器型号（如 "Intel Core i7-10750H"） |

### 内存信息

| 属性 | 类型 | 说明 |
|------|------|------|
| `Memory` | `ulong` | 内存总量，单位字节 |
| `AvailableMemory` | `ulong` | 可用内存，单位字节 |
| `FreeMemory` | `ulong` | 空闲内存，单位字节（Linux 上可能不同于可用内存） |

### 性能指标

| 属性 | 类型 | 说明 |
|------|------|------|
| `CpuRate` | `double` | CPU 占用率，范围 0.0 ~ 1.0 |
| `UplinkSpeed` | `ulong` | 网络上行速度，字节每秒 |
| `DownlinkSpeed` | `ulong` | 网络下行速度，字节每秒 |

### 其他属性

| 属性 | 类型 | 说明 |
|------|------|------|
| `Temperature` | `double` | 温度，单位度（取决于硬件支持） |
| `Battery` | `double` | 电池剩余，范围 0.0 ~ 1.0（取决于设备类型） |

### 扩展属性

```csharp
// 索引器，用于存储和检索自定义属性
object? this[string key] { get; set; }
```

## 方法说明

### Get()

静态方法，获取并初始化一个新的 `MachineInfo` 实例。

```csharp
var machineInfo = MachineInfo.Get();
```

### Init()

初始化机器信息，加载静态硬件信息。该方法在 `Get()` 中自动调用。

```csharp
machineInfo.Init();
```

### Refresh()

刷新动态性能信息（CPU、内存等）。建议定期调用以获取最新性能数据。

```csharp
machineInfo.Refresh();
```

### RefreshSpeed()

刷新网络速度信息。需要至少调用两次才能计算速度。

```csharp
machineInfo.RefreshSpeed();
Thread.Sleep(1000);
machineInfo.RefreshSpeed(); // 现在可以获取准确的速度
```

## 平台支持

### Windows
- 支持 .NET Framework 4.5+ 和 .NET Core 3.0+
- 通过注册表和 WMIC 获取硬件信息
- 使用 Win32 API 获取内存和 CPU 信息

### Linux
- 支持 .NET Core 3.0+
- 读取 `/proc/cpuinfo`、`/proc/meminfo`、`/proc/stat` 等系统文件
- 读取 `/sys/class/dmi/id/` 下的 DMI 信息
- 读取 `/etc/machine-id` 获取系统 GUID

### macOS
- 支持 .NET Core 3.0+
- 使用 `system_profiler` 获取硬件信息
- 使用 `vm_stat` 获取内存信息
- 使用 `top` 获取 CPU 使用率

## 注意事项

1. **性能影响**：某些操作（如 Windows 上的 WMIC 查询）可能耗时较长，建议缓存 `MachineInfo` 实例。

2. **CPU 使用率**：需要两次 `Refresh()` 调用（中间间隔至少 1 秒）才能获得准确的 CPU 使用率。

3. **网络速度**：同样需要两次 `RefreshSpeed()` 调用才能计算速度。

4. **权限要求**：
   - Windows：某些注册表项可能需要管理员权限
   - Linux：读取某些系统文件可能需要 root 权限
   - macOS：某些系统命令可能需要适当权限

5. **唯一标识**：
   - `UUID` 和 `Guid` 可能在某些环境下无法获取或不唯一（如虚拟机、Ghost 系统）
   - 在无法获取时会自动生成随机 GUID

6. **跨平台兼容性**：并非所有属性在所有平台上都可用。使用前应检查属性是否为 `null` 或空字符串。

## 最佳实践

1. **单例模式**：建议使用单例模式缓存 `MachineInfo` 实例，避免重复初始化。

```csharp
public class SystemMonitor
{
    private static readonly Lazy<MachineInfo> _instance = 
        new Lazy<MachineInfo>(() => MachineInfo.Get());
    
    public static MachineInfo Instance => _instance.Value;
}
```

2. **定时刷新**：使用定时器定期刷新性能数据。

```csharp
var timer = new System.Timers.Timer(1000); // 1 秒
timer.Elapsed += (sender, e) =>
{
    machineInfo.Refresh();
    machineInfo.RefreshSpeed();
    UpdateUI(machineInfo);
};
timer.Start();
```

3. **异常处理**：某些操作可能失败，应适当处理异常。

```csharp
try
{
    var machineInfo = MachineInfo.Get();
    machineInfo.Refresh();
}
catch (Exception ex)
{
    Console.WriteLine($"获取机器信息失败: {ex.Message}");
}
```

## 参考

本实现参考了 [NewLifeX](https://github.com/NewLifeX/X) 项目的 MachineInfo 实现。

## 相关文档

- [SizeHelper 文档](SizeHelper.md) - 用于格式化字节大小
- [UnitConverter 文档](UnitConverter.md) - 单位转换工具
