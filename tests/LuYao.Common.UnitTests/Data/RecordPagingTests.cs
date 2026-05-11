using System;

namespace LuYao.Data;

[TestClass]
public class RecordPagingTests
{
    [TestMethod]
    public void WhenDefaultConstructorThenPageIsOne()
    {
        var table = new RecordTable();
        Assert.AreEqual(1, table.Page);
    }

    [TestMethod]
    public void WhenDefaultConstructorThenPageSizeIsZero()
    {
        var table = new RecordTable();
        Assert.AreEqual(0, table.PageSize);
    }

    [TestMethod]
    public void WhenMaxCountNotSetThenReturnsCount()
    {
        var table = new RecordTable();
        table.Columns.Add<int>("Id");
        table.AddRow();
        table.AddRow();

        Assert.AreEqual(2, table.MaxCount);
    }

    [TestMethod]
    public void WhenMaxCountSetThenReturnsSetValue()
    {
        var table = new RecordTable();
        table.MaxCount = 100;

        Assert.AreEqual(100, table.MaxCount);
    }

    [TestMethod]
    public void WhenMaxCountZeroThenMaxPageIsZero()
    {
        var table = new RecordTable();

        Assert.AreEqual(0, table.MaxPage);
    }

    [TestMethod]
    public void WhenPageSizeZeroThenMaxPageUsesDefault20()
    {
        var table = new RecordTable();
        table.MaxCount = 50;

        Assert.AreEqual(3, table.MaxPage); // (50-1)/20+1 = 3
    }

    [TestMethod]
    public void WhenPageSizeSetThenMaxPageCalculatesCorrectly()
    {
        var table = new RecordTable();
        table.MaxCount = 100;
        table.PageSize = 10;

        Assert.AreEqual(10, table.MaxPage);
    }

    [TestMethod]
    public void WhenMaxCountNotExactMultipleThenMaxPageRoundsUp()
    {
        var table = new RecordTable();
        table.MaxCount = 101;
        table.PageSize = 10;

        Assert.AreEqual(11, table.MaxPage);
    }

    [TestMethod]
    public void WhenCloneThenPagingPropertiesAreCopied()
    {
        var table = new RecordTable("Test", 0);
        table.Page = 3;
        table.PageSize = 25;
        table.MaxCount = 200;

        var clone = table.Clone();

        Assert.AreEqual(3, clone.Page);
        Assert.AreEqual(25, clone.PageSize);
        Assert.AreEqual(200, clone.MaxCount);
        Assert.AreEqual(8, clone.MaxPage);
    }
}
