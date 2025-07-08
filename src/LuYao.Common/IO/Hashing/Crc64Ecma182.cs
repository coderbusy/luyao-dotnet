using System;
using System.Security.Cryptography;

namespace LuYao.IO.Hashing;

/// <summary>
/// 基于 ECMA-182 多项式的 CRC64 哈希算法实现。
/// </summary>
public class Crc64Ecma182 : HashAlgorithm
{
    private const ulong Polynomial = 0xC96C5795D7870F42; // ECMA-182 polynomial
    private static readonly ulong[] _table;

    private ulong _crc;

    /// <summary>
    /// 初始化 <see cref="Crc64Ecma182"/> 类的新实例。
    /// </summary>
    public Crc64Ecma182()
    {
        _crc = ulong.MaxValue;
    }

    static Crc64Ecma182()
    {
        _table = new ulong[256];
        for (ulong i = 0; i < 256; i++)
        {
            ulong crc = i;
            for (int j = 0; j < 8; j++)
            {
                crc = (crc & 1) == 1 ? (crc >> 1) ^ Polynomial : crc >> 1;
            }
            _table[i] = crc;
        }
    }

    /// <summary>
    /// 重置哈希算法以便重新使用。
    /// </summary>
    public override void Initialize()
    {
        _crc = ulong.MaxValue;
    }

    /// <summary>
    /// 处理输入字节数组的一部分并更新哈希状态。
    /// </summary>
    /// <param name="array">要计算哈希值的输入字节数组。</param>
    /// <param name="ibStart">字节数组中用于哈希计算的起始位置。</param>
    /// <param name="cbSize">用于哈希计算的字节数。</param>
    protected override void HashCore(byte[] array, int ibStart, int cbSize)
    {
        for (int i = ibStart; i < ibStart + cbSize; i++)
        {
            _crc = (_crc >> 8) ^ _table[(_crc & 0xFF) ^ array[i]];
        }
    }

    /// <summary>
    /// 在所有数据都被处理后，完成哈希计算并返回最终的哈希值。
    /// </summary>
    /// <returns>计算所得的哈希值字节数组。</returns>
    protected override byte[] HashFinal()
    {
        ulong hashValue = _crc ^ ulong.MaxValue;
        return BitConverter.GetBytes(hashValue);
    }

    /// <summary>
    /// 获取哈希值的大小（以位为单位）。
    /// </summary>
    public override int HashSize => 64; // HashSize 返回哈希值的大小（以位为单位）
}
