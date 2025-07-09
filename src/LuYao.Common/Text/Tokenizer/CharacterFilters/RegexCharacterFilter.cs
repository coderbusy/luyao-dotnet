using System.Text.RegularExpressions;

namespace LuYao.Text.Tokenizer.CharacterFilters;

/// <summary>
/// 基于正则表达式的字符过滤器，用于通过正则表达式匹配并替换文本中的字符。
/// </summary>
public class RegexCharacterFilter : ICharacterFilter
{
    /// <summary>
    /// 获取用于匹配文本的正则表达式。
    /// </summary>
    public Regex Regex { get; }

    /// <summary>
    /// 获取替换匹配结果的字符串。
    /// </summary>
    public string Replacement { get; }

    /// <summary>
    /// 初始化一个 <see cref="RegexCharacterFilter"/> 实例。
    /// </summary>
    /// <param name="regex">用于匹配文本的正则表达式。</param>
    /// <param name="replacement">用于替换匹配结果的字符串。</param>
    public RegexCharacterFilter(Regex regex, string replacement)
    {
        Regex = regex;
        Replacement = replacement;
    }

    /// <summary>
    /// 对输入文本进行过滤处理，根据正则表达式匹配并替换文本中的字符。
    /// </summary>
    /// <param name="text">要过滤的文本。</param>
    /// <returns>过滤后的文本。</returns>
    public string Filter(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return string.Empty;
        return Regex.Replace(text, this.Replacement);
    }
}
