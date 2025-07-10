# LuYao .NET 通用工具库

[![MIT License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)
[![NuGet](https://img.shields.io/nuget/v/LuYao.Common.svg)](https://www.nuget.org/packages/LuYao.Common)

## 简介

`luyao-dotnet` 是一个高质量、易用的 .NET 工具库，提供丰富的通用工具类和扩展方法，涵盖缓存、哈希、数组、线程锁、临时文件、字符串压缩等常用开发场景，助力 .NET 开发者提升开发效率与代码质量。

## 主要功能

- **缓存对象**：`CachedObject<T>` 支持缓存值及多种过期策略。
- **数组工具**：`Arrays` 类便捷获取指定类型的空数组。
- **哈希算法**：`HashAgent`/`HashAgents` 支持 CRC32、CRC64、MD5、SHA1 等常用哈希算法。
- **临时文件管理**：`AutoCleanTempFile` 对象自动清理临时文件，避免资源泄露。
- **线程锁机制**：`KeyedLocker<T>` 实现基于 Key 的高并发锁。
- **字符串压缩/解压**：`LzString`、`GZipString` 支持多种字符串压缩与解压缩算法。

## 安装

推荐通过 NuGet 安装：

```bash
dotnet add package LuYao.Common
```
或访问 [NuGet 官网](https://www.nuget.org/packages/LuYao.Common) 获取最新信息。

也可通过源码集成到你的 .NET 项目中：

```bash
git clone https://github.com/coderbusy/luyao-dotnet.git
```

## 示例

```csharp
// 缓存对象的使用
var cache = new CachedObject<string>("Hello", TimeSpan.FromMinutes(5));
Console.WriteLine(cache.Value);

// 获取空数组
var emptyInts = Arrays.Empty<int>();

// 计算文件的 MD5
var md5 = HashAgents.MD5.HashFile("test.txt");

// 创建自动清理的临时文件
using var tempFile = new AutoCleanTempFile();
File.WriteAllText(tempFile.FileName, "临时数据");

// 线程锁机制
var lockObj = KeyedLocker<string>.GetLock("my-key");
lock (lockObj)
{
    // 临界区代码
}
```

## 授权协议

本项目基于 [MIT License](LICENSE) 开源，欢迎自由使用与贡献。

---

## 贡献

欢迎提交 Issue 和 PR。

---

## 联系

GitHub: [coderbusy/luyao-dotnet](https://github.com/coderbusy/luyao-dotnet)  
NuGet: [LuYao.Common](https://www.nuget.org/packages/LuYao.Common)