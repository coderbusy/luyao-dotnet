using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LuYao.Globalization;

partial class SizeHelper
{
    private static readonly Regex DimensionLabelPattern = new Regex(@"(\d+(?:\.\d+)?)\s*([''""]*)?\s*([WwHhLl]|Width|HEIGHT|Length)?", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    
    /// <summary>
    /// 从字符串中提取尺寸信息，返回包含单位和维度信息的Dimension列表
    /// </summary>
    /// <param name="size">包含尺寸信息的字符串</param>
    /// <returns>解析后的Dimension列表</returns>
    /// <remarks>
    /// <para>提取规则：</para>
    /// <list type="bullet">
    /// <item><description>支持厘米(cm)和英寸(in/'')两种单位</description></item>
    /// <item><description>如果输入单位是cm或in，保持原单位；其他单位转换为cm</description></item>
    /// <item><description>无单位标记的输入(如1x2x3)，DimensionKind为Unspecified</description></item>
    /// <item><description>支持维度标记: W/Width(宽), H/Height(高), L/Length(长)</description></item>
    /// <item><description>支持英寸的双引号标记: 10'' 或 10"</description></item>
    /// <item><description>支持括号内的多组尺寸，每组返回独立的Dimension对象</description></item>
    /// </list>
    /// </remarks>
    public static IReadOnlyList<Dimension> Extract(string size)
    {
        if (string.IsNullOrWhiteSpace(size))
        {
            return new List<Dimension>();
        }

        var results = new List<Dimension>();
        
        // Extract all groups (including those in parentheses)
        var groups = ExtractSizeGroups(size);
        
        foreach (var group in groups)
        {
            if (string.IsNullOrWhiteSpace(group))
                continue;
                
            var dimension = ParseDimensionGroup(group);
            if (dimension != null && !dimension.IsEmpty)
            {
                results.Add(dimension);
            }
        }
        
        return results;
    }
    
    /// <summary>
    /// 解析单个尺寸组，返回Dimension对象
    /// </summary>
    private static Dimension? ParseDimensionGroup(string group)
    {
        if (string.IsNullOrWhiteSpace(group))
            return null;
            
        var dimension = new Dimension();
        
        // 检测单位
        DimensionUnit detectedUnit = DetectDimensionUnit(group);
        dimension.Unit = detectedUnit;
        
        // 检查是否有分隔符
        bool hasSeparator = ContainsIgnoreCase(group, "x", "*");
        
        if (!hasSeparator)
        {
            // 单个值
            var item = ParseSingleDimensionValue(group);
            if (item.HasValue)
            {
                dimension.Items.Add(item.Value);
            }
            return dimension;
        }
        
        // 多个值的情况
        bool hasPerValueUnit = HasPerValueUnit(group);
        
        if (hasPerValueUnit)
        {
            // 每个值有自己的单位
            dimension.Items = ParseWithPerValueUnits(group, dimension.Unit);
        }
        else
        {
            // 所有值共享一个单位
            dimension.Items = ParseWithSingleUnit(group, dimension.Unit);
        }
        
        return dimension;
    }
    
    /// <summary>
    /// 检测并返回尺寸组的单位类型
    /// </summary>
    private static DimensionUnit DetectDimensionUnit(string group)
    {
        // 检查是否有英寸标记 ('' 或 " 或 in/inch)
        if (group.Contains("''") || group.Contains("\"") || 
            ContainsIgnoreCase(group, "inch") || 
            System.Text.RegularExpressions.Regex.IsMatch(group, @"\d+\s*in\b", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
        {
            return DimensionUnit.Inch;
        }
        
        // 检查是否有厘米标记
        if (ContainsIgnoreCase(group, "cm"))
        {
            return DimensionUnit.Centimeter;
        }
        
        // 检查是否有其他单位（mm, dm, m）- 这些需要转换为cm
        if (ContainsIgnoreCase(group, "mm", "dm") || 
            (ContainsIgnoreCase(group, "m") && !ContainsIgnoreCase(group, "cm", "mm", "dm")))
        {
            return DimensionUnit.Centimeter; // 转换为厘米
        }
        
        // 默认无单位，使用厘米
        return DimensionUnit.Centimeter;
    }
    
    /// <summary>
    /// 解析单个尺寸值（不含分隔符）
    /// </summary>
    private static DimensionItem? ParseSingleDimensionValue(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return null;
            
        // 提取数值和维度标签
        var match = DimensionLabelPattern.Match(input);
        
        if (!match.Success || !match.Groups[1].Success)
            return null;
            
        if (!decimal.TryParse(match.Groups[1].Value, out decimal value))
            return null;
            
        // 检测维度类型
        DimensionKind kind = DimensionKind.Unspecified;
        if (match.Groups[3].Success && !string.IsNullOrEmpty(match.Groups[3].Value))
        {
            kind = ParseDimensionKind(match.Groups[3].Value);
        }
        
        // 单位转换（如果需要）
        decimal finalValue = ConvertToTargetUnit(value, input, DetectDimensionUnit(input));
        
        return new DimensionItem(kind, finalValue);
    }
    
    /// <summary>
    /// 解析带有每个值独立单位的尺寸组
    /// </summary>
    private static List<DimensionItem> ParseWithPerValueUnits(string group, DimensionUnit targetUnit)
    {
        var items = new List<DimensionItem>();
        
        // 先按分隔符分割，然后逐个解析
        var parts = System.Text.RegularExpressions.Regex.Split(group, @"\s*[xX\*]\s*");
        
        foreach (var part in parts)
        {
            if (string.IsNullOrWhiteSpace(part))
                continue;
            
            var trimmed = part.Trim();
            
            // 解析单个部分: 数字 + 可选英寸标记 + 可选维度标签 + 可选单位 + 可选维度标签
            var match = System.Text.RegularExpressions.Regex.Match(trimmed, 
                @"^(\d+(?:\.\d+)?)\s*([''""])?\s*([WwHhLl]|Width|HEIGHT|Length)?\s*(inch|in|mm|cm|dm|m)?\s*([WwHhLl]|Width|HEIGHT|Length)?$",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            
            if (!match.Success || !match.Groups[1].Success)
                continue;
                
            if (!decimal.TryParse(match.Groups[1].Value, out decimal value))
                continue;
            
            // 检测英寸标记 (group 2)
            bool hasInchMarker = match.Groups[2].Success && !string.IsNullOrEmpty(match.Groups[2].Value);
            
            // 检测单位 (group 4)
            string unitStr = match.Groups[4].Success ? match.Groups[4].Value : "";
            
            // 检测维度标签 (group 3 在单位前, group 5 在单位后)
            // 优先使用单位后的标签
            string labelStr = "";
            if (match.Groups[5].Success && !string.IsNullOrEmpty(match.Groups[5].Value))
            {
                labelStr = match.Groups[5].Value;
            }
            else if (match.Groups[3].Success && !string.IsNullOrEmpty(match.Groups[3].Value))
            {
                labelStr = match.Groups[3].Value;
            }
            
            // 转换值到目标单位
            decimal convertedValue = ConvertValueToUnit(value, unitStr, hasInchMarker, targetUnit);
            
            // 解析维度类型
            DimensionKind kind = DimensionKind.Unspecified;
            if (!string.IsNullOrEmpty(labelStr))
            {
                kind = ParseDimensionKind(labelStr);
            }
            
            items.Add(new DimensionItem(kind, convertedValue));
        }
        
        return items;
    }
    
    /// <summary>
    /// 解析带有单一单位的尺寸组
    /// </summary>
    private static List<DimensionItem> ParseWithSingleUnit(string group, DimensionUnit targetUnit)
    {
        var items = new List<DimensionItem>();
        
        // 确定组的单位
        string detectedUnitStr = DetermineUnit(group);
        
        // 移除单位标记
        var cleaned = UnitPattern.Replace(group, string.Empty);
        // 移除英寸标记
        cleaned = cleaned.Replace("''", "").Replace("\"", "");
        
        // 提取所有数值和可能的维度标签
        var pattern = @"(\d+(?:\.\d+)?)\s*([WwHhLl]|Width|HEIGHT|Length)?";
        var matches = System.Text.RegularExpressions.Regex.Matches(cleaned, pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        
        foreach (System.Text.RegularExpressions.Match match in matches)
        {
            if (!match.Groups[1].Success || string.IsNullOrWhiteSpace(match.Groups[1].Value))
                continue;
                
            if (!decimal.TryParse(match.Groups[1].Value, out decimal value))
                continue;
                
            if (value == 0)
                continue;
                
            // 转换值到目标单位
            bool hasInchMarker = group.Contains("''") || group.Contains("\"");
            decimal convertedValue = ConvertValueToUnit(value, detectedUnitStr, hasInchMarker, targetUnit);
            
            // 检测维度类型
            DimensionKind kind = DimensionKind.Unspecified;
            if (match.Groups[2].Success && !string.IsNullOrEmpty(match.Groups[2].Value))
            {
                kind = ParseDimensionKind(match.Groups[2].Value);
            }
            
            items.Add(new DimensionItem(kind, convertedValue));
        }
        
        return items;
    }
    
    /// <summary>
    /// 将值转换到目标单位
    /// </summary>
    private static decimal ConvertValueToUnit(decimal value, string unitStr, bool hasInchMarker, DimensionUnit targetUnit)
    {
        // 如果有英寸标记
        if (hasInchMarker || ContainsIgnoreCase(unitStr, "inch", "in"))
        {
            if (targetUnit == DimensionUnit.Inch)
            {
                return value; // 保持英寸
            }
            else
            {
                return value * 2.54m; // 转换为厘米
            }
        }
        
        // 根据单位字符串转换
        if (ContainsIgnoreCase(unitStr, "cm"))
        {
            return value; // 已经是厘米
        }
        else if (ContainsIgnoreCase(unitStr, "mm"))
        {
            return value * 0.1m; // 毫米转厘米
        }
        else if (ContainsIgnoreCase(unitStr, "dm"))
        {
            return value * 10m; // 分米转厘米
        }
        else if (ContainsIgnoreCase(unitStr, "m") && !ContainsIgnoreCase(unitStr, "cm", "mm", "dm"))
        {
            return value * 100m; // 米转厘米
        }
        
        // 无单位或无法识别，默认为厘米
        return value;
    }
    
    /// <summary>
    /// 转换值到目标单位（根据输入字符串检测单位）
    /// </summary>
    private static decimal ConvertToTargetUnit(decimal value, string input, DimensionUnit targetUnit)
    {
        // 检测输入中的单位
        bool hasInchMarker = input.Contains("''") || input.Contains("\"");
        
        if (hasInchMarker || ContainsIgnoreCase(input, "inch", "in"))
        {
            if (targetUnit == DimensionUnit.Inch)
            {
                return value;
            }
            else
            {
                return value * 2.54m;
            }
        }
        
        // 其他单位处理
        if (ContainsIgnoreCase(input, "cm"))
        {
            return value;
        }
        else if (ContainsIgnoreCase(input, "mm"))
        {
            return value * 0.1m;
        }
        else if (ContainsIgnoreCase(input, "dm"))
        {
            return value * 10m;
        }
        else if (ContainsIgnoreCase(input, "m") && !ContainsIgnoreCase(input, "cm", "mm", "dm"))
        {
            return value * 100m;
        }
        
        return value;
    }
    
    /// <summary>
    /// 解析维度类型（宽、高、长）
    /// </summary>
    private static DimensionKind ParseDimensionKind(string label)
    {
        if (string.IsNullOrEmpty(label))
            return DimensionKind.Unspecified;
            
        label = label.ToUpperInvariant();
        
        if (label == "W" || label == "WIDTH")
            return DimensionKind.Width;
        else if (label == "H" || label == "HEIGHT")
            return DimensionKind.Height;
        else if (label == "L" || label == "LENGTH")
            return DimensionKind.Length;
            
        return DimensionKind.Unspecified;
    }
}
