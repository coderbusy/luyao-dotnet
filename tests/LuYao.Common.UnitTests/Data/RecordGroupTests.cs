using System.Collections.Generic;

namespace LuYao.Data;

[TestClass]
public class RecordGroupTests
{
    private static Record BuildRecord(params (string cat, int id, string sub, int score, string tag, string region)[] rows)
    {
        var record = new Record();
        var catCol    = record.Columns.Add<string>("Category");
        var idCol     = record.Columns.Add<int>("Id");
        var subCol    = record.Columns.Add<string>("Sub");
        var scoreCol  = record.Columns.Add<int>("Score");
        var tagCol    = record.Columns.Add<string>("Tag");
        var regionCol = record.Columns.Add<string>("Region");

        foreach (var (cat, id, sub, score, tag, region) in rows)
        {
            var r = record.AddRow();
            catCol.SetValue(r.Row, cat);
            idCol.SetValue(r.Row, id);
            subCol.SetValue(r.Row, sub);
            scoreCol.SetValue(r.Row, score);
            tagCol.SetValue(r.Row, tag);
            regionCol.SetValue(r.Row, region);
        }
        return record;
    }

    // ── Group<T> ─────────────────────────────────────────────────────────────

    [TestMethod]
    public void Group_ByExistingColumn_GroupsRowsCorrectly()
    {
        var record = BuildRecord(
            ("A", 1, "x", 10, "t1", "r1"),
            ("B", 2, "y", 20, "t2", "r2"),
            ("A", 3, "z", 30, "t3", "r3"),
            ("B", 4, "w", 40, "t4", "r4"));

        var groups = record.Group<string>("Category");

        Assert.AreEqual(2, groups.Count);
        Assert.AreEqual(2, groups["A"].Count);
        Assert.AreEqual(2, groups["B"].Count);
        Assert.AreEqual(0, groups["A"][0].Row);
        Assert.AreEqual(2, groups["A"][1].Row);
        Assert.AreEqual(1, groups["B"][0].Row);
        Assert.AreEqual(3, groups["B"][1].Row);
    }

    [TestMethod]
    public void Group_AllRowsSameKey_ReturnsSingleGroup()
    {
        var record = BuildRecord(
            ("X", 1, "a", 1, "t", "r"),
            ("X", 2, "b", 2, "t", "r"),
            ("X", 3, "c", 3, "t", "r"));

        var groups = record.Group<string>("Category");

        Assert.AreEqual(1, groups.Count);
        Assert.AreEqual(3, groups["X"].Count);
    }

    [TestMethod]
    public void Group_AllRowsDistinctKey_ReturnsOneGroupPerRow()
    {
        var record = BuildRecord(
            ("A", 10, "a", 1, "t", "r"),
            ("B", 20, "b", 2, "t", "r"),
            ("C", 30, "c", 3, "t", "r"));

        var groups = record.Group<string>("Category");

        Assert.AreEqual(3, groups.Count);
        Assert.AreEqual(1, groups["A"].Count);
        Assert.AreEqual(1, groups["B"].Count);
        Assert.AreEqual(1, groups["C"].Count);
    }

    [TestMethod]
    public void Group_NonExistentColumn_ReturnsEmptyDictionary()
    {
        var record = BuildRecord(("A", 1, "x", 1, "t", "r"));

        var groups = record.Group<string>("NoSuchColumn");

        Assert.AreEqual(0, groups.Count);
    }

    [TestMethod]
    public void Group_EmptyRecord_ReturnsEmptyDictionary()
    {
        var record = new Record();
        record.Columns.Add<string>("Category");

        var groups = record.Group<string>("Category");

        Assert.AreEqual(0, groups.Count);
    }

    // ── Group<T1, T2> ────────────────────────────────────────────────────────

    [TestMethod]
    public void Group2_GroupsRowsByTwoColumns()
    {
        var record = BuildRecord(
            ("A", 1, "x", 10, "t1", "r1"),
            ("A", 2, "y", 20, "t2", "r2"),
            ("B", 1, "z", 30, "t3", "r3"),
            ("A", 1, "w", 40, "t4", "r4"));

        var groups = record.Group<string, int>("Category", "Id");

        Assert.AreEqual(3, groups.Count);
        Assert.AreEqual(2, groups[("A", 1)].Count);
        Assert.AreEqual(1, groups[("A", 2)].Count);
        Assert.AreEqual(1, groups[("B", 1)].Count);
    }

    [TestMethod]
    public void Group2_MissingFirstColumn_ReturnsEmpty()
    {
        var record = BuildRecord(("A", 1, "x", 1, "t", "r"));

        var groups = record.Group<string, int>("NoColumn", "Id");

        Assert.AreEqual(0, groups.Count);
    }

    [TestMethod]
    public void Group2_MissingSecondColumn_ReturnsEmpty()
    {
        var record = BuildRecord(("A", 1, "x", 1, "t", "r"));

        var groups = record.Group<string, int>("Category", "NoColumn");

        Assert.AreEqual(0, groups.Count);
    }

    [TestMethod]
    public void Group2_EmptyRecord_ReturnsEmpty()
    {
        var record = new Record();
        record.Columns.Add<string>("Category");
        record.Columns.Add<int>("Id");

        var groups = record.Group<string, int>("Category", "Id");

        Assert.AreEqual(0, groups.Count);
    }

    // ── Group<T1, T2, T3> ────────────────────────────────────────────────────

