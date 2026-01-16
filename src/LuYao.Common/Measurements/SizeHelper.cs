using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace LuYao.Measurements;

/// <summary>
/// 尺寸解析帮助类，用于从字符串中提取尺寸信息。
/// </summary>
public static class SizeHelper
{
    private static readonly Dictionary<string, DimensionUnit> UnitMappings = new(StringComparer.OrdinalIgnoreCase)
    {
        { "cm", DimensionUnit.Centimeter },
        { "centimeter", DimensionUnit.Centimeter },
        { "centimeters", DimensionUnit.Centimeter },
        { "厘米", DimensionUnit.Centimeter },
        { "in", DimensionUnit.Inch },
        { "inch", DimensionUnit.Inch },
        { "inches", DimensionUnit.Inch },
        { "\"", DimensionUnit.Inch },
        { "''", DimensionUnit.Inch },
        { "'", DimensionUnit.Inch },
        { "英寸", DimensionUnit.Inch }
    };

    private static readonly Dictionary<string, DimensionKind> KindMappings = new(StringComparer.OrdinalIgnoreCase)
    {
        { "w", DimensionKind.Width },
        { "width", DimensionKind.Width },
        { "宽", DimensionKind.Width },
        { "h", DimensionKind.Height },
        { "height", DimensionKind.Height },
        { "高", DimensionKind.Height },
        { "d", DimensionKind.Depth },
        { "depth", DimensionKind.Depth },
        { "深", DimensionKind.Depth }
    };

    /// <summary>
    /// 从字符串中提取尺寸信息。
    /// </summary>
    /// <param name="input">包含尺寸信息的字符串，例如 "1x2x3"、"10cmx20cmx30cm"、"10''W X 36''H"、"10cmx10cmx10cm(3.94x3.94x3.94in)"。</param>
    /// <returns>包含解析结果的列表。如果输入包含多组尺寸（如括号内的尺寸），将返回多个结果。</returns>
    public static List<DimensionParseResult> Extract(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return new List<DimensionParseResult>();
        }

        var results = new List<DimensionParseResult>();

        // 检查是否有括号，如 "10cmx10cmx10cm(3.94x3.94x3.94in)"
        var parenthesesPattern = @"^(.+?)\((.+?)\)$";
        var parenthesesMatch = Regex.Match(input.Trim(), parenthesesPattern);

        if (parenthesesMatch.Success)
        {
            // 解析括号外的部分
            var outerPart = parenthesesMatch.Groups[1].Value.Trim();
            var outerResult = ParseSingleDimension(outerPart);
            if (outerResult != null)
            {
                results.Add(outerResult);
            }

            // 解析括号内的部分
            var innerPart = parenthesesMatch.Groups[2].Value.Trim();
            var innerResult = ParseSingleDimension(innerPart);
            if (innerResult != null)
            {
                results.Add(innerResult);
            }
        }
        else
        {
            // 没有括号，直接解析
            var result = ParseSingleDimension(input.Trim());
            if (result != null)
            {
                results.Add(result);
            }
        }

        return results;
    }

    private static DimensionParseResult? ParseSingleDimension(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return null;
        }

        // 尝试匹配带有单位标记的模式，如 "10''W X 36''H" 或 "10cmW x 20cmH" 或 "10cmx20cmx30cm"
        // 支持多种格式：数字 + 可选单位 + 可选维度类型标记（W/H/D）
        var dimensionPattern = @"(\d+(?:\.\d+)?)\s*(?:(['\""]{1,2})|([cC][mM])|([iI][nN])|厘米|英寸)?\s*([wWhHdD]?)";
        var matches = Regex.Matches(input, dimensionPattern);

        if (matches.Count > 0)
        {
            var dimensions = new List<DimensionValue>();
            DimensionUnit? detectedUnit = null;

            foreach (Match match in matches)
            {
                if (!match.Success || string.IsNullOrWhiteSpace(match.Groups[1].Value))
                {
                    continue;
                }

                var valueStr = match.Groups[1].Value;
                // 组合所有可能的单位组（组2-4）
                var unitStr = "";
                for (int i = 2; i <= 4; i++)
                {
                    if (match.Groups[i].Success && !string.IsNullOrWhiteSpace(match.Groups[i].Value))
                    {
                        unitStr = match.Groups[i].Value.Trim();
                        break;
                    }
                }
                var kindStr = match.Groups[5].Value.Trim();

                if (!decimal.TryParse(valueStr, NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
                {
                    continue;
                }

                // 确定尺寸单位
                DimensionUnit? currentUnit = null;
                if (!string.IsNullOrWhiteSpace(unitStr))
                {
                    if (UnitMappings.TryGetValue(unitStr, out var mappedUnit))
                    {
                        currentUnit = mappedUnit;
                    }
                }

                // 如果检测到单位，保存它
                if (currentUnit.HasValue)
                {
                    if (!detectedUnit.HasValue)
                    {
                        detectedUnit = currentUnit.Value;
                    }
                    else if (detectedUnit.Value != currentUnit.Value)
                    {
                        // 单位不一致的情况，保持第一个检测到的单位
                        // 这里可以根据需求调整逻辑
                    }
                }

                // 确定尺寸类型
                var kind = DimensionKind.Unspecified;
                if (!string.IsNullOrWhiteSpace(kindStr))
                {
                    if (KindMappings.TryGetValue(kindStr, out var mappedKind))
                    {
                        kind = mappedKind;
                    }
                }

                dimensions.Add(new DimensionValue(value, kind));
            }

            if (dimensions.Count > 0)
            {
                // 如果没有检测到单位，默认为厘米
                var finalUnit = detectedUnit ?? DimensionUnit.Centimeter;
                return new DimensionParseResult(finalUnit, dimensions);
            }
        }

        return null;
    }
}
