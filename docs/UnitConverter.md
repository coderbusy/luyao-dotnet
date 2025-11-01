# UnitConverter 使用文档

## 概述

`UnitConverter` 类提供了强大的单位转换功能，支持多种物理量单位之间的自动转换。该类已从amazon_units.csv文件中整合了100多个常用单位，覆盖16个主要类别。

## 支持的单位类别

### 1. 长度 (Length)
支持从极小尺度（埃）到极大尺度（英里）的各种长度单位转换。

**基准单位**: 米 (meters)

**支持的单位**:
- 公制: meters, centimeters, millimeters, kilometers, decimeters, nanometers, micrometers, picometers, angstrom
- 英制: inches, feet, yards, miles, nautical_miles

**示例**:
```csharp
var converter = new UnitConverter();

// 英寸转厘米
if (converter.TryExchange("inches", "centimeters", 10m, out decimal result))
{
    Console.WriteLine($"10 inches = {result} cm"); // 输出: 10 inches = 25.4 cm
}

// 纳米转微米
if (converter.TryExchange("nanometers", "micrometers", 1000m, out result))
{
    Console.WriteLine($"1000 nm = {result} μm"); // 输出: 1000 nm = 1 μm
}
```

### 2. 质量 (Mass)
支持从微克到公吨的各种质量单位转换。

**基准单位**: 千克 (kilograms)

**支持的单位**:
- 公制: kilograms, grams, milligrams, micrograms, metric_tons
- 英制: pounds, lbs, ounces, ounce, tons
- 其他: carats (克拉)

**示例**:
```csharp
// 磅转千克
if (converter.TryExchange("pounds", "kilograms", 10m, out decimal result))
{
    Console.WriteLine($"10 lbs = {result} kg"); // 输出: 10 lbs = 4.5359237 kg
}

// 克拉转克
if (converter.TryExchange("carats", "grams", 5m, out result))
{
    Console.WriteLine($"5 carats = {result} g"); // 输出: 5 carats = 1 g
}
```

### 3. 温度 (Temperature)
支持摄氏度、华氏度和开尔文之间的转换（需要特殊处理偏移量）。

**基准单位**: 摄氏度 (degrees_celsius)

**支持的单位**: degrees_celsius, degrees_fahrenheit, kelvin

**示例**:
```csharp
// 摄氏度转华氏度
if (converter.TryExchange("degrees_celsius", "degrees_fahrenheit", 100m, out decimal result))
{
    Console.WriteLine($"100°C = {result}°F"); // 输出: 100°C = 212°F
}

// 摄氏度转开尔文
if (converter.TryExchange("degrees_celsius", "kelvin", 0m, out result))
{
    Console.WriteLine($"0°C = {result} K"); // 输出: 0°C = 273.15 K
}
```

### 4. 体积 (Volume)
支持公制和英制体积单位转换。

**基准单位**: 升 (liters)

**支持的单位**:
- 公制: liters, milliliters, centiliters, deciliters, cubic_meters, cubic_centimeters
- 英制: gallons, fluid_ounces, cubic_feet, cubic_inches, cubic_yards, cups

**示例**:
```csharp
// 加仑转升
if (converter.TryExchange("gallons", "liters", 1m, out decimal result))
{
    Console.WriteLine($"1 gallon = {result} L"); // 输出: 1 gallon = 3.78541 L
}
```

### 5. 速度 (Speed)
支持各种速度单位转换。

**基准单位**: 米/秒 (meters_per_second)

**支持的单位**: meters_per_second, kilometers_per_hour, miles_per_hour, feet_per_second

**示例**:
```csharp
// 公里/小时 转 米/秒
if (converter.TryExchange("kilometers_per_hour", "meters_per_second", 36m, out decimal result))
{
    Console.WriteLine($"36 km/h = {result} m/s"); // 输出: 36 km/h = 10 m/s
}
```

### 6. 面积 (Area)
支持各种面积单位转换。

