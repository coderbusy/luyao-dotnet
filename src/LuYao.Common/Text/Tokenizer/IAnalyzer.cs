using System.Collections.Generic;

namespace LuYao.Text.Tokenizer;

public interface IAnalyzer
{
    ITokenizer Tokenizer { get; }
    IReadOnlyCollection<ICharacterFilter> CharacterFilters { get; }
    IReadOnlyCollection<ITokenFilter> TokenFilters { get; }
    IEnumerable<string> Analyze(string? text);
}
