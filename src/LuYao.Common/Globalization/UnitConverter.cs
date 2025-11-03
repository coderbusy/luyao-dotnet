using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Globalization;

/// <summary>
/// 提供单位转换功能的类，支持多种物理量单位之间的转换。
/// </summary>
/// <remarks>
/// 支持的转换类别包括：
/// <list type="bullet">
/// <item><description>长度 (Length): 米、厘米、毫米、千米、英寸、英尺、码、英里、海里、纳米、微米、皮米、埃</description></item>
/// <item><description>质量 (Mass): 千克、克、毫克、微克、磅、盎司、吨、公吨、克拉</description></item>
/// <item><description>温度 (Temperature): 摄氏度、华氏度、开尔文</description></item>
/// <item><description>体积 (Volume): 升、毫升、立方米、加仑、液盎司、立方英寸</description></item>
/// <item><description>速度 (Speed): 米/秒、千米/时、英里/时、英尺/秒</description></item>
/// <item><description>面积 (Area): 平方米、平方厘米、平方毫米、平方英寸、平方英尺、平方码、英亩、公顷</description></item>
/// <item><description>数据存储 (DataStorage): 字节、千字节、兆字节、吉字节、太字节、拍字节、比特</description></item>
/// <item><description>能量 (Energy): 焦耳、千焦、卡路里、千卡、千瓦时、英热单位</description></item>
/// <item><description>压力 (Pressure): 帕斯卡、千帕、兆帕、巴、毫巴、大气压、磅/平方英寸、托</description></item>
/// <item><description>时间 (Time): 秒、毫秒、微秒、纳秒、皮秒、分钟、小时、天、周</description></item>
/// <item><description>功率 (Power): 瓦特、毫瓦、微瓦、千瓦、兆瓦、马力</description></item>
/// <item><description>频率 (Frequency): 赫兹、千赫兹、兆赫兹、吉赫兹</description></item>
/// <item><description>角度 (Angle): 弧度、度、角分、角秒</description></item>
/// <item><description>电流 (ElectricCurrent): 安培、毫安、微安</description></item>
/// <item><description>电压 (Voltage): 伏特、毫伏、千伏</description></item>
/// <item><description>电阻 (ElectricResistance): 欧姆、毫欧、千欧、兆欧</description></item>
/// </list>
/// </remarks>
/// <example>
/// <code>
/// var converter = new UnitConverter();
/// 
/// // 长度转换
/// if (converter.TryExchange("inches", "centimeters", 10m, out decimal result))
/// {
///     Console.WriteLine($"10 inches = {result} cm"); // 输出: 10 inches = 25.4 cm
/// }
/// 
/// // 温度转换
/// if (converter.TryExchange("degrees_celsius", "degrees_fahrenheit", 100m, out result))
/// {
///     Console.WriteLine($"100°C = {result}°F"); // 输出: 100°C = 212°F
/// }
/// </code>
/// </example>
public class UnitConverter
{
    private readonly Dictionary<string, UnitDefinition> _unitDefinitions;
    private readonly Dictionary<string, ConversionCategory> _categories;

    /// <summary>
    /// 初始化 <see cref="UnitConverter"/> 类的新实例。
    /// </summary>
    public UnitConverter()
    {
        _unitDefinitions = new Dictionary<string, UnitDefinition>(StringComparer.OrdinalIgnoreCase);
        _categories = new Dictionary<string, ConversionCategory>();
        InitializeUnitDefinitions();
    }

    /// <summary>
    /// 单位转换请求的参数类，用于封装多单位选择转换所需的所有参数。
    /// </summary>
    public class UnitExchangeRequest
    {
        /// <summary>
        /// 获取或设置源单位名称（不区分大小写）。
        /// </summary>
        public string From { get; set; } = string.Empty;

        /// <summary>
        /// 获取或设置可接受的目标单位数组（不区分大小写）。
        /// </summary>
        public string[] TargetUnits { get; set; } = new string[0];

