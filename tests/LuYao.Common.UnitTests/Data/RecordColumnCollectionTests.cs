using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LuYao.Data;

/// <summary>
/// RecordColumnCollection 新语义单元测试：
/// - Find 不存在返回 null；
/// - Get 不存在抛 KeyNotFoundException；
/// - Add 同名同类型返回已存在列；
/// - Add 同名不同类型抛 InvalidOperationException；
/// - 实现 IReadOnlyList&lt;RecordColumn&gt; 而不再继承 List。
/// </summary>
[TestClass]
public class RecordColumnCollectionTests
{
    [TestMethod]
    public void Find_ColumnNotExist_ReturnsNull()
    {
        var table = new RecordTable();
        Assert.IsNull(table.Columns.Find("NoSuchColumn"));
    }

    [TestMethod]
    public void Find_ColumnExists_ReturnsColumn()
    {
        var table = new RecordTable();
        var added = table.Columns.Add<int>("Id");
        var found = table.Columns.Find("Id");
        Assert.AreSame(added, found);
    }

    [TestMethod]
    public void Get_ColumnNotExist_ThrowsKeyNotFoundException()
    {
        var table = new RecordTable();
        Assert.Throws<KeyNotFoundException>(() => table.Columns.Get("NoSuchColumn"));
    }

    [TestMethod]
    public void Get_ColumnExists_ReturnsColumn()
    {
        var table = new RecordTable();
        var added = table.Columns.Add<string>("Name");
        var got = table.Columns.Get("Name");
        Assert.AreSame(added, got);
    }

    [TestMethod]
    public void Add_SameNameSameType_ReturnsExistingInstance()
    {
        var table = new RecordTable();
        var first = table.Columns.Add<int>("Id");
        var second = table.Columns.Add<int>("Id");
        Assert.AreSame(first, second);
        Assert.AreEqual(1, table.Columns.Count);
    }

    [TestMethod]
    public void Add_SameNameSameType_NonGeneric_ReturnsExistingInstance()
    {
        var table = new RecordTable();
        var first = table.Columns.Add("Id", typeof(int));
        var second = table.Columns.Add("Id", typeof(int));
        Assert.AreSame(first, second);
        Assert.AreEqual(1, table.Columns.Count);
    }

    [TestMethod]
    public void Add_SameNameDifferentType_ThrowsInvalidOperationException()
    {
        var table = new RecordTable();
        table.Columns.Add<int>("Id");
        Assert.Throws<InvalidOperationException>(() => table.Columns.Add<string>("Id"));
    }

    [TestMethod]
    public void Add_SameNameDifferentType_NonGeneric_ThrowsInvalidOperationException()
    {
        var table = new RecordTable();
        table.Columns.Add("Id", typeof(int));
        Assert.Throws<InvalidOperationException>(() => table.Columns.Add("Id", typeof(string)));
    }

    [TestMethod]
    public void Collection_IsReadOnlyList()
    {
        var table = new RecordTable();
        table.Columns.Add<int>("Id");
        IReadOnlyList<RecordColumn> readOnly = table.Columns;
        Assert.AreEqual(1, readOnly.Count);
        Assert.AreEqual("Id", readOnly[0].Name);
    }

    [TestMethod]
    public void Collection_DoesNotInheritList()
    {
        // 显式破坏期望：不应再继承 List<RecordColumn>，避免 Add(RecordColumn)、Insert 等绕过校验。
        var table = new RecordTable();
        Assert.IsFalse(typeof(System.Collections.Generic.List<RecordColumn>).IsAssignableFrom(typeof(RecordColumnCollection)));
    }

    [TestMethod]
    public void IndexOf_ByName_ReturnsCorrectIndex()
    {
        var table = new RecordTable();
        table.Columns.Add<int>("A");
        table.Columns.Add<string>("B");
        table.Columns.Add<bool>("C");

        Assert.AreEqual(0, table.Columns.IndexOf("A"));
        Assert.AreEqual(1, table.Columns.IndexOf("B"));
        Assert.AreEqual(2, table.Columns.IndexOf("C"));
        Assert.AreEqual(-1, table.Columns.IndexOf("Missing"));
    }

    [TestMethod]
    public void Contains_Name_Works()
    {
        var table = new RecordTable();
        table.Columns.Add<int>("A");
        Assert.IsTrue(table.Columns.Contains("A"));
        Assert.IsFalse(table.Columns.Contains("B"));
    }

    [TestMethod]
    public void Remove_ByName_RemovesAndReturnsTrue()
    {
        var table = new RecordTable();
        table.Columns.Add<int>("A");
        Assert.IsTrue(table.Columns.Remove("A"));
        Assert.AreEqual(0, table.Columns.Count);
        Assert.IsFalse(table.Columns.Remove("A"));
    }

    [TestMethod]
    public void Rename_Works()
    {
        var table = new RecordTable();
        table.Columns.Add<int>("A");
        table.Columns.Rename("A", "B");
        Assert.IsFalse(table.Columns.Contains("A"));
        Assert.IsTrue(table.Columns.Contains("B"));
    }

    [TestMethod]
    public void Enumerate_PreservesInsertionOrder()
    {
        var table = new RecordTable();
        table.Columns.Add<int>("A");
        table.Columns.Add<string>("B");
        table.Columns.Add<bool>("C");
        var names = table.Columns.Select(c => c.Name).ToArray();
        CollectionAssert.AreEqual(new[] { "A", "B", "C" }, names);
    }
}
