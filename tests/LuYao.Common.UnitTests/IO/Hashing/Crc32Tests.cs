using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace LuYao.IO.Hashing;

[TestClass]
public class Crc32Tests
{
    [TestMethod]
    public void Compute_EmptyBuffer_ReturnsZero()
    {
        // 空数组 CRC32 结果应为 0
        var buffer = Array.Empty<byte>();
        var crc = Crc32.Compute(buffer);
        Assert.AreEqual(0u, crc);
    }

    [TestMethod]
    public void Compute_KnownAsciiString_ReturnsExpectedCrc()
    {
        // "123456789" 的 CRC32 标准值为 0xCBF43926
        var buffer = Encoding.ASCII.GetBytes("123456789");
        var crc = Crc32.Compute(buffer);
        Assert.AreEqual(0xCBF43926u, crc);
    }

    [TestMethod]
    public void Compute_CustomSeed_ReturnsDifferentResult()
    {
        var buffer = Encoding.ASCII.GetBytes("test");
        var crcDefault = Crc32.Compute(buffer);
        var crcCustomSeed = Crc32.Compute(0u, buffer);
        Assert.AreNotEqual(crcDefault, crcCustomSeed);
    }

    [TestMethod]
    public void Compute_CustomPolynomial_ReturnsDifferentResult()
    {
        var buffer = Encoding.ASCII.GetBytes("test");
        var crcDefault = Crc32.Compute(buffer);
        var crcCustomPoly = Crc32.Compute(0x1EDC6F41u, Crc32.DefaultSeed, buffer); // CRC-32C
        Assert.AreNotEqual(crcDefault, crcCustomPoly);
    }

    [TestMethod]
    public void Reflect_KnownValue_ReturnsExpectedResult()
    {
        // 0x3A (00111010) 反射 8 位应为 0x5C (01011100)
        uint input = 0x3A;
        uint expected = 0x5C;
        var reflected = Crc32.reflect(input, 8);
        Assert.AreEqual(expected, reflected);
    }

    [TestMethod]
    public void HashAlgorithm_StreamInput_ReturnsExpectedCrc()
    {
        // 使用 HashAlgorithm 接口处理流式数据
        var buffer = Encoding.ASCII.GetBytes("123456789");
        using (var crc32 = new Crc32())
        {
            crc32.Initialize();
            crc32.TransformBlock(buffer, 0, buffer.Length, buffer, 0);
            crc32.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
            var hash = crc32.Hash;
            // 结果应为 0xCBF43926
            Assert.IsNotNull(hash);
            Assert.AreEqual(4, hash.Length);
            Array.Reverse(hash);
            var value = BitConverter.ToUInt32(hash, 0);
            Assert.AreEqual(0xCBF43926u, value);
        }
    }

    [TestMethod]
    public void Constructor_BigEndian_ThrowsPlatformNotSupportedException()
    {
        // 仅在非小端平台抛出异常，通常不会触发
        if (!BitConverter.IsLittleEndian)
        {
            Assert.Throws<PlatformNotSupportedException>(() =>
            {
                var crc32 = new Crc32();
            });
        }
    }
}