**基准单位**: 平方米 (square_meters)

**支持的单位**:
- 公制: square_meters, square_centimeters, square_millimeters, hectares
- 英制: square_inches, square_feet, square_yards, acres

**示例**:
```csharp
// 平方英尺转平方米
if (converter.TryExchange("square_feet", "square_meters", 100m, out decimal result))
{
    Console.WriteLine($"100 ft² = {result} m²"); // 输出: 100 ft² ≈ 9.29 m²
}
```

### 7. 数据存储 (DataStorage)
支持各种数据存储单位转换，使用二进制（1024进制）。

**基准单位**: 字节 (bytes)

**支持的单位**:
- bits, bytes
- kilobyte, kilobytes, kb
- megabyte, megabytes, mb
- gigabyte, gigabytes, gb
- terabyte, terabytes, tb
- petabyte, petabytes

**示例**:
```csharp
// GB转MB
if (converter.TryExchange("gb", "mb", 1m, out decimal result))
{
    Console.WriteLine($"1 GB = {result} MB"); // 输出: 1 GB = 1024 MB
}

// 比特转字节
if (converter.TryExchange("bits", "bytes", 8m, out result))
{
    Console.WriteLine($"8 bits = {result} byte"); // 输出: 8 bits = 1 byte
}
```

### 8. 能量 (Energy)
支持各种能量单位转换。

**基准单位**: 焦耳 (joules)

**支持的单位**: joules, kilojoules, calories, kilocalories, kilowatt_hours, btus, watt_hours

**示例**:
```csharp
// 千卡转焦耳
if (converter.TryExchange("kilocalories", "joules", 1m, out decimal result))
{
    Console.WriteLine($"1 kcal = {result} J"); // 输出: 1 kcal = 4184 J
}
```

### 9. 压力 (Pressure) 🆕
支持各种压力单位转换。

**基准单位**: 帕斯卡 (pascals)

**支持的单位**:
- pascals, pascal, hectopascal, kilopascal, kilopascals, megapascal, megapascals
- bar, bars, millibar, millibars
- atmosphere, atmospheres
- pounds_per_square_inch, psi
- torr

**示例**:
```csharp
// PSI转帕斯卡
if (converter.TryExchange("psi", "pascals", 14.7m, out decimal result))
{
    Console.WriteLine($"14.7 psi = {result} Pa"); // 输出: 14.7 psi ≈ 101325 Pa
}

// 大气压转巴
if (converter.TryExchange("atmosphere", "bar", 1m, out result))
{
    Console.WriteLine($"1 atm = {result} bar"); // 输出: 1 atm ≈ 1.01325 bar
}
```

### 10. 时间 (Time) 🆕
支持从皮秒到周的各种时间单位转换。

**基准单位**: 秒 (seconds)

**支持的单位**:
- seconds, second
- milliseconds, millisecond
- microseconds, microsecond
- nanoseconds, nanosecond
- picoseconds, picosecond
- minutes, minute
- hours, hour
- days, day
- weeks, week

**示例**:
```csharp
// 小时转分钟
if (converter.TryExchange("hours", "minutes", 2.5m, out decimal result))
{
    Console.WriteLine($"2.5 hours = {result} minutes"); // 输出: 2.5 hours = 150 minutes
}

// 毫秒转秒
if (converter.TryExchange("milliseconds", "seconds", 1500m, out result))
{
    Console.WriteLine($"1500 ms = {result} s"); // 输出: 1500 ms = 1.5 s
}
```

### 11. 功率 (Power) 🆕
支持各种功率单位转换。

**基准单位**: 瓦特 (watts)

**支持的单位**:
- watts, watt
- milliwatts, milliwatt
- microwatts, microwatt
- kilowatts, kilowatt
- megawatts, megawatt
- horsepower

