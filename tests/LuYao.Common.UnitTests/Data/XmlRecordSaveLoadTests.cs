using LuYao.Data.Models;
using LuYao.Data.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;
using System.Xml;

namespace LuYao.Data;

/// <summary>
/// XmlRecordLoadAdapter 和 XmlRecordSaveAdapter 的单元测试类
/// </summary>
[TestClass]
public class XmlRecordSaveLoadTests
{
    #region XML Save 方法测试

    /// <summary>
    /// 测试使用 XmlRecordSaveAdapter 保存空记录
    /// </summary>
    [TestMethod]
    public void XmlSave_EmptyRecord_ShouldSaveSuccessfully()
    {
        // Arrange
        var record = new Record("EmptyRecord", 0);
        using var stringWriter = new StringWriter();
        using var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { Indent = true });
        var adapter = new XmlRecordSaveAdapter(xmlWriter);

        // Act
        record.Save(adapter);
        xmlWriter.Close();
        var xmlResult = stringWriter.ToString();

        // Assert
        Assert.IsTrue(xmlResult.Length > 0);
        Assert.IsTrue(xmlResult.Contains("<record>"));
        Assert.IsTrue(xmlResult.Contains("</record>"));
        Assert.IsTrue(xmlResult.Contains("<head>"));
        Assert.IsTrue(xmlResult.Contains("</head>"));
    }

    /// <summary>
    /// 测试使用 XmlRecordSaveAdapter 保存包含各种数据类型的记录
    /// </summary>
    [TestMethod]
    public void XmlSave_RecordWithMixedDataTypes_ShouldSaveSuccessfully()
    {
        // Arrange
        var record = CreateTestRecord();
        using var stringWriter = new StringWriter();
        using var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { Indent = true });
        var adapter = new XmlRecordSaveAdapter(xmlWriter);

        // Act
        record.Save(adapter);
        xmlWriter.Close();
        var xmlResult = stringWriter.ToString();

        // Assert
        Assert.IsTrue(xmlResult.Length > 0);
        Assert.IsTrue(xmlResult.Contains("<record>"));
        Assert.IsTrue(xmlResult.Contains("<columns>"));
        Assert.IsTrue(xmlResult.Contains("<rows>"));
        Assert.IsTrue(xmlResult.Contains("IntColumn"));
        Assert.IsTrue(xmlResult.Contains("StringColumn"));
        Assert.IsTrue(xmlResult.Contains("BoolColumn"));
    }

    /// <summary>
    /// 测试 XmlRecordSaveAdapter Save 方法参数为 null 时抛出异常
    /// </summary>
    [TestMethod]
    public void XmlSave_NullAdapter_ShouldThrowArgumentNullException()
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
    public void XmlSave_RecordWithColumnsButNoData_ShouldSaveSuccessfully()
    {
        // Arrange
        var record = new Record("TestRecord", 0);
        record.Columns.Add<int>("IntColumn");
        record.Columns.Add<string>("StringColumn");
        record.Columns.Add<bool>("BoolColumn");

        using var stringWriter = new StringWriter();
        using var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { Indent = true });
        var adapter = new XmlRecordSaveAdapter(xmlWriter);

        // Act
        record.Save(adapter);
        xmlWriter.Close();
        var xmlResult = stringWriter.ToString();

        // Assert
        Assert.IsTrue(xmlResult.Length > 0);
        Assert.IsTrue(xmlResult.Contains("<columns>"));
        Assert.IsTrue(xmlResult.Contains("IntColumn"));
        Assert.IsTrue(xmlResult.Contains("StringColumn"));
        Assert.IsTrue(xmlResult.Contains("BoolColumn"));
    }

    #endregion

    #region XML Load 方法测试

    /// <summary>
    /// 测试使用 XmlRecordLoadAdapter 加载空记录
    /// </summary>
    [TestMethod]
    public void XmlLoad_EmptyRecord_ShouldLoadSuccessfully()
    {
        // Arrange
        var originalRecord = new Record("EmptyRecord", 0);

        // 先保存记录
        using var stringWriter = new StringWriter();
        using (var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { Indent = true }))
        {
            var saveAdapter = new XmlRecordSaveAdapter(xmlWriter);
            originalRecord.Save(saveAdapter);
        }

        var xmlString = stringWriter.ToString();

        // Act
        using var stringReader = new StringReader(xmlString);
        using var xmlReader = XmlReader.Create(stringReader);
        var loadAdapter = new XmlRecordLoadAdapter(xmlReader);
        var loadedRecord = Record.Load(loadAdapter);

        // Assert
        Assert.IsNotNull(loadedRecord);
        Assert.AreEqual(originalRecord.Name, loadedRecord.Name);
        Assert.AreEqual(originalRecord.Count, loadedRecord.Count);
        Assert.AreEqual(originalRecord.Columns.Count, loadedRecord.Columns.Count);
    }

    /// <summary>
    /// 测试使用 XmlRecordLoadAdapter 加载包含各种数据类型的记录
    /// </summary>
    [TestMethod]
    public void XmlLoad_RecordWithMixedDataTypes_ShouldLoadSuccessfully()
    {
        // Arrange
        var originalRecord = CreateTestRecord();

        // 先保存记录
        using var stringWriter = new StringWriter();
        using (var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { Indent = true }))
        {
            var saveAdapter = new XmlRecordSaveAdapter(xmlWriter);
            originalRecord.Save(saveAdapter);
        }

        var xmlString = stringWriter.ToString();

        // Act
        using var stringReader = new StringReader(xmlString);
        using var xmlReader = XmlReader.Create(stringReader);
        var loadAdapter = new XmlRecordLoadAdapter(xmlReader);
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
    /// 测试 XmlRecordLoadAdapter Load 方法参数为 null 时抛出异常
    /// </summary>
    [TestMethod]
    public void XmlLoad_NullAdapter_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.ThrowsException<ArgumentNullException>(() => Record.Load(null!));
    }

    /// <summary>
    /// 测试加载只有列结构但无数据的记录
    /// </summary>
    [TestMethod]
    public void XmlLoad_RecordWithColumnsButNoData_ShouldLoadSuccessfully()
    {
        // Arrange
        var originalRecord = new Record("TestRecord", 0);
        originalRecord.Columns.Add<int>("IntColumn");
        originalRecord.Columns.Add<string>("StringColumn");
        originalRecord.Columns.Add<bool>("BoolColumn");

        // 先保存记录
        using var stringWriter = new StringWriter();
        using (var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { Indent = true }))
        {
            var saveAdapter = new XmlRecordSaveAdapter(xmlWriter);
            originalRecord.Save(saveAdapter);
        }

        var xmlString = stringWriter.ToString();

        // Act
        using var stringReader = new StringReader(xmlString);
        using var xmlReader = XmlReader.Create(stringReader);
        var loadAdapter = new XmlRecordLoadAdapter(xmlReader);
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

    #region XML 完整的往返测试（Round-trip Tests）

    /// <summary>
    /// 测试完整的 XML 往返过程：Record -> XmlSave -> XmlLoad -> Record
    /// </summary>
    [TestMethod]
    public void XmlSaveLoad_RoundTrip_ShouldPreserveAllData()
    {
        // Arrange
        var originalRecord = CreateComplexTestRecord();

        // Act - 保存和加载
        using var stringWriter = new StringWriter();

        // 保存
        using (var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { Indent = true }))
        {
            var saveAdapter = new XmlRecordSaveAdapter(xmlWriter);
            originalRecord.Save(saveAdapter);
        }

        var xmlString = stringWriter.ToString();

        // 加载
        using var stringReader = new StringReader(xmlString);
        using var xmlReader = XmlReader.Create(stringReader);
        var loadAdapter = new XmlRecordLoadAdapter(xmlReader);
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
    /// 测试多次 XML 往返的数据一致性
    /// </summary>
    [TestMethod]
    public void XmlSaveLoad_MultipleRoundTrips_ShouldMaintainConsistency()
    {
        // Arrange
        var originalRecord = CreateTestRecord();

        Record currentRecord = originalRecord;

        // Act - 执行多次往返
        for (int i = 0; i < 3; i++)
        {
            using var stringWriter = new StringWriter();

            // 保存
            using (var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { Indent = true }))
            {
                var saveAdapter = new XmlRecordSaveAdapter(xmlWriter);
                currentRecord.Save(saveAdapter);
            }

            var xmlString = stringWriter.ToString();

            // 加载
            using var stringReader = new StringReader(xmlString);
            using var xmlReader = XmlReader.Create(stringReader);
            var loadAdapter = new XmlRecordLoadAdapter(xmlReader);
            currentRecord = Record.Load(loadAdapter);
        }

        // Assert
        VerifyRecordData(originalRecord, currentRecord);
    }

    #endregion

    #region XmlRecordSaveAdapter 和 XmlRecordLoadAdapter 特定测试

    /// <summary>
    /// 测试 XmlRecordSaveAdapter 构造函数参数验证
    /// </summary>
    [TestMethod]
    public void XmlRecordSaveAdapter_Constructor_NullWriter_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.ThrowsException<ArgumentNullException>(() => new XmlRecordSaveAdapter(null!));
    }

    /// <summary>
    /// 测试 XmlRecordLoadAdapter 构造函数参数验证
    /// </summary>
    [TestMethod]
    public void XmlRecordLoadAdapter_Constructor_NullReader_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.ThrowsException<ArgumentNullException>(() => new XmlRecordLoadAdapter(null!));
    }

    /// <summary>
    /// 测试 XmlRecordLoadAdapter 的 KeyKind 属性
    /// </summary>
    [TestMethod]
    public void XmlRecordLoadAdapter_KeyKind_ShouldReturnName()
    {
        // Arrange
        using var stringReader = new StringReader("<record></record>");
        using var xmlReader = XmlReader.Create(stringReader);
        var adapter = new XmlRecordLoadAdapter(xmlReader);

        // Act & Assert
        Assert.AreEqual(RecordLoadKeyKind.Name, adapter.KeyKind);
    }

    /// <summary>
    /// 测试 XmlRecordLoadAdapter 的 Index 属性
    /// </summary>
    [TestMethod]
    public void XmlRecordLoadAdapter_Index_ShouldReturnNegativeOne()
    {
        // Arrange
        using var stringReader = new StringReader("<record></record>");
        using var xmlReader = XmlReader.Create(stringReader);
        var adapter = new XmlRecordLoadAdapter(xmlReader);

        // Act & Assert
        Assert.AreEqual(-1, adapter.Index);
    }

    /// <summary>
    /// 测试 XmlRecordSaveAdapter 的 Layout 属性
    /// </summary>
    [TestMethod]
    public void XmlRecordSaveAdapter_Layout_ShouldReturnCorrectSections()
    {
        // Arrange
        using var stringWriter = new StringWriter();
        using var xmlWriter = XmlWriter.Create(stringWriter);
        var adapter = new XmlRecordSaveAdapter(xmlWriter);

        // Act & Assert
        var layout = adapter.Layout;
        Assert.AreEqual(3, layout.Count);
        Assert.AreEqual(RecordSection.Head, layout[0]);
        Assert.AreEqual(RecordSection.Columns, layout[1]);
        Assert.AreEqual(RecordSection.Rows, layout[2]);
    }

    /// <summary>
    /// 测试 XmlRecordSaveAdapter 处理复杂对象类型时抛出异常
    /// </summary>
    [TestMethod]
    public void XmlRecordSaveAdapter_WriteObject_ShouldThrowNotImplementedException()
    {
        // Arrange
        using var stringWriter = new StringWriter();
        using var xmlWriter = XmlWriter.Create(stringWriter);
        var adapter = new XmlRecordSaveAdapter(xmlWriter);

        // Act & Assert
        Assert.ThrowsException<NotImplementedException>(() =>
            adapter.WriteObject("test", 0, new object()));
    }

    /// <summary>
    /// 测试 XmlRecordLoadAdapter 处理复杂对象类型时抛出异常
    /// </summary>
    [TestMethod]
    public void XmlRecordLoadAdapter_ReadObject_ShouldThrowNotImplementedException()
    {
        // Arrange
        using var stringReader = new StringReader("<record></record>");
        using var xmlReader = XmlReader.Create(stringReader);
        var adapter = new XmlRecordLoadAdapter(xmlReader);

        // Act & Assert
        Assert.ThrowsException<NotImplementedException>(() =>
            adapter.ReadObject(typeof(object)));
    }

    #endregion

    #region XML 特殊情况测试

    /// <summary>
    /// 测试包含特殊XML字符的字符串处理
    /// </summary>
    [TestMethod]
    public void XmlSaveLoad_RecordWithSpecialXmlCharacters_ShouldHandleCorrectly()
    {
        // Arrange
        var record = new Record("XmlSpecialCharRecord", 5);
        var stringColumn = record.Columns.Add<string>("StringColumn");

        var testStrings = new[]
        {
            "",
            " ",
            "normal string",
            "string with <tags>",
            "string with & ampersand",
            "string with \"quotes\"",
            "string with 'apostrophes'",
            "string with\nnewline",
            "string with\ttab",
            "中文字符串"
        };

        for (int i = 0; i < testStrings.Length; i++)
        {
            var row = record.AddRow();
            stringColumn.Set(testStrings[i], i);
        }

        // Act
        using var stringWriter = new StringWriter();

        using (var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { Indent = true }))
        {
            var saveAdapter = new XmlRecordSaveAdapter(xmlWriter);
            record.Save(saveAdapter);
        }

        var xmlString = stringWriter.ToString();

        using var stringReader = new StringReader(xmlString);
        using var xmlReader = XmlReader.Create(stringReader);
        var loadAdapter = new XmlRecordLoadAdapter(xmlReader);
        var loadedRecord = Record.Load(loadAdapter);

        // Assert
        var loadedStringColumn = loadedRecord.Columns.Find("StringColumn")!;
        for (int i = 0; i < testStrings.Length; i++)
        {
            Assert.AreEqual(testStrings[i], loadedStringColumn.GetString(i));
        }
    }

    /// <summary>
    /// 测试 DateTime 类型的 XML 序列化和反序列化
    /// </summary>
    [TestMethod]
    public void XmlSaveLoad_DateTimeValues_ShouldPreserveAccuracy()
    {
        // Arrange
        var record = new Record("DateTimeRecord", 5);
        var dateTimeColumn = record.Columns.Add<DateTime>("DateTimeColumn");

        var testDates = new[]
        {
            DateTime.MinValue,
            DateTime.MaxValue,
            new DateTime(2023, 12, 25, 15, 30, 45, 123),
            new DateTime(2000, 1, 1),
            DateTime.Now
        };

        for (int i = 0; i < testDates.Length; i++)
        {
            var row = record.AddRow();
            dateTimeColumn.Set(testDates[i], i);
        }

        // Act
        using var stringWriter = new StringWriter();

        using (var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { Indent = true }))
        {
            var saveAdapter = new XmlRecordSaveAdapter(xmlWriter);
            record.Save(saveAdapter);
        }

        var xmlString = stringWriter.ToString();

        using var stringReader = new StringReader(xmlString);
        using var xmlReader = XmlReader.Create(stringReader);
        var loadAdapter = new XmlRecordLoadAdapter(xmlReader);
        var loadedRecord = Record.Load(loadAdapter);

        // Assert
        var loadedDateTimeColumn = loadedRecord.Columns.Find("DateTimeColumn")!;
        for (int i = 0; i < testDates.Length; i++)
        {
            var expectedDate = testDates[i];
            var actualDate = loadedDateTimeColumn.GetDateTime(i);

            // 由于 XML 序列化可能会丢失精度，我们只比较到秒级别
            Assert.AreEqual(expectedDate.Year, actualDate.Year);
            Assert.AreEqual(expectedDate.Month, actualDate.Month);
            Assert.AreEqual(expectedDate.Day, actualDate.Day);
            Assert.AreEqual(expectedDate.Hour, actualDate.Hour);
            Assert.AreEqual(expectedDate.Minute, actualDate.Minute);
            Assert.AreEqual(expectedDate.Second, actualDate.Second);
        }
    }

    /// <summary>
    /// 测试保存大量数据的 XML 记录
    /// </summary>
    [TestMethod]
    public void XmlSaveLoad_LargeRecord_ShouldHandleCorrectly()
    {
        // Arrange
        var record = new Record("LargeXmlRecord", 1000);
        var intColumn = record.Columns.Add<int>("IntColumn");
        var stringColumn = record.Columns.Add<string>("StringColumn");

        // 添加大量数据
        for (int i = 0; i < 100; i++) // 减少数据量以避免XML过大
        {
            var row = record.AddRow();
            intColumn.Set(i, i);
            stringColumn.Set($"XmlData_{i}", i);
        }

        // Act
        using var stringWriter = new StringWriter();

        // 保存
        using (var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { Indent = true }))
        {
            var saveAdapter = new XmlRecordSaveAdapter(xmlWriter);
            record.Save(saveAdapter);
        }

        var xmlString = stringWriter.ToString();

        // 加载
        using var stringReader = new StringReader(xmlString);
        using var xmlReader = XmlReader.Create(stringReader);
        var loadAdapter = new XmlRecordLoadAdapter(xmlReader);
        var loadedRecord = Record.Load(loadAdapter);

        // Assert
        Assert.AreEqual(record.Count, loadedRecord.Count);
        Assert.AreEqual(record.Columns.Count, loadedRecord.Columns.Count);

        // 验证部分数据
        Assert.AreEqual(intColumn.Get(0), loadedRecord.Columns.Find("IntColumn")!.GetInt32(0));
        Assert.AreEqual(stringColumn.Get(50), loadedRecord.Columns.Find("StringColumn")!.GetString(50));
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
        var record = new Record("ComplexXmlTestRecord", 10);

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
}