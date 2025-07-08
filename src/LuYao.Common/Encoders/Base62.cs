using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Encoders;

/// <summary>
/// Base62 编码工具类，提供 Base62 编码与解码功能。
/// </summary>
public static class Base62
{
    private const string DefaultCharacterSet =
        "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    private const string InvertedCharacterSet =
        "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

    /// <summary>
    /// 使用 Base62 编码字节数组。
    /// </summary>
    /// <param name="original">要编码的字节数组。</param>
    /// <param name="inverted">是否使用反转字符集。</param>
    /// <returns>Base62 编码字符串。</returns>
    public static string ToBase62(byte[] original, bool inverted = false)
    {
        if (original == null || original.Length == 0) return string.Empty;
        var characterSet = inverted ? InvertedCharacterSet : DefaultCharacterSet;
        var arr = Array.ConvertAll(original, t => (int)t);

        var converted = BaseConvert(arr, 256, 62);
        var builder = new StringBuilder();
        foreach (var t in converted)
        {
            builder.Append(characterSet[t]);
        }
        return builder.ToString();
    }

    /// <summary>
    /// 解码 Base62 编码的字符串为字节数组。
    /// </summary>
    /// <param name="base62">Base62 编码字符串。</param>
    /// <param name="inverted">是否使用反转字符集。</param>
    /// <returns>解码后的字节数组。</returns>
    public static byte[] FromBase62(string base62, bool inverted = false)
    {
        if (string.IsNullOrWhiteSpace(base62)) throw new ArgumentNullException(nameof(base62));

        var characterSet = inverted ? InvertedCharacterSet : DefaultCharacterSet;
        var arr = Array.ConvertAll(base62.ToCharArray(), characterSet.IndexOf);

        var converted = BaseConvert(arr, 62, 256);
        return Array.ConvertAll(converted, Convert.ToByte);
    }

    private static int[] BaseConvert(int[] source, int sourceBase, int targetBase)
    {
        var result = new List<int>();
        var leadingZeroCount = Math.Min(source.TakeWhile(x => x == 0).Count(), source.Length - 1);
        int count;
        while ((count = source.Length) > 0)
        {
            var quotient = new List<int>();
            var remainder = 0;
            for (var i = 0; i != count; i++)
            {
                var accumulator = source[i] + remainder * sourceBase;
                var digit = accumulator / targetBase;
                remainder = accumulator % targetBase;
                if (quotient.Count > 0 || digit > 0)
                {
                    quotient.Add(digit);
                }
            }

            result.Insert(0, remainder);
            source = quotient.ToArray();
        }
        result.InsertRange(0, Enumerable.Repeat(0, leadingZeroCount));
        return result.ToArray();
    }
}
