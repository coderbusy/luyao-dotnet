using System;
using System.Globalization;

namespace LuYao.Text;

/// <summary>
/// 提供字符串操作的实用工具方法。
/// </summary>
public static class StringHelper
{
    /// <summary>
    /// 截断字符串到指定的最大长度，保留完整的单词或亚洲字符片段。
    /// </summary>
    /// <param name="str">要截断的字符串</param>
    /// <param name="maxlen">最大长度</param>
    /// <param name="remain">被截断的剩余字符串</param>
    /// <returns>截断后的字符串</returns>
    /// <exception cref="ArgumentNullException">当 str 为 null 时</exception>
    /// <exception cref="ArgumentOutOfRangeException">当 maxlen 小于 1 时</exception>
    /// <example>
    /// <code>
    /// var truncated = StringHelper.Truncate("Hello World", 8, out string remain);
    /// Console.WriteLine(truncated);  // "Hello"
    /// Console.WriteLine(remain);     // " World"
    /// 
    /// var truncated2 = StringHelper.Truncate("你好世界，这是一个测试", 6, out string remain2);
    /// Console.WriteLine(truncated2);  // "你好世界"
    /// Console.WriteLine(remain2);     // "，这是一个测试"
    /// </code>
    /// </example>
    public static string Truncate(string str, int maxlen, out string remain)
    {
        if (str == null)
            throw new ArgumentNullException(nameof(str));

        if (maxlen < 1)
            throw new ArgumentOutOfRangeException(nameof(maxlen), "最大长度必须大于或等于 1");

        // 如果文本长度小于等于最大长度，不需要截断
        if (str.Length <= maxlen)
        {
            remain = string.Empty;
            return str;
        }

        // 找到合适的截断位置
        int truncatePos = FindTruncatePosition(str, maxlen);

        // 截断字符串
        string truncated = str.Substring(0, truncatePos);
        remain = str.Substring(truncatePos);

        return truncated;
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
