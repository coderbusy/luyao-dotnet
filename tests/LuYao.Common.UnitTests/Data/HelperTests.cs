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
    public void MakeFrameColumn_ShouldCreateFrameColumnOfType()
    {
        // Arrange
        var record = new Frame();
        string name = "TestColumn";
        Type type = typeof(int);
        // Act
        var column = Helpers.MakeFrameColumn(record, name, type);
        // Assert
        Assert.IsNotNull(column);
        Assert.AreEqual(name, column.Name);
        Assert.AreEqual(type, column.Type);
        Assert.IsInstanceOfType(column, typeof(FrameColumn<int>));
    }

    [TestMethod]
    public void MakeFrameColumn_UnsupportedType_ShouldThrow()
    {
        var record = new Frame();
        Assert.Throws<NotSupportedException>(() => Helpers.MakeFrameColumn(record, "Test", typeof(Foo)));
    }
}
