using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LuYao.Data;

[TestClass]
public class RecordSchemaOperationsTests
{
    private static RecordTable CreateTestRecord()
    {
        var table = new RecordTable("Test", 3);
        var idCol = table.Columns.Add<int>("Id");
        var nameCol = table.Columns.Add<string>("Name");
        var ageCol = table.Columns.Add<int>("Age");
        for (int i = 0; i < 3; i++)
        {
            var row = table.AddRow();
            idCol.SetValue(row.Row, i + 1);
            nameCol.SetValue(row.Row, $"Person{i + 1}");
            ageCol.SetValue(row.Row, 20 + i);
        }
        return table;
    }

    #region RenameColumn

    [TestMethod]
    public void WhenRenameColumnThenColumnNameChanges()
    {
        var table = CreateTestRecord();

        table.RenameColumn("Name", "FullName");

        Assert.IsNotNull(table.Columns.Find("FullName"));
        Assert.IsNull(table.Columns.Find("Name"));
        Assert.AreEqual("Person1", table.Columns.Find("FullName")!.Get(0));
    }

    [TestMethod]
    public void WhenRenameColumnNonExistentThenThrowsKeyNotFoundException()
    {
        var table = CreateTestRecord();

        Assert.Throws<KeyNotFoundException>(() => table.RenameColumn("NonExistent", "NewName"));
    }

    [TestMethod]
    public void WhenRenameColumnToExistingNameThenThrowsDuplicateNameException()
    {
        var table = CreateTestRecord();

        Assert.Throws<DuplicateNameException>(() => table.RenameColumn("Name", "Id"));
    }

    [TestMethod]
    public void WhenRenameColumnToEmptyNameThenThrowsArgumentException()
    {
        var table = CreateTestRecord();

        Assert.Throws<ArgumentException>(() => table.RenameColumn("Name", ""));
    }

    [TestMethod]
    public void WhenRenameColumnToSameNameThenSucceeds()
    {
        var table = CreateTestRecord();

        table.RenameColumn("Name", "Name");

        Assert.IsNotNull(table.Columns.Find("Name"));
    }

    [TestMethod]
    public void WhenRenameColumnThenDataPreserved()
    {
        var table = CreateTestRecord();

        table.RenameColumn("Id", "RecordId");

        var col = table.Columns.Find("RecordId");
        Assert.IsNotNull(col);
        Assert.AreEqual(1, col!.Get(0));
        Assert.AreEqual(2, col.Get(1));
        Assert.AreEqual(3, col.Get(2));
    }

    #endregion

    #region CastColumn

    [TestMethod]
    public void WhenCastColumnThenTypeChanges()
    {
        var table = CreateTestRecord();

        table.CastColumn("Id", typeof(string));

        var col = table.Columns.Find("Id");
        Assert.IsNotNull(col);
        Assert.AreEqual(typeof(string), col!.Type);
    }

    [TestMethod]
    public void WhenCastColumnThenDataConverted()
    {
        var table = CreateTestRecord();

        table.CastColumn("Id", typeof(string));

        var col = table.Columns.Find("Id");
        Assert.AreEqual("1", col!.Get(0));
        Assert.AreEqual("2", col.Get(1));
        Assert.AreEqual("3", col.Get(2));
    }

    [TestMethod]
    public void WhenCastColumnToSameTypeThenNoChange()
    {
        var table = CreateTestRecord();
        var originalCol = table.Columns.Find("Id");

        table.CastColumn("Id", typeof(int));

        var col = table.Columns.Find("Id");
        Assert.AreSame(originalCol, col);
    }

    [TestMethod]
    public void WhenCastColumnNonExistentThenThrowsKeyNotFoundException()
    {
        var table = CreateTestRecord();

        Assert.Throws<KeyNotFoundException>(() => table.CastColumn("NonExistent", typeof(string)));
    }

    [TestMethod]
    public void WhenCastColumnNullTypeThenThrowsArgumentNullException()
    {
        var table = CreateTestRecord();

        Assert.Throws<ArgumentNullException>(() => table.CastColumn("Id", null!));
    }

    [TestMethod]
    public void WhenCastColumnThenColumnPositionPreserved()
    {
        var table = CreateTestRecord();

        table.CastColumn("Age", typeof(long));

        Assert.AreEqual("Id", table.Columns[0].Name);
        Assert.AreEqual("Name", table.Columns[1].Name);
        Assert.AreEqual("Age", table.Columns[2].Name);
        Assert.AreEqual(typeof(long), table.Columns[2].Type);
    }

