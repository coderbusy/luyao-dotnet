using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Encoders;

/// <summary>
/// 提供字符串压缩和解压缩的功能。
/// </summary>
public class LzString
{
    private static readonly string keyStrBase64 =
        "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";

    private static readonly string keyStrUriSafe =
        "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+-$";

    private static Dictionary<string, Dictionary<char, int>> baseReverseDic =
        new Dictionary<string, Dictionary<char, int>>();

    private static readonly GetCharFromInt f = a => Convert.ToChar(a);
    private static readonly object lockObj = new object();

    private static int GetBaseValue(string alphabet, char character)
    {
        if (!baseReverseDic.ContainsKey(alphabet))
        {
            lock (lockObj)
            {
                if (!baseReverseDic.ContainsKey(alphabet))
                {
                    var copy = new Dictionary<string, Dictionary<char, int>>(baseReverseDic);
                    var charMap = new Dictionary<char, int>();
                    for (var i = 0; i < alphabet.Length; i++)
                    {
                        charMap[alphabet[i]] = i;
                    }
                    copy[alphabet] = charMap;
                    baseReverseDic = copy;
                }
            }
        }

        return baseReverseDic[alphabet][character];
    }

    /// <summary>
    /// 将字符串压缩为 Base64 编码格式。
    /// </summary>
    /// <param name="input">要压缩的字符串。</param>
    /// <returns>Base64 编码的压缩字符串。</returns>
    public static string CompressToBase64(string input)
    {
        if (input == null)
        {
            return string.Empty;
        }

        var res = Compress(input, 6, a => keyStrBase64[a]);
        switch (res.Length % 4)
        {
            case 0:
                return res;
            case 1:
                return res + "===";
            case 2:
                return res + "==";
            case 3:
                return res + "=";
        }

        return null;
    }

    /// <summary>
    /// 从 Base64 编码的压缩字符串解压缩为原始字符串。
    /// </summary>
    /// <param name="input">Base64 编码的压缩字符串。</param>
    /// <returns>解压缩后的原始字符串。</returns>
    public static string DecompressFromBase64(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        return Decompress(input.Length, 32, index => GetBaseValue(keyStrBase64, input[index]));
    }

    /// <summary>
    /// 将字符串压缩为 UTF16 编码格式。
    /// </summary>
    /// <param name="input">要压缩的字符串。</param>
    /// <returns>UTF16 编码的压缩字符串。</returns>
    public static string CompressToUTF16(string input)
    {
        if (input == null)
        {
            return string.Empty;
        }

        return Compress(input, 15, a => f(a + 32)) + " ";
    }

    /// <summary>
    /// 从 UTF16 编码的压缩字符串解压缩为原始字符串。
    /// </summary>
    /// <param name="compressed">UTF16 编码的压缩字符串。</param>
    /// <returns>解压缩后的原始字符串。</returns>
    public static string DecompressFromUTF16(string compressed)
    {
        if (string.IsNullOrWhiteSpace(compressed))
        {
            return string.Empty;
        }

        return Decompress(
            compressed.Length,
            16384,
            index => Convert.ToInt32(compressed[index]) - 32
        );
    }

    /// <summary>
    /// 将字符串压缩为字节数组（Uint8Array）。
    /// </summary>
    /// <param name="uncompressed">要压缩的字符串。</param>
    /// <returns>压缩后的字节数组。</returns>
    public static byte[] CompressToUint8Array(string uncompressed)
    {
        var compressed = Compress(uncompressed);
        var buf = new byte[compressed.Length * 2];

        for (int i = 0, TotalLen = compressed.Length; i < TotalLen; i++)
        {
            var current_value = Convert.ToInt32(compressed[i]);
            buf[i * 2] = (byte)((uint)current_value >> 8);
            buf[i * 2 + 1] = (byte)(current_value % 256);
        }

        return buf;
    }

    /// <summary>
    /// 从字节数组（Uint8Array）解压缩为原始字符串。
    /// </summary>
    /// <param name="compressed">压缩后的字节数组。</param>
    /// <returns>解压缩后的原始字符串。</returns>
    public static string DecompressFromUint8Array(byte[] compressed)
    {
        if (compressed == null)
        {
            return string.Empty;
        }

        var buf = new int[compressed.Length / 2];
        for (int i = 0, TotalLen = buf.Length; i < TotalLen; i++)
        {
            buf[i] = compressed[i * 2] * 256 + compressed[i * 2 + 1];
        }

        var result = new char[buf.Length];
        for (var i = 0; i < buf.Length; i++)
        {
            result[i] = f(buf[i]);
        }

        return Decompress(new string(result));
    }

