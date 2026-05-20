#if !NET45 && !NET461
using System;
using System.IO;
using System.IO.Compression;

namespace LuYao.Data.Binary;

/// <summary>
/// Record 二进制负载编解码器。
/// </summary>
/// <remarks>
/// <para>当前格式（版本 1）头部共 6 字节：</para>
/// <code>
/// 0xFE 'L' 'Y' 'Z' &lt;version=1&gt; &lt;algo&gt;
/// </code>
/// <para>
/// 其中 algo 对应 <see cref="RecordPayloadCompression"/>：0 = 不压缩，1 = GZip。
/// 头部中的 0xFE 与 <see cref="BinaryPayloadHeader"/> 的 marker (0xFF) 以及遗留格式的版本号字节 (1) 均不冲突。
/// </para>
/// <para>
/// 为防御 zip bomb / DoS 攻击，可通过 <see cref="MaxDecompressedBytes"/> 和
/// <see cref="MaxCompressedBytes"/> 属性配置限制。
/// </para>
/// </remarks>
public sealed class RecordBinaryPayloadCodec
{
    // ── Magic ──────────────────────────────────────────────────
    private const byte MagicByte0 = 0xFE;
    private const byte MagicByte1 = (byte)'L';
    private const byte MagicByte2 = (byte)'Y';
    private const byte MagicByte3 = (byte)'Z';
    private const int MagicLength = 4;

    // 版本 1 头部：magic(4) + version(1) + algo(1)
    private const byte FormatVersion = 1;
    private const int HeaderLength = MagicLength + 1 + 1; // 6

    private const int DecodeBufferSize = 16 * 1024;
    private const int InitialOutputCapacity = 4 * 1024;

    // ── 默认 DoS 限制 ──────────────────────────────────────────
    private const int DefaultMaxDecompressedBytes = 64 * 1024 * 1024;
    private const int DefaultMaxCompressedBytes = 16 * 1024 * 1024;

    // ── 单例：使用默认配置 ──────────────────────────────────────
    /// <summary>使用默认配置的共享实例（GZip 压缩，默认 DoS 阈值）。</summary>
    public static readonly RecordBinaryPayloadCodec Default = new();

    // ── 属性 ────────────────────────────────────────────────────

    /// <summary>序列化时使用的压缩算法，默认 <see cref="RecordPayloadCompression.GZip"/>。</summary>
    public RecordPayloadCompression Compression { get; set; } = RecordPayloadCompression.GZip;

    /// <summary>
    /// 解压后允许的最大字节数，用于防御 zip bomb 等 DoS 攻击。
    /// 仅在使用压缩算法时生效。默认 64 MB。
    /// </summary>
    public int MaxDecompressedBytes { get; set; } = DefaultMaxDecompressedBytes;

    /// <summary>
    /// 压缩负载（Base64 解码后）允许的最大字节数，用于防御过大压缩输入。
    /// 默认 16 MB。
    /// </summary>
    public int MaxCompressedBytes { get; set; } = DefaultMaxCompressedBytes;

    /// <summary>
    /// JSON 字符串中 Base64 文本允许的最大字符数。
    /// 取 <see cref="MaxCompressedBytes"/> 对应的 Base64 长度上限并向上取整。
    /// </summary>
    public int MaxBase64Length => ((MaxCompressedBytes + 2) / 3) * 4;

    // ── Encode ──────────────────────────────────────────────────

    /// <summary>按配置的压缩算法编码负载，并写入版本化头部。</summary>
    /// <param name="payload">原始负载。</param>
    /// <returns>带头部的编码负载。</returns>
    public byte[] Encode(byte[] payload)
    {
        if (payload == null) throw new ArgumentNullException(nameof(payload));
        return Encode(payload, 0, payload.Length);
    }

