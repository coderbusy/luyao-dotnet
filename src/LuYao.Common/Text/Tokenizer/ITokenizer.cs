using System.Collections.Generic;

namespace LuYao.Text.Tokenizer;

/// <summary>
/// 分词器接口，用于将输入文本分解为词条集合。
/// </summary>
public interface ITokenizer
{
    /// <summary>
    /// 对输入文本进行分词处理，返回分解后的词条集合。
    /// </summary>
    /// <param name="input">要分词的输入文本。</param>
    /// <returns>分词后的词条集合。</returns>
    IEnumerable<string> Tokenize(string input);
}