        /// <summary>
        /// 获取或设置要转换的数值。
        /// </summary>
        public decimal Value { get; set; }

        /// <summary>
        /// 获取或设置转换结果的最小值（包含）。
        /// </summary>
        public decimal Min { get; set; }

        /// <summary>
        /// 获取或设置转换结果的最大值（包含）。
        /// </summary>
        public decimal Max { get; set; }
    }

    /// <summary>
    /// 单位转换结果的响应类，包含转换后的单位和值。
    /// </summary>
    public class UnitExchangeResponse
    {
        /// <summary>
        /// 获取或设置转换是否成功。
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 获取或设置转换后选择的单位。如果转换失败，则为 <c>null</c>。
        /// </summary>
        public string? Unit { get; set; }

        /// <summary>
        /// 获取或设置转换后的结果值。如果转换失败，则为 0。
        /// </summary>
        public decimal Result { get; set; }
    }

    /// <summary>
    /// 从可接受的目标单位数组中选择合适的单位进行转换，返回第一个满足范围条件的单位。
    /// </summary>
    /// <param name="request">包含转换所需参数的请求对象。</param>
    /// <returns>包含转换结果的响应对象。</returns>
    /// <remarks>
    /// <para>算法规则：</para>
    /// <list type="number">
    /// <item><description>按顺序遍历目标单位数组，将源值转换为每个单位的值。</description></item>
    /// <item><description>返回第一个转换后的值在 [min, max] 范围内的单位。</description></item>
    /// </list>
    /// <para>只有同一类别的单位才能相互转换。例如，长度单位只能转换为其他长度单位，不能转换为质量单位。</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var converter = new UnitConverter();
    /// var request = new UnitConverter.UnitExchangeRequest
    /// {
    ///     From = "grams",
    ///     TargetUnits = new[] { "grams", "kilograms", "tons", "milligrams" },
    ///     Value = 1500m,
    ///     Min = 0m,
    ///     Max = 99.99m
    /// };
    /// 
    /// var response = converter.TryExchange(request);
    /// if (response.Success)
    /// {
    ///     Console.WriteLine($"选择单位: {response.Unit}, 值: {response.Result}");
    ///     // 返回第一个满足条件的单位 kilograms (1.5)
    /// }
    /// </code>
    /// </example>
    public UnitExchangeResponse TryExchange(UnitExchangeRequest request)
    {
        if (request == null)
        {
            return new UnitExchangeResponse { Success = false };
        }

        bool success = TryExchange(request.From, request.TargetUnits, request.Value, request.Min, request.Max, out string? unit, out decimal result);
        
        return new UnitExchangeResponse
        {
            Success = success,
            Unit = unit,
            Result = result
        };
    }

