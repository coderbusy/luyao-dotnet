using System.Text.RegularExpressions;

namespace LuYao.Text.Tokenizer.CharacterFilters;

/// <summary>
/// 过滤文本中的ASCII字符，并在每个ASCII字符前后添加空格。
/// </summary>
public class WrapAsciiCharacterFilter : ICharacterFilter
{
    /// <summary>
    /// 过滤文本中的ASCII字符，并在每个ASCII字符前后添加空格。
    /// </summary>
    /// <param name="text">要过滤的文本</param>
    /// <returns>过滤后的文本</returns>
    public string Filter(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return string.Empty;
        return Regex.Replace(text, "[\x00-\x7F]+", " $0 ");
    }
}
