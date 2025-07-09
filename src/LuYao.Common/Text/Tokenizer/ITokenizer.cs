using System.Collections.Generic;

namespace LuYao.Text.Tokenizer;

public interface ITokenizer
{
    IEnumerable<string> Tokenize(string input);
}