    /// <summary>
    /// 从可接受的目标单位数组中选择合适的单位进行转换，返回第一个满足范围条件的单位。
    /// </summary>
    /// <param name="from">源单位名称（不区分大小写）。</param>
    /// <param name="targetUnits">可接受的目标单位数组（不区分大小写）。</param>
    /// <param name="value">要转换的数值。</param>
    /// <param name="min">转换结果的最小值（包含）。</param>
    /// <param name="max">转换结果的最大值（包含）。</param>
    /// <param name="unit">转换后选择的单位。如果转换失败，则为 <c>null</c>。</param>
    /// <param name="result">转换后的结果值。如果转换失败，则为 0。</param>
    /// <returns>
    /// 如果找到满足条件的单位并转换成功，返回 <c>true</c>；否则返回 <c>false</c>。
    /// </returns>
    /// <remarks>
    /// <para>算法规则：</para>
    /// <list type="number">
    /// <item><description>按顺序遍历目标单位数组，将源值转换为每个单位的值。</description></item>
    /// <item><description>返回第一个转换后的值在 [min, max] 范围内的单位。</description></item>
    /// </list>
    /// <para>只有同一类别的单位才能相互转换。例如，长度单位只能转换为其他长度单位，不能转换为质量单位。</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var converter = new UnitConverter();
    /// string[] units = { "grams", "kilograms", "tons", "milligrams" };
    /// 
    /// // 转换 1500 克，要求结果在 0 到 99.99 之间
    /// if (converter.TryExchange("grams", units, 1500m, 0m, 99.99m, out string selectedUnit, out decimal result))
    /// {
    ///     Console.WriteLine($"选择单位: {selectedUnit}, 值: {result}"); 
    ///     // 因为 grams 的值 1500 超过 99.99，会跳过并返回第一个满足条件的 kilograms (1.5)
    /// }
    /// </code>
    /// </example>
    public bool TryExchange(string from, string[] targetUnits, decimal value, decimal min, decimal max, out string? unit, out decimal result)
    {
        unit = null;
        result = 0;

        if (string.IsNullOrEmpty(from) || targetUnits == null || targetUnits.Length == 0)
            return false;

        if (min > max)
            return false;

        // 按顺序尝试转换到每个目标单位，返回第一个满足条件的
        foreach (var targetUnit in targetUnits)
        {
            if (string.IsNullOrEmpty(targetUnit))
                continue;

            // 执行转换
            if (TryExchange(from, targetUnit, value, out decimal convertedValue))
            {
                // 检查转换后的值是否在范围内
                if (convertedValue >= min && convertedValue <= max)
                {
                    // 找到第一个满足条件的单位，立即返回
                    unit = targetUnit;
                    result = convertedValue;
                    return true;
                }
            }
        }

        // 没有找到满足条件的单位
        return false;
    }

