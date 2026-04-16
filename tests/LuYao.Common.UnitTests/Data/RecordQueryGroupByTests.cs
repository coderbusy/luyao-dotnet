using System;
using System.Linq;

namespace LuYao.Data;

[TestClass]
public class RecordQueryGroupByTests
{
    private static Record CreateSalesRecord()
    {
        var record = new Record("Sales", 6);
        var regionCol = record.Columns.Add<string>("Region");
        var productCol = record.Columns.Add<string>("Product");
        var amountCol = record.Columns.Add<int>("Amount");
        var priceCol = record.Columns.Add<double>("Price");

        var data = new[]
        {
            ("East", "Widget", 10, 5.0),
            ("East", "Gadget", 20, 3.0),
            ("West", "Widget", 15, 5.0),
            ("West", "Gadget", 25, 3.0),
            ("East", "Widget", 5, 5.0),
            ("West", "Widget", 30, 5.0),
        };
        foreach (var (region, product, amount, price) in data)
        {
            var row = record.AddRow();
            regionCol.Set(region, row.Row);
            productCol.Set(product, row.Row);
            amountCol.Set(amount, row.Row);
            priceCol.Set(price, row.Row);
        }
        return record;
    }

    #region GroupBy + Count

    [TestMethod]
    public void WhenGroupByWithCountThenCountsPerGroup()
    {
        var sales = CreateSalesRecord();

        var result = sales.AsQuery()
            .GroupBy(new[] { "Region" }, AggregateDefinition.Count())
            .ToRecord();

        Assert.AreEqual(2, result.Count); // East, West
        Assert.AreEqual(2, result.Columns.Count); // Region, Count

        var regionCol = result.Columns.Get("Region");
        var countCol = result.Columns.Get("Count");

        // East: 3 rows, West: 3 rows
        for (int i = 0; i < result.Count; i++)
        {
            Assert.AreEqual(3, countCol.GetValue(i));
        }
    }

    [TestMethod]
    public void WhenGroupByMultipleKeysThenGroupsCorrectly()
    {
        var sales = CreateSalesRecord();

        var result = sales.AsQuery()
            .GroupBy(new[] { "Region", "Product" }, AggregateDefinition.Count())
            .ToRecord();

        // East+Widget(2), East+Gadget(1), West+Widget(2), West+Gadget(1) = 4 groups
        Assert.AreEqual(4, result.Count);
        Assert.AreEqual(3, result.Columns.Count); // Region, Product, Count
    }

    #endregion

    #region GroupBy + Sum

    [TestMethod]
    public void WhenGroupByWithSumThenSumsPerGroup()
    {
        var sales = CreateSalesRecord();

        var result = sales.AsQuery()
            .GroupBy(new[] { "Region" }, AggregateDefinition.Sum("Amount"))
            .ToRecord();

        Assert.AreEqual(2, result.Count);
        var sumCol = result.Columns.Get("Sum_Amount");
        var regionCol = result.Columns.Get("Region");

        for (int i = 0; i < result.Count; i++)
        {
            var region = (string)regionCol.GetValue(i)!;
            var sum = (double)sumCol.GetValue(i)!;
            if (region == "East") Assert.AreEqual(35.0, sum); // 10+20+5
            else Assert.AreEqual(70.0, sum); // 15+25+30
        }
    }

    #endregion

    #region GroupBy + Min/Max

    [TestMethod]
    public void WhenGroupByWithMinThenFindsMinPerGroup()
    {
        var sales = CreateSalesRecord();

        var result = sales.AsQuery()
            .GroupBy(new[] { "Region" }, AggregateDefinition.Min("Amount"))
            .ToRecord();

        var regionCol = result.Columns.Get("Region");
        var minCol = result.Columns.Get("Min_Amount");

        for (int i = 0; i < result.Count; i++)
        {
            var region = (string)regionCol.GetValue(i)!;
            var min = (int)minCol.GetValue(i)!;
            if (region == "East") Assert.AreEqual(5, min);
            else Assert.AreEqual(15, min);
        }
    }

    [TestMethod]
    public void WhenGroupByWithMaxThenFindsMaxPerGroup()
    {
        var sales = CreateSalesRecord();

        var result = sales.AsQuery()
            .GroupBy(new[] { "Region" }, AggregateDefinition.Max("Amount"))
            .ToRecord();

        var regionCol = result.Columns.Get("Region");
        var maxCol = result.Columns.Get("Max_Amount");

        for (int i = 0; i < result.Count; i++)
        {
            var region = (string)regionCol.GetValue(i)!;
            var max = (int)maxCol.GetValue(i)!;
            if (region == "East") Assert.AreEqual(20, max);
            else Assert.AreEqual(30, max);
        }
    }

    #endregion

    #region GroupBy + Avg

    [TestMethod]
    public void WhenGroupByWithAvgThenAveragesPerGroup()
    {
        var sales = CreateSalesRecord();

        var result = sales.AsQuery()
            .GroupBy(new[] { "Region" }, AggregateDefinition.Avg("Amount"))
            .ToRecord();

        var regionCol = result.Columns.Get("Region");
        var avgCol = result.Columns.Get("Avg_Amount");

        for (int i = 0; i < result.Count; i++)
        {
            var region = (string)regionCol.GetValue(i)!;
            var avg = (double)avgCol.GetValue(i)!;
            if (region == "East") Assert.AreEqual(35.0 / 3, avg, 0.001); // (10+20+5)/3
            else Assert.AreEqual(70.0 / 3, avg, 0.001); // (15+25+30)/3
        }
    }