    /// <summary>
    /// 将字符串压缩为可安全用于 URI 的编码格式。
    /// </summary>
    /// <param name="input">要压缩的字符串。</param>
    /// <returns>可用于 URI 的压缩字符串。</returns>
    public static string CompressToEncodedURIComponent(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        return Compress(input, 6, a => keyStrUriSafe[a]);
    }

    /// <summary>
    /// 从 URI 编码的压缩字符串解压缩为原始字符串。
    /// </summary>
    /// <param name="input">URI 编码的压缩字符串。</param>
    /// <returns>解压缩后的原始字符串。</returns>
    public static string DecompressFromEncodedURIComponent(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        input = input.Replace(' ', '+');
        return Decompress(input.Length, 32, index => GetBaseValue(keyStrUriSafe, input[index]));
    }

    /// <summary>
    /// 使用默认参数将字符串压缩为 LZ 格式。
    /// </summary>
    /// <param name="uncompressed">要压缩的字符串。</param>
    /// <returns>压缩后的字符串。</returns>
    public static string Compress(string uncompressed)
    {
        return Compress(uncompressed, 16, f);
    }

    private static string Compress(
        string uncompressed,
        int bitsPerChar,
        GetCharFromInt getCharFromInt
    )
    {
        if (uncompressed == null)
        {
            return string.Empty;
        }

        int i,
            value,
            ii,
            context_enlargeIn = 2,
            context_dictSize = 3,
            context_numBits = 2,
            context_data_val = 0,
            context_data_position = 0;
        var context_dictionaryToCreate = new Dictionary<string, bool>();
        var context_dictionary = new Dictionary<string, int>();
        var context_data = new StringBuilder();
        var context_c = string.Empty;
        string context_wc = string.Empty,
            context_w = string.Empty;

        for (ii = 0; ii < uncompressed.Length; ii++)
        {
            context_c = uncompressed[ii].ToString();
            if (!context_dictionary.ContainsKey(context_c))
            {
                context_dictionary[context_c] = context_dictSize++;
                context_dictionaryToCreate[context_c] = true;
            }

            context_wc = context_w + context_c;
            if (context_dictionary.ContainsKey(context_wc))
            {
                context_w = context_wc;
            }
            else
            {
                if (context_dictionaryToCreate.ContainsKey(context_w))
                {
                    if (Convert.ToInt32(context_w[0]) < 256)
                    {
                        for (i = 0; i < context_numBits; i++)
                        {
                            context_data_val = context_data_val << 1;
                            if (context_data_position == bitsPerChar - 1)
                            {
                                context_data_position = 0;
                                context_data.Append(getCharFromInt(context_data_val));
                                context_data_val = 0;
                            }
                            else
                            {
                                context_data_position++;
                            }
                        }

                        value = Convert.ToInt32(context_w[0]);
                        for (i = 0; i < 8; i++)
                        {
                            context_data_val = context_data_val << 1 | value & 1;
                            if (context_data_position == bitsPerChar - 1)
                            {
                                context_data_position = 0;
                                context_data.Append(getCharFromInt(context_data_val));
                                context_data_val = 0;
                            }
                            else
                            {
                                context_data_position++;
                            }

                            value = value >> 1;
                        }
                    }
                    else
                    {
                        value = 1;
                        for (i = 0; i < context_numBits; i++)
                        {
                            context_data_val = context_data_val << 1 | value;
                            if (context_data_position == bitsPerChar - 1)
                            {
                                context_data_position = 0;
                                context_data.Append(getCharFromInt(context_data_val));
                                context_data_val = 0;
                            }
                            else
                            {
                                context_data_position++;
                            }

                            value = 0;
                        }

                        value = Convert.ToInt32(context_w[0]);
                        for (i = 0; i < 16; i++)
                        {
                            context_data_val = context_data_val << 1 | value & 1;
                            if (context_data_position == bitsPerChar - 1)
                            {
                                context_data_position = 0;
                                context_data.Append(getCharFromInt(context_data_val));
                                context_data_val = 0;
                            }
                            else
                            {
                                context_data_position++;
                            }

                            value = value >> 1;
                        }
                    }

                    context_enlargeIn--;
                    if (context_enlargeIn == 0)
                    {
                        context_enlargeIn = (int)Math.Pow(2, context_numBits);
                        context_numBits++;
                    }

                    context_dictionaryToCreate.Remove(context_w);
                }
                else
                {
                    value = context_dictionary[context_w];
                    for (i = 0; i < context_numBits; i++)
                    {
                        context_data_val = context_data_val << 1 | value & 1;
                        if (context_data_position == bitsPerChar - 1)
                        {
                            context_data_position = 0;
                            context_data.Append(getCharFromInt(context_data_val));
                            context_data_val = 0;
                        }
                        else
                        {
                            context_data_position++;
                        }

                        value = value >> 1;
                    }
                }

                context_enlargeIn--;
                if (context_enlargeIn == 0)
                {
                    context_enlargeIn = (int)Math.Pow(2, context_numBits);
                    context_numBits++;
                }

                //Add wc to the dictionary
                context_dictionary[context_wc] = context_dictSize++;
                context_w = context_c;
            }
        }

        //Output the code for w
        if (context_w != string.Empty)
        {
            if (context_dictionaryToCreate.ContainsKey(context_w))
            {
                if (Convert.ToInt32(context_w[0]) < 256)
                {
                    for (i = 0; i < context_numBits; i++)
                    {
                        context_data_val = context_data_val << 1;
                        if (context_data_position == bitsPerChar - 1)
                        {
                            context_data_position = 0;
                            context_data.Append(getCharFromInt(context_data_val));
                            context_data_val = 0;
                        }
                        else
                        {
                            context_data_position++;
                        }
                    }

                    value = Convert.ToInt32(context_w[0]);
                    for (i = 0; i < 8; i++)
                    {
                        context_data_val = context_data_val << 1 | value & 1;
                        if (context_data_position == bitsPerChar - 1)
                        {
                            context_data_position = 0;
                            context_data.Append(getCharFromInt(context_data_val));
                            context_data_val = 0;
                        }
                        else
                        {
                            context_data_position++;
                        }

                        value = value >> 1;
                    }
                }
                else
                {
                    value = 1;
                    for (i = 0; i < context_numBits; i++)
                    {
                        context_data_val = context_data_val << 1 | value;
                        if (context_data_position == bitsPerChar - 1)
                        {
                            context_data_position = 0;
                            context_data.Append(getCharFromInt(context_data_val));
                            context_data_val = 0;
                        }
                        else
                        {
                            context_data_position++;
                        }

                        value = 0;
                    }

                    value = Convert.ToInt32(context_w[0]);
                    for (i = 0; i < 16; i++)
                    {
                        context_data_val = context_data_val << 1 | value & 1;
                        if (context_data_position == bitsPerChar - 1)
                        {
                            context_data_position = 0;
                            context_data.Append(getCharFromInt(context_data_val));
                            context_data_val = 0;
                        }
                        else
                        {
                            context_data_position++;
                        }

                        value = value >> 1;
                    }
                }

                context_enlargeIn--;
                if (context_enlargeIn == 0)
                {
                    context_enlargeIn = (int)Math.Pow(2, context_numBits);
                    context_numBits++;
                }

                context_dictionaryToCreate.Remove(context_w);
            }
            else
            {
                value = context_dictionary[context_w];
                for (i = 0; i < context_numBits; i++)
                {
                    context_data_val = context_data_val << 1 | value & 1;
                    if (context_data_position == bitsPerChar - 1)
                    {
                        context_data_position = 0;
                        context_data.Append(getCharFromInt(context_data_val));
                        context_data_val = 0;
                    }
                    else
                    {
                        context_data_position++;
                    }

                    value = value >> 1;
                }
            }

            context_enlargeIn--;
            if (context_enlargeIn == 0)
            {
                context_enlargeIn = (int)Math.Pow(2, context_numBits);
                context_numBits++;
            }
        }

        //Mark the end of the stream
        value = 2;
        for (i = 0; i < context_numBits; i++)
        {
            context_data_val = context_data_val << 1 | value & 1;
            if (context_data_position == bitsPerChar - 1)
            {
                context_data_position = 0;
                context_data.Append(getCharFromInt(context_data_val));
                context_data_val = 0;
            }
            else
            {
                context_data_position++;
            }

            value = value >> 1;
        }

        //Flush the last char
        while (true)
        {
            context_data_val = context_data_val << 1;
            if (context_data_position == bitsPerChar - 1)
            {
                context_data.Append(getCharFromInt(context_data_val));
                break;
            }

            context_data_position++;
        }

        return context_data.ToString();
    }

