using System;
using System.Collections.Generic;
using LuYao.Data.Meta;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LuYao.Data;

/// <summary>
/// 测试 <see cref="RecordRow"/> 的构造、字段访问、赋值与 dynamic 行为。
/// </summary>
[TestClass]
public class RecordRowTests
{
    /// <summary>
    /// 创建包含整型、字符串和布尔列的测试记录。
    /// </summary>
    private (Record record, RecordColumn<int> intColumn, RecordColumn<string> stringColumn, RecordColumn<bool> boolColumn) CreateTestRecord()
    {
        var record = new Record("TestTable", 5);
        var intColumn = record.Columns.Add<int>("IntColumn");
        var stringColumn = record.Columns.Add<string>("StringColumn");
        var boolColumn = record.Columns.Add<bool>("BoolColumn");

        var row1 = record.AddRow();
        var row2 = record.AddRow();

        intColumn.SetValue(0, 100);
        intColumn.SetValue(1, 200);
        stringColumn.SetValue(0, "Test1");
        stringColumn.SetValue(1, "Test2");
        boolColumn.SetValue(0, true);
        boolColumn.SetValue(1, false);

        return (record, intColumn, stringColumn, boolColumn);
    }


    /// <summary>
    /// 使用有效参数构造时，应正确初始化记录与行号。
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
    /// 当记录为 null 时，应抛出 <see cref="ArgumentNullException"/>。
    /// </summary>
    [TestMethod]
    public void Constructor_NullRecord_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new RecordRow(null!, 0));
    }

    /// <summary>
    /// 当行索引为负数时，应抛出 <see cref="ArgumentOutOfRangeException"/>。
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
    /// 当行索引超出记录范围时，应抛出 <see cref="ArgumentOutOfRangeException"/>。
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



    /// <summary>
    /// <see cref="RecordRow.Record"/> 应返回所属的 <see cref="Record"/>。
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
    /// <see cref="RecordRow.Row"/> 应返回当前行号。
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



    /// <summary>
    /// 隐式转换为 <see cref="int"/> 时，应返回当前行号。
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



    /// <summary>
    /// 按列名读取已存在的布尔列时，应返回正确值。
    /// </summary>
    [TestMethod]
    public void GetBoolean_ByName_ColumnExists_ShouldReturnCorrectValue()
    {
        // Arrange
        var (record, _, _, boolColumn) = CreateTestRecord();
        var recordRow = new RecordRow(record, 0);

        // Act
        var result = recordRow.To<bool>("BoolColumn");

        // Assert
        Assert.AreEqual(true, result);
    }

    /// <summary>
    /// 按列名读取不存在的布尔列时，应返回默认值。
    /// </summary>
    [TestMethod]
    public void GetBoolean_ByName_ColumnNotExists_ShouldReturnDefault()
    {
        // Arrange
        var (record, _, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(record, 0);

        // Act
        var result = recordRow.To<bool>("NonExistentColumn");

        // Assert
        Assert.AreEqual(default(bool), result);
    }

    /// <summary>
    /// 按列名读取已存在的字符串列时，应返回正确值。
    /// </summary>
    [TestMethod]
    public void GetString_ByName_ColumnExists_ShouldReturnCorrectValue()
    {
        // Arrange
        var (record, _, stringColumn, _) = CreateTestRecord();
        var recordRow = new RecordRow(record, 1);

        // Act
        var result = recordRow.To<string>("StringColumn");

        // Assert
        Assert.AreEqual("Test2", result);
    }

    /// <summary>
    /// 按列名读取不存在的字符串列时，应返回默认值。
    /// </summary>
    [TestMethod]
    public void GetString_ByName_ColumnNotExists_ShouldReturnDefault()
    {
        // Arrange
        var (record, _, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(record, 0);

        // Act
        var result = recordRow.To<string>("NonExistentColumn");

        // Assert
        Assert.AreEqual(default(string), result);
    }

    /// <summary>
    /// 按列名读取已存在的整型列时，应返回正确值。
    /// </summary>
    [TestMethod]
    public void GetInt32_ByName_ColumnExists_ShouldReturnCorrectValue()
    {
        // Arrange
        var (record, intColumn, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(record, 1);

        // Act
        var result = recordRow.To<int>("IntColumn");

        // Assert
        Assert.AreEqual(200, result);
    }

    /// <summary>
    /// 按列名读取不存在的整型列时，应返回默认值。
    /// </summary>
    [TestMethod]
    public void GetInt32_ByName_ColumnNotExists_ShouldReturnDefault()
    {
        // Arrange
        var (record, _, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(record, 0);

        // Act
        var result = recordRow.To<int>("NonExistentColumn");

        // Assert
        Assert.AreEqual(default(int), result);
    }

    /// <summary>
    /// 泛型按列名读取已存在列时，应返回正确值。
    /// </summary>
    [TestMethod]
    public void GetGeneric_ByName_ColumnExists_ShouldReturnCorrectValue()
    {
        // Arrange
        var (record, intColumn, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(record, 0);

        // Act
        var result = recordRow.To<int>("IntColumn");

        // Assert
        Assert.AreEqual(100, result);
    }

    /// <summary>
    /// 泛型按列名读取不存在列时，应返回默认值。
    /// </summary>
    [TestMethod]
    public void GetGeneric_ByName_ColumnNotExists_ShouldReturnDefault()
    {
        // Arrange
        var (record, _, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(record, 0);

        // Act
        var result = recordRow.To<int>("NonExistentColumn");

        // Assert
        Assert.AreEqual(default(int), result);
    }

    /// <summary>
    /// 应支持按列名读取字节值。
    /// </summary>
    [TestMethod]
    public void GetByte_ByName_ShouldWork()
    {
        // Arrange
        var record = new Record("TestTable", 1);
        var byteColumn = record.Columns.Add<byte>("ByteColumn");
        var row = record.AddRow();
        byteColumn.Set(row.Row, 255);
        var recordRow = new RecordRow(record, 0);

        // Act
        var result = recordRow.To<byte>("ByteColumn");

        // Assert
        Assert.AreEqual((byte)255, result);
    }

    /// <summary>
    /// 应支持按列名读取双精度浮点值。
    /// </summary>
    [TestMethod]
    public void GetDouble_ByName_ShouldWork()
    {
        // Arrange
        var record = new Record("TestTable", 1);
        var doubleColumn = record.Columns.Add<double>("DoubleColumn");
        var row = record.AddRow();
        doubleColumn.Set(row.Row, 3.14159);
        var recordRow = new RecordRow(record, 0);

        // Act
        var result = recordRow.To<double>("DoubleColumn");

        // Assert
        Assert.AreEqual(3.14159, result, 0.00001);
    }

    /// <summary>
    /// 应支持按列名读取日期时间值。
    /// </summary>
    [TestMethod]
    public void GetDateTime_ByName_ShouldWork()
    {
        // Arrange
        var record = new Record("TestTable", 1);
        var dateTimeColumn = record.Columns.Add<DateTime>("DateTimeColumn");
        var testDate = new DateTime(2023, 8, 15, 14, 30, 0);
        var row = record.AddRow();
        dateTimeColumn.Set(row.Row, testDate);
        var recordRow = new RecordRow(record, 0);

        // Act
        var result = recordRow.To<DateTime>("DateTimeColumn");

        // Assert
        Assert.AreEqual(testDate, result);
    }



    /// <summary>
    /// 当列名为空时，各类型读取方法应返回默认值。
    /// </summary>
    [TestMethod]
    public void GetMethods_EmptyColumnName_ShouldReturnDefault()
    {
        // Arrange
        var (record, _, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(record, 0);

        // Act & Assert
        Assert.AreEqual(default(int), recordRow.To<int>(""));
        Assert.AreEqual(default(string), recordRow.To<string>(""));
        Assert.AreEqual(default(bool), recordRow.To<bool>(""));
    }

    /// <summary>
    /// 在多行数据场景下，应能读取到各行的正确值。
    /// </summary>
    [TestMethod]
    public void GetMethods_MultipleRows_ShouldReturnCorrectValues()
    {
        // Arrange
        var (record, intColumn, stringColumn, boolColumn) = CreateTestRecord();

        var row3 = record.AddRow();
        intColumn.Set(row3.Row, 300);
        stringColumn.Set(row3.Row, "Test3");
        boolColumn.Set(row3.Row, true);

        var recordRow0 = new RecordRow(record, 0);
        var recordRow1 = new RecordRow(record, 1);
        var recordRow2 = new RecordRow(record, 2);

        // Act & Assert
        Assert.AreEqual(100, recordRow0.To<int>("IntColumn"));
        Assert.AreEqual(200, recordRow1.To<int>("IntColumn"));
        Assert.AreEqual(300, recordRow2.To<int>("IntColumn"));

        Assert.AreEqual("Test1", recordRow0.To<string>("StringColumn"));
        Assert.AreEqual("Test2", recordRow1.To<string>("StringColumn"));
        Assert.AreEqual("Test3", recordRow2.To<string>("StringColumn"));

        Assert.AreEqual(true, recordRow0.To<bool>("BoolColumn"));
        Assert.AreEqual(false, recordRow1.To<bool>("BoolColumn"));
        Assert.AreEqual(true, recordRow2.To<bool>("BoolColumn"));
    }



    /// <summary>
    /// 多次访问同一字段时，应保持结果一致。
    /// </summary>
    [TestMethod]
    public void GetMethods_RepeatedAccess_ShouldReturnConsistentResults()
    {
        // Arrange
        var (record, _, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(record, 0);

        // Act
        var result1 = recordRow.To<int>("IntColumn");
        var result2 = recordRow.To<int>("IntColumn");

        // Assert
        Assert.AreEqual(result1, result2);
        Assert.AreEqual(100, result1);
    }



    /// <summary>
    /// 使用索引器更新已存在的整型列时，应写入成功。
    /// </summary>
    [TestMethod]
    public void Set_TypedColumn_ShouldUpdateValue()
    {
        // Arrange
        var (record, intColumn, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(record, 0);

        // Act
        recordRow["IntColumn"] = 999;

        // Assert
        Assert.AreEqual(999, recordRow.To<int>("IntColumn"));
    }

    /// <summary>
    /// 使用索引器更新已存在的字符串列时，应写入成功。
    /// </summary>
    [TestMethod]
    public void Set_StringColumn_ShouldUpdateValue()
    {
        // Arrange
        var (record, _, stringColumn, _) = CreateTestRecord();
        var recordRow = new RecordRow(record, 1);

        // Act
        recordRow["StringColumn"] = "NewValue";

        // Assert
        Assert.AreEqual("NewValue", recordRow.To<string>("StringColumn"));
    }

    /// <summary>
    /// 当列不存在时，<see cref="RecordRow.Set{T}(string, T)"/> 应自动创建对应列。
    /// </summary>
    [TestMethod]
    public void Set_ColumnNotExists_ShouldAutoCreateColumn()
    {
        // Arrange
        var (record, _, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(record, 0);
        int beforeCount = record.Columns.Count;

        // Act
        recordRow["NewColumn"] = 12345;

        // Assert
        Assert.AreEqual(beforeCount + 1, record.Columns.Count);
        var col = record.Columns.Find("NewColumn");
        Assert.IsNotNull(col);
        Assert.AreEqual(typeof(int), col.Type);
        Assert.AreEqual(12345, recordRow.To<int>("NewColumn"));
    }

    /// <summary>
    /// 写入后再读取时，应得到与写入一致的值。
    /// </summary>
    [TestMethod]
    public void Set_ThenReadViaIndexer_ShouldBeConsistent()
    {
        // Arrange
        var (record, intColumn, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(record, 0);

        // Act
        recordRow["IntColumn"] = 42;

        // Assert
        Assert.AreEqual(42, recordRow.To<int>("IntColumn"));
    }



    /// <summary>
    /// dynamic 按成员名读取时，应返回正确值。
    /// </summary>
    [TestMethod]
    public void Dynamic_GetMember_ShouldReturnCorrectValue()
    {
        // Arrange
        var (record, _, stringColumn, _) = CreateTestRecord();
        dynamic row = new RecordRow(record, 0);

        // Act
        var result = row.StringColumn;

        // Assert
        Assert.AreEqual("Test1", result);
    }

    /// <summary>
    /// dynamic 按成员名写入时，应更新底层记录值。
    /// </summary>
    [TestMethod]
    public void Dynamic_SetMember_ShouldUpdateValue()
    {
        // Arrange
        var (record, _, stringColumn, _) = CreateTestRecord();
        dynamic row = new RecordRow(record, 0);

        // Act
        row.StringColumn = "DynValue";

        // Assert
        var recordRow = new RecordRow(record, 0);
        Assert.AreEqual("DynValue", recordRow.To<string>("StringColumn"));
    }

    /// <summary>
    /// dynamic 按索引器读取时，应返回正确值。
    /// </summary>
    [TestMethod]
    public void Dynamic_GetIndex_ShouldReturnCorrectValue()
    {
        // Arrange
        var (record, intColumn, _, _) = CreateTestRecord();
        dynamic row = new RecordRow(record, 1);

        // Act - dynamic 索引读取
        var result = row["IntColumn"];

        // Assert
        Assert.AreEqual(200, (int)result!);
    }

    /// <summary>
    /// dynamic 按索引器写入时，应更新底层记录值。
    /// </summary>
    [TestMethod]
    public void Dynamic_SetIndex_ShouldUpdateValue()
    {
        // Arrange
        var (record, intColumn, _, _) = CreateTestRecord();
        dynamic row = new RecordRow(record, 0);

        // Act - dynamic 索引写入
        row["IntColumn"] = 777;

        // Assert
        var recordRow = new RecordRow(record, 0);
        Assert.AreEqual(777, recordRow.To<int>("IntColumn"));
    }

    /// <summary>
    /// dynamic 读取不存在的成员时，应返回 null。
    /// </summary>
    [TestMethod]
    public void Dynamic_GetMember_ColumnNotExists_ShouldReturnNull()
    {
        // Arrange
        var (record, _, _, _) = CreateTestRecord();
        dynamic row = new RecordRow(record, 0);

        // Act
        var result = row.NoSuchColumn;

        // Assert
        Assert.IsNull(result);
    }

    /// <summary>
    /// dynamic 写入不存在的成员时，不应抛出异常。
    /// </summary>
    [TestMethod]
    public void Dynamic_SetMember_ColumnNotExists_ShouldNotThrow()
    {
        // Arrange
        var (record, _, _, _) = CreateTestRecord();
        dynamic row = new RecordRow(record, 0);

        // Act & Assert
        row.NoSuchColumn = "ignored"; // should not throw
    }

    /// <summary>
    /// dynamic 成员读取结果应与显式调用字段读取方法一致。
    /// </summary>
    [TestMethod]
    public void Dynamic_GetMember_ShouldBeConsistentWithGetMethod()
    {
        // Arrange
        var (record, intColumn, _, boolColumn) = CreateTestRecord();
        dynamic row = new RecordRow(record, 0);
        var recordRow = new RecordRow(record, 0);

        // Act & Assert
        Assert.AreEqual(recordRow.To<int>("IntColumn"), (int)row.IntColumn!);
        Assert.AreEqual(recordRow.To<bool>("BoolColumn"), (bool)row.BoolColumn!);
    }

    /// <summary>
    /// 索引器按列名读取已存在列时，应返回对应对象值。
    /// </summary>
    [TestMethod]
    public void FieldObject_ByName_ColumnExists_ShouldReturnValue()
    {
        // Arrange
        var (record, _, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(record, 0);

        // Act
        var result = recordRow["IntColumn"];

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(100, (int)result!);
    }

    /// <summary>
    /// 按列名读取不存在列时，索引器应返回 null。
    /// </summary>
    [TestMethod]
    public void FieldObject_ByName_ColumnNotExists_ShouldReturnNull()
    {
        // Arrange
        var (record, _, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(record, 0);

        // Act
        var result = recordRow["NonExistentColumn"];

        // Assert
        Assert.IsNull(result);
    }

    /// <summary>
    /// <see cref="RecordRow.ToDictionary"/> 应返回当前行的全部列与对应值。
    /// </summary>
    [TestMethod]
    public void ToDictionary_ShouldReturnAllColumnsWithCurrentRowValues()
    {
        // Arrange
        var (record, _, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(record, 1);

        // Act
        var result = recordRow.ToDictionary();

        // Assert
        Assert.AreEqual(3, result.Count);
        Assert.AreEqual(200, (int)result["IntColumn"]!);
        Assert.AreEqual("Test2", (string)result["StringColumn"]!);
        Assert.AreEqual(false, (bool)result["BoolColumn"]!);
    }

    /// <summary>
    /// <see cref="RecordRow.ToDictionary"/> 对于 null 列值应保留 null。
    /// </summary>
    [TestMethod]
    public void ToDictionary_WhenColumnValueIsNull_ShouldKeepNullValue()
    {
        // Arrange
        var record = new Record("TestTable", 2);
        var nullableColumn = record.Columns.Add<string>("NullableColumn");
        record.AddRow();
        nullableColumn.SetValue(0, null);
        var recordRow = new RecordRow(record, 0);

        // Act
        var result = recordRow.ToDictionary();

        // Assert
        Assert.AreEqual(1, result.Count);
        Assert.IsTrue(result.ContainsKey("NullableColumn"));
        Assert.IsNull(result["NullableColumn"]);
    }

    /// <summary>
    /// <see cref="RecordRow.ToString"/> 应输出行号以及字典风格的列值信息。
    /// </summary>
    [TestMethod]
    public void ToString_ShouldContainRowAndDictionaryLikeValues()
    {
        // Arrange
        var (record, _, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(record, 1);

        // Act
        var result = recordRow.ToString();

        // Assert
        Assert.AreEqual("{ Row = 1, Data = { IntColumn = 200, StringColumn = Test2, BoolColumn = False } }", result);
    }

    /// <summary>
    /// <see cref="RecordRow.ToString"/> 对于 null 值应按匿名类型风格输出空值。
    /// </summary>
    [TestMethod]
    public void ToString_WhenValueIsNull_ShouldRenderEmptyValue()
    {
        // Arrange
        var record = new Record("TestTable", 2);
        var nullableColumn = record.Columns.Add<string>("NullableColumn");
        record.AddRow();
        nullableColumn.SetValue(0, null);
        var recordRow = new RecordRow(record, 0);

        // Act
        var result = recordRow.ToString();

        // Assert
        Assert.AreEqual("{ Row = 0, Data = { NullableColumn =  } }", result);
    }
}