    [TestMethod]
    public void Group3_GroupsRowsByThreeColumns()
    {
        var record = BuildRecord(
            ("A", 1, "x", 10, "t1", "r1"),
            ("A", 1, "y", 20, "t2", "r2"),
            ("A", 2, "x", 30, "t3", "r3"),
            ("B", 1, "x", 40, "t4", "r4"),
            ("A", 1, "x", 50, "t5", "r5"));

        var groups = record.Group<string, int, string>("Category", "Id", "Sub");

        Assert.AreEqual(4, groups.Count);
        Assert.AreEqual(2, groups[("A", 1, "x")].Count);
        Assert.AreEqual(1, groups[("A", 1, "y")].Count);
        Assert.AreEqual(1, groups[("A", 2, "x")].Count);
        Assert.AreEqual(1, groups[("B", 1, "x")].Count);
    }

    [TestMethod]
    public void Group3_MissingColumn_ReturnsEmpty()
    {
        var record = BuildRecord(("A", 1, "x", 1, "t", "r"));

        var groups = record.Group<string, int, string>("Category", "Id", "NoColumn");

        Assert.AreEqual(0, groups.Count);
    }

    // ── Group<T1, T2, T3, T4> ────────────────────────────────────────────────

    [TestMethod]
    public void Group4_GroupsRowsByFourColumns()
    {
        var record = BuildRecord(
            ("A", 1, "x", 10, "t1", "r1"),
            ("A", 1, "x", 10, "t2", "r2"),
            ("A", 1, "x", 20, "t3", "r3"),
            ("B", 1, "x", 10, "t4", "r4"));

        var groups = record.Group<string, int, string, int>("Category", "Id", "Sub", "Score");

        Assert.AreEqual(3, groups.Count);
        Assert.AreEqual(2, groups[("A", 1, "x", 10)].Count);
        Assert.AreEqual(1, groups[("A", 1, "x", 20)].Count);
        Assert.AreEqual(1, groups[("B", 1, "x", 10)].Count);
    }

    [TestMethod]
    public void Group4_MissingColumn_ReturnsEmpty()
    {
        var record = BuildRecord(("A", 1, "x", 1, "t", "r"));

        var groups = record.Group<string, int, string, int>("Category", "Id", "Sub", "NoColumn");

        Assert.AreEqual(0, groups.Count);
    }

    // ── Group<T1, T2, T3, T4, T5> ────────────────────────────────────────────

    [TestMethod]
    public void Group5_GroupsRowsByFiveColumns()
    {
        var record = BuildRecord(
            ("A", 1, "x", 10, "t1", "r1"),
            ("A", 1, "x", 10, "t1", "r2"),
            ("A", 1, "x", 10, "t2", "r1"),
            ("B", 1, "x", 10, "t1", "r1"));

        var groups = record.Group<string, int, string, int, string>("Category", "Id", "Sub", "Score", "Tag");

        Assert.AreEqual(3, groups.Count);
        Assert.AreEqual(2, groups[("A", 1, "x", 10, "t1")].Count);
        Assert.AreEqual(1, groups[("A", 1, "x", 10, "t2")].Count);
        Assert.AreEqual(1, groups[("B", 1, "x", 10, "t1")].Count);
    }

    [TestMethod]
    public void Group5_MissingColumn_ReturnsEmpty()
    {
        var record = BuildRecord(("A", 1, "x", 1, "t", "r"));

        var groups = record.Group<string, int, string, int, string>("Category", "Id", "Sub", "Score", "NoColumn");

        Assert.AreEqual(0, groups.Count);
    }

    // ── Group<T1, T2, T3, T4, T5, T6> ───────────────────────────────────────

    [TestMethod]
    public void Group6_GroupsRowsBySixColumns()
    {
        var record = BuildRecord(
            ("A", 1, "x", 10, "t1", "r1"),
            ("A", 1, "x", 10, "t1", "r1"),
            ("A", 1, "x", 10, "t1", "r2"),
            ("B", 1, "x", 10, "t1", "r1"));

        var groups = record.Group<string, int, string, int, string, string>("Category", "Id", "Sub", "Score", "Tag", "Region");

        Assert.AreEqual(3, groups.Count);
        Assert.AreEqual(2, groups[("A", 1, "x", 10, "t1", "r1")].Count);
        Assert.AreEqual(1, groups[("A", 1, "x", 10, "t1", "r2")].Count);
        Assert.AreEqual(1, groups[("B", 1, "x", 10, "t1", "r1")].Count);
    }

    [TestMethod]
    public void Group6_MissingColumn_ReturnsEmpty()
    {
        var record = BuildRecord(("A", 1, "x", 1, "t", "r"));

        var groups = record.Group<string, int, string, int, string, string>("Category", "Id", "Sub", "Score", "Tag", "NoColumn");

        Assert.AreEqual(0, groups.Count);
    }

    [TestMethod]
    public void Group6_EmptyRecord_ReturnsEmpty()
    {
        var record = new Record();
        record.Columns.Add<string>("Category");
        record.Columns.Add<int>("Id");
        record.Columns.Add<string>("Sub");
        record.Columns.Add<int>("Score");
        record.Columns.Add<string>("Tag");
        record.Columns.Add<string>("Region");

        var groups = record.Group<string, int, string, int, string, string>("Category", "Id", "Sub", "Score", "Tag", "Region");

        Assert.AreEqual(0, groups.Count);
    }
}
