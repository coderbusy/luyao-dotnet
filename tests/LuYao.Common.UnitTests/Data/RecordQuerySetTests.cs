using System;
using System.Linq;

namespace LuYao.Data;

[TestClass]
public class RecordQuerySetTests
{
    private static Record CreateRecordA()
    {
        var record = new Record("A", 3);
        var idCol = record.Columns.Add<int>("Id");
        var nameCol = record.Columns.Add<string>("Name");

        var data = new[] { (1, "Alice"), (2, "Bob"), (3, "Charlie") };
        foreach (var (id, name) in data)
        {
            var row = record.AddRow();
            idCol.Set(id, row.Row);
            nameCol.Set(name, row.Row);
        }
        return record;
    }

    private static Record CreateRecordB()
    {
        var record = new Record("B", 3);
        var idCol = record.Columns.Add<int>("Id");
        var nameCol = record.Columns.Add<string>("Name");

        var data = new[] { (2, "Bob"), (3, "Charlie"), (4, "Diana") };
        foreach (var (id, name) in data)
        {
            var row = record.AddRow();
            idCol.Set(id, row.Row);
            nameCol.Set(name, row.Row);
        }
        return record;
    }

    #region Union

    [TestMethod]
    public void WhenUnionThenDeduplicates()
    {
        var a = CreateRecordA();
        var b = CreateRecordB();

        var result = a.AsQuery().Union(b).ToRecord();

        // {1,Alice}, {2,Bob}, {3,Charlie}, {4,Diana} = 4 unique
        Assert.AreEqual(4, result.Count);
    }

    [TestMethod]
    public void WhenUnionEmptyLeftThenReturnsRight()
    {
        var empty = new Record("Empty", 0);
        empty.Columns.Add<int>("Id");
        empty.Columns.Add<string>("Name");

        var b = CreateRecordB();

        var result = empty.AsQuery().Union(b).ToRecord();

        Assert.AreEqual(3, result.Count);
    }

    [TestMethod]
    public void WhenUnionEmptyRightThenReturnsLeft()
    {
        var a = CreateRecordA();
        var empty = new Record("Empty", 0);
        empty.Columns.Add<int>("Id");
        empty.Columns.Add<string>("Name");

        var result = a.AsQuery().Union(empty).ToRecord();

        Assert.AreEqual(3, result.Count);
    }

    [TestMethod]
    public void WhenUnionBothEmptyThenReturnsEmpty()
    {
        var a = new Record("A", 0);
        a.Columns.Add<int>("Id");
        var b = new Record("B", 0);
        b.Columns.Add<int>("Id");

        var result = a.AsQuery().Union(b).ToRecord();

        Assert.AreEqual(0, result.Count);
        Assert.AreEqual(1, result.Columns.Count);
    }

    [TestMethod]
    public void WhenUnionWithRecordQueryThenWorks()
    {
        var a = CreateRecordA();
        var b = CreateRecordB();

        var result = a.AsQuery()
            .Union(b.AsQuery().Where(r => r.Get<int>("Id") == 4))
            .ToRecord();

        // 3 from A + 1 new from B = 4
        Assert.AreEqual(4, result.Count);
    }

    #endregion

    #region UnionAll

    [TestMethod]
    public void WhenUnionAllThenKeepsAllRows()
    {
        var a = CreateRecordA();
        var b = CreateRecordB();

        var result = a.AsQuery().UnionAll(b).ToRecord();

        // 3 + 3 = 6
        Assert.AreEqual(6, result.Count);
    }

    [TestMethod]
    public void WhenUnionAllPreservesOrder()
    {
        var a = CreateRecordA();
        var b = CreateRecordB();

        var result = a.AsQuery().UnionAll(b).ToRecord();

        var idCol = result.Columns.Get("Id");
        Assert.AreEqual(1, idCol.GetValue(0));
        Assert.AreEqual(2, idCol.GetValue(1));
        Assert.AreEqual(3, idCol.GetValue(2));
        Assert.AreEqual(2, idCol.GetValue(3));
        Assert.AreEqual(3, idCol.GetValue(4));
        Assert.AreEqual(4, idCol.GetValue(5));
    }

    #endregion

    #region Intersect

