using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LuYao.Data;

[TestClass]
public class FrameSchemaOperationsTests
{
    private static Frame CreateTestFrame()
    {
        var record = new Frame("Test", 3);
        var idCol = record.Columns.Add<int>("Id");
        var nameCol = record.Columns.Add<string>("Name");
        var ageCol = record.Columns.Add<int>("Age");
        for (int i = 0; i < 3; i++)
        {
            var row = record.AddRow();
            idCol.SetValue(row.Row, i + 1);
            nameCol.SetValue(row.Row, $"Person{i + 1}");
            ageCol.SetValue(row.Row, 20 + i);
        }
        return record;
    }

    #region RenameColumn

    [TestMethod]
    public void WhenRenameColumnThenColumnNameChanges()
    {
        var record = CreateTestFrame();

        record.RenameColumn("Name", "FullName");

        Assert.IsNotNull(record.Columns.Find("FullName"));
        Assert.IsNull(record.Columns.Find("Name"));
        Assert.AreEqual("Person1", record.Columns.Find("FullName")!.Get(0));
    }

    [TestMethod]
    public void WhenRenameColumnNonExistentThenThrowsKeyNotFoundException()
    {
        var record = CreateTestFrame();

        Assert.Throws<KeyNotFoundException>(() => record.RenameColumn("NonExistent", "NewName"));
    }

    [TestMethod]
    public void WhenRenameColumnToExistingNameThenThrowsDuplicateNameException()
    {
        var record = CreateTestFrame();

        Assert.Throws<DuplicateNameException>(() => record.RenameColumn("Name", "Id"));
    }

    [TestMethod]
    public void WhenRenameColumnToEmptyNameThenThrowsArgumentException()
    {
        var record = CreateTestFrame();

        Assert.Throws<ArgumentException>(() => record.RenameColumn("Name", ""));
    }

    [TestMethod]
    public void WhenRenameColumnToSameNameThenSucceeds()
    {
        var record = CreateTestFrame();

        record.RenameColumn("Name", "Name");

        Assert.IsNotNull(record.Columns.Find("Name"));
    }

    [TestMethod]
    public void WhenRenameColumnThenDataPreserved()
    {
        var record = CreateTestFrame();

        record.RenameColumn("Id", "FrameId");

        var col = record.Columns.Find("FrameId");
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
        var record = CreateTestFrame();

        record.CastColumn("Id", typeof(string));

        var col = record.Columns.Find("Id");
        Assert.IsNotNull(col);
        Assert.AreEqual(typeof(string), col!.Type);
    }

    [TestMethod]
    public void WhenCastColumnThenDataConverted()
    {
        var record = CreateTestFrame();

        record.CastColumn("Id", typeof(string));

        var col = record.Columns.Find("Id");
        Assert.AreEqual("1", col!.Get(0));
        Assert.AreEqual("2", col.Get(1));
        Assert.AreEqual("3", col.Get(2));
    }

    [TestMethod]
    public void WhenCastColumnToSameTypeThenNoChange()
    {
        var record = CreateTestFrame();
        var originalCol = record.Columns.Find("Id");

        record.CastColumn("Id", typeof(int));

        var col = record.Columns.Find("Id");
        Assert.AreSame(originalCol, col);
    }

    [TestMethod]
    public void WhenCastColumnNonExistentThenThrowsKeyNotFoundException()
    {
        var record = CreateTestFrame();

        Assert.Throws<KeyNotFoundException>(() => record.CastColumn("NonExistent", typeof(string)));
    }

    [TestMethod]
    public void WhenCastColumnNullTypeThenThrowsArgumentNullException()
    {
        var record = CreateTestFrame();

        Assert.Throws<ArgumentNullException>(() => record.CastColumn("Id", null!));
    }

    [TestMethod]
    public void WhenCastColumnThenColumnPositionPreserved()
    {
        var record = CreateTestFrame();

        record.CastColumn("Age", typeof(long));

        Assert.AreEqual("Id", record.Columns[0].Name);
        Assert.AreEqual("Name", record.Columns[1].Name);
        Assert.AreEqual("Age", record.Columns[2].Name);
        Assert.AreEqual(typeof(long), record.Columns[2].Type);
    }

    #endregion

    #region CloneSchema

    [TestMethod]
    public void WhenCloneSchemaThenNewFrameHasSameColumns()
    {
        var record = CreateTestFrame();

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
    public void WhenCloneSchemaThenNewFrameHasZeroRows()
    {
        var record = CreateTestFrame();

        var clone = record.CloneSchema();

        Assert.AreEqual(0, clone.Count);
    }

    [TestMethod]
    public void WhenCloneSchemaThenNamePreserved()
    {
        var record = CreateTestFrame();

        var clone = record.CloneSchema();

        Assert.AreEqual("Test", clone.Name);
    }

    [TestMethod]
    public void WhenCloneSchemaThenOriginalUnchanged()
    {
        var record = CreateTestFrame();

        var clone = record.CloneSchema();
        clone.Columns.Add<double>("Score");

        Assert.AreEqual(3, record.Columns.Count);
    }

    [TestMethod]
    public void WhenCloneSchemaEmptyFrameThenReturnsEmptySchema()
    {
        var record = new Frame();

        var clone = record.CloneSchema();

        Assert.AreEqual(0, clone.Columns.Count);
        Assert.AreEqual(0, clone.Count);
    }

    #endregion

    #region Clone

    [TestMethod]
    public void WhenCloneThenNewFrameHasSameColumnsAndData()
    {
        var record = CreateTestFrame();

        var clone = record.Clone();

        Assert.AreEqual(3, clone.Columns.Count);
        Assert.AreEqual(3, clone.Count);
        Assert.AreEqual(1, clone.Columns[0].Get(0));
        Assert.AreEqual("Person2", clone.Columns[1].Get(1));
        Assert.AreEqual(22, clone.Columns[2].Get(2));
    }

    [TestMethod]
    public void WhenCloneThenModifyingCloneDoesNotAffectOriginal()
    {
        var record = CreateTestFrame();

        var clone = record.Clone();
        clone.Columns[0].Set(0, 999);

        Assert.AreEqual(1, record.Columns[0].Get(0));
        Assert.AreEqual(999, clone.Columns[0].Get(0));
    }

    [TestMethod]
    public void WhenCloneThenNamePreserved()
    {
        var record = CreateTestFrame();

        var clone = record.Clone();

        Assert.AreEqual("Test", clone.Name);
    }

    [TestMethod]
    public void WhenCloneEmptyFrameThenReturnsEmptyFrame()
    {
        var record = new Frame("Empty", 0);
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
        var record = CreateTestFrame();

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
    public void WhenGetSchemaEmptyFrameThenReturnsEmptySchema()
    {
        var record = new Frame();

        var schema = record.GetSchema();

        Assert.AreEqual(0, schema.Columns.Count);
    }

    [TestMethod]
    public void WhenGetSchemaThenChangingFrameDoesNotAffectSchema()
    {
        var record = CreateTestFrame();
        var schema = record.GetSchema();

        record.RenameColumn("Id", "FrameId");

        Assert.AreEqual("Id", schema.Columns[0].Name);
    }

    #endregion
}
