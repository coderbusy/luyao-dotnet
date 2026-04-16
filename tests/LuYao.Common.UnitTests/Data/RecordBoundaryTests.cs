using System;
using System.Linq;

namespace LuYao.Data;

[TestClass]
public class RecordBoundaryTests
{
    #region Null Values in Distinct / GroupBy

    [TestMethod]
    public void WhenDistinctWithAllNullColumnThenDeduplicatesCorrectly()
    {
        var record = new Record("Test", 3);
        var idCol = record.Columns.Add<int>("Id");
        record.Columns.Add<string>("Name");

        for (int i = 0; i < 3; i++)
        {
            var row = record.AddRow();
            idCol.Set(i, row.Row);
            // Name left as null for all rows
        }

        var result = record.AsQuery().Distinct("Name").ToRecord();

        // All nulls should deduplicate to 1 row
        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    public void WhenGroupByNullKeyColumnThenGroupsNullsTogether()
    {
        var record = new Record("Test", 4);
        var nameCol = record.Columns.Add<string>("Name");
        var valCol = record.Columns.Add<int>("Value");

        var row1 = record.AddRow();
        nameCol.Set("A", row1.Row);
        valCol.Set(10, row1.Row);

        var row2 = record.AddRow();
        // Name is null
        valCol.Set(20, row2.Row);

        var row3 = record.AddRow();
        // Name is null
        valCol.Set(30, row3.Row);

        var row4 = record.AddRow();
        nameCol.Set("A", row4.Row);
        valCol.Set(40, row4.Row);

        var result = record.AsQuery()
            .GroupBy(new[] { "Name" }, new AggregateDefinition(AggregateFunction.Sum, "Value", "Total"))
            .ToRecord();

        // 2 groups: "A" and null
        Assert.AreEqual(2, result.Count);
    }

    #endregion

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

    #region BuildRowKey Collision Resistance

    [TestMethod]
    public void WhenDistinctWithAmbiguousStringValuesThenNoFalseCollision()
    {
        var record = new Record("Test", 2);
        var col1 = record.Columns.Add<string>("A");
        var col2 = record.Columns.Add<string>("B");

        // These two rows previously could collide with simple separator join
        var r1 = record.AddRow();
        col1.Set("a\0b", r1.Row);
        col2.Set("c", r1.Row);

        var r2 = record.AddRow();
        col1.Set("a", r2.Row);
        col2.Set("b\0c", r2.Row);

        var result = record.AsQuery().Distinct().ToRecord();

        // Should remain 2 distinct rows
        Assert.AreEqual(2, result.Count);
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

    #region QueryOptions.Indexes Stability

    [TestMethod]
    public void WhenAsQueryWithIndexesThenDoesNotThrow()
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

        var options = new QueryOptions
        {
            EnableIndexing = true,
            Indexes = new[] { new[] { "Id" }, new[] { "Name" } }
        };

        var result = record.AsQuery(options).Where(r => r.Get<int>("Id") > 0).ToRecord();

        Assert.AreEqual(2, result.Count);
    }

    #endregion
}
