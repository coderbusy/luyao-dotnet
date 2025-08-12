using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace LuYao.Data;

/// <summary>
/// 测试泛型 Set 和 To 方法的功能和性能
/// </summary>
[TestClass]
public class GenericMethodTests
{
    /// <summary>
    /// 测试 RecordColumn 的泛型 Set 方法
    /// </summary>
    [TestMethod]
    public void RecordColumn_GenericSet_ShouldWorkWithAllTypes()
    {
        // Arrange
        var record = new Record("TestTable", 1);
        var intColumn = record.Columns.Add<Int32>("IntColumn");
        var stringColumn = record.Columns.Add<String>("StringColumn");
        var boolColumn = record.Columns.Add<Boolean>("BoolColumn");
        var dateTimeColumn = record.Columns.Add<DateTime>("DateTimeColumn");
        var doubleColumn = record.Columns.Add<Double>("DoubleColumn");

        var row = record.AddRow();
        var rowIndex = 0;

        // Act & Assert - 测试所有基础类型
        intColumn.Set(42, rowIndex);
        Assert.AreEqual(42, intColumn.Get<Int32>(rowIndex));

        stringColumn.Set("Hello World", rowIndex);
        Assert.AreEqual("Hello World", stringColumn.Get<String>(rowIndex));

        boolColumn.Set(true, rowIndex);
        Assert.AreEqual(true, boolColumn.Get<Boolean>(rowIndex));

        var testDate = new DateTime(2023, 7, 28, 10, 30, 0);
        dateTimeColumn.Set(testDate, rowIndex);
        Assert.AreEqual(testDate, dateTimeColumn.Get<DateTime>(rowIndex));

        doubleColumn.Set(3.14159, rowIndex);
        Assert.AreEqual(3.14159, doubleColumn.Get<Double>(rowIndex), 0.00001);
    }

    /// <summary>
    /// 测试 RecordRow 的泛型 Set 方法
    /// </summary>
    [TestMethod]
    public void RecordRow_GenericSet_ShouldWorkWithAllTypes()
    {
        // Arrange
        var record = new Record("TestTable", 1);
        var intColumn = record.Columns.Add<Int32>("IntColumn");
        var stringColumn = record.Columns.Add<String>("StringColumn");
        var boolColumn = record.Columns.Add<Boolean>("BoolColumn");
        var row = record.AddRow();

        // Act
        intColumn.Set(123);
        stringColumn.Set("Test String");
        boolColumn.Set(false);

        // Assert
        Assert.AreEqual(123, row.Get<Int32>(intColumn));
        Assert.AreEqual("Test String", row.Get<String>(stringColumn));
        Assert.AreEqual(false, row.Get<Boolean>(boolColumn));
    }


    /// <summary>
    /// 测试 To 泛型方法
    /// </summary>
    [TestMethod]
    public void GenericTo_ShouldReturnCorrectTypes()
    {
        // Arrange
        var record = new Record("TestTable", 1);
        var intColumn = record.Columns.Add<Int32>("IntColumn");
        var stringColumn = record.Columns.Add<String>("StringColumn");
        var row = record.AddRow();

        // 设置一些测试数据
        intColumn.Set(42);
        stringColumn.Set("Hello");

        // Act & Assert
        int intValue = intColumn.Get<int>(0);
        Assert.AreEqual(42, intValue);

        string stringValue = stringColumn.Get<string>(0);
        Assert.AreEqual("Hello", stringValue);

        // 测试类型转换
        string intAsString = intColumn.Get<string>(0);
        Assert.AreEqual("42", intAsString);

        // 通过 RecordRow 测试
        int intFromRow = row.Get<int>(intColumn);
        Assert.AreEqual(42, intFromRow);

        string stringFromRow = row.Get<string>(stringColumn);
        Assert.AreEqual("Hello", stringFromRow);
    }

    /// <summary>
    /// 测试可空类型支持
    /// </summary>
    [TestMethod]
    public void GenericMethods_NullableTypes_ShouldWork()
    {
        // Arrange
        var record = new Record("TestTable", 1);
        var intColumn = record.Columns.Add<Int32>("IntColumn");
        var row = record.AddRow();

        // Act & Assert - 测试可空类型
        int? nullableValue = 42;
        intColumn.SetValue(nullableValue, 0);
        Assert.AreEqual(42, intColumn.Get<Int32>(0));

        // 测试 null 值处理
        intColumn.SetValue(null, 0);
        int defaultValue = intColumn.Get<Int32>(0);
        Assert.AreEqual(0, defaultValue); // 默认值
    }

