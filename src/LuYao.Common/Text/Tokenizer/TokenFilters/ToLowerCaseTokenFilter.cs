using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Text.Tokenizer.TokenFilters;

public class ToLowerCaseTokenFilter : ITokenFilter
{
    public IEnumerable<string> Filter(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) yield break;
        else yield return text.ToLowerInvariant();
    }
}
