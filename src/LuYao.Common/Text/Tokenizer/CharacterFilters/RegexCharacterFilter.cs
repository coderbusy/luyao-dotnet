using System.Text.RegularExpressions;

namespace LuYao.Text.Tokenizer.CharacterFilters;

public class RegexCharacterFilter : ICharacterFilter
{
    public Regex Regex { get; }
    public string Replacement { get; }

    public RegexCharacterFilter(Regex regex, string replacement)
    {
        Regex = regex;
        Replacement = replacement;
    }

    public string Filter(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return string.Empty;
        return Regex.Replace(text, this.Replacement);
    }
}
