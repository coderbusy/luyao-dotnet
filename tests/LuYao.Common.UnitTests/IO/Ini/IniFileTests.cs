using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LuYao.IO.Ini;

namespace LuYao.IO.Ini;

[TestClass]
public class IniFileTests
{
    private const string SampleIniContent = @"; 这是一个示例 INI 文件
[database]
host=localhost
port=5432
username=admin
password=secret123 ; 这是密码注释

[app_settings]
debug=true
max_connections=100

# 另一种注释格式
[logging]
level=info
file_path=/var/log/app.log

[empty_section]
";

    [TestMethod]
    public void Parse_ValidIniContent_ParsesCorrectly()
    {
        // Act
        var iniFile = IniFile.Parse(SampleIniContent);

        // Assert
        Assert.IsNotNull(iniFile);
        Assert.IsTrue(iniFile.Data.Count > 0);
    }

    [TestMethod]
    public void GetSectionNames_ReturnsAllSections()
    {
        // Arrange
        var iniFile = IniFile.Parse(SampleIniContent);

        // Act
        var sections = iniFile.GetSectionNames().ToList();

        // Assert
        Assert.AreEqual(4, sections.Count);
        Assert.IsTrue(sections.Contains("database"));
        Assert.IsTrue(sections.Contains("app_settings"));
        Assert.IsTrue(sections.Contains("logging"));
        Assert.IsTrue(sections.Contains("empty_section"));
    }

    [TestMethod]
    public void GetValue_ExistingKey_ReturnsCorrectValue()
    {
        // Arrange
        var iniFile = IniFile.Parse(SampleIniContent);

        // Act & Assert
        Assert.AreEqual("localhost", iniFile.GetValue("database", "host"));
        Assert.AreEqual("5432", iniFile.GetValue("database", "port"));
        Assert.AreEqual("admin", iniFile.GetValue("database", "username"));
        Assert.AreEqual("secret123", iniFile.GetValue("database", "password"));
        Assert.AreEqual("true", iniFile.GetValue("app_settings", "debug"));
        Assert.AreEqual("100", iniFile.GetValue("app_settings", "max_connections"));
    }

    [TestMethod]
    public void GetValue_NonExistentKey_ReturnsNull()
    {
        // Arrange
        var iniFile = IniFile.Parse(SampleIniContent);

        // Act & Assert
        Assert.IsNull(iniFile.GetValue("database", "nonexistent"));
        Assert.IsNull(iniFile.GetValue("nonexistent_section", "key"));
    }

    [TestMethod]
    public void GetSection_ExistingSection_ReturnsAllKeyValuePairs()
    {
        // Arrange
        var iniFile = IniFile.Parse(SampleIniContent);

        // Act
        var databaseSection = iniFile.GetSection("database");

        // Assert
        Assert.AreEqual(4, databaseSection.Count);
        Assert.AreEqual("localhost", databaseSection["host"]);
        Assert.AreEqual("5432", databaseSection["port"]);
        Assert.AreEqual("admin", databaseSection["username"]);
        Assert.AreEqual("secret123", databaseSection["password"]);
    }

    [TestMethod]
    public void GetSection_NonExistentSection_ReturnsEmptyDictionary()
    {
        // Arrange
        var iniFile = IniFile.Parse(SampleIniContent);

        // Act
        var section = iniFile.GetSection("nonexistent");

        // Assert
        Assert.AreEqual(0, section.Count);
    }

    [TestMethod]
    public void SetValue_ExistingKey_UpdatesValue()
    {
        // Arrange
        var iniFile = IniFile.Parse(SampleIniContent);

        // Act
        iniFile.SetValue("database", "host", "newhost");

        // Assert
        Assert.AreEqual("newhost", iniFile.GetValue("database", "host"));
    }

    [TestMethod]
    public void SetValue_NewKey_AddsKeyValuePair()
    {
        // Arrange
        var iniFile = IniFile.Parse(SampleIniContent);

        // Act
        iniFile.SetValue("database", "timeout", "30");

        // Assert
        Assert.AreEqual("30", iniFile.GetValue("database", "timeout"));
    }

