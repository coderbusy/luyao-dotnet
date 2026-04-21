using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace LuYao.Data;

/// <summary>
/// ���Է��� Set �� To �����Ĺ��ܺ�����
/// </summary>
[TestClass]
public class GenericMethodTests
{
    /// <summary>
    /// ���� RecordColumn �ķ��� Set ����
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

        // Act & Assert - �������л�������
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
    /// ���� RecordRow �ķ��� Set ����
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
    /// ���� To ���ͷ���
    /// </summary>
    [TestMethod]
    public void GenericTo_ShouldReturnCorrectTypes()
    {
        // Arrange
        var record = new Record("TestTable", 1);
        var intColumn = record.Columns.Add<Int32>("IntColumn");
        var stringColumn = record.Columns.Add<String>("StringColumn");
        var row = record.AddRow();

        // ����һЩ��������
        intColumn.Set(42, 0);
        stringColumn.Set("Hello", 0);

        // Act & Assert
        int intValue = intColumn.Get<int>(0);
        Assert.AreEqual(42, intValue);

        string stringValue = stringColumn.Get<string>(0);
        Assert.AreEqual("Hello", stringValue);

        // ��������ת��
        string intAsString = intColumn.Get<string>(0);
        Assert.AreEqual("42", intAsString);

        // ͨ�� RecordRow ����
        int intFromRow = row.Field<int>(intColumn);
        Assert.AreEqual(42, intFromRow);

        string stringFromRow = row.Field<string>(stringColumn);
        Assert.AreEqual("Hello", stringFromRow);
    }

    /// <summary>
    /// ���Կɿ�����֧��
    /// </summary>
    [TestMethod]
    public void GenericMethods_NullableTypes_ShouldWork()
    {
        // Arrange
        var record = new Record("TestTable", 1);
        var intColumn = record.Columns.Add<Int32>("IntColumn");
        var row = record.AddRow();

        // Act & Assert - ���Կɿ�����
        int? nullableValue = 42;
        intColumn.SetValue(nullableValue, 0);
        Assert.AreEqual(42, intColumn.Get<Int32>(0));

        // ���� null ֵ����
        intColumn.SetValue(null, 0);
        int defaultValue = intColumn.Get<Int32>(0);
        Assert.AreEqual(0, defaultValue); // Ĭ��ֵ
    }

    /// <summary>
    /// ���Ա߽����
    /// </summary>
    [TestMethod]
    public void GenericMethods_EdgeCases_ShouldHandleCorrectly()
    {
        // Arrange
        var record = new Record("TestTable", 1);
        var stringColumn = record.Columns.Add<String>("StringColumn");
        var row = record.AddRow();

        // Act & Assert - ���Կ��ַ���
        stringColumn.Set("", 0);
        Assert.AreEqual("", stringColumn.Get<String>(0));

        // ���� null �ַ���
        stringColumn.Set(null, 0);
        string result = stringColumn.Get<String>(0);
        Assert.IsNull(result); // Ӧ�÷��� null
    }

    /// <summary>
    /// ����������֤
    /// </summary>
    [TestMethod]
    public void GenericSet_InvalidIndex_ShouldThrowException()
    {
        // Arrange
        var record = new Record("TestTable", 1);
        var intColumn = record.Columns.Add<Int32>("IntColumn");
        record.AddRow(); // ֻ��һ�У���Ч������ 0

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            intColumn.Set(42, 1)); // ��Ч����

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            intColumn.Set(42, -1)); // ������
    }

    /// <summary>
    /// ���ܶԱȲ��� - չʾ���ͷ��������ͨ�÷���������
    /// </summary>
    [TestMethod]
    public void GenericMethods_PerformanceComparison()
    {
        // Arrange
        var record = new Record("PerfTest", 1000);
        var intColumn = record.Columns.Add<Int32>("IntColumn");

        // ��� 1000 ��
        for (int i = 0; i < 1000; i++)
        {
            record.AddRow();
        }

        //Ԥ��
        intColumn.Set(0, 0);
        intColumn.SetValue(1, 1);

        var sw = System.Diagnostics.Stopwatch.StartNew();

        // ���Է��ͷ�������
        sw.Restart();
        for (int i = 0; i < 1000; i++)
        {
            intColumn.Set(i, i);
        }
        var genericTime = sw.ElapsedTicks;

        // ����ͨ�÷�������  
        sw.Restart();
        for (int i = 0; i < 1000; i++)
        {
            intColumn.SetValue(i, i);
        }
        var objectTime = sw.ElapsedTicks;

        sw.Stop();

        // ��֤�����ȷ��
        for (int i = 0; i < 1000; i++)
        {
            Assert.AreEqual(i, intColumn.Get<Int32>(i));
        }

        // ������ܶԱȣ�������Ϣ��
        System.Diagnostics.Debug.WriteLine($"���ͷ�����ʱ: {genericTime} ticks");
        System.Diagnostics.Debug.WriteLine($"ͨ�÷�����ʱ: {objectTime} ticks");
        System.Diagnostics.Debug.WriteLine($"��������: {(double)objectTime / genericTime:F2}x");

        // ���ͷ���Ӧ�ø��죨����һ���Ĳ�����
        Assert.IsTrue(genericTime <= objectTime * 1.5,
            $"���ͷ�������Ӧ�����ڻ�ӽ�ͨ�÷���������: {genericTime}, ͨ��: {objectTime}");
    }

    /// <summary>
    /// ���Բ�ͬ�������͵�ת������
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

        // Act & Assert - ���Ը�������ת��

        // int -> ��������
        intColumn.Set(42, 0);
        Assert.AreEqual(42.0, intColumn.Get<double>(0), 0.001);
        Assert.AreEqual("42", intColumn.Get<string>(0));
        Assert.AreEqual(true, intColumn.Get<bool>(0)); // ����ֵΪ true

        // double -> ��������
        doubleColumn.Set(3.14, 0);
        Assert.AreEqual(3, doubleColumn.Get<int>(0)); // �ض�
        Assert.AreEqual("3.14", doubleColumn.Get<string>(0));

        // string -> ��������
        stringColumn.Set("123", 0);
        Assert.AreEqual(123, stringColumn.Get<int>(0));
        Assert.AreEqual(123.0, stringColumn.Get<double>(0), 0.001);

        // bool -> ��������
        boolColumn.Set(true, 0);
        Assert.AreEqual(1, boolColumn.Get<int>(0));
        Assert.AreEqual("True", boolColumn.Get<string>(0));
    }
}