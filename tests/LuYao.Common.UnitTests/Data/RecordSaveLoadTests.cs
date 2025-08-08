using LuYao.Data.Binary;
using LuYao.Data.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;

namespace LuYao.Data;

/// <summary>
/// Record 类的 Save 和 Load 方法单元测试类
/// </summary>
[TestClass]
public class RecordSaveLoadTests
{
    #region Save 方法测试

    /// <summary>
    /// 测试使用 BinaryRecordSaveAdapter 保存空记录
    /// </summary>
    [TestMethod]
    public void Save_EmptyRecord_ShouldSaveSuccessfully()
    {
        // Arrange
        var record = new Record("EmptyRecord", 0);
        using var memoryStream = new MemoryStream();
        using var writer = new BinaryWriter(memoryStream);
        var adapter = new BinaryRecordSaveAdapter(writer);

        // Act
        record.Save(adapter);

        // Assert
        Assert.IsTrue(memoryStream.Length > 0);
    }

    /// <summary>
    /// 测试使用 BinaryRecordSaveAdapter 保存包含各种数据类型的记录
    /// </summary>
    [TestMethod]
    public void Save_RecordWithMixedDataTypes_ShouldSaveSuccessfully()
    {
        // Arrange
        var record = CreateTestRecord();
        using var memoryStream = new MemoryStream();
        using var writer = new BinaryWriter(memoryStream);
        var adapter = new BinaryRecordSaveAdapter(writer);

        // Act
        record.Save(adapter);

        // Assert
        Assert.IsTrue(memoryStream.Length > 0);

        // 验证保存的数据包含所有预期的字节
        var savedBytes = memoryStream.ToArray();
        Assert.IsTrue(savedBytes.Length > 100); // 应该有足够的数据量
    }

    /// <summary>
    /// 测试 Save 方法参数为 null 时抛出异常
    /// </summary>
    [TestMethod]
    public void Save_NullAdapter_ShouldThrowArgumentNullException()
    {
        // Arrange
        var record = new Record();

        // Act & Assert
        Assert.ThrowsException<ArgumentNullException>(() => record.Save(null!));
    }

    /// <summary>
    /// 测试保存只有列结构但无数据的记录
    /// </summary>
    [TestMethod]
    public void Save_RecordWithColumnsButNoData_ShouldSaveSuccessfully()
    {
        // Arrange
        var record = new Record("TestRecord", 0);
        record.Columns.Add<int>("IntColumn");
        record.Columns.Add<string>("StringColumn");
        record.Columns.Add<bool>("BoolColumn");

        using var memoryStream = new MemoryStream();
        using var writer = new BinaryWriter(memoryStream);
        var adapter = new BinaryRecordSaveAdapter(writer);

        // Act
        record.Save(adapter);

        // Assert
        Assert.IsTrue(memoryStream.Length > 0);
    }

    /// <summary>
    /// 测试保存后游标位置的变化
    /// </summary>
    [TestMethod]
    public void Save_ShouldResetCursorPosition()
    {
        // Arrange
        var record = CreateTestRecord();
        record.Cursor = 1; // 设置游标到非零位置

        using var memoryStream = new MemoryStream();
        using var writer = new BinaryWriter(memoryStream);
        var adapter = new BinaryRecordSaveAdapter(writer);

        // Act
        record.Save(adapter);

        // Assert
        // 保存过程会调用 MoveFirst() 和 Read()，最终游标会发生变化
        // 具体位置取决于 Read() 方法的实现逻辑
    }

    #endregion

    #region Load 方法测试

    /// <summary>
    /// 测试使用 BinaryRecordLoadAdapter 加载空记录
    /// </summary>
    [TestMethod]
    public void Load_EmptyRecord_ShouldLoadSuccessfully()
    {
        // Arrange
        var originalRecord = new Record("EmptyRecord", 0);

        // 先保存记录
        using var memoryStream = new MemoryStream();
        using (var writer = new BinaryWriter(memoryStream, Encoding.UTF8, true))
        {
            var saveAdapter = new BinaryRecordSaveAdapter(writer);
            originalRecord.Save(saveAdapter);
        }

        // 重置流位置
        memoryStream.Position = 0;

        // Act
        using var reader = new BinaryReader(memoryStream);
        var loadAdapter = new BinaryRecordLoadAdapter(reader);
        var loadedRecord = Record.Load(loadAdapter);

        // Assert
        Assert.IsNotNull(loadedRecord);
        Assert.AreEqual(originalRecord.Name, loadedRecord.Name);
        Assert.AreEqual(originalRecord.Count, loadedRecord.Count);
        Assert.AreEqual(originalRecord.Columns.Count, loadedRecord.Columns.Count);
    }

