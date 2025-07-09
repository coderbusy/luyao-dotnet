using System;

namespace LuYao.Text.Tokenizer.CharacterFilters;

/// <summary>
/// 全角的函数(SBC case)
/// </summary>
public class ToSbcCaseCharacterFilter : ICharacterFilter
{
    public string Filter(string text)
    {
        // 半角转全角：
        char[] c = text.ToCharArray();
        for (int i = 0; i < c.Length; i++)
        {
            if (c[i] == 32)
            {
                c[i] = (char)12288;
                continue;
            }
            if (c[i] < 127)
                c[i] = (char)(c[i] + 65248);
        }
        return new String(c);
    }
}
