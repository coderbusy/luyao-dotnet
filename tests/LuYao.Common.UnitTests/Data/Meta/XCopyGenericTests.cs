namespace LuYao.Data.Meta;

[TestClass]
public class XCopyGenericTests
{
    // ── 测试用模型 ────────────────────────────────────────────────────────────

    private class Source
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public double Score { get; set; }
        public string? Extra { get; set; }          // Target 中不存在
        public int ReadOnly { get; } = 99;          // Source 只读，不影响结果
    }

    private class Target
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public double Score { get; set; }
        public string? OnlyInTarget { get; set; }   // Source 中不存在
        public int WriteOnly { private get; set; }  // Source 不可读，跳过
    }

    private class TypeMismatch
    {
        public int Id { get; set; }         // Source.Id 是 int，此处也是 int → 匹配
        public long Score { get; set; }     // Source.Score 是 double，此处是 long → 不匹配，应跳过
    }

    // ── Copy ─────────────────────────────────────────────────────────────────

    [TestMethod]
    public void Copy_NullSource_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => XCopy<Source, Target>.Copy(null!));
    }

    [TestMethod]
    public void Copy_MatchingProperties_AreCopied()
    {
        var source = new Source { Id = 1, Name = "Alice", Score = 9.5 };
        var target = XCopy<Source, Target>.Copy(source);

        Assert.AreEqual(1, target.Id);
        Assert.AreEqual("Alice", target.Name);
        Assert.AreEqual(9.5, target.Score);
    }

    [TestMethod]
    public void Copy_ExtraSourceProperty_IsIgnored()
    {
        // Extra 属性在 Target 中不存在，不应引发任何异常
        var source = new Source { Id = 2, Extra = "extra" };
        var target = XCopy<Source, Target>.Copy(source);

        Assert.AreEqual(2, target.Id);
        Assert.IsNull(target.OnlyInTarget);
    }

    [TestMethod]
    public void Copy_ExtraTargetProperty_RemainsDefault()
    {
        var source = new Source { Id = 3 };
        var target = XCopy<Source, Target>.Copy(source);

        Assert.IsNull(target.OnlyInTarget);
    }

    [TestMethod]
    public void Copy_ReturnsNewInstance()
    {
        var source = new Source { Id = 4 };
        var t1 = XCopy<Source, Target>.Copy(source);
        var t2 = XCopy<Source, Target>.Copy(source);

        Assert.AreNotSame(t1, t2);
    }

    [TestMethod]
    public void Copy_NullStringValue_IsCopied()
    {
        var source = new Source { Id = 5, Name = null };
        var target = XCopy<Source, Target>.Copy(source);

        Assert.IsNull(target.Name);
    }

    // ── Copy ───────────────────────────────────────────────────────────────

    [TestMethod]
    public void CopyTo_NullSource_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(
            () => XCopy<Source, Target>.CopyTo(null!, new Target()));
    }

    [TestMethod]
    public void CopyTo_NullTarget_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(
            () => XCopy<Source, Target>.CopyTo(new Source(), null!));
    }

    [TestMethod]
    public void CopyTo_OverwritesExistingTargetValues()
    {
        var source = new Source { Id = 10, Name = "Bob" };
        var target = new Target { Id = 99, Name = "Old" };

        XCopy<Source, Target>.CopyTo(source, target);

        Assert.AreEqual(10, target.Id);
        Assert.AreEqual("Bob", target.Name);
    }

    // ── 类型不匹配 ────────────────────────────────────────────────────────────

    [TestMethod]
    public void Copy_TypeMismatchProperty_IsSkipped()
    {
        var source = new Source { Id = 7, Score = 3.14 };
        var result = XCopy<Source, TypeMismatch>.Copy(source);

        // Id 类型匹配，应被复制
        Assert.AreEqual(7, result.Id);
        // Score 类型不匹配（double vs long），应保持默认值 0
        Assert.AreEqual(0L, result.Score);
    }

    // ── 同类型自复制 ──────────────────────────────────────────────────────────

    [TestMethod]
    public void Copy_SameType_CopiesAllSupportedProperties()
    {
        var source = new Source { Id = 8, Name = "Charlie", Score = 1.0 };
        var copy = XCopy<Source, Source>.Copy(source);

        Assert.AreNotSame(source, copy);
        Assert.AreEqual(source.Id, copy.Id);
        Assert.AreEqual(source.Name, copy.Name);
        Assert.AreEqual(source.Score, copy.Score);
    }

    // ── record class ──────────────────────────────────────────────────────────

    private record class RecordSource
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public double Score { get; set; }
    }

    private record class RecordTarget
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public double Score { get; set; }
    }

    [TestMethod]
    public void Copy_RecordClassSource_MatchingPropertiesAreCopied()
    {
        var source = new RecordSource { Id = 10, Name = "Record", Score = 7.7 };
        var target = XCopy<RecordSource, RecordTarget>.Copy(source);

        Assert.AreEqual(10, target.Id);
        Assert.AreEqual("Record", target.Name);
        Assert.AreEqual(7.7, target.Score);
    }

    [TestMethod]
    public void Copy_RecordClassSource_ReturnsNewInstance()
    {
        var source = new RecordSource { Id = 11 };
        var t1 = XCopy<RecordSource, RecordTarget>.Copy(source);
        var t2 = XCopy<RecordSource, RecordTarget>.Copy(source);

        Assert.AreNotSame(t1, t2);
    }

    [TestMethod]
    public void CopyTo_RecordClassTarget_OverwritesExistingValues()
    {
        var source = new RecordSource { Id = 12, Name = "New" };
        var target = new RecordTarget { Id = 99, Name = "Old" };

        XCopy<RecordSource, RecordTarget>.CopyTo(source, target);

        Assert.AreEqual(12, target.Id);
        Assert.AreEqual("New", target.Name);
    }

    [TestMethod]
    public void Copy_ClassSourceToRecordClassTarget_MatchingPropertiesAreCopied()
    {
        var source = new Source { Id = 13, Name = "Mixed", Score = 3.3 };
        var target = XCopy<Source, RecordTarget>.Copy(source);

        Assert.AreEqual(13, target.Id);
        Assert.AreEqual("Mixed", target.Name);
        Assert.AreEqual(3.3, target.Score);
    }
}
