using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace LuYao.IO.Hashing;

[TestClass]
public class HashAgentTests
{
    private readonly HashAgent _md5Agent = HashAgents.MD5;

    [TestMethod]
    public void Hash_String_NormalText_ReturnsCorrectHash()
    {
        // Arrange
        string input = "hello";
        // 预期值可通过在线工具或 .NET 计算得出
        string expected = "5d41402abc4b2a76b9719d911017c592";

        // Act
        string actual = _md5Agent.Hash(input);

        // Assert
        Assert.AreEqual(expected, actual, ignoreCase: true);
    }

    [TestMethod]
    public void Hash_ByteArray_NormalBytes_ReturnsCorrectHash()
    {
        // Arrange
        byte[] input = Encoding.UTF8.GetBytes("world");
        string expected = "7d793037a0760186574b0282f2f435e7";

        // Act
        string actual = _md5Agent.Hash(input);

        // Assert
        Assert.AreEqual(expected, actual, ignoreCase: true);
    }

    [TestMethod]
    public void Hash_Stream_NormalStream_ReturnsCorrectHash()
    {
        // Arrange
        string expected = "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855";
        using (var ms = new MemoryStream())
        {
            // Act
            string actual = HashAgents.SHA256.Hash(ms);

            // Assert
            Assert.AreEqual(expected, actual, ignoreCase: true);
        }
    }

    [TestMethod]
    public void HashFile_ValidFile_ReturnsCorrectHash()
    {
        // Arrange
        string tempFile = Path.GetTempFileName();
        try
        {
            File.WriteAllText(tempFile, "file");
            string expected = "8c7dd922ad47494fc02c388e12c00eac";

            // Act
            string actual = _md5Agent.HashFile(tempFile);

            // Assert
            Assert.AreEqual(expected, actual, ignoreCase: true);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [TestMethod]
    public void Hash_EmptyString_ReturnsCorrectHash()
    {
        // Arrange
        string input = "";
        string expected = "d41d8cd98f00b204e9800998ecf8427e";

        // Act
        string actual = _md5Agent.Hash(input);

        // Assert
        Assert.AreEqual(expected, actual, ignoreCase: true);
    }

    [TestMethod]
    public void Hash_NullString_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.ThrowsException<ArgumentNullException>(() => _md5Agent.Hash((string)null));
    }

    [TestMethod]
    public void Hash_NullByteArray_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.ThrowsException<ArgumentNullException>(() => _md5Agent.Hash((byte[])null));
    }

    [TestMethod]
    public void Hash_NullStream_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.ThrowsException<ArgumentNullException>(() => _md5Agent.Hash((Stream)null));
    }

    [TestMethod]
    public void HashFile_FileNotExist_ThrowsFileNotFoundException()
    {
        // Act & Assert
        Assert.ThrowsException<FileNotFoundException>(() => _md5Agent.HashFile("not_exist_file.txt"));
    }
}