namespace LuYao.Data;

[TestClass]
public class NameFilterTests
{
    private class SampleData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    [TestMethod]
    public void ToNames_Default_ReturnsAllSupportedProperties()
    {
        var filter = new NameFilter<SampleData>();
        var names = filter.ToNames();
        CollectionAssert.Contains(names, nameof(SampleData.Id));
        CollectionAssert.Contains(names, nameof(SampleData.Name));
        CollectionAssert.Contains(names, nameof(SampleData.Email));
        CollectionAssert.Contains(names, nameof(SampleData.CreatedAt));
    }

    [TestMethod]
    public void Exclude_RemovesProperty()
    {
        var filter = new NameFilter<SampleData>();
        filter.Exclude(x => x.Email);
        var names = filter.ToNames();
        CollectionAssert.Contains(names, nameof(SampleData.Id));
        CollectionAssert.Contains(names, nameof(SampleData.Name));
        CollectionAssert.DoesNotContain(names, nameof(SampleData.Email));
    }

    [TestMethod]
    public void Include_AddsBackExcludedProperty()
    {
        var filter = new NameFilter<SampleData>();
        filter.Exclude(x => x.Email);
        filter.Include(x => x.Email);
        var names = filter.ToNames();
        CollectionAssert.Contains(names, nameof(SampleData.Email));
    }

    [TestMethod]
    public void All_ResetsToAllProperties()
    {
        var filter = new NameFilter<SampleData>();
        filter.Exclude(x => x.Id).Exclude(x => x.Name);
        filter.All();
        var names = filter.ToNames();
        CollectionAssert.Contains(names, nameof(SampleData.Id));
        CollectionAssert.Contains(names, nameof(SampleData.Name));
    }

    [TestMethod]
    public void ToNames_AfterAll_PreservesDeclarationOrder()
    {
        var filter = new NameFilter<SampleData>();
        var names = filter.ToNames();
        var expected = new[] { nameof(SampleData.Id), nameof(SampleData.Name), nameof(SampleData.Email), nameof(SampleData.CreatedAt) };
        CollectionAssert.AreEqual(expected, names);
    }

    [TestMethod]
    public void ToNames_AfterClearAndManualIncludes_RespectsInsertionOrder()
    {
        // 手动 Include 的顺序决定最终列顺序
        var filter = new NameFilter<SampleData>();
        filter.Clear()
              .Include(x => x.Email)
              .Include(x => x.Id)
              .Include(x => x.CreatedAt);
        var names = filter.ToNames();
        CollectionAssert.AreEqual(new[] { nameof(SampleData.Email), nameof(SampleData.Id), nameof(SampleData.CreatedAt) }, names);
    }

    [TestMethod]
    public void Clear_RemovesAllProperties()
    {
        var filter = new NameFilter<SampleData>();
        filter.Clear();
        Assert.AreEqual(0, filter.ToNames().Length);
    }

    [TestMethod]
    public void Clear_ThenInclude_ReturnsOnlyIncludedInOrder()
    {
        var filter = new NameFilter<SampleData>();
        filter.Clear().Include(x => x.Name).Include(x => x.Id);
        CollectionAssert.AreEqual(new[] { nameof(SampleData.Name), nameof(SampleData.Id) }, filter.ToNames());
    }

    [TestMethod]
    public void ToNames_IncludeSamePropertyTwice_NoDuplicates()
    {
        var filter = new NameFilter<SampleData>();
        filter.Include(x => x.Id).Include(x => x.Id);
        var names = filter.ToNames();
        var distinct = new HashSet<string>(names, StringComparer.Ordinal);
        Assert.AreEqual(distinct.Count, names.Length, "ToNames() 结果不应包含重复项。");
    }

    [TestMethod]
    public void AppendColumns_WithNames_RespectsInputOrder()
    {
        // 传入顺序与声明顺序故意相反，验证列顺序与传入顺序一致
        var record = new Record();
        record.AppendColumns<SampleData>(new[] { nameof(SampleData.Email), nameof(SampleData.Id) });
        Assert.AreEqual(nameof(SampleData.Email), record.Columns[0].Name);
        Assert.AreEqual(nameof(SampleData.Id), record.Columns[1].Name);
    }

    [TestMethod]
    public void AppendColumns_WithFilter_RespectsManualIncludeOrder()
    {
        // 通过 NameFilter 手动 Include 的顺序应正确反映到 Record 列顺序
        var record = new Record();
        record.AppendColumns<SampleData>(f => f.Clear()
            .Include(x => x.CreatedAt)
            .Include(x => x.Name));
        Assert.AreEqual(nameof(SampleData.CreatedAt), record.Columns[0].Name);
        Assert.AreEqual(nameof(SampleData.Name), record.Columns[1].Name);
    }

    [TestMethod]
    public void AppendColumns_WithNames_OnlyAddsSpecifiedColumns()
    {
        var record = new Record();
        record.AppendColumns<SampleData>(new[] { nameof(SampleData.Id), nameof(SampleData.Name) });
        Assert.AreEqual(2, record.Columns.Count);
        Assert.IsNotNull(record.Columns[nameof(SampleData.Id)]);
        Assert.IsNotNull(record.Columns[nameof(SampleData.Name)]);
    }

    [TestMethod]
    public void AppendColumns_WithNullOrEmptyNames_AddsNoColumns()
    {
        var record1 = new Record();
        record1.AppendColumns<SampleData>(new string[0]);
        Assert.AreEqual(0, record1.Columns.Count, "空数组不应追加任何列。");

        var record2 = new Record();
        record2.AppendColumns<SampleData>((string[])null);
        Assert.AreEqual(0, record2.Columns.Count, "null 不应追加任何列。");
    }

    [TestMethod]
    public void AppendColumns_WithFilter_ExcludesSpecifiedColumn()
    {
        var record = new Record();
        record.AppendColumns<SampleData>(f => f.Exclude(x => x.Email));
        Assert.AreEqual(3, record.Columns.Count);
        Assert.IsNull(record.Columns[nameof(SampleData.Email)], "Email column should not exist.");
    }
}
