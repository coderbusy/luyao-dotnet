using System;

namespace LuYao.Data;

/// <summary>
/// Tests for RecordTable Binary (byte[]) column type support.
/// </summary>
[TestClass]
public class RecordArrayTypeTests
{
    #region Basic Binary column tests

    [TestMethod]
    public void WhenAddBinaryColumnThenColumnTypeIsBinary()
    {
        var table = new RecordTable("Test");
        var col = table.Columns.Add<byte[]>("Data");

        Assert.AreEqual(RecordColumnType.Binary, col.ColumnType);
        Assert.AreEqual(typeof(byte[]), col.Type);
    }

    [TestMethod]
    public void WhenAddNonArrayColumnThenColumnTypeIsCorrect()
    {
        var table = new RecordTable("Test");
        var col = table.Columns.Add<int>("Value");

        Assert.AreEqual(RecordColumnType.Int32, col.ColumnType);
        Assert.AreEqual(typeof(int), col.Type);
    }

    #endregion

    #region Binary data operations

    [TestMethod]
    public void WhenSetAndGetByteArrayThenValuesPreserved()
    {
        var table = new RecordTable("Test");
        table.Columns.Add<byte[]>("Data");

        var row = table.AddRow();
        row["Data"] = new byte[] { 1, 2, 3, 4, 5 };

        var retrieved = row.To<byte[]>("Data");
        Assert.IsNotNull(retrieved);
        CollectionAssert.AreEqual(new byte[] { 1, 2, 3, 4, 5 }, retrieved);
    }

    [TestMethod]
    public void WhenSetNullByteArrayThenGetReturnsNull()
    {
        var table = new RecordTable("Test");
        table.Columns.Add<byte[]>("Data");

        var row = table.AddRow();
        row["Data"] = null;

        var retrieved = row.To<byte[]>("Data");
        Assert.IsNull(retrieved);
    }

    [TestMethod]
    public void WhenSetEmptyByteArrayThenGetReturnsEmptyArray()
    {
        var table = new RecordTable("Test");
        table.Columns.Add<byte[]>("Data");

        var row = table.AddRow();
        row["Data"] = new byte[0];

        var retrieved = row.To<byte[]>("Data");
        Assert.IsNotNull(retrieved);
        Assert.AreEqual(0, retrieved.Length);
    }

    #endregion

    #region Binary serialization tests

    [TestMethod]
    public void WhenSerializeByteArrayThenRoundTripSucceeds()
    {
        var original = new RecordTable("Test");
        original.Columns.Add<byte[]>("Data");

        var row = original.AddRow();
        row["Data"] = new byte[] { 10, 20, 30, 40 };

        var bytes = original.ToBytes();
        var deserialized = RecordTable.FromBytes(bytes);

        Assert.AreEqual(1, deserialized.Count);
        var data = deserialized[0].To<byte[]>("Data");
        CollectionAssert.AreEqual(new byte[] { 10, 20, 30, 40 }, data);
    }

    [TestMethod]
    public void WhenSerializeNullByteArrayThenRoundTripSucceeds()
    {
        var original = new RecordTable("Test");
        original.Columns.Add<byte[]>("Data");

        var row = original.AddRow();
        row["Data"] = null;

        var bytes = original.ToBytes();
        var deserialized = RecordTable.FromBytes(bytes);

        Assert.IsNull(deserialized[0].To<byte[]>("Data"));
    }

    [TestMethod]
    public void WhenSerializeEmptyByteArrayThenRoundTripSucceeds()
    {
        var original = new RecordTable("Test");
        original.Columns.Add<byte[]>("Data");

        var row = original.AddRow();
        row["Data"] = new byte[0];

        var bytes = original.ToBytes();
        var deserialized = RecordTable.FromBytes(bytes);

        var data = deserialized[0].To<byte[]>("Data");
        Assert.IsNotNull(data);
        Assert.AreEqual(0, data.Length);
    }