    [TestMethod]
    public void ToString_ReturnsValidIniFormat()
    {
        // Arrange
        var iniFile = IniFile.Parse(SampleIniContent);

        // Act
        var result = iniFile.ToString();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Contains("[database]"));
        Assert.IsTrue(result.Contains("host=localhost"));
        Assert.IsTrue(result.Contains("[app_settings]"));
    }

    [TestMethod]
    public void Data_ContainsCorrectColumns()
    {
        // Arrange
        var iniFile = new IniFile();

        // Act & Assert
        Assert.AreEqual(4, iniFile.Data.Columns.Count);
        Assert.IsTrue(iniFile.Data.Contains("Section"));
        Assert.IsTrue(iniFile.Data.Contains("Key"));
        Assert.IsTrue(iniFile.Data.Contains("Value"));
        Assert.IsTrue(iniFile.Data.Contains("Comment"));
    }

    [TestMethod]
    public void Data_ContainsAllParsedData()
    {
        // Arrange
        var iniFile = IniFile.Parse(SampleIniContent);

        // Act & Assert
        Assert.IsTrue(iniFile.Data.Count > 0);
        
        // 检查是否包含数据库配置
        bool foundDatabaseHost = false;
        for (int i = 0; i < iniFile.Data.Count; i++)
        {
            var section = iniFile.Data.GetValue("Section", i)?.ToString();
            var key = iniFile.Data.GetValue("Key", i)?.ToString();
            var value = iniFile.Data.GetValue("Value", i)?.ToString();

            if (section == "database" && key == "host" && value == "localhost")
            {
                foundDatabaseHost = true;
                break;
            }
        }
        Assert.IsTrue(foundDatabaseHost);
    }

    [TestMethod]
    public void Read_FromFile_ParsesCorrectly()
    {
        // Arrange
        using var tempFile = AutoCleanTempFile.Create();
        File.WriteAllText(tempFile.FileName, SampleIniContent);

        // Act
        var iniFile = IniFile.Read(tempFile.FileName);

        // Assert
        Assert.IsNotNull(iniFile);
        Assert.AreEqual("localhost", iniFile.GetValue("database", "host"));
    }

    [TestMethod]
    public void Save_ToFile_CreatesValidFile()
    {
        // Arrange
        var iniFile = IniFile.Parse(SampleIniContent);
        using var tempFile = AutoCleanTempFile.Create();

        // Act
        iniFile.Save(tempFile.FileName);

        // Assert
        Assert.IsTrue(File.Exists(tempFile.FileName));
        var content = File.ReadAllText(tempFile.FileName);
        Assert.IsTrue(content.Contains("[database]"));
        Assert.IsTrue(content.Contains("host=localhost"));
    }

    [TestMethod]
    public void CaseInsensitive_SectionAndKeyLookup()
    {
        // Arrange
        var iniFile = IniFile.Parse(SampleIniContent);

        // Act & Assert
        Assert.AreEqual("localhost", iniFile.GetValue("database", "host"));
    }

    [TestMethod]
    public void EmptyIniFile_InitializesCorrectly()
    {
        // Arrange & Act
        var iniFile = new IniFile();

        // Assert
        Assert.IsNotNull(iniFile.Data);
        Assert.AreEqual(0, iniFile.Data.Count);
        Assert.AreEqual(4, iniFile.Data.Columns.Count);
    }

    [TestMethod]
    public void Parse_EmptyContent_ReturnsEmptyIniFile()
    {
        // Act
        var iniFile1 = IniFile.Parse("");
        var iniFile2 = IniFile.Parse(string.Empty);
        var iniFile3 = IniFile.Parse("   ");

        // Assert
        Assert.AreEqual(0, iniFile1.Data.Count);
        Assert.AreEqual(0, iniFile2.Data.Count);
        Assert.AreEqual(0, iniFile3.Data.Count);
    }
}
