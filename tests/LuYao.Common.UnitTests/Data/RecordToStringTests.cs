using System;
using System.Linq;

namespace LuYao.Data;

[TestClass]
public class RecordToStringTests
{
    [TestMethod]
    public void WhenEmptyRecordThenContainsCountAndColumnInfo()
    {
        // Arrange
        var table = new RecordTable();

        // Act
        var result = table.ToString();

        // Assert
        StringAssert.Contains(result, "count 0 column 0");
    }

    [TestMethod]
    public void WhenNoNameThenShowsNone()
    {
        // Arrange
        var table = new RecordTable();

        // Act
        var result = table.ToString();

        // Assert
        Assert.IsTrue(result.StartsWith("None "));
    }

    [TestMethod]
    public void WhenHasNameThenShowsName()
    {
        // Arrange
        var table = new RecordTable("Users");

        // Act
        var result = table.ToString();

        // Assert
        Assert.IsTrue(result.StartsWith("Users "));
    }

    [TestMethod]
    public void WhenZeroColumnsAndZeroRowsThenDoesNotThrow()
    {
        // Arrange
        var table = new RecordTable();

        // Act
        var result = table.ToString();

        // Assert
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void WhenColumnsButZeroRowsThenDoesNotThrow()
    {
        // Arrange
        var table = new RecordTable();
        table.Columns.Add<int>("Id");
        table.Columns.Add<string>("Name");

        // Act
        var result = table.ToString();

        // Assert
        StringAssert.Contains(result, "count 0 column 2");
    }

    [TestMethod]
    public void WhenOneRowAndZeroColumnsThenDoesNotThrow()
    {
        // Arrange — 通过 DataTable 构造一个有行但无列的情况不可行，直接 AddRow
        var table = new RecordTable();
        table.AddRow();

        // Act
        var result = table.ToString();

        // Assert
        StringAssert.Contains(result, "count 1 column 0");
    }

    [TestMethod]
    public void WhenOneRowThenOutputsVerticalLayout()
    {
        // Arrange
        var table = new RecordTable("Test");
        table.Columns.Add<int>("Id");
        table.Columns.Add<string>("Name");
        var row = table.AddRow();
        table.Columns[0].Set(row, 1);
        table.Columns[1].Set(row, "Alice");

        // Act
        var result = table.ToString();

        // Assert — 单行模式每列一行，格式为 "ColName | Value"
        StringAssert.Contains(result, "| 1");
        StringAssert.Contains(result, "| Alice");
    }

    [TestMethod]
    public void WhenMultipleRowsThenOutputsTableWithSeparator()
    {
        // Arrange
        var table = new RecordTable("Test");
        table.Columns.Add<int>("Id");
        table.Columns.Add<string>("Name");
        for (int i = 0; i < 2; i++)
        {
            var row = table.AddRow();
            table.Columns[0].Set(row, i);
            table.Columns[1].Set(row, "R" + i);
        }

        // Act
        var result = table.ToString();
        var lines = result.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

        // Assert — 应包含表头、分隔线、数据行
        Assert.IsTrue(lines.Any(l => l.Contains("Id") && l.Contains("Name")), "应包含表头");
        Assert.IsTrue(lines.Any(l => l.Contains("-+-")), "应包含分隔线");
    }

    [TestMethod]
    public void WhenNullValueThenOutputsEmptyString()
    {
        // Arrange
        var table = new RecordTable();
        table.Columns.Add<string>("Val");
        var r1 = table.AddRow();
        var r2 = table.AddRow();
        table.Columns[0].Set(r1, "A");
        // r2 不设置值，保持 null

        // Act
        var result = table.ToString();

        // Assert — 不应抛出异常
        Assert.IsNotNull(result);
        StringAssert.Contains(result, "A");
    }

    [TestMethod]
    public void WhenSingleColumnThenNoSeparatorPrefix()
    {
        // Arrange
        var table = new RecordTable();
        table.Columns.Add<int>("X");
        var r1 = table.AddRow();
        var r2 = table.AddRow();
        table.Columns[0].Set(r1, 1);
        table.Columns[0].Set(r2, 2);

        // Act
        var result = table.ToString();
        var lines = result.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

        // Assert — 数据行不应以 " | " 开头
        foreach (var line in lines.Skip(1)) // 跳过摘要行
        {
            Assert.IsFalse(line.StartsWith(" | "), $"行不应以分隔符开头: '{line}'");
        }
    }

    [TestMethod]
    public void WhenLongValueThenTruncatedWithDots()
    {
        // Arrange
        var table = new RecordTable();
        table.Columns.Add<string>("Data");
        var r1 = table.AddRow();
        var r2 = table.AddRow();
        table.Columns[0].Set(r1, new string('A', 100));
        table.Columns[0].Set(r2, "short");

        // Act
        var result = table.ToString();

        // Assert — 过长的值应被截断并以 ".." 结尾
        StringAssert.Contains(result, "..");
        // 原始的 100 个 A 不应完整出现
        Assert.IsFalse(result.Contains(new string('A', 100)));
    }
}
