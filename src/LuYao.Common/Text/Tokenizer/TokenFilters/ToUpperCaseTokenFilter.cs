using System.Collections.Generic;

namespace LuYao.Text.Tokenizer.TokenFilters;

/// <summary>
/// 将词条转换为大写的词条过滤器。
/// </summary>
public class ToUpperCaseTokenFilter : ITokenFilter
{
    /// <summary>
    /// 对输入文本进行过滤处理，将文本转换为大写后返回。
    /// </summary>
    /// <param name="text">要过滤的文本。</param>
    /// <returns>转换为大写的词条集合。</returns>
    public IEnumerable<string> Filter(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) yield break;
        else yield return text.ToUpperInvariant();
    }
}
