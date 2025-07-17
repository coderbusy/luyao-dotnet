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
        int dimension = 1;
        int capacity = 10;

        // Act
        var column = new Column(name, type, dimension, capacity, 0);

        // Assert
        Assert.AreEqual(name, column.Name);
        Assert.AreEqual(type, column.Type);
        Assert.AreEqual(dimension, column.Dimension);
        Assert.AreEqual(typeof(int[]), column.DataType);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Constructor_NullName_ThrowsArgumentNullException()
    {
        // Arrange
        string name = null;
        TypeCode type = TypeCode.Int32;

        // Act
        var column = new Column(name, type, 0, 60, 0);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Constructor_NegativeDimension_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        string name = "TestColumn";
        TypeCode type = TypeCode.Int32;
        int dimension = -1;

        // Act
        var column = new Column(name, type, dimension, 60, 0);
    }

}