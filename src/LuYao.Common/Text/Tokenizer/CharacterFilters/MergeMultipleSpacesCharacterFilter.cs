using System.Text.RegularExpressions;

namespace LuYao.Text.Tokenizer.CharacterFilters;

/// <summary>
/// 合并多个连续空格的字符过滤器。
/// </summary>
public class MergeMultipleSpacesCharacterFilter : RegexCharacterFilter
{
    /// <summary>
    /// 初始化一个 <see cref="MergeMultipleSpacesCharacterFilter"/> 实例。
    /// 使用正则表达式匹配多个连续的空白字符，并将其替换为单个空格。
    /// </summary>
    public MergeMultipleSpacesCharacterFilter() : base(new Regex(@"\s+", RegexOptions.Compiled), " ")
    {

    }
}
