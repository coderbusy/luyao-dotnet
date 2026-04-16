using System;
using System.Collections.Generic;
using System.Linq;

namespace LuYao.Data;

[TestClass]
public class RecordQueryTests
{
    private static Record CreateTestRecord()
    {
        var record = new Record("Test", 5);
        var idCol = record.Columns.Add<int>("Id");
        var nameCol = record.Columns.Add<string>("Name");
        var ageCol = record.Columns.Add<int>("Age");

        var data = new (int id, string name, int age)[]
        {
            (1, "Alice", 30),
            (2, "Bob", 25),
            (3, "Charlie", 35),
            (4, "Diana", 25),
            (5, "Eve", 30),
        };

        foreach (var (id, name, age) in data)
        {
            var row = record.AddRow();
            idCol.Set(id, row.Row);
            nameCol.Set(name, row.Row);
            ageCol.Set(age, row.Row);
        }
        return record;
    }

    #region AsQuery + ToRecord

    [TestMethod]
    public void WhenToRecordWithNoStepsThenReturnsClone()
    {
        var record = CreateTestRecord();

        var result = record.AsQuery().ToRecord();

        Assert.AreEqual(5, result.Count);
        Assert.AreEqual(3, result.Columns.Count);
        Assert.AreEqual("Test", result.Name);
        // 验证是独立实例
        Assert.AreNotSame(record, result);
    }

    [TestMethod]
    public void WhenToRecordMultipleTimesThenIndependentResults()
    {
        var record = CreateTestRecord();
        var query = record.AsQuery().Where(r => r.Get<int>("Id") > 2);

        var r1 = query.ToRecord();
        var r2 = query.ToRecord();

        Assert.AreNotSame(r1, r2);
        Assert.AreEqual(r1.Count, r2.Count);
    }

