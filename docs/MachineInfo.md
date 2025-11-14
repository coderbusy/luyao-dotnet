# MachineInfo 类文档

## 概述

`MachineInfo` 类用于获取和存储机器的基本硬件和系统信息。该类提供了跨平台的机器信息获取功能，支持 Windows、Linux 和 macOS 操作系统。

## 主要功能

### 系统信息
- **操作系统信息**：操作系统名称和版本
- **硬件信息**：处理器型号、制造商、产品名称
- **标识信息**：计算机序列号、主板信息、磁盘序列号

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
Console.WriteLine($"序列号: {machineInfo.Serial}");
Console.WriteLine($"主板: {machineInfo.Board}");
Console.WriteLine($"磁盘ID: {machineInfo.DiskID}");
```

### 重新加载信息

```csharp
var machineInfo = MachineInfo.Get();

// 重新加载机器信息
machineInfo.Reload();

// 访问更新后的信息
Console.WriteLine($"操作系统: {machineInfo.OSName}");
```

### 完整示例：显示系统信息

```csharp
using System;
using LuYao.Devices;

class Program
{
    static void Main()
    {
        var machineInfo = MachineInfo.Get();
        
        Console.WriteLine("=== 系统信息 ===");
        Console.WriteLine($"操作系统: {machineInfo.OSName}");
        Console.WriteLine($"系统版本: {machineInfo.OSVersion}");
        Console.WriteLine($"处理器: {machineInfo.Processor}");
        Console.WriteLine($"制造商: {machineInfo.Vendor}");
        Console.WriteLine($"产品: {machineInfo.Product}");
        Console.WriteLine($"序列号: {machineInfo.Serial}");
        Console.WriteLine($"主板: {machineInfo.Board}");
        Console.WriteLine($"磁盘ID: {machineInfo.DiskID}");
    }
}
```

## 属性说明

| 属性 | 类型 | 说明 |
|------|------|------|
| `OSName` | `string?` | 操作系统名称（如 "Windows 11", "Ubuntu 22.04"） |
| `OSVersion` | `string?` | 操作系统版本号 |
| `Product` | `string?` | 产品名称（如 "ThinkPad X1 Carbon"） |
| `Vendor` | `string?` | 制造商（如 "Lenovo", "Dell", "Apple"） |
| `Processor` | `string?` | 处理器型号（如 "Intel Core i7-10750H"） |
| `Serial` | `string?` | 计算机序列号，适用于品牌机，跟笔记本标签显示一致 |
| `Board` | `string?` | 主板序列号或家族信息 |
| `DiskID` | `string?` | 磁盘序列号 |

## 方法说明

### Get()

静态方法，获取并初始化一个新的 `MachineInfo` 实例。

```csharp
var machineInfo = MachineInfo.Get();
```

### Reload()

重新加载机器信息。

```csharp
machineInfo.Reload();
```

## 平台支持

### Windows
- 支持 .NET Framework 4.5+ 和 .NET Core 3.0+
- 通过注册表和 WMIC 获取硬件信息
- 获取：OSName, OSVersion, Product, Vendor, Processor, Serial, Board, DiskID

### Linux
- 支持 .NET Core 3.0+
- 读取 `/proc/cpuinfo` 获取处理器信息
- 读取 `/sys/class/dmi/id/` 下的 DMI 信息
- 读取 `/sys/block/` 获取磁盘信息
- 获取：OSName, OSVersion, Product, Vendor, Processor, Serial, Board, DiskID

### macOS
- 支持 .NET Core 3.0+
- 使用 `system_profiler` 获取硬件信息
- 获取：OSName, OSVersion, Product, Processor, Serial
- Vendor 默认为 "Apple"

## 注意事项

1. **性能影响**：某些操作（如 Windows 上的 WMIC 查询）可能耗时较长，建议缓存 `MachineInfo` 实例。

2. **权限要求**：
   - Windows：某些注册表项可能需要管理员权限
   - Linux：读取某些系统文件可能需要 root 权限
   - macOS：某些系统命令可能需要适当权限

3. **跨平台兼容性**：并非所有属性在所有平台上都可用。使用前应检查属性是否为 `null` 或空字符串。例如：
   - `Serial`、`Board`、`DiskID` 在某些虚拟机或特定硬件上可能为空
   - macOS 上的 `Board` 和 `DiskID` 可能无法获取

## 最佳实践

1. **单例模式**：建议使用单例模式缓存 `MachineInfo` 实例，避免重复初始化。

```csharp
public class SystemInfo
{
    private static readonly Lazy<MachineInfo> _instance = 
        new Lazy<MachineInfo>(() => MachineInfo.Get());
    
    public static MachineInfo Instance => _instance.Value;
}
```

2. **异常处理**：某些操作可能失败，应适当处理异常。

```csharp
try
{
    var machineInfo = MachineInfo.Get();
    Console.WriteLine($"系统: {machineInfo.OSName}");
}
catch (Exception ex)
{
    Console.WriteLine($"获取机器信息失败: {ex.Message}");
}
```

3. **空值检查**：某些属性可能为空，使用前应检查。

```csharp
var machineInfo = MachineInfo.Get();

if (!string.IsNullOrEmpty(machineInfo.Serial))
{
    Console.WriteLine($"序列号: {machineInfo.Serial}");
}
else
{
    Console.WriteLine("无法获取序列号");
}
```

## 参考

本实现参考了 [NewLifeX](https://github.com/NewLifeX/X) 项目的 MachineInfo 实现。

## 相关文档

- [SizeHelper 文档](SizeHelper.md) - 用于格式化字节大小
- [UnitConverter 文档](UnitConverter.md) - 单位转换工具
