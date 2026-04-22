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
            col.SetField(row.Row, i);
        }

        Assert.AreEqual(rowCount, record.Count);
        // Verify data integrity at boundaries
        Assert.AreEqual(0, col.To<int>(0));
        Assert.AreEqual(rowCount - 1, col.To<int>(rowCount - 1));
        Assert.AreEqual(5000, col.To<int>(5000));
    }

    #endregion

    #region Delete Consistency

    //[TestMethod]
    //public void WhenDeleteThenEnumerationIsConsistent()
    //{
    //    var record = new Record("Test", 5);
    //    var idCol = record.Columns.Add<int>("Id");

    //    for (int i = 0; i < 5; i++)
    //    {
    //        var row = record.AddRow();
    //        idCol.Set(row.Row, i * 10);
    //    }

    //    // Delete middle row (index 2, value 20)
    //    record.Delete(2);

    //    Assert.AreEqual(4, record.Count);
    //    var values = record.Select(r => r.Field<int>("Id")).ToArray();
    //    CollectionAssert.AreEqual(new[] { 0, 10, 30, 40 }, values);
    //}

    [TestMethod]
    public void WhenDeleteThenAddRowThenDataIsCorrect()
    {
        var record = new Record("Test", 3);
        var idCol = record.Columns.Add<int>("Id");
        var nameCol = record.Columns.Add<string>("Name");

        for (int i = 0; i < 3; i++)
        {
            var row = record.AddRow();
            idCol.SetField(row.Row, i);
            nameCol.SetField(row.Row, $"Item{i}");
        }

        // Delete row 1
        record.Delete(1);
        Assert.AreEqual(2, record.Count);

        // Add a new row
        var newRow = record.AddRow();
        idCol.SetField(newRow.Row, 99);
        nameCol.SetField(newRow.Row, "New");

        Assert.AreEqual(3, record.Count);
        Assert.AreEqual(99, idCol.To<int>(2));
        Assert.AreEqual("New", nameCol.To<string>(2));
    }

    #endregion

    #region Batch Delete

    //[TestMethod]
    //public void WhenDeleteWhereThenMatchingRowsRemoved()
    //{
    //    var record = new Record("Test", 5);
    //    var idCol = record.Columns.Add<int>("Id");

    //    for (int i = 0; i < 5; i++)
    //    {
    //        var row = record.AddRow();
    //        idCol.Set(row.Row, i);
    //    }

    //    int deleted = record.DeleteWhere(r => r.Field<int>("Id") % 2 == 0);

    //    Assert.AreEqual(3, deleted);
    //    Assert.AreEqual(2, record.Count);
    //    var remaining = record.Select(r => r.Field<int>("Id")).ToArray();
    //    CollectionAssert.AreEqual(new[] { 1, 3 }, remaining);
    //}

    //[TestMethod]
    //public void WhenDeleteRowsWithIndicesThenCorrectRowsRemoved()
    //{
    //    var record = new Record("Test", 5);
    //    var idCol = record.Columns.Add<int>("Id");

    //    for (int i = 0; i < 5; i++)
    //    {
    //        var row = record.AddRow();
    //        idCol.Set(row.Row, i * 10);
    //    }

    //    int deleted = record.DeleteRows(new[] { 0, 2, 4 });

    //    Assert.AreEqual(3, deleted);
    //    Assert.AreEqual(2, record.Count);
    //    var remaining = record.Select(r => r.Field<int>("Id")).ToArray();
    //    CollectionAssert.AreEqual(new[] { 10, 30 }, remaining);
    //}

    [TestMethod]
    public void WhenDeleteRowsWithDuplicateIndicesThenDeduplicates()
    {
        var record = new Record("Test", 3);
        var idCol = record.Columns.Add<int>("Id");

        for (int i = 0; i < 3; i++)
        {
            var row = record.AddRow();
            idCol.SetField(row.Row, i);
        }

        int deleted = record.DeleteRows(new[] { 1, 1, 1 });

        Assert.AreEqual(1, deleted);
        Assert.AreEqual(2, record.Count);
    }

    //[TestMethod]
    //public void WhenDeleteWhereNoMatchThenNothingDeleted()
    //{
    //    var record = new Record("Test", 3);
    //    var idCol = record.Columns.Add<int>("Id");

    //    for (int i = 0; i < 3; i++)
    //    {
    //        var row = record.AddRow();
    //        idCol.Set(row.Row, i);
    //    }

    //    int deleted = record.DeleteWhere(r => r.Field<int>("Id") > 100);

    //    Assert.AreEqual(0, deleted);
    //    Assert.AreEqual(3, record.Count);
    //}

    #endregion

    #region RecordRow.Set<T>

    //[TestMethod]
    //public void WhenRecordRowSetGenericThenValueSetCorrectly()
    //{
    //    var record = new Record("Test", 1);
    //    record.Columns.Add<int>("Id");
    //    record.Columns.Add<string>("Name");

    //    var row = record.AddRow();
    //    row.Set("Id", 42);
    //    row.Set("Name", "Test");

    //    Assert.AreEqual(42, row.Field<int>("Id"));
    //    Assert.AreEqual("Test", row.Field<string>("Name"));
    //}

    #endregion
}
