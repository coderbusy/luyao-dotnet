using System.Collections.Generic;

namespace LuYao.Text.Tokenizer.TokenFilters;

public class ToUpperCaseTokenFilter : ITokenFilter
{
    public IEnumerable<string> Filter(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) yield break;
        else yield return text.ToUpperInvariant();
    }
}
