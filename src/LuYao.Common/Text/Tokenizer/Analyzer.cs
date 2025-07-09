using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Text.Tokenizer;

public class Analyzer : IAnalyzer
{
    public ITokenizer Tokenizer { get; }

    public Analyzer(ITokenizer tokenizer, IEnumerable<ICharacterFilter> characterFilters, IEnumerable<ITokenFilter> tokenFilters)
    {
        Tokenizer = tokenizer;
        CharacterFilters = characterFilters?.ToArray() ?? Arrays.Empty<ICharacterFilter>();
        TokenFilters = tokenFilters?.ToArray() ?? Arrays.Empty<ITokenFilter>();
    }

    public IReadOnlyCollection<ICharacterFilter> CharacterFilters { get; }

    public IReadOnlyCollection<ITokenFilter> TokenFilters { get; }

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
