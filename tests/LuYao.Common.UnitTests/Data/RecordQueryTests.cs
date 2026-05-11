using System;
using System.Collections.Generic;
using System.Linq;

namespace LuYao.Data;

[TestClass]
public class RecordQueryTests
{
    private RecordTable CreateTestRecord()
    {
        var table = new RecordTable("TestRecord");
        table.Columns.Add("Id", typeof(int));
        table.Columns.Add("Name", typeof(string));
        table.Columns.Add("IsActive", typeof(bool));

        var row1 = table.AddRow();
        table.Columns["Id"].Set(row1, 1);
        table.Columns["Name"].Set(row1, "Alice");
        table.Columns["IsActive"].Set(row1, true);

        var row2 = table.AddRow();
        table.Columns["Id"].Set(row2, 2);
        table.Columns["Name"].Set(row2, "Bob");
        table.Columns["IsActive"].Set(row2, false);

        var row3 = table.AddRow();
        table.Columns["Id"].Set(row3, 3);
        table.Columns["Name"].Set(row3, "Charlie");
        table.Columns["IsActive"].Set(row3, true);

        return table;
    }

    [TestMethod]
    public void FindT_WithExistingValue_ReturnsFirstMatch()
    {
        // Arrange
        var table = CreateTestRecord();

        // Act
        var result = table.Find<bool>("IsActive", true);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Value.To<int>("Id"));
    }

    [TestMethod]
    public void FindT_WithNonExistingValue_ReturnsNull()
    {
        // Arrange
        var table = CreateTestRecord();

        // Act
        var result = table.Find<int>("Id", 99);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void FindT_WithNonExistingColumn_ReturnsNull()
    {
        // Arrange
        var table = CreateTestRecord();

        // Act
        var result = table.Find<string>("NonExisting", "Alice");

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void FindAllT_WithExistingValue_ReturnsAllMatches()
    {
        // Arrange
        var table = CreateTestRecord();

        // Act
        var results = table.FindAll<bool>("IsActive", true).ToList();

        // Assert
        Assert.AreEqual(2, results.Count);
        Assert.AreEqual(1, results[0].To<int>("Id"));
        Assert.AreEqual(3, results[1].To<int>("Id"));
    }

    [TestMethod]
    public void Find_WithValidPredicate_ReturnsFirstMatch()
    {
        // Arrange
        var table = CreateTestRecord();

        // Act
        var result = table.Find(r => r.To<string>("Name") == "Bob");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Value.To<int>("Id"));
    }

    [TestMethod]
    public void Find_WithInvalidPredicate_ReturnsNull()
    {
        // Arrange
        var table = CreateTestRecord();

        // Act
        var result = table.Find(r => r.To<int>("Id") > 10);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void FindAll_WithValidPredicate_ReturnsAllMatches()
    {
        // Arrange
        var table = CreateTestRecord();

        // Act
        var results = table.FindAll(r => r.To<bool>("IsActive")).ToList();

        // Assert
        Assert.AreEqual(2, results.Count);
        Assert.AreEqual(1, results[0].To<int>("Id"));
        Assert.AreEqual(3, results[1].To<int>("Id"));
    }

    [TestMethod]
    public void FindByDynamic_WithValidPredicate_ReturnsFirstMatch()
    {
        // Arrange
        var table = CreateTestRecord();

        // Act
        var result = table.FindByDynamic(d => d.Name == "Charlie");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(3, result.Value.To<int>("Id"));
    }

    [TestMethod]
    public void FindByDynamic_WithInvalidPredicate_ReturnsNull()
    {
        // Arrange
        var table = CreateTestRecord();

        // Act
        var result = table.FindByDynamic(d => d.Id == 99);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void FindAllByDynamic_WithValidPredicate_ReturnsAllMatches()
    {
        // Arrange
        var table = CreateTestRecord();

        // Act
        var results = table.FindAllByDynamic(d => d.IsActive == true).ToList();

        // Assert
        Assert.AreEqual(2, results.Count);
        Assert.AreEqual(1, results[0].To<int>("Id"));
        Assert.AreEqual(3, results[1].To<int>("Id"));
    }

    [TestMethod]
    public void Find_ObjectOverload_ReturnsFirstMatchingRow()
    {
        // Arrange
        var table = CreateTestRecord();

        // Act
        var result = table.Find("Name", (object)"Bob");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Value.To<int>("Id"));
    }

    [TestMethod]
    public void Find_ObjectOverload_ReturnsNullWhenColumnNotFound()
    {
        // Arrange
        var table = CreateTestRecord();

        // Act
        var result = table.Find("NotExist", (object)1);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Find_ObjectOverload_ReturnsNullWhenNoMatch()
    {
        // Arrange
        var table = CreateTestRecord();

        // Act
        var result = table.Find("Id", (object)99);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Find_ObjectOverload_SupportsNullValue()
    {
        // Arrange
        var table = new RecordTable();
        var colName = table.Columns.Add<string>("Name");
        var r1 = table.AddRow();
        colName.SetValue(r1.Row, "Alice");
        var r2 = table.AddRow();
        colName.SetValue(r2.Row, null);

        // Act
        var result = table.Find("Name", (object?)null);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNull(result.Value.To<string>("Name"));
    }

    [TestMethod]
    public void FindAll_ObjectOverload_ReturnsAllMatchingRows()
    {
        // Arrange
        var table = CreateTestRecord();

        // Act
        var results = table.FindAll("IsActive", (object)true).ToList();

        // Assert
        Assert.AreEqual(2, results.Count);
        Assert.AreEqual(1, results[0].To<int>("Id"));
        Assert.AreEqual(3, results[1].To<int>("Id"));
    }

    [TestMethod]
    public void FindAll_ObjectOverload_ReturnsEmptyWhenColumnNotFound()
    {
        // Arrange
        var table = CreateTestRecord();

        // Act
        var results = table.FindAll("NotExist", (object)1).ToList();

        // Assert
        Assert.AreEqual(0, results.Count);
    }

    [TestMethod]
    public void FindAll_ObjectOverload_ReturnsEmptyWhenNoMatch()
    {
        // Arrange
        var table = CreateTestRecord();

        // Act
        var results = table.FindAll("Id", (object)99).ToList();

        // Assert
        Assert.AreEqual(0, results.Count);
    }

    [TestMethod]
    public void FindAll_ObjectOverload_SupportsNullValue()
    {
        // Arrange
        var table = new RecordTable();
        var colName = table.Columns.Add<string>("Name");
        var r1 = table.AddRow();
        colName.SetValue(r1.Row, "Alice");
        var r2 = table.AddRow();
        colName.SetValue(r2.Row, null);
        var r3 = table.AddRow();
        colName.SetValue(r3.Row, null);

        // Act
        var results = table.FindAll("Name", (object?)null).ToList();

        // Assert
        Assert.AreEqual(2, results.Count);
        Assert.IsNull(results[0].To<string>("Name"));
        Assert.IsNull(results[1].To<string>("Name"));
    }
}
