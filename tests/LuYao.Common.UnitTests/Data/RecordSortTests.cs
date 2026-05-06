using System;
using System.Collections.Generic;

namespace LuYao.Data;

[TestClass]
public class RecordSortTests
{
    // ── 辅助构建方法 ────────────────────────────────────────────────────────

    private static Record BuildIntRecord(params int[] values)
    {
        var rec = new Record("T");
        rec.Columns.Add("Value", typeof(int));
        foreach (int v in values)
        {
            var row = rec.AddRow();
            rec.Columns["Value"].Set(row, v);
        }
        return rec;
    }

    private static Record BuildStringRecord(params string?[] values)
    {
        var rec = new Record("T");
        rec.Columns.Add("Value", typeof(string));
        foreach (string? v in values)
        {
            var row = rec.AddRow();
            if (v is not null) rec.Columns["Value"].Set(row, v);
        }
        return rec;
    }

    private static int[] GetIntColumn(Record rec, string col)
    {
        var result = new int[rec.Count];
        for (int i = 0; i < rec.Count; i++)
            result[i] = rec.Columns[col]!.To<int>(i);
        return result;
    }

    private static string?[] GetStringColumn(Record rec, string col)
    {
        var result = new string?[rec.Count];
        for (int i = 0; i < rec.Count; i++)
            result[i] = rec.Columns[col]!.Get(i) as string;
        return result;
    }

    // ── 单列升序 ─────────────────────────────────────────────────────────────

    [TestMethod]
    public void Sort_SingleColumn_Int_Ascending()
    {
        var rec = BuildIntRecord(3, 1, 4, 1, 5, 9, 2, 6);
        rec.Sort("Value ASC");
        var result = GetIntColumn(rec, "Value");
        CollectionAssert.AreEqual(new[] { 1, 1, 2, 3, 4, 5, 6, 9 }, result);
    }

    [TestMethod]
    public void Sort_SingleColumn_Int_Descending()
    {
        var rec = BuildIntRecord(3, 1, 4, 1, 5);
        rec.Sort("Value DESC");
        var result = GetIntColumn(rec, "Value");
        CollectionAssert.AreEqual(new[] { 5, 4, 3, 1, 1 }, result);
    }

    [TestMethod]
    public void Sort_SingleColumn_DefaultIsAscending()
    {
        var rec = BuildIntRecord(5, 2, 8, 1);
        rec.Sort("Value");
        var result = GetIntColumn(rec, "Value");
        CollectionAssert.AreEqual(new[] { 1, 2, 5, 8 }, result);
    }

    [TestMethod]
    public void Sort_SingleColumn_String_Ascending()
    {
        var rec = BuildStringRecord("Charlie", "Alice", "Bob");
        rec.Sort("Value ASC");
        var result = GetStringColumn(rec, "Value");
        CollectionAssert.AreEqual(new[] { "Alice", "Bob", "Charlie" }, result);
    }

    [TestMethod]
    public void Sort_SingleColumn_String_Descending()
    {
        var rec = BuildStringRecord("Charlie", "Alice", "Bob");
        rec.Sort("Value DESC");
        var result = GetStringColumn(rec, "Value");
        CollectionAssert.AreEqual(new[] { "Charlie", "Bob", "Alice" }, result);
    }

    [TestMethod]
    public void Sort_SingleColumn_DateTime_Ascending()
    {
        var rec = new Record("T");
        rec.Columns.Add("Date", typeof(DateTime));
        var d1 = new DateTime(2024, 1, 1);
        var d2 = new DateTime(2023, 6, 15);
        var d3 = new DateTime(2025, 3, 10);
        foreach (var d in new[] { d1, d2, d3 })
        {
            var row = rec.AddRow();
            rec.Columns["Date"].Set(row, d);
        }
        rec.Sort("Date ASC");
        var col = rec.Columns["Date"]!;
        Assert.AreEqual(d2, col.Get(0));
        Assert.AreEqual(d1, col.Get(1));
        Assert.AreEqual(d3, col.Get(2));
    }

    // ── 多列混合方向 ─────────────────────────────────────────────────────────

    [TestMethod]
    public void Sort_MultiColumn_Mixed_Direction()
    {
        // dept ASC, salary DESC
        var rec = new Record("T");
        rec.Columns.Add("Dept", typeof(string));
        rec.Columns.Add("Salary", typeof(int));

        void AddRow(string dept, int salary)
        {
            var row = rec.AddRow();
            rec.Columns["Dept"].Set(row, dept);
            rec.Columns["Salary"].Set(row, salary);
        }

        AddRow("B", 300);
        AddRow("A", 100);
        AddRow("A", 200);
        AddRow("B", 400);

        rec.Sort("Dept ASC, Salary DESC");

        var depts = GetStringColumn(rec, "Dept");
        var salaries = GetIntColumn(rec, "Salary");
        CollectionAssert.AreEqual(new[] { "A", "A", "B", "B" }, depts);
        CollectionAssert.AreEqual(new[] { 200, 100, 400, 300 }, salaries);
    }

