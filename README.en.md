# LuYao .NET Utility Library

[![MIT License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)
[![NuGet](https://img.shields.io/nuget/v/LuYao.Common.svg)](https://www.nuget.org/packages/LuYao.Common)
[![Build](https://img.shields.io/github/actions/workflow/status/coderbusy/luyao-dotnet/build.yml?branch=main)](https://github.com/coderbusy/luyao-dotnet/actions)

[中文](README.zh-CN.md) | [English](README.en.md)

> This document is kept in sync with [README.zh-CN.md](README.zh-CN.md).

### Introduction

LuYao is a comprehensive collection of .NET utility libraries designed for enterprise-level applications. It provides a rich set of common tools, extensions, and utilities to help .NET developers write business code more efficiently.

The project consists of four main packages:
- **LuYao.Common**: Core utilities library with caching, hashing, I/O helpers, threading utilities, and more
- **LuYao.Net.Http.FakeUserAgent**: Fake user agent utilities
- **LuYao.Text.Json**: JSON processing utilities built on Newtonsoft.Json
- **LuYao.Text.Json.Jint**: JavaScript-powered JSON transformation using Jint engine

### Supported Frameworks

- .NET Framework 4.5+
- .NET Framework 4.6.1+
- .NET Standard 2.0
- .NET Standard 2.1
- .NET 6.0
- .NET 8.0
- .NET 10.0

### Installation

Install via NuGet Package Manager:

```bash
# Core utilities
dotnet add package LuYao.Common

# Fake User Agent
dotnet add package LuYao.Net.Http.FakeUserAgent

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
  - [LuYao.Net.Http.FakeUserAgent](https://www.nuget.org/packages/LuYao.Net.Http.FakeUserAgent)
  - [LuYao.Text.Json](https://www.nuget.org/packages/LuYao.Text.Json)
  - [LuYao.Text.Json.Jint](https://www.nuget.org/packages/LuYao.Text.Json.Jint)
- **Website**: [https://www.coderbusy.com/](https://www.coderbusy.com/)
