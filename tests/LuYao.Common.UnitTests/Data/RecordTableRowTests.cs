using System;
using System.Collections.Generic;
using LuYao.Data.Meta;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LuYao.Data;

/// <summary>
/// ���� <see cref="RecordRow"/> �Ĺ��졢�ֶη��ʡ���ֵ�� dynamic ��Ϊ��
/// </summary>
[TestClass]
public class RecordTableRowTests
{
    /// <summary>
    /// �����������͡��ַ����Ͳ����еĲ��Լ�¼��
    /// </summary>
    private (RecordTable table, RecordColumn<int> intColumn, RecordColumn<string> stringColumn, RecordColumn<bool> boolColumn) CreateTestRecord()
    {
        var table = new RecordTable("TestTable", 5);
        var intColumn = table.Columns.Add<int>("IntColumn");
        var stringColumn = table.Columns.Add<string>("StringColumn");
        var boolColumn = table.Columns.Add<bool>("BoolColumn");

        var row1 = table.AddRow();
        var row2 = table.AddRow();

        intColumn.SetValue(0, 100);
        intColumn.SetValue(1, 200);
        stringColumn.SetValue(0, "Test1");
        stringColumn.SetValue(1, "Test2");
        boolColumn.SetValue(0, true);
        boolColumn.SetValue(1, false);

        return (table, intColumn, stringColumn, boolColumn);
    }


    /// <summary>
    /// ʹ����Ч��������ʱ��Ӧ��ȷ��ʼ����¼���кš�
    /// </summary>
    [TestMethod]
    public void Constructor_ValidParameters_ShouldInitializeCorrectly()
    {
        // Arrange
        var (table, _, _, _) = CreateTestRecord();

        // Act
        var recordRow = new RecordRow(table, 0);

        // Assert
        Assert.AreEqual(table, recordRow.Table);
        Assert.AreEqual(0, recordRow.Row);
    }

