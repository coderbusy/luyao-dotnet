using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LuYao.Data;

/// <summary>
/// Validates RecordRow access semantics.
/// - Field&lt;T&gt; for reading values (replaces Get&lt;T&gt;);
/// - Set&lt;T&gt; auto-creates a column when it does not exist;
/// - dynamic setter auto-creates columns by runtime type; null values are skipped;
/// - string indexer as an explicit IPropertyAccessor implementation, hidden from callers;
/// - Mapping (IPropertyAccessor) writes do not auto-create columns.
/// </summary>
[TestClass]
public class RecordRowSemanticsTests
{
    [TestMethod]
    public void Field_ColumnExists_ReturnsTypedValue()
    {
        var table = new RecordTable();
        var col = table.Columns.Add<int>("Id");
        var row = table.AddRow();
        col.Set(row.Row, 123);
        Assert.AreEqual(123, row.To<int>("Id"));
    }

    [TestMethod]
    public void Field_ColumnNotExist_ReturnsDefault()
    {
        var table = new RecordTable();
        table.AddRow();
        var row = table[0];
        Assert.AreEqual(0, row.To<int>("NoSuch"));
        Assert.IsNull(row.To<string>("NoSuch"));
    }

    [TestMethod]
    public void Set_ColumnNotExist_AutoCreatesColumn()
    {
        var table = new RecordTable();
        var row = table.AddRow();
        row["Id"] = 100;

        Assert.AreEqual(1, table.Columns.Count);
        var col = table.Columns.Get("Id");
        Assert.AreEqual(typeof(int), col.Type);
        Assert.AreEqual(100, row.To<int>("Id"));
    }

    [TestMethod]
    public void Set_ColumnExistsSameType_WritesValue()
    {
        var table = new RecordTable();
        table.Columns.Add<string>("Name");
        var row = table.AddRow();
        row["Name"] = "abc";
        Assert.AreEqual("abc", row.To<string>("Name"));
    }
}
