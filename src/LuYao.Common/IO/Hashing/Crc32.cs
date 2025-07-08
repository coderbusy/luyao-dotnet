using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace LuYao.IO.Hashing;

/// <summary>
/// 实现兼容 Zip 等格式的 32 位 CRC 哈希算法。
/// </summary>
/// <remarks>
/// Crc32 仅用于与旧文件格式和算法的兼容，不建议用于新的安全场景。
/// 如需多次对同一数据调用，请使用 HashAlgorithm 接口，或记住每次 Compute 调用的结果需取反（XOR）后作为下一次的种子。
/// </remarks>
public sealed class Crc32 : HashAlgorithm
{
    /// <summary>
    /// 默认多项式（0x04C11DB7）。
    /// </summary>
    public const uint DefaultPolynomial = 0x04C11DB7u;

    /// <summary>
    /// 默认种子（0xFFFFFFFF）。
    /// </summary>
    public const uint DefaultSeed = 0xffffffffu;

    static uint[] defaultTable;

    readonly uint seed;
    readonly uint[] table;
    uint hash;
    uint xorOut = 0xffffffff;
    bool reflectIn = true;
    bool reflectOut = true;

    /// <summary>
    /// 创建一个使用默认多项式和默认种子的 <see cref="Crc32"/> 实例。
    /// </summary>
    public Crc32()
        : this(DefaultPolynomial, DefaultSeed) { }

    /// <summary>
    /// 创建一个自定义多项式、种子及算法选项的 <see cref="Crc32"/> 实例。
    /// 注意：默认 CRC32 算法会对输入和输出都做反射，并在最后取反（XorOut = 0xFFFFFFFF）。
    /// </summary>
    /// <param name="polynomial">CRC 多项式。</param>
    /// <param name="seed">初始种子。</param>
    /// <param name="XorOut">最终结果异或值，默认 0xFFFFFFFF。</param>
    /// <param name="refIn">输入是否反射，默认 true。</param>
    /// <param name="refOut">输出是否反射，默认 true。</param>
    public Crc32(
        uint polynomial,
        uint seed,
        uint XorOut = 0xFFFFFFFF,
        bool refIn = true,
        bool refOut = true
    )
    {
        if (!BitConverter.IsLittleEndian)
            throw new PlatformNotSupportedException("Not supported on Big Endian processors");

        table = InitializeTable(polynomial, refIn);
        this.seed = hash = seed;

        xorOut = XorOut;
        reflectIn = refIn;
        reflectOut = refOut;
    }

    /// <inheritdoc/>
    public override void Initialize()
    {
        hash = seed;
    }

    /// <inheritdoc/>
    protected override void HashCore(byte[] array, int ibStart, int cbSize)
    {
        hash = CalculateHash(table, hash, array, ibStart, cbSize, xorOut, reflectIn, reflectOut);
    }

    /// <inheritdoc/>
    protected override byte[] HashFinal()
    {
        var hashBuffer = UInt32ToBigEndianBytes(hash ^ xorOut);
        HashValue = hashBuffer;
        return hashBuffer;
    }

    /// <inheritdoc/>
    /// <summary>
    /// 获取哈希值的位数（32 位）。
    /// </summary>
    public override int HashSize => 32;

    /// <summary>
    /// 使用默认多项式和默认种子计算指定缓冲区的 CRC32 值。
    /// </summary>
    /// <param name="buffer">要计算 CRC32 的字节数组。</param>
    /// <returns>缓冲区的 CRC32 值。</returns>
    public static uint Compute(byte[] buffer) => Compute(DefaultSeed, buffer);

    /// <summary>
    /// 使用指定种子和默认多项式计算指定缓冲区的 CRC32 值。
    /// </summary>
    /// <param name="seed">初始种子。</param>
    /// <param name="buffer">要计算 CRC32 的字节数组。</param>
    /// <returns>缓冲区的 CRC32 值。</returns>
    public static uint Compute(uint seed, byte[] buffer) =>
        Compute(DefaultPolynomial, seed, buffer);

