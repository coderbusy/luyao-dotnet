using System;
using System.Collections.Generic;
using System.Linq;

namespace LuYao.Data;

[TestClass]
public class RecordQueryJoinTests
{
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

    #region InnerJoin

    [TestMethod]
    public void WhenInnerJoinThenOnlyMatchingRows()
    {
        var employees = CreateEmployees();
        var departments = CreateDepartments();

        var result = employees.AsQuery()
            .InnerJoin(departments, "DeptId", "Id", "Dept_")
            .ToRecord();

        // Alice(10), Bob(20), Charlie(10) match; Diana(30) doesn't; Finance(40) doesn't
        Assert.AreEqual(3, result.Count);
        Assert.AreEqual("Alice", result.Columns.Get("Name").GetValue(0));
    }

    [TestMethod]
    public void WhenJoinIsAliasForInnerJoin()
    {
        var employees = CreateEmployees();
        var departments = CreateDepartments();

        var result = employees.AsQuery()
            .Join(departments, "DeptId", "Id", "Dept_")
            .ToRecord();

        Assert.AreEqual(3, result.Count);
    }

    [TestMethod]
    public void WhenInnerJoinThenResultHasAllColumns()
    {
        var employees = CreateEmployees();
        var departments = CreateDepartments();

        var result = employees.AsQuery()
            .InnerJoin(departments, "DeptId", "Id", "Dept_")
            .ToRecord();

        // 3 left cols + 2 right cols = 5
        Assert.AreEqual(5, result.Columns.Count);
        Assert.IsNotNull(result.Columns.Find("Id"));
        Assert.IsNotNull(result.Columns.Find("Name"));
        Assert.IsNotNull(result.Columns.Find("DeptId"));
        Assert.IsNotNull(result.Columns.Find("Dept_Id"));
        Assert.IsNotNull(result.Columns.Find("DeptName"));
    }

