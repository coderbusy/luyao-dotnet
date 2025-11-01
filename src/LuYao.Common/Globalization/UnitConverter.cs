using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Globalization
{
  /// <summary>
    /// 提供单位转换功能的类，支持多种物理量单位之间的转换。
    /// </summary>
    /// <remarks>
    /// 支持的转换类别包括：
    /// <list type="bullet">
    /// <item><description>长度 (Length): 米、厘米、毫米、千米、英寸、英尺、码、英里、海里</description></item>
    /// <item><description>质量 (Mass): 千克、克、毫克、磅、盎司</description></item>
    /// <item><description>温度 (Temperature): 摄氏度、华氏度、开尔文</description></item>
    /// <item><description>体积 (Volume): 升、毫升、立方米、加仑、液盎司、立方英寸</description></item>
    /// <item><description>速度 (Speed): 米/秒、千米/时、英里/时、英尺/秒</description></item>
    /// <item><description>面积 (Area): 平方米、平方厘米、平方毫米、平方英寸、平方英尺、平方码、英亩、公顷</description></item>
    /// <item><description>数据存储 (DataStorage): 字节、千字节、兆字节、吉字节、太字节</description></item>
    /// <item><description>能量 (Energy): 焦耳、千焦、卡路里、千卡、千瓦时、英热单位</description></item>
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
        }

        private void InitializeMassUnits()
        {
            var category = "Mass";

            AddUnit("kilograms", category, 1.0m, 0m); // 基准单位：千克
            AddUnit("grams", category, 0.001m, 0m);
            AddUnit("milligrams", category, 0.000001m, 0m);
            AddUnit("pounds", category, 0.45359237m, 0m);
            AddUnit("ounces", category, 0.028349523125m, 0m);
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
            AddUnit("megabytes", category, 1024m * 1024m, 0m);
            AddUnit("gigabytes", category, 1024m * 1024m * 1024m, 0m);
            AddUnit("terabytes", category, 1024m * 1024m * 1024m * 1024m, 0m);
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
}
