using System;
using System.Linq;

namespace LuYao.Data;

[TestClass]
public class RecordQueryFullOuterJoinTests
{
    /// <summary>
    /// 左表：Employees(Id, Name, DeptId)
    /// DeptId=10 → 有匹配；DeptId=30 → 仅左侧有
    /// </summary>
    private static Record CreateEmployees()
    {
        var record = new Record("Employees", 4);
        var idCol = record.Columns.Add<int>("Id");
        var nameCol = record.Columns.Add<string>("Name");
        var deptIdCol = record.Columns.Add<int>("DeptId");

        var data = new[] { (1, "Alice", 10), (2, "Bob", 20), (3, "Charlie", 10), (4, "Diana", 30) };
        foreach (var (id, name, deptId) in data)
        {
            var row = record.AddRow();
            idCol.Set(id, row.Row);
            nameCol.Set(name, row.Row);
            deptIdCol.Set(deptId, row.Row);
        }
        return record;
    }

    /// <summary>
    /// 右表：Departments(Id, DeptName)
    /// Id=10,20 → 有匹配；Id=40 → 仅右侧有
    /// </summary>
    private static Record CreateDepartments()
    {
        var record = new Record("Departments", 3);
        var idCol = record.Columns.Add<int>("Id");
        var nameCol = record.Columns.Add<string>("DeptName");

        var data = new[] { (10, "Engineering"), (20, "Marketing"), (40, "Finance") };
        foreach (var (id, name) in data)
        {
            var row = record.AddRow();
            idCol.Set(id, row.Row);
            nameCol.Set(name, row.Row);
        }
        return record;
    }

    #region 行数验证

    [TestMethod]
    public void WhenFullOuterJoinThenRowCountIsCorrect()
    {
        // 匹配行: Alice(10)×Eng, Bob(20)×Mkt, Charlie(10)×Eng = 3
        // 仅左侧: Diana(30) = 1
        // 仅右侧: Finance(40) = 1
        // 总计 = 5
        var result = CreateEmployees().AsQuery()
            .FullOuterJoin(CreateDepartments(), "DeptId", "Id", "Dept_")
            .ToRecord();

        Assert.AreEqual(5, result.Count);
    }

    [TestMethod]
    public void WhenFullOuterJoinLeftEmptyThenReturnsAllRightRows()
    {
        var left = new Record("Left", 0);
        left.Columns.Add<int>("DeptId");

        var result = left.AsQuery()
            .FullOuterJoin(CreateDepartments(), "DeptId", "Id", "R_")
            .ToRecord();

        Assert.AreEqual(3, result.Count);
    }

    [TestMethod]
    public void WhenFullOuterJoinRightEmptyThenReturnsAllLeftRows()
    {
        var right = new Record("Right", 0);
        right.Columns.Add<int>("Id");
        right.Columns.Add<string>("DeptName");

        var result = CreateEmployees().AsQuery()
            .FullOuterJoin(right, "DeptId", "Id", "R_")
            .ToRecord();

        Assert.AreEqual(4, result.Count);
    }

    [TestMethod]
    public void WhenFullOuterJoinBothEmptyThenReturnsEmpty()
    {
        var left = new Record("L", 0);
        left.Columns.Add<int>("Key");

        var right = new Record("R", 0);
        right.Columns.Add<int>("Key");
        right.Columns.Add<string>("Val");

        var result = left.AsQuery()
            .FullOuterJoin(right, "Key", "Key", "R_")
            .ToRecord();

        Assert.AreEqual(0, result.Count);
    }

    #endregion

    #region 列内容验证

    [TestMethod]
    public void WhenFullOuterJoinLeftOnlyRowThenRightColumnsAreNull()
    {
        var result = CreateEmployees().AsQuery()
            .FullOuterJoin(CreateDepartments(), "DeptId", "Id", "Dept_")
            .ToRecord();

        var nameCol = result.Columns.Get("Name");
        var deptNameCol = result.Columns.Get("DeptName");

        bool found = false;
        for (int i = 0; i < result.Count; i++)
        {
            if ("Diana".Equals(nameCol.GetValue(i)))
            {
                Assert.IsNull(deptNameCol.GetValue(i), "Diana 的 DeptName 应为 null");
                found = true;
                break;
            }
        }
        Assert.IsTrue(found, "应能找到 Diana 行");
    }

    [TestMethod]
    public void WhenFullOuterJoinRightOnlyRowThenLeftColumnsAreNull()
    {
        var result = CreateEmployees().AsQuery()
            .FullOuterJoin(CreateDepartments(), "DeptId", "Id", "Dept_")
            .ToRecord();

        var nameCol = result.Columns.Get("Name");
        var deptNameCol = result.Columns.Get("DeptName");

        bool found = false;
        for (int i = 0; i < result.Count; i++)
        {
            if ("Finance".Equals(deptNameCol.GetValue(i)))
            {
                Assert.IsNull(nameCol.GetValue(i), "Finance 行的 Name 应为 null");
                found = true;
                break;
            }
        }
        Assert.IsTrue(found, "应能找到 Finance 行");
    }

