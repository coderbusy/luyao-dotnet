using System;
using System.Linq;

namespace LuYao.Data;

[TestClass]
public class RecordBoundaryTests
{
    #region Large Row Count Expansion

    [TestMethod]
    public void WhenAddingManyRowsThenCapacityGrowsEfficiently()
    {
        var table = new RecordTable("Big", 0);
        var col = table.Columns.Add<int>("Id");

        const int rowCount = 10000;
        for (int i = 0; i < rowCount; i++)
        {
            var row = table.AddRow();
            col.SetValue(row.Row, i);
        }

        Assert.AreEqual(rowCount, table.Count);
        // Verify data integrity at boundaries
        Assert.AreEqual(0, col.To<int>(0));
        Assert.AreEqual(rowCount - 1, col.To<int>(rowCount - 1));
        Assert.AreEqual(5000, col.To<int>(5000));
    }

    #endregion

    #region Delete Consistency

    [TestMethod]
    public void WhenDeleteThenEnumerationIsConsistent()
    {
        var table = new RecordTable("Test", 5);
        var idCol = table.Columns.Add<int>("Id");

        for (int i = 0; i < 5; i++)
        {
            var row = table.AddRow();
            idCol.Set(row.Row, i * 10);
        }

        // Delete middle row (index 2, value 20)
        table.Delete(2);

        Assert.AreEqual(4, table.Count);
        var values = table.Select(r => r.To<int>("Id")).ToArray();
        CollectionAssert.AreEqual(new[] { 0, 10, 30, 40 }, values);
    }

    [TestMethod]
    public void WhenDeleteThenAddRowThenDataIsCorrect()
    {
        var table = new RecordTable("Test", 3);
        var idCol = table.Columns.Add<int>("Id");
        var nameCol = table.Columns.Add<string>("Name");

        for (int i = 0; i < 3; i++)
        {
            var row = table.AddRow();
            idCol.SetValue(row.Row, i);
            nameCol.SetValue(row.Row, $"Item{i}");
        }

        // Delete row 1
        table.Delete(1);
        Assert.AreEqual(2, table.Count);

        // Add a new row
        var newRow = table.AddRow();
        idCol.SetValue(newRow.Row, 99);
        nameCol.SetValue(newRow.Row, "New");

        Assert.AreEqual(3, table.Count);
        Assert.AreEqual(99, idCol.To<int>(2));
        Assert.AreEqual("New", nameCol.To<string>(2));
    }

    #endregion

    #region Batch Delete

    [TestMethod]
    public void WhenDeleteWhereThenMatchingRowsRemoved()
    {
        var table = new RecordTable("Test", 5);
        var idCol = table.Columns.Add<int>("Id");

        for (int i = 0; i < 5; i++)
        {
            var row = table.AddRow();
            idCol.Set(row.Row, i);
        }

        int deleted = table.DeleteWhere(r => r.To<int>("Id") % 2 == 0);

        Assert.AreEqual(3, deleted);
        Assert.AreEqual(2, table.Count);
        var remaining = table.Select(r => r.To<int>("Id")).ToArray();
        CollectionAssert.AreEqual(new[] { 1, 3 }, remaining);
    }

    [TestMethod]
    public void WhenDeleteRowsWithIndicesThenCorrectRowsRemoved()
    {
        var table = new RecordTable("Test", 5);
        var idCol = table.Columns.Add<int>("Id");

        for (int i = 0; i < 5; i++)
        {
            var row = table.AddRow();
            idCol.Set(row.Row, i * 10);
        }

        int deleted = table.DeleteRows(new[] { 0, 2, 4 });

        Assert.AreEqual(3, deleted);
        Assert.AreEqual(2, table.Count);
        var remaining = table.Select(r => r.To<int>("Id")).ToArray();
        CollectionAssert.AreEqual(new[] { 10, 30 }, remaining);
    }

    [TestMethod]
    public void WhenDeleteRowsWithDuplicateIndicesThenDeduplicates()
    {
        var table = new RecordTable("Test", 3);
        var idCol = table.Columns.Add<int>("Id");

        for (int i = 0; i < 3; i++)
        {
            var row = table.AddRow();
            idCol.SetValue(row.Row, i);
        }

        int deleted = table.DeleteRows(new[] { 1, 1, 1 });

        Assert.AreEqual(1, deleted);
        Assert.AreEqual(2, table.Count);
    }

    [TestMethod]
    public void WhenDeleteWhereNoMatchThenNothingDeleted()
    {
        var table = new RecordTable("Test", 3);
        var idCol = table.Columns.Add<int>("Id");

        for (int i = 0; i < 3; i++)
        {
            var row = table.AddRow();
            idCol.Set(row.Row, i);
        }

        int deleted = table.DeleteWhere(r => r.To<int>("Id") > 100);

        Assert.AreEqual(0, deleted);
        Assert.AreEqual(3, table.Count);
    }

    #endregion

    #region RecordRow.Set<T>

    [TestMethod]
    public void WhenRecordRowSetGenericThenValueSetCorrectly()
    {
        var table = new RecordTable("Test", 1);
        table.Columns.Add<int>("Id");
        table.Columns.Add<string>("Name");

        var row = table.AddRow();
        row["Id"] = 42;
        row["Name"] = "Test";

        Assert.AreEqual(42, row.To<int>("Id"));
        Assert.AreEqual("Test", row.To<string>("Name"));
    }

    #endregion
}
