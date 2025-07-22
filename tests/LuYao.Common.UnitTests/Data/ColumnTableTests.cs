using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Data;

[TestClass]
public class ColumnTableTests
{
    [TestMethod]
    public void Constructor_ValidParameters_InitializesCorrectly()
    {
        // Arrange
        var dt = new ColumnTable();

        // Assert
        Assert.AreEqual(0, dt.Count);
        Assert.IsNotNull(dt.Columns);
        Assert.AreEqual(0, dt.Columns.Count);
    }

    [TestMethod]
    public void AddRowAndSetColumnValue_WorksCorrectly()
    {
        // Arrange
        var table = new ColumnTable();
        var colId = table.Columns.Add("Id", TypeCode.Int32);
        var colName = table.Columns.Add("Name", TypeCode.String);

        // Act
        var row = table.AddRow();
        colId.Set(1, row);
        colName.Set("Test", row);

        // Assert
        Assert.AreEqual(1, table.Count);
        Assert.AreEqual(2, table.Columns.Count);
        Assert.AreEqual(1, colId.Get(row));
        Assert.AreEqual("Test", colName.Get(row));
    }
}
