using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LuYao.Data;

[TestClass]
public class FrameGroupTests
{
    private Frame CreateTestFrame()
    {
        var record = new Frame();
        record.Columns.Add<int>("Id");
        record.Columns.Add<string>("Category");
        record.Columns.Add<string>("Status");

        var r1 = record.AddRow(); r1["Id"] = 1; r1["Category"] = "A"; r1["Status"] = "Active";
        var r2 = record.AddRow(); r2["Id"] = 2; r2["Category"] = "B"; r2["Status"] = "Inactive";
        var r3 = record.AddRow(); r3["Id"] = 3; r3["Category"] = "A"; r3["Status"] = "Active";
        var r4 = record.AddRow(); r4["Id"] = 4; r4["Category"] = "C"; r4["Status"] = "Active";
        var r5 = record.AddRow(); r5["Id"] = 5; r5["Category"] = "B"; r5["Status"] = "Active";

        return record;
    }

    [TestMethod]
    public void Group_ByStructColumn_ReturnsGroupedDictionary()
    {
        var record = CreateTestFrame();
        var groups = record.Group<int>("Id");

        Assert.IsNotNull(groups);
        Assert.AreEqual(5, groups.Count);
        Assert.IsTrue(groups.ContainsKey(1));
    }

    [TestMethod]
    public void Group_ByStringColumn_ReturnsGroupedDictionary()
    {
        var record = CreateTestFrame();
        var groups = record.Group("Category");

        Assert.IsNotNull(groups);
        Assert.AreEqual(3, groups.Count);
        Assert.IsTrue(groups.ContainsKey("A"));
        Assert.AreEqual(2, groups["A"].Count);
        Assert.IsTrue(groups.ContainsKey("B"));
        Assert.AreEqual(2, groups["B"].Count);
        Assert.IsTrue(groups.ContainsKey("C"));
        Assert.AreEqual(1, groups["C"].Count);
    }

    [TestMethod]
    public void Group_ByParamsStringColumns_ReturnsGroupedDictionary()
    {
        var record = CreateTestFrame();
        var groups = record.Group("Category", "Status");

        Assert.IsNotNull(groups);
        Assert.AreEqual(4, groups.Count);
        Assert.IsTrue(groups.ContainsKey("A-Active"));
        Assert.AreEqual(2, groups["A-Active"].Count);
        Assert.IsTrue(groups.ContainsKey("B-Inactive"));
        Assert.AreEqual(1, groups["B-Inactive"].Count);
        Assert.IsTrue(groups.ContainsKey("B-Active"));
        Assert.AreEqual(1, groups["B-Active"].Count);
        Assert.IsTrue(groups.ContainsKey("C-Active"));
        Assert.AreEqual(1, groups["C-Active"].Count);
    }

#if NETSTANDARD2_0_OR_GREATER || NET6_0_OR_GREATER
    [TestMethod]
    public void Group_ByTwoColumns_ReturnsGroupedDictionary()
    {
        var record = CreateTestFrame();
        var groups = record.Group<string, string>("Category", "Status");

        Assert.IsNotNull(groups);
        Assert.AreEqual(4, groups.Count);
        Assert.IsTrue(groups.ContainsKey(("A", "Active")));
        Assert.AreEqual(2, groups[("A", "Active")].Count);
    }

    [TestMethod]
    public void Group_ByThreeColumns_ReturnsGroupedDictionary()
    {
        var record = CreateTestFrame();
        var groups = record.Group<string, string, int>("Category", "Status", "Id");

        Assert.IsNotNull(groups);
        Assert.AreEqual(5, groups.Count);
        Assert.IsTrue(groups.ContainsKey(("A", "Active", 1)));
        Assert.AreEqual(1, groups[("A", "Active", 1)].Count);
    }
#endif
}