    [TestMethod]
    public void WhenFullOuterJoinMatchedRowThenBothSidesHaveValues()
    {
        var result = CreateEmployees().AsQuery()
            .FullOuterJoin(CreateDepartments(), "DeptId", "Id", "Dept_")
            .ToRecord();

        var nameCol = result.Columns.Get("Name");
        var deptNameCol = result.Columns.Get("DeptName");

        int matchedCount = 0;
        for (int i = 0; i < result.Count; i++)
        {
            if (nameCol.GetValue(i) != null && deptNameCol.GetValue(i) != null)
                matchedCount++;
        }
        Assert.AreEqual(3, matchedCount); // Alice, Bob, Charlie
    }

    #endregion

    #region Schema 验证

    [TestMethod]
    public void WhenFullOuterJoinThenResultHasAllColumns()
    {
        var result = CreateEmployees().AsQuery()
            .FullOuterJoin(CreateDepartments(), "DeptId", "Id", "Dept_")
            .ToRecord();

        // 3 left + 2 right = 5
        Assert.AreEqual(5, result.Columns.Count);
        Assert.IsNotNull(result.Columns.Find("Id"));
        Assert.IsNotNull(result.Columns.Find("Name"));
        Assert.IsNotNull(result.Columns.Find("DeptId"));
        Assert.IsNotNull(result.Columns.Find("Dept_Id"));
        Assert.IsNotNull(result.Columns.Find("DeptName"));
    }

    [TestMethod]
    public void WhenFullOuterJoinDuplicateColumnWithoutPrefixThenThrows()
    {
        var query = CreateEmployees().AsQuery()
            .FullOuterJoin(CreateDepartments(), "DeptId", "Id");

        Assert.Throws<InvalidOperationException>(() => query.ToRecord());
    }

    #endregion

    #region RecordQuery 右表

    [TestMethod]
    public void WhenFullOuterJoinWithRecordQueryRightThenWorks()
    {
        var result = CreateEmployees().AsQuery()
            .FullOuterJoin(
                CreateDepartments().AsQuery().Where(r => r.Get<int>("Id") != 40),
                "DeptId", "Id", "Dept_")
            .ToRecord();

        // 匹配: Alice(10), Bob(20), Charlie(10) = 3
        // 仅左侧: Diana(30) = 1
        // 仅右侧: Finance(40) 已被过滤掉
        Assert.AreEqual(4, result.Count);
    }

    #endregion

    #region 参数校验

    [TestMethod]
    public void WhenFullOuterJoinNullRightThenThrows()
    {
        Assert.Throws<ArgumentNullException>(() =>
            CreateEmployees().AsQuery().FullOuterJoin((Record)null!, "DeptId", "Id"));
    }

    [TestMethod]
    public void WhenFullOuterJoinNullLeftKeyThenThrows()
    {
        Assert.Throws<ArgumentNullException>(() =>
            CreateEmployees().AsQuery().FullOuterJoin(CreateDepartments(), null!, "Id"));
    }

    [TestMethod]
    public void WhenFullOuterJoinNullRightKeyThenThrows()
    {
        Assert.Throws<ArgumentNullException>(() =>
            CreateEmployees().AsQuery().FullOuterJoin(CreateDepartments(), "DeptId", null!));
    }

    [TestMethod]
    public void WhenFullOuterJoinNonExistentLeftKeyThenThrowsOnToRecord()
    {
        var query = CreateEmployees().AsQuery()
            .FullOuterJoin(CreateDepartments(), "NonExistent", "Id", "R_");

        Assert.Throws<InvalidOperationException>(() => query.ToRecord());
    }

    [TestMethod]
    public void WhenFullOuterJoinNonExistentRightKeyThenThrowsOnToRecord()
    {
        var query = CreateEmployees().AsQuery()
            .FullOuterJoin(CreateDepartments(), "DeptId", "NonExistent", "R_");

        Assert.Throws<InvalidOperationException>(() => query.ToRecord());
    }

    #endregion
}

[TestClass]
public class RecordQueryCrossJoinTests
{
    private static Record CreateColors()
    {
        var record = new Record("Colors", 2);
        var col = record.Columns.Add<string>("Color");
        record.AddRow(); col.Set("Red", 0);
        record.AddRow(); col.Set("Blue", 1);
        return record;
    }

    private static Record CreateSizes()
    {
        var record = new Record("Sizes", 3);
        var col = record.Columns.Add<string>("Size");
        record.AddRow(); col.Set("S", 0);
        record.AddRow(); col.Set("M", 1);
        record.AddRow(); col.Set("L", 2);
        return record;
    }

    #region 行数验证

    [TestMethod]
    public void WhenCrossJoinThenRowCountIsProduct()
    {
        var result = CreateColors().AsQuery()
            .CrossJoin(CreateSizes())
            .ToRecord();

        Assert.AreEqual(6, result.Count); // 2 × 3
    }

