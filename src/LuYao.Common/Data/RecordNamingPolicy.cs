using System;
using System.Text;

namespace LuYao.Data;

/// <summary>
/// 控制 Record 映射时属性名与列名之间转换策略的抽象基类。
/// 对标 <see cref="System.Text.Json.JsonNamingPolicy"/>。
/// </summary>
public abstract class RecordNamingPolicy
{
    /// <summary>小驼峰命名策略，如 MyProperty → myProperty</summary>
    public static RecordNamingPolicy CamelCase { get; } = new CamelCasePolicy();

    /// <summary>小写蛇形命名策略，如 MyProperty → my_property</summary>
    public static RecordNamingPolicy SnakeCaseLower { get; } = new SnakeCasePolicy(false);

    /// <summary>大写蛇形命名策略，如 MyProperty → MY_PROPERTY</summary>
    public static RecordNamingPolicy SnakeCaseUpper { get; } = new SnakeCasePolicy(true);

    /// <summary>小写短横线命名策略，如 MyProperty → my-property</summary>
    public static RecordNamingPolicy KebabCaseLower { get; } = new KebabCasePolicy(false);

    /// <summary>大写短横线命名策略，如 MyProperty → MY-PROPERTY</summary>
    public static RecordNamingPolicy KebabCaseUpper { get; } = new KebabCasePolicy(true);

    /// <summary>
    /// 将属性名转换为目标列名。
    /// </summary>
    /// <param name="name">原始属性名，不可为 null。</param>
    /// <returns>转换后的列名。</returns>
    public abstract string ConvertName(string name);

    // ────────────────────────────────────────────────────────────────────────────
    // 分词工具：将 PascalCase / camelCase 拆分为单词列表。
    // 规则：
    //   1. 连续大写字母视为一个词（如 XML），最后一个大写字母若紧跟小写则归入下一个词。
    //   2. 数字单独成词或跟随前一词（连续数字不打断）。
    // ────────────────────────────────────────────────────────────────────────────
    internal static string[] SplitWords(string name)
    {
        if (string.IsNullOrEmpty(name)) return new string[0];

        var words = new System.Collections.Generic.List<string>();
        var sb = new StringBuilder();

        for (int i = 0; i < name.Length; i++)
        {
            char c = name[i];

            if (i == 0)
            {
                sb.Append(c);
                continue;
            }

            char prev = name[i - 1];
            bool isUpper = char.IsUpper(c);
            bool prevUpper = char.IsUpper(prev);
            bool nextLower = (i + 1 < name.Length) && char.IsLower(name[i + 1]);

            if (isUpper)
            {
                // 情形1：前一个字符是小写字母 → 新词开始（camelCase 或 PascalCase 边界）
                // 情形2：前一个字符是大写字母，且当前字符后面跟小写字母 → 新词开始（如 XMLParser: L→P边界）
                if (!prevUpper || nextLower)
                {
                    if (sb.Length > 0) { words.Add(sb.ToString()); sb.Clear(); }
                }
                sb.Append(c);
            }
            else
            {
                sb.Append(c);
            }
        }

        if (sb.Length > 0) words.Add(sb.ToString());
        return words.ToArray();
    }

    // ────────────────────────────────────────────────────────────────────────────
    // 内置实现
    // ────────────────────────────────────────────────────────────────────────────

    private sealed class CamelCasePolicy : RecordNamingPolicy
    {
        public override string ConvertName(string name)
        {
            if (string.IsNullOrEmpty(name)) return name;
            var words = SplitWords(name);
            if (words.Length == 0) return name;
            var sb = new StringBuilder();
            for (int i = 0; i < words.Length; i++)
            {
                var w = words[i];
                if (i == 0)
                    sb.Append(char.ToLowerInvariant(w[0])).Append(w.Substring(1).ToLowerInvariant());
                else
                    sb.Append(char.ToUpperInvariant(w[0])).Append(w.Substring(1).ToLowerInvariant());
            }
            return sb.ToString();
        }
    }

    private sealed class SnakeCasePolicy : RecordNamingPolicy
    {
        private readonly bool _upper;
        public SnakeCasePolicy(bool upper) { _upper = upper; }

        public override string ConvertName(string name)
        {
            if (string.IsNullOrEmpty(name)) return name;
            var words = SplitWords(name);
            var parts = new string[words.Length];
            for (int i = 0; i < words.Length; i++)
                parts[i] = _upper ? words[i].ToUpperInvariant() : words[i].ToLowerInvariant();
            return string.Join("_", parts);
        }
    }

    private sealed class KebabCasePolicy : RecordNamingPolicy
    {
        private readonly bool _upper;
        public KebabCasePolicy(bool upper) { _upper = upper; }

        public override string ConvertName(string name)
        {
            if (string.IsNullOrEmpty(name)) return name;
            var words = SplitWords(name);
            var parts = new string[words.Length];
            for (int i = 0; i < words.Length; i++)
                parts[i] = _upper ? words[i].ToUpperInvariant() : words[i].ToLowerInvariant();
            return string.Join("-", parts);
        }
    }
}
