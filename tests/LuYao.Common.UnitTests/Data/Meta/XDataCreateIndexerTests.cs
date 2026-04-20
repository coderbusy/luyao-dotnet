using LuYao.Data.Meta;

namespace LuYao.Data.Meta;

[TestClass]
public class XDataCreateIndexerTests
{
    // ── 测试用模型 ──────────────────────────────────────────────────────────
    private class SampleModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int ReadOnly { get; } = 99;
    }

    // ── CreateIndexer 参数校验 ───────────────────────────────────────────────

    [TestMethod]
    public void CreateIndexer_NullData_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => XData<SampleModel>.CreateIndexer(null!));
    }

    // ── 写入（set） ──────────────────────────────────────────────────────────

    [TestMethod]
    public void Set_ExistingKey_WritesValue()
    {
        var model = new SampleModel();
        var indexer = XData<SampleModel>.CreateIndexer(model);

        indexer["Id"] = 42;

        Assert.AreEqual(42, model.Id);
    }

    [TestMethod]
    public void Set_ExistingStringKey_WritesValue()
    {
        var model = new SampleModel();
        var indexer = XData<SampleModel>.CreateIndexer(model);

        indexer["Name"] = "hello";

        Assert.AreEqual("hello", model.Name);
    }

    [TestMethod]
    public void Set_NonExistingKey_DoesNotThrow()
    {
        var model = new SampleModel { Id = 1 };
        var indexer = XData<SampleModel>.CreateIndexer(model);

        // 不存在的 key 应静默跳过，不抛异常
        indexer["NotExist"] = 99;

        Assert.AreEqual(1, model.Id);
    }

    [TestMethod]
    public void Set_KeyIsCaseSensitive_DoesNotWriteWrongCase()
    {
        var model = new SampleModel { Id = 7 };
        var indexer = XData<SampleModel>.CreateIndexer(model);

        // "id" 与 "Id" 大小写不同，应跳过
        indexer["id"] = 100;

        Assert.AreEqual(7, model.Id);
    }

    // ── 读取（get） ──────────────────────────────────────────────────────────

    [TestMethod]
    public void Get_ExistingKey_ReturnsValue()
    {
        var model = new SampleModel { Id = 5 };
        var indexer = XData<SampleModel>.CreateIndexer(model);

        var result = indexer["Id"];

        Assert.AreEqual(5, result);
    }

    [TestMethod]
    public void Get_NonExistingKey_ReturnsNull()
    {
        var model = new SampleModel();
        var indexer = XData<SampleModel>.CreateIndexer(model);

        var result = indexer["NotExist"];

        Assert.IsNull(result);
    }

    [TestMethod]
    public void Get_KeyIsCaseSensitive_ReturnsNullForWrongCase()
    {
        var model = new SampleModel { Id = 3 };
        var indexer = XData<SampleModel>.CreateIndexer(model);

        var result = indexer["id"];

        Assert.IsNull(result);
    }

    [TestMethod]
    public void Get_ReadOnlyProperty_ReturnsValue()
    {
        var model = new SampleModel();
        var indexer = XData<SampleModel>.CreateIndexer(model);

        var result = indexer["ReadOnly"];

        Assert.AreEqual(99, result);
    }

    // ── 读写一致性 ────────────────────────────────────────────────────────────

    [TestMethod]
    public void SetThenGet_SameKey_ReturnsWrittenValue()
    {
        var model = new SampleModel();
        var indexer = XData<SampleModel>.CreateIndexer(model);

        indexer["Name"] = "world";

        Assert.AreEqual("world", indexer["Name"]);
    }

    [TestMethod]
    public void Indexer_ReflectsUnderlyingModelChanges()
    {
        var model = new SampleModel { Id = 1 };
        var indexer = XData<SampleModel>.CreateIndexer(model);

        model.Id = 99;

        Assert.AreEqual(99, indexer["Id"]);
    }
}
