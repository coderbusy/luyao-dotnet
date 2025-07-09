using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;

namespace LuYao;

/// <summary>
/// 字符串压缩工具
/// </summary>
public static class GZipString
{
    #region ICompressor

    /// <summary>
    /// 压缩器
    /// </summary>
    public interface ICompressor
    {
        /// <summary>
        /// 压缩器标识符
        /// </summary>
        string Identifier { get; }

        /// <summary>
        /// 压缩字符串
        /// </summary>
        /// <param name="value">要压缩的字符串</param>
        /// <returns>压缩后的字节数组</returns>
        byte[] Compress(string value);

        /// <summary>
        /// 解压缩字节数组
        /// </summary>
        /// <param name="data">要解压缩的字节数组</param>
        /// <returns>解压缩后的字符串</returns>
        string Decompress(byte[] data);
    }

    private class LzStringCompressor : ICompressor
    {
        public static LzStringCompressor Instance { get; } = new LzStringCompressor();
        public string Identifier => "lzstring";

        public byte[] Compress(string value) => Encoders.LzString.CompressToUint8Array(value);

        public string Decompress(byte[] data) => Encoders.LzString.DecompressFromUint8Array(data);
    }

    /// <summary>
    /// 基于流的压缩器抽象基类
    /// </summary>
    /// <typeparam name="TStream">流类型</typeparam>
    abstract class StreamCompressor<TStream> : ICompressor where TStream : Stream
    {
        /// <summary>
        /// 压缩器标识符
        /// </summary>
        public abstract string Identifier { get; }

        /// <summary>
        /// 创建指定模式的压缩流
        /// </summary>
        /// <param name="stream">底层流</param>
        /// <param name="mode">压缩模式</param>
        /// <param name="leaveOpen">是否保持流打开</param>
        /// <returns>压缩流实例</returns>
        protected abstract TStream Create(Stream stream, CompressionMode mode, bool leaveOpen);

        /// <summary>
        /// 使用的文本编码
        /// </summary>
        protected virtual Encoding Encoding => Encoding.UTF8;

        public virtual byte[] Compress(string value)
        {
            using var o = new MemoryStream(Encoding.GetBytes(value));
            using var t = new MemoryStream();
            using (Stream c = Create(t, CompressionMode.Compress, true))
            {
                o.CopyTo(c);
                c.Flush();
            }
            return t.ToArray();
        }

        public virtual string Decompress(byte[] data)
        {
            using var o = new MemoryStream(data);
            using var t = new MemoryStream();
            using (Stream u = Create(o, CompressionMode.Decompress, true))
            {
                u.CopyTo(t);
            }
            var bytes = t.ToArray();
            return Encoding.GetString(bytes);
        }
    }

    private class DeflateStreamCompressor : StreamCompressor<DeflateStream>
    {
        public static DeflateStreamCompressor Instance { get; } = new DeflateStreamCompressor();
        public override string Identifier => "deflate";

        protected override DeflateStream Create(
            Stream stream,
            CompressionMode mode,
            bool leaveOpen
        ) => new(stream, mode, leaveOpen);
    }

    private class GZipStreamCompressor : StreamCompressor<GZipStream>
    {
        public static GZipStreamCompressor Instance { get; } = new GZipStreamCompressor();
        public override string Identifier => "gzip";

        protected override GZipStream Create(Stream stream, CompressionMode mode, bool leaveOpen) =>
            new(stream, mode, leaveOpen);
    }

    /// <summary>
    /// LZ字符串压缩器实例
    /// </summary>
    public static ICompressor LzString => LzStringCompressor.Instance;

    /// <summary>
    /// Deflate压缩器实例
    /// </summary>
    public static ICompressor Deflate => DeflateStreamCompressor.Instance;

    /// <summary>
    /// GZip压缩器实例
    /// </summary>
    public static ICompressor GZip => GZipStreamCompressor.Instance;

    #endregion

    #region IEncoder

    /// <summary>
    /// 编码器
    /// </summary>
    public interface IEncoder
    {
        /// <summary>
        /// 编码器标识符
        /// </summary>
        string Identifier { get; }

        /// <summary>
        /// 将字节数组编码为字符串
        /// </summary>
        /// <param name="data">要编码的字节数组</param>
        /// <returns>编码后的字符串</returns>
        string Encode(byte[] data);

        /// <summary>
        /// 将字符串解码为字节数组
        /// </summary>
        /// <param name="value">要解码的字符串</param>
        /// <returns>解码后的字节数组</returns>
        byte[] Decode(string value);
    }

    private class Base16Encoder : IEncoder
    {
        public static Base16Encoder Instance { get; } = new Base16Encoder();
        public string Identifier => "base16";

        public byte[] Decode(string value) => Encoders.Base16.FromBase16(value);

        public string Encode(byte[] original) => Encoders.Base16.ToBase16(original);
    }


    private class Base62Encoder : IEncoder
    {
        public static Base62Encoder Instance { get; } = new Base62Encoder();
        public string Identifier => "base62";

        public byte[] Decode(string value) => Encoders.Base62.FromBase62(value);

        public string Encode(byte[] data) => Encoders.Base62.ToBase62(data);
    }

    private class Base32Encoder : IEncoder
    {
        public static Base32Encoder Instance { get; } = new Base32Encoder();
        public string Identifier => "base32";
        public byte[] Decode(string value) => Encoders.Base32.FromBase32(value);
        public string Encode(byte[] data) => Encoders.Base32.ToBase32(data);
    }

