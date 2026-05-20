using System;
using System.Collections.Generic;
using LuYao.Data.Meta;
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

    [TestMethod]
    public void Dynamic_SetMember_AutoCreatesColumnByRuntimeType()
    {
        var table = new RecordTable();
        dynamic dto = table.AddRow();
        dto.Id = 100;
        dto.Name = "abc";

        Assert.AreEqual(2, table.Columns.Count);
        Assert.AreEqual(typeof(int), table.Columns.Get("Id").Type);
        Assert.AreEqual(typeof(string), table.Columns.Get("Name").Type);

        var row = table[0];
        Assert.AreEqual(100, row.To<int>("Id"));
        Assert.AreEqual("abc", row.To<string>("Name"));
    }

    [TestMethod]
    public void Dynamic_SetMember_NullAndColumnMissing_IsSkipped()
    {
        var table = new RecordTable();
        dynamic dto = table.AddRow();
        dto.Tag = null;
        Assert.AreEqual(0, table.Columns.Count);
    }

    [TestMethod]
    public void Dynamic_SetMember_NullOnExistingColumn_IsWritten()
    {
        var table = new RecordTable();
        table.Columns.Add<string>("Name");
        dynamic dto = table.AddRow();
        dto.Name = "x";
        dto.Name = null;
        Assert.IsNull(((RecordRow)dto).To<string>("Name"));
    }

    [TestMethod]
    public void Dynamic_SetIndex_AutoCreatesColumn()
    {
        var table = new RecordTable();
        dynamic dto = table.AddRow();
        dto["Id"] = 7;
        Assert.AreEqual(1, table.Columns.Count);
        Assert.AreEqual(7, table[0].To<int>("Id"));
    }

    // ── MapTo(object) non-generic overload ─────────────────────────────────────────

    private class CopyToBase
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    private class CopyToDerived : CopyToBase
    {
        public double Score { get; set; }
    }

    [TestMethod]
    public void CopyTo_NonGeneric_FillsBaseProps()
    {
        var table = new RecordTable();
        table.Columns.Add<int>("Id");
        table.Columns.Add<string>("Name");
        var row = table.AddRow();
        table.Columns.Get("Id").Set(row, 10);
        table.Columns.Get("Name").Set(row, "Alice");

        var obj = new CopyToBase();
        row.MapTo((object)obj);

        Assert.AreEqual(10, obj.Id);
        Assert.AreEqual("Alice", obj.Name);
    }

    [TestMethod]
    public void CopyTo_NonGeneric_FillsDerivedPropsViaBaseRef()
    {
        var table = new RecordTable();
        table.Columns.Add<int>("Id");
        table.Columns.Add<double>("Score");
        var row = table.AddRow();
        table.Columns.Get("Id").Set(row, 20);
        table.Columns.Get("Score").Set(row, 9.9);

        // Base reference holds a derived instance; runtime type should be correctly identified
        CopyToBase obj = new CopyToDerived();
        row.MapTo((object)obj);

        Assert.AreEqual(20, obj.Id);
        Assert.AreEqual(9.9, ((CopyToDerived)obj).Score);
    }

    [TestMethod]
    public void CopyTo_NonGeneric_MissingColumn_SilentlySkips()
    {
        var table = new RecordTable();
        table.Columns.Add<int>("Id");
        var row = table.AddRow();
        table.Columns.Get("Id").Set(row, 5);

        var obj = new CopyToBase { Name = "original" };
        row.MapTo((object)obj);

        Assert.AreEqual(5, obj.Id);
        Assert.AreEqual("original", obj.Name); // Name column does not exist; value should remain unchanged
    }

    [TestMethod]
    public void CopyTo_NonGeneric_NullData_ThrowsArgumentNullException()
    {
        var table = new RecordTable();
        var row = table.AddRow();

        try
        {
            row.MapTo((object)null!);
            Assert.Fail("Expected ArgumentNullException");
        }
        catch (ArgumentNullException) { }
    }

    [TestMethod]
    public void Mapping_CopyFromObject_DoesNotAutoCreateColumns()
    {
        // Expected: missing column is silently skipped; no column is created
        var table = new RecordTable();
        table.Columns.Add<int>("Id"); // Only declare Id column; Name is intentionally absent
        var row = table.AddRow();
        row.MapFrom(new MappingDto { Id = 9, Name = "ignored" });

        Assert.AreEqual(1, table.Columns.Count);
        Assert.AreEqual(9, row.To<int>("Id"));
    }

    private sealed class MappingDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