    // ── NULL 行为 ─────────────────────────────────────────────────────────────

    [TestMethod]
    public void Sort_NullIsSmallest_AscNullFirst()
    {
        var rec = BuildStringRecord("B", null, "A");
        rec.Sort("Value ASC");
        var result = GetStringColumn(rec, "Value");
        // null 视为最小值 → ASC 时排在最前
        Assert.IsNull(result[0]);
        Assert.AreEqual("A", result[1]);
        Assert.AreEqual("B", result[2]);
    }

    [TestMethod]
    public void Sort_NullIsSmallest_DescNullLast()
    {
        var rec = BuildStringRecord("B", null, "A");
        rec.Sort("Value DESC");
        var result = GetStringColumn(rec, "Value");
        // null 视为最小值 → DESC 时排在最后
        Assert.AreEqual("B", result[0]);
        Assert.AreEqual("A", result[1]);
        Assert.IsNull(result[2]);
    }

    [TestMethod]
    public void Sort_AllNull_Stable()
    {
        var rec = new Record("T");
        rec.Columns.Add("A", typeof(string));
        rec.Columns.Add("B", typeof(int));
        for (int i = 1; i <= 3; i++)
        {
            var row = rec.AddRow();
            rec.Columns["B"].Set(row, i); // B 用于验证稳定性
        }
        rec.Sort("A ASC");
        // 全为 null，顺序应维持原始（B 仍为 1,2,3）
        var bs = GetIntColumn(rec, "B");
        CollectionAssert.AreEqual(new[] { 1, 2, 3 }, bs);
    }

    // ── 排序稳定性 ──────────────────────────────────────────────────────────

    [TestMethod]
    public void Sort_StableWhenKeysEqual()
    {
        var rec = new Record("T");
        rec.Columns.Add("Key", typeof(int));
        rec.Columns.Add("Seq", typeof(int));
        for (int i = 0; i < 5; i++)
        {
            var row = rec.AddRow();
            rec.Columns["Key"].Set(row, 1); // 所有 Key 相等
            rec.Columns["Seq"].Set(row, i);
        }
        rec.Sort("Key ASC");
        var seqs = GetIntColumn(rec, "Seq");
        CollectionAssert.AreEqual(new[] { 0, 1, 2, 3, 4 }, seqs);
    }

    // ── 字符串解析：空白与大小写 ─────────────────────────────────────────────

    [TestMethod]
    public void Sort_ParseString_WhitespaceAndCasing()
    {
        var rec = BuildIntRecord(3, 1, 2);
        // 方向关键字大小写不敏感，前后可有空格
        rec.Sort("  Value   DESC  ");
        var result = GetIntColumn(rec, "Value");
        CollectionAssert.AreEqual(new[] { 3, 2, 1 }, result);
    }

    [TestMethod]
    public void Sort_ParseString_MixedCaseDirection()
    {
        var rec = BuildIntRecord(3, 1, 2);
        rec.Sort("Value desc");
        var result = GetIntColumn(rec, "Value");
        CollectionAssert.AreEqual(new[] { 3, 2, 1 }, result);
    }

    // ── 元组重载 ─────────────────────────────────────────────────────────────

    [TestMethod]
    public void Sort_TupleOverload_Ascending()
    {
        var rec = BuildIntRecord(3, 1, 2);
        rec.Sort(("Value", false));
        var result = GetIntColumn(rec, "Value");
        CollectionAssert.AreEqual(new[] { 1, 2, 3 }, result);
    }

    [TestMethod]
    public void Sort_TupleOverload_Descending()
    {
        var rec = BuildIntRecord(3, 1, 2);
        rec.Sort(("Value", true));
        var result = GetIntColumn(rec, "Value");
        CollectionAssert.AreEqual(new[] { 3, 2, 1 }, result);
    }

    [TestMethod]
    public void Sort_TupleOverload_MultiColumn()
    {
        var rec = new Record("T");
        rec.Columns.Add("A", typeof(int));
        rec.Columns.Add("B", typeof(int));
        void AddRow(int a, int b) { var r = rec.AddRow(); rec.Columns["A"].Set(r, a); rec.Columns["B"].Set(r, b); }
        AddRow(2, 10);
        AddRow(1, 20);
        AddRow(1, 10);

        rec.Sort(("A", false), ("B", true));

        var aVals = GetIntColumn(rec, "A");
        var bVals = GetIntColumn(rec, "B");
        CollectionAssert.AreEqual(new[] { 1, 1, 2 }, aVals);
        CollectionAssert.AreEqual(new[] { 20, 10, 10 }, bVals);
    }

