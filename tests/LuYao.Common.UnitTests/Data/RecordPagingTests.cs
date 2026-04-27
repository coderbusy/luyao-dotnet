using System;

namespace LuYao.Data;

[TestClass]
public class RecordPagingTests
{
    [TestMethod]
    public void WhenDefaultConstructorThenPageIsOne()
    {
        var record = new Record();
        Assert.AreEqual(1, record.Page);
    }

    [TestMethod]
    public void WhenDefaultConstructorThenPageSizeIsZero()
    {
        var record = new Record();
        Assert.AreEqual(0, record.PageSize);
    }

    [TestMethod]
    public void WhenMaxCountNotSetThenReturnsCount()
    {
        var record = new Record();
        record.Columns.Add<int>("Id");
        record.AddRow();
        record.AddRow();

        Assert.AreEqual(2, record.MaxCount);
    }

    [TestMethod]
    public void WhenMaxCountSetThenReturnsSetValue()
    {
        var record = new Record();
        record.MaxCount = 100;

        Assert.AreEqual(100, record.MaxCount);
    }

    [TestMethod]
    public void WhenMaxCountZeroThenMaxPageIsZero()
    {
        var record = new Record();

        Assert.AreEqual(0, record.MaxPage);
    }

    [TestMethod]
    public void WhenPageSizeZeroThenMaxPageUsesDefault20()
    {
        var record = new Record();
        record.MaxCount = 50;

        Assert.AreEqual(3, record.MaxPage); // (50-1)/20+1 = 3
    }

    [TestMethod]
    public void WhenPageSizeSetThenMaxPageCalculatesCorrectly()
    {
        var record = new Record();
        record.MaxCount = 100;
        record.PageSize = 10;

        Assert.AreEqual(10, record.MaxPage);
    }

    [TestMethod]
    public void WhenMaxCountNotExactMultipleThenMaxPageRoundsUp()
    {
        var record = new Record();
        record.MaxCount = 101;
        record.PageSize = 10;

        Assert.AreEqual(11, record.MaxPage);
    }

    [TestMethod]
    public void WhenCloneThenPagingPropertiesAreCopied()
    {
        var record = new Record("Test", 0);
        record.Page = 3;
        record.PageSize = 25;
        record.MaxCount = 200;

        var clone = record.Clone();

        Assert.AreEqual(3, clone.Page);
        Assert.AreEqual(25, clone.PageSize);
        Assert.AreEqual(200, clone.MaxCount);
        Assert.AreEqual(8, clone.MaxPage);
    }
}
