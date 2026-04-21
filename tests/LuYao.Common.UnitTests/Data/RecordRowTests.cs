using System;
using System.Collections.Generic;
using LuYao.Data.Meta;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LuYao.Data;

/// <summary>
/// RecordRow �ṹ��ĵ�Ԫ������
/// </summary>
[TestClass]
public class RecordRowTests
{
    /// <summary>
    /// ���������õ� Record ���������
    /// </summary>
    private (Record record, RecordColumn<int> intColumn, RecordColumn<string> stringColumn, RecordColumn<bool> boolColumn) CreateTestRecord()
    {
        var record = new Record("TestTable", 5);
        var intColumn = record.Columns.Add<int>("IntColumn");
        var stringColumn = record.Columns.Add<string>("StringColumn");
        var boolColumn = record.Columns.Add<bool>("BoolColumn");

        // ��Ӳ�������
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

    #region ���캯������

    /// <summary>
    /// ���Թ��캯�� - ��������
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
    /// ���Թ��캯�� - Record Ϊ null
    /// </summary>
    [TestMethod]
    public void Constructor_NullRecord_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new RecordRow(null!, 0));
    }

    /// <summary>
    /// ���Թ��캯�� - ������С��0
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
    /// ���Թ��캯�� - ���������ڻ���� Record.Count
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

    #region ���Բ���

    /// <summary>
    /// ���� Record ����
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
    /// ���� Row ����
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

    #region ��ʽת������

    /// <summary>
    /// ������ʽת���� int
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

    #region GetBoolean ����

    /// <summary>
    /// ���� GetBoolean(string name) - �д���
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
    /// ���� GetBoolean(string name) - �в�����
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
    /// ���� GetBoolean(RecordColumn col) - �����ڵ�ǰ��¼
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
    /// ���� GetBoolean(RecordColumn col) - �����ڲ�ͬ��¼
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
        Assert.AreEqual(true, result); // Ӧ��ͨ���������ҵ� record1 �е� BoolColumn
    }

    #endregion

    #region GetString ����

    /// <summary>
    /// ���� GetString(string name) - �д���
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
    /// ���� GetString(string name) - �в�����
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
    /// ���� GetString(RecordColumn col) - �����ڵ�ǰ��¼
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

    #endregion

    #region GetInt32 ����

    /// <summary>
    /// ���� GetInt32(string name) - �д���
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
    /// ���� GetInt32(string name) - �в�����
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
    /// ���� GetInt32(RecordColumn col) - �����ڵ�ǰ��¼
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

    #endregion

    #region ���� Get<T> ����

    /// <summary>
    /// ���� Get<T>(string name) - �д���
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
    /// ���� Get<T>(string name) - �в�����
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
    /// ���� Get<T>(RecordColumn col) - �����ڵ�ǰ��¼
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


    #endregion

    #region ��ֵ���Ͳ��� (��������)

    /// <summary>
    /// ���� GetByte ����
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
    /// ���� GetDouble ����
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
    /// ���� GetDateTime ����
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

    #endregion

    #region �߽�������쳣����

    /// <summary>
    /// ���Կ��ַ�������
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
    /// ���Զ������ݵ���ȷ��
    /// </summary>
    [TestMethod]
    public void GetMethods_MultipleRows_ShouldReturnCorrectValues()
    {
        // Arrange
        var (record, intColumn, stringColumn, boolColumn) = CreateTestRecord();

        // ��Ӹ����������
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

    #endregion

    #region ���ܺ�һ���Բ���

    /// <summary>
    /// ������ͬ���ݵ��ظ�����Ӧ�÷�����ͬ���
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
    /// ���Բ�ͬ��ȡ��ʽ(������ vs ���ж���)�Ľ��һ����
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

    #endregion

    #region IPropertyAccessor ����

    /// <summary>
    /// Props ����Ӧ���� Record ���м���
    /// </summary>
    [TestMethod]
    public void Props_ShouldReturnRecordColumns()
    {
        // Arrange
        var (record, _, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(record, 0);

        // Act
        var props = recordRow.Props;

        // Assert
        Assert.AreSame(record.Columns, props);
    }

    /// <summary>
    /// ������ get - �д���ʱӦ������ȷֵ
    /// </summary>
    [TestMethod]
    public void Indexer_Get_ColumnExists_ShouldReturnCorrectValue()
    {
        // Arrange
        var (record, _, stringColumn, _) = CreateTestRecord();
        var recordRow = new RecordRow(record, 0);

        // Act
        var result = ((IPropertyAccessor)recordRow)["StringColumn"];

        // Assert
        Assert.AreEqual("Test1", result);
    }

    /// <summary>
    /// ������ get - �в�����ʱӦ���� null
    /// </summary>
    [TestMethod]
    public void Indexer_Get_ColumnNotExists_ShouldReturnNull()
    {
        // Arrange
        var (record, _, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(record, 0);

        // Act
        var result = ((IPropertyAccessor)recordRow)["NoSuchColumn"];

        // Assert
        Assert.IsNull(result);
    }

    /// <summary>
    /// ������ set - �д���ʱӦ����ֵ
    /// </summary>
    [TestMethod]
    public void Indexer_Set_ColumnExists_ShouldUpdateValue()
    {
        // Arrange
        var (record, _, stringColumn, _) = CreateTestRecord();
        var recordRow = new RecordRow(record, 0);

        // Act
        ((IPropertyAccessor)recordRow)["StringColumn"] = "Updated";

        // Assert
        Assert.AreEqual("Updated", recordRow.Field<string>("StringColumn"));
    }

    /// <summary>
    /// ������ set - �в�����ʱӦ��Ĭ���������׳��쳣
    /// </summary>
    [TestMethod]
    public void Indexer_Set_ColumnNotExists_ShouldNotThrow()
    {
        // Arrange
        var (record, _, _, _) = CreateTestRecord();
        var recordRow = new RecordRow(record, 0);

        // Act & Assert：通过 IPropertyAccessor（即 Mapping 路径）写入不存在的列应静默跳过，且不会自动建列。
        ((IPropertyAccessor)recordRow)["NoSuchColumn"] = "Value";
        Assert.IsFalse(record.Columns.Contains("NoSuchColumn"));
    }

    #endregion

    #region Set<T> ����

    /// <summary>
    /// Set<T> - ǿ�����и�ֵӦ�ɹ�
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
    /// Set<T> - string �и�ֵӦ�ɹ�
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
    /// Set<T> - �в�����ʱӦ�׳� KeyNotFoundException
    /// </summary>
    /// <summary>
    /// Set&lt;T&gt; - 列不存在时应自动建列（新语义）。
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
    /// Set<T> ��ͨ����������ȡӦһ��
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
        Assert.AreEqual(42, (int)((IPropertyAccessor)recordRow)["IntColumn"]!);
    }

    #endregion

    #region IDynamicMetaObjectProvider (dynamic) ����

    /// <summary>
    /// dynamic ��Ա��ȡ - Ӧ������ȷֵ
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
    /// dynamic ��Աд�� - Ӧ������ֵ
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
    /// dynamic ��������ȡ - Ӧ������ȷֵ
    /// </summary>
    [TestMethod]
    public void Dynamic_GetIndex_ShouldReturnCorrectValue()
    {
        // Arrange
        var (record, intColumn, _, _) = CreateTestRecord();
        dynamic row = new RecordRow(record, 1);

        // Act：dynamic 索引访问
        var result = row["IntColumn"];

        // Assert
        Assert.AreEqual(200, (int)result!);
    }

    /// <summary>
    /// dynamic ������д�� - Ӧ������ֵ
    /// </summary>
    [TestMethod]
    public void Dynamic_SetIndex_ShouldUpdateValue()
    {
        // Arrange
        var (record, intColumn, _, _) = CreateTestRecord();
        dynamic row = new RecordRow(record, 0);

        // Act：dynamic 索引写入
        row["IntColumn"] = 777;

        // Assert
        var recordRow = new RecordRow(record, 0);
        Assert.AreEqual(777, recordRow.Field<int>("IntColumn"));
    }

    /// <summary>
    /// dynamic ��ȡ�����ڵ��� - Ӧ���� null�������쳣��
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
    /// dynamic д�벻���ڵ��� - Ӧ��Ĭ�����������쳣��
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
    /// dynamic ��ǿ���� Get ��ȡ���Ӧһ��
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

    #endregion
}
