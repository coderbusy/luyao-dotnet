namespace LuYao.Data;

[TestClass]
public class RecordMappingTests
{
    private sealed class TestModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    // ── AddRows ──────────────────────────────────────────────────────────────

    [TestMethod]
    public void AddRowsFromList_ShouldAppendAllItemsAsRows()
    {
        var list = new List<TestModel>
        {
            new TestModel { Id = 1, Name = "Name1" },
            new TestModel { Id = 2, Name = "Name2" }
        };

        var table = new RecordTable();
        table.Columns.AddFrom<TestModel>();
        table.AddRowsFromList(list);

        Assert.AreEqual(2, table.Count);
        Assert.AreEqual(1,       table[0]["Id"]);
        Assert.AreEqual("Name1", table[0]["Name"]);
        Assert.AreEqual(2,       table[1]["Id"]);
        Assert.AreEqual("Name2", table[1]["Name"]);
    }

    // ── From<T> ──────────────────────────────────────────────────────────────

    [TestMethod]
    public void From_ShouldCreateRecordWithSingleRow()
    {
        var model = new TestModel { Id = 42, Name = "Alice" };

        var table = RecordTable.From(model);

        Assert.AreEqual(1,       table.Count);
        Assert.AreEqual(42,      table[0]["Id"]);
        Assert.AreEqual("Alice", table[0]["Name"]);
    }

    [TestMethod]
    public void From_WithOutParameter_ShouldCreateRecordAndReturnRow()
    {
        var model = new TestModel { Id = 99, Name = "Bob" };

        var table = RecordTable.From(model, out var row);

        Assert.AreEqual(1,     table.Count);
        Assert.IsNotNull(row);
        Assert.AreEqual(0,     row.Row);
        Assert.AreEqual(99,    row["Id"]);
        Assert.AreEqual("Bob", row["Name"]);
    }

    // ── FromList<T> ───────────────────────────────────────────────────────────

    [TestMethod]
    public void FromList_ShouldCreateRecordWithAllRows()
    {
        var list = new List<TestModel>
        {
            new TestModel { Id = 1, Name = "Alice" },
            new TestModel { Id = 2, Name = "Bob" },
            new TestModel { Id = 3, Name = "Carol" }
        };

        var table = RecordTable.FromList(list);

        Assert.AreEqual(3,       table.Count);
        Assert.AreEqual(1,       table[0]["Id"]);
        Assert.AreEqual("Alice", table[0]["Name"]);
        Assert.AreEqual(3,       table[2]["Id"]);
        Assert.AreEqual("Carol", table[2]["Name"]);
    }

    [TestMethod]
    public void FromList_WithEmptyCollection_ShouldCreateRecordWithNoRows()
    {
        var table = RecordTable.FromList(new List<TestModel>());

        Assert.AreEqual(0, table.Count);
    }

    // ── To<T> ─────────────────────────────────────────────────────────────────

    [TestMethod]
    public void To_WithRows_ShouldMapFirstRowToModel()
    {
        var table = new RecordTable("Users", 2);
        var idCol   = table.Columns.Add<int>("Id");
        var nameCol = table.Columns.Add<string>("Name");

        var row1 = table.AddRow();
        idCol.SetValue(row1.Row, 1);
        nameCol.SetValue(row1.Row, "Alice");

        var row2 = table.AddRow();
        idCol.SetValue(row2.Row, 2);
        nameCol.SetValue(row2.Row, "Bob");

        var model = table.To<TestModel>();

        Assert.AreEqual(1,       model.Id);
        Assert.AreEqual("Alice", model.Name);
    }

    [TestMethod]
    public void To_WithNoRows_ShouldReturnDefaultInstance()
    {
        var table = new RecordTable("Users", 0);
        table.Columns.Add<int>("Id");
        table.Columns.Add<string>("Name");

        var model = table.To<TestModel>();

        Assert.IsNotNull(model);
        Assert.AreEqual(0,    model.Id);
        Assert.IsNull(model.Name);
    }

    // ── ToList<T> ─────────────────────────────────────────────────────────────

    [TestMethod]
    public void ToList_ShouldMapAllRowsToModels()
    {
        var source = new List<TestModel>
        {
            new TestModel { Id = 1, Name = "Alice" },
            new TestModel { Id = 2, Name = "Bob" }
        };
        var table = RecordTable.FromList(source);

        var result = table.ToList<TestModel>();

        Assert.AreEqual(2,       result.Count);
        Assert.AreEqual(1,       result[0].Id);
        Assert.AreEqual("Alice", result[0].Name);
        Assert.AreEqual(2,       result[1].Id);
        Assert.AreEqual("Bob",   result[1].Name);
    }

    [TestMethod]
    public void ToList_WithNoRows_ShouldReturnEmptyList()
    {
        var table = new RecordTable();
        table.Columns.AddFrom<TestModel>();

        var result = table.ToList<TestModel>();

        Assert.AreEqual(0, result.Count);
    }

    // ── ToDictionary<TKey, T> ─────────────────────────────────────────────────

    [TestMethod]
    public void ToDictionary_ShouldMapFirstColumnAsKey()
    {
        var list = new List<TestModel>
        {
            new TestModel { Id = 1, Name = "Alice" },
            new TestModel { Id = 2, Name = "Bob" }
        };
        var table = RecordTable.FromList(list);

        var dict = table.ToDictionary<int, TestModel>();

        Assert.AreEqual(2,       dict.Count);
        Assert.AreEqual("Alice", dict[1].Name);
        Assert.AreEqual("Bob",   dict[2].Name);
    }

    [TestMethod]
    public void ToDictionary_WithDuplicateKeys_ShouldUseLastValue()
    {
        var list = new List<TestModel>
        {
            new TestModel { Id = 1, Name = "First" },
            new TestModel { Id = 1, Name = "Second" }
        };
        var table = RecordTable.FromList(list);

        var dict = table.ToDictionary<int, TestModel>();

        Assert.AreEqual(1,        dict.Count);
        Assert.AreEqual("Second", dict[1].Name);
    }

    [TestMethod]
    public void ToDictionary_WithNoRows_ShouldReturnEmptyDictionary()
    {
        var table = new RecordTable();
        table.Columns.AddFrom<TestModel>();

        var dict = table.ToDictionary<int, TestModel>();

        Assert.AreEqual(0, dict.Count);
    }

    [TestMethod]
    public void ToDictionary_WithNoColumns_ShouldReturnEmptyDictionary()
    {
        var table = new RecordTable();

        var dict = table.ToDictionary<int, TestModel>();

        Assert.AreEqual(0, dict.Count);
    }
}
