using System;

namespace LuYao.Text.Tokenizer.CharacterFilters;

/// <summary>
/// 转半角的函数(DBC case)
/// </summary>
public class ToDbcCaseCharacterFilter : ICharacterFilter
{
    public string Filter(string input)
    {
        char[] c = input.ToCharArray();
        for (int i = 0; i < c.Length; i++)
        {
            if (c[i] == 12288)
            {
                c[i] = (char)32;
                continue;
            }
            if (c[i] > 65280 && c[i] < 65375)
            {
                c[i] = (char)(c[i] - 65248);
            }
        }
        return new String(c);
    }
}
