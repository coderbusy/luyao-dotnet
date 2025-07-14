using System.Collections.Generic;
using System.Text;

namespace LuYao.Text.Tokenizer.CharacterFilters;

/// <summary>
/// 去除文本中重音字符的字符过滤器。
/// 将重音字符替换为对应的基本拉丁字母或其他替代形式。
/// </summary>
public class RemoveAccentsCharacterFilter : ICharacterFilter
{
    /// <summary>
    /// 对输入文本进行过滤处理，去除重音字符并返回处理后的文本。
    /// </summary>
    /// <param name="text">要过滤的文本。</param>
    /// <returns>去除重音字符后的文本。</returns>
    public string Filter(string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return string.Empty;

        StringBuilder sb = new StringBuilder(text.Length);
        foreach (char c in text)
        {
            // 如果字典中有对应的字符，使用映射值
            if (accentMap.ContainsKey(c))
            {
                sb.Append(accentMap[c]);
            }
            else
            {
                // 如果没有映射值，保留原字符
                sb.Append(c);
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// 定义重音字符到基本拉丁字母的映射字典。
    /// 包含常见的拉丁字母、汉语拼音声调等字符的映射。
    /// </summary>
    private static readonly IReadOnlyDictionary<char, string> accentMap = new Dictionary<char, string>()
    {
        // 拉丁字母（法语、西班牙语、德语等语言）
        // A系列
        {'À', "A"}, {'Á', "A"}, {'Â', "A"}, {'Ã', "A"}, // 法语、葡萄牙语
        {'Ä', "A"}, {'Å', "A"}, // 德语、瑞典语
        {'à', "a"}, {'á', "a"}, {'â', "a"}, {'ã', "a"}, // 法语、葡萄牙语
        {'ä', "a"}, {'å', "a"}, // 德语、瑞典语
        // E系列
        {'È', "E"}, {'É', "E"}, {'Ê', "E"}, {'Ë', "E"}, // 法语、德语
        {'è', "e"}, {'é', "e"}, {'ê', "e"}, {'ë', "e"}, // 法语、德语
        // I系列
        {'Ì', "I"}, {'Í', "I"}, {'Î', "I"}, {'Ï', "I"}, // 法语、意大利语
        {'ì', "i"}, {'í', "i"}, {'î', "i"}, {'ï', "i"}, // 法语、意大利语
        // O系列
        {'Ò', "O"}, {'Ó', "O"}, {'Ô', "O"}, {'Õ', "O"}, // 法语、葡萄牙语、意大利语
        {'Ö', "O"}, {'Ø', "O"}, // 德语、挪威语
        {'ò', "o"}, {'ó', "o"}, {'ô', "o"}, {'õ', "o"}, // 法语、葡萄牙语、意大利语
        {'ö', "o"}, {'ø', "o"}, // 德语、挪威语
        // U系列
        {'Ù', "U"}, {'Ú', "U"}, {'Û', "U"}, {'Ü', "U"}, // 法语、德语、意大利语
        {'ù', "u"}, {'ú', "u"}, {'û', "u"}, {'ü', "u"}, // 法语、德语、意大利语
        // Y系列
        {'Ý', "Y"}, {'Ÿ', "Y"}, // 法语、冰岛语
        {'ý', "y"}, {'ÿ', "y"}, // 法语
        // C系列
        {'Ç', "C"}, {'ç', "c"}, // 法语、葡萄牙语、土耳其语
        // N系列
        {'Ñ', "N"}, {'ñ', "n"}, // 西班牙语
        // 其他
        {'Æ', "AE"}, {'æ', "ae"}, // 法语、德语、挪威语
        {'Œ', "OE"}, {'œ', "oe"}, // 法语
        {'ß', "ss"}, // 德语
        {'Þ', "Th"}, {'þ', "th"}, // 冰岛语
        {'Š', "S"}, {'š', "s"}, // 捷克语、斯洛伐克语
        {'Ž', "Z"}, {'ž', "z"}, // 捷克语、斯洛伐克语
        {'Ł', "L"}, {'ł', "l"}, // 波兰语

        // 汉语拼音（拼音声调）只补充之前没有出现过的。
        {'ā', "a"},  {'ǎ', "a"},
        {'ē', "e"},  {'ě', "e"},
        {'ī', "i"},{'ǐ', "i"},
        {'ō', "o"}, {'ǒ', "o"},
        {'ū', "u"},  {'ǔ', "u"},
        {'ǖ', "u"}, {'ǘ', "u"}, {'ǚ', "u"}, {'ǜ', "u"},
    };
}
