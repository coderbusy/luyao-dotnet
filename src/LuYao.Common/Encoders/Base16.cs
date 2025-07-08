using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Encoders;

/// <summary>
/// 提供 Base16（十六进制）编码与解码的静态方法。
/// </summary>
public static class Base16
{
    /// <summary>
    /// 将字节数组编码为 Base16（十六进制）字符串。
    /// </summary>
    /// <param name="original">要编码的字节数组。</param>
    /// <returns>编码后的十六进制字符串。</returns>
    public static string ToBase16(byte[] original)
    {
        var sb = new StringBuilder();

        foreach (var t in original)
            sb.Append(t.ToString("X2"));

        return sb.ToString();
    }

    /// <summary>
    /// 将 Base16（十六进制）字符串解码为字节数组。
    /// </summary>
    /// <param name="base16">要解码的十六进制字符串。</param>
    /// <returns>解码后的字节数组。</returns>
    public static byte[] FromBase16(string base16)
    {
        if (base16.Length % 2 != 0) throw new ArgumentOutOfRangeException(nameof(base16), "Hex string length must be even.");

        var bytes = new byte[base16.Length / 2];
        for (var i = 0; i < bytes.Length; i++)
        {
            bytes[i] = Convert.ToByte(base16.Substring(i * 2, 2), 16);
        }
        return bytes;
    }
}