    [TestMethod]
    public void WhenIntersectThenReturnsCommonRows()
    {
        var a = CreateRecordA();
        var b = CreateRecordB();

        var result = a.AsQuery().Intersect(b).ToRecord();

        // Common: {2,Bob}, {3,Charlie} = 2
        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void WhenIntersectNoOverlapThenReturnsEmpty()
    {
        var a = new Record("A", 1);
        a.Columns.Add<int>("Id");
        var row = a.AddRow();
        a.Columns[0].SetValue(1, row.Row);

        var b = new Record("B", 1);
        b.Columns.Add<int>("Id");
        row = b.AddRow();
        b.Columns[0].SetValue(2, row.Row);

        var result = a.AsQuery().Intersect(b).ToRecord();

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void WhenIntersectEmptyLeftThenReturnsEmpty()
    {
        var empty = new Record("Empty", 0);
        empty.Columns.Add<int>("Id");
        empty.Columns.Add<string>("Name");

        var b = CreateRecordB();

        var result = empty.AsQuery().Intersect(b).ToRecord();

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void WhenIntersectEmptyRightThenReturnsEmpty()
    {
        var a = CreateRecordA();
        var empty = new Record("Empty", 0);
        empty.Columns.Add<int>("Id");
        empty.Columns.Add<string>("Name");

        var result = a.AsQuery().Intersect(empty).ToRecord();

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void WhenIntersectDeduplicates()
    {
        var a = new Record("A", 3);
        var col = a.Columns.Add<int>("Id");
        a.AddRow(); col.Set(1, 0);
        a.AddRow(); col.Set(1, 1);
        a.AddRow(); col.Set(2, 2);

        var b = new Record("B", 2);
        var col2 = b.Columns.Add<int>("Id");
        b.AddRow(); col2.Set(1, 0);
        b.AddRow(); col2.Set(1, 1);

        var result = a.AsQuery().Intersect(b).ToRecord();

        // Only one {1}
        Assert.AreEqual(1, result.Count);
    }

    #endregion

    #region Except

    [TestMethod]
    public void WhenExceptThenReturnsDifference()
    {
        var a = CreateRecordA();
        var b = CreateRecordB();

        var result = a.AsQuery().Except(b).ToRecord();

        // A has {1,Alice} not in B
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(1, result.Columns.Get("Id").GetValue(0));
    }

    [TestMethod]
    public void WhenExceptEmptyRightThenReturnsAll()
    {
        var a = CreateRecordA();
        var empty = new Record("Empty", 0);
        empty.Columns.Add<int>("Id");
        empty.Columns.Add<string>("Name");

        var result = a.AsQuery().Except(empty).ToRecord();

        Assert.AreEqual(3, result.Count);
    }

    [TestMethod]
    public void WhenExceptEmptyLeftThenReturnsEmpty()
    {
        var empty = new Record("Empty", 0);
        empty.Columns.Add<int>("Id");
        empty.Columns.Add<string>("Name");

        var b = CreateRecordB();

        var result = empty.AsQuery().Except(b).ToRecord();

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void WhenExceptAllOverlapThenReturnsEmpty()
    {
        var a = CreateRecordA();

        var result = a.AsQuery().Except(a).ToRecord();

        Assert.AreEqual(0, result.Count);
    }

    #endregion

    #region Concat

    [TestMethod]
    public void WhenConcatThenAppendsAllRows()
    {
        var a = CreateRecordA();
        var b = CreateRecordB();

        var result = a.AsQuery().Concat(b).ToRecord();

        Assert.AreEqual(6, result.Count);
    }

    [TestMethod]
    public void WhenConcatEmptyThenReturnsOriginal()
    {
        var a = CreateRecordA();
        var empty = new Record("Empty", 0);
        empty.Columns.Add<int>("Id");
        empty.Columns.Add<string>("Name");

        var result = a.AsQuery().Concat(empty).ToRecord();

        Assert.AreEqual(3, result.Count);
    }

    #endregion

    #region Schema Compatibility

    [TestMethod]
    public void WhenUnionIncompatibleColumnCountThenThrowsOnToRecord()
    {
        var a = CreateRecordA();
        var b = new Record("B", 1);
        b.Columns.Add<int>("Id");
        b.AddRow();
        b.Columns[0].SetValue(1, 0);

        var query = a.AsQuery().Union(b);

        Assert.Throws<InvalidOperationException>(() => query.ToRecord());
    }

    [TestMethod]
    public void WhenUnionIncompatibleColumnTypeThenThrowsOnToRecord()
    {
        var a = new Record("A", 1);
        a.Columns.Add<int>("Id");
        a.AddRow();
        a.Columns[0].SetValue(1, 0);

        var b = new Record("B", 1);
        b.Columns.Add<string>("Id");
        b.AddRow();
        b.Columns[0].SetValue("x", 0);

        var query = a.AsQuery().Union(b);

        Assert.Throws<InvalidOperationException>(() => query.ToRecord());
    }

    [TestMethod]
    public void WhenIntersectIncompatibleSchemaThenThrowsOnToRecord()
    {
        var a = CreateRecordA();
        var b = new Record("B", 0);
        b.Columns.Add<int>("Id");

        var query = a.AsQuery().Intersect(b);

        Assert.Throws<InvalidOperationException>(() => query.ToRecord());
    }

    [TestMethod]
    public void WhenExceptIncompatibleSchemaThenThrowsOnToRecord()
    {
        var a = CreateRecordA();
        var b = new Record("B", 0);
        b.Columns.Add<int>("Id");

        var query = a.AsQuery().Except(b);

        Assert.Throws<InvalidOperationException>(() => query.ToRecord());
    }

    #endregion

    #region Null Arguments

    [TestMethod]
    public void WhenUnionNullThenThrows()
    {
        Assert.Throws<ArgumentNullException>(() => CreateRecordA().AsQuery().Union((Record)null!));
    }

    [TestMethod]
    public void WhenIntersectNullThenThrows()
    {
        Assert.Throws<ArgumentNullException>(() => CreateRecordA().AsQuery().Intersect((Record)null!));
    }

    [TestMethod]
    public void WhenExceptNullThenThrows()
    {
        Assert.Throws<ArgumentNullException>(() => CreateRecordA().AsQuery().Except((Record)null!));
    }

    [TestMethod]
    public void WhenConcatNullThenThrows()
    {
        Assert.Throws<ArgumentNullException>(() => CreateRecordA().AsQuery().Concat((Record)null!));
    }

    #endregion
}
