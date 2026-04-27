namespace LuYao.Data;

[TestClass]
public class FrameMappingTests
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

        var record = new Frame();
        record.Columns.AddFrom<TestModel>();
        record.AddRowsFromList(list);

        Assert.AreEqual(2, record.Count);
        Assert.AreEqual(1,       record[0]["Id"]);
        Assert.AreEqual("Name1", record[0]["Name"]);
        Assert.AreEqual(2,       record[1]["Id"]);
        Assert.AreEqual("Name2", record[1]["Name"]);
    }

    // ── From<T> ──────────────────────────────────────────────────────────────

    [TestMethod]
    public void From_ShouldCreateFrameWithSingleRow()
    {
        var model = new TestModel { Id = 42, Name = "Alice" };

        var record = Frame.From(model);

        Assert.AreEqual(1,       record.Count);
        Assert.AreEqual(42,      record[0]["Id"]);
        Assert.AreEqual("Alice", record[0]["Name"]);
    }

    // ── FromList<T> ───────────────────────────────────────────────────────────

    [TestMethod]
    public void FromList_ShouldCreateFrameWithAllRows()
    {
        var list = new List<TestModel>
        {
            new TestModel { Id = 1, Name = "Alice" },
            new TestModel { Id = 2, Name = "Bob" },
            new TestModel { Id = 3, Name = "Carol" }
        };

        var record = Frame.FromList(list);

        Assert.AreEqual(3,       record.Count);
        Assert.AreEqual(1,       record[0]["Id"]);
        Assert.AreEqual("Alice", record[0]["Name"]);
        Assert.AreEqual(3,       record[2]["Id"]);
        Assert.AreEqual("Carol", record[2]["Name"]);
    }

    [TestMethod]
    public void FromList_WithEmptyCollection_ShouldCreateFrameWithNoRows()
    {
        var record = Frame.FromList(new List<TestModel>());

        Assert.AreEqual(0, record.Count);
    }

    // ── To<T> ─────────────────────────────────────────────────────────────────

    [TestMethod]
    public void To_WithRows_ShouldMapFirstRowToModel()
    {
        var record = new Frame("Users", 2);
        var idCol   = record.Columns.Add<int>("Id");
        var nameCol = record.Columns.Add<string>("Name");

        var row1 = record.AddRow();
        idCol.SetValue(row1.Row, 1);
        nameCol.SetValue(row1.Row, "Alice");

        var row2 = record.AddRow();
        idCol.SetValue(row2.Row, 2);
        nameCol.SetValue(row2.Row, "Bob");

        var model = record.To<TestModel>();

        Assert.AreEqual(1,       model.Id);
        Assert.AreEqual("Alice", model.Name);
    }

    [TestMethod]
    public void To_WithNoRows_ShouldReturnDefaultInstance()
    {
        var record = new Frame("Users", 0);
        record.Columns.Add<int>("Id");
        record.Columns.Add<string>("Name");

        var model = record.To<TestModel>();

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
        var record = Frame.FromList(source);

        var result = record.ToList<TestModel>();

        Assert.AreEqual(2,       result.Count);
        Assert.AreEqual(1,       result[0].Id);
        Assert.AreEqual("Alice", result[0].Name);
        Assert.AreEqual(2,       result[1].Id);
        Assert.AreEqual("Bob",   result[1].Name);
    }

    [TestMethod]
    public void ToList_WithNoRows_ShouldReturnEmptyList()
    {
        var record = new Frame();
        record.Columns.AddFrom<TestModel>();

        var result = record.ToList<TestModel>();

        Assert.AreEqual(0, result.Count);
    }
}
