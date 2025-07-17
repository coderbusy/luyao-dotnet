namespace LuYao.Data;

[TestClass]
public class ColumnTests
{
    [TestMethod]
    public void Constructor_ValidParameters_InitializesCorrectly()
    {
        // Arrange
        string name = "TestColumn";
        DataType type = DataType.Int32;
        int dimension = 1;
        int capacity = 10;

        // Act
        var column = new Column(name, type, dimension, capacity);

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
        DataType type = DataType.Int32;

        // Act
        var column = new Column(name, type);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Constructor_NegativeDimension_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        string name = "TestColumn";
        DataType type = DataType.Int32;
        int dimension = -1;

        // Act
        var column = new Column(name, type, dimension);
    }

    [TestMethod]
    public void Add_ValidValue_AddsValueToColumn()
    {
        // Arrange
        var column = new Column("TestColumn", DataType.Int32);
        int value = 42;

        // Act
        column.Add(value);

        // Assert
        Assert.AreEqual(0, column.Dimension); // Check dimension remains unchanged
    }

    [TestMethod]
    [ExpectedException(typeof(FormatException))]
    public void Add_InvalidValueType_ThrowsArgumentException()
    {
        // Arrange
        var column = new Column("TestColumn", DataType.Int32);
        string invalidValue = "Invalid";

        // Act
        column.Add(invalidValue);
    }

    [TestMethod]
    public void Add_ConvertibleValue_AddsValueToColumn()
    {
        // Arrange
        var column = new Column("TestColumn", DataType.Int32);
        string value = "42";

        // Act
        column.Add(value);

        // Assert
        Assert.AreEqual(42, column.Data.GetValue(0)); // Check the value is converted and added
    }

    [TestMethod]
    public void Clear_ColumnWithData_ClearsAllData()
    {
        // Arrange
        var column = new Column("TestColumn", DataType.Int32);
        column.Add(42);

        // Act
        column.Clear();

        // Assert
        Assert.AreEqual(0, column.Dimension); // Check dimension is reset
    }
}