    [TestMethod]
    public void WhenSerializeMultipleRowsWithByteArrayThenAllRowsPreserved()
    {
        var original = new RecordTable("Test");
        original.Columns.Add<int>("Id");
        original.Columns.Add<byte[]>("Data");

        for (int i = 1; i <= 3; i++)
        {
            var row = original.AddRow();
            row["Id"] = i;
            row["Data"] = new byte[] { (byte)i, (byte)(i * 2) };
        }

        var bytes = original.ToBytes();
        var deserialized = RecordTable.FromBytes(bytes);

        Assert.AreEqual(3, deserialized.Count);
        for (int i = 0; i < 3; i++)
        {
            var id = deserialized[i].To<int>("Id");
            var data = deserialized[i].To<byte[]>("Data");
            Assert.AreEqual(i + 1, id);
            CollectionAssert.AreEqual(new byte[] { (byte)(i + 1), (byte)((i + 1) * 2) }, data);
        }
    }

    #endregion

    #region ToString tests

    [TestMethod]
    public void WhenToStringOnByteArrayThenReturnsBase64()
    {
        var table = new RecordTable("Test");
        table.Columns.Add<byte[]>("Data");

        var row = table.AddRow();
        row["Data"] = new byte[] { 1, 2, 3 };

        var str = row.ToString("Data");
        Assert.AreEqual(Convert.ToBase64String(new byte[] { 1, 2, 3 }), str);
    }

    [TestMethod]
    public void WhenToStringOnNullByteArrayThenReturnsEmptyString()
    {
        var table = new RecordTable("Test");
        table.Columns.Add<byte[]>("Data");

        var row = table.AddRow();
        row["Data"] = null;

        var str = row.ToString("Data");
        Assert.AreEqual("", str);
    }

    #endregion

    #region RecordSchema tests

    [TestMethod]
    public void WhenGetSchemaWithBinaryColumnThenColumnTypeIsBinary()
    {
        var table = new RecordTable("Test");
        table.Columns.Add<int>("Id");
        table.Columns.Add<byte[]>("Data");

        var schema = table.GetSchema();

        Assert.AreEqual(2, schema.Columns.Count);

        var idCol = schema.Columns[0];
        Assert.AreEqual("Id", idCol.Name);
        Assert.AreEqual(typeof(int), idCol.Type);

        var dataCol = schema.Columns[1];
        Assert.AreEqual("Data", dataCol.Name);
        Assert.AreEqual(RecordColumnType.Binary, dataCol.ColumnType);
        Assert.AreEqual(typeof(byte[]), dataCol.Type);
    }

    #endregion

    #region Delete operation tests

    [TestMethod]
    public void WhenDeleteRowWithBinaryColumnThenDataShiftsCorrectly()
    {
        var table = new RecordTable("Test");
        table.Columns.Add<int>("Id");
        table.Columns.Add<byte[]>("Data");

        for (int i = 1; i <= 3; i++)
        {
            var row = table.AddRow();
            row["Id"] = i;
            row["Data"] = new byte[] { (byte)i };
        }

        table.Delete(1);

        Assert.AreEqual(2, table.Count);
        Assert.AreEqual(1, table[0].To<int>("Id"));
        CollectionAssert.AreEqual(new byte[] { 1 }, table[0].To<byte[]>("Data"));
        Assert.AreEqual(3, table[1].To<int>("Id"));
        CollectionAssert.AreEqual(new byte[] { 3 }, table[1].To<byte[]>("Data"));
    }

    #endregion

    #region Clone operation tests

    [TestMethod]
    public void WhenCloneRecordWithBinaryColumnThenDataCopied()
    {
        var original = new RecordTable("Test");
        original.Columns.Add<int>("Id");
        original.Columns.Add<byte[]>("Data");

        var row = original.AddRow();
        row["Id"] = 1;
        row["Data"] = new byte[] { 1, 2, 3 };

        var clone = original.Clone();

        Assert.AreEqual(1, clone.Count);
        Assert.AreEqual(2, clone.Columns.Count);
        CollectionAssert.AreEqual(new byte[] { 1, 2, 3 }, clone[0].To<byte[]>("Data"));
    }

    [TestMethod]
    public void WhenCloneSchemaWithBinaryColumnThenStructureCopied()
    {
        var original = new RecordTable("Test");
        original.Columns.Add<int>("Id");
        original.Columns.Add<byte[]>("Data");

        var clone = original.CloneSchema();

        Assert.AreEqual(0, clone.Count);
        Assert.AreEqual(2, clone.Columns.Count);
        Assert.AreEqual(RecordColumnType.Int32, clone.Columns[0].ColumnType);
        Assert.AreEqual(RecordColumnType.Binary, clone.Columns[1].ColumnType);
    }

    #endregion
}