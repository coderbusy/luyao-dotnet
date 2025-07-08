using System;

namespace LuYao.Encoders;

/// <summary>
/// Base64 编码与解码工具类。
/// </summary>
public static class Base64
{
    /// <summary>
    /// 将字节数组编码为 Base64 字符串。
    /// </summary>
    /// <param name="data">要编码的字节数组。</param>
    /// <param name="trim">是否去除末尾的等号填充字符。</param>
    /// <returns>Base64 编码后的字符串。</returns>
    public static string ToBase64(byte[] data, bool trim = false)
    {
        var str = Convert.ToBase64String(data);
        if (trim)
            str = str.TrimEnd('=');
        return str;
    }

    /// <summary>
    /// 将 Base64 字符串解码为字节数组。
    /// </summary>
    /// <param name="s">Base64 编码的字符串。</param>
    /// <returns>解码后的字节数组。</returns>
    public static byte[] FromBase64(string s)
    {
        if (!string.IsNullOrWhiteSpace(s) && s.Length % 4 != 0)
            s += new string('=', 4 - s.Length % 4);
        var data = Convert.FromBase64String(s);
        return data;
    }
}
