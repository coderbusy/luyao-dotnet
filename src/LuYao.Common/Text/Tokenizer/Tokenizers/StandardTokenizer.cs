using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Text.Tokenizer.Tokenizers;

/// <summary>
/// 标准分词器类，用于将输入文本分解为词条集合，支持基于分隔符和 n-gram 的分词方式。
/// </summary>
public class StandardTokenizer : ITokenizer
{
    /// <summary>
    /// 获取或设置 n-gram 的长度，用于生成固定长度的词条。
    /// 默认值为 2。
    /// </summary>
    public int Gram => 2;

    /// <summary>
    /// 获取或设置分隔符字符数组，用于分割输入文本。
    /// 默认包含常见的标点符号和空格。
    /// </summary>
    public char[] Separator { get; set; } = [' ', '_', '-', ',', ';', ':', '!', '?', '.', '"', '(', ')', '[', ']', '{', '}', '@', '*', '/', '\\', '\'', '&', '#', '%', '`', '^', '+', '<', '=', '>', '|', '~', '$'];

    /// <summary>
    /// 对输入文本进行分词处理，返回分解后的词条集合。
    /// </summary>
    /// <param name="input">要分词的输入文本。</param>
    /// <returns>分词后的词条集合。</returns>
    public IEnumerable<string> Tokenize(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            yield break;
        }
        else
        {
            var terms = input.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
            foreach (var term in terms)
            {
                if (IsAsciiLetterOrDigit(term))
                {
                    yield return term;
                }
                else if (term.Length <= Gram)
                {
                    yield return term;
                }
                else
                {
                    var total = term.Length - Gram;
                    for (var i = 0; i <= total; i++)
                    {
                        if (char.IsHighSurrogate(term[i]) && char.IsLowSurrogate(term[i + 1]))
                        {
                            yield return term.Substring(i, 2);
                            i++;
                        }
                        else
                        {
                            yield return term.Substring(i, Gram);
                        }
                    }
                }
            }
        }
    }

    private static bool IsAsciiLetterOrDigit(string str)
    {
        for (int i = 0; i < str.Length; i++)
        {
            if (IsAsciiLetterOrDigit(str[i]) == false) return false;
        }
        return true;
    }

    private static bool IsAsciiLetter(char c)
    {
        return (uint)((c | 0x20) - 97) <= 25u;
    }

    static bool IsBetween(char c, char minInclusive, char maxInclusive)
    {
        return (uint)(c - minInclusive) <= (uint)(maxInclusive - minInclusive);
    }

    private static bool IsAsciiLetterOrDigit(char c)
    {
        return IsAsciiLetter(c) || IsBetween(c, '0', '9');
    }
}
