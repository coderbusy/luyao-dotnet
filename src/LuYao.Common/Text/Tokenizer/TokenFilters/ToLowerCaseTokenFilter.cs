using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Text.Tokenizer.TokenFilters;

/// <summary>
/// 将词条转换为小写的词条过滤器。
/// </summary>
public class ToLowerCaseTokenFilter : ITokenFilter
{
    /// <summary>
    /// 对输入文本进行过滤处理，将文本转换为小写后返回。
    /// </summary>
    /// <param name="text">要过滤的文本。</param>
    /// <returns>转换为小写的词条集合。</returns>
    public IEnumerable<string> Filter(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) yield break;
        else yield return text.ToLowerInvariant();
    }
}
