using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LuYao.Globalization;

/// <summary>
/// 提供尺寸提取和转换功能的辅助类。
/// </summary>
/// <remarks>
/// <para>此类支持从字符串中提取尺寸信息，并将其转换为以厘米为单位的数值数组。</para>
/// <para>支持的单位包括：</para>
/// <list type="bullet">
/// <item><description>INCH, IN - 英寸 (1 inch = 2.54 cm)</description></item>
/// <item><description>MM - 毫米 (1 mm = 0.1 cm)</description></item>
/// <item><description>CM - 厘米（基准单位）</description></item>
/// <item><description>DM - 分米 (1 dm = 10 cm)</description></item>
/// <item><description>M - 米 (1 m = 100 cm)</description></item>
/// </list>
/// </remarks>
/// <example>
/// <code>
/// // 单一数值
/// if (SizeHelper.ExtractSize("50cm", out decimal[] result))
/// {
///     // result = [50]
/// }
/// 
/// // 单一单位
/// if (SizeHelper.ExtractSize("10x10x10cm", out decimal[] result))
/// {
///     // result = [10, 10, 10]
/// }
/// 
/// // 多个不同单位
/// if (SizeHelper.ExtractSize("10cmx5inx10m", out decimal[] result))
/// {
///     // result = [10, 12.7, 1000]
/// }
/// 
/// // 多组尺寸
/// if (SizeHelper.ExtractSize("10cmx10cmx10cm(3.94x3.94x3.94in)", out decimal[] result))
/// {
///     // result = [10, 10, 10, 10.0076, 10.0076, 10.0076]
/// }
/// 
/// // 忽略不支持的文字
/// if (SizeHelper.ExtractSize("尺寸(cm)：10x10x10", out decimal[] result))
/// {
///     // result = [10, 10, 10]
/// }
/// </code>
/// </example>
public static class SizeHelper
{
    // Compiled regex patterns for better performance
    private static readonly Regex ParenthesesPattern = new Regex(@"\(([^)]+)\)", RegexOptions.Compiled);
    private static readonly Regex PerValueUnitPattern = new Regex(@"\d+\.?\d*\s*(inch|in|mm|cm|dm|m)\s*[x\*]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex NumberWithOptionalUnitPattern = new Regex(@"(\d+\.?\d*)\s*(inch|in|mm|cm|dm|m)?", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex UnitPattern = new Regex(@"(inch|in|mm|cm|dm|m)\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex NumberPattern = new Regex(@"(\d+\.?\d*)", RegexOptions.Compiled);
    
    // Unit conversion factors (relative to centimeters)
    private static readonly Dictionary<string, decimal> UnitConversions = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase)
    {
        { "CM", 1m },
        { "MM", 0.1m },
        { "INCH", 2.54m },
        { "IN", 2.54m },
        { "DM", 10m },
        { "M", 100m }
    };
    /// <summary>
    /// 从字符串中提取尺寸信息，并将其转换为以厘米为单位的数值数组。
    /// </summary>
    /// <param name="size">包含尺寸信息的字符串</param>
    /// <param name="arr">输出参数，包含转换为厘米的尺寸数组。如果提取失败，则返回空数组。</param>
    /// <returns>如果成功提取尺寸信息返回 <c>true</c>；否则返回 <c>false</c>。</returns>
    /// <remarks>
    /// <para>提取规则：</para>
    /// <list type="bullet">
    /// <item><description>支持单一数值输入：50cm，结果为 [50]</description></item>
    /// <item><description>使用 'x' 或 '*' 分隔多个尺寸值</description></item>
    /// <item><description>如果没有明确单位，默认单位为厘米（CM）</description></item>
    /// <item><description>支持小数输入，例如 "10.5cm"</description></item>
    /// <item><description>支持单一单位：10x10x10cm</description></item>
    /// <item><description>支持多个统一单位：10cmx10cmx10cm</description></item>
    /// <item><description>支持多个不统一单位：10cmx5inx10m</description></item>
    /// <item><description>支持多组不同单位：10cmx10cmx10cm(3.94x3.94x3.94in)，结果为 [10,10,10,10,10,10]</description></item>
    /// <item><description>忽略不支持的文字：尺寸(cm)：10x10x10，结果为 [10,10,10]</description></item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <code>
    /// // 单一数值
    /// if (SizeHelper.ExtractSize("50cm", out decimal[] result))
    /// {
    ///     // result = [50]
    /// }
    /// 
    /// // 多个尺寸
    /// if (SizeHelper.ExtractSize("10x20x30cm", out decimal[] result))
    /// {
    ///     foreach (var size in result)
    ///     {
    ///         Console.WriteLine($"{size} cm");
    ///     }
    /// }
    /// </code>
    /// </example>
    public static bool ExtractSize(string size, out decimal[] arr)
    {
        if (string.IsNullOrWhiteSpace(size))
        {
            arr = new decimal[0];
            return false;
        }

        var allResults = new List<decimal>();

        // Check if contains separators (x or *)
        if (!ContainsIgnoreCase(size, "x", "*"))
        {
            // Try to extract a single value
            var singleValue = ExtractSingleValue(size);
            if (singleValue.HasValue)
            {
                arr = new decimal[] { singleValue.Value };
                return true;
            }
            
            arr = new decimal[0];
            return false;
        }

        // Extract all groups (including those in parentheses)
        // Split by parentheses to handle multiple groups
        var groups = ExtractSizeGroups(size);

        foreach (var group in groups)
        {
            if (string.IsNullOrWhiteSpace(group))
                continue;

            var groupResults = ExtractSizeFromGroup(group);
            allResults.AddRange(groupResults);
        }

        if (allResults.Count > 0)
        {
            arr = allResults.ToArray();
            return true;
        }

        arr = new decimal[0];
        return false;
    }

