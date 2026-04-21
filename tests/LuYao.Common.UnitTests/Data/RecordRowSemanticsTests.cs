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
        var record = new Record();
        var col = record.Columns.Add<int>("Id");
        var row = record.AddRow();
        col.Set(row.Row, 123);
        Assert.AreEqual(123, row.Field<int>("Id"));
    }

    [TestMethod]
    public void Field_ColumnNotExist_ReturnsDefault()
    {
        var record = new Record();
        record.AddRow();
        var row = record[0];
        Assert.AreEqual(0, row.Field<int>("NoSuch"));
        Assert.IsNull(row.Field<string>("NoSuch"));
    }

    [TestMethod]
    public void Set_ColumnNotExist_AutoCreatesColumn()
    {
        var record = new Record();
        var row = record.AddRow();
        row.Set<int>("Id", 100);

        Assert.AreEqual(1, record.Columns.Count);
        var col = record.Columns.Get("Id");
        Assert.AreEqual(typeof(int), col.Type);
        Assert.AreEqual(100, row.Field<int>("Id"));
    }

    [TestMethod]
    public void Set_ColumnExistsSameType_WritesValue()
    {
        var record = new Record();
        record.Columns.Add<string>("Name");
        var row = record.AddRow();
        row.Set<string>("Name", "abc");
        Assert.AreEqual("abc", row.Field<string>("Name"));
    }

    [TestMethod]
    public void Set_ColumnExistsDifferentType_Throws()
    {
        var record = new Record();
        record.Columns.Add<string>("Id");
        var row = record.AddRow();
        Assert.Throws<InvalidOperationException>(() => row.Set<int>("Id", 1));
    }

    [TestMethod]
    public void Dynamic_SetMember_AutoCreatesColumnByRuntimeType()
    {
        var record = new Record();
        dynamic dto = record.AddRow();
        dto.Id = 100;
        dto.Name = "abc";

        Assert.AreEqual(2, record.Columns.Count);
        Assert.AreEqual(typeof(int), record.Columns.Get("Id").Type);
        Assert.AreEqual(typeof(string), record.Columns.Get("Name").Type);

        var row = record[0];
        Assert.AreEqual(100, row.Field<int>("Id"));
        Assert.AreEqual("abc", row.Field<string>("Name"));
    }

    [TestMethod]
    public void Dynamic_SetMember_NullAndColumnMissing_IsSkipped()
    {
        var record = new Record();
        dynamic dto = record.AddRow();
        dto.Tag = null;
        Assert.AreEqual(0, record.Columns.Count);
    }

    [TestMethod]
    public void Dynamic_SetMember_NullOnExistingColumn_IsWritten()
    {
        var record = new Record();
        record.Columns.Add<string>("Name");
        dynamic dto = record.AddRow();
        dto.Name = "x";
        dto.Name = null;
        Assert.IsNull(((RecordRow)dto).Field<string>("Name"));
    }

    [TestMethod]
    public void Dynamic_SetIndex_AutoCreatesColumn()
    {
        var record = new Record();
        dynamic dto = record.AddRow();
        dto["Id"] = 7;
        Assert.AreEqual(1, record.Columns.Count);
        Assert.AreEqual(7, record[0].Field<int>("Id"));
    }


    [TestMethod]
    public void Mapping_CopyFromObject_DoesNotAutoCreateColumns()
    {
        // 预期：缺列静默跳过，不建列。
        var record = new Record();
        record.Columns.Add<int>("Id"); // 只声明 Id 列；Name 故意不建
        var row = record.AddRow();
        row.CopyFrom(new MappingDto { Id = 9, Name = "ignored" });

        Assert.AreEqual(1, record.Columns.Count);
        Assert.AreEqual(9, row.Field<int>("Id"));
    }

    private sealed class MappingDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