    /// <summary>
    /// 解压缩 LZ 格式的字符串为原始字符串。
    /// </summary>
    /// <param name="compressed">压缩后的字符串。</param>
    /// <returns>解压缩后的原始字符串。</returns>
    public static string Decompress(string compressed)
    {
        if (string.IsNullOrWhiteSpace(compressed))
        {
            return string.Empty;
        }

        return Decompress(compressed.Length, 32768, index => Convert.ToInt32(compressed[index]));
    }

    private static string Decompress(int length, int resetValue, GetNextValue getNextValue)
    {
        var dictionary = new Dictionary<int, string>();
        int next,
            enlargeIn = 4,
            dictSize = 4,
            numBits = 3,
            i,
            bits,
            resb,
            maxpower,
            power;
        var c = 0;
        string entry = string.Empty,
            w;
        var result = new StringBuilder();
        var data = new DataStruct
        {
            val = getNextValue(0),
            position = resetValue,
            index = 1
        };

        for (i = 0; i < 3; i++)
        {
            dictionary[i] = Convert.ToChar(i).ToString();
        }

        bits = 0;
        maxpower = (int)Math.Pow(2, 2);
        power = 1;
        while (power != maxpower)
        {
            resb = data.val & data.position;
            data.position >>= 1;
            if (data.position == 0)
            {
                data.position = resetValue;
                data.val = getNextValue(data.index++);
            }

            bits |= (resb > 0 ? 1 : 0) * power;
            power <<= 1;
        }

        switch (next = bits)
        {
            case 0:
                bits = 0;
                maxpower = (int)Math.Pow(2, 8);
                power = 1;
                while (power != maxpower)
                {
                    resb = data.val & data.position;
                    data.position >>= 1;
                    if (data.position == 0)
                    {
                        data.position = resetValue;
                        data.val = getNextValue(data.index++);
                    }

                    bits |= (resb > 0 ? 1 : 0) * power;
                    power <<= 1;
                }

                c = Convert.ToInt32(f(bits));
                break;
            case 1:
                bits = 0;
                maxpower = (int)Math.Pow(2, 16);
                power = 1;
                while (power != maxpower)
                {
                    resb = data.val & data.position;
                    data.position >>= 1;
                    if (data.position == 0)
                    {
                        data.position = resetValue;
                        data.val = getNextValue(data.index++);
                    }

                    bits |= (resb > 0 ? 1 : 0) * power;
                    power <<= 1;
                }

                c = Convert.ToInt32(f(bits));
                break;
            case 2:
                return string.Empty;
        }

        dictionary[3] = Convert.ToChar(c).ToString();
        w = Convert.ToChar(c).ToString();
        result.Append(Convert.ToChar(c));
        while (true)
        {
            if (data.index > length)
            {
                return string.Empty;
            }

            bits = 0;
            maxpower = (int)Math.Pow(2, numBits);
            power = 1;
            while (power != maxpower)
            {
                resb = data.val & data.position;
                data.position >>= 1;
                if (data.position == 0)
                {
                    data.position = resetValue;
                    data.val = getNextValue(data.index++);
                }

                bits |= (resb > 0 ? 1 : 0) * power;
                power <<= 1;
            }

            switch (c = bits)
            {
                case 0:
                    bits = 0;
                    maxpower = (int)Math.Pow(2, 8);
                    power = 1;
                    while (power != maxpower)
                    {
                        resb = data.val & data.position;
                        data.position >>= 1;
                        if (data.position == 0)
                        {
                            data.position = resetValue;
                            data.val = getNextValue(data.index++);
                        }

                        bits |= (resb > 0 ? 1 : 0) * power;
                        power <<= 1;
                    }

                    dictionary[dictSize++] = f(bits).ToString();
                    c = dictSize - 1;
                    enlargeIn--;
                    break;
                case 1:
                    bits = 0;
                    maxpower = (int)Math.Pow(2, 16);
                    power = 1;
                    while (power != maxpower)
                    {
                        resb = data.val & data.position;
                        data.position >>= 1;
                        if (data.position == 0)
                        {
                            data.position = resetValue;
                            data.val = getNextValue(data.index++);
                        }

                        bits |= (resb > 0 ? 1 : 0) * power;
                        power <<= 1;
                    }

                    dictionary[dictSize++] = f(bits).ToString();
                    c = dictSize - 1;
                    enlargeIn--;
                    break;
                case 2:
                    return result.ToString();
            }

            if (enlargeIn == 0)
            {
                enlargeIn = (int)Math.Pow(2, numBits);
                numBits++;
            }

            if (dictionary.ContainsKey(c))
            {
                entry = dictionary[c];
            }
            else
            {
                if (c == dictSize)
                {
                    entry = w + w[0];
                }
                else
                {
                    return null;
                }
            }

            result.Append(entry);

            //Add w+entry[0] to the dictionary.
            dictionary[dictSize++] = w + entry[0];
            enlargeIn--;
            w = entry;
            if (enlargeIn == 0)
            {
                enlargeIn = (int)Math.Pow(2, numBits);
                numBits++;
            }
        }
    }

    private delegate char GetCharFromInt(int a);

    private delegate int GetNextValue(int index);

    private struct DataStruct
    {
        public int val,
            position,
            index;
    }
}