    /// <summary>
    /// 从字符串中提取所有尺寸组（包括括号内的组）。
    /// </summary>
    private static List<string> ExtractSizeGroups(string size)
    {
        var groups = new List<string>();

        // First, extract groups in parentheses that contain separators
        var matches = ParenthesesPattern.Matches(size);
        
        foreach (Match match in matches)
        {
            if (match.Groups.Count > 1)
            {
                var content = match.Groups[1].Value;
                // Only add if the content contains separators
                if (ContainsIgnoreCase(content, "x", "*"))
                {
                    groups.Add(content);
                }
            }
        }

        // Then, remove parentheses content and add the remaining as a group
        var withoutParentheses = ParenthesesPattern.Replace(size, string.Empty);
        if (!string.IsNullOrWhiteSpace(withoutParentheses))
        {
            groups.Insert(0, withoutParentheses);
        }

        return groups;
    }

    /// <summary>
    /// 从单个尺寸组中提取尺寸值。
    /// </summary>
    private static List<decimal> ExtractSizeFromGroup(string group)
    {
        var results = new List<decimal>();

        // Detect if there are per-value units or a single unit for all values
        bool hasPerValueUnit = HasPerValueUnit(group);

        if (hasPerValueUnit)
        {
            // Each value has its own unit (e.g., "10cmx5inx10m")
            results = ExtractWithPerValueUnits(group);
        }
        else
        {
            // Single unit for all values (e.g., "10x10x10cm")
            results = ExtractWithSingleUnit(group);
        }

        return results;
    }

    /// <summary>
    /// 从不包含分隔符的字符串中提取单个尺寸值。
    /// </summary>
    private static decimal? ExtractSingleValue(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return null;

        // Try to match number with optional unit pattern
        var match = NumberWithOptionalUnitPattern.Match(input);
        
        if (match.Success && match.Groups[1].Success && !string.IsNullOrWhiteSpace(match.Groups[1].Value))
        {
            if (decimal.TryParse(match.Groups[1].Value, out decimal value))
            {
                string unit = match.Groups[2].Success && !string.IsNullOrEmpty(match.Groups[2].Value) 
                    ? match.Groups[2].Value 
                    : "CM";
                return ConvertToCentimeters(value, unit);
            }
        }

        return null;
    }

    /// <summary>
    /// 检查组是否包含每个值的单位。
    /// </summary>
    private static bool HasPerValueUnit(string group)
    {
        // Check if units appear multiple times before separators
        // Pattern: number + unit + separator
        return PerValueUnitPattern.IsMatch(group);
    }

    /// <summary>
    /// 从带有每个值单位的组中提取尺寸。
    /// </summary>
    private static List<decimal> ExtractWithPerValueUnits(string group)
    {
        var results = new List<decimal>();

        // Pattern to match: number + optional unit
        // We need to extract each number with its unit
        var matches = NumberWithOptionalUnitPattern.Matches(group);

        foreach (Match match in matches)
        {
            if (match.Groups[1].Success && !string.IsNullOrWhiteSpace(match.Groups[1].Value))
            {
                if (decimal.TryParse(match.Groups[1].Value, out decimal value))
                {
                    string unit = match.Groups[2].Success && !string.IsNullOrEmpty(match.Groups[2].Value) 
                        ? match.Groups[2].Value 
                        : "CM";
                    decimal converted = ConvertToCentimeters(value, unit);
                    results.Add(converted);
                }
            }
        }

        return results;
    }

    /// <summary>
    /// 从带有单一单位的组中提取尺寸。
    /// </summary>
    private static List<decimal> ExtractWithSingleUnit(string group)
    {
        var results = new List<decimal>();

        // Determine the unit (default is CM)
        string unit = DetermineUnit(group);

        // Remove unit suffixes
        var cleaned = UnitPattern.Replace(group, string.Empty);

        // Extract all numbers separated by x or *
        // Use a more robust pattern that looks for numbers around separators
        var matches = NumberPattern.Matches(cleaned);

        foreach (Match match in matches)
        {
            if (decimal.TryParse(match.Value, out decimal value) && value != 0)
            {
                decimal converted = ConvertToCentimeters(value, unit);
                results.Add(converted);
            }
        }

        return results;
    }

    /// <summary>
    /// 确定字符串中的单位类型。
    /// </summary>
    private static string DetermineUnit(string group)
    {
        // Priority order: INCH/IN > MM > DM > M > CM
        if (ContainsIgnoreCase(group, "INCH", "IN"))
            return "INCH";
        if (ContainsIgnoreCase(group, "MM"))
            return "MM";
        if (ContainsIgnoreCase(group, "DM"))
            return "DM";
        if (ContainsIgnoreCase(group, "M") && !ContainsIgnoreCase(group, "CM", "MM", "DM"))
            return "M";
        
        return "CM"; // Default
    }

    /// <summary>
    /// 将指定单位的值转换为厘米。
    /// </summary>
    private static decimal ConvertToCentimeters(decimal value, string unit)
    {
        if (UnitConversions.TryGetValue(unit, out decimal factor))
        {
            return value * factor;
        }
        return value; // Default to CM if unit not found
    }

    /// <summary>
    /// 检查字符串是否包含任意一个指定的子串（忽略大小写）。
    /// </summary>
    private static bool ContainsIgnoreCase(string source, params string[] values)
    {
        if (string.IsNullOrEmpty(source) || values == null || values.Length == 0)
            return false;

        foreach (var value in values)
        {
            if (!string.IsNullOrEmpty(value) && source.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0)
                return true;
        }

        return false;
    }
}