    /// <summary>
    /// 测试边界情况
    /// </summary>
    [TestMethod]
    public void GenericMethods_EdgeCases_ShouldHandleCorrectly()
    {
        // Arrange
        var record = new Record("TestTable", 1);
        var stringColumn = record.Columns.Add<String>("StringColumn");
        var row = record.AddRow();

        // Act & Assert - 测试空字符串
        stringColumn.Set("", 0);
        Assert.AreEqual("", stringColumn.Get<String>(0));

        // 测试 null 字符串
        stringColumn.Set(null, 0);
        string result = stringColumn.Get<String>(0);
        Assert.IsNull(result); // 应该返回 null
    }

    /// <summary>
    /// 测试索引验证
    /// </summary>
    [TestMethod]
    public void GenericSet_InvalidIndex_ShouldThrowException()
    {
        // Arrange
        var record = new Record("TestTable", 1);
        var intColumn = record.Columns.Add<Int32>("IntColumn");
        record.AddRow(); // 只有一行，有效索引是 0

        // Act & Assert
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            intColumn.Set(42, 1)); // 无效索引

        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            intColumn.Set(42, -1)); // 负索引
    }

    /// <summary>
    /// 性能对比测试 - 展示泛型方法相对于通用方法的优势
    /// </summary>
    [TestMethod]
    public void GenericMethods_PerformanceComparison()
    {
        // Arrange
        var record = new Record("PerfTest", 1000);
        var intColumn = record.Columns.Add<Int32>("IntColumn");

        // 添加 1000 行
        for (int i = 0; i < 1000; i++)
        {
            record.AddRow();
        }

        //预热
        intColumn.Set(0, 0);
        intColumn.SetValue(1, 1);

        var sw = System.Diagnostics.Stopwatch.StartNew();

        // 测试泛型方法性能
        sw.Restart();
        for (int i = 0; i < 1000; i++)
        {
            intColumn.Set(i, i);
        }
        var genericTime = sw.ElapsedTicks;

        // 测试通用方法性能  
        sw.Restart();
        for (int i = 0; i < 1000; i++)
        {
            intColumn.SetValue(i, i);
        }
        var objectTime = sw.ElapsedTicks;

        sw.Stop();

        // 验证结果正确性
        for (int i = 0; i < 1000; i++)
        {
            Assert.AreEqual(i, intColumn.Get<Int32>(i));
        }

        // 输出性能对比（调试信息）
        System.Diagnostics.Debug.WriteLine($"泛型方法耗时: {genericTime} ticks");
        System.Diagnostics.Debug.WriteLine($"通用方法耗时: {objectTime} ticks");
        System.Diagnostics.Debug.WriteLine($"性能提升: {(double)objectTime / genericTime:F2}x");

        // 泛型方法应该更快（允许一定的测量误差）
        Assert.IsTrue(genericTime <= objectTime * 1.5,
            $"泛型方法性能应该优于或接近通用方法。泛型: {genericTime}, 通用: {objectTime}");
    }

    /// <summary>
    /// 测试不同数据类型的转换矩阵
    /// </summary>
    [TestMethod]
    public void GenericMethods_TypeConversionMatrix_ShouldWork()
    {
        // Arrange
        var record = new Record("ConversionTest", 1);
        var intColumn = record.Columns.Add<Int32>("IntColumn");
        var doubleColumn = record.Columns.Add<Double>("DoubleColumn");
        var stringColumn = record.Columns.Add<String>("StringColumn");
        var boolColumn = record.Columns.Add<Boolean>("BoolColumn");

        var row = record.AddRow();

        // Act & Assert - 测试各种类型转换

        // int -> 其他类型
        intColumn.Set(42, 0);
        Assert.AreEqual(42.0, intColumn.Get<double>(0), 0.001);
        Assert.AreEqual("42", intColumn.Get<string>(0));
        Assert.AreEqual(true, intColumn.Get<bool>(0)); // 非零值为 true

        // double -> 其他类型
        doubleColumn.Set(3.14, 0);
        Assert.AreEqual(3, doubleColumn.Get<int>(0)); // 截断
        Assert.AreEqual("3.14", doubleColumn.Get<string>(0));

        // string -> 其他类型
        stringColumn.Set("123", 0);
        Assert.AreEqual(123, stringColumn.Get<int>(0));
        Assert.AreEqual(123.0, stringColumn.Get<double>(0), 0.001);

        // bool -> 其他类型
        boolColumn.Set(true, 0);
        Assert.AreEqual(1, boolColumn.Get<int>(0));
        Assert.AreEqual("True", boolColumn.Get<string>(0));
    }
}