    private class Base64Encoder : IEncoder
    {
        public static Base64Encoder Instance { get; } = new Base64Encoder();
        public string Identifier => "base64";

        public byte[] Decode(string value) => Convert.FromBase64String(value);

        public string Encode(byte[] data) => Convert.ToBase64String(data);
    }


    private class Ascii85Encoder : IEncoder
    {
        public static Ascii85Encoder Instance { get; } = new Ascii85Encoder();
        public string Identifier => "ascii85";

        public byte[] Decode(string value) => Encoders.Ascii85.FromAscii85String(value);

        public string Encode(byte[] data) => Encoders.Ascii85.ToAscii85String(data);
    }

    /// <summary>
    /// Base16编码器实例
    /// </summary>
    public static IEncoder Base16 => Base16Encoder.Instance;

    /// <summary>
    /// Base32编码器实例
    /// </summary>
    public static IEncoder Base32 => Base32Encoder.Instance;

    /// <summary>
    /// Base62编码器实例
    /// </summary>
    public static IEncoder Base62 => Base62Encoder.Instance;

    /// <summary>
    /// Base64编码器实例
    /// </summary>
    public static IEncoder Base64 => Base64Encoder.Instance;

    /// <summary>
    /// Ascii85编码器实例
    /// </summary>
    public static IEncoder Ascii85 => Ascii85Encoder.Instance;
    #endregion

    private static readonly Regex PayloadRegex = new Regex(
        "^data:text/x-(?<Compresser>\\w+);(?<Encoder>\\w+),(?<Data>.*)",
        RegexOptions.Compiled
    );

    private static ICompressor GetCompressor(string identifier)
    {
        if (StringComparer.OrdinalIgnoreCase.Equals(LzString.Identifier, identifier)) return LzString;
        if (StringComparer.OrdinalIgnoreCase.Equals(Deflate.Identifier, identifier)) return Deflate;
        if (StringComparer.OrdinalIgnoreCase.Equals(GZip.Identifier, identifier)) return GZip;
        throw new KeyNotFoundException($"未找到压缩器标识符: {identifier}");
    }

    private static IEncoder GetEncoder(string identifier)
    {
        if (StringComparer.OrdinalIgnoreCase.Equals(Base16.Identifier, identifier)) return Base16;
        if (StringComparer.OrdinalIgnoreCase.Equals(Base32.Identifier, identifier)) return Base32;
        if (StringComparer.OrdinalIgnoreCase.Equals(Base62.Identifier, identifier)) return Base62;
        if (StringComparer.OrdinalIgnoreCase.Equals(Base64.Identifier, identifier)) return Base64;
        if (StringComparer.OrdinalIgnoreCase.Equals(Ascii85.Identifier, identifier)) return Ascii85;
        throw new KeyNotFoundException($"未找到编码器标识符: {identifier}");
    }

    /// <summary>
    /// 压缩字符串
    /// </summary>
    /// <param name="str">待压缩字符串</param>
    /// <param name="compressor">压缩器</param>
    /// <param name="encoder">编码器</param>
    /// <returns>压缩后的字符串</returns>
    public static string Compress(string str, string compressor, string encoder)
    {
        if (string.IsNullOrEmpty(str)) return string.Empty;
        if (string.IsNullOrWhiteSpace(compressor)) throw new ArgumentNullException(nameof(compressor));
        if (string.IsNullOrWhiteSpace(encoder)) throw new ArgumentNullException(nameof(encoder));
        ICompressor xCompressor = GetCompressor(compressor);
        IEncoder xEncoder = GetEncoder(encoder);
        return Compress(str, xCompressor, xEncoder);
    }

    /// <summary>
    /// 压缩字符串
    /// </summary>
    /// <param name="str">待压缩字符串</param>
    /// <param name="compressor">压缩器</param>
    /// <param name="encoder">编码器</param>
    /// <returns>压缩后的字符串</returns>
    public static string Compress(string str, ICompressor compressor, IEncoder encoder)
    {
        if (string.IsNullOrEmpty(str)) return string.Empty;
        if (compressor == null) throw new ArgumentNullException(nameof(compressor));
        if (encoder == null) throw new ArgumentNullException(nameof(encoder));
        if (PayloadRegex.IsMatch(str)) return str;// 如果已经是压缩格式，则直接返回
        var compressedData = compressor.Compress(str);
        var encodedData = encoder.Encode(compressedData);
        return $"data:text/x-{compressor.Identifier};{encoder.Identifier},{encodedData}";
    }

    /// <summary>
    /// 压缩字符串
    /// </summary>
    /// <param name="str">待压缩字符串</param>
    /// <returns>压缩后的字符串</returns>
    public static string Compress(string str) => Compress(str, Deflate, Base64);

    /// <summary>
    /// 解压缩字符串
    /// </summary>
    /// <param name="input">待解压缩的字符串</param>
    /// <returns>解压缩后的原始字符串</returns>
    public static string Decompress(string input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;
        var m = PayloadRegex.Match(input);
        if (!m.Success) return input;
        var strCompresser = m.Groups["Compresser"].Value;
        var strEncoder = m.Groups["Encoder"].Value;
        var strData = m.Groups["Data"].Value;
        var compressor = GetCompressor(strCompresser);
        var encoder = GetEncoder(strEncoder);
        var bytes = encoder.Decode(strData);
        var str = compressor.Decompress(bytes);
        return str;
    }
}
