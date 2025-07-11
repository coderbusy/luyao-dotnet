namespace LuYao.IO.Hosts;

[TestClass]
public class HostFileTests
{
    [TestMethod]
    public void Read_FileExists_ReturnsParsedHostFile()
    {
        // Arrange
        using (var temp = AutoCleanTempFile.Create())
        {
            var tempFile = temp.FileName;

            File.WriteAllText(tempFile, "127.0.0.1 localhost\n# Comment line");

            // Act
            var hostFile = HostFile.Read(tempFile);

            // Assert
            Assert.AreEqual(2, hostFile.Lines.Count);

            Assert.AreEqual(HostLineType.Record, hostFile.Lines[0].LineType);
            if (hostFile.Lines[0] is RecordLine recordLine)
            {
                Assert.AreEqual("127.0.0.1", recordLine.IPAddress);
                Assert.AreEqual("localhost", recordLine.Domain);
            }

            Assert.AreEqual(HostLineType.Comment, hostFile.Lines[1].LineType);
            if (hostFile.Lines[1] is CommentLine commentLine)
            {
                Assert.AreEqual("# Comment line", commentLine.Comment);
            }
        }
    }

    [TestMethod]
    public void Read_FileDoesNotExist_ReturnsEmptyHostFile()
    {
        // Arrange
        var nonExistentFile = Path.Combine(Path.GetTempPath(), "nonexistentfile.txt");

        // Act
        var hostFile = HostFile.Read(nonExistentFile);

        // Assert
        Assert.AreEqual(0, hostFile.Lines.Count);
    }

    [TestMethod]
    public void Read_LinesContainsValidAndInvalidLines_ParsesCorrectly()
    {
        // Arrange
        var lines = new List<string>
        {
            "127.0.0.1 localhost",
            "# Comment line",
            "Invalid line"
        };

        // Act
        var hostFile = HostFile.Read(lines);

        // Assert
        Assert.AreEqual(3, hostFile.Lines.Count);

        Assert.AreEqual(HostLineType.Record, hostFile.Lines[0].LineType);
        if (hostFile.Lines[0] is RecordLine recordLine)
        {
            Assert.AreEqual("127.0.0.1", recordLine.IPAddress);
            Assert.AreEqual("localhost", recordLine.Domain);
        }

        Assert.AreEqual(HostLineType.Comment, hostFile.Lines[1].LineType);
        if (hostFile.Lines[1] is CommentLine commentLine)
        {
            Assert.AreEqual("# Comment line", commentLine.Comment);
        }

        Assert.AreEqual(HostLineType.Invalid, hostFile.Lines[2].LineType);
        if (hostFile.Lines[2] is InvalidLine invalidLine)
        {
            Assert.AreEqual("Invalid line", invalidLine.Raw);
        }
    }

    [TestMethod]
    public void Read_TextReaderContainsValidLines_ReturnsParsedHostFile()
    {
        // Arrange
        var content = "127.0.0.1 localhost\n# Comment line";
        using var reader = new StringReader(content);

        // Act
        var hostFile = HostFile.Read(reader);

        // Assert
        Assert.AreEqual(2, hostFile.Lines.Count);

        Assert.AreEqual(HostLineType.Record, hostFile.Lines[0].LineType);
        if (hostFile.Lines[0] is RecordLine recordLine)
        {
            Assert.AreEqual("127.0.0.1", recordLine.IPAddress);
            Assert.AreEqual("localhost", recordLine.Domain);
        }

        Assert.AreEqual(HostLineType.Comment, hostFile.Lines[1].LineType);
        if (hostFile.Lines[1] is CommentLine commentLine)
        {
            Assert.AreEqual("# Comment line", commentLine.Comment);
        }
    }

    [TestMethod]
    public void Parse_EmptyContent_ReturnsEmptyHostFile()
    {
        // Arrange
        var content = string.Empty;

        // Act
        var hostFile = HostFile.Parse(content);

        // Assert
        Assert.AreEqual(0, hostFile.Lines.Count);
    }

    [TestMethod]
    public void Parse_ValidContent_ReturnsParsedHostFile()
    {
        // Arrange
        var content = "127.0.0.1 localhost\n# Comment line";

        // Act
        var hostFile = HostFile.Parse(content);

        // Assert
        Assert.AreEqual(2, hostFile.Lines.Count);

        Assert.AreEqual(HostLineType.Record, hostFile.Lines[0].LineType);
        if (hostFile.Lines[0] is RecordLine recordLine)
        {
            Assert.AreEqual("127.0.0.1", recordLine.IPAddress);
            Assert.AreEqual("localhost", recordLine.Domain);
        }

        Assert.AreEqual(HostLineType.Comment, hostFile.Lines[1].LineType);
        if (hostFile.Lines[1] is CommentLine commentLine)
        {
            Assert.AreEqual("# Comment line", commentLine.Comment);
        }
    }

    [TestMethod]
    public void ToString_HostFileWithLines_ReturnsCorrectStringRepresentation()
    {
        // Arrange
        var lines = new List<string>
        {
            "127.0.0.1 localhost",
            "# Comment line"
        };
        var hostFile = HostFile.Read(lines);

        // Act
        var result = hostFile.ToString();

        // Assert
        var expected = "127.0.0.1\tlocalhost" + Environment.NewLine + "# Comment line";
        Assert.AreEqual(expected, result);
    }
}