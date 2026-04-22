using System;

namespace LuYao.Data;

[TestClass]
public class RecordMappingTests
{
    private sealed class TestModel
    {
        public int Id { get; set; }

        public string? Name { get; set; }
    }

    [TestMethod]
    public void To_WithRows_ShouldMapFromFirstRow()
    {
        // Arrange
        var record = new Record("Users", 2);
        var idColumn = record.Columns.Add<int>("Id");
        var nameColumn = record.Columns.Add<string>("Name");

        var row1 = record.AddRow();
        idColumn.SetValue(row1.Row, 1);
        nameColumn.SetValue(row1.Row, "Alice");

        var row2 = record.AddRow();
        idColumn.SetValue(row2.Row, 2);
        nameColumn.SetValue(row2.Row, "Bob");

        // Act
        var model = record.To<TestModel>();

        // Assert
        Assert.AreEqual(1, model.Id);
        Assert.AreEqual("Alice", model.Name);
    }

    [TestMethod]
    public void To_WithoutRows_ShouldReturnDefaultConstructedInstance()
    {
        // Arrange
        var record = new Record("Users", 0);
        record.Columns.Add<int>("Id");
        record.Columns.Add<string>("Name");

        // Act
        var model = record.To<TestModel>();

        // Assert
        Assert.IsNotNull(model);
        Assert.AreEqual(0, model.Id);
        Assert.IsNull(model.Name);
    }
}
