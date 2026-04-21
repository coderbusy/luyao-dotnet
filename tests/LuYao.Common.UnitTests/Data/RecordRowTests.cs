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

        intColumn.Set(100, 0);
        intColumn.Set(200, 1);
        stringColumn.Set("Test1", 0);
        stringColumn.Set("Test2", 1);
        boolColumn.Set(true, 0);
        boolColumn.Set(false, 1);

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
        var result = recordRow.Field<Boolean>("BoolColumn");

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
        var result = recordRow.Field<Boolean>("NonExistentColumn");

        // Assert
        Assert.AreEqual(default(bool), result);
    }

    /// <summary>
    /// 使用同一记录上的列对象读取布尔值时，应返回正确值。
    /// </summary>
    [TestMethod]
    public void GetBoolean_ByColumn_SameRecord_ShouldReturnCorrectValue()
    {
        // Arrange
        var (record, _, _, boolColumn) = CreateTestRecord();
        var recordRow = new RecordRow(record, 1);

        // Act
        var result = recordRow.Field<Boolean>(boolColumn);

        // Assert
        Assert.AreEqual(false, result);
    }

    /// <summary>
    /// 使用其他记录的同名列对象读取时，应回退到按列名查找。
    /// </summary>
    [TestMethod]
    public void GetBoolean_ByColumn_DifferentRecord_ShouldFallbackToNameSearch()
    {
        // Arrange
        var (record1, _, _, _) = CreateTestRecord();
        var (record2, _, _, boolColumn2) = CreateTestRecord();
        var recordRow = new RecordRow(record1, 0);

        // Act
        var result = recordRow.Field<Boolean>(boolColumn2);

        // Assert
        Assert.AreEqual(true, result);
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
        var result = recordRow.Field<String>("StringColumn");

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
        var result = recordRow.Field<String>("NonExistentColumn");

        // Assert
        Assert.AreEqual(default(string), result);
    }

    /// <summary>
    /// 使用同一记录上的列对象读取字符串值时，应返回正确值。
    /// </summary>
    [TestMethod]
    public void GetString_ByColumn_SameRecord_ShouldReturnCorrectValue()
    {
        // Arrange
        var (record, _, stringColumn, _) = CreateTestRecord();
        var recordRow = new RecordRow(record, 0);

        // Act
        var result = recordRow.Field<String>(stringColumn);

        // Assert
        Assert.AreEqual("Test1", result);
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
        var result = recordRow.Field<Int32>("IntColumn");

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
        var result = recordRow.Field<Int32>("NonExistentColumn");

        // Assert
        Assert.AreEqual(default(int), result);
    }

    /// <summary>
    /// 使用同一记录上的列对象读取整型值时，应返回正确值。
    /// </summary>
    [TestMethod]
    public void GetInt32_ByColumn_SameRecord_ShouldReturnCorrectValue()
    {
        // Arrange
        var (record, intColumn, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(record, 0);

        // Act
        var result = recordRow.Field<Int32>(intColumn);

        // Assert
        Assert.AreEqual(100, result);
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
        var result = recordRow.Field<int>("IntColumn");

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
        var result = recordRow.Field<int>("NonExistentColumn");

        // Assert
        Assert.AreEqual(default(int), result);
    }

    /// <summary>
    /// 泛型使用列对象读取值时，应返回正确值。
    /// </summary>
    [TestMethod]
    public void GetGeneric_ByColumn_SameRecord_ShouldReturnCorrectValue()
    {
        // Arrange
        var (record, intColumn, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(record, 1);

        // Act
        var result = recordRow.Field<int>(intColumn);

        // Assert
        Assert.AreEqual(200, result);
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
        byteColumn.Set(255, row.Row);
        var recordRow = new RecordRow(record, 0);

        // Act
        var result = recordRow.Field<Byte>("ByteColumn");

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
        doubleColumn.Set(3.14159, row.Row);
        var recordRow = new RecordRow(record, 0);

        // Act
        var result = recordRow.Field<Double>("DoubleColumn");

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
        dateTimeColumn.Set(testDate, row.Row);
        var recordRow = new RecordRow(record, 0);

        // Act
        var result = recordRow.Field<DateTime>("DateTimeColumn");

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
        Assert.AreEqual(default(int), recordRow.Field<Int32>(""));
        Assert.AreEqual(default(string), recordRow.Field<String>(""));
        Assert.AreEqual(default(bool), recordRow.Field<Boolean>(""));
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
        intColumn.Set(300, row3.Row);
        stringColumn.Set("Test3", row3.Row);
        boolColumn.Set(true, row3.Row);

        var recordRow0 = new RecordRow(record, 0);
        var recordRow1 = new RecordRow(record, 1);
        var recordRow2 = new RecordRow(record, 2);

        // Act & Assert
        Assert.AreEqual(100, recordRow0.Field<Int32>("IntColumn"));
        Assert.AreEqual(200, recordRow1.Field<Int32>("IntColumn"));
        Assert.AreEqual(300, recordRow2.Field<Int32>("IntColumn"));

        Assert.AreEqual("Test1", recordRow0.Field<String>("StringColumn"));
        Assert.AreEqual("Test2", recordRow1.Field<String>("StringColumn"));
        Assert.AreEqual("Test3", recordRow2.Field<String>("StringColumn"));

        Assert.AreEqual(true, recordRow0.Field<Boolean>("BoolColumn"));
        Assert.AreEqual(false, recordRow1.Field<Boolean>("BoolColumn"));
        Assert.AreEqual(true, recordRow2.Field<Boolean>("BoolColumn"));
    }



    /// <summary>
    /// 多次访问同一字段时，应保持结果一致。
    /// </summary>
    [TestMethod]
    public void GetMethods_RepeatedAccess_ShouldReturnConsistentResults()
    {
        // Arrange
        var (record, intColumn, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(record, 0);

        // Act
        var result1 = recordRow.Field<Int32>("IntColumn");
        var result2 = recordRow.Field<Int32>("IntColumn");
        var result3 = recordRow.Field<Int32>(intColumn);

        // Assert
        Assert.AreEqual(result1, result2);
        Assert.AreEqual(result2, result3);
        Assert.AreEqual(100, result1);
    }

    /// <summary>
    /// 按列名和按列对象读取时，应得到相同结果。
    /// </summary>
    [TestMethod]
    public void GetMethods_ByNameVsByColumn_ShouldReturnSameResults()
    {
        // Arrange
        var (record, intColumn, stringColumn, boolColumn) = CreateTestRecord();
        var recordRow = new RecordRow(record, 1);

        // Act & Assert
        Assert.AreEqual(recordRow.Field<Int32>("IntColumn"), recordRow.Field<Int32>(intColumn));
        Assert.AreEqual(recordRow.Field<String>("StringColumn"), recordRow.Field<String>(stringColumn));
        Assert.AreEqual(recordRow.Field<Boolean>("BoolColumn"), recordRow.Field<Boolean>(boolColumn));
    }



    /// <summary>
    /// 调用 <see cref="RecordRow.Set{T}(string, T)"/> 更新已存在的整型列时，应写入成功。
    /// </summary>
    [TestMethod]
    public void Set_TypedColumn_ShouldUpdateValue()
    {
        // Arrange
        var (record, intColumn, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(record, 0);

        // Act
        recordRow.Set("IntColumn", 999);

        // Assert
        Assert.AreEqual(999, recordRow.Field<int>("IntColumn"));
    }

    /// <summary>
    /// 调用 <see cref="RecordRow.Set{T}(string, T)"/> 更新已存在的字符串列时，应写入成功。
    /// </summary>
    [TestMethod]
    public void Set_StringColumn_ShouldUpdateValue()
    {
        // Arrange
        var (record, _, stringColumn, _) = CreateTestRecord();
        var recordRow = new RecordRow(record, 1);

        // Act
        recordRow.Set("StringColumn", "NewValue");

        // Assert
        Assert.AreEqual("NewValue", recordRow.Field<string>("StringColumn"));
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
        recordRow.Set("NewColumn", 12345);

        // Assert
        Assert.AreEqual(beforeCount + 1, record.Columns.Count);
        var col = record.Columns.Get("NewColumn");
        Assert.AreEqual(typeof(int), col.Type);
        Assert.AreEqual(12345, recordRow.Field<int>("NewColumn"));
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
        recordRow.Set("IntColumn", 42);

        // Assert
        Assert.AreEqual(42, recordRow.Field<int>("IntColumn"));
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
        Assert.AreEqual("DynValue", recordRow.Field<string>("StringColumn"));
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
        Assert.AreEqual(777, recordRow.Field<int>("IntColumn"));
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
        Assert.AreEqual(recordRow.Field<int>("IntColumn"), (int)row.IntColumn!);
        Assert.AreEqual(recordRow.Field<bool>("BoolColumn"), (bool)row.BoolColumn!);
    }

    /// <summary>
    /// <see cref="RecordRow.Field(string)"/> 按列名读取已存在列时，应返回对应对象值。
    /// </summary>
    [TestMethod]
    public void FieldObject_ByName_ColumnExists_ShouldReturnValue()
    {
        // Arrange
        var (record, _, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(record, 0);

        // Act
        var result = recordRow.Field("IntColumn");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(100, (int)result!);
    }

    /// <summary>
    /// <see cref="RecordRow.Field(string)"/> 按列名读取不存在列时，应返回 null。
    /// </summary>
    [TestMethod]
    public void FieldObject_ByName_ColumnNotExists_ShouldReturnNull()
    {
        // Arrange
        var (record, _, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(record, 0);

        // Act
        var result = recordRow.Field("NonExistentColumn");

        // Assert
        Assert.IsNull(result);
    }

    /// <summary>
    /// <see cref="RecordRow.Field(RecordColumn)"/> 使用同一记录上的列对象读取时，应返回对应对象值。
    /// </summary>
    [TestMethod]
    public void FieldObject_ByColumn_SameRecord_ShouldReturnValue()
    {
        // Arrange
        var (record, _, stringColumn, _) = CreateTestRecord();
        var recordRow = new RecordRow(record, 1);

        // Act
        var result = recordRow.Field(stringColumn);

        // Assert
        Assert.AreEqual("Test2", result);
    }

    /// <summary>
    /// <see cref="RecordRow.Field(RecordColumn)"/> 使用其他记录的同名列对象读取时，应回退到按列名查找并返回值。
    /// </summary>
    [TestMethod]
    public void FieldObject_ByColumn_DifferentRecord_ShouldFallbackToNameSearch()
    {
        // Arrange
        var (record1, _, _, _) = CreateTestRecord();
        var (record2, _, _, boolColumn2) = CreateTestRecord();
        var recordRow = new RecordRow(record1, 0);

        // Act
        var result = recordRow.Field(boolColumn2);

        // Assert
        Assert.AreEqual(true, result);
    }

}

