using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LuYao.Data;

[TestClass]
public class RecordSchemaOperationsTests
{
    private static Record CreateTestRecord()
    {
        var record = new Record("Test", 3);
        var idCol = record.Columns.Add<int>("Id");
        var nameCol = record.Columns.Add<string>("Name");
        var ageCol = record.Columns.Add<int>("Age");
        for (int i = 0; i < 3; i++)
        {
            var row = record.AddRow();
            idCol.Set(i + 1, row.Row);
            nameCol.Set($"Person{i + 1}", row.Row);
            ageCol.Set(20 + i, row.Row);
        }
        return record;
    }

    #region RenameColumn

    [TestMethod]
    public void WhenRenameColumnThenColumnNameChanges()
    {
        var record = CreateTestRecord();

        record.RenameColumn("Name", "FullName");

        Assert.IsNotNull(record.Columns.Find("FullName"));
        Assert.IsNull(record.Columns.Find("Name"));
        Assert.AreEqual("Person1", record.Columns.Find("FullName")!.GetValue(0));
    }

    [TestMethod]
    public void WhenRenameColumnNonExistentThenThrowsKeyNotFoundException()
    {
        var record = CreateTestRecord();

        Assert.Throws<KeyNotFoundException>(() => record.RenameColumn("NonExistent", "NewName"));
    }

    [TestMethod]
    public void WhenRenameColumnToExistingNameThenThrowsDuplicateNameException()
    {
        var record = CreateTestRecord();

        Assert.Throws<DuplicateNameException>(() => record.RenameColumn("Name", "Id"));
    }

    [TestMethod]
    public void WhenRenameColumnToEmptyNameThenThrowsArgumentException()
    {
        var record = CreateTestRecord();

        Assert.Throws<ArgumentException>(() => record.RenameColumn("Name", ""));
    }

    [TestMethod]
    public void WhenRenameColumnToSameNameThenSucceeds()
    {
        var record = CreateTestRecord();

        record.RenameColumn("Name", "Name");

        Assert.IsNotNull(record.Columns.Find("Name"));
    }

    [TestMethod]
    public void WhenRenameColumnThenDataPreserved()
    {
        var record = CreateTestRecord();

        record.RenameColumn("Id", "RecordId");

        var col = record.Columns.Find("RecordId");
        Assert.IsNotNull(col);
        Assert.AreEqual(1, col!.GetValue(0));
        Assert.AreEqual(2, col.GetValue(1));
        Assert.AreEqual(3, col.GetValue(2));
    }

    #endregion

    #region CastColumn

    [TestMethod]
    public void WhenCastColumnThenTypeChanges()
    {
        var record = CreateTestRecord();

        record.CastColumn("Id", typeof(string));

        var col = record.Columns.Find("Id");
        Assert.IsNotNull(col);
        Assert.AreEqual(typeof(string), col!.Type);
    }

    [TestMethod]
    public void WhenCastColumnThenDataConverted()
    {
        var record = CreateTestRecord();

        record.CastColumn("Id", typeof(string));

        var col = record.Columns.Find("Id");
        Assert.AreEqual("1", col!.GetValue(0));
        Assert.AreEqual("2", col.GetValue(1));
        Assert.AreEqual("3", col.GetValue(2));
    }

    [TestMethod]
    public void WhenCastColumnToSameTypeThenNoChange()
    {
        var record = CreateTestRecord();
        var originalCol = record.Columns.Find("Id");

        record.CastColumn("Id", typeof(int));

        var col = record.Columns.Find("Id");
        Assert.AreSame(originalCol, col);
    }

    [TestMethod]
    public void WhenCastColumnNonExistentThenThrowsKeyNotFoundException()
    {
        var record = CreateTestRecord();

        Assert.Throws<KeyNotFoundException>(() => record.CastColumn("NonExistent", typeof(string)));
    }