**示例**:
```csharp
// 马力转瓦特
if (converter.TryExchange("horsepower", "watts", 1m, out decimal result))
{
    Console.WriteLine($"1 hp = {result} W"); // 输出: 1 hp = 745.7 W
}

// 千瓦转马力
if (converter.TryExchange("kilowatts", "horsepower", 75m, out result))
{
    Console.WriteLine($"75 kW = {result} hp"); // 输出: 75 kW ≈ 100.6 hp
}
```

### 12. 频率 (Frequency) 🆕
支持各种频率单位转换。

**基准单位**: 赫兹 (hertz)

**支持的单位**: hertz, hz, kilohertz, khz, megahertz, mhz, gigahertz, ghz

**示例**:
```csharp
// MHz转Hz
if (converter.TryExchange("megahertz", "hertz", 2.4m, out decimal result))
{
    Console.WriteLine($"2.4 MHz = {result} Hz"); // 输出: 2.4 MHz = 2400000 Hz
}

// GHz转MHz
if (converter.TryExchange("ghz", "mhz", 3.6m, out result))
{
    Console.WriteLine($"3.6 GHz = {result} MHz"); // 输出: 3.6 GHz = 3600 MHz
}
```

### 13. 角度 (Angle) 🆕
支持各种角度单位转换。

**基准单位**: 弧度 (radians)

**支持的单位**:
- radians, radian
- degrees, degree
- arc_minute, arc_minutes
- arc_sec, arc_seconds

**示例**:
```csharp
// 度转弧度
if (converter.TryExchange("degrees", "radians", 180m, out decimal result))
{
    Console.WriteLine($"180° = {result} rad"); // 输出: 180° ≈ 3.14159 rad (π)
}

// 角分转度
if (converter.TryExchange("arc_minute", "degrees", 60m, out result))
{
    Console.WriteLine($"60' = {result}°"); // 输出: 60' = 1°
}
```

### 14. 电流 (ElectricCurrent) 🆕
支持各种电流单位转换。

**基准单位**: 安培 (amperes)

**支持的单位**:
- amperes, ampere, amps, amp
- milliamps, milliamperes
- microamps, microamperes

**示例**:
```csharp
// 毫安转安培
if (converter.TryExchange("milliamps", "amperes", 500m, out decimal result))
{
    Console.WriteLine($"500 mA = {result} A"); // 输出: 500 mA = 0.5 A
}
```

### 15. 电压 (Voltage) 🆕
支持各种电压单位转换。

**基准单位**: 伏特 (volts)

**支持的单位**:
- volts, volt
- millivolts, millivolt
- kilovolts, kilovolt

**示例**:
```csharp
// 千伏转伏特
if (converter.TryExchange("kilovolts", "volts", 2.2m, out decimal result))
{
    Console.WriteLine($"2.2 kV = {result} V"); // 输出: 2.2 kV = 2200 V
}
```

### 16. 电阻 (ElectricResistance) 🆕
支持各种电阻单位转换。

**基准单位**: 欧姆 (ohms)

**支持的单位**:
- ohms, ohm
- milliohms, milliohm
- kilohms, kilohm
- megohms, megohm

**示例**:
```csharp
// 千欧转欧姆
if (converter.TryExchange("kilohms", "ohms", 4.7m, out decimal result))
{
    Console.WriteLine($"4.7 kΩ = {result} Ω"); // 输出: 4.7 kΩ = 4700 Ω
}
```

## 使用方法

### 基本用法

```csharp
using LuYao.Globalization;

var converter = new UnitConverter();

// 使用TryExchange方法进行单位转换
if (converter.TryExchange("sourceUnit", "targetUnit", value, out decimal result))
{
    Console.WriteLine($"转换成功: {result}");
}
else
{
    Console.WriteLine("转换失败：单位不兼容或不存在");
}
```

### 特点

1. **不区分大小写**: 单位名称查询不区分大小写
   ```csharp
   converter.TryExchange("METERS", "kilometers", 1000m, out result); // 正常工作
   ```

2. **类型安全**: 不同类别的单位无法相互转换
   ```csharp
   converter.TryExchange("meters", "kilograms", 10m, out result); // 返回 false
   ```

