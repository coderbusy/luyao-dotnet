using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.IO.Hashing;

[TestClass]
public class Crc64Ecma182Tests
{
    [TestMethod]
    public void ComputeHash_EmptyArray_ReturnsCrcOfEmpty()
    {
        // Arrange
        var crc64 = new Crc64Ecma182();
        var data = Array.Empty<byte>();
        // ECMA-182 空数据的 CRC64 结果为 0x0000000000000000
        var expected = BitConverter.GetBytes(0x0000000000000000UL);

        // Act
        var actual = crc64.ComputeHash(data);

        // Assert
        CollectionAssert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void ComputeHash_SimpleAsciiString_ReturnsExpectedCrc()
    {
        // Arrange
        var crc64 = new Crc64Ecma182();
        var data = Encoding.ASCII.GetBytes("123456789");
        // ECMA-182 标准测试向量 "123456789" 的 CRC64 结果为 0x995DC9BBDF1939FA
        var expected = BitConverter.GetBytes(0x995DC9BBDF1939FAUL);

        // Act
        var actual = crc64.ComputeHash(data);

        // Assert
        CollectionAssert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Initialize_AfterComputeHash_ResetsState()
    {
        // Arrange
        var crc64 = new Crc64Ecma182();
        var data1 = Encoding.ASCII.GetBytes("abc");
        var data2 = Encoding.ASCII.GetBytes("abc");
        var hash1 = crc64.ComputeHash(data1);

        // Act
        crc64.Initialize();
        var hash2 = crc64.ComputeHash(data2);

        // Assert
        CollectionAssert.AreEqual(hash1, hash2);
    }

    [TestMethod]
    public void HashCore_PartialArray_CorrectResult()
    {
        // Arrange
        var crc64 = new Crc64Ecma182();
        var data = Encoding.ASCII.GetBytes("xx123456789yy");
        // 只计算 "123456789"
        int start = 2;
        int size = 9;
        var expected = BitConverter.GetBytes(0x995DC9BBDF1939FAUL);

        // Act
        crc64.Initialize();
        crc64.TransformBlock(data, start, size, null, 0);
        crc64.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
        var actual = crc64.Hash;

        // Assert
        CollectionAssert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void HashSize_Always_Returns64()
    {
        // Arrange
        var crc64 = new Crc64Ecma182();

        // Act
        var hashSize = crc64.HashSize;

        // Assert
        Assert.AreEqual(64, hashSize);
    }
}