    [TestMethod]
    public void WhenCastColumnNullTypeThenThrowsArgumentNullException()
    {
        var record = CreateTestRecord();

        Assert.Throws<ArgumentNullException>(() => record.CastColumn("Id", null!));
    }

    [TestMethod]
    public void WhenCastColumnThenColumnPositionPreserved()
    {
        var record = CreateTestRecord();

        record.CastColumn("Age", typeof(long));

        Assert.AreEqual("Id", record.Columns[0].Name);
        Assert.AreEqual("Name", record.Columns[1].Name);
        Assert.AreEqual("Age", record.Columns[2].Name);
        Assert.AreEqual(typeof(long), record.Columns[2].Type);
    }

    #endregion

    #region CloneSchema

    [TestMethod]
    public void WhenCloneSchemaThenNewRecordHasSameColumns()
    {
        var record = CreateTestRecord();

        var clone = record.CloneSchema();

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
        var record = CreateTestRecord();

        var clone = record.CloneSchema();

        Assert.AreEqual(0, clone.Count);
    }

    [TestMethod]
    public void WhenCloneSchemaThenNamePreserved()
    {
        var record = CreateTestRecord();

        var clone = record.CloneSchema();

        Assert.AreEqual("Test", clone.Name);
    }

    [TestMethod]
    public void WhenCloneSchemaThenOriginalUnchanged()
    {
        var record = CreateTestRecord();

        var clone = record.CloneSchema();
        clone.Columns.Add<double>("Score");

        Assert.AreEqual(3, record.Columns.Count);
    }

    [TestMethod]
    public void WhenCloneSchemaEmptyRecordThenReturnsEmptySchema()
    {
        var record = new Record();

        var clone = record.CloneSchema();

        Assert.AreEqual(0, clone.Columns.Count);
        Assert.AreEqual(0, clone.Count);
    }

    #endregion

    #region Clone

    [TestMethod]
    public void WhenCloneThenNewRecordHasSameColumnsAndData()
    {
        var record = CreateTestRecord();

        var clone = record.Clone();

        Assert.AreEqual(3, clone.Columns.Count);
        Assert.AreEqual(3, clone.Count);
        Assert.AreEqual(1, clone.Columns[0].GetValue(0));
        Assert.AreEqual("Person2", clone.Columns[1].GetValue(1));
        Assert.AreEqual(22, clone.Columns[2].GetValue(2));
    }

    [TestMethod]
    public void WhenCloneThenModifyingCloneDoesNotAffectOriginal()
    {
        var record = CreateTestRecord();

        var clone = record.Clone();
        clone.Columns[0].SetValue(999, 0);

        Assert.AreEqual(1, record.Columns[0].GetValue(0));
        Assert.AreEqual(999, clone.Columns[0].GetValue(0));
    }

    [TestMethod]
    public void WhenCloneThenNamePreserved()
    {
        var record = CreateTestRecord();

        var clone = record.Clone();

        Assert.AreEqual("Test", clone.Name);
    }

    [TestMethod]
    public void WhenCloneEmptyRecordThenReturnsEmptyRecord()
    {
        var record = new Record("Empty", 0);
        record.Columns.Add<int>("Id");

        var clone = record.Clone();

        Assert.AreEqual(1, clone.Columns.Count);
        Assert.AreEqual(0, clone.Count);
    }

    #endregion

    #region GetSchema

    [TestMethod]
    public void WhenGetSchemaThenReturnsColumnDefinitions()
    {
        var record = CreateTestRecord();

        var schema = record.GetSchema();

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
        var record = new Record();

        var schema = record.GetSchema();

        Assert.AreEqual(0, schema.Columns.Count);
    }

    [TestMethod]
    public void WhenGetSchemaThenChangingRecordDoesNotAffectSchema()
    {
        var record = CreateTestRecord();
        var schema = record.GetSchema();

        record.RenameColumn("Id", "RecordId");

        Assert.AreEqual("Id", schema.Columns[0].Name);
    }

    #endregion
}
