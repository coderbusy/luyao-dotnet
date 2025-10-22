using System;
using System.Globalization;
using System.Text;

namespace LuYao.Text;

/// <summary>
/// 提供字符串操作的实用工具方法。
/// </summary>
public static class StringHelper
{
    /// <summary>
    /// 表示字符串截断的结果。
    /// </summary>
    public readonly struct TruncateResult
    {
        /// <summary>
        /// 初始化 <see cref="TruncateResult"/> 结构的新实例。
        /// </summary>
        /// <param name="truncated">截断后的字符串</param>
        /// <param name="remaining">被截断的剩余字符串</param>
        /// <param name="wasTruncated">指示字符串是否被截断</param>
        public TruncateResult(string truncated, string remaining, bool wasTruncated)
        {
            Truncated = truncated;
            Remaining = remaining;
            WasTruncated = wasTruncated;
        }

        /// <summary>
        /// 获取截断后的字符串。
        /// </summary>
        public string Truncated { get; }

        /// <summary>
        /// 获取被截断的剩余字符串。
        /// </summary>
        public string Remaining { get; }

        /// <summary>
        /// 获取一个值，该值指示字符串是否被截断。
        /// </summary>
        public bool WasTruncated { get; }
    }

    /// <summary>
    /// 截断字符串到指定的最大长度，保留完整的单词或亚洲字符片段。
    /// </summary>
    /// <param name="text">要截断的字符串</param>
    /// <param name="maxLength">最大长度</param>
    /// <param name="suffix">截断后添加的后缀，默认为空字符串</param>
    /// <returns>截断结果，包含截断后的字符串和剩余字符串</returns>
    /// <exception cref="ArgumentNullException">当 text 为 null 时</exception>
    /// <exception cref="ArgumentOutOfRangeException">当 maxLength 小于 1 时</exception>
    /// <example>
    /// <code>
    /// var result = StringHelper.Truncate("Hello World", 8);
    /// Console.WriteLine(result.Truncated);  // "Hello"
    /// Console.WriteLine(result.Remaining);  // " World"
    /// 
    /// var result2 = StringHelper.Truncate("你好世界，这是一个测试", 6);
    /// Console.WriteLine(result2.Truncated);  // "你好世界"
    /// Console.WriteLine(result2.Remaining);  // "，这是一个测试"
    /// </code>
    /// </example>
    public static TruncateResult Truncate(string text, int maxLength, string suffix = "")
    {
        if (text == null)
            throw new ArgumentNullException(nameof(text));

        if (maxLength < 1)
            throw new ArgumentOutOfRangeException(nameof(maxLength), "最大长度必须大于或等于 1");

        if (suffix == null)
            suffix = string.Empty;

        // 如果文本长度小于等于最大长度，不需要截断
        if (text.Length <= maxLength)
            return new TruncateResult(text, string.Empty, false);

        // 计算实际可用长度（减去后缀长度）
        int effectiveMaxLength = maxLength - suffix.Length;
        if (effectiveMaxLength < 1)
            effectiveMaxLength = 1;

        // 找到合适的截断位置
        int truncatePos = FindTruncatePosition(text, effectiveMaxLength);

        // 截断字符串
        string truncated = text.Substring(0, truncatePos) + suffix;
        string remaining = text.Substring(truncatePos);

        return new TruncateResult(truncated, remaining, true);
    }

    /// <summary>
    /// 找到合适的截断位置，保留完整的单词或字符。
    /// </summary>
    private static int FindTruncatePosition(string text, int maxLength)
    {
        if (maxLength >= text.Length)
            return text.Length;

        // 如果没有找到合适的边界，使用最大长度作为后备
        int fallbackPos = maxLength;

        // 从最大长度位置开始向前查找合适的断点
        for (int i = maxLength; i > 0; i--)
        {
            if (IsTruncatePoint(text, i))
            {
                // 跳过尾部空白字符
                while (i > 0 && char.IsWhiteSpace(text[i - 1]))
                    i--;
                return i;
            }
        }

        // 如果找不到合适的边界，在最大长度处截断
        return fallbackPos;
    }

    /// <summary>
    /// 判断指定位置是否适合作为截断点。
    /// </summary>
    private static bool IsTruncatePoint(string text, int position)
    {
        if (position <= 0 || position >= text.Length)
            return true;

        char currentChar = text[position];
        char prevChar = text[position - 1];

        // 空白字符后面是截断点
        if (char.IsWhiteSpace(prevChar))
            return true;

        // 标点符号后面是截断点
        if (char.IsPunctuation(prevChar))
            return true;

        // 检查是否是亚洲字符
        bool currentIsAsian = IsAsianCharacter(currentChar);
        bool prevIsAsian = IsAsianCharacter(prevChar);

        // 亚洲字符和非亚洲字符之间是截断点
        if (currentIsAsian != prevIsAsian)
            return true;

        // 两个亚洲字符之间是截断点（亚洲字符通常每个字符都是一个完整的意义单元）
        if (currentIsAsian && prevIsAsian)
            return true;

        return false;
    }

    /// <summary>
    /// 判断字符是否是亚洲字符（CJK：中文、日文、韩文）。
    /// </summary>
    private static bool IsAsianCharacter(char c)
    {
        // CJK 统一表意文字
        // 基本区：U+4E00 - U+9FFF
        // 扩展 A：U+3400 - U+4DBF
        // 扩展 B-F：U+20000 - U+2EBEF
        if (c >= 0x4E00 && c <= 0x9FFF)
            return true;
        if (c >= 0x3400 && c <= 0x4DBF)
            return true;

        // 日文假名
        // 平假名：U+3040 - U+309F
        // 片假名：U+30A0 - U+30FF
        if (c >= 0x3040 && c <= 0x30FF)
            return true;

        // 韩文音节
        // 韩文音节：U+AC00 - U+D7AF
        if (c >= 0xAC00 && c <= 0xD7AF)
            return true;

        // CJK 兼容表意文字：U+F900 - U+FAFF
        if (c >= 0xF900 && c <= 0xFAFF)
            return true;

        // CJK 兼容形式：U+FE30 - U+FE4F
        if (c >= 0xFE30 && c <= 0xFE4F)
            return true;

        // 全角 ASCII、全角标点：U+FF00 - U+FFEF
        if (c >= 0xFF00 && c <= 0xFFEF)
            return true;

        return false;
    }
}
