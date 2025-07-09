using System;

namespace LuYao.Text.Tokenizer.CharacterFilters;

/// <summary>
/// 转换全角字符为半角字符的字符过滤器。
/// </summary>
public class ToDbcCaseCharacterFilter : ICharacterFilter
{
    /// <summary>
    /// 对输入文本进行过滤处理，将全角字符转换为半角字符。
    /// </summary>
    /// <param name="input">要过滤的文本。</param>
    /// <returns>转换后的文本，其中全角字符已替换为半角字符。</returns>
    public string Filter(string input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;
        char[] c = input.ToCharArray();
        for (int i = 0; i < c.Length; i++)
        {
            // 如果字符是全角空格（Unicode值12288），转换为半角空格（Unicode值32）
            if (c[i] == 12288)
            {
                c[i] = (char)32;
                continue;
            }
            // 如果字符在全角字符范围内（Unicode值65281到65374），转换为对应的半角字符
            if (c[i] > 65280 && c[i] < 65375)
            {
                c[i] = (char)(c[i] - 65248);
            }
        }
        return new String(c);
    }
}
