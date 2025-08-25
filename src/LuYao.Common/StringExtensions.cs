using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LuYao;

/// <summary>
/// 提供一系列字符串操作的扩展方法，包括切片、查找、提取和哈希计算等功能。
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// 表示一个字符串中的切片区域，由起始位置和结束位置组成。
    /// </summary>
    public readonly struct FindSlice
    {
        /// <summary>
        /// 初始化 <see cref="FindSlice"/> 结构的新实例。
        /// </summary>
        /// <param name="start">切片的起始位置（包含）</param>
        /// <param name="end">切片的结束位置（包含）</param>
        public FindSlice(int start, int end)
        {
            Start = start;
            End = end;
        }

        /// <summary>
        /// 获取切片的起始位置。
        /// </summary>
        public int Start { get; }

        /// <summary>
        /// 获取切片的结束位置。
        /// </summary>
        public int End { get; }

        /// <summary>
        /// 获取一个值，该值指示切片是否有效（起始位置不等于结束位置且结束位置大于0）。
        /// </summary>
        public bool Success => Start != End && End > 0;

        /// <summary>
        /// 判断指定位置是否在切片范围内。
        /// </summary>
        /// <param name="x">要检查的位置</param>
        /// <returns>如果位置在切片范围内，则为 true；否则为 false。</returns>
        public bool Contains(int x)
        {
            return x >= Start && x <= End;
        }
    }

    /// <summary>
    /// 在字符串中查找所有指定开始和结束标记之间的区域，并返回这些区域的切片集合。
    /// </summary>
    /// <param name="value">要搜索的字符串</param>
    /// <param name="start">开始标记</param>
    /// <param name="end">结束标记</param>
    /// <param name="comparison">字符串比较方式，默认为忽略大小写</param>
    /// <returns>包含所有匹配区域的切片集合</returns>
    /// <example>
    /// <code>
    /// var html = "&lt;div&gt;内容1&lt;/div&gt;&lt;div&gt;内容2&lt;/div&gt;";
    /// var slices = html.FindAll("&lt;div&gt;", "&lt;/div&gt;");
    /// foreach(var slice in slices) {
    ///     Console.WriteLine(html.Substring(slice.Start, slice.End - slice.Start + 5));
    /// }
    /// </code>
    /// </example>
    public static IEnumerable<FindSlice> FindAll(
        this string value,
        string start,
        string end,
        StringComparison comparison = StringComparison.OrdinalIgnoreCase
    )
    {
        var s = 0;
        while (s < value.Length)
        {
            s = value.IndexOf(start, s, comparison);
            if (s < 0)
            {
                break;
            }

            var e = value.IndexOf(end, s + start.Length, comparison);
            if (e < 0)
            {
                break;
            }

            yield return new FindSlice(s, e + end.Length - 1);
            s = e + end.Length;
        }
    }

    /// <summary>
    /// 在字符串中查找第一个指定开始和结束标记之间的区域，并返回该区域的切片。
    /// </summary>
    /// <param name="value">要搜索的字符串</param>
    /// <param name="start">开始标记</param>
    /// <param name="end">结束标记</param>
    /// <returns>匹配区域的切片，如果未找到则返回默认值</returns>
    public static FindSlice Find(this string value, string start, string end)
    {
        return FindAll(value, start, end).FirstOrDefault();
    }

    /// <summary>
    /// 获取字符串的确定性哈希代码，确保在不同平台和运行时上获得相同的哈希值。
    /// 与 <see cref="string.GetHashCode"/> 不同，此方法在所有平台上都返回相同的值。
    /// </summary>
    /// <param name="value">要计算哈希码的字符串</param>
    /// <returns>字符串的确定性哈希码</returns>
    public static int GetDeterministicHashCode(this string value)
    {
        unchecked
        {
            int hash1 = (5381 << 16) + 5381;
            int hash2 = hash1;

            for (int i = 0; i < value.Length; i += 2)
            {
                hash1 = (hash1 << 5) + hash1 ^ value[i];
                if (i == value.Length - 1)
                    break;
                hash2 = (hash2 << 5) + hash2 ^ value[i + 1];
            }

            return hash1 + hash2 * 1566083941;
        }
    }

    /// <summary>
    /// 从字符串中提取指定范围的子字符串，支持负索引（类似Python切片）。
    /// </summary>
    /// <param name="str">源字符串</param>
    /// <param name="startIndex">开始索引（包含），负数表示从末尾开始计数</param>
    /// <param name="endIndex">结束索引（不包含），0表示到字符串末尾，负数表示从末尾开始计数</param>
    /// <returns>指定范围的子字符串</returns>
    /// <example>
    /// <code>
    /// var str = "Hello, world!";
    /// Console.WriteLine(str.Slice(0, 5));  // "Hello"
    /// Console.WriteLine(str.Slice(-6));    // "world!"
    /// Console.WriteLine(str.Slice(7, -1)); // "world"
    /// </code>
    /// </example>
    public static string Slice(this string str, int startIndex, int endIndex = 0)
    {
        if (startIndex < 0)
        {
            startIndex = str.Length + startIndex;
        }

        if (endIndex < 0)
        {
            endIndex = str.Length + endIndex;
        }

        if (endIndex == 0)
        {
            endIndex = str.Length;
        }

        var len = endIndex - startIndex;
        return str.Substring(startIndex, len);
    }
}
