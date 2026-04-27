using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LuYao.Data;

/// <summary>
/// FrameColumnCollection 新语义单元测试：
/// - Find 不存在返回 null；
/// - Get 不存在抛 KeyNotFoundException；
/// - Add 同名同类型返回已存在列；
/// - Add 同名不同类型抛 InvalidOperationException；
/// - 实现 IReadOnlyList&lt;FrameColumn&gt; 而不再继承 List。
/// </summary>
[TestClass]
public class FrameColumnCollectionTests
{
    [TestMethod]
    public void Find_ColumnNotExist_ReturnsNull()
    {
        var record = new Frame();
        Assert.IsNull(record.Columns.Find("NoSuchColumn"));
    }

    [TestMethod]
    public void Find_ColumnExists_ReturnsColumn()
    {
        var record = new Frame();
        var added = record.Columns.Add<int>("Id");
        var found = record.Columns.Find("Id");
        Assert.AreSame(added, found);
    }

    [TestMethod]
    public void Get_ColumnNotExist_ThrowsKeyNotFoundException()
    {
        var record = new Frame();
        Assert.Throws<KeyNotFoundException>(() => record.Columns.Get("NoSuchColumn"));
    }

    [TestMethod]
    public void Get_ColumnExists_ReturnsColumn()
    {
        var record = new Frame();
        var added = record.Columns.Add<string>("Name");
        var got = record.Columns.Get("Name");
        Assert.AreSame(added, got);
    }

    [TestMethod]
    public void Add_SameNameSameType_ReturnsExistingInstance()
    {
        var record = new Frame();
        var first = record.Columns.Add<int>("Id");
        var second = record.Columns.Add<int>("Id");
        Assert.AreSame(first, second);
        Assert.AreEqual(1, record.Columns.Count);
    }

    [TestMethod]
    public void Add_SameNameSameType_NonGeneric_ReturnsExistingInstance()
    {
        var record = new Frame();
        var first = record.Columns.Add("Id", typeof(int));
        var second = record.Columns.Add("Id", typeof(int));
        Assert.AreSame(first, second);
        Assert.AreEqual(1, record.Columns.Count);
    }

    [TestMethod]
    public void Add_SameNameDifferentType_ThrowsInvalidOperationException()
    {
        var record = new Frame();
        record.Columns.Add<int>("Id");
        Assert.Throws<InvalidOperationException>(() => record.Columns.Add<string>("Id"));
    }

    [TestMethod]
    public void Add_SameNameDifferentType_NonGeneric_ThrowsInvalidOperationException()
    {
        var record = new Frame();
        record.Columns.Add("Id", typeof(int));
        Assert.Throws<InvalidOperationException>(() => record.Columns.Add("Id", typeof(string)));
    }

    [TestMethod]
    public void Collection_IsReadOnlyList()
    {
        var record = new Frame();
        record.Columns.Add<int>("Id");
        IReadOnlyList<FrameColumn> readOnly = record.Columns;
        Assert.AreEqual(1, readOnly.Count);
        Assert.AreEqual("Id", readOnly[0].Name);
    }

    [TestMethod]
    public void Collection_DoesNotInheritList()
    {
        // 显式破坏期望：不应再继承 List<FrameColumn>，避免 Add(FrameColumn)、Insert 等绕过校验。
        var record = new Frame();
        Assert.IsFalse(typeof(System.Collections.Generic.List<FrameColumn>).IsAssignableFrom(typeof(FrameColumnCollection)));
    }

    [TestMethod]
    public void IndexOf_ByName_ReturnsCorrectIndex()
    {
        var record = new Frame();
        record.Columns.Add<int>("A");
        record.Columns.Add<string>("B");
        record.Columns.Add<bool>("C");

        Assert.AreEqual(0, record.Columns.IndexOf("A"));
        Assert.AreEqual(1, record.Columns.IndexOf("B"));
        Assert.AreEqual(2, record.Columns.IndexOf("C"));
        Assert.AreEqual(-1, record.Columns.IndexOf("Missing"));
    }

    [TestMethod]
    public void Contains_Name_Works()
    {
        var record = new Frame();
        record.Columns.Add<int>("A");
        Assert.IsTrue(record.Columns.Contains("A"));
        Assert.IsFalse(record.Columns.Contains("B"));
    }

    [TestMethod]
    public void Remove_ByName_RemovesAndReturnsTrue()
    {
        var record = new Frame();
        record.Columns.Add<int>("A");
        Assert.IsTrue(record.Columns.Remove("A"));
        Assert.AreEqual(0, record.Columns.Count);
        Assert.IsFalse(record.Columns.Remove("A"));
    }

    [TestMethod]
    public void Rename_Works()
    {
        var record = new Frame();
        record.Columns.Add<int>("A");
        record.Columns.Rename("A", "B");
        Assert.IsFalse(record.Columns.Contains("A"));
        Assert.IsTrue(record.Columns.Contains("B"));
    }

    [TestMethod]
    public void Enumerate_PreservesInsertionOrder()
    {
        var record = new Frame();
        record.Columns.Add<int>("A");
        record.Columns.Add<string>("B");
        record.Columns.Add<bool>("C");
        var names = record.Columns.Select(c => c.Name).ToArray();
        CollectionAssert.AreEqual(new[] { "A", "B", "C" }, names);
    }
}