    /// <summary>
    /// 使用指定多项式和种子计算指定缓冲区的 CRC32 值。
    /// </summary>
    /// <param name="polynomial">CRC 多项式。</param>
    /// <param name="seed">初始种子。</param>
    /// <param name="buffer">要计算 CRC32 的字节数组。</param>
    /// <returns>缓冲区的 CRC32 值。</returns>
    public static uint Compute(uint polynomial, uint seed, byte[] buffer) =>
        ~CalculateHash(InitializeTable(polynomial), seed, buffer, 0, buffer.Length);

    /// <summary>
    /// 初始化指定多项式的 CRC32 计算表。
    /// </summary>
    /// <param name="polynomial">CRC 多项式。</param>
    /// <param name="refIn">输入是否反射，默认 true。</param>
    /// <returns>用于计算 CRC32 的 UInt32[] 表。</returns>
    static uint[] InitializeTable(uint polynomial, bool refIn = true)
    {
        if (polynomial == DefaultPolynomial && defaultTable != null && refIn)
            return defaultTable;

        uint bit;
        var createTable = new uint[256];
        for (var i = 0; i < 256; i++)
        {
            var entry = refIn ? reflect((uint)i, 8) : (uint)i;

            entry <<= 24;
            for (var j = 0; j < 8; j++)
            {
                bit = entry & 1u << 31;
                entry <<= 1;
                if (bit > 0)
                {
                    entry ^= polynomial;
                }
            }

            if (refIn)
            {
                entry = reflect(entry, 32);
            }
            createTable[i] = entry;
        }

        if (polynomial == DefaultPolynomial && refIn)
            defaultTable = createTable;

        return createTable;
    }

    /// <summary>
    /// 使用多项式派生的表对指定缓冲区进行反转 CRC32 计算。
    /// </summary>
    /// <param name="table">CRC32 计算表。</param>
    /// <param name="seed">初始种子。</param>
    /// <param name="buffer">要计算 CRC32 的缓冲区。</param>
    /// <param name="start">缓冲区的起始位置。</param>
    /// <param name="size">缓冲区的大小。</param>
    /// <param name="xorOut">最终结果异或值，默认 0xFFFFFFFF。</param>
    /// <param name="refIn">输入是否反射，默认 true。</param>
    /// <param name="refOut">输出是否反射，默认 true。</param>
    /// <returns>反转后的 CRC32 值。</returns>
    /// <remarks>此哈希值已反转。请使用此类中的其他方法或对结果取反（~）。</remarks>
    static uint CalculateHash(
        uint[] table,
        uint seed,
        IList<byte> buffer,
        int start,
        int size,
        uint xorOut = 0xffffffff,
        bool refIn = true,
        bool refOut = true
    )
    {
        var hash = seed;
        //if (refIn) hash = reflect(hash, 32);

        if (refIn)
        {
            for (var i = start; i < start + size; i++)
                hash = hash >> 8 ^ table[buffer[i] ^ hash & 0xff];
        }
        else
        {
            for (var i = start; i < start + size; i++)
                hash = hash << 8 ^ table[buffer[i] ^ hash >> 24 & 0xff];
        }

        if (refIn ^ refOut)
        {
            hash = reflect(hash, 32);
        }

        return hash;
    }

    /// <summary>
    /// 将 <see cref="uint"/> 转换为字节数组，并在小端处理器上反转字节。
    /// </summary>
    /// <param name="uint32">要转换的 <see cref="uint"/>。</param>
    /// <returns>包含转换字节的字节数组。</returns>
    static byte[] UInt32ToBigEndianBytes(uint uint32)
    {
        var result = BitConverter.GetBytes(uint32);

        if (BitConverter.IsLittleEndian)
            Array.Reverse(result);

        return result;
    }

    /// <summary>
    /// 对指定数据进行位反射（高低位互换）。
    /// </summary>
    /// <param name="crc">待反射的数值。</param>
    /// <param name="bitnum">反射的位数。</param>
    /// <returns>反射后的数值。</returns>
    public static uint reflect(uint crc, int bitnum)
    {
        // reflects the lower 'bitnum' bits of 'crc'

        uint i,
            j = 1,
            crcout = 0;

        for (i = (uint)1 << bitnum - 1; i > 0; i >>= 1)
        {
            if ((crc & i) > 0)
                crcout |= j;
            j <<= 1;
        }
        return crcout;
    }
}
