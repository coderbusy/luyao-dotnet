using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LuYao.IO;

[TestClass]
public class AutoCleanTempFileTests
{
    [TestMethod]
    public void Constructor_FileNameIsNullOrWhiteSpace_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() => new AutoCleanTempFile(null));
        Assert.ThrowsException<ArgumentException>(() => new AutoCleanTempFile(""));
        Assert.ThrowsException<ArgumentException>(() => new AutoCleanTempFile("   "));
    }

    [TestMethod]
    public void Create_Always_ReturnsInstanceAndFileExists()
    {
        using (var tempFile = AutoCleanTempFile.Create())
        {
            Assert.IsNotNull(tempFile);
            Assert.IsTrue(File.Exists(tempFile.FileName));
        }
    }

    [TestMethod]
    public void OpenWrite_ValidFile_CanWriteAndReadContent()
    {
        const string testContent = "Hello, AutoCleanTempFile!";
        using (var tempFile = AutoCleanTempFile.Create())
        {
            using (var writer = new StreamWriter(tempFile.OpenWrite()))
            {
                writer.Write(testContent);
            }

            using (var reader = new StreamReader(tempFile.OpenRead()))
            {
                var content = reader.ReadToEnd();
                Assert.AreEqual(testContent, content);
            }
        }
    }

    [TestMethod]
    public void Dispose_AfterDispose_FileIsDeleted()
    {
        string fileName;
        using (var tempFile = AutoCleanTempFile.Create())
        {
            fileName = tempFile.FileName;
            Assert.IsTrue(File.Exists(fileName));
        }
        Assert.IsFalse(File.Exists(fileName));
    }

    [TestMethod]
    public void Dispose_MultipleCalls_NoExceptionThrown()
    {
        var tempFile = AutoCleanTempFile.Create();
        tempFile.Dispose();
        tempFile.Dispose();
    }
}