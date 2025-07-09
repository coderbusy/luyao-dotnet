using System.Collections.Generic;

namespace LuYao.Text.Tokenizer;

/// <summary>
/// 分析器接口，用于对文本进行分词、字符过滤和词条过滤。
/// </summary>
public interface IAnalyzer
{
    /// <summary>
    /// 获取分词器，用于将输入文本分解为词条。
    /// </summary>
    ITokenizer Tokenizer { get; }

    /// <summary>
    /// 获取字符过滤器集合，用于在分词之前对文本进行预处理。
    /// </summary>
    IReadOnlyCollection<ICharacterFilter> CharacterFilters { get; }

    /// <summary>
    /// 获取词条过滤器集合，用于在分词之后对词条进行处理。
    /// </summary>
    IReadOnlyCollection<ITokenFilter> TokenFilters { get; }

    /// <summary>
    /// 对输入文本进行分析，返回处理后的词条集合。
    /// </summary>
    /// <param name="text">要分析的文本。</param>
    /// <returns>处理后的词条集合。</returns>
    IEnumerable<string> Analyze(string text);
}