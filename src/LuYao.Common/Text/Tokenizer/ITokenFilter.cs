using System.Collections.Generic;

namespace LuYao.Text.Tokenizer;

/// <summary>
/// 词条过滤器接口，用于对分词后的词条集合进行进一步处理。
/// </summary>
public interface ITokenFilter
{
    /// <summary>
    /// 对输入文本进行词条过滤处理，返回处理后的词条集合。
    /// </summary>
    /// <param name="text">要过滤的文本。</param>
    /// <returns>过滤后的词条集合。</returns>
    IEnumerable<string> Filter(string text);
}