    /// <summary>按配置的压缩算法编码负载，并写入版本化头部。</summary>
    /// <param name="payload">原始负载。</param>
    /// <param name="offset">起始偏移。</param>
    /// <param name="count">长度。</param>
    /// <returns>带头部的编码负载。</returns>
    public byte[] Encode(byte[] payload, int offset, int count)
    {
        if (payload == null) throw new ArgumentNullException(nameof(payload));
        if (offset < 0 || offset > payload.Length) throw new ArgumentOutOfRangeException(nameof(offset));
        if (count < 0 || payload.Length - offset < count) throw new ArgumentOutOfRangeException(nameof(count));

        using var output = new MemoryStream(count + HeaderLength + 16);
        WriteHeader(output, Compression);

        switch (Compression)
        {
            case RecordPayloadCompression.None:
                output.Write(payload, offset, count);
                break;
            case RecordPayloadCompression.GZip:
                using (var gzip = new GZipStream(output, CompressionLevel.Fastest, leaveOpen: true))
                {
                    gzip.Write(payload, offset, count);
                }
                break;
            default:
                throw new InvalidOperationException($"不支持的压缩算法：{Compression}。");
        }

        return output.ToArray();
    }

    // ── Decode ──────────────────────────────────────────────────

    /// <summary>
    /// 读取负载并自动识别版本与压缩算法进行解码；若负载不带本格式头则原样返回（向后兼容遗留无压缩格式）。
    /// </summary>
    /// <param name="payload">Base64 解码后的负载。</param>
    /// <returns>解码后的原始负载。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="payload"/> 为 null。</exception>
    /// <exception cref="InvalidDataException">输入过大、解压结果超出限制或算法不支持。</exception>
    public byte[] Decode(byte[] payload)
    {
        if (payload == null) throw new ArgumentNullException(nameof(payload));

        if (!TryReadHeader(payload, out byte version, out RecordPayloadCompression algo, out int dataOffset))
        {
            // 无头部：遗留格式，原样返回
            return payload;
        }

        if (algo == RecordPayloadCompression.None)
        {
            // 无压缩：直接截取数据部分
            int dataLength = payload.Length - dataOffset;
            var result = new byte[dataLength];
            Buffer.BlockCopy(payload, dataOffset, result, 0, dataLength);
            return result;
        }

        // 有压缩：施加输入上限防御 DoS
        if (payload.Length > MaxCompressedBytes)
            throw new InvalidDataException($"压缩负载超过限制：{MaxCompressedBytes} bytes。");

        return Decompress(payload, dataOffset, algo);
    }

    // ── Private helpers ─────────────────────────────────────────

    private static void WriteHeader(Stream stream, RecordPayloadCompression algo)
    {
        stream.WriteByte(MagicByte0);
        stream.WriteByte(MagicByte1);
        stream.WriteByte(MagicByte2);
        stream.WriteByte(MagicByte3);
        stream.WriteByte(FormatVersion);
        stream.WriteByte((byte)algo);
    }

    /// <summary>
    /// 尝试解析版本 1 头部（6 字节）。
    /// </summary>
    private static bool TryReadHeader(byte[] data, out byte version, out RecordPayloadCompression algo, out int dataOffset)
    {
        version = 0;
        algo = RecordPayloadCompression.None;
        dataOffset = 0;

        if (data.Length < HeaderLength) return false;
        if (data[0] != MagicByte0 || data[1] != MagicByte1 || data[2] != MagicByte2 || data[3] != MagicByte3)
            return false;

        version = data[MagicLength];
        algo = (RecordPayloadCompression)data[MagicLength + 1];
        dataOffset = HeaderLength;
        return true;
    }

    private byte[] Decompress(byte[] payload, int dataOffset, RecordPayloadCompression algo)
    {
        int dataLength = payload.Length - dataOffset;
        using var input = new MemoryStream(payload, dataOffset, dataLength, writable: false);

        Stream decompressStream = algo switch
        {
            RecordPayloadCompression.GZip => new GZipStream(input, CompressionMode.Decompress, leaveOpen: false),
            _ => throw new InvalidDataException($"不支持的压缩算法标识：0x{(byte)algo:X2}。")
        };

        using (decompressStream)
        using (var output = new MemoryStream(InitialOutputCapacity))
        {
            var buffer = new byte[DecodeBufferSize];
            long total = 0;
            while (true)
            {
                int read = decompressStream.Read(buffer, 0, buffer.Length);
                if (read <= 0) break;

                if (total + read > MaxDecompressedBytes)
                    throw new InvalidDataException($"解压后数据超过限制：{MaxDecompressedBytes} bytes。");

                total += read;
                output.Write(buffer, 0, read);
            }
            return output.ToArray();
        }
    }
}
#endif