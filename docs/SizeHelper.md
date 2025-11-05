# SizeHelper 使用文档

## 概述

`SizeHelper` 类提供了从字符串中提取尺寸信息的功能，并将尺寸值统一转换为厘米（CM）单位。该类支持多种单位格式、混合单位以及复杂的尺寸表达式。

## 特性

- **多种单位支持**: INCH, IN, MM, CM, DM, M
- **灵活的分隔符**: 支持 `x` 和 `*` 作为尺寸分隔符
- **小数支持**: 支持小数输入，如 10.5cm
- **混合单位**: 支持同一表达式中使用不同单位
- **多组尺寸**: 支持从括号中提取多组尺寸
- **智能过滤**: 自动忽略不支持的文字内容
- **统一输出**: 所有结果统一转换为厘米（CM）

## 支持的单位

| 单位 | 说明 | 转换率（相对于厘米） |
|------|------|-------------------|
| CM   | 厘米（基准单位） | 1 |
| MM   | 毫米 | 1 mm = 0.1 cm |
| DM   | 分米 | 1 dm = 10 cm |
| M    | 米 | 1 m = 100 cm |
| INCH, IN | 英寸 | 1 inch = 2.54 cm |

## 使用方法

### 基本用法

```csharp
using LuYao.Globalization;

// 单一单位格式
if (SizeHelper.ExtractSize("10x10x10cm", out decimal[] result))
{
    Console.WriteLine($"尺寸数量: {result.Length}");
    foreach (var size in result)
    {
        Console.WriteLine($"{size} cm");
    }
}
// 输出:
// 尺寸数量: 3
// 10 cm
// 10 cm
// 10 cm
```

### 无单位（默认为厘米）

```csharp
// 当没有明确单位时，默认使用厘米
if (SizeHelper.ExtractSize("10x20x30", out decimal[] result))
{
    // result = [10, 20, 30]
    Console.WriteLine($"长: {result[0]}cm, 宽: {result[1]}cm, 高: {result[2]}cm");
}
// 输出: 长: 10cm, 宽: 20cm, 高: 30cm
```

### 多个统一单位

```csharp
// 每个尺寸值都带有相同的单位
if (SizeHelper.ExtractSize("10cmx20cmx30cm", out decimal[] result))
{
    // result = [10, 20, 30]
}
```

### 多个不统一单位

```csharp
// 每个尺寸值使用不同的单位
if (SizeHelper.ExtractSize("10cmx5inx10m", out decimal[] result))
{
    // result = [10, 12.7, 1000]
    Console.WriteLine($"10cm = {result[0]}cm");
    Console.WriteLine($"5inch = {result[1]}cm");
    Console.WriteLine($"10m = {result[2]}cm");
}
// 输出:
// 10cm = 10cm
// 5inch = 12.7cm
// 10m = 1000cm
```

### 小数输入

```csharp
// 支持小数精度
if (SizeHelper.ExtractSize("10.5x20.3x30.7cm", out decimal[] result))
{
    // result = [10.5, 20.3, 30.7]
}
```

### 多组不同单位

```csharp
// 使用括号分隔多组尺寸，每组可以使用不同单位
if (SizeHelper.ExtractSize("10cmx10cmx10cm(3.94x3.94x3.94in)", out decimal[] result))
{
    // result = [10, 10, 10, 10.0076, 10.0076, 10.0076]
    Console.WriteLine($"第一组（厘米）: {result[0]}, {result[1]}, {result[2]}");
    Console.WriteLine($"第二组（英寸转厘米）: {result[3]}, {result[4]}, {result[5]}");
}
// 输出:
// 第一组（厘米）: 10, 10, 10
// 第二组（英寸转厘米）: 10.0076, 10.0076, 10.0076
```

### 忽略不支持的文字

```csharp
// 自动忽略非数字文字，提取有效的尺寸数据
if (SizeHelper.ExtractSize("尺寸(cm)：10x10x10", out decimal[] result))
{
    // result = [10, 10, 10]
    // 自动忽略 "尺寸" 和 "：" 等文字
}

if (SizeHelper.ExtractSize("Product dimensions: 5*10*15 inches", out decimal[] result))
{
    // result = [12.7, 25.4, 38.1]
    // 自动忽略 "Product dimensions: " 文字，识别 inches 单位
}
```

### 使用星号分隔符

