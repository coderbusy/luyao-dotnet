using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace LuYao.Data;

/// <summary>
/// Tests the generic Set and To APIs.
/// </summary>
[TestClass]
public class GenericMethodTests
{
    /// <summary>
    /// Verifies generic Set on RecordColumn.
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

        // Act & Assert - verify all supported types
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
    /// Verifies generic Set through RecordRow.
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
        intColumn.Set(123, row.Row);
        stringColumn.Set("Test String", row.Row);
        boolColumn.Set(false, row.Row);

        // Assert
        Assert.AreEqual(123, row.Field<Int32>(intColumn));
        Assert.AreEqual("Test String", row.Field<String>(stringColumn));
        Assert.AreEqual(false, row.Field<Boolean>(boolColumn));
    }


    /// <summary>
    /// Verifies generic type conversion helpers.
    /// </summary>
    [TestMethod]
    public void GenericTo_ShouldReturnCorrectTypes()
    {
        // Arrange
        var record = new Record("TestTable", 1);
        var intColumn = record.Columns.Add<Int32>("IntColumn");
        var stringColumn = record.Columns.Add<String>("StringColumn");
        var row = record.AddRow();

        // Seed test data
        intColumn.Set(42, 0);
        stringColumn.Set("Hello", 0);

        // Act & Assert
        int intValue = intColumn.Get<int>(0);
        Assert.AreEqual(42, intValue);

        string stringValue = stringColumn.Get<string>(0);
        Assert.AreEqual("Hello", stringValue);

        // Verify cross-type conversion
        string intAsString = intColumn.Get<string>(0);
        Assert.AreEqual("42", intAsString);

        // Verify access through RecordRow
        int intFromRow = row.Field<int>(intColumn);
        Assert.AreEqual(42, intFromRow);

        string stringFromRow = row.Field<string>(stringColumn);
        Assert.AreEqual("Hello", stringFromRow);
    }

    /// <summary>
    /// Verifies nullable type support.
    /// </summary>
    [TestMethod]
    public void GenericMethods_NullableTypes_ShouldWork()
    {
        // Arrange
        var record = new Record("TestTable", 1);
        var intColumn = record.Columns.Add<Int32>("IntColumn");
        var row = record.AddRow();

        // Act & Assert - nullable value
        int? nullableValue = 42;
        intColumn.SetValue(0, nullableValue);
        Assert.AreEqual(42, intColumn.Get<Int32>(0));

        // Verify null assignment
        intColumn.SetValue(0, null);
        int defaultValue = intColumn.Get<Int32>(0);
        Assert.AreEqual(0, defaultValue); // default value
    }

    /// <summary>
    /// Verifies edge cases.
    /// </summary>
    [TestMethod]
    public void GenericMethods_EdgeCases_ShouldHandleCorrectly()
    {
        // Arrange
        var record = new Record("TestTable", 1);
        var stringColumn = record.Columns.Add<String>("StringColumn");
        var row = record.AddRow();

        // Act & Assert - empty string
        stringColumn.Set("", 0);
        Assert.AreEqual("", stringColumn.Get<String>(0));

        // Verify null string
        stringColumn.Set(null, 0);
        string result = stringColumn.Get<String>(0);
        Assert.IsNull(result); // should return null
    }

    /// <summary>
    /// Verifies invalid index handling.
    /// </summary>
    [TestMethod]
    public void GenericSet_InvalidIndex_ShouldThrowException()
    {
        // Arrange
        var record = new Record("TestTable", 1);
        var intColumn = record.Columns.Add<Int32>("IntColumn");
        record.AddRow(); // only one row, valid index is 0

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            intColumn.Set(42, 1)); // invalid index

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            intColumn.Set(42, -1)); // negative index
    }

    /// <summary>
    /// Compares generic APIs with object-based APIs.
    /// </summary>
    [TestMethod]
    public void GenericMethods_PerformanceComparison()
    {
        // Arrange
        var record = new Record("PerfTest", 1000);
        var intColumn = record.Columns.Add<Int32>("IntColumn");

        // Add 1000 rows
        for (int i = 0; i < 1000; i++)
        {
            record.AddRow();
        }

        // Warm up
        intColumn.Set(0, 0);
        intColumn.SetValue(1, 1);

        var sw = System.Diagnostics.Stopwatch.StartNew();

        // Measure generic method
        sw.Restart();
        for (int i = 0; i < 1000; i++)
        {
            intColumn.Set(i, i);
        }
        var genericTime = sw.ElapsedTicks;

        // Measure object-based method
        sw.Restart();
        for (int i = 0; i < 1000; i++)
        {
            intColumn.SetValue(i, i);
        }
        var objectTime = sw.ElapsedTicks;

        sw.Stop();

        // Verify results
        for (int i = 0; i < 1000; i++)
        {
            Assert.AreEqual(i, intColumn.Get<Int32>(i));
        }

        // Output comparison details
        System.Diagnostics.Debug.WriteLine($"Generic method time: {genericTime} ticks");
        System.Diagnostics.Debug.WriteLine($"Object method time: {objectTime} ticks");
        System.Diagnostics.Debug.WriteLine($"Performance ratio: {(double)objectTime / genericTime:F2}x");

        // Generic method should be faster, or at least close enough.
        Assert.IsTrue(genericTime <= objectTime * 1.5,
            $"Generic method should not be meaningfully slower than object method. Generic: {genericTime}, Object: {objectTime}");
    }

    /// <summary>
    /// Verifies conversions across different source types.
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

        // Act & Assert - verify conversions across types

        // int -> other types
        intColumn.Set(42, 0);
        Assert.AreEqual(42.0, intColumn.Get<double>(0), 0.001);
        Assert.AreEqual("42", intColumn.Get<string>(0));
        Assert.AreEqual(true, intColumn.Get<bool>(0)); // non-zero means true

        // double -> other types
        doubleColumn.Set(3.14, 0);
        Assert.AreEqual(3, doubleColumn.Get<int>(0)); // truncated
        Assert.AreEqual("3.14", doubleColumn.Get<string>(0));

        // string -> other types
        stringColumn.Set("123", 0);
        Assert.AreEqual(123, stringColumn.Get<int>(0));
        Assert.AreEqual(123.0, stringColumn.Get<double>(0), 0.001);

        // bool -> other types
        boolColumn.Set(true, 0);
        Assert.AreEqual(1, boolColumn.Get<int>(0));
        Assert.AreEqual("True", boolColumn.Get<string>(0));
    }
}