    #endregion

    #region Multiple Aggregates

    [TestMethod]
    public void WhenGroupByWithMultipleAggregatesThenAllComputed()
    {
        var sales = CreateSalesRecord();

        var result = sales.AsQuery()
            .GroupBy(new[] { "Region" },
                AggregateDefinition.Count(),
                AggregateDefinition.Sum("Amount", "TotalAmount"),
                AggregateDefinition.Min("Amount", "MinAmount"),
                AggregateDefinition.Max("Amount", "MaxAmount"))
            .ToRecord();

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual(5, result.Columns.Count); // Region, Count, TotalAmount, MinAmount, MaxAmount
        Assert.IsNotNull(result.Columns.Find("Count"));
        Assert.IsNotNull(result.Columns.Find("TotalAmount"));
        Assert.IsNotNull(result.Columns.Find("MinAmount"));
        Assert.IsNotNull(result.Columns.Find("MaxAmount"));
    }

    #endregion

    #region Custom Output Column Names

    [TestMethod]
    public void WhenAggregateWithCustomOutputNameThenUsesIt()
    {
        var sales = CreateSalesRecord();

        var result = sales.AsQuery()
            .GroupBy(new[] { "Region" }, AggregateDefinition.Sum("Amount", "Revenue"))
            .ToRecord();

        Assert.IsNotNull(result.Columns.Find("Revenue"));
        Assert.IsNull(result.Columns.Find("Sum_Amount"));
    }

    #endregion

    #region Empty Table

    [TestMethod]
    public void WhenGroupByEmptyTableThenReturnsEmptyWithSchema()
    {
        var record = new Record("Empty", 0);
        record.Columns.Add<string>("Region");
        record.Columns.Add<int>("Amount");

        var result = record.AsQuery()
            .GroupBy(new[] { "Region" }, AggregateDefinition.Count())
            .ToRecord();

        Assert.AreEqual(0, result.Count);
        Assert.AreEqual(2, result.Columns.Count); // Region, Count
    }

    #endregion

    #region Error Cases

    [TestMethod]
    public void WhenGroupByNonExistentKeyColumnThenThrowsOnToRecord()
    {
        var sales = CreateSalesRecord();
        var query = sales.AsQuery()
            .GroupBy(new[] { "NonExistent" }, AggregateDefinition.Count());

        Assert.Throws<InvalidOperationException>(() => query.ToRecord());
    }

    [TestMethod]
    public void WhenGroupByNonExistentAggregateColumnThenThrowsOnToRecord()
    {
        var sales = CreateSalesRecord();
        var query = sales.AsQuery()
            .GroupBy(new[] { "Region" }, AggregateDefinition.Sum("NonExistent"));

        Assert.Throws<InvalidOperationException>(() => query.ToRecord());
    }

    [TestMethod]
    public void WhenGroupByNullKeysThenThrows()
    {
        var sales = CreateSalesRecord();
        Assert.Throws<ArgumentNullException>(() => sales.AsQuery().GroupBy(null!, AggregateDefinition.Count()));
    }

    [TestMethod]
    public void WhenGroupByEmptyKeysThenThrows()
    {
        var sales = CreateSalesRecord();
        Assert.Throws<ArgumentException>(() => sales.AsQuery().GroupBy(new string[0], AggregateDefinition.Count()));
    }

    [TestMethod]
    public void WhenGroupByNullAggregatesThenThrows()
    {
        var sales = CreateSalesRecord();
        Assert.Throws<ArgumentNullException>(() => sales.AsQuery().GroupBy(new[] { "Region" }, null!));
    }

    #endregion

    #region Combined with other operations

    [TestMethod]
    public void WhenWhereBeforeGroupByThenFiltersFirst()
    {
        var sales = CreateSalesRecord();

        var result = sales.AsQuery()
            .Where(r => r.Get<string>("Product") == "Widget")
            .GroupBy(new[] { "Region" }, AggregateDefinition.Sum("Amount", "Total"))
            .ToRecord();

        Assert.AreEqual(2, result.Count);
        var regionCol = result.Columns.Get("Region");
        var totalCol = result.Columns.Get("Total");

        for (int i = 0; i < result.Count; i++)
        {
            var region = (string)regionCol.GetValue(i)!;
            var total = (double)totalCol.GetValue(i)!;
            if (region == "East") Assert.AreEqual(15.0, total); // 10+5
            else Assert.AreEqual(45.0, total); // 15+30
        }
    }

    [TestMethod]
    public void WhenGroupByThenOrderByThenSorts()
    {
        var sales = CreateSalesRecord();

        var result = sales.AsQuery()
            .GroupBy(new[] { "Region" },
                AggregateDefinition.Sum("Amount", "Total"))
            .OrderBy("Total", descending: true)
            .ToRecord();

        Assert.AreEqual(2, result.Count);
        var totalCol = result.Columns.Get("Total");
        var first = (double)totalCol.GetValue(0)!;
        var second = (double)totalCol.GetValue(1)!;
        Assert.IsTrue(first >= second);
    }

    #endregion
}