```csharp
// 支持使用 * 作为分隔符
if (SizeHelper.ExtractSize("10*20*30cm", out decimal[] result))
{
    // result = [10, 20, 30]
}
```

### 不同单位示例

```csharp
// 毫米单位
if (SizeHelper.ExtractSize("100x100x100mm", out decimal[] result))
{
    // result = [10, 10, 10]  (100mm = 10cm)
}

// 分米单位
if (SizeHelper.ExtractSize("1x2x3dm", out decimal[] result))
{
    // result = [10, 20, 30]  (1dm = 10cm)
}

// 米单位
if (SizeHelper.ExtractSize("1x2x3m", out decimal[] result))
{
    // result = [100, 200, 300]  (1m = 100cm)
}

// 英寸单位
if (SizeHelper.ExtractSize("10x10x10in", out decimal[] result))
{
    // result = [25.4, 25.4, 25.4]  (1in = 2.54cm)
}
```

## 错误处理

```csharp
// 空字符串或 null
if (!SizeHelper.ExtractSize("", out decimal[] result))
{
    Console.WriteLine("提取失败");
    // result = []
}

// 没有分隔符
if (!SizeHelper.ExtractSize("10cm", out decimal[] result))
{
    Console.WriteLine("提取失败：需要至少两个尺寸值");
    // result = []
}

// 仅包含文字
if (!SizeHelper.ExtractSize("abc x def", out decimal[] result))
{
    Console.WriteLine("提取失败或返回空数组");
    // result = [] 或返回 false
}
```

## 实际应用场景

### 电商产品尺寸解析

```csharp
string productDescription = "包装尺寸: 30x20x15cm (11.8x7.9x5.9in)";
if (SizeHelper.ExtractSize(productDescription, out decimal[] sizes))
{
    Console.WriteLine($"厘米尺寸: {sizes[0]} x {sizes[1]} x {sizes[2]}");
    Console.WriteLine($"英寸转厘米: {sizes[3]:F2} x {sizes[4]:F2} x {sizes[5]:F2}");
    
    // 计算体积（立方厘米）
    decimal volume = sizes[0] * sizes[1] * sizes[2];
    Console.WriteLine($"体积: {volume} cm³");
}
```

### 物流包裹尺寸标准化

```csharp
string[] packageInputs = 
{
    "10*20*30cm",
    "5x10x15 inches",
    "100x200x300mm",
    "尺寸：1x2x3dm"
};

foreach (var input in packageInputs)
{
    if (SizeHelper.ExtractSize(input, out decimal[] sizes))
    {
        Console.WriteLine($"输入: {input}");
        Console.WriteLine($"标准化（厘米）: {sizes[0]} x {sizes[1]} x {sizes[2]}");
        Console.WriteLine();
    }
}
```

### 国际化尺寸转换

```csharp
// 美国客户输入（英寸）
string usInput = "24x18x12in";
if (SizeHelper.ExtractSize(usInput, out decimal[] sizes))
{
    Console.WriteLine("美国尺寸转为公制:");
    Console.WriteLine($"长: {sizes[0]}cm ({sizes[0] / 100}m)");
    Console.WriteLine($"宽: {sizes[1]}cm ({sizes[1] / 100}m)");
    Console.WriteLine($"高: {sizes[2]}cm ({sizes[2] / 100}m)");
}
```

## 注意事项

1. **返回值**: 方法返回 `bool` 类型，表示是否成功提取。成功时 `arr` 包含转换后的尺寸值；失败时 `arr` 为空数组。

2. **单位优先级**: 当字符串中同时出现多个单位关键字时（如 "CM" 和 "M"），优先级为：
   - INCH/IN > MM > DM > M > CM（仅当没有其他单位时）

3. **大小写不敏感**: 所有单位识别都不区分大小写（`cm`、`CM`、`Cm` 都有效）。

4. **小数精度**: 使用 `decimal` 类型确保高精度的尺寸计算。

5. **括号组**: 括号内的组只有在包含有效分隔符（x 或 *）时才会被处理。

6. **空格处理**: 空格会被自动处理和忽略。

## 性能考虑

- 使用正则表达式进行模式匹配，适合中等复杂度的字符串
- 对于大批量数据处理，建议缓存结果或考虑批量处理优化
- `decimal` 类型提供精确计算但比 `double` 稍慢

## 相关类

- `UnitConverter`: 提供更通用的单位转换功能，支持16个类别的100多种单位
- `RmbHelper`: 提供人民币金额大小写转换功能