    [TestMethod]
    public void WhenCrossJoinLeftEmptyThenReturnsEmpty()
    {
        var left = new Record("L", 0);
        left.Columns.Add<string>("Color");

        var result = left.AsQuery()
            .CrossJoin(CreateSizes())
            .ToRecord();

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void WhenCrossJoinRightEmptyThenReturnsEmpty()
    {
        var right = new Record("R", 0);
        right.Columns.Add<string>("Size");

        var result = CreateColors().AsQuery()
            .CrossJoin(right)
            .ToRecord();

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void WhenCrossJoinSingleRowByMultipleRowsThenCountEquals()
    {
        var left = new Record("L", 1);
        var lCol = left.Columns.Add<string>("Color");
        left.AddRow(); lCol.Set("Red", 0);

        var result = left.AsQuery()
            .CrossJoin(CreateSizes())
            .ToRecord();

        Assert.AreEqual(3, result.Count); // 1 × 3
    }

    #endregion

    #region 列内容验证

    [TestMethod]
    public void WhenCrossJoinThenAllCombinationsPresent()
    {
        var result = CreateColors().AsQuery()
            .CrossJoin(CreateSizes())
            .ToRecord();

        var colorCol = result.Columns.Get("Color");
        var sizeCol = result.Columns.Get("Size");

        var combinations = Enumerable.Range(0, result.Count)
            .Select(i => $"{colorCol.GetValue(i)}-{sizeCol.GetValue(i)}")
            .OrderBy(x => x)
            .ToArray();

        var expected = new[] { "Blue-L", "Blue-M", "Blue-S", "Red-L", "Red-M", "Red-S" };
        CollectionAssert.AreEqual(expected, combinations);
    }

    [TestMethod]
    public void WhenCrossJoinThenLeftValuesRepeatedPerRightCount()
    {
        var result = CreateColors().AsQuery()
            .CrossJoin(CreateSizes())
            .ToRecord();

        var colorCol = result.Columns.Get("Color");

        // Red 应出现 3 次（对应 S/M/L）
        int redCount = Enumerable.Range(0, result.Count)
            .Count(i => "Red".Equals(colorCol.GetValue(i)));
        Assert.AreEqual(3, redCount);
    }

    #endregion

    #region Schema 验证

    [TestMethod]
    public void WhenCrossJoinThenResultHasBothTablesColumns()
    {
        var result = CreateColors().AsQuery()
            .CrossJoin(CreateSizes())
            .ToRecord();

        Assert.AreEqual(2, result.Columns.Count);
        Assert.IsNotNull(result.Columns.Find("Color"));
        Assert.IsNotNull(result.Columns.Find("Size"));
    }

    [TestMethod]
    public void WhenCrossJoinDuplicateColumnWithPrefixThenDistinguishes()
    {
        // 两表都有 "Id" 列时必须提供前缀
        var left = new Record("L", 1);
        var lId = left.Columns.Add<int>("Id");
        left.AddRow(); lId.Set(1, 0);

        var right = new Record("R", 1);
        var rId = right.Columns.Add<int>("Id");
        right.AddRow(); rId.Set(100, 0);

        var result = left.AsQuery()
            .CrossJoin(right, "R_")
            .ToRecord();

        Assert.AreEqual(2, result.Columns.Count);
        Assert.IsNotNull(result.Columns.Find("Id"));
        Assert.IsNotNull(result.Columns.Find("R_Id"));
    }

    [TestMethod]
    public void WhenCrossJoinDuplicateColumnWithoutPrefixThenThrows()
    {
        var left = new Record("L", 1);
        left.Columns.Add<int>("Id");
        left.AddRow(); left.Columns[0].SetValue(1, 0);

        var right = new Record("R", 1);
        right.Columns.Add<int>("Id");
        right.AddRow(); right.Columns[0].SetValue(100, 0);

        var query = left.AsQuery().CrossJoin(right);

        Assert.Throws<InvalidOperationException>(() => query.ToRecord());
    }

    #endregion

    #region RecordQuery 右表

    [TestMethod]
    public void WhenCrossJoinWithRecordQueryRightThenWorks()
    {
        var result = CreateColors().AsQuery()
            .CrossJoin(CreateSizes().AsQuery().Where(r => r.Get<string>("Size") != "L"))
            .ToRecord();

        Assert.AreEqual(4, result.Count); // 2 colors × 2 sizes (S, M)
    }

    #endregion

    #region 参数校验

    [TestMethod]
    public void WhenCrossJoinNullRightThenThrows()
    {
        Assert.Throws<ArgumentNullException>(() =>
            CreateColors().AsQuery().CrossJoin((Record)null!));
    }

    [TestMethod]
    public void WhenCrossJoinNullRightQueryThenThrows()
    {
        Assert.Throws<ArgumentNullException>(() =>
            CreateColors().AsQuery().CrossJoin((RecordQuery)null!));
    }

    #endregion
}