    /// <summary>
    /// 将指定数值从原始单位转换为目标单位。
    /// </summary>
    /// <param name="from">源单位名称（不区分大小写）。</param>
    /// <param name="to">目标单位名称（不区分大小写）。</param>
    /// <param name="value">要转换的数值。</param>
    /// <param name="result">转换后的结果值。如果转换失败，则为 0。</param>
    /// <returns>
    /// 如果转换成功，返回 <c>true</c>；如果单位不兼容、单位不存在或转换过程中发生错误，则返回 <c>false</c>。
    /// </returns>
    /// <remarks>
    /// 只有同一类别的单位才能相互转换。例如，长度单位只能转换为其他长度单位，不能转换为质量单位。
    /// 如果源单位和目标单位相同，则直接返回原始值。
    /// </remarks>
    /// <example>
    /// <code>
    /// var converter = new UnitConverter();
    /// if (converter.TryExchange("meters", "feet", 10m, out decimal result))
    /// {
    ///     Console.WriteLine($"10 meters = {result} feet");
    /// }
    /// </code>
    /// </example>
    public bool TryExchange(string from, string to, decimal value, out decimal result)
    {
        result = 0;

        if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to))
            return false;

        // 如果单位相同，直接返回
        if (string.Equals(from, to, StringComparison.OrdinalIgnoreCase))
        {
            result = value;
            return true;
        }

        // 查找单位定义
        if (!_unitDefinitions.TryGetValue(from, out var fromUnit) ||
            !_unitDefinitions.TryGetValue(to, out var toUnit))
            return false;

        // 检查单位是否兼容（同一类别）
        if (fromUnit.Category != toUnit.Category)
            return false;

        // 执行转换
        try
        {
            // 先转换为基准单位，再转换为目标单位
            decimal baseValue = fromUnit.ToBaseUnit(value);
            result = toUnit.FromBaseUnit(baseValue);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private void InitializeUnitDefinitions()
    {
        // 1. 长度单位
        InitializeLengthUnits();

        // 2. 质量单位
        InitializeMassUnits();

        // 3. 温度单位（需要特殊处理）
        InitializeTemperatureUnits();

        // 4. 体积/容量单位
        InitializeVolumeUnits();

        // 5. 速度单位
        InitializeSpeedUnits();

        // 6. 面积单位
        InitializeAreaUnits();

        // 7. 数据存储单位
        InitializeDataStorageUnits();

        // 8. 能量单位
        InitializeEnergyUnits();

        // 9. 压力单位
        InitializePressureUnits();

        // 10. 时间单位（增强版）
        InitializeTimeUnits();

        // 11. 功率单位（增强版）
        InitializePowerUnits();

        // 12. 频率单位
        InitializeFrequencyUnits();

        // 13. 角度单位
        InitializeAngleUnits();

        // 14. 电流单位
        InitializeElectricCurrentUnits();

        // 15. 电压单位
        InitializeVoltageUnits();

        // 16. 电阻单位
        InitializeElectricResistanceUnits();

    }

    private void InitializeLengthUnits()
    {
        var category = "Length";

        AddUnit("meters", category, 1.0m, 0m); // 基准单位：米
        AddUnit("centimeters", category, 0.01m, 0m);
        AddUnit("millimeters", category, 0.001m, 0m);
        AddUnit("kilometers", category, 1000m, 0m);
        AddUnit("inches", category, 0.0254m, 0m);
        AddUnit("feet", category, 0.3048m, 0m);
        AddUnit("yards", category, 0.9144m, 0m);
        AddUnit("miles", category, 1609.344m, 0m);
        AddUnit("nautical_miles", category, 1852m, 0m);

        // Additional length units from amazon_units.csv
        AddUnit("angstrom", category, 0.0000000001m, 0m); // 埃 (Angstrom)
        AddUnit("nanometer", category, 0.000000001m, 0m); // 纳米
        AddUnit("nanometers", category, 0.000000001m, 0m);
        AddUnit("micrometer", category, 0.000001m, 0m); // 微米
        AddUnit("micrometers", category, 0.000001m, 0m);
        AddUnit("picometer", category, 0.000000000001m, 0m); // 皮米
        AddUnit("decimeters", category, 0.1m, 0m); // 分米
        AddUnit("meter", category, 1.0m, 0m); // 米（单数形式）
        AddUnit("kilometer", category, 1000m, 0m); // 千米（单数形式）
    }

    private void InitializeMassUnits()
    {
        var category = "Mass";

        AddUnit("kilograms", category, 1.0m, 0m); // 基准单位：千克
        AddUnit("grams", category, 0.001m, 0m);
        AddUnit("milligrams", category, 0.000001m, 0m);
        AddUnit("pounds", category, 0.45359237m, 0m);
        AddUnit("ounces", category, 0.028349523125m, 0m);

        // Additional mass units from amazon_units.csv
        AddUnit("micrograms", category, 0.000000001m, 0m); // 微克
        AddUnit("tons", category, 907.185m, 0m); // 美制吨
        AddUnit("metric_tons", category, 1000m, 0m); // 公吨
        AddUnit("lbs", category, 0.45359237m, 0m); // 磅（简写）
        AddUnit("ounce", category, 0.028349523125m, 0m); // 盎司（单数）
        AddUnit("carats", category, 0.0002m, 0m); // 克拉
        AddUnit("hundredths_pounds", category, 0.0045359237m, 0m); // 百分之一磅
    }

    private void InitializeTemperatureUnits()
    {
        var category = "Temperature";

        // 温度单位需要特殊处理，使用自定义转换函数
        AddUnit("degrees_celsius", category,
            toBase: (value) => value, // 摄氏度作为基准
            fromBase: (value) => value);

        AddUnit("degrees_fahrenheit", category,
            toBase: (value) => (value - 32m) * 5m / 9m, // 华氏度转摄氏度
            fromBase: (value) => value * 9m / 5m + 32m); // 摄氏度转华氏度

        AddUnit("kelvin", category,
            toBase: (value) => value - 273.15m, // 开尔文转摄氏度
            fromBase: (value) => value + 273.15m); // 摄氏度转开尔文
    }

    private void InitializeVolumeUnits()
    {
        var category = "Volume";

        AddUnit("liters", category, 1.0m, 0m); // 基准单位：升
        AddUnit("milliliters", category, 0.001m, 0m);
        AddUnit("cubic_meters", category, 1000m, 0m);
        AddUnit("gallons", category, 3.785411784m, 0m); // 美制加仑
        AddUnit("fluid_ounces", category, 0.0295735295625m, 0m); // 美制液盎司
        AddUnit("cubic_inches", category, 0.016387064m, 0m);
    }

    private void InitializeSpeedUnits()
    {
        var category = "Speed";

        AddUnit("meters_per_second", category, 1.0m, 0m); // 基准单位：米/秒
        AddUnit("kilometers_per_hour", category, 1m / 3.6m, 0m);
        AddUnit("miles_per_hour", category, 0.44704m, 0m);
        AddUnit("feet_per_second", category, 0.3048m, 0m);
    }

    private void InitializeAreaUnits()
    {
        var category = "Area";

        AddUnit("square_meters", category, 1.0m, 0m); // 基准单位：平方米
        AddUnit("square_centimeters", category, 0.0001m, 0m);
        AddUnit("square_millimeters", category, 0.000001m, 0m);
        AddUnit("square_inches", category, 0.00064516m, 0m);
        AddUnit("square_feet", category, 0.09290304m, 0m);
        AddUnit("square_yards", category, 0.83612736m, 0m);
        AddUnit("acres", category, 4046.8564224m, 0m);
        AddUnit("hectares", category, 10000m, 0m);
    }

    private void InitializeDataStorageUnits()
    {
        var category = "DataStorage";

        AddUnit("bytes", category, 1.0m, 0m); // 基准单位：字节
        AddUnit("kilobytes", category, 1024m, 0m);
        AddUnit("megabytes", category, 1024m * 1024m, 0m); // 1024^2
        AddUnit("gigabytes", category, 1024m * 1024m * 1024m, 0m); // 1024^3
        AddUnit("terabytes", category, 1024m * 1024m * 1024m * 1024m, 0m); // 1024^4

        // Additional data storage units from amazon_units.csv
        AddUnit("bits", category, 0.125m, 0m); // 比特
        AddUnit("kilobyte", category, 1024m, 0m); // 单数形式
        AddUnit("kb", category, 1024m, 0m); // 简写
        AddUnit("megabyte", category, 1024m * 1024m, 0m); // 单数形式
        AddUnit("mb", category, 1024m * 1024m, 0m); // 简写
        AddUnit("gigabyte", category, 1024m * 1024m * 1024m, 0m); // 单数形式
        AddUnit("gb", category, 1024m * 1024m * 1024m, 0m); // 简写
        AddUnit("terabyte", category, 1024m * 1024m * 1024m * 1024m, 0m); // 单数形式
        AddUnit("tb", category, 1024m * 1024m * 1024m * 1024m, 0m); // 简写
        AddUnit("petabyte", category, 1024m * 1024m * 1024m * 1024m * 1024m, 0m); // 1024^5
        AddUnit("petabytes", category, 1024m * 1024m * 1024m * 1024m * 1024m, 0m);
    }

    private void InitializeEnergyUnits()
    {
        var category = "Energy";

        AddUnit("joules", category, 1.0m, 0m); // 基准单位：焦耳
        AddUnit("kilojoules", category, 1000m, 0m);
        AddUnit("calories", category, 4.184m, 0m); // 热化学卡路里
        AddUnit("kilocalories", category, 4184m, 0m); // 千卡
        AddUnit("kilowatt_hours", category, 3600000m, 0m);
        AddUnit("btus", category, 1055.05585262m, 0m); // 英热单位
    }

    private void InitializePressureUnits()
    {
        var category = "Pressure";

        AddUnit("pascals", category, 1.0m, 0m); // 基准单位：帕斯卡
        AddUnit("pascal", category, 1.0m, 0m);
        AddUnit("hectopascal", category, 100m, 0m); // 百帕
        AddUnit("kilopascal", category, 1000m, 0m); // 千帕
        AddUnit("kilopascals", category, 1000m, 0m);
        AddUnit("megapascal", category, 1000000m, 0m); // 兆帕
        AddUnit("megapascals", category, 1000000m, 0m);
        AddUnit("bar", category, 100000m, 0m); // 巴
        AddUnit("bars", category, 100000m, 0m);
        AddUnit("millibar", category, 100m, 0m); // 毫巴
        AddUnit("millibars", category, 100m, 0m);
        AddUnit("atmosphere", category, 101325m, 0m); // 标准大气压
        AddUnit("atmospheres", category, 101325m, 0m);
        AddUnit("pounds_per_square_inch", category, 6894.76m, 0m); // 磅力/平方英寸
        AddUnit("psi", category, 6894.76m, 0m);
        AddUnit("torr", category, 133.322m, 0m); // 托
    }

    private void InitializeTimeUnits()
    {
        var category = "Time";

        AddUnit("seconds", category, 1.0m, 0m); // 基准单位：秒
        AddUnit("second", category, 1.0m, 0m);
        AddUnit("milliseconds", category, 0.001m, 0m); // 毫秒
        AddUnit("millisecond", category, 0.001m, 0m);
        AddUnit("microseconds", category, 0.000001m, 0m); // 微秒
        AddUnit("microsecond", category, 0.000001m, 0m);
        AddUnit("nanoseconds", category, 0.000000001m, 0m); // 纳秒
        AddUnit("nanosecond", category, 0.000000001m, 0m);
        AddUnit("picoseconds", category, 0.000000000001m, 0m); // 皮秒
        AddUnit("picosecond", category, 0.000000000001m, 0m);
        AddUnit("minutes", category, 60m, 0m); // 分钟
        AddUnit("minute", category, 60m, 0m);
        AddUnit("hours", category, 3600m, 0m); // 小时
        AddUnit("hour", category, 3600m, 0m);
        AddUnit("days", category, 86400m, 0m); // 天
        AddUnit("day", category, 86400m, 0m);
        AddUnit("weeks", category, 604800m, 0m); // 周
        AddUnit("week", category, 604800m, 0m);
    }

    private void InitializePowerUnits()
    {
        var category = "Power";

        AddUnit("watts", category, 1.0m, 0m); // 基准单位：瓦特
        AddUnit("watt", category, 1.0m, 0m);
        AddUnit("milliwatts", category, 0.001m, 0m); // 毫瓦
        AddUnit("milliwatt", category, 0.001m, 0m);
        AddUnit("microwatts", category, 0.000001m, 0m); // 微瓦
        AddUnit("microwatt", category, 0.000001m, 0m);
        AddUnit("kilowatts", category, 1000m, 0m); // 千瓦
        AddUnit("kilowatt", category, 1000m, 0m);
        AddUnit("megawatts", category, 1000000m, 0m); // 兆瓦
        AddUnit("megawatt", category, 1000000m, 0m);
        AddUnit("horsepower", category, 745.7m, 0m); // 马力
    }

    private void InitializeFrequencyUnits()
    {
        var category = "Frequency";

        AddUnit("hertz", category, 1.0m, 0m); // 基准单位：赫兹
        AddUnit("hz", category, 1.0m, 0m);
        AddUnit("kilohertz", category, 1000m, 0m); // 千赫兹
        AddUnit("khz", category, 1000m, 0m);
        AddUnit("megahertz", category, 1000000m, 0m); // 兆赫兹
        AddUnit("mhz", category, 1000000m, 0m);
        AddUnit("gigahertz", category, 1000000000m, 0m); // 吉赫兹
        AddUnit("ghz", category, 1000000000m, 0m);
    }

    private void InitializeAngleUnits()
    {
        var category = "Angle";

        AddUnit("radians", category, 1.0m, 0m); // 基准单位：弧度
        AddUnit("radian", category, 1.0m, 0m);

        // π/180 弧度 = 1度
        decimal degreesToRadians = (decimal)(Math.PI / 180.0);
        AddUnit("degrees", category, degreesToRadians, 0m); // 度 (π/180)
        AddUnit("degree", category, degreesToRadians, 0m);

        // 1角分 = 1/60度
        AddUnit("arc_minute", category, degreesToRadians / 60m, 0m); // 角分
        AddUnit("arc_minutes", category, degreesToRadians / 60m, 0m);

        // 1角秒 = 1/3600度
        AddUnit("arc_sec", category, degreesToRadians / 3600m, 0m); // 角秒
        AddUnit("arc_seconds", category, degreesToRadians / 3600m, 0m);
    }

    private void InitializeElectricCurrentUnits()
    {
        var category = "ElectricCurrent";

        AddUnit("amperes", category, 1.0m, 0m); // 基准单位：安培
        AddUnit("ampere", category, 1.0m, 0m);
        AddUnit("amps", category, 1.0m, 0m);
        AddUnit("amp", category, 1.0m, 0m);
        AddUnit("milliamps", category, 0.001m, 0m); // 毫安
        AddUnit("milliamperes", category, 0.001m, 0m);
        AddUnit("microamps", category, 0.000001m, 0m); // 微安
        AddUnit("microamperes", category, 0.000001m, 0m);
    }

    private void InitializeVoltageUnits()
    {
        var category = "Voltage";

        AddUnit("volts", category, 1.0m, 0m); // 基准单位：伏特
        AddUnit("volt", category, 1.0m, 0m);
        AddUnit("millivolts", category, 0.001m, 0m); // 毫伏
        AddUnit("millivolt", category, 0.001m, 0m);
        AddUnit("kilovolts", category, 1000m, 0m); // 千伏
        AddUnit("kilovolt", category, 1000m, 0m);
    }

    private void InitializeElectricResistanceUnits()
    {
        var category = "ElectricResistance";

        AddUnit("ohms", category, 1.0m, 0m); // 基准单位：欧姆
        AddUnit("ohm", category, 1.0m, 0m);
        AddUnit("milliohms", category, 0.001m, 0m); // 毫欧
        AddUnit("milliohm", category, 0.001m, 0m);
        AddUnit("kilohms", category, 1000m, 0m); // 千欧
        AddUnit("kilohm", category, 1000m, 0m);
        AddUnit("megohms", category, 1000000m, 0m); // 兆欧
        AddUnit("megohm", category, 1000000m, 0m);
    }

    private void AddUnit(string unitName, string category, decimal factor, decimal offset)
    {
        _unitDefinitions[unitName] = new UnitDefinition
        {
            Name = unitName,
            Category = category,
            ToBaseUnit = (value) => value * factor + offset,
            FromBaseUnit = (value) => (value - offset) / factor
        };
    }

    private void AddUnit(string unitName, string category,
        Func<decimal, decimal> toBase, Func<decimal, decimal> fromBase)
    {
        _unitDefinitions[unitName] = new UnitDefinition
        {
            Name = unitName,
            Category = category,
            ToBaseUnit = toBase,
            FromBaseUnit = fromBase
        };
    }

    // 单位定义内部类
    private class UnitDefinition
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public Func<decimal, decimal> ToBaseUnit { get; set; }
        public Func<decimal, decimal> FromBaseUnit { get; set; }
    }

    // 转换类别（用于组织）
    private class ConversionCategory
    {
        public string Name { get; set; }
        public string BaseUnit { get; set; }
    }
}
