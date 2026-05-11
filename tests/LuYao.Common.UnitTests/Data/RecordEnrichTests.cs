using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LuYao.Data;

[TestClass]
public class RecordEnrichTests
{
    // 构建员工 Record：Id, Name
    private static Record BuildEmployees()
    {
        var r = new Record();
        var colId = r.Columns.Add<int>("Id");
        var colName = r.Columns.Add<string>("Name");
        void Add(int id, string name)
        {
            var row = r.AddRow();
            colId.Set(row, id);
            colName.Set(row, name);
        }
        Add(1, "Alice");
        Add(2, "Bob");
        Add(3, "Charlie");
        return r;
    }

    // 构建部门 Record：DeptId, DeptName, Location
    private static Record BuildDepartments()
    {
        var r = new Record();
        var colId = r.Columns.Add<int>("DeptId");
        var colName = r.Columns.Add<string>("DeptName");
        var colLoc = r.Columns.Add<string>("Location");
        void Add(int id, string name, string loc)
        {
            var row = r.AddRow();
            colId.Set(row, id);
            colName.Set(row, name);
            colLoc.Set(row, loc);
        }
        Add(1, "Engineering", "Beijing");
        Add(2, "Marketing", "Shanghai");
        return r;
    }

    // 构建带 DeptId 的员工 Record：Id, Name, DeptId
    private static Record BuildEmployeesWithDept()
    {
        var r = new Record();
        var colId = r.Columns.Add<int>("Id");
        var colName = r.Columns.Add<string>("Name");
        var colDept = r.Columns.Add<int>("DeptId");
        void Add(int id, string name, int deptId)
        {
            var row = r.AddRow();
            colId.Set(row, id);
            colName.Set(row, name);
            colDept.Set(row, deptId);
        }
        Add(1, "Alice", 1);
        Add(2, "Bob", 2);
        Add(3, "Charlie", 99); // 无匹配
        return r;
    }

    [TestMethod]
    public void Enrich_SharedColumn_AddsNewColumnsFromSource()
    {
        // Arrange：两个 Record 共享 DeptId 列
        var left = new Record();
        var colDeptId = left.Columns.Add<int>("DeptId");
        var row1 = left.AddRow(); colDeptId.Set(row1, 1);
        var row2 = left.AddRow(); colDeptId.Set(row2, 2);

        var right = BuildDepartments(); // DeptId, DeptName, Location

        // Act
        left.Enrich(right, "DeptId");

        // Assert：DeptName 和 Location 列被追加
        Assert.IsNotNull(left.Columns.Find("DeptName"));
        Assert.IsNotNull(left.Columns.Find("Location"));
        Assert.AreEqual("Engineering", left.Columns.Get("DeptName").To<string>(0));
        Assert.AreEqual("Marketing", left.Columns.Get("DeptName").To<string>(1));
        Assert.AreEqual("Beijing", left.Columns.Get("Location").To<string>(0));
    }

    [TestMethod]
    public void Enrich_DifferentColumnNames_MatchesCorrectly()
    {
        // Arrange
        var employees = BuildEmployeesWithDept(); // Id, Name, DeptId
        var departments = BuildDepartments();     // DeptId, DeptName, Location

        // Act：employees.DeptId 对应 departments.DeptId
        employees.Enrich(departments, "DeptId", "DeptId");

        // Assert
        var deptNameCol = employees.Columns.Find("DeptName");
        Assert.IsNotNull(deptNameCol);
        Assert.AreEqual("Engineering", deptNameCol.To<string>(0)); // Alice -> dept 1
        Assert.AreEqual("Marketing", deptNameCol.To<string>(1));   // Bob   -> dept 2
    }

    [TestMethod]
    public void Enrich_WithColumnsFilter_OnlyAddsSpecifiedColumns()
    {
        // Arrange
        var employees = BuildEmployeesWithDept();
        var departments = BuildDepartments();

        // Act：只补充 DeptName，不补充 Location
        employees.Enrich(departments, "DeptId", "DeptId", new[] { "DeptName" });

        // Assert
        Assert.IsNotNull(employees.Columns.Find("DeptName"));
        Assert.IsNull(employees.Columns.Find("Location"));
    }

    [TestMethod]
    public void Enrich_NoMatchingRow_RowRemainsUnchanged()
    {
        // Arrange
        var employees = BuildEmployeesWithDept(); // Charlie -> DeptId 99，无匹配
        var departments = BuildDepartments();

        // Act
        employees.Enrich(departments, "DeptId", "DeptId");

        // Assert：Charlie 行的 DeptName 应为 null
        var deptNameCol = employees.Columns.Find("DeptName");
        Assert.IsNotNull(deptNameCol);
        Assert.IsNull(deptNameCol.To<string>(2)); // Charlie
    }

    [TestMethod]
    public void Enrich_NullSource_ThrowsArgumentNullException()
    {
        var r = new Record();
        Assert.Throws<ArgumentNullException>(() => r.Enrich(null!, "Id"));
    }

    [TestMethod]
    public void Enrich_EmptySharedColumn_ThrowsArgumentException()
    {
        var r = new Record();
        Assert.Throws<ArgumentException>(() => r.Enrich(new Record(), ""));
    }

    [TestMethod]
    public void Enrich_SelfColumnNotFound_ThrowsArgumentException()
    {
        var r = new Record();
        r.Columns.Add<int>("Id");
        var source = new Record();
        source.Columns.Add<int>("OtherId");

        Assert.Throws<ArgumentException>(() => r.Enrich(source, "NoSuchCol", "OtherId"));
    }

    [TestMethod]
    public void Enrich_SourceColumnNotFound_ThrowsArgumentException()
    {
        var r = new Record();
        r.Columns.Add<int>("Id");
        var source = new Record();
        source.Columns.Add<int>("OtherId");

        Assert.Throws<ArgumentException>(() => r.Enrich(source, "Id", "NoSuchCol"));
    }

    [TestMethod]
    public void Enrich_ColumnsFilterWithNoMatch_AddsNoColumns()
    {
        // Arrange
        var employees = BuildEmployeesWithDept();
        var departments = BuildDepartments();

        // Act：过滤列名在 source 中不存在
        employees.Enrich(departments, "DeptId", "DeptId", new[] { "NonExistentCol" });

        // Assert：不会追加任何新列（除了已有的 Id、Name、DeptId）
        Assert.AreEqual(3, employees.Columns.Count);
    }
}
