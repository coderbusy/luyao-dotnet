using System.Collections.Generic;
using System.Linq;

namespace LuYao.Text.Tokenizer;

/// <summary>
/// 分析器类，用于对文本进行分词、字符过滤和词条过滤。
/// </summary>
public class Analyzer : IAnalyzer
{
    /// <summary>
    /// 获取分词器，用于将输入文本分解为词条。
    /// </summary>
    public ITokenizer Tokenizer { get; }

    /// <summary>
    /// 初始化分析器实例。
    /// </summary>
    /// <param name="tokenizer">分词器实例。</param>
    /// <param name="characterFilters">字符过滤器集合。</param>
    /// <param name="tokenFilters">词条过滤器集合。</param>
    public Analyzer(ITokenizer tokenizer, IEnumerable<ICharacterFilter> characterFilters, IEnumerable<ITokenFilter> tokenFilters)
    {
        Tokenizer = tokenizer;
        CharacterFilters = characterFilters?.ToArray() ?? Arrays.Empty<ICharacterFilter>();
        TokenFilters = tokenFilters?.ToArray() ?? Arrays.Empty<ITokenFilter>();
    }

    /// <summary>
    /// 获取字符过滤器集合，用于在分词之前对文本进行预处理。
    /// </summary>
    public IReadOnlyCollection<ICharacterFilter> CharacterFilters { get; }

    /// <summary>
    /// 获取词条过滤器集合，用于在分词之后对词条进行处理。
    /// </summary>
    public IReadOnlyCollection<ITokenFilter> TokenFilters { get; }

    /// <summary>
    /// 对输入文本进行分析，返回处理后的词条集合。
    /// </summary>
    /// <param name="text">要分析的文本。</param>
    /// <returns>处理后的词条集合。</returns>
    public IEnumerable<string> Analyze(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            yield break;
        }
        else
        {
            var temp = text;
            foreach (var characterFilter in CharacterFilters) temp = characterFilter.Filter(temp);
            foreach (var token in this.Tokenizer.Tokenize(temp))
            {
                if (this.TokenFilters.Count == 0)
                {
                    yield return token;
                }
                else
                {
                    foreach (var tokenFilter in TokenFilters)
                    {
                        foreach (var str in tokenFilter.Filter(token))
                        {
                            yield return str;
                        }
                    }
                }
            }
        }
    }
}