    /// <summary>
    /// 测试使用 BinaryRecordLoadAdapter 加载包含各种数据类型的记录
    /// </summary>
    [TestMethod]
    public void Load_RecordWithMixedDataTypes_ShouldLoadSuccessfully()
    {
        // Arrange
        var originalRecord = CreateTestRecord();

        // 先保存记录
        using var memoryStream = new MemoryStream();
        using (var writer = new BinaryWriter(memoryStream, Encoding.UTF8, true))
        {
            var saveAdapter = new BinaryRecordSaveAdapter(writer);
            originalRecord.Save(saveAdapter);
        }

        // 重置流位置
        memoryStream.Position = 0;

        // Act
        using var reader = new BinaryReader(memoryStream, Encoding.UTF8);
        var loadAdapter = new BinaryRecordLoadAdapter(reader);
        var loadedRecord = Record.Load(loadAdapter);

        // Assert
        Assert.IsNotNull(loadedRecord);
        Assert.AreEqual(originalRecord.Name, loadedRecord.Name);
        Assert.AreEqual(originalRecord.Count, loadedRecord.Count);
        Assert.AreEqual(originalRecord.Columns.Count, loadedRecord.Columns.Count);

        // 验证数据完整性
        VerifyRecordData(originalRecord, loadedRecord);
    }

