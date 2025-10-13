# LuYao .NET 通用工具库

[![MIT License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)
[![NuGet](https://img.shields.io/nuget/v/LuYao.Common.svg)](https://www.nuget.org/packages/LuYao.Common)
[![Build](https://img.shields.io/github/actions/workflow/status/coderbusy/luyao-dotnet/build.yml?branch=main)](https://github.com/coderbusy/luyao-dotnet/actions)

[中文](#chinese) | [English](#english)

---

<a name="chinese"></a>
## 中文

### 简介

LuYao 是一个面向企业级应用的 .NET 实用工具库集合，提供丰富的通用工具、扩展方法和实用程序，帮助 .NET 开发人员更高效地编写业务代码。

该项目包含三个主要包：
- **LuYao.Common**：核心工具库，包含缓存、哈希、I/O 助手、线程工具等
- **LuYao.Text.Json**：基于 Newtonsoft.Json 的 JSON 处理工具
- **LuYao.Text.Json.Jint**：使用 Jint 引擎的 JavaScript 驱动的 JSON 转换

### 支持的框架

- .NET Framework 4.5+
- .NET Framework 4.6.1+
- .NET Standard 2.0
- .NET Standard 2.1
- .NET 6.0
- .NET 8.0

### 安装

通过 NuGet 包管理器安装：

```bash
# 核心工具库
dotnet add package LuYao.Common

# JSON 工具
dotnet add package LuYao.Text.Json

# 基于 Jint 的 JSON 转换
dotnet add package LuYao.Text.Json.Jint
```

或克隆源代码：

```bash
git clone https://github.com/coderbusy/luyao-dotnet.git
```

### 主要功能

#### LuYao.Common

##### 1. 缓存机制
```csharp
using LuYao;

// 创建带自动过期的缓存对象
var cache = new CachedObject<string>("Hello World", TimeSpan.FromMinutes(5));
Console.WriteLine(cache.Value); // "Hello World"
Console.WriteLine(cache.IsExpired); // false
```

##### 2. 数组工具
```csharp
using LuYao;

// 获取空数组实例（内存优化）
var emptyInts = Arrays.Empty<int>();
var emptyStrings = Arrays.Empty<string>();
```

##### 3. 哈希算法
```csharp
using LuYao.IO.Hashing;

// 计算文件哈希
var md5 = HashAgents.MD5.HashFile("test.txt");
var sha1 = HashAgents.SHA1.HashFile("test.txt");

// 计算字符串哈希
var crc32 = HashAgents.CRC32.HashString("hello");
var crc64 = HashAgents.CRC64.HashString("world");

// 流哈希
using var stream = File.OpenRead("data.bin");
var murmur128 = HashAgents.Murmur128.HashStream(stream);
```

##### 4. 临时文件管理
```csharp
using LuYao.IO;

// 自动清理的临时文件
using (var tempFile = AutoCleanTempFile.Create())
{
    File.WriteAllText(tempFile.FileName, "临时内容");
    // 文件将在 dispose 时自动删除
}

// 创建指定扩展名的临时文件
using var tempCsv = AutoCleanTempFile.Create(".csv");
```

##### 5. 线程同步
```csharp
using LuYao.Threading;

// 基于键的锁，用于细粒度同步
var lockObj = KeyedLocker<string>.GetLock("my-resource");
lock (lockObj)
{
    // 临界区
}

// 异步锁
using var asyncLock = new AsyncLock();
using (await asyncLock.LockAsync())
{
    // 异步临界区
}
```

##### 6. 字符串压缩
```csharp
using LuYao.Encoders;
using LuYao;

// LZ-string 压缩
var compressed = LzString.CompressToBase64("Hello World");
var decompressed = LzString.DecompressFromBase64(compressed);

// GZip 压缩
var gzipCompressed = GZipString.Compress("大量文本数据");
var gzipDecompressed = GZipString.Decompress(gzipCompressed);
```

##### 7. Hosts 文件管理
```csharp
using LuYao.IO.Hosts;

// 读取和解析 hosts 文件
var hostsFile = HostFile.Read(@"C:\Windows\System32\drivers\etc\hosts");

// 从字符串解析
var hosts = HostFile.Parse("127.0.0.1 localhost\n192.168.1.1 router");

// 修改并访问条目
foreach (var line in hosts.Lines)
{
    if (line is RecordLine record)
    {
        Console.WriteLine($"{record.IPAddress} -> {record.Domain}");
    }
}
```

##### 8. 伪造用户代理
```csharp
using LuYao.Net.Http.FakeUserAgent;

// 获取随机用户代理
var selector = UserAgentSelector.All;
var userAgent = selector.Random();
Console.WriteLine(userAgent?.UserAgent);

// 按条件过滤
var chromeSelector = UserAgentSelector.Filter(x => x.Browser == "Chrome");
var chromeUA = chromeSelector.Random();
```

##### 9. 限流（令牌桶）
```csharp
using LuYao.Limiters.TokenBucket;

// 创建令牌桶限流器
var bucket = TokenBuckets.Builder()
    .WithCapacity(100)
    .WithFixedIntervalRefillStrategy(10, TimeSpan.FromSeconds(1))
    .Build();

// 尝试消费令牌
if (bucket.TryConsume(1))
{
    // 处理请求
}
else
{
    // 超过速率限制
}
```

##### 10. 密码强度检测
```csharp
using LuYao.Security;

var advisor = new PasswordAdvisor();
var score = advisor.CheckStrength("MyP@ssw0rd123");
Console.WriteLine(score); // Strong, Medium, Weak 等
```

##### 11. 文本分词与分析
```csharp
using LuYao.Text.Tokenizer;

// 构建自定义分析器
var analyzer = new AnalyzerBuilder()
    .WithCharacterFilter(new ToDbcCaseCharacterFilter())
    .WithTokenizer(new StandardTokenizer())
    .WithTokenFilter(new ToLowerCaseTokenFilter())
    .Build();

var tokens = analyzer.Analyze("Hello World!");
```

##### 12. 文件和路径工具
```csharp
using LuYao.IO;

// 文件大小格式化
var sizeStr = FileSizeHelper.FormatSize(1024 * 1024); // "1 MB"

// 路径工具
var normalized = PathHelper.NormalizePath(@"C:\Users\..\Documents");
```

#### LuYao.Text.Json

##### JSON 提取与处理
```csharp
using LuYao.Text.Json;

// 从混合内容中提取 JSON
var text = "前面的文本 {\"name\":\"张三\",\"age\":30} 后面的文本";
var json = JsonHelper.ExtractJson(text, Formatting.Indented);
Console.WriteLine(json); // {"name":"张三","age":30}
```

#### LuYao.Text.Json.Jint

##### JavaScript 驱动的 JSON 转换
```csharp
using LuYao.Text.Json;

// 定义带嵌入式 JS 的转换模型
public class MyModel : TranslatableJsonModel
{
    // 实现转换逻辑
}

// 使用模型进行 JSON 转换
var model = new MyModel();
// 使用 JavaScript 转换 JSON
```

### 从源码构建

```bash
# 克隆仓库
git clone https://github.com/coderbusy/luyao-dotnet.git
cd luyao-dotnet

# 构建解决方案
dotnet build --configuration Release

# 运行测试
dotnet test --configuration Release
```

### 运行测试

```bash
# 运行所有测试
dotnet test

# 运行特定测试项目
dotnet test tests/LuYao.Common.UnitTests/LuYao.Common.UnitTests.csproj
```

### 贡献

欢迎贡献！您可以：
- 通过 [Issues](https://github.com/coderbusy/luyao-dotnet/issues) 报告错误或请求功能
- 提交改进的拉取请求
- 改进文档
- 分享您的使用案例

### 许可证

本项目基于 [MIT License](LICENSE) 开源。

### 链接

- **GitHub**: [coderbusy/luyao-dotnet](https://github.com/coderbusy/luyao-dotnet)
- **NuGet 包**:
  - [LuYao.Common](https://www.nuget.org/packages/LuYao.Common)
  - [LuYao.Text.Json](https://www.nuget.org/packages/LuYao.Text.Json)
  - [LuYao.Text.Json.Jint](https://www.nuget.org/packages/LuYao.Text.Json.Jint)
- **网站**: [https://www.coderbusy.com/](https://www.coderbusy.com/)


---

<a name="english"></a>
## English

### Introduction

LuYao is a comprehensive collection of .NET utility libraries designed for enterprise-level applications. It provides a rich set of common tools, extensions, and utilities to help .NET developers write business code more efficiently.

The project consists of three main packages:
- **LuYao.Common**: Core utilities library with caching, hashing, I/O helpers, threading utilities, and more
- **LuYao.Text.Json**: JSON processing utilities built on Newtonsoft.Json
- **LuYao.Text.Json.Jint**: JavaScript-powered JSON transformation using Jint engine

### Supported Frameworks

- .NET Framework 4.5+
- .NET Framework 4.6.1+
- .NET Standard 2.0
- .NET Standard 2.1
- .NET 6.0
- .NET 8.0

### Installation

Install via NuGet Package Manager:

```bash
# Core utilities
dotnet add package LuYao.Common

# JSON utilities
dotnet add package LuYao.Text.Json

# Jint-based JSON transformation
dotnet add package LuYao.Text.Json.Jint
```

Or clone the source code:

```bash
git clone https://github.com/coderbusy/luyao-dotnet.git
```

### Key Features

#### LuYao.Common

##### 1. Caching
```csharp
using LuYao;

// Create a cached object with automatic expiration
var cache = new CachedObject<string>("Hello World", TimeSpan.FromMinutes(5));
Console.WriteLine(cache.Value); // "Hello World"
Console.WriteLine(cache.IsExpired); // false
```

##### 2. Array Utilities
```csharp
using LuYao;

// Get empty array instances (optimized for memory)
var emptyInts = Arrays.Empty<int>();
var emptyStrings = Arrays.Empty<string>();
```

##### 3. Hashing Algorithms
```csharp
using LuYao.IO.Hashing;

// Compute file hash
var md5 = HashAgents.MD5.HashFile("test.txt");
var sha1 = HashAgents.SHA1.HashFile("test.txt");

// Compute string hash
var crc32 = HashAgents.CRC32.HashString("hello");
var crc64 = HashAgents.CRC64.HashString("world");

// Stream hashing
using var stream = File.OpenRead("data.bin");
var murmur128 = HashAgents.Murmur128.HashStream(stream);
```

##### 4. Temporary File Management
```csharp
using LuYao.IO;

// Auto-cleaning temporary file
using (var tempFile = AutoCleanTempFile.Create())
{
    File.WriteAllText(tempFile.FileName, "Temporary content");
    // File will be automatically deleted when disposed
}

// Create temp file with specific extension
using var tempCsv = AutoCleanTempFile.Create(".csv");
```

##### 5. Thread Synchronization
```csharp
using LuYao.Threading;

// Keyed lock for fine-grained synchronization
var lockObj = KeyedLocker<string>.GetLock("my-resource");
lock (lockObj)
{
    // Critical section
}

// Async lock
using var asyncLock = new AsyncLock();
using (await asyncLock.LockAsync())
{
    // Async critical section
}
```

##### 6. String Compression
```csharp
using LuYao.Encoders;
using LuYao;

// LZ-string compression
var compressed = LzString.CompressToBase64("Hello World");
var decompressed = LzString.DecompressFromBase64(compressed);

// GZip compression
var gzipCompressed = GZipString.Compress("Large text data");
var gzipDecompressed = GZipString.Decompress(gzipCompressed);
```

##### 7. Hosts File Management
```csharp
using LuYao.IO.Hosts;

// Read and parse hosts file
var hostsFile = HostFile.Read(@"C:\Windows\System32\drivers\etc\hosts");

// Parse from string
var hosts = HostFile.Parse("127.0.0.1 localhost\n192.168.1.1 router");

// Modify and access entries
foreach (var line in hosts.Lines)
{
    if (line is RecordLine record)
    {
        Console.WriteLine($"{record.IPAddress} -> {record.Domain}");
    }
}
```

##### 8. Fake User Agent
```csharp
using LuYao.Net.Http.FakeUserAgent;

// Get random user agent
var selector = UserAgentSelector.All;
var userAgent = selector.Random();
Console.WriteLine(userAgent?.UserAgent);

// Filter by criteria
var chromeSelector = UserAgentSelector.Filter(x => x.Browser == "Chrome");
var chromeUA = chromeSelector.Random();
```

##### 9. Rate Limiting (Token Bucket)
```csharp
using LuYao.Limiters.TokenBucket;

// Create token bucket limiter
var bucket = TokenBuckets.Builder()
    .WithCapacity(100)
    .WithFixedIntervalRefillStrategy(10, TimeSpan.FromSeconds(1))
    .Build();

// Try to consume tokens
if (bucket.TryConsume(1))
{
    // Process request
}
else
{
    // Rate limit exceeded
}
```

##### 10. Password Strength Advisor
```csharp
using LuYao.Security;

var advisor = new PasswordAdvisor();
var score = advisor.CheckStrength("MyP@ssw0rd123");
Console.WriteLine(score); // Strong, Medium, Weak, etc.
```

##### 11. Text Tokenization and Analysis
```csharp
using LuYao.Text.Tokenizer;

// Build custom analyzer
var analyzer = new AnalyzerBuilder()
    .WithCharacterFilter(new ToDbcCaseCharacterFilter())
    .WithTokenizer(new StandardTokenizer())
    .WithTokenFilter(new ToLowerCaseTokenFilter())
    .Build();

var tokens = analyzer.Analyze("Hello World!");
```

##### 12. File and Path Helpers
```csharp
using LuYao.IO;

// File size formatting
var sizeStr = FileSizeHelper.FormatSize(1024 * 1024); // "1 MB"

// Path utilities
var normalized = PathHelper.NormalizePath(@"C:\Users\..\Documents");
```

#### LuYao.Text.Json

##### JSON Extraction and Processing
```csharp
using LuYao.Text.Json;

// Extract JSON from mixed content
var text = "Some text before {\"name\":\"John\",\"age\":30} some text after";
var json = JsonHelper.ExtractJson(text, Formatting.Indented);
Console.WriteLine(json); // {"name":"John","age":30}
```

#### LuYao.Text.Json.Jint

##### JavaScript-Powered JSON Transformation
```csharp
using LuYao.Text.Json;

// Define transformation model with embedded JS
public class MyModel : TranslatableJsonModel
{
    // Implement transformation logic
}

// Use the model for JSON transformation
var model = new MyModel();
// Transform JSON using JavaScript
```

### Building from Source

```bash
# Clone the repository
git clone https://github.com/coderbusy/luyao-dotnet.git
cd luyao-dotnet

# Build the solution
dotnet build --configuration Release

# Run tests
dotnet test --configuration Release
```

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/LuYao.Common.UnitTests/LuYao.Common.UnitTests.csproj
```

### Contributing

We welcome contributions! Please feel free to:
- Report bugs or request features via [Issues](https://github.com/coderbusy/luyao-dotnet/issues)
- Submit pull requests with improvements
- Improve documentation
- Share your use cases

### License

This project is licensed under the [MIT License](LICENSE).

### Links

- **GitHub**: [coderbusy/luyao-dotnet](https://github.com/coderbusy/luyao-dotnet)
- **NuGet Packages**:
  - [LuYao.Common](https://www.nuget.org/packages/LuYao.Common)
  - [LuYao.Text.Json](https://www.nuget.org/packages/LuYao.Text.Json)
  - [LuYao.Text.Json.Jint](https://www.nuget.org/packages/LuYao.Text.Json.Jint)
- **Website**: [https://www.coderbusy.com/](https://www.coderbusy.com/)
