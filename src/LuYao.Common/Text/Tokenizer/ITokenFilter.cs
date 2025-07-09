using System.Collections.Generic;

namespace LuYao.Text.Tokenizer;

public interface ITokenFilter
{
    IEnumerable<string> Filter(string text);
}
