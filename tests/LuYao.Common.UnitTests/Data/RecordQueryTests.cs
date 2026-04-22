using System;
using System.Collections.Generic;
using System.Linq;

namespace LuYao.Data;

[TestClass]
public class RecordQueryTests
{
    private Record CreateTestRecord()
    {
        var record = new Record("TestRecord");
        record.Columns.Add("Id", typeof(int));
        record.Columns.Add("Name", typeof(string));
        record.Columns.Add("IsActive", typeof(bool));

        var row1 = record.AddRow();
        record.Columns["Id"].Set(row1, 1);
        record.Columns["Name"].Set(row1, "Alice");
        record.Columns["IsActive"].Set(row1, true);

        var row2 = record.AddRow();
        record.Columns["Id"].Set(row2, 2);
        record.Columns["Name"].Set(row2, "Bob");
        record.Columns["IsActive"].Set(row2, false);

        var row3 = record.AddRow();
        record.Columns["Id"].Set(row3, 3);
        record.Columns["Name"].Set(row3, "Charlie");
        record.Columns["IsActive"].Set(row3, true);

        return record;
    }

    [TestMethod]
    public void FindT_WithExistingValue_ReturnsFirstMatch()
    {
        // Arrange
        var record = CreateTestRecord();

        // Act
        var result = record.Find<bool>("IsActive", true);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Value.To<int>("Id"));
    }

    [TestMethod]
    public void FindT_WithNonExistingValue_ReturnsNull()
    {
        // Arrange
        var record = CreateTestRecord();

        // Act
        var result = record.Find<int>("Id", 99);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void FindT_WithNonExistingColumn_ReturnsNull()
    {
        // Arrange
        var record = CreateTestRecord();

        // Act
        var result = record.Find<string>("NonExisting", "Alice");

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void FindAllT_WithExistingValue_ReturnsAllMatches()
    {
        // Arrange
        var record = CreateTestRecord();

        // Act
        var results = record.FindAll<bool>("IsActive", true).ToList();

        // Assert
        Assert.AreEqual(2, results.Count);
        Assert.AreEqual(1, results[0].To<int>("Id"));
        Assert.AreEqual(3, results[1].To<int>("Id"));
    }

    [TestMethod]
    public void Find_WithValidPredicate_ReturnsFirstMatch()
    {
        // Arrange
        var record = CreateTestRecord();

        // Act
        var result = record.Find(r => r.To<string>("Name") == "Bob");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Value.To<int>("Id"));
    }

    [TestMethod]
    public void Find_WithInvalidPredicate_ReturnsNull()
    {
        // Arrange
        var record = CreateTestRecord();

        // Act
        var result = record.Find(r => r.To<int>("Id") > 10);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void FindAll_WithValidPredicate_ReturnsAllMatches()
    {
        // Arrange
        var record = CreateTestRecord();

        // Act
        var results = record.FindAll(r => r.To<bool>("IsActive")).ToList();

        // Assert
        Assert.AreEqual(2, results.Count);
        Assert.AreEqual(1, results[0].To<int>("Id"));
        Assert.AreEqual(3, results[1].To<int>("Id"));
    }

    [TestMethod]
    public void FindByDynamic_WithValidPredicate_ReturnsFirstMatch()
    {
        // Arrange
        var record = CreateTestRecord();

        // Act
        var result = record.FindByDynamic(d => d.Name == "Charlie");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(3, result.Value.To<int>("Id"));
    }

    [TestMethod]
    public void FindByDynamic_WithInvalidPredicate_ReturnsNull()
    {
        // Arrange
        var record = CreateTestRecord();

        // Act
        var result = record.FindByDynamic(d => d.Id == 99);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void FindAllByDynamic_WithValidPredicate_ReturnsAllMatches()
    {
        // Arrange
        var record = CreateTestRecord();

        // Act
        var results = record.FindAllByDynamic(d => d.IsActive == true).ToList();

        // Assert
        Assert.AreEqual(2, results.Count);
        Assert.AreEqual(1, results[0].To<int>("Id"));
        Assert.AreEqual(3, results[1].To<int>("Id"));
    }
}
