using System;

namespace LuYao.Text.Tokenizer.CharacterFilters;

/// <summary>
/// 转换半角字符为全角字符的字符过滤器。
/// </summary>
public class ToSbcCaseCharacterFilter : ICharacterFilter
{
    /// <summary>
    /// 对输入文本进行过滤处理，将半角字符转换为全角字符。
    /// </summary>
    /// <param name="text">要过滤的文本。</param>
    /// <returns>转换后的文本，其中半角字符已替换为全角字符。</returns>
    public string Filter(string text)
    {
        // 半角转全角：
        char[] c = text.ToCharArray();
        for (int i = 0; i < c.Length; i++)
        {
            // 如果字符是半角空格（Unicode值32），转换为全角空格（Unicode值12288）
            if (c[i] == 32)
            {
                c[i] = (char)12288;
                continue;
            }
            // 如果字符在半角字符范围内（Unicode值33到126），转换为对应的全角字符
            if (c[i] < 127)
                c[i] = (char)(c[i] + 65248);
        }
        return new String(c);
    }
}
