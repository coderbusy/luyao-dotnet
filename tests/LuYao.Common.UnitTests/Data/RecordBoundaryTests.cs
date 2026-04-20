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
        var record = new Record("Big", 0);
        var col = record.Columns.Add<int>("Id");

        const int rowCount = 10000;
        for (int i = 0; i < rowCount; i++)
        {
            var row = record.AddRow();
            col.Set(i, row.Row);
        }

        Assert.AreEqual(rowCount, record.Count);
        // Verify data integrity at boundaries
        Assert.AreEqual(0, col.Get<int>(0));
        Assert.AreEqual(rowCount - 1, col.Get<int>(rowCount - 1));
        Assert.AreEqual(5000, col.Get<int>(5000));
    }

    #endregion

    #region Delete Consistency

    [TestMethod]
    public void WhenDeleteThenEnumerationIsConsistent()
    {
        var record = new Record("Test", 5);
        var idCol = record.Columns.Add<int>("Id");

        for (int i = 0; i < 5; i++)
        {
            var row = record.AddRow();
            idCol.Set(i * 10, row.Row);
        }

        // Delete middle row (index 2, value 20)
        record.Delete(2);

        Assert.AreEqual(4, record.Count);
        var values = record.Select(r => r.Get<int>("Id")).ToArray();
        CollectionAssert.AreEqual(new[] { 0, 10, 30, 40 }, values);
    }

    [TestMethod]
    public void WhenDeleteThenAddRowThenDataIsCorrect()
    {
        var record = new Record("Test", 3);
        var idCol = record.Columns.Add<int>("Id");
        var nameCol = record.Columns.Add<string>("Name");

        for (int i = 0; i < 3; i++)
        {
            var row = record.AddRow();
            idCol.Set(i, row.Row);
            nameCol.Set($"Item{i}", row.Row);
        }

        // Delete row 1
        record.Delete(1);
        Assert.AreEqual(2, record.Count);

        // Add a new row
        var newRow = record.AddRow();
        idCol.Set(99, newRow.Row);
        nameCol.Set("New", newRow.Row);

        Assert.AreEqual(3, record.Count);
        Assert.AreEqual(99, idCol.Get<int>(2));
        Assert.AreEqual("New", nameCol.Get<string>(2));
    }

    #endregion

    #region Batch Delete

    [TestMethod]
    public void WhenDeleteWhereThenMatchingRowsRemoved()
    {
        var record = new Record("Test", 5);
        var idCol = record.Columns.Add<int>("Id");

        for (int i = 0; i < 5; i++)
        {
            var row = record.AddRow();
            idCol.Set(i, row.Row);
        }

        int deleted = record.DeleteWhere(r => r.Get<int>("Id") % 2 == 0);

        Assert.AreEqual(3, deleted);
        Assert.AreEqual(2, record.Count);
        var remaining = record.Select(r => r.Get<int>("Id")).ToArray();
        CollectionAssert.AreEqual(new[] { 1, 3 }, remaining);
    }

    [TestMethod]
    public void WhenDeleteRowsWithIndicesThenCorrectRowsRemoved()
    {
        var record = new Record("Test", 5);
        var idCol = record.Columns.Add<int>("Id");

        for (int i = 0; i < 5; i++)
        {
            var row = record.AddRow();
            idCol.Set(i * 10, row.Row);
        }

        int deleted = record.DeleteRows(new[] { 0, 2, 4 });

        Assert.AreEqual(3, deleted);
        Assert.AreEqual(2, record.Count);
        var remaining = record.Select(r => r.Get<int>("Id")).ToArray();
        CollectionAssert.AreEqual(new[] { 10, 30 }, remaining);
    }

    [TestMethod]
    public void WhenDeleteRowsWithDuplicateIndicesThenDeduplicates()
    {
        var record = new Record("Test", 3);
        var idCol = record.Columns.Add<int>("Id");

        for (int i = 0; i < 3; i++)
        {
            var row = record.AddRow();
            idCol.Set(i, row.Row);
        }

        int deleted = record.DeleteRows(new[] { 1, 1, 1 });

        Assert.AreEqual(1, deleted);
        Assert.AreEqual(2, record.Count);
    }

    [TestMethod]
    public void WhenDeleteWhereNoMatchThenNothingDeleted()
    {
        var record = new Record("Test", 3);
        var idCol = record.Columns.Add<int>("Id");

        for (int i = 0; i < 3; i++)
        {
            var row = record.AddRow();
            idCol.Set(i, row.Row);
        }

        int deleted = record.DeleteWhere(r => r.Get<int>("Id") > 100);

        Assert.AreEqual(0, deleted);
        Assert.AreEqual(3, record.Count);
    }

    #endregion

    #region RecordRow.Set<T>

    [TestMethod]
    public void WhenRecordRowSetGenericThenValueSetCorrectly()
    {
        var record = new Record("Test", 1);
        record.Columns.Add<int>("Id");
        record.Columns.Add<string>("Name");

        var row = record.AddRow();
        row.Set("Id", 42);
        row.Set("Name", "Test");

        Assert.AreEqual(42, row.Get<int>("Id"));
        Assert.AreEqual("Test", row.Get<string>("Name"));
    }

    #endregion
}
