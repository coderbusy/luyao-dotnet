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
    /// <summary>
    /// 从字符串中提取尺寸信息，并将其转换为以厘米为单位的数值数组。
    /// </summary>
    /// <param name="size">包含尺寸信息的字符串</param>
    /// <param name="arr">输出参数，包含转换为厘米的尺寸数组。如果提取失败，则返回空数组。</param>
    /// <returns>如果成功提取尺寸信息返回 <c>true</c>；否则返回 <c>false</c>。</returns>
    /// <remarks>
    /// <para>提取规则：</para>
    /// <list type="bullet">
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

        // Check if contains separators (x or *)
        if (!ContainsIgnoreCase(size, "x", "*"))
        {
            arr = new decimal[0];
            return false;
        }

        var allResults = new List<decimal>();

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
        var parenthesesPattern = @"\(([^)]+)\)";
        var matches = Regex.Matches(size, parenthesesPattern);
        
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
        var withoutParentheses = Regex.Replace(size, parenthesesPattern, string.Empty);
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
    /// 检查组是否包含每个值的单位。
    /// </summary>
    private static bool HasPerValueUnit(string group)
    {
        // Check if units appear multiple times before separators
        // Pattern: number + unit + separator
        var pattern = @"\d+\.?\d*\s*(inch|in|mm|cm|dm|m)\s*[x\*]";
        return Regex.IsMatch(group, pattern, RegexOptions.IgnoreCase);
    }

    /// <summary>
    /// 从带有每个值单位的组中提取尺寸。
    /// </summary>
    private static List<decimal> ExtractWithPerValueUnits(string group)
    {
        var results = new List<decimal>();

        // Pattern to match: number + optional unit
        // We need to extract each number with its unit
        var pattern = @"(\d+\.?\d*)\s*(inch|in|mm|cm|dm|m)?";
        var matches = Regex.Matches(group, pattern, RegexOptions.IgnoreCase);

        foreach (Match match in matches)
        {
            if (match.Groups[1].Success && !string.IsNullOrWhiteSpace(match.Groups[1].Value))
            {
                if (decimal.TryParse(match.Groups[1].Value, out decimal value))
                {
                    string unit = match.Groups[2].Success ? match.Groups[2].Value : "CM";
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
        string unit = "CM";
        if (ContainsIgnoreCase(group, "INCH", "IN"))
        {
            unit = "INCH";
        }
        else if (ContainsIgnoreCase(group, "MM"))
        {
            unit = "MM";
        }
        else if (ContainsIgnoreCase(group, "DM"))
        {
            unit = "DM";
        }
        else if (ContainsIgnoreCase(group, "M") && !ContainsIgnoreCase(group, "CM", "MM", "DM"))
        {
            unit = "M";
        }
        else if (ContainsIgnoreCase(group, "CM"))
        {
            unit = "CM";
        }

        // Remove unit suffixes
        var cleaned = Regex.Replace(group, @"(inch|in|mm|cm|dm|m)\b", string.Empty, RegexOptions.IgnoreCase);

        // Extract all numbers separated by x or *
        // Use a more robust pattern that looks for numbers around separators
        var numberPattern = @"(\d+\.?\d*)";
        var matches = Regex.Matches(cleaned, numberPattern);

        foreach (Match match in matches)
        {
            if (decimal.TryParse(match.Value, out decimal value))
            {
                if (value != 0)
                {
                    decimal converted = ConvertToCentimeters(value, unit);
                    results.Add(converted);
                }
            }
        }

        return results;
    }

    /// <summary>
    /// 将指定单位的值转换为厘米。
    /// </summary>
    private static decimal ConvertToCentimeters(decimal value, string unit)
    {
        switch (unit.ToUpperInvariant())
        {
            case "CM":
                return value;
            case "MM":
                return value / 10m; // 1 cm = 10 mm
            case "INCH":
            case "IN":
                return value * 2.54m; // 1 inch = 2.54 cm
            case "DM":
                return value * 10m; // 1 dm = 10 cm
            case "M":
                return value * 100m; // 1 m = 100 cm
            default:
                return value; // Default to CM
        }
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