    #endregion

    #region CloneSchema

    [TestMethod]
    public void WhenCloneSchemaThenNewRecordHasSameColumns()
    {
        var table = CreateTestRecord();

        var clone = table.CloneSchema();

        Assert.AreEqual(3, clone.Columns.Count);
        Assert.AreEqual("Id", clone.Columns[0].Name);
        Assert.AreEqual(typeof(int), clone.Columns[0].Type);
        Assert.AreEqual("Name", clone.Columns[1].Name);
        Assert.AreEqual(typeof(string), clone.Columns[1].Type);
        Assert.AreEqual("Age", clone.Columns[2].Name);
        Assert.AreEqual(typeof(int), clone.Columns[2].Type);
    }

    [TestMethod]
    public void WhenCloneSchemaThenNewRecordHasZeroRows()
    {
        var table = CreateTestRecord();

        var clone = table.CloneSchema();

        Assert.AreEqual(0, clone.Count);
    }

    [TestMethod]
    public void WhenCloneSchemaThenNamePreserved()
    {
        var table = CreateTestRecord();

        var clone = table.CloneSchema();

        Assert.AreEqual("Test", clone.Name);
    }

    [TestMethod]
    public void WhenCloneSchemaThenOriginalUnchanged()
    {
        var table = CreateTestRecord();

        var clone = table.CloneSchema();
        clone.Columns.Add<double>("Score");

        Assert.AreEqual(3, table.Columns.Count);
    }

    [TestMethod]
    public void WhenCloneSchemaEmptyRecordThenReturnsEmptySchema()
    {
        var table = new RecordTable();

        var clone = table.CloneSchema();

        Assert.AreEqual(0, clone.Columns.Count);
        Assert.AreEqual(0, clone.Count);
    }

    #endregion

    #region Clone

    [TestMethod]
    public void WhenCloneThenNewRecordHasSameColumnsAndData()
    {
        var table = CreateTestRecord();

        var clone = table.Clone();

        Assert.AreEqual(3, clone.Columns.Count);
        Assert.AreEqual(3, clone.Count);
        Assert.AreEqual(1, clone.Columns[0].Get(0));
        Assert.AreEqual("Person2", clone.Columns[1].Get(1));
        Assert.AreEqual(22, clone.Columns[2].Get(2));
    }

    [TestMethod]
    public void WhenCloneThenModifyingCloneDoesNotAffectOriginal()
    {
        var table = CreateTestRecord();

        var clone = table.Clone();
        clone.Columns[0].Set(0, 999);

        Assert.AreEqual(1, table.Columns[0].Get(0));
        Assert.AreEqual(999, clone.Columns[0].Get(0));
    }

    [TestMethod]
    public void WhenCloneThenNamePreserved()
    {
        var table = CreateTestRecord();

        var clone = table.Clone();

        Assert.AreEqual("Test", clone.Name);
    }

    [TestMethod]
    public void WhenCloneEmptyRecordThenReturnsEmptyRecord()
    {
        var table = new RecordTable("Empty", 0);
        table.Columns.Add<int>("Id");

        var clone = table.Clone();

        Assert.AreEqual(1, clone.Columns.Count);
        Assert.AreEqual(0, clone.Count);
    }

    #endregion

    #region GetSchema

    [TestMethod]
    public void WhenGetSchemaThenReturnsColumnDefinitions()
    {
        var table = CreateTestRecord();

        var schema = table.GetSchema();

        Assert.AreEqual(3, schema.Columns.Count);
        Assert.AreEqual("Id", schema.Columns[0].Name);
        Assert.AreEqual(typeof(int), schema.Columns[0].Type);
        Assert.AreEqual("Name", schema.Columns[1].Name);
        Assert.AreEqual(typeof(string), schema.Columns[1].Type);
        Assert.AreEqual("Age", schema.Columns[2].Name);
        Assert.AreEqual(typeof(int), schema.Columns[2].Type);
    }

    [TestMethod]
    public void WhenGetSchemaEmptyRecordThenReturnsEmptySchema()
    {
        var table = new RecordTable();

        var schema = table.GetSchema();

        Assert.AreEqual(0, schema.Columns.Count);
    }

    [TestMethod]
    public void WhenGetSchemaThenChangingRecordDoesNotAffectSchema()
    {
        var table = CreateTestRecord();
        var schema = table.GetSchema();

        table.RenameColumn("Id", "RecordId");

        Assert.AreEqual("Id", schema.Columns[0].Name);
    }

    #endregion
}