    // ── Count / Capacity / 列引用不变 ───────────────────────────────────────

    [TestMethod]
    public void Sort_PreservesCountAndCapacity()
    {
        var rec = BuildIntRecord(5, 3, 1, 4, 2);
        int beforeCount = rec.Count;
        int beforeCap = rec.Capacity;
        var colRef = rec.Columns["Value"];

        rec.Sort("Value ASC");

        Assert.AreEqual(beforeCount, rec.Count);
        Assert.AreEqual(beforeCap, rec.Capacity);
        Assert.AreSame(colRef, rec.Columns["Value"]);
    }

    [TestMethod]
    public void Sort_NonSortedColumnsAlsoReordered()
    {
        var rec = new Record("T");
        rec.Columns.Add("Key", typeof(int));
        rec.Columns.Add("Tag", typeof(string));
        void AddRow(int k, string t) { var r = rec.AddRow(); rec.Columns["Key"].Set(r, k); rec.Columns["Tag"].Set(r, t); }
        AddRow(3, "three");
        AddRow(1, "one");
        AddRow(2, "two");

        rec.Sort("Key ASC");

        var keys = GetIntColumn(rec, "Key");
        var tags = GetStringColumn(rec, "Tag");
        CollectionAssert.AreEqual(new[] { 1, 2, 3 }, keys);
        CollectionAssert.AreEqual(new[] { "one", "two", "three" }, tags);
    }

    // ── 边界情况 ─────────────────────────────────────────────────────────────

    [TestMethod]
    public void Sort_EmptyRecord_NoException()
    {
        var rec = new Record("T");
        rec.Columns.Add("Value", typeof(int));
        rec.Sort("Value ASC"); // 空表不应抛异常
        Assert.AreEqual(0, rec.Count);
    }

    [TestMethod]
    public void Sort_SingleRow_NoChange()
    {
        var rec = BuildIntRecord(42);
        rec.Sort("Value ASC");
        Assert.AreEqual(42, rec.Columns["Value"]!.To<int>(0));
    }

    [TestMethod]
    public void Sort_AlreadySorted_NoChange()
    {
        var rec = BuildIntRecord(1, 2, 3);
        rec.Sort("Value ASC");
        CollectionAssert.AreEqual(new[] { 1, 2, 3 }, GetIntColumn(rec, "Value"));
    }

    // ── 错误用例 ─────────────────────────────────────────────────────────────

    [TestMethod]
    public void Sort_EmptyString_Returns()
    {
        var rec = BuildIntRecord(3, 1, 2);
        rec.Sort("");
        // 数据应保持不变
        CollectionAssert.AreEqual(new[] { 3, 1, 2 }, GetIntColumn(rec, "Value"));
    }

    [TestMethod]
    public void Sort_WhitespaceString_Returns()
    {
        var rec = BuildIntRecord(3, 1, 2);
        rec.Sort("   ");
        CollectionAssert.AreEqual(new[] { 3, 1, 2 }, GetIntColumn(rec, "Value"));
    }

    [TestMethod]
    public void Sort_UnknownColumn_Throws()
    {
        var rec = BuildIntRecord(1, 2);
        Assert.Throws<KeyNotFoundException>(() => rec.Sort("NoSuchColumn ASC"));
    }

    [TestMethod]
    public void Sort_InvalidDirection_Throws()
    {
        var rec = BuildIntRecord(1, 2);
        Assert.Throws<FormatException>(() => rec.Sort("Value UPWARD"));
    }

    [TestMethod]
    public void Sort_DuplicateColumn_Throws()
    {
        var rec = BuildIntRecord(1, 2);
        Assert.Throws<ArgumentException>(() =>
            rec.Sort(new RecordSortKey("Value", false), new RecordSortKey("Value", true)));
    }

    [TestMethod]
    public void Sort_EmptyKeys_Returns()
    {
        var rec = BuildIntRecord(3, 1, 2);
        rec.Sort(Array.Empty<RecordSortKey>());
        CollectionAssert.AreEqual(new[] { 3, 1, 2 }, GetIntColumn(rec, "Value"));
    }

    [TestMethod]
    public void Sort_TooManyTokensInSegment_Throws()
    {
        var rec = BuildIntRecord(1, 2);
        Assert.Throws<FormatException>(() => rec.Sort("Value ASC Extra"));
    }
}
