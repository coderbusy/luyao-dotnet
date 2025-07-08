using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LuYao.IO.Hashing;

[TestClass]
public class Murmur128Tests
{
    [TestMethod]
    public void Constructor_WithDefaultSeed_SeedIsZero()
    {
        var murmur = new Murmur128();
        Assert.AreEqual((uint)0, murmur.Seed);
    }

    [TestMethod]
    public void Constructor_WithCustomSeed_SeedIsSet()
    {
        uint seed = 123456;
        var murmur = new Murmur128(seed);
        Assert.AreEqual(seed, murmur.Seed);
    }

    [TestMethod]
    public void HashSize_Always_Returns128()
    {
        var murmur = new Murmur128();
        Assert.AreEqual(128, murmur.HashSize);
    }

    [TestMethod]
    public void Initialize_AfterHashing_StateIsReset()
    {
        var murmur = new Murmur128();
        var data = new byte[] { 1, 2, 3, 4 };
        murmur.ComputeHash(data);
        murmur.Initialize();
        var hashAfterReset = murmur.ComputeHash(data);
        var hashNew = new Murmur128().ComputeHash(data);
        CollectionAssert.AreEqual(hashNew, hashAfterReset);
    }

    [TestMethod]
    public void ComputeHash_EmptyInput_ReturnsConsistentHash()
    {
        var murmur = new Murmur128();
        var hash1 = murmur.ComputeHash(Array.Empty<byte>());
        var hash2 = murmur.ComputeHash(Array.Empty<byte>());
        CollectionAssert.AreEqual(hash1, hash2);
    }

    [TestMethod]
    public void ComputeHash_SameInputSameSeed_ReturnsSameHash()
    {
        var data = new byte[] { 10, 20, 30, 40, 50 };
        var murmur1 = new Murmur128(42);
        var murmur2 = new Murmur128(42);
        var hash1 = murmur1.ComputeHash(data);
        var hash2 = murmur2.ComputeHash(data);
        CollectionAssert.AreEqual(hash1, hash2);
    }

    [TestMethod]
    public void ComputeHash_DifferentSeeds_ReturnsDifferentHash()
    {
        var data = new byte[] { 10, 20, 30, 40, 50 };
        var murmur1 = new Murmur128(1);
        var murmur2 = new Murmur128(2);
        var hash1 = murmur1.ComputeHash(data);
        var hash2 = murmur2.ComputeHash(data);
        // 理论上不同种子应产生不同哈希
        Assert.IsFalse(hash1.SequenceEqual(hash2));
    }

    [TestMethod]
    public void ComputeHash_DifferentInput_ReturnsDifferentHash()
    {
        var murmur = new Murmur128();
        var hash1 = murmur.ComputeHash(new byte[] { 1, 2, 3 });
        var hash2 = murmur.ComputeHash(new byte[] { 1, 2, 4 });
        Assert.IsFalse(hash1.SequenceEqual(hash2));
    }

    [TestMethod]
    public void ComputeHash_InputLengthNotMultipleOf16_HandlesTailCorrectly()
    {
        var murmur = new Murmur128();
        var data = Enumerable.Range(0, 23).Select(i => (byte)i).ToArray(); // 23字节
        var hash = murmur.ComputeHash(data);
        Assert.AreEqual(16, hash.Length);
    }
}
