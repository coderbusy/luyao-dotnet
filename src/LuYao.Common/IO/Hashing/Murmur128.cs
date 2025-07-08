using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace LuYao.IO.Hashing;

/// <summary>
/// 高性能低碰撞Murmur128哈希算法，Redis等大量使用，比MD5要好。
/// </summary>
public class Murmur128 : HashAlgorithm
{
    /// <summary>
    /// 获取哈希种子。
    /// </summary>
    public uint Seed { get; }

    /// <summary>
    /// 获取哈希值的大小（位），固定为128。
    /// </summary>
    public override int HashSize => 128;

    /// <summary>
    /// 使用指定种子初始化 <see cref="Murmur128"/> 实例。
    /// </summary>
    /// <param name="seed">哈希种子。</param>
    public Murmur128(uint seed)
    {
        Seed = seed;
        Reset();
    }

    /// <summary>
    /// 使用默认种子（0）初始化 <see cref="Murmur128"/> 实例。
    /// </summary>
    public Murmur128()
        : this(0) { }

    private const ulong C1 = 0x87c37b91114253d5;
    private const ulong C2 = 0x4cf5ad432745937f;

    private int _Length;
    private ulong _H1;
    private ulong _H2;

    private void Reset()
    {
        _H1 = _H2 = Seed;
        _Length = 0;
    }

    /// <summary>
    /// 重置哈希算法以便重新使用。
    /// </summary>
    public override void Initialize()
    {
        Reset();
    }

    /// <summary>
    /// 处理输入字节数据，参与哈希计算。
    /// </summary>
    /// <param name="array">输入字节数组。</param>
    /// <param name="ibStart">起始偏移量。</param>
    /// <param name="cbSize">处理的字节数。</param>
    protected override void HashCore(byte[] array, int ibStart, int cbSize)
    {
        _Length += cbSize;
        Body(array, ibStart, cbSize);
    }

    private ulong ToUInt64(byte[] data, int offset)
    {
        return BitConverter.ToUInt64(data, offset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Body(byte[] data, int start, int length)
    {
        var remainder = length & 15;
        var alignedLength = start + (length - remainder);
        for (var i = start; i < alignedLength; i += 16)
        {
            _H1 ^= RotateLeft(ToUInt64(data, i) * C1, 31) * C2;
            _H1 = (RotateLeft(_H1, 27) + _H2) * 5 + 0x52dce729;

            _H2 ^= RotateLeft(ToUInt64(data, i + 8) * C2, 33) * C1;
            _H2 = (RotateLeft(_H2, 31) + _H1) * 5 + 0x38495ab5;
        }

        if (remainder > 0)
        {
            Tail(data, alignedLength, remainder);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Tail(byte[] tail, int start, int remaining)
    {
        // create our keys and initialize to 0
        ulong k1 = 0,
            k2 = 0;

        // determine how many bytes we have left to work with based on length
        switch (remaining)
        {
            case 15:
                k2 ^= (ulong)tail[start + 14] << 48;
                goto case 14;
            case 14:
                k2 ^= (ulong)tail[start + 13] << 40;
                goto case 13;
            case 13:
                k2 ^= (ulong)tail[start + 12] << 32;
                goto case 12;
            case 12:
                k2 ^= (ulong)tail[start + 11] << 24;
                goto case 11;
            case 11:
                k2 ^= (ulong)tail[start + 10] << 16;
                goto case 10;
            case 10:
                k2 ^= (ulong)tail[start + 9] << 8;
                goto case 9;
            case 9:
                k2 ^= (ulong)tail[start + 8] << 0;
                goto case 8;
            case 8:
                k1 ^= (ulong)tail[start + 7] << 56;
                goto case 7;
            case 7:
                k1 ^= (ulong)tail[start + 6] << 48;
                goto case 6;
            case 6:
                k1 ^= (ulong)tail[start + 5] << 40;
                goto case 5;
            case 5:
                k1 ^= (ulong)tail[start + 4] << 32;
                goto case 4;
            case 4:
                k1 ^= (ulong)tail[start + 3] << 24;
                goto case 3;
            case 3:
                k1 ^= (ulong)tail[start + 2] << 16;
                goto case 2;
            case 2:
                k1 ^= (ulong)tail[start + 1] << 8;
                goto case 1;
            case 1:
                k1 ^= (ulong)tail[start] << 0;
                break;
        }

        _H2 ^= RotateLeft(k2 * C2, 33) * C1;
        _H1 ^= RotateLeft(k1 * C1, 31) * C2;
    }

    /// <summary>
    /// 计算并返回最终的哈希值。
    /// </summary>
    /// <returns>哈希值的字节数组。</returns>
    protected override byte[] HashFinal()
    {
        var len = (ulong)_Length;
        _H1 ^= len;
        _H2 ^= len;

        _H1 += _H2;
        _H2 += _H1;

        _H1 = FMix(_H1);
        _H2 = FMix(_H2);

        _H1 += _H2;
        _H2 += _H1;

        var result = new byte[16];
        Array.Copy(BitConverter.GetBytes(_H1), 0, result, 0, 8);
        Array.Copy(BitConverter.GetBytes(_H2), 0, result, 8, 8);

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong RotateLeft(ulong x, byte r)
    {
        return x << r | x >> 64 - r;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong FMix(ulong h)
    {
        h = (h ^ h >> 33) * 0xff51afd7ed558ccd;
        h = (h ^ h >> 33) * 0xc4ceb9fe1a85ec53;

        return h ^ h >> 33;
    }
}
