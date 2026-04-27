using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Data;

[TestClass]
public class HelperTests
{
    public class Foo
    {

    }

    [TestMethod]
    public void MakeRecordColumn_ShouldCreateRecordColumnOfType()
    {
        // Arrange
        var record = new Record();
        string name = "TestColumn";
        Type type = typeof(int);
        // Act
        var column = Helpers.MakeRecordColumn(record, name, type);
        // Assert
        Assert.IsNotNull(column);
        Assert.AreEqual(name, column.Name);
        Assert.AreEqual(type, column.Type);
        Assert.IsInstanceOfType(column, typeof(RecordColumn<int>));
    }

    [TestMethod]
    public void MakeRecordColumn_UnsupportedType_ShouldThrow()
    {
        var record = new Record();
        Assert.Throws<NotSupportedException>(() => Helpers.MakeRecordColumn(record, "Test", typeof(Foo)));
    }
}