    [TestMethod]
    public void WhenToRecordThenOriginalUnchanged()
    {
        var record = CreateTestRecord();

        var result = record.AsQuery().Where(r => r.Get<int>("Id") == 1).ToRecord();

        Assert.AreEqual(5, record.Count);
        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    public void WhenEmptyRecordAsQueryThenReturnsEmptyWithSchema()
    {
        var record = new Record("Empty", 0);
        record.Columns.Add<int>("Id");
        record.Columns.Add<string>("Name");

        var result = record.AsQuery().ToRecord();

        Assert.AreEqual(0, result.Count);
        Assert.AreEqual(2, result.Columns.Count);
        Assert.AreEqual("Id", result.Columns[0].Name);
    }

    [TestMethod]
    public void WhenAsQueryWithOptionsThenAcceptsOptions()
    {
        var record = CreateTestRecord();
        var options = new QueryOptions
        {
            EnableIndexing = true,
            StringComparison = StringComparison.OrdinalIgnoreCase
        };

        var result = record.AsQuery(options).ToRecord();

        Assert.AreEqual(5, result.Count);
    }

    #endregion

    #region Where

    [TestMethod]
    public void WhenWhereMatchesSomeRowsThenFiltersCorrectly()
    {
        var record = CreateTestRecord();

        var result = record.AsQuery()
            .Where(r => r.Get<int>("Age") == 25)
            .ToRecord();

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("Bob", result.Columns.Get("Name").GetValue(0));
        Assert.AreEqual("Diana", result.Columns.Get("Name").GetValue(1));
    }

    [TestMethod]
    public void WhenWhereMatchesNoRowsThenReturnsEmptyWithSchema()
    {
        var record = CreateTestRecord();

        var result = record.AsQuery()
            .Where(r => r.Get<int>("Age") > 100)
            .ToRecord();

        Assert.AreEqual(0, result.Count);
        Assert.AreEqual(3, result.Columns.Count);
    }

    [TestMethod]
    public void WhenWhereMatchesAllRowsThenReturnsAll()
    {
        var record = CreateTestRecord();

        var result = record.AsQuery()
            .Where(r => r.Get<int>("Age") > 0)
            .ToRecord();

        Assert.AreEqual(5, result.Count);
    }

    [TestMethod]
    public void WhenWhereNullPredicateThenThrowsArgumentNullException()
    {
        var record = CreateTestRecord();

        Assert.Throws<ArgumentNullException>(() => record.AsQuery().Where(null!));
    }

    [TestMethod]
    public void WhenMultipleWheresThenChainedFiltering()
    {
        var record = CreateTestRecord();

        var result = record.AsQuery()
            .Where(r => r.Get<int>("Age") >= 30)
            .Where(r => r.Get<int>("Id") > 1)
            .ToRecord();

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("Charlie", result.Columns.Get("Name").GetValue(0));
        Assert.AreEqual("Eve", result.Columns.Get("Name").GetValue(1));
    }

    #endregion

    #region Select

    [TestMethod]
    public void WhenSelectColumnsThenProjectsCorrectly()
    {
        var record = CreateTestRecord();

        var result = record.AsQuery()
            .Select("Id", "Name")
            .ToRecord();

        Assert.AreEqual(5, result.Count);
        Assert.AreEqual(2, result.Columns.Count);
        Assert.AreEqual("Id", result.Columns[0].Name);
        Assert.AreEqual("Name", result.Columns[1].Name);
    }

    [TestMethod]
    public void WhenSelectPreservesDataValues()
    {
        var record = CreateTestRecord();

        var result = record.AsQuery()
            .Select("Name")
            .ToRecord();

        Assert.AreEqual("Alice", result.Columns.Get("Name").GetValue(0));
        Assert.AreEqual("Eve", result.Columns.Get("Name").GetValue(4));
    }

    [TestMethod]
    public void WhenSelectNonExistentColumnThenThrowsOnToRecord()
    {
        var record = CreateTestRecord();

        // 链式调用阶段不抛异常
        var query = record.AsQuery().Select("Id", "NonExistent");

        // 物化时抛出
        Assert.Throws<InvalidOperationException>(() => query.ToRecord());
    }

    [TestMethod]
    public void WhenSelectNullThenThrowsArgumentNullException()
    {
        var record = CreateTestRecord();

        Assert.Throws<ArgumentNullException>(() => record.AsQuery().Select(null!));
    }

    [TestMethod]
    public void WhenSelectEmptyArrayThenThrowsOnToRecord()
    {
        var record = CreateTestRecord();
        var query = record.AsQuery().Select();

        Assert.Throws<ArgumentException>(() => query.ToRecord());
    }

    #endregion

    #region OrderBy

    [TestMethod]
    public void WhenOrderByAscendingThenSortsCorrectly()
    {
        var record = CreateTestRecord();

        var result = record.AsQuery()
            .OrderBy("Age")
            .ToRecord();

        var ageCol = result.Columns.Get("Age");
        Assert.AreEqual(25, ageCol.GetValue(0));
        Assert.AreEqual(25, ageCol.GetValue(1));
        Assert.AreEqual(30, ageCol.GetValue(2));
        Assert.AreEqual(30, ageCol.GetValue(3));
        Assert.AreEqual(35, ageCol.GetValue(4));
    }

    [TestMethod]
    public void WhenOrderByDescendingThenSortsCorrectly()
    {
        var record = CreateTestRecord();

        var result = record.AsQuery()
            .OrderBy("Age", descending: true)
            .ToRecord();

        var ageCol = result.Columns.Get("Age");
        Assert.AreEqual(35, ageCol.GetValue(0));
        Assert.AreEqual(30, ageCol.GetValue(1));
        Assert.AreEqual(30, ageCol.GetValue(2));
        Assert.AreEqual(25, ageCol.GetValue(3));
        Assert.AreEqual(25, ageCol.GetValue(4));
    }

    [TestMethod]
    public void WhenOrderByNonExistentColumnThenThrowsOnToRecord()
    {
        var record = CreateTestRecord();
        var query = record.AsQuery().OrderBy("NonExistent");

        Assert.Throws<InvalidOperationException>(() => query.ToRecord());
    }

    [TestMethod]
    public void WhenOrderByEmptyTableThenReturnsEmpty()
    {
        var record = new Record("Empty", 0);
        record.Columns.Add<int>("Id");

        var result = record.AsQuery().OrderBy("Id").ToRecord();

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void WhenOrderByThenAllColumnsPreserved()
    {
        var record = CreateTestRecord();

        var result = record.AsQuery()
            .OrderBy("Age")
            .ToRecord();

        Assert.AreEqual(3, result.Columns.Count);
        // 最小 Age=25 的行应该有对应的 Name
        var nameCol = result.Columns.Get("Name");
        var ageCol = result.Columns.Get("Age");
        Assert.AreEqual(25, ageCol.GetValue(0));
        var name = (string)nameCol.GetValue(0)!;
        Assert.IsTrue(name == "Bob" || name == "Diana");
    }

    #endregion

    #region Distinct

    [TestMethod]
    public void WhenDistinctOnAllColumnsThenRemovesDuplicates()
    {
        var record = new Record("Test", 4);
        var idCol = record.Columns.Add<int>("Id");
        var nameCol = record.Columns.Add<string>("Name");
        record.AddRow(); idCol.Set(1, 0); nameCol.Set("A", 0);
        record.AddRow(); idCol.Set(2, 1); nameCol.Set("B", 1);
        record.AddRow(); idCol.Set(1, 2); nameCol.Set("A", 2);
        record.AddRow(); idCol.Set(3, 3); nameCol.Set("C", 3);

        var result = record.AsQuery().Distinct().ToRecord();

        Assert.AreEqual(3, result.Count);
    }

    [TestMethod]
    public void WhenDistinctOnSpecificColumnThenRemovesDuplicatesByColumn()
    {
        var record = CreateTestRecord();

        var result = record.AsQuery()
            .Distinct("Age")
            .ToRecord();

        // Ages are 30, 25, 35, 25, 30 → distinct ages: 30, 25, 35
        Assert.AreEqual(3, result.Count);
    }

    [TestMethod]
    public void WhenDistinctOnUniqueColumnThenReturnsAll()
    {
        var record = CreateTestRecord();

        var result = record.AsQuery()
            .Distinct("Id")
            .ToRecord();

        Assert.AreEqual(5, result.Count);
    }

    [TestMethod]
    public void WhenDistinctNonExistentColumnThenThrowsOnToRecord()
    {
        var record = CreateTestRecord();
        var query = record.AsQuery().Distinct("NonExistent");

        Assert.Throws<InvalidOperationException>(() => query.ToRecord());
    }

    [TestMethod]
    public void WhenDistinctEmptyTableThenReturnsEmpty()
    {
        var record = new Record("Empty", 0);
        record.Columns.Add<int>("Id");

        var result = record.AsQuery().Distinct().ToRecord();

        Assert.AreEqual(0, result.Count);
    }

    #endregion

    #region Take

    [TestMethod]
    public void WhenTakeFewerThanTotalThenReturnsThatMany()
    {
        var record = CreateTestRecord();

        var result = record.AsQuery().Take(3).ToRecord();

        Assert.AreEqual(3, result.Count);
        Assert.AreEqual(1, result.Columns.Get("Id").GetValue(0));
        Assert.AreEqual(3, result.Columns.Get("Id").GetValue(2));
    }

    [TestMethod]
    public void WhenTakeMoreThanTotalThenReturnsAll()
    {
        var record = CreateTestRecord();

        var result = record.AsQuery().Take(100).ToRecord();

        Assert.AreEqual(5, result.Count);
    }

    [TestMethod]
    public void WhenTakeZeroThenReturnsEmptyWithSchema()
    {
        var record = CreateTestRecord();

        var result = record.AsQuery().Take(0).ToRecord();

        Assert.AreEqual(0, result.Count);
        Assert.AreEqual(3, result.Columns.Count);
    }

    [TestMethod]
    public void WhenTakeNegativeThenReturnsEmptyWithSchema()
    {
        var record = CreateTestRecord();

        var result = record.AsQuery().Take(-1).ToRecord();

        Assert.AreEqual(0, result.Count);
        Assert.AreEqual(3, result.Columns.Count);
    }

    #endregion

    #region Skip

    [TestMethod]
    public void WhenSkipSomeThenSkipsRows()
    {
        var record = CreateTestRecord();

        var result = record.AsQuery().Skip(2).ToRecord();

        Assert.AreEqual(3, result.Count);
        Assert.AreEqual(3, result.Columns.Get("Id").GetValue(0));
    }

    [TestMethod]
    public void WhenSkipAllThenReturnsEmptyWithSchema()
    {
        var record = CreateTestRecord();

        var result = record.AsQuery().Skip(5).ToRecord();

        Assert.AreEqual(0, result.Count);
        Assert.AreEqual(3, result.Columns.Count);
    }

    [TestMethod]
    public void WhenSkipMoreThanTotalThenReturnsEmptyWithSchema()
    {
        var record = CreateTestRecord();

        var result = record.AsQuery().Skip(100).ToRecord();

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void WhenSkipZeroThenReturnsAll()
    {
        var record = CreateTestRecord();

        var result = record.AsQuery().Skip(0).ToRecord();

        Assert.AreEqual(5, result.Count);
    }

    [TestMethod]
    public void WhenSkipNegativeThenReturnsAll()
    {
        var record = CreateTestRecord();

        var result = record.AsQuery().Skip(-1).ToRecord();

        Assert.AreEqual(5, result.Count);
    }

    #endregion

    #region Combined Operations

    [TestMethod]
    public void WhenWhereAndSelectCombinedThenWorks()
    {
        var record = CreateTestRecord();

        var result = record.AsQuery()
            .Where(r => r.Get<int>("Age") == 30)
            .Select("Name")
            .ToRecord();

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual(1, result.Columns.Count);
        Assert.AreEqual("Alice", result.Columns.Get("Name").GetValue(0));
        Assert.AreEqual("Eve", result.Columns.Get("Name").GetValue(1));
    }

    [TestMethod]
    public void WhenSkipAndTakeCombinedThenPagination()
    {
        var record = CreateTestRecord();

        var result = record.AsQuery()
            .Skip(1)
            .Take(2)
            .ToRecord();

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual(2, result.Columns.Get("Id").GetValue(0));
        Assert.AreEqual(3, result.Columns.Get("Id").GetValue(1));
    }

    [TestMethod]
    public void WhenOrderByAndTakeCombinedThenTopN()
    {
        var record = CreateTestRecord();

        var result = record.AsQuery()
            .OrderBy("Age")
            .Take(2)
            .ToRecord();

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual(25, result.Columns.Get("Age").GetValue(0));
        Assert.AreEqual(25, result.Columns.Get("Age").GetValue(1));
    }

    [TestMethod]
    public void WhenFullPipelineThenWorks()
    {
        var record = CreateTestRecord();

        var result = record.AsQuery()
            .Where(r => r.Get<int>("Age") >= 25)
            .OrderBy("Age")
            .Select("Name", "Age")
            .Distinct("Age")
            .Take(2)
            .ToRecord();

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual(2, result.Columns.Count);
        Assert.AreEqual(25, result.Columns.Get("Age").GetValue(0));
        Assert.AreEqual(30, result.Columns.Get("Age").GetValue(1));
    }

    #endregion
}