3. **精确计算**: 使用 `decimal` 类型确保高精度计算

4. **同单位处理**: 源单位和目标单位相同时直接返回原值
   ```csharp
   converter.TryExchange("meters", "meters", 100m, out result); // result = 100
   ```

## 完整示例程序

```csharp
using System;
using LuYao.Globalization;

public class Program
{
    public static void Main()
    {
        var converter = new UnitConverter();
        
        Console.WriteLine("=== 单位转换示例 ===\n");
        
        // 1. 长度转换
        if (converter.TryExchange("inches", "centimeters", 10m, out decimal result))
        {
            Console.WriteLine($"长度: 10 inches = {result} cm");
        }
        
        // 2. 温度转换
        if (converter.TryExchange("degrees_celsius", "degrees_fahrenheit", 100m, out result))
        {
            Console.WriteLine($"温度: 100°C = {result}°F");
        }
        
        // 3. 压力转换
        if (converter.TryExchange("psi", "bar", 14.7m, out result))
        {
            Console.WriteLine($"压力: 14.7 psi = {result:F2} bar");
        }
        
        // 4. 数据存储转换
        if (converter.TryExchange("gb", "mb", 5m, out result))
        {
            Console.WriteLine($"存储: 5 GB = {result} MB");
        }
        
        // 5. 功率转换
        if (converter.TryExchange("horsepower", "kilowatts", 100m, out result))
        {
            Console.WriteLine($"功率: 100 hp = {result:F2} kW");
        }
        
        // 6. 频率转换
        if (converter.TryExchange("ghz", "mhz", 3.5m, out result))
        {
            Console.WriteLine($"频率: 3.5 GHz = {result} MHz");
        }
        
        // 7. 角度转换
        if (converter.TryExchange("degrees", "radians", 90m, out result))
        {
            Console.WriteLine($"角度: 90° = {result:F4} rad");
        }
        
        // 8. 电阻转换
        if (converter.TryExchange("kilohms", "ohms", 10m, out result))
        {
            Console.WriteLine($"电阻: 10 kΩ = {result} Ω");
        }
        
        // 9. 错误处理：不兼容单位
        if (!converter.TryExchange("meters", "kilograms", 10m, out result))
        {
            Console.WriteLine("\n错误示例: 无法将长度转换为质量");
        }
        
        // 10. 时间转换
        if (converter.TryExchange("hours", "seconds", 2m, out result))
        {
            Console.WriteLine($"时间: 2 hours = {result} seconds");
        }
    }
}
```

**输出**:
```
=== 单位转换示例 ===

长度: 10 inches = 25.4 cm
温度: 100°C = 212°F
压力: 14.7 psi = 1.01 bar
存储: 5 GB = 5120 MB
功率: 100 hp = 74.57 kW
频率: 3.5 GHz = 3500 MHz
角度: 90° = 1.5708 rad
电阻: 10 kΩ = 10000 Ω

错误示例: 无法将长度转换为质量
时间: 2 hours = 7200 seconds
```

## 注意事项

1. **单位名称必须精确**: 使用文档中列出的单位名称，虽然不区分大小写，但拼写必须正确
2. **温度转换特殊性**: 温度转换涉及偏移量（如摄氏度到华氏度），不仅仅是简单的乘法
3. **数据存储使用二进制**: KB/MB/GB等使用1024进制，而不是1000进制
4. **精度考虑**: 某些转换可能涉及无理数（如π），结果会有精度限制

## 数据来源

本单位转换器基于 `amazon_units.csv` 文件（包含786个单位，其中433个可简单转换）进行开发，确保了广泛的单位覆盖和准确的转换系数。

## 版本历史

- **v2.0** (当前版本): 新增8个单位类别，100+个新单位，全面增强转换能力
- **v1.0**: 初始版本，支持8个基础类别

## 许可证

本项目遵循MIT许可证。
