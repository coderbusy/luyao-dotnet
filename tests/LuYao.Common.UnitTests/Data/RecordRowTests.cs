using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LuYao.Data;

/// <summary>
/// RecordRow 结构体的单元测试类
/// </summary>
[TestClass]
public class RecordRowTests
{
    /// <summary>
    /// 创建测试用的 Record 和相关数据
    /// </summary>
    private (Record record, RecordColumn<int> intColumn, RecordColumn<string> stringColumn, RecordColumn<bool> boolColumn) CreateTestRecord()
    {
        var record = new Record("TestTable", 5);
        var intColumn = record.Columns.Add<int>("IntColumn");
        var stringColumn = record.Columns.Add<string>("StringColumn");
        var boolColumn = record.Columns.Add<bool>("BoolColumn");

        // 添加测试数据
        var row1 = record.AddRow();
        var row2 = record.AddRow();

        intColumn.Set(100, 0);
        intColumn.Set(200, 1);
        stringColumn.Set("Test1", 0);
        stringColumn.Set("Test2", 1);
        boolColumn.Set(true, 0);
        boolColumn.Set(false, 1);

        return (record, intColumn, stringColumn, boolColumn);
    }

    #region 构造函数测试

    /// <summary>
    /// 测试构造函数 - 正常参数
    /// </summary>
    [TestMethod]
    public void Constructor_ValidParameters_ShouldInitializeCorrectly()
    {
        // Arrange
        var (record, _, _, _) = CreateTestRecord();

        // Act
        var recordRow = new RecordRow(record, 0);

        // Assert
        Assert.AreEqual(record, recordRow.Record);
        Assert.AreEqual(0, recordRow.Row);
    }

