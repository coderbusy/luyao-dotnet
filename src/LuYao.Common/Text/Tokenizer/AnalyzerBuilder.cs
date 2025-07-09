using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuYao.Text.Tokenizer.Tokenizers;

namespace LuYao.Text.Tokenizer;

/// <summary>
/// 分析器构建器类，用于动态构建分析器实例。
/// </summary>
public class AnalyzerBuilder
{
    /// <summary>
    /// 分词器，用于将输入文本分解为词条。
    /// 默认值为 <see cref="StandardTokenizer"/>。
    /// </summary>
    public ITokenizer Tokenizer { get; set; } = new StandardTokenizer();

    /// <summary>
    /// 字符过滤器集合，用于在分词之前对文本进行预处理。
    /// </summary>
    public List<ICharacterFilter> CharacterFilters { get; } = new List<ICharacterFilter>();

    /// <summary>
    /// 词条过滤器集合，用于在分词之后对词条进行处理。
    /// </summary>
    public List<ITokenFilter> TokenFilters { get; } = new List<ITokenFilter>();

    /// <summary>
    /// 构建分析器实例。
    /// </summary>
    /// <returns>构建的 <see cref="IAnalyzer"/> 实例。</returns>
    public IAnalyzer Build()
    {
        return new Analyzer(this.Tokenizer, this.CharacterFilters, this.TokenFilters);
    }
}
