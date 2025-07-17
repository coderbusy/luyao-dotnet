namespace LuYao.Data;

[TestClass]
public class ColumnTests
{
    [TestMethod]
    public void Constructor_ValidParameters_InitializesCorrectly()
    {
        // Arrange
        string name = "TestColumn";
        TypeCode type = TypeCode.Int32;
        bool isArray = true;
        int capacity = 10;

        // Act
        var column = new Column(name, type, isArray, capacity, 0);

        // Assert
        Assert.AreEqual(name, column.Name);
        Assert.AreEqual(type, column.Code);
        Assert.AreEqual(isArray, column.IsArray);
        Assert.AreEqual(typeof(int[]), column.Type);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Constructor_NullName_ThrowsArgumentNullException()
    {
        // Arrange
        string name = null;
        TypeCode type = TypeCode.Int32;

        // Act
        var column = new Column(name, type, false, 60, 0);
    }
}