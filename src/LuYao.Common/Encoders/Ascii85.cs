using System;
using System.IO;
using System.Text;

namespace LuYao.Encoders;

/// <summary>
/// ASCII85 编码的 C# 实现。
/// 基于 http://www.stillhq.com/cgi-bin/cvsweb/ascii85/ 的 C 代码。
/// </summary>
/// <remarks>
/// Jeff Atwood
/// http://www.codinghorror.com/blog/archives/000410.html
/// </remarks>
public class Ascii85
{
    /// <summary>
    /// 标识 ASCII85 编码字符串的前缀标记，通常为 "&lt;~"
    /// </summary>
    public string PrefixMark = "<~";

    /// <summary>
    /// 标识 ASCII85 编码字符串的后缀标记，通常为 "~&gt;"
    /// </summary>
    public string SuffixMark = "~>";

    /// <summary>
    /// ASCII85 编码字符串的最大行长度；
    /// 设置为 0 表示输出为一行不换行。
    /// </summary>
    public int LineLength = 75;

    /// <summary>
    /// 编码时是否添加前缀和后缀标记，解码时是否强制要求标记存在。
    /// </summary>
    public bool EnforceMarks = true;

    private const int _asciiOffset = 33;
    private byte[] _encodedBlock = new byte[5];
    private byte[] _decodedBlock = new byte[4];
    private uint _tuple = 0;
    private int _linePos = 0;

    private uint[] pow85 = { 85 * 85 * 85 * 85, 85 * 85 * 85, 85 * 85, 85, 1 };

    /// <summary>
    /// 从 ASCII85 编码字符串解码为原始二进制数据。
    /// </summary>
    /// <param name="str">ASCII85 编码字符串</param>
    /// <returns>解码后的字节数组</returns>
    public static byte[] FromAscii85String(string str)
    {
        var ascii = new Ascii85();
        return ascii.Decode(str);
    }

    /// <summary>
    /// 将二进制数据编码为 ASCII85 字符串。
    /// </summary>
    /// <param name="data">要编码的字节数组</param>
    /// <returns>ASCII85 编码字符串</returns>
    public static string ToAscii85String(byte[] data)
    {
        var ascii = new Ascii85();
        ascii.LineLength = 0;
        return ascii.Encode(data);
    }

    /// <summary>
    /// 解码 ASCII85 编码字符串为原始二进制数据。
    /// </summary>
    /// <param name="s">ASCII85 编码字符串</param>
    /// <returns>解码后的字节数组</returns>
    public byte[] Decode(string s)
    {
        if (EnforceMarks)
        {
            if (!s.StartsWith(PrefixMark) | !s.EndsWith(SuffixMark))
            {
                throw new Exception(
                    "ASCII85 encoded data should begin with '"
                        + PrefixMark
                        + "' and end with '"
                        + SuffixMark
                        + "'"
                );
            }
        }

        // strip prefix and suffix if present
        if (s.StartsWith(PrefixMark))
        {
            s = s.Substring(PrefixMark.Length);
        }
        if (s.EndsWith(SuffixMark))
        {
            s = s.Substring(0, s.Length - SuffixMark.Length);
        }

        MemoryStream ms = new MemoryStream();
        int count = 0;
        foreach (char c in s)
        {
            bool processChar;
            switch (c)
            {
                case 'z':
                    if (count != 0)
                    {
                        throw new Exception(
                            "The character 'z' is invalid inside an ASCII85 block."
                        );
                    }
                    _decodedBlock[0] = 0;
                    _decodedBlock[1] = 0;
                    _decodedBlock[2] = 0;
                    _decodedBlock[3] = 0;
                    ms.Write(_decodedBlock, 0, _decodedBlock.Length);
                    processChar = false;
                    break;
                case '\n':
                case '\r':
                case '\t':
                case '\0':
                case '\f':
                case '\b':
                    processChar = false;
                    break;
                default:
                    if (c < '!' || c > 'u')
                    {
                        throw new Exception(
                            "Bad character '"
                                + c
                                + "' found. ASCII85 only allows characters '!' to 'u'."
                        );
                    }
                    processChar = true;
                    break;
            }

            if (processChar)
            {
                _tuple += (uint)(c - _asciiOffset) * pow85[count];
                count++;
                if (count == _encodedBlock.Length)
                {
                    DecodeBlock();
                    ms.Write(_decodedBlock, 0, _decodedBlock.Length);
                    _tuple = 0;
                    count = 0;
                }
            }
        }

        // if we have some bytes left over at the end..
        if (count != 0)
        {
            if (count == 1)
            {
                throw new Exception("The last block of ASCII85 data cannot be a single byte.");
            }
            count--;
            _tuple += pow85[count];
            DecodeBlock(count);
            for (int i = 0; i < count; i++)
            {
                ms.WriteByte(_decodedBlock[i]);
            }
        }

        return ms.ToArray();
    }

    /// <summary>
    /// 将二进制数据编码为 ASCII85 格式的字符串。
    /// </summary>
    /// <param name="ba">要编码的二进制数据</param>
    /// <returns>ASCII85 编码字符串</returns>
    public string Encode(byte[] ba)
    {
        StringBuilder sb = new StringBuilder(
            ba.Length * (_encodedBlock.Length / _decodedBlock.Length)
        );
        _linePos = 0;

        if (EnforceMarks)
        {
            AppendString(sb, PrefixMark);
        }

        int count = 0;
        _tuple = 0;
        foreach (byte b in ba)
        {
            if (count >= _decodedBlock.Length - 1)
            {
                _tuple |= b;
                if (_tuple == 0)
                {
                    AppendChar(sb, 'z');
                }
                else
                {
                    EncodeBlock(sb);
                }
                _tuple = 0;
                count = 0;
            }
            else
            {
                _tuple |= (uint)(b << 24 - count * 8);
                count++;
            }
        }

        // if we have some bytes left over at the end..
        if (count > 0)
        {
            EncodeBlock(count + 1, sb);
        }

        if (EnforceMarks)
        {
            AppendString(sb, SuffixMark);
        }
        return sb.ToString();
    }

    private void EncodeBlock(StringBuilder sb)
    {
        EncodeBlock(_encodedBlock.Length, sb);
    }

    private void EncodeBlock(int count, StringBuilder sb)
    {
        for (int i = _encodedBlock.Length - 1; i >= 0; i--)
        {
            _encodedBlock[i] = (byte)(_tuple % 85 + _asciiOffset);
            _tuple /= 85;
        }

        for (int i = 0; i < count; i++)
        {
            char c = (char)_encodedBlock[i];
            AppendChar(sb, c);
        }
    }

    private void DecodeBlock()
    {
        DecodeBlock(_decodedBlock.Length);
    }

    private void DecodeBlock(int bytes)
    {
        for (int i = 0; i < bytes; i++)
        {
            _decodedBlock[i] = (byte)(_tuple >> 24 - i * 8);
        }
    }

    private void AppendString(StringBuilder sb, string s)
    {
        if (LineLength > 0 && _linePos + s.Length > LineLength)
        {
            _linePos = 0;
            sb.Append('\n');
        }
        else
        {
            _linePos += s.Length;
        }
        sb.Append(s);
    }

    private void AppendChar(StringBuilder sb, char c)
    {
        sb.Append(c);
        _linePos++;
        if (LineLength > 0 && _linePos >= LineLength)
        {
            _linePos = 0;
            sb.Append('\n');
        }
    }
}
