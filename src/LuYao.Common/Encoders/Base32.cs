using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Encoders;

/// <summary>
/// 参见：https://www.crockford.com/base32.html
/// 由 ChatGPT 生成
/// </summary>
public static class Base32
{
    private const string Base32Alphabet = "0123456789ABCDEFGHJKMNPQRSTVWXYZ";
    private static readonly int[] AlphabetIndex = new int[256];

    static Base32()
    {
        for (int i = 0; i < AlphabetIndex.Length; i++)
        {
            AlphabetIndex[i] = -1; // 初始化为无效索引
        }

        for (int i = 0; i < Base32Alphabet.Length; i++)
        {
            char c = Base32Alphabet[i];
            AlphabetIndex[c] = i;
        }
    }

    /// <summary>
    /// 将字节数组编码为 Base32 字符串。
    /// </summary>
    /// <param name="bytes">要编码的字节数组。</param>
    /// <returns>Base32 编码后的字符串。</returns>
    public static string ToBase32(byte[] bytes)
    {
        if (bytes == null || bytes.Length == 0)
            return string.Empty;

        StringBuilder encoded = new StringBuilder();
        int bitCount = 0;
        int currentByte = 0;

        foreach (byte b in bytes)
        {
            currentByte = (currentByte << 8) | b;
            bitCount += 8;

            while (bitCount >= 5)
            {
                int index = (currentByte >> (bitCount - 5)) & 0x1F;
                encoded.Append(Base32Alphabet[index]);
                bitCount -= 5;
            }
        }

        if (bitCount > 0)
        {
            int index = (currentByte << (5 - bitCount)) & 0x1F;
            encoded.Append(Base32Alphabet[index]);
        }

        return encoded.ToString();
    }

    /// <summary>
    /// 将 Base32 字符串解码为字节数组。
    /// </summary>
    /// <param name="base32">要解码的 Base32 字符串。</param>
    /// <returns>解码后的字节数组。</returns>
    /// <exception cref="ArgumentException">当输入包含无效的 Base32 字符时抛出。</exception>
    public static byte[] FromBase32(string base32)
    {
        if (string.IsNullOrWhiteSpace(base32))
        {
#if NET45
            return new byte[0];
#else
            return Array.Empty<byte>();
#endif
        }

        byte[] bytes = new byte[base32.Length * 5 / 8];
        int bitCount = 0;
        int currentByte = 0;
        int byteIndex = 0;

        foreach (char c in base32)
        {
            int index = AlphabetIndex[c];
            if (index < 0)
                throw new ArgumentException("Invalid base32 character.", nameof(base32));

            currentByte = (currentByte << 5) | index;
            bitCount += 5;

            if (bitCount >= 8)
            {
                bytes[byteIndex++] = (byte)(currentByte >> (bitCount - 8));
                bitCount -= 8;
            }
        }

        return bytes;
    }
}