    /// <summary>
    /// ����¼Ϊ null ʱ��Ӧ�׳� <see cref="ArgumentNullException"/>��
    /// </summary>
    [TestMethod]
    public void Constructor_NullRecord_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new RecordRow(null!, 0));
    }

    /// <summary>
    /// ��������Ϊ����ʱ��Ӧ�׳� <see cref="ArgumentOutOfRangeException"/>��
    /// </summary>
    [TestMethod]
    public void Constructor_NegativeRowIndex_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var (table, _, _, _) = CreateTestRecord();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new RecordRow(table, -1));
    }

    /// <summary>
    /// ��������������¼��Χʱ��Ӧ�׳� <see cref="ArgumentOutOfRangeException"/>��
    /// </summary>
    [TestMethod]
    public void Constructor_RowIndexOutOfRange_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var (table, _, _, _) = CreateTestRecord();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new RecordRow(table, table.Count));
        Assert.Throws<ArgumentOutOfRangeException>(() => new RecordRow(table, table.Count + 1));
    }



    /// <summary>
    /// <see cref="RecordRow.Table"/> Ӧ��������� <see cref="Record"/>��
    /// </summary>
    [TestMethod]
    public void Record_Property_ShouldReturnCorrectRecord()
    {
        // Arrange
        var (table, _, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(table, 0);

        // Act
        var result = recordRow.Table;

        // Assert
        Assert.AreEqual(table, result);
    }

    /// <summary>
    /// <see cref="RecordRow.Row"/> Ӧ���ص�ǰ�кš�
    /// </summary>
    [TestMethod]
    public void Row_Property_ShouldReturnCorrectRowIndex()
    {
        // Arrange
        var (table, _, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(table, 1);

        // Act
        var result = recordRow.Row;

        // Assert
        Assert.AreEqual(1, result);
    }



    /// <summary>
    /// ��ʽת��Ϊ <see cref="int"/> ʱ��Ӧ���ص�ǰ�кš�
    /// </summary>
    [TestMethod]
    public void ImplicitConversion_ToInt_ShouldReturnRowIndex()
    {
        // Arrange
        var (table, _, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(table, 1);

        // Act
        int rowIndex = recordRow;

        // Assert
        Assert.AreEqual(1, rowIndex);
    }



    /// <summary>
    /// ��������ȡ�Ѵ��ڵĲ�����ʱ��Ӧ������ȷֵ��
    /// </summary>
    [TestMethod]
    public void GetBoolean_ByName_ColumnExists_ShouldReturnCorrectValue()
    {
        // Arrange
        var (table, _, _, boolColumn) = CreateTestRecord();
        var recordRow = new RecordRow(table, 0);

        // Act
        var result = recordRow.To<bool>("BoolColumn");

        // Assert
        Assert.AreEqual(true, result);
    }

    /// <summary>
    /// ��������ȡ�����ڵĲ�����ʱ��Ӧ����Ĭ��ֵ��
    /// </summary>
    [TestMethod]
    public void GetBoolean_ByName_ColumnNotExists_ShouldReturnDefault()
    {
        // Arrange
        var (table, _, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(table, 0);

        // Act
        var result = recordRow.To<bool>("NonExistentColumn");

        // Assert
        Assert.AreEqual(default(bool), result);
    }

    /// <summary>
    /// ��������ȡ�Ѵ��ڵ��ַ�����ʱ��Ӧ������ȷֵ��
    /// </summary>
    [TestMethod]
    public void GetString_ByName_ColumnExists_ShouldReturnCorrectValue()
    {
        // Arrange
        var (table, _, stringColumn, _) = CreateTestRecord();
        var recordRow = new RecordRow(table, 1);

        // Act
        var result = recordRow.To<string>("StringColumn");

        // Assert
        Assert.AreEqual("Test2", result);
    }

    /// <summary>
    /// ��������ȡ�����ڵ��ַ�����ʱ��Ӧ����Ĭ��ֵ��
    /// </summary>
    [TestMethod]
    public void GetString_ByName_ColumnNotExists_ShouldReturnDefault()
    {
        // Arrange
        var (table, _, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(table, 0);

        // Act
        var result = recordRow.To<string>("NonExistentColumn");

        // Assert
        Assert.AreEqual(default(string), result);
    }

    /// <summary>
    /// ��������ȡ�Ѵ��ڵ�������ʱ��Ӧ������ȷֵ��
    /// </summary>
    [TestMethod]
    public void GetInt32_ByName_ColumnExists_ShouldReturnCorrectValue()
    {
        // Arrange
        var (table, intColumn, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(table, 1);

        // Act
        var result = recordRow.To<int>("IntColumn");

        // Assert
        Assert.AreEqual(200, result);
    }

    /// <summary>
    /// ��������ȡ�����ڵ�������ʱ��Ӧ����Ĭ��ֵ��
    /// </summary>
    [TestMethod]
    public void GetInt32_ByName_ColumnNotExists_ShouldReturnDefault()
    {
        // Arrange
        var (table, _, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(table, 0);

        // Act
        var result = recordRow.To<int>("NonExistentColumn");

        // Assert
        Assert.AreEqual(default(int), result);
    }

    /// <summary>
    /// ���Ͱ�������ȡ�Ѵ�����ʱ��Ӧ������ȷֵ��
    /// </summary>
    [TestMethod]
    public void GetGeneric_ByName_ColumnExists_ShouldReturnCorrectValue()
    {
        // Arrange
        var (table, intColumn, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(table, 0);

        // Act
        var result = recordRow.To<int>("IntColumn");

        // Assert
        Assert.AreEqual(100, result);
    }

    /// <summary>
    /// ���Ͱ�������ȡ��������ʱ��Ӧ����Ĭ��ֵ��
    /// </summary>
    [TestMethod]
    public void GetGeneric_ByName_ColumnNotExists_ShouldReturnDefault()
    {
        // Arrange
        var (table, _, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(table, 0);

        // Act
        var result = recordRow.To<int>("NonExistentColumn");

        // Assert
        Assert.AreEqual(default(int), result);
    }

    /// <summary>
    /// Ӧ֧�ְ�������ȡ�ֽ�ֵ��
    /// </summary>
    [TestMethod]
    public void GetByte_ByName_ShouldWork()
    {
        // Arrange
        var table = new RecordTable("TestTable", 1);
        var byteColumn = table.Columns.Add<byte>("ByteColumn");
        var row = table.AddRow();
        byteColumn.Set(row.Row, 255);
        var recordRow = new RecordRow(table, 0);

        // Act
        var result = recordRow.To<byte>("ByteColumn");

        // Assert
        Assert.AreEqual((byte)255, result);
    }

    /// <summary>
    /// Ӧ֧�ְ�������ȡ˫���ȸ���ֵ��
    /// </summary>
    [TestMethod]
    public void GetDouble_ByName_ShouldWork()
    {
        // Arrange
        var table = new RecordTable("TestTable", 1);
        var doubleColumn = table.Columns.Add<double>("DoubleColumn");
        var row = table.AddRow();
        doubleColumn.Set(row.Row, 3.14159);
        var recordRow = new RecordRow(table, 0);

        // Act
        var result = recordRow.To<double>("DoubleColumn");

        // Assert
        Assert.AreEqual(3.14159, result, 0.00001);
    }

    /// <summary>
    /// Ӧ֧�ְ�������ȡ����ʱ��ֵ��
    /// </summary>
    [TestMethod]
    public void GetDateTime_ByName_ShouldWork()
    {
        // Arrange
        var table = new RecordTable("TestTable", 1);
        var dateTimeColumn = table.Columns.Add<DateTime>("DateTimeColumn");
        var testDate = new DateTime(2023, 8, 15, 14, 30, 0);
        var row = table.AddRow();
        dateTimeColumn.Set(row.Row, testDate);
        var recordRow = new RecordRow(table, 0);

        // Act
        var result = recordRow.To<DateTime>("DateTimeColumn");

        // Assert
        Assert.AreEqual(testDate, result);
    }



    /// <summary>
    /// ������Ϊ��ʱ�������Ͷ�ȡ����Ӧ����Ĭ��ֵ��
    /// </summary>
    [TestMethod]
    public void GetMethods_EmptyColumnName_ShouldReturnDefault()
    {
        // Arrange
        var (table, _, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(table, 0);

        // Act & Assert
        Assert.AreEqual(default(int), recordRow.To<int>(""));
        Assert.AreEqual(default(string), recordRow.To<string>(""));
        Assert.AreEqual(default(bool), recordRow.To<bool>(""));
    }

    /// <summary>
    /// �ڶ������ݳ����£�Ӧ�ܶ�ȡ�����е���ȷֵ��
    /// </summary>
    [TestMethod]
    public void GetMethods_MultipleRows_ShouldReturnCorrectValues()
    {
        // Arrange
        var (table, intColumn, stringColumn, boolColumn) = CreateTestRecord();

        var row3 = table.AddRow();
        intColumn.Set(row3.Row, 300);
        stringColumn.Set(row3.Row, "Test3");
        boolColumn.Set(row3.Row, true);

        var recordRow0 = new RecordRow(table, 0);
        var recordRow1 = new RecordRow(table, 1);
        var recordRow2 = new RecordRow(table, 2);

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
    /// ��η���ͬһ�ֶ�ʱ��Ӧ���ֽ��һ�¡�
    /// </summary>
    [TestMethod]
    public void GetMethods_RepeatedAccess_ShouldReturnConsistentResults()
    {
        // Arrange
        var (table, _, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(table, 0);

        // Act
        var result1 = recordRow.To<int>("IntColumn");
        var result2 = recordRow.To<int>("IntColumn");

        // Assert
        Assert.AreEqual(result1, result2);
        Assert.AreEqual(100, result1);
    }



    /// <summary>
    /// ʹ�������������Ѵ��ڵ�������ʱ��Ӧд��ɹ���
    /// </summary>
    [TestMethod]
    public void Set_TypedColumn_ShouldUpdateValue()
    {
        // Arrange
        var (table, intColumn, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(table, 0);

        // Act
        recordRow["IntColumn"] = 999;

        // Assert
        Assert.AreEqual(999, recordRow.To<int>("IntColumn"));
    }

    /// <summary>
    /// ʹ�������������Ѵ��ڵ��ַ�����ʱ��Ӧд��ɹ���
    /// </summary>
    [TestMethod]
    public void Set_StringColumn_ShouldUpdateValue()
    {
        // Arrange
        var (table, _, stringColumn, _) = CreateTestRecord();
        var recordRow = new RecordRow(table, 1);

        // Act
        recordRow["StringColumn"] = "NewValue";

        // Assert
        Assert.AreEqual("NewValue", recordRow.To<string>("StringColumn"));
    }

    /// <summary>
    /// ���в�����ʱ��<see cref="RecordRow.Set{T}(string, T)"/> Ӧ�Զ�������Ӧ�С�
    /// </summary>
    [TestMethod]
    public void Set_ColumnNotExists_ShouldAutoCreateColumn()
    {
        // Arrange
        var (table, _, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(table, 0);
        int beforeCount = table.Columns.Count;

        // Act
        recordRow["NewColumn"] = 12345;

        // Assert
        Assert.AreEqual(beforeCount + 1, table.Columns.Count);
        var col = table.Columns.Find("NewColumn");
        Assert.IsNotNull(col);
        Assert.AreEqual(typeof(int), col.Type);
        Assert.AreEqual(12345, recordRow.To<int>("NewColumn"));
    }

    /// <summary>
    /// д����ٶ�ȡʱ��Ӧ�õ���д��һ�µ�ֵ��
    /// </summary>
    [TestMethod]
    public void Set_ThenReadViaIndexer_ShouldBeConsistent()
    {
        // Arrange
        var (table, intColumn, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(table, 0);

        // Act
        recordRow["IntColumn"] = 42;

        // Assert
        Assert.AreEqual(42, recordRow.To<int>("IntColumn"));
    }



    /// <summary>
    /// dynamic ����Ա����ȡʱ��Ӧ������ȷֵ��
    /// </summary>
    [TestMethod]
    public void Dynamic_GetMember_ShouldReturnCorrectValue()
    {
        // Arrange
        var (table, _, stringColumn, _) = CreateTestRecord();
        dynamic row = new RecordRow(table, 0);

        // Act
        var result = row.StringColumn;

        // Assert
        Assert.AreEqual("Test1", result);
    }

    /// <summary>
    /// dynamic ����Ա��д��ʱ��Ӧ���µײ��¼ֵ��
    /// </summary>
    [TestMethod]
    public void Dynamic_SetMember_ShouldUpdateValue()
    {
        // Arrange
        var (table, _, stringColumn, _) = CreateTestRecord();
        dynamic row = new RecordRow(table, 0);

        // Act
        row.StringColumn = "DynValue";

        // Assert
        var recordRow = new RecordRow(table, 0);
        Assert.AreEqual("DynValue", recordRow.To<string>("StringColumn"));
    }

    /// <summary>
    /// dynamic ����������ȡʱ��Ӧ������ȷֵ��
    /// </summary>
    [TestMethod]
    public void Dynamic_GetIndex_ShouldReturnCorrectValue()
    {
        // Arrange
        var (table, intColumn, _, _) = CreateTestRecord();
        dynamic row = new RecordRow(table, 1);

        // Act - dynamic ������ȡ
        var result = row["IntColumn"];

        // Assert
        Assert.AreEqual(200, (int)result!);
    }

    /// <summary>
    /// dynamic ��������д��ʱ��Ӧ���µײ��¼ֵ��
    /// </summary>
    [TestMethod]
    public void Dynamic_SetIndex_ShouldUpdateValue()
    {
        // Arrange
        var (table, intColumn, _, _) = CreateTestRecord();
        dynamic row = new RecordRow(table, 0);

        // Act - dynamic ����д��
        row["IntColumn"] = 777;

        // Assert
        var recordRow = new RecordRow(table, 0);
        Assert.AreEqual(777, recordRow.To<int>("IntColumn"));
    }

    /// <summary>
    /// dynamic ��ȡ�����ڵĳ�Աʱ��Ӧ���� null��
    /// </summary>
    [TestMethod]
    public void Dynamic_GetMember_ColumnNotExists_ShouldReturnNull()
    {
        // Arrange
        var (table, _, _, _) = CreateTestRecord();
        dynamic row = new RecordRow(table, 0);

        // Act
        var result = row.NoSuchColumn;

        // Assert
        Assert.IsNull(result);
    }

    /// <summary>
    /// dynamic д�벻���ڵĳ�Աʱ����Ӧ�׳��쳣��
    /// </summary>
    [TestMethod]
    public void Dynamic_SetMember_ColumnNotExists_ShouldNotThrow()
    {
        // Arrange
        var (table, _, _, _) = CreateTestRecord();
        dynamic row = new RecordRow(table, 0);

        // Act & Assert
        row.NoSuchColumn = "ignored"; // should not throw
    }

    /// <summary>
    /// dynamic ��Ա��ȡ���Ӧ����ʽ�����ֶζ�ȡ����һ�¡�
    /// </summary>
    [TestMethod]
    public void Dynamic_GetMember_ShouldBeConsistentWithGetMethod()
    {
        // Arrange
        var (table, intColumn, _, boolColumn) = CreateTestRecord();
        dynamic row = new RecordRow(table, 0);
        var recordRow = new RecordRow(table, 0);

        // Act & Assert
        Assert.AreEqual(recordRow.To<int>("IntColumn"), (int)row.IntColumn!);
        Assert.AreEqual(recordRow.To<bool>("BoolColumn"), (bool)row.BoolColumn!);
    }

    /// <summary>
    /// ��������������ȡ�Ѵ�����ʱ��Ӧ���ض�Ӧ����ֵ��
    /// </summary>
    [TestMethod]
    public void FieldObject_ByName_ColumnExists_ShouldReturnValue()
    {
        // Arrange
        var (table, _, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(table, 0);

        // Act
        var result = recordRow["IntColumn"];

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(100, (int)result!);
    }

    /// <summary>
    /// ��������ȡ��������ʱ��������Ӧ���� null��
    /// </summary>
    [TestMethod]
    public void FieldObject_ByName_ColumnNotExists_ShouldReturnNull()
    {
        // Arrange
        var (table, _, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(table, 0);

        // Act
        var result = recordRow["NonExistentColumn"];

        // Assert
        Assert.IsNull(result);
    }

    /// <summary>
    /// <see cref="RecordRow.ToDictionary"/> Ӧ���ص�ǰ�е�ȫ�������Ӧֵ��
    /// </summary>
    [TestMethod]
    public void ToDictionary_ShouldReturnAllColumnsWithCurrentRowValues()
    {
        // Arrange
        var (table, _, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(table, 1);

        // Act
        var result = recordRow.ToDictionary();

        // Assert
        Assert.AreEqual(3, result.Count);
        Assert.AreEqual(200, (int)result["IntColumn"]!);
        Assert.AreEqual("Test2", (string)result["StringColumn"]!);
        Assert.AreEqual(false, (bool)result["BoolColumn"]!);
    }

    /// <summary>
    /// <see cref="RecordRow.ToDictionary"/> ���� null ��ֵӦ���� null��
    /// </summary>
    [TestMethod]
    public void ToDictionary_WhenColumnValueIsNull_ShouldKeepNullValue()
    {
        // Arrange
        var table = new RecordTable("TestTable", 2);
        var nullableColumn = table.Columns.Add<string>("NullableColumn");
        table.AddRow();
        nullableColumn.SetValue(0, null!);
        var recordRow = new RecordRow(table, 0);

        // Act
        var result = recordRow.ToDictionary();

        // Assert
        Assert.AreEqual(1, result.Count);
        Assert.IsTrue(result.ContainsKey("NullableColumn"));
        Assert.IsNull(result["NullableColumn"]);
    }

    /// <summary>
    /// <see cref="RecordRow.ToString"/> Ӧ����к��Լ��ֵ������ֵ��Ϣ��
    /// </summary>
    [TestMethod]
    public void ToString_ShouldContainRowAndDictionaryLikeValues()
    {
        // Arrange
        var (table, _, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(table, 1);

        // Act
        var result = recordRow.ToString();

        // Assert
        Assert.AreEqual("{ Row = 1, Data = { IntColumn = 200, StringColumn = Test2, BoolColumn = False } }", result);
    }

    /// <summary>
    /// <see cref="RecordRow.ToString"/> ���� null ֵӦ���������ͷ�������ֵ��
    /// </summary>
    [TestMethod]
    public void ToString_WhenValueIsNull_ShouldRenderEmptyValue()
    {
        // Arrange
        var table = new RecordTable("TestTable", 2);
        var nullableColumn = table.Columns.Add<string>("NullableColumn");
        table.AddRow();
        nullableColumn.SetValue(0, null!);
        var recordRow = new RecordRow(table, 0);

        // Act
        var result = recordRow.ToString();

        // Assert
        Assert.AreEqual("{ Row = 0, Data = { NullableColumn =  } }", result);
    }

    /// <summary>
    /// ������ת��Ϊ�ַ���ʱ�����д�������ֵ��Ӧ���ظ�ֵ���ַ�����ʾ��
    /// </summary>
    [TestMethod]
    public void ToString_ByName_ColumnExistsWithValue_ShouldReturnStringRepresentation()
    {
        // Arrange
        var (table, _, stringColumn, _) = CreateTestRecord();
        var recordRow = new RecordRow(table, 0);

        // Act
        var result = recordRow.ToString("StringColumn");

        // Assert
        Assert.AreEqual("Test1", result);
    }

    /// <summary>
    /// ������ת��Ϊ�ַ���ʱ��������Ϊ null��Ӧ���ؿ��ַ�����
    /// </summary>
    [TestMethod]
    public void ToString_ByName_NullColumnName_ShouldReturnEmptyString()
    {
        // Arrange
        var (table, _, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(table, 0);

        // Act
        var result = recordRow.ToString(null!);

        // Assert
        Assert.AreEqual(string.Empty, result);
    }

    /// <summary>
    /// ������ת��Ϊ�ַ���ʱ��������Ϊ���ַ�����Ӧ���ؿ��ַ�����
    /// </summary>
    [TestMethod]
    public void ToString_ByName_EmptyColumnName_ShouldReturnEmptyString()
    {
        // Arrange
        var (table, _, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(table, 0);

        // Act
        var result = recordRow.ToString("");

        // Assert
        Assert.AreEqual(string.Empty, result);
    }

    /// <summary>
    /// ������ת��Ϊ�ַ���ʱ���������������հ��ַ���Ӧ���ؿ��ַ�����
    /// </summary>
    [TestMethod]
    public void ToString_ByName_WhitespaceColumnName_ShouldReturnEmptyString()
    {
        // Arrange
        var (table, _, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(table, 0);

        // Act
        var result = recordRow.ToString("   ");

        // Assert
        Assert.AreEqual(string.Empty, result);
    }

    /// <summary>
    /// ������ת��Ϊ�ַ���ʱ�����в����ڣ�Ӧ���ؿ��ַ�����
    /// </summary>
    [TestMethod]
    public void ToString_ByName_ColumnNotExists_ShouldReturnEmptyString()
    {
        // Arrange
        var (table, _, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(table, 0);

        // Act
        var result = recordRow.ToString("NonExistentColumn");

        // Assert
        Assert.AreEqual(string.Empty, result);
    }

    /// <summary>
    /// ������ת��Ϊ�ַ���ʱ�����д��ڵ�ֵΪ null��Ӧ���ؿ��ַ�����
    /// </summary>
    [TestMethod]
    public void ToString_ByName_ColumnExistsWithNullValue_ShouldReturnEmptyString()
    {
        // Arrange
        var table = new RecordTable("TestTable", 1);
        var nullableColumn = table.Columns.Add<string>("NullableColumn");
        table.AddRow();
        nullableColumn.SetValue(0, null!);
        var recordRow = new RecordRow(table, 0);

        // Act
        var result = recordRow.ToString("NullableColumn");

        // Assert
        Assert.AreEqual(string.Empty, result);
    }

    /// <summary>
    /// ������ת��Ϊ�ַ���ʱ�����д�����Ϊ����ֵ��Ӧ��������ֵ���ַ�����ʾ��
    /// </summary>
    [TestMethod]
    public void ToString_ByName_IntColumnExists_ShouldReturnStringRepresentation()
    {
        // Arrange
        var (table, intColumn, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(table, 1);

        // Act
        var result = recordRow.ToString("IntColumn");

        // Assert
        Assert.AreEqual("200", result);
    }

    /// <summary>
    /// ������ת��Ϊ�ַ���ʱ�����д�����Ϊ����ֵ��Ӧ���ز���ֵ���ַ�����ʾ��
    /// </summary>
    [TestMethod]
    public void ToString_ByName_BoolColumnExists_ShouldReturnStringRepresentation()
    {
        // Arrange
        var (table, _, _, boolColumn) = CreateTestRecord();
        var recordRow = new RecordRow(table, 0);

        // Act
        var result = recordRow.ToString("BoolColumn");

        // Assert
        Assert.AreEqual("True", result);
    }
}
