using System;
using System.IO;
using System.IO.Compression;

namespace LuYao.Data;

/// <summary>
/// Record 二进制负载压缩辅助类。
/// </summary>
internal static class RecordBinaryPayloadHelper
{
    private const int MaxDecompressedBytes = 64 * 1024 * 1024;
    private const int DecodeBufferSize = 16 * 1024;
    private const byte CompressionGZip = 1;
    private const byte GZipSignatureFirst = 0x1F;
    private const byte GZipSignatureSecond = 0x8B;

    /// <summary>
    /// 压缩并写入头部。
    /// </summary>
    /// <param name="payload">原始负载。</param>
    /// <returns>带头部的压缩负载。</returns>
    public static byte[] Encode(byte[] payload)
    {
        if (payload == null) throw new ArgumentNullException(nameof(payload));
        return Encode(payload, 0, payload.Length);
    }

    /// <summary>
    /// 压缩并写入头部。
    /// </summary>
    /// <param name="payload">原始负载。</param>
    /// <param name="offset">起始偏移。</param>
    /// <param name="count">长度。</param>
    /// <returns>带头部的压缩负载。</returns>
    public static byte[] Encode(byte[] payload, int offset, int count)
    {
        if (payload == null) throw new ArgumentNullException(nameof(payload));
        if (offset < 0 || offset > payload.Length) throw new ArgumentOutOfRangeException(nameof(offset));
        if (count < 0 || payload.Length - offset < count) throw new ArgumentOutOfRangeException(nameof(count));

        using var output = new MemoryStream(count + 16);
        output.WriteByte(CompressionGZip);
        using (var gzip = new GZipStream(output, CompressionLevel.Fastest, leaveOpen: true))
        {
            gzip.Write(payload, offset, count);
        }
        return output.ToArray();
    }

    /// <summary>
    /// 读取负载并按头部解压。
    /// </summary>
    /// <param name="payload">Base64 解码后的负载。</param>
    /// <returns>解压后的原始负载。</returns>
    public static byte[] Decode(byte[] payload)
    {
        if (payload == null) throw new ArgumentNullException(nameof(payload));
        if (!IsGZipPayload(payload)) return payload;

        using var input = new MemoryStream(payload, 1, payload.Length - 1, writable: false);
        using var gzip = new GZipStream(input, CompressionMode.Decompress, leaveOpen: false);
        using var output = new MemoryStream(Math.Min(payload.Length * 4, MaxDecompressedBytes));
        var buffer = new byte[DecodeBufferSize];
        var total = 0;
        while (true)
        {
            var read = gzip.Read(buffer, 0, buffer.Length);
            if (read <= 0) break;

            if (total + read > MaxDecompressedBytes)
                throw new InvalidDataException($"解压后数据超过限制：{MaxDecompressedBytes} bytes.");

            total += read;
            output.Write(buffer, 0, read);
        }

        return output.ToArray();
    }

    private static bool IsGZipPayload(byte[] payload) =>
        payload.Length >= 3
        && payload[0] == CompressionGZip
        && payload[1] == GZipSignatureFirst
        && payload[2] == GZipSignatureSecond;
}
