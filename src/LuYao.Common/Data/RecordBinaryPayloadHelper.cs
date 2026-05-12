using System;
using System.IO;
using System.IO.Compression;

namespace LuYao.Data;

/// <summary>
/// Record 二进制负载压缩辅助类。
/// </summary>
/// <remarks>
/// 使用 5 字节 magic 头 (<c>0xFE 'L' 'Y' 'Z' &lt;algo&gt;</c>) 区分压缩负载与遗留无压缩负载，
/// 避免与 <see cref="BinaryPayloadHeader"/> 的 marker (0xFF) 以及无头部遗留格式的版本号字节产生歧义。
/// </remarks>
internal static class RecordBinaryPayloadHelper
{
    /// <summary>解压后允许的最大字节数，用于防御 zip bomb 等 DoS 攻击。</summary>
    public const int MaxDecompressedBytes = 64 * 1024 * 1024;

    /// <summary>压缩负载允许的最大字节数（解码 Base64 后），用于防御过大压缩输入。</summary>
    public const int MaxCompressedBytes = 16 * 1024 * 1024;

    /// <summary>
    /// JSON 字符串中 Base64 文本允许的最大字符数；用于在解码前阻止过大输入。
    /// 取 <see cref="MaxCompressedBytes"/> 对应的 Base64 长度上限并向上取整。
    /// </summary>
    public const int MaxBase64Length = ((MaxCompressedBytes + 2) / 3) * 4;

    private const int DecodeBufferSize = 16 * 1024;
    private const int InitialOutputCapacity = 4 * 1024;

    // Magic: 0xFE 'L' 'Y' 'Z' + 算法 ID。0xFE 与 BinaryPayloadHeader.Marker (0xFF) 及 BinaryFormatVersion (1) 都不冲突。
    private const byte MagicByte0 = 0xFE;
    private const byte MagicByte1 = (byte)'L';
    private const byte MagicByte2 = (byte)'Y';
    private const byte MagicByte3 = (byte)'Z';
    private const int MagicLength = 4;
    private const int HeaderLength = MagicLength + 1; // magic + algorithm id

    private const byte AlgorithmGZip = 1;

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

        using var output = new MemoryStream(count + HeaderLength + 16);
        output.WriteByte(MagicByte0);
        output.WriteByte(MagicByte1);
        output.WriteByte(MagicByte2);
        output.WriteByte(MagicByte3);
        output.WriteByte(AlgorithmGZip);
        using (var gzip = new GZipStream(output, CompressionLevel.Fastest, leaveOpen: true))
        {
            gzip.Write(payload, offset, count);
        }
        return output.ToArray();
    }

    /// <summary>
    /// 读取负载并按头部解压；若负载不带本类型的压缩头则原样返回（向后兼容遗留无压缩格式）。
    /// </summary>
    /// <param name="payload">Base64 解码后的负载。</param>
    /// <returns>解压后的原始负载。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="payload"/> 为 null。</exception>
    /// <exception cref="InvalidDataException">压缩输入过大、解压结果超出限制、迭代次数异常或算法不支持。</exception>
    public static byte[] Decode(byte[] payload)
    {
        if (payload == null) throw new ArgumentNullException(nameof(payload));

        // 不带本类型压缩头的负载（含遗留无压缩格式）原样透传，不做 16MB 限制：
        // Converter 层的 Base64 长度守卫已经是入口总闸门。
        if (!HasCompressedHeader(payload)) return payload;

        // 仅对带压缩头、即将进入解压流程的负载施加输入上限，防止过大压缩输入引发的内存压力。
        if (payload.Length > MaxCompressedBytes)
            throw new InvalidDataException($"压缩负载超过限制：{MaxCompressedBytes} bytes.");

        byte algorithm = payload[MagicLength];
        if (algorithm != AlgorithmGZip)
            throw new InvalidDataException($"不支持的压缩算法标识：0x{algorithm:X2}。");

        int dataOffset = HeaderLength;
        int dataLength = payload.Length - HeaderLength;

        using var input = new MemoryStream(payload, dataOffset, dataLength, writable: false);
        using var gzip = new GZipStream(input, CompressionMode.Decompress, leaveOpen: false);
        // 不信任攻击者控制的长度作为容量提示；固定小初始容量，让 MemoryStream 按需扩容。
        using var output = new MemoryStream(InitialOutputCapacity);
        var buffer = new byte[DecodeBufferSize];
        long total = 0;
        while (true)
        {
            // 注意：Stream.Read 允许返回少于 count 的字节数，因此不能基于"调用次数"做迭代上限，
            // 否则可能误杀合法但分块较小的解压流。退出条件以 read <= 0 为准。
            int read = gzip.Read(buffer, 0, buffer.Length);
            if (read <= 0) break;

            if (total + read > MaxDecompressedBytes)
                throw new InvalidDataException($"解压后数据超过限制：{MaxDecompressedBytes} bytes.");

            total += read;
            output.Write(buffer, 0, read);
        }

        return output.ToArray();
    }

    private static bool HasCompressedHeader(byte[] payload) =>
        payload.Length >= HeaderLength
        && payload[0] == MagicByte0
        && payload[1] == MagicByte1
        && payload[2] == MagicByte2
        && payload[3] == MagicByte3;
}
