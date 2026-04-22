using System.Collections.Generic;
using System.Linq;

namespace LuYao.Data;

[TestClass]
public class RecordExtensionsTests
{
    [TestMethod]
    public void Group_WithExistingColumn_GroupsRowsByKey()
    {
        var record = new Record("TestRecord");
        record.Columns.Add("Category", typeof(int));
        record.Columns.Add("Name", typeof(string));

        var row1 = record.AddRow();
        record.Columns["Category"].Set(row1, 1);
        record.Columns["Name"].Set(row1, "A");

        var row2 = record.AddRow();
        record.Columns["Category"].Set(row2, 2);
        record.Columns["Name"].Set(row2, "B");

        var row3 = record.AddRow();
        record.Columns["Category"].Set(row3, 1);
        record.Columns["Name"].Set(row3, "C");

        Dictionary<int, List<RecordRow>> result = record.Group<int>("Category");

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual(2, result[1].Count);
        Assert.AreEqual(1, result[2].Count);
        Assert.AreEqual("A", result[1][0].To<string>("Name"));
        Assert.AreEqual("C", result[1][1].To<string>("Name"));
        Assert.AreEqual("B", result[2][0].To<string>("Name"));
    }

    [TestMethod]
    public void Group_WithMissingColumn_ReturnsSingleDefaultGroupContainingAllRows()
    {
        var record = new Record("TestRecord");
        record.Columns.Add("Id", typeof(int));

        var row1 = record.AddRow();
        record.Columns["Id"].Set(row1, 1);
        var row2 = record.AddRow();
        record.Columns["Id"].Set(row2, 2);

        Dictionary<int, List<RecordRow>> result = record.Group<int>("MissingColumn");

        Assert.AreEqual(1, result.Count);
        Assert.IsTrue(result.ContainsKey(default));
        Assert.AreEqual(2, result[default].Count);
        CollectionAssert.AreEqual(record.Select(r => r.To<int>("Id")).ToList(), result[default].Select(r => r.To<int>("Id")).ToList());
    }

    [TestMethod]
    public void Group_WithMissingColumnAndReferenceTypeKey_ThrowsInvalidOperationException()
    {
        var record = new Record("TestRecord");
        record.Columns.Add("Id", typeof(int));
        var row = record.AddRow();
        record.Columns["Id"].Set(row, 1);

        Assert.Throws<InvalidOperationException>(() => record.Group<string>("MissingColumn"));
    }
}
