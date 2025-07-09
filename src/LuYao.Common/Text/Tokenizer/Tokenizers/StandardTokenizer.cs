using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Text.Tokenizer.Tokenizers;

public class StandardTokenizer : ITokenizer
{
    public int Gram => 2;
    public char[] Separator { get; set; } = [' ', '_', '-', ',', ';', ':', '!', '?', '.', '"', '(', ')', '[', ']', '{', '}', '@', '*', '/', '\\', '\'', '&', '#', '%', '`', '^', '+', '<', '=', '>', '|', '~', '$'];

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
