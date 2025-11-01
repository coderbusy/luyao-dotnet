# UnitConverter 单位转换器

## 概述

`UnitConverter` 是 LuYao.Common 库中的单位转换工具，支持 16 个类别、100+ 个物理量单位之间的高精度转换。

## 快速开始

```csharp
using LuYao.Globalization;

var converter = new UnitConverter();

// 长度转换
if (converter.TryExchange("inches", "centimeters", 10m, out decimal result))
{
    Console.WriteLine($"10 inches = {result} cm"); // 输出: 10 inches = 25.4 cm
}

// 温度转换
if (converter.TryExchange("degrees_celsius", "degrees_fahrenheit", 100m, out result))
{
    Console.WriteLine($"100°C = {result}°F"); // 输出: 100°C = 212°F
}

// 压力转换 (新增)
if (converter.TryExchange("psi", "bar", 14.7m, out result))
{
    Console.WriteLine($"14.7 psi = {result} bar"); // 输出: 14.7 psi ≈ 1.01 bar
}

// 数据存储转换
if (converter.TryExchange("gb", "mb", 5m, out result))
{
    Console.WriteLine($"5 GB = {result} MB"); // 输出: 5 GB = 5120 MB
}
```

## 支持的单位类别

### 基础类别 (v1.0)
1. **长度 (Length)** - 米、厘米、英寸、英尺等
2. **质量 (Mass)** - 千克、克、磅、盎司等
3. **温度 (Temperature)** - 摄氏度、华氏度、开尔文
4. **体积 (Volume)** - 升、加仑、立方米等
5. **速度 (Speed)** - 米/秒、千米/时、英里/时等
6. **面积 (Area)** - 平方米、平方英尺等
7. **数据存储 (DataStorage)** - 字节、KB、MB、GB、TB等
8. **能量 (Energy)** - 焦耳、卡路里、千瓦时等

### 新增类别 (v2.0) 🆕
9. **压力 (Pressure)** - 帕斯卡、巴、PSI、大气压等
10. **时间 (Time)** - 秒、毫秒、微秒、分钟、小时、天、周
11. **功率 (Power)** - 瓦特、千瓦、兆瓦、马力
12. **频率 (Frequency)** - 赫兹、千赫兹、兆赫兹、吉赫兹
13. **角度 (Angle)** - 弧度、度、角分、角秒
14. **电流 (ElectricCurrent)** - 安培、毫安、微安
15. **电压 (Voltage)** - 伏特、毫伏、千伏
16. **电阻 (ElectricResistance)** - 欧姆、千欧、兆欧

## 文档链接

- **[完整文档 (中文)](./UnitConverter.md)** - 详细的使用指南和示例
- **[Full Documentation (English)](./UnitConverter-EN.md)** - Complete guide with examples
- **[快速参考](./UnitConverter-QuickRef.md)** - 单位名称速查表

## 特性

- ✅ **16 个类别，100+ 个单位** - 涵盖常用物理量单位
- ✅ **高精度计算** - 使用 decimal 类型确保精确转换
- ✅ **类型安全** - 自动检查单位兼容性
- ✅ **不区分大小写** - 单位名称查询灵活
- ✅ **易于使用** - 简洁的 TryExchange API
- ✅ **全面测试** - 79 个单元测试覆盖所有转换场景

## 单位数据来源

本单位转换器基于 [amazon_units.csv](../src/LuYao.Common/Data/amazon_units.csv) 文件开发，该文件包含 786 个单位定义，其中 433 个支持简单固定转换。转换器实现了其中最常用的 100+ 个单位。

## 版本历史

- **v2.0** (2024): 新增 8 个类别，扩展至 100+ 个单位
- **v1.0**: 初始版本，支持 8 个基础类别

## 许可证

MIT License
