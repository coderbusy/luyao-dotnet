using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuYao.Text.Tokenizer.Tokenizers;

namespace LuYao.Text.Tokenizer;

public class AnalyzerBuilder
{
    public ITokenizer Tokenizer { get; set; } = new StandardTokenizer();
    public List<ICharacterFilter> CharacterFilters { get; } = new List<ICharacterFilter>();
    public List<ITokenFilter> TokenFilters { get; } = new List<ITokenFilter>();

    public IAnalyzer Build()
    {
        return new Analyzer(this.Tokenizer, this.CharacterFilters, this.TokenFilters);
    }
}