    [TestMethod]
    public void WhenInnerJoinNoMatchThenReturnsEmpty()
    {
        var left = new Record("Left", 1);
        left.Columns.Add<int>("Key");
        var row = left.AddRow();
        left.Columns.Get("Key").SetValue(999, row.Row);

        var right = new Record("Right", 1);
        right.Columns.Add<int>("Key");
        var rrow = right.AddRow();
        right.Columns.Get("Key").SetValue(1, rrow.Row);

        var result = left.AsQuery()
            .InnerJoin(right, "Key", "Key", "R_")
            .ToRecord();

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void WhenInnerJoinLeftEmptyThenReturnsEmpty()
    {
        var left = new Record("Left", 0);
        left.Columns.Add<int>("Key");

        var right = CreateDepartments();

        var result = left.AsQuery()
            .InnerJoin(right, "Key", "Id", "R_")
            .ToRecord();

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void WhenInnerJoinRightEmptyThenReturnsEmpty()
    {
        var employees = CreateEmployees();
        var right = new Record("Right", 0);
        right.Columns.Add<int>("Id");
        right.Columns.Add<string>("DeptName");

        var result = employees.AsQuery()
            .InnerJoin(right, "DeptId", "Id", "R_")
            .ToRecord();

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void WhenInnerJoinDuplicateColumnWithoutPrefixThenThrowsOnToRecord()
    {
        var employees = CreateEmployees();
        var departments = CreateDepartments();

        var query = employees.AsQuery().InnerJoin(departments, "DeptId", "Id");

        Assert.Throws<InvalidOperationException>(() => query.ToRecord());
    }

    [TestMethod]
    public void WhenInnerJoinNonExistentLeftKeyThenThrowsOnToRecord()
    {
        var employees = CreateEmployees();
        var departments = CreateDepartments();

        var query = employees.AsQuery().InnerJoin(departments, "NonExistent", "Id", "R_");

        Assert.Throws<InvalidOperationException>(() => query.ToRecord());
    }

    [TestMethod]
    public void WhenInnerJoinNonExistentRightKeyThenThrowsOnToRecord()
    {
        var employees = CreateEmployees();
        var departments = CreateDepartments();

        var query = employees.AsQuery().InnerJoin(departments, "DeptId", "NonExistent", "R_");

        Assert.Throws<InvalidOperationException>(() => query.ToRecord());
    }

    [TestMethod]
    public void WhenInnerJoinWithRecordQueryRightThenWorks()
    {
        var employees = CreateEmployees();
        var departments = CreateDepartments();

        var rightQuery = departments.AsQuery().Where(r => r.Get<int>("Id") == 10);

        var result = employees.AsQuery()
            .InnerJoin(rightQuery, "DeptId", "Id", "Dept_")
            .ToRecord();

        // Only DeptId=10 matches: Alice, Charlie
        Assert.AreEqual(2, result.Count);
    }

    #endregion

    #region LeftJoin

    [TestMethod]
    public void WhenLeftJoinThenPreservesAllLeftRows()
    {
        var employees = CreateEmployees();
        var departments = CreateDepartments();

        var result = employees.AsQuery()
            .LeftJoin(departments, "DeptId", "Id", "Dept_")
            .ToRecord();

        // All 4 employees, Diana(30) has no match but is preserved
        Assert.AreEqual(4, result.Count);
    }

    [TestMethod]
    public void WhenLeftJoinUnmatchedThenRightColumnsAreDefault()
    {
        var employees = CreateEmployees();
        var departments = CreateDepartments();

        var result = employees.AsQuery()
            .LeftJoin(departments, "DeptId", "Id", "Dept_")
            .ToRecord();

        // Diana is at index 3 (DeptId=30, no match)
        var deptNameCol = result.Columns.Get("DeptName");
        Assert.AreEqual("Diana", result.Columns.Get("Name").GetValue(3));
        Assert.IsNull(deptNameCol.GetValue(3));
    }

    [TestMethod]
    public void WhenLeftJoinRightEmptyThenAllLeftRowsPreserved()
    {
        var employees = CreateEmployees();
        var right = new Record("Right", 0);
        right.Columns.Add<int>("Id");
        right.Columns.Add<string>("DeptName");

        var result = employees.AsQuery()
            .LeftJoin(right, "DeptId", "Id", "R_")
            .ToRecord();

        Assert.AreEqual(4, result.Count);
    }

    [TestMethod]
    public void WhenLeftJoinLeftEmptyThenReturnsEmpty()
    {
        var left = new Record("Left", 0);
        left.Columns.Add<int>("DeptId");

        var departments = CreateDepartments();

        var result = left.AsQuery()
            .LeftJoin(departments, "DeptId", "Id", "R_")
            .ToRecord();

        Assert.AreEqual(0, result.Count);
    }

    #endregion

    #region RightJoin

    [TestMethod]
    public void WhenRightJoinThenPreservesAllRightRows()
    {
        var employees = CreateEmployees();
        var departments = CreateDepartments();

        var result = employees.AsQuery()
            .RightJoin(departments, "DeptId", "Id", "Dept_")
            .ToRecord();

        // Matched: Alice(10), Bob(20), Charlie(10) = 3 rows
        // Unmatched right: Finance(40) = 1 row
        // Diana(30) is NOT preserved (right join doesn't keep unmatched left)
        Assert.AreEqual(4, result.Count);
    }

    [TestMethod]
    public void WhenRightJoinUnmatchedThenLeftColumnsAreDefault()
    {
        var employees = CreateEmployees();
        var departments = CreateDepartments();

        var result = employees.AsQuery()
            .RightJoin(departments, "DeptId", "Id", "Dept_")
            .ToRecord();

        // The last row should be Finance(40) with null left columns
        var deptNameCol = result.Columns.Get("DeptName");
        var nameCol = result.Columns.Get("Name");

        // Find the Finance row
        bool foundFinance = false;
        for (int i = 0; i < result.Count; i++)
        {
            if ("Finance".Equals(deptNameCol.GetValue(i)))
            {
                Assert.IsNull(nameCol.GetValue(i));
                foundFinance = true;
                break;
            }
        }
        Assert.IsTrue(foundFinance);
    }

    [TestMethod]
    public void WhenRightJoinLeftEmptyThenAllRightRowsPreserved()
    {
        var left = new Record("Left", 0);
        left.Columns.Add<int>("DeptId");

        var departments = CreateDepartments();

        var result = left.AsQuery()
            .RightJoin(departments, "DeptId", "Id", "Dept_")
            .ToRecord();

        Assert.AreEqual(3, result.Count);
    }

    #endregion

    #region Null Arguments

    [TestMethod]
    public void WhenJoinNullRightThenThrows()
    {
        var record = CreateEmployees();
        Assert.Throws<ArgumentNullException>(() => record.AsQuery().Join((Record)null!, "DeptId", "Id"));
    }

    [TestMethod]
    public void WhenJoinNullLeftKeyThenThrows()
    {
        var record = CreateEmployees();
        Assert.Throws<ArgumentNullException>(() => record.AsQuery().Join(CreateDepartments(), null!, "Id"));
    }

    [TestMethod]
    public void WhenJoinNullRightKeyThenThrows()
    {
        var record = CreateEmployees();
        Assert.Throws<ArgumentNullException>(() => record.AsQuery().Join(CreateDepartments(), "DeptId", null!));
    }

    #endregion
}