    /// <summary>
    /// 测试构造函数 - Record 为 null
    /// </summary>
    [TestMethod]
    public void Constructor_NullRecord_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new RecordRow(null!, 0));
    }

    /// <summary>
    /// 测试构造函数 - 行索引小于0
    /// </summary>
    [TestMethod]
    public void Constructor_NegativeRowIndex_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var (record, _, _, _) = CreateTestRecord();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new RecordRow(record, -1));
    }

    /// <summary>
    /// 测试构造函数 - 行索引等于或大于 Record.Count
    /// </summary>
    [TestMethod]
    public void Constructor_RowIndexOutOfRange_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var (record, _, _, _) = CreateTestRecord();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new RecordRow(record, record.Count));
        Assert.Throws<ArgumentOutOfRangeException>(() => new RecordRow(record, record.Count + 1));
    }

    #endregion

    #region 属性测试

    /// <summary>
    /// 测试 Record 属性
    /// </summary>
    [TestMethod]
    public void Record_Property_ShouldReturnCorrectRecord()
    {
        // Arrange
        var (record, _, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(record, 0);

        // Act
        var result = recordRow.Record;

        // Assert
        Assert.AreEqual(record, result);
    }

    /// <summary>
    /// 测试 Row 属性
    /// </summary>
    [TestMethod]
    public void Row_Property_ShouldReturnCorrectRowIndex()
    {
        // Arrange
        var (record, _, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(record, 1);

        // Act
        var result = recordRow.Row;

        // Assert
        Assert.AreEqual(1, result);
    }

    #endregion

    #region 隐式转换测试

    /// <summary>
    /// 测试隐式转换到 int
    /// </summary>
    [TestMethod]
    public void ImplicitConversion_ToInt_ShouldReturnRowIndex()
    {
        // Arrange
        var (record, _, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(record, 1);

        // Act
        int rowIndex = recordRow;

        // Assert
        Assert.AreEqual(1, rowIndex);
    }

    #endregion

    #region GetBoolean 测试

    /// <summary>
    /// 测试 GetBoolean(string name) - 列存在
    /// </summary>
    [TestMethod]
    public void GetBoolean_ByName_ColumnExists_ShouldReturnCorrectValue()
    {
        // Arrange
        var (record, _, _, boolColumn) = CreateTestRecord();
        var recordRow = new RecordRow(record, 0);

        // Act
        var result = recordRow.Get<Boolean>("BoolColumn");

        // Assert
        Assert.AreEqual(true, result);
    }

    /// <summary>
    /// 测试 GetBoolean(string name) - 列不存在
    /// </summary>
    [TestMethod]
    public void GetBoolean_ByName_ColumnNotExists_ShouldReturnDefault()
    {
        // Arrange
        var (record, _, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(record, 0);

        // Act
        var result = recordRow.Get<Boolean>("NonExistentColumn");

        // Assert
        Assert.AreEqual(default(bool), result);
    }

    /// <summary>
    /// 测试 GetBoolean(RecordColumn col) - 列属于当前记录
    /// </summary>
    [TestMethod]
    public void GetBoolean_ByColumn_SameRecord_ShouldReturnCorrectValue()
    {
        // Arrange
        var (record, _, _, boolColumn) = CreateTestRecord();
        var recordRow = new RecordRow(record, 1);

        // Act
        var result = recordRow.Get<Boolean>(boolColumn);

        // Assert
        Assert.AreEqual(false, result);
    }

    /// <summary>
    /// 测试 GetBoolean(RecordColumn col) - 列属于不同记录
    /// </summary>
    [TestMethod]
    public void GetBoolean_ByColumn_DifferentRecord_ShouldFallbackToNameSearch()
    {
        // Arrange
        var (record1, _, _, _) = CreateTestRecord();
        var (record2, _, _, boolColumn2) = CreateTestRecord();
        var recordRow = new RecordRow(record1, 0);

        // Act
        var result = recordRow.Get<Boolean>(boolColumn2);

        // Assert
        Assert.AreEqual(true, result); // 应该通过列名查找到 record1 中的 BoolColumn
    }

    #endregion

    #region GetString 测试

    /// <summary>
    /// 测试 GetString(string name) - 列存在
    /// </summary>
    [TestMethod]
    public void GetString_ByName_ColumnExists_ShouldReturnCorrectValue()
    {
        // Arrange
        var (record, _, stringColumn, _) = CreateTestRecord();
        var recordRow = new RecordRow(record, 1);

        // Act
        var result = recordRow.Get<String>("StringColumn");

        // Assert
        Assert.AreEqual("Test2", result);
    }

    /// <summary>
    /// 测试 GetString(string name) - 列不存在
    /// </summary>
    [TestMethod]
    public void GetString_ByName_ColumnNotExists_ShouldReturnDefault()
    {
        // Arrange
        var (record, _, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(record, 0);

        // Act
        var result = recordRow.Get<String>("NonExistentColumn");

        // Assert
        Assert.AreEqual(default(string), result);
    }

    /// <summary>
    /// 测试 GetString(RecordColumn col) - 列属于当前记录
    /// </summary>
    [TestMethod]
    public void GetString_ByColumn_SameRecord_ShouldReturnCorrectValue()
    {
        // Arrange
        var (record, _, stringColumn, _) = CreateTestRecord();
        var recordRow = new RecordRow(record, 0);

        // Act
        var result = recordRow.Get<String>(stringColumn);

        // Assert
        Assert.AreEqual("Test1", result);
    }

    #endregion

    #region GetInt32 测试

    /// <summary>
    /// 测试 GetInt32(string name) - 列存在
    /// </summary>
    [TestMethod]
    public void GetInt32_ByName_ColumnExists_ShouldReturnCorrectValue()
    {
        // Arrange
        var (record, intColumn, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(record, 1);

        // Act
        var result = recordRow.Get<Int32>("IntColumn");

        // Assert
        Assert.AreEqual(200, result);
    }

    /// <summary>
    /// 测试 GetInt32(string name) - 列不存在
    /// </summary>
    [TestMethod]
    public void GetInt32_ByName_ColumnNotExists_ShouldReturnDefault()
    {
        // Arrange
        var (record, _, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(record, 0);

        // Act
        var result = recordRow.Get<Int32>("NonExistentColumn");

        // Assert
        Assert.AreEqual(default(int), result);
    }

    /// <summary>
    /// 测试 GetInt32(RecordColumn col) - 列属于当前记录
    /// </summary>
    [TestMethod]
    public void GetInt32_ByColumn_SameRecord_ShouldReturnCorrectValue()
    {
        // Arrange
        var (record, intColumn, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(record, 0);

        // Act
        var result = recordRow.Get<Int32>(intColumn);

        // Assert
        Assert.AreEqual(100, result);
    }

    #endregion

    #region 泛型 Get<T> 测试

    /// <summary>
    /// 测试 Get<T>(string name) - 列存在
    /// </summary>
    [TestMethod]
    public void GetGeneric_ByName_ColumnExists_ShouldReturnCorrectValue()
    {
        // Arrange
        var (record, intColumn, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(record, 0);

        // Act
        var result = recordRow.Get<int>("IntColumn");

        // Assert
        Assert.AreEqual(100, result);
    }

    /// <summary>
    /// 测试 Get<T>(string name) - 列不存在
    /// </summary>
    [TestMethod]
    public void GetGeneric_ByName_ColumnNotExists_ShouldReturnDefault()
    {
        // Arrange
        var (record, _, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(record, 0);

        // Act
        var result = recordRow.Get<int>("NonExistentColumn");

        // Assert
        Assert.AreEqual(default(int), result);
    }

    /// <summary>
    /// 测试 Get<T>(RecordColumn col) - 列属于当前记录
    /// </summary>
    [TestMethod]
    public void GetGeneric_ByColumn_SameRecord_ShouldReturnCorrectValue()
    {
        // Arrange
        var (record, intColumn, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(record, 1);

        // Act
        var result = recordRow.Get<int>(intColumn);

        // Assert
        Assert.AreEqual(200, result);
    }


    #endregion

    #region 数值类型测试 (抽样测试)

    /// <summary>
    /// 测试 GetByte 方法
    /// </summary>
    [TestMethod]
    public void GetByte_ByName_ShouldWork()
    {
        // Arrange
        var record = new Record("TestTable", 1);
        var byteColumn = record.Columns.Add<byte>("ByteColumn");
        var row = record.AddRow();
        byteColumn.Set(255);
        var recordRow = new RecordRow(record, 0);

        // Act
        var result = recordRow.Get<Byte>("ByteColumn");

        // Assert
        Assert.AreEqual((byte)255, result);
    }

    /// <summary>
    /// 测试 GetDouble 方法
    /// </summary>
    [TestMethod]
    public void GetDouble_ByName_ShouldWork()
    {
        // Arrange
        var record = new Record("TestTable", 1);
        var doubleColumn = record.Columns.Add<double>("DoubleColumn");
        var row = record.AddRow();
        doubleColumn.Set(3.14159);
        var recordRow = new RecordRow(record, 0);

        // Act
        var result = recordRow.Get<Double>("DoubleColumn");

        // Assert
        Assert.AreEqual(3.14159, result, 0.00001);
    }

    /// <summary>
    /// 测试 GetDateTime 方法
    /// </summary>
    [TestMethod]
    public void GetDateTime_ByName_ShouldWork()
    {
        // Arrange
        var record = new Record("TestTable", 1);
        var dateTimeColumn = record.Columns.Add<DateTime>("DateTimeColumn");
        var testDate = new DateTime(2023, 8, 15, 14, 30, 0);
        var row = record.AddRow();
        dateTimeColumn.Set(testDate);
        var recordRow = new RecordRow(record, 0);

        // Act
        var result = recordRow.Get<DateTime>("DateTimeColumn");

        // Assert
        Assert.AreEqual(testDate, result);
    }

    #endregion

    #region 边界情况和异常测试

    /// <summary>
    /// 测试空字符串列名
    /// </summary>
    [TestMethod]
    public void GetMethods_EmptyColumnName_ShouldReturnDefault()
    {
        // Arrange
        var (record, _, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(record, 0);

        // Act & Assert
        Assert.AreEqual(default(int), recordRow.Get<Int32>(""));
        Assert.AreEqual(default(string), recordRow.Get<String>(""));
        Assert.AreEqual(default(bool), recordRow.Get<Boolean>(""));
    }

    /// <summary>
    /// 测试多行数据的正确性
    /// </summary>
    [TestMethod]
    public void GetMethods_MultipleRows_ShouldReturnCorrectValues()
    {
        // Arrange
        var (record, intColumn, stringColumn, boolColumn) = CreateTestRecord();

        // 添加更多测试数据
        var row3 = record.AddRow();
        intColumn.Set(300);
        stringColumn.Set("Test3");
        boolColumn.Set(true);

        var recordRow0 = new RecordRow(record, 0);
        var recordRow1 = new RecordRow(record, 1);
        var recordRow2 = new RecordRow(record, 2);

        // Act & Assert
        Assert.AreEqual(100, recordRow0.Get<Int32>("IntColumn"));
        Assert.AreEqual(200, recordRow1.Get<Int32>("IntColumn"));
        Assert.AreEqual(300, recordRow2.Get<Int32>("IntColumn"));

        Assert.AreEqual("Test1", recordRow0.Get<String>("StringColumn"));
        Assert.AreEqual("Test2", recordRow1.Get<String>("StringColumn"));
        Assert.AreEqual("Test3", recordRow2.Get<String>("StringColumn"));

        Assert.AreEqual(true, recordRow0.Get<Boolean>("BoolColumn"));
        Assert.AreEqual(false, recordRow1.Get<Boolean>("BoolColumn"));
        Assert.AreEqual(true, recordRow2.Get<Boolean>("BoolColumn"));
    }

    #endregion

    #region 性能和一致性测试

    /// <summary>
    /// 测试相同数据的重复访问应该返回相同结果
    /// </summary>
    [TestMethod]
    public void GetMethods_RepeatedAccess_ShouldReturnConsistentResults()
    {
        // Arrange
        var (record, intColumn, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(record, 0);

        // Act
        var result1 = recordRow.Get<Int32>("IntColumn");
        var result2 = recordRow.Get<Int32>("IntColumn");
        var result3 = recordRow.Get<Int32>(intColumn);

        // Assert
        Assert.AreEqual(result1, result2);
        Assert.AreEqual(result2, result3);
        Assert.AreEqual(100, result1);
    }

    /// <summary>
    /// 测试不同获取方式(按名称 vs 按列对象)的结果一致性
    /// </summary>
    [TestMethod]
    public void GetMethods_ByNameVsByColumn_ShouldReturnSameResults()
    {
        // Arrange
        var (record, intColumn, stringColumn, boolColumn) = CreateTestRecord();
        var recordRow = new RecordRow(record, 1);

        // Act & Assert
        Assert.AreEqual(recordRow.Get<Int32>("IntColumn"), recordRow.Get<Int32>(intColumn));
        Assert.AreEqual(recordRow.Get<String>("StringColumn"), recordRow.Get<String>(stringColumn));
        Assert.AreEqual(recordRow.Get<Boolean>("BoolColumn"), recordRow.Get<Boolean>(boolColumn));
    }

    #endregion
}