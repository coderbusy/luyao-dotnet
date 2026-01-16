using Microsoft.VisualStudio.TestTools.UnitTesting;
using LuYao.Encoders;
using System;

namespace LuYao.Encoders.Tests
{
    [TestClass]
    public class Base16Tests
    {
        [TestMethod]
        public void ToBase16_EncodesBytesToHexString()
        {
            byte[] input = { 0x00, 0xAB, 0xCD, 0xEF, 0x12, 0x34, 0xFF };
            string expected = "00ABCDEF1234FF";
            string actual = Base16.ToBase16(input);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void FromBase16_DecodesHexStringToBytes()
        {
            string input = "00ABCDEF1234FF";
            byte[] expected = { 0x00, 0xAB, 0xCD, 0xEF, 0x12, 0x34, 0xFF };
            byte[] actual = Base16.FromBase16(input);
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ToBase16_EmptyArray_ReturnsEmptyString()
        {
            byte[] input = Array.Empty<byte>();
            string actual = Base16.ToBase16(input);
            Assert.AreEqual(string.Empty, actual);
        }

        [TestMethod]
        public void FromBase16_EmptyString_ReturnsEmptyArray()
        {
            string input = string.Empty;
            byte[] actual = Base16.FromBase16(input);
            Assert.AreEqual(0, actual.Length);
        }

        [TestMethod]
        public void FromBase16_InvalidHex_ThrowsFormatException()
        {
            string input = "ZZ";
            Assert.Throws<FormatException>(() => Base16.FromBase16(input));
        }

        [TestMethod]
        public void FromBase16_OddLength_ThrowsArgumentOutOfRangeException()
        {
            string input = "ABC";
            Assert.Throws<ArgumentOutOfRangeException>(() => Base16.FromBase16(input));
        }
    }
}