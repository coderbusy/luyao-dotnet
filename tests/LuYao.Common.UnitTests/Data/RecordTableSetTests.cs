using System;
using System.Data;
using System.Linq;

namespace LuYao.Data;

[TestClass]
public class RecordTableSetTests
{
    private static RecordTable CreateTestRecord(string name, int rowCount)
    {
        var table = new RecordTable(name, rowCount);
        var idCol = table.Columns.Add<int>("Id");
        var nameCol = table.Columns.Add<string>("Name");
        for (int i = 0; i < rowCount; i++)
        {
            var row = table.AddRow();
            idCol.SetValue(row.Row, i + 1);
            nameCol.SetValue(row.Row, $"Item{i + 1}");
        }
        return table;
    }

    #region Constructor

    [TestMethod]
    public void WhenDefaultConstructorThenEmptySet()
    {
        var set = new RecordSet();

        Assert.AreEqual(0, set.Count);
        Assert.AreEqual(0, set.Names.Count());
    }

    [TestMethod]
    public void WhenNullComparerThenThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new RecordSet(null!));
    }

    #endregion

    #region Add

    [TestMethod]
    public void WhenAddValidRecordThenCountIncreases()
    {
        var set = new RecordSet();
        var table = CreateTestRecord("Orders", 2);

        set.Add("Orders", table);

        Assert.AreEqual(1, set.Count);
        Assert.AreEqual("Orders", table.Name);
    }

    [TestMethod]
    public void WhenAddNullNameThenThrowsArgumentException()
    {
        var set = new RecordSet();
        var table = CreateTestRecord("test", 1);

        Assert.Throws<ArgumentException>(() => set.Add(null!, table));
    }

    [TestMethod]
    public void WhenAddEmptyNameThenThrowsArgumentException()
    {
        var set = new RecordSet();
        var table = CreateTestRecord("test", 1);

        Assert.Throws<ArgumentException>(() => set.Add("", table));
    }

    [TestMethod]
    public void WhenAddWhitespaceNameThenThrowsArgumentException()
    {
        var set = new RecordSet();
        var table = CreateTestRecord("test", 1);

        Assert.Throws<ArgumentException>(() => set.Add("   ", table));
    }

    [TestMethod]
    public void WhenAddNullRecordThenThrowsArgumentNullException()
    {
        var set = new RecordSet();

        Assert.Throws<ArgumentNullException>(() => set.Add("Orders", null!));
    }

    [TestMethod]
    public void WhenAddDuplicateNameThenThrowsArgumentException()
    {
        var set = new RecordSet();
        set.Add("Orders", CreateTestRecord("Orders", 1));

        Assert.Throws<ArgumentException>(() => set.Add("Orders", CreateTestRecord("Orders", 2)));
    }

    [TestMethod]
    public void WhenAddWithCaseSensitiveComparerThenDifferentCaseIsAllowed()
    {
        var set = new RecordSet(StringComparer.Ordinal);
        set.Add("Orders", CreateTestRecord("Orders", 1));
        set.Add("orders", CreateTestRecord("orders", 2));

        Assert.AreEqual(2, set.Count);
    }

    [TestMethod]
    public void WhenAddWithCaseInsensitiveComparerThenDifferentCaseThrows()
    {
        var set = new RecordSet(StringComparer.OrdinalIgnoreCase);
        set.Add("Orders", CreateTestRecord("Orders", 1));

        Assert.Throws<ArgumentException>(() => set.Add("orders", CreateTestRecord("orders", 2)));
    }

    #endregion

    #region Set

    [TestMethod]
    public void WhenSetNewNameThenAddsRecord()
    {
        var set = new RecordSet();
        var table = CreateTestRecord("Orders", 2);

        set.Set("Orders", table);

        Assert.AreEqual(1, set.Count);
        Assert.AreSame(table, set.Get("Orders"));
    }

    [TestMethod]
    public void WhenSetExistingNameThenReplacesRecord()
    {
        var set = new RecordSet();
        var record1 = CreateTestRecord("Orders", 1);
        var record2 = CreateTestRecord("Orders", 3);

        set.Add("Orders", record1);
        set.Set("Orders", record2);

        Assert.AreEqual(1, set.Count);
        Assert.AreSame(record2, set.Get("Orders"));
    }

    [TestMethod]
    public void WhenSetNullRecordThenThrowsArgumentNullException()
    {
        var set = new RecordSet();

        Assert.Throws<ArgumentNullException>(() => set.Set("Orders", null!));
    }

    [TestMethod]
    public void WhenSetEmptyNameThenThrowsArgumentException()
    {
        var set = new RecordSet();

        Assert.Throws<ArgumentException>(() => set.Set("", CreateTestRecord("test", 1)));
    }

    #endregion

    #region Get / TryGet

    [TestMethod]
    public void WhenGetExistingNameThenReturnsRecord()
    {
        var set = new RecordSet();
        var table = CreateTestRecord("Orders", 2);
        set.Add("Orders", table);

        var result = set.Get("Orders");

        Assert.AreSame(table, result);
    }

    [TestMethod]
    public void WhenGetNonExistingNameThenThrowsKeyNotFoundException()
    {
        var set = new RecordSet();

        Assert.Throws<KeyNotFoundException>(() => set.Get("NonExistent"));
    }

    [TestMethod]
    public void WhenTryGetExistingNameThenReturnsTrueAndRecord()
    {
        var set = new RecordSet();
        var table = CreateTestRecord("Orders", 2);
        set.Add("Orders", table);

        var found = set.TryGet("Orders", out var result);

        Assert.IsTrue(found);
        Assert.AreSame(table, result);
    }

    [TestMethod]
    public void WhenTryGetNonExistingNameThenReturnsFalse()
    {
        var set = new RecordSet();

        var found = set.TryGet("NonExistent", out var result);

        Assert.IsFalse(found);
        Assert.IsNull(result);
    }

    #endregion

    #region Indexer

    [TestMethod]
    public void WhenIndexerExistingNameThenReturnsRecord()
    {
        var set = new RecordSet();
        var table = CreateTestRecord("Orders", 1);
        set.Add("Orders", table);

        var result = set["Orders"];

        Assert.AreSame(table, result);
    }

    [TestMethod]
    public void WhenIndexerNonExistingNameThenThrowsKeyNotFoundException()
    {
        var set = new RecordSet();

        Assert.Throws<KeyNotFoundException>(() => _ = set["NonExistent"]);
    }

    #endregion

    #region Remove

    [TestMethod]
    public void WhenRemoveExistingThenReturnsTrueAndRemoves()
    {
        var set = new RecordSet();
        set.Add("Orders", CreateTestRecord("Orders", 1));

        var removed = set.Remove("Orders");

        Assert.IsTrue(removed);
        Assert.AreEqual(0, set.Count);
        Assert.IsFalse(set.Contains("Orders"));
    }

    [TestMethod]
    public void WhenRemoveNonExistingThenReturnsFalse()
    {
        var set = new RecordSet();

        var removed = set.Remove("NonExistent");

        Assert.IsFalse(removed);
    }

    [TestMethod]
    public void WhenRemoveThenNamesListUpdated()
    {
        var set = new RecordSet();
        set.Add("A", CreateTestRecord("A", 1));
        set.Add("B", CreateTestRecord("B", 1));
        set.Add("C", CreateTestRecord("C", 1));

        set.Remove("B");

        Assert.AreEqual(2, set.Names.Count());
        CollectionAssert.AreEqual(new[] { "A", "C" }, set.Names.ToArray());
    }

    #endregion

    #region Contains

    [TestMethod]
    public void WhenContainsExistingNameThenReturnsTrue()
    {
        var set = new RecordSet();
        set.Add("Orders", CreateTestRecord("Orders", 1));

        Assert.IsTrue(set.Contains("Orders"));
    }

    [TestMethod]
    public void WhenContainsNonExistingNameThenReturnsFalse()
    {
        var set = new RecordSet();

        Assert.IsFalse(set.Contains("NonExistent"));
    }

    #endregion

    #region Rename

    [TestMethod]
    public void WhenRenameExistingThenNameChanges()
    {
        var set = new RecordSet();
        var table = CreateTestRecord("Orders", 1);
        set.Add("Orders", table);

        set.Rename("Orders", "AllOrders");

        Assert.IsFalse(set.Contains("Orders"));
        Assert.IsTrue(set.Contains("AllOrders"));
        Assert.AreSame(table, set.Get("AllOrders"));
        Assert.AreEqual("AllOrders", table.Name);
    }

    [TestMethod]
    public void WhenRenameNonExistingThenThrowsKeyNotFoundException()
    {
        var set = new RecordSet();

        Assert.Throws<KeyNotFoundException>(() => set.Rename("NonExistent", "NewName"));
    }

    [TestMethod]
    public void WhenRenameToExistingNameThenThrowsArgumentException()
    {
        var set = new RecordSet();
        set.Add("A", CreateTestRecord("A", 1));
        set.Add("B", CreateTestRecord("B", 1));

        Assert.Throws<ArgumentException>(() => set.Rename("A", "B"));
    }

    [TestMethod]
    public void WhenRenameToEmptyNameThenThrowsArgumentException()
    {
        var set = new RecordSet();
        set.Add("A", CreateTestRecord("A", 1));

        Assert.Throws<ArgumentException>(() => set.Rename("A", ""));
    }

    [TestMethod]
    public void WhenRenameToSameNameThenSucceeds()
    {
        var set = new RecordSet();
        var table = CreateTestRecord("Orders", 1);
        set.Add("Orders", table);

        set.Rename("Orders", "Orders");

        Assert.IsTrue(set.Contains("Orders"));
        Assert.AreEqual(1, set.Count);
    }

    [TestMethod]
    public void WhenRenameThenNamesListHasDeterministicOrder()
    {
        var set = new RecordSet();
        set.Add("A", CreateTestRecord("A", 1));
        set.Add("B", CreateTestRecord("B", 1));
        set.Add("C", CreateTestRecord("C", 1));

        set.Rename("B", "D");

        CollectionAssert.AreEqual(new[] { "A", "C", "D" }, set.Names.ToArray());
    }

    #endregion

    #region Clear

    [TestMethod]
    public void WhenClearThenSetIsEmpty()
    {
        var set = new RecordSet();
        set.Add("A", CreateTestRecord("A", 1));
        set.Add("B", CreateTestRecord("B", 1));

        set.Clear();

        Assert.AreEqual(0, set.Count);
        Assert.AreEqual(0, set.Names.Count());
    }

    #endregion

    #region Enumeration

    [TestMethod]
    public void WhenEnumerateThenReturnsAllRecordsInOrder()
    {
        var set = new RecordSet();
        var r1 = CreateTestRecord("A", 1);
        var r2 = CreateTestRecord("B", 2);
        var r3 = CreateTestRecord("C", 3);
        set.Add("A", r1);
        set.Add("B", r2);
        set.Add("C", r3);

        var records = set.ToArray();

        Assert.AreEqual(3, records.Length);
        Assert.AreSame(r1, records[0]);
        Assert.AreSame(r2, records[1]);
        Assert.AreSame(r3, records[2]);
    }

    [TestMethod]
    public void WhenEnumerateEmptySetThenReturnsEmpty()
    {
        var set = new RecordSet();

        var records = set.ToArray();

        Assert.AreEqual(0, records.Length);
    }

    #endregion
}