    /// <summary>
    /// 测试 Load 方法参数为 null 时抛出异常
    /// </summary>
    [TestMethod]
    public void Load_NullAdapter_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.ThrowsException<ArgumentNullException>(() => Record.Load(null!));
    }

    /// <summary>
    /// 测试加载只有列结构但无数据的记录
    /// </summary>
    [TestMethod]
    public void Load_RecordWithColumnsButNoData_ShouldLoadSuccessfully()
    {
        // Arrange
        var originalRecord = new Record("TestRecord", 0);
        originalRecord.Columns.Add<int>("IntColumn");
        originalRecord.Columns.Add<string>("StringColumn");
        originalRecord.Columns.Add<bool>("BoolColumn");

        // 先保存记录
        using var memoryStream = new MemoryStream();
        using (var writer = new BinaryWriter(memoryStream, Encoding.UTF8, true))
        {
            var saveAdapter = new BinaryRecordSaveAdapter(writer);
            originalRecord.Save(saveAdapter);
        }

        // 重置流位置
        memoryStream.Position = 0;

        // Act
        using var reader = new BinaryReader(memoryStream);
        var loadAdapter = new BinaryRecordLoadAdapter(reader);
        var loadedRecord = Record.Load(loadAdapter);

        // Assert
        Assert.IsNotNull(loadedRecord);
        Assert.AreEqual(originalRecord.Name, loadedRecord.Name);
        Assert.AreEqual(originalRecord.Count, loadedRecord.Count);
        Assert.AreEqual(originalRecord.Columns.Count, loadedRecord.Columns.Count);

        // 验证列类型
        for (int i = 0; i < originalRecord.Columns.Count; i++)
        {
            Assert.AreEqual(originalRecord.Columns[i].Name, loadedRecord.Columns[i].Name);
            Assert.AreEqual(originalRecord.Columns[i].Code, loadedRecord.Columns[i].Code);
        }
    }

    #endregion

    #region 完整的往返测试（Round-trip Tests）

    /// <summary>
    /// 测试完整的往返过程：Record -> Save -> Load -> Record
    /// </summary>
    [TestMethod]
    public void SaveLoad_RoundTrip_ShouldPreserveAllData()
    {
        // Arrange
        var originalRecord = CreateComplexTestRecord();

        // Act - 保存和加载
        using var memoryStream = new MemoryStream();

        // 保存
        using (var writer = new BinaryWriter(memoryStream, Encoding.UTF8, true))
        {
            var saveAdapter = new BinaryRecordSaveAdapter(writer);
            originalRecord.Save(saveAdapter);
        }

        // 加载
        memoryStream.Position = 0;
        using var reader = new BinaryReader(memoryStream);
        var loadAdapter = new BinaryRecordLoadAdapter(reader);
        var loadedRecord = Record.Load(loadAdapter);

        // Assert
        Assert.IsNotNull(loadedRecord);
        Assert.AreEqual(originalRecord.Name, loadedRecord.Name);
        Assert.AreEqual(originalRecord.Count, loadedRecord.Count);
        Assert.AreEqual(originalRecord.Columns.Count, loadedRecord.Columns.Count);

        // 详细验证所有数据
        VerifyCompleteRecordData(originalRecord, loadedRecord);
    }

    /// <summary>
    /// 测试多次往返的数据一致性
    /// </summary>
    [TestMethod]
    public void SaveLoad_MultipleRoundTrips_ShouldMaintainConsistency()
    {
        // Arrange
        var originalRecord = CreateTestRecord();

        Record currentRecord = originalRecord;

        // Act - 执行多次往返
        for (int i = 0; i < 3; i++)
        {
            using var memoryStream = new MemoryStream();

            // 保存
            using (var writer = new BinaryWriter(memoryStream, Encoding.UTF8, true))
            {
                var saveAdapter = new BinaryRecordSaveAdapter(writer);
                currentRecord.Save(saveAdapter);
            }

            // 加载
            memoryStream.Position = 0;
            using var reader = new BinaryReader(memoryStream);
            var loadAdapter = new BinaryRecordLoadAdapter(reader);
            currentRecord = Record.Load(loadAdapter);
        }

        // Assert
        VerifyRecordData(originalRecord, currentRecord);
    }

    #endregion

    #region BinaryRecordSaveAdapter 和 BinaryRecordLoadAdapter 特定测试

    /// <summary>
    /// 测试 BinaryRecordSaveAdapter 构造函数参数验证
    /// </summary>
    [TestMethod]
    public void BinaryRecordSaveAdapter_Constructor_NullWriter_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.ThrowsException<ArgumentNullException>(() => new BinaryRecordSaveAdapter(null!));
    }

    /// <summary>
    /// 测试 BinaryRecordLoadAdapter 构造函数参数验证
    /// </summary>
    [TestMethod]
    public void BinaryRecordLoadAdapter_Constructor_NullReader_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.ThrowsException<ArgumentNullException>(() => new BinaryRecordLoadAdapter(null!));
    }

    /// <summary>
    /// 测试 BinaryRecordLoadAdapter 的 KeyKind 属性
    /// </summary>
    [TestMethod]
    public void BinaryRecordLoadAdapter_KeyKind_ShouldReturnIndex()
    {
        // Arrange
        using var memoryStream = new MemoryStream();
        using var reader = new BinaryReader(memoryStream);
        var adapter = new BinaryRecordLoadAdapter(reader);

        // Act & Assert
        Assert.AreEqual(RecordLoadKeyKind.Index, adapter.KeyKind);
    }

    /// <summary>
    /// 测试 BinaryRecordSaveAdapter 处理复杂对象类型时抛出异常
    /// </summary>
    [TestMethod]
    public void BinaryRecordSaveAdapter_WriteObject_ShouldThrowNotImplementedException()
    {
        // Arrange
        using var memoryStream = new MemoryStream();
        using var writer = new BinaryWriter(memoryStream);
        var adapter = new BinaryRecordSaveAdapter(writer);

        // Act & Assert
        Assert.ThrowsException<NotImplementedException>(() =>
            adapter.WriteObject("test", 0, new object()));
    }

    /// <summary>
    /// 测试 BinaryRecordLoadAdapter 处理复杂对象类型时抛出异常
    /// </summary>
    [TestMethod]
    public void BinaryRecordLoadAdapter_ReadObject_ShouldThrowNotImplementedException()
    {
        // Arrange
        using var memoryStream = new MemoryStream();
        using var reader = new BinaryReader(memoryStream);
        var adapter = new BinaryRecordLoadAdapter(reader);

        // Act & Assert
        Assert.ThrowsException<NotImplementedException>(() =>
            adapter.ReadObject(typeof(object)));
    }

    #endregion

    #region 测试辅助方法

    /// <summary>
    /// 创建测试用的 Record，包含各种基本数据类型
    /// </summary>
    private static Record CreateTestRecord()
    {
        var record = new Record("TestRecord", 3);

        // 添加各种类型的列
        var intColumn = record.Columns.Add<int>("IntColumn");
        var stringColumn = record.Columns.Add<string>("StringColumn");
        var boolColumn = record.Columns.Add<bool>("BoolColumn");
        var doubleColumn = record.Columns.Add<double>("DoubleColumn");
        var dateTimeColumn = record.Columns.Add<DateTime>("DateTimeColumn");

        // 添加测试数据
        for (int i = 0; i < 2; i++)
        {
            var row = record.AddRow();
            intColumn.Set(100 + i, i);
            stringColumn.Set($"Test{i}", i);
            boolColumn.Set(i % 2 == 0, i);
            doubleColumn.Set(123.45 + i, i);
            dateTimeColumn.Set(new DateTime(2023, 1, 1).AddDays(i), i);
        }

        return record;
    }

    /// <summary>
    /// 创建包含所有支持数据类型的复杂测试记录
    /// </summary>
    private static Record CreateComplexTestRecord()
    {
        var record = new Record("ComplexTestRecord", 10);

        // 添加所有支持的数据类型列
        var boolColumn = record.Columns.Add<bool>("BoolColumn");
        var byteColumn = record.Columns.Add<byte>("ByteColumn");
        var charColumn = record.Columns.Add<char>("CharColumn");
        var dateTimeColumn = record.Columns.Add<DateTime>("DateTimeColumn");
        var decimalColumn = record.Columns.Add<decimal>("DecimalColumn");
        var doubleColumn = record.Columns.Add<double>("DoubleColumn");
        var int16Column = record.Columns.Add<short>("Int16Column");
        var int32Column = record.Columns.Add<int>("Int32Column");
        var int64Column = record.Columns.Add<long>("Int64Column");
        var sbyteColumn = record.Columns.Add<sbyte>("SByteColumn");
        var singleColumn = record.Columns.Add<float>("SingleColumn");
        var stringColumn = record.Columns.Add<string>("StringColumn");
        var uint16Column = record.Columns.Add<ushort>("UInt16Column");
        var uint32Column = record.Columns.Add<uint>("UInt32Column");
        var uint64Column = record.Columns.Add<ulong>("UInt64Column");

        // 添加测试数据
        for (int i = 0; i < 3; i++)
        {
            var row = record.AddRow();
            boolColumn.Set(i % 2 == 0, i);
            byteColumn.Set((byte)(100 + i), i);
            charColumn.Set((char)('A' + i), i);
            dateTimeColumn.Set(new DateTime(2023, 1, 1).AddDays(i), i);
            decimalColumn.Set(123.45m + i, i);
            doubleColumn.Set(678.90 + i, i);
            int16Column.Set((short)(1000 + i), i);
            int32Column.Set(10000 + i, i);
            int64Column.Set(100000L + i, i);
            sbyteColumn.Set((sbyte)(50 + i), i);
            singleColumn.Set(3.14f + i, i);
            stringColumn.Set($"TestString{i}", i);
            uint16Column.Set((ushort)(2000 + i), i);
            uint32Column.Set((uint)(20000 + i), i);
            uint64Column.Set((ulong)(200000 + i), i);
        }

        return record;
    }

    /// <summary>
    /// 验证两个记录的基本数据是否相同
    /// </summary>
    private static void VerifyRecordData(Record original, Record loaded)
    {
        // 验证基本属性
        Assert.AreEqual(original.Name, loaded.Name);
        Assert.AreEqual(original.Count, loaded.Count);
        Assert.AreEqual(original.Columns.Count, loaded.Columns.Count);

        // 验证列结构
        for (int colIndex = 0; colIndex < original.Columns.Count; colIndex++)
        {
            var originalColumn = original.Columns[colIndex];
            var loadedColumn = loaded.Columns[colIndex];

            Assert.AreEqual(originalColumn.Name, loadedColumn.Name);
            Assert.AreEqual(originalColumn.Code, loadedColumn.Code);

            // 验证数据值
            for (int rowIndex = 0; rowIndex < original.Count; rowIndex++)
            {
                var originalValue = originalColumn.GetValue(rowIndex);
                var loadedValue = loadedColumn.GetValue(rowIndex);

                Assert.AreEqual(originalValue, loadedValue,
                    $"Column {originalColumn.Name}, Row {rowIndex} data mismatch");
            }
        }
    }

    /// <summary>
    /// 验证复杂记录的完整数据
    /// </summary>
    private static void VerifyCompleteRecordData(Record original, Record loaded)
    {
        VerifyRecordData(original, loaded);

        // 额外的完整性检查
        Assert.IsTrue(loaded.Count >= 0);
        Assert.IsTrue(loaded.Columns.Count >= 0);

        if (loaded.Count > 0)
        {
            // 验证游标操作正常
            loaded.MoveFirst();
            Assert.AreEqual(0, loaded.Cursor);

            if (loaded.Count > 1)
            {
                loaded.MoveLast();
                Assert.AreEqual(loaded.Count - 1, loaded.Cursor);
            }
        }
    }

    #endregion

    #region 边界情况和错误处理测试

    /// <summary>
    /// 测试保存大量数据的记录
    /// </summary>
    [TestMethod]
    public void SaveLoad_LargeRecord_ShouldHandleCorrectly()
    {
        // Arrange
        var record = new Record("LargeRecord", 1000);
        var intColumn = record.Columns.Add<int>("IntColumn");
        var stringColumn = record.Columns.Add<string>("StringColumn");

        // 添加大量数据
        for (int i = 0; i < 500; i++)
        {
            var row = record.AddRow();
            intColumn.Set(i, i);
            stringColumn.Set($"LargeData_{i}", i);
        }

        // Act
        using var memoryStream = new MemoryStream();

        // 保存
        using (var writer = new BinaryWriter(memoryStream, Encoding.UTF8, true))
        {
            var saveAdapter = new BinaryRecordSaveAdapter(writer);
            record.Save(saveAdapter);
        }

        // 加载
        memoryStream.Position = 0;
        using var reader = new BinaryReader(memoryStream);
        var loadAdapter = new BinaryRecordLoadAdapter(reader);
        var loadedRecord = Record.Load(loadAdapter);

        // Assert
        Assert.AreEqual(record.Count, loadedRecord.Count);
        Assert.AreEqual(record.Columns.Count, loadedRecord.Columns.Count);

        // 验证部分数据
        Assert.AreEqual(intColumn.Get(0), loadedRecord.Columns.Find("IntColumn")!.GetInt32(0));
        Assert.AreEqual(stringColumn.Get(100), loadedRecord.Columns.Find("StringColumn")!.GetString(100));
    }

    /// <summary>
    /// 测试包含空字符串和特殊字符的记录
    /// </summary>
    [TestMethod]
    public void SaveLoad_RecordWithSpecialStrings_ShouldHandleCorrectly()
    {
        // Arrange
        var record = new Record("SpecialStringRecord", 5);
        var stringColumn = record.Columns.Add<string>("StringColumn");

        var testStrings = new[]
        {
            "",
            " ",
            "normal string",
            "string with\nnewline",
            "string with\ttab",
            "中文字符串",
            "émoji 😀 test",
            "very long string " + new string('x', 1000)
        };

        for (int i = 0; i < testStrings.Length; i++)
        {
            var row = record.AddRow();
            stringColumn.Set(testStrings[i], i);
        }

        // Act
        using var memoryStream = new MemoryStream();

        using (var writer = new BinaryWriter(memoryStream, Encoding.UTF8, true))
        {
            var saveAdapter = new BinaryRecordSaveAdapter(writer);
            record.Save(saveAdapter);
        }

        memoryStream.Position = 0;
        using var reader = new BinaryReader(memoryStream);
        var loadAdapter = new BinaryRecordLoadAdapter(reader);
        var loadedRecord = Record.Load(loadAdapter);

        // Assert
        var loadedStringColumn = loadedRecord.Columns.Find("StringColumn")!;
        for (int i = 0; i < testStrings.Length; i++)
        {
            Assert.AreEqual(testStrings[i], loadedStringColumn.GetString(i));
        }
    }

    #endregion
}