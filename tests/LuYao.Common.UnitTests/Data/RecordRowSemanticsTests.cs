using System;
using System.Collections.Generic;
using LuYao.Data.Meta;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LuYao.Data;

/// <summary>
/// 验证 RecordRow 新的访问语义：
/// - Field&lt;T&gt; 取值（替代旧 Get&lt;T&gt;）；
/// - Set&lt;T&gt; 写入时若列不存在自动建列；
/// - dynamic 写入时按运行时类型自动建列，null 值跳过；
/// - 字符串索引器作为 IPropertyAccessor 的显式实现，对调用方隐藏；
/// - Mapping（IPropertyAccessor）写入不会自动建列。
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

    [TestMethod]
    public void Mapping_CopyFromObject_DoesNotAutoCreateColumns()
    {
        // 预期：缺列静默跳过，不建列。
        var table = new RecordTable();
        table.Columns.Add<int>("Id"); // 只声明 Id 列；Name 故意不建
        var row = table.AddRow();
        row.CopyFrom(new MappingDto { Id = 9, Name = "ignored" });

        Assert.AreEqual(1, table.Columns.Count);
        Assert.AreEqual(9, row.To<int>("Id"));
    }

    private sealed class MappingDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
