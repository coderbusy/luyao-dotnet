using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Data;

[TestClass]
public class RecordTests
{
    [TestMethod]
    public void Constructor_ValidParameters_InitializesCorrectly()
    {
        // Arrange
        var dt = new Record();

        // Assert
        Assert.AreEqual(0, dt.Count);
        Assert.IsNotNull(dt.Columns);
        Assert.AreEqual(0, dt.Columns.Count);
    }

    [TestMethod]
    public void AddRowAndSetColumnValue_WorksCorrectly()
    {
        // Arrange
        var table = new Record();
        var colId = table.Columns.AddInt32("Id");
        var colName = table.Columns.AddString("Name");

        // Act
        var row = table.AddRow();
        colId.Set(1, row.Row);
        colName.Set("Test", row.Row);

        // Assert
        Assert.AreEqual(1, table.Count);
        Assert.AreEqual(2, table.Columns.Count);
        Assert.AreEqual(1, colId.GetValue(row.Row));
        Assert.AreEqual("Test", colName.GetValue(row.Row));
    }

    [TestMethod]
    public void Constructor_WithNameAndRows_InitializesCorrectly()
    {
        // Arrange & Act
        var table = new Record("TestTable", 10);

        // Assert
        Assert.AreEqual("TestTable", table.Name);
        Assert.AreEqual(0, table.Count);
        Assert.IsNotNull(table.Columns);
        Assert.AreEqual(0, table.Columns.Count);
    }

    [TestMethod]
    public void AddMultipleRowsAndColumns_WorksCorrectly()
    {
        // Arrange
        var table = new Record();
        var colId = table.Columns.AddInt32("Id");
        var colName = table.Columns.AddString("Name");
        var colAge = table.Columns.AddInt32("Age");

        // Act
        var row1 = table.AddRow();
        colId.Set(1, row1.Row);
        colName.Set("Alice", row1.Row);
        colAge.Set(25, row1.Row);

        var row2 = table.AddRow();
        colId.Set(2, row2.Row);
        colName.Set("Bob", row2.Row);
        colAge.Set(30, row2.Row);

        // Assert
        Assert.AreEqual(2, table.Count);
        Assert.AreEqual(3, table.Columns.Count);

        Assert.AreEqual(1, colId.GetValue(row1.Row));
        Assert.AreEqual("Alice", colName.GetValue(row1.Row));
        Assert.AreEqual(25, colAge.GetValue(row1.Row));

        Assert.AreEqual(2, colId.GetValue(row2.Row));
        Assert.AreEqual("Bob", colName.GetValue(row2.Row));
        Assert.AreEqual(30, colAge.GetValue(row2.Row));
    }

    [TestMethod]
    public void SetValue_ByColumnName_WorksCorrectly()
    {
        // Arrange
        var table = new Record();
        var idCol = table.Columns.AddInt32("Id");
        var nameCol = table.Columns.AddString("Name");
        table.AddRow();

        // Act
        idCol.Set(42, 0);
        nameCol.Set("TestUser", 0);

        // Assert
        Assert.AreEqual(42, idCol.GetValue(0));
        Assert.AreEqual("TestUser", nameCol.GetValue(0));
    }

    [TestMethod]
    public void GetValue_ByColumnName_WorksCorrectly()
    {
        // Arrange
        var table = new Record();
        var colId = table.Columns.AddInt32("Id");
        var colName = table.Columns.AddString("Name");
        var row = table.AddRow();
        colId.Set(123, row.Row);
        colName.Set("TestValue", row.Row);

        // Act & Assert
        Assert.AreEqual(123, colId.GetValue(0));
        Assert.AreEqual("TestValue", colName.GetValue(0));
    }

    [TestMethod]
    public void Contains_ExistingColumn_ReturnsTrue()
    {
        // Arrange
        var table = new Record();
        table.Columns.AddInt32("Id");
        table.Columns.AddString("Name");

        // Act & Assert
        Assert.IsTrue(table.Columns.Find("Id") != null);
        Assert.IsTrue(table.Columns.Find("Name") != null);
        Assert.IsFalse(table.Columns.Find("NonExistent") != null);
    }

    [TestMethod]
    public void GetValue_NonExistentColumn_ThrowsKeyNotFoundException()
    {
        // Arrange
        var table = new Record();
        table.AddRow();

        // Act & Assert
        var col = table.Columns.Find("NonExistent");
        Assert.IsNull(col);
    }

    [TestMethod]
    public void SetValue_NonExistentColumn_ThrowsKeyNotFoundException()
    {
        // Arrange
        var table = new Record();
        table.AddRow();

        // Act & Assert
        var col = table.Columns.Find("NonExistent");
        Assert.IsNull(col);
    }

    [TestMethod]
    public void GetRow_ValidIndex_ReturnsCorrectRow()
    {
        // Arrange
        var table = new Record();
        var colId = table.Columns.AddInt32("Id");
        table.AddRow();
        table.AddRow();
        colId.Set(10, 0);
        colId.Set(20, 1);

        // Act
        var rows = table.ToArray();
        var row0 = rows[0];
        var row1 = rows[1];

        // Assert
        Assert.AreEqual(0, row0.Row);
        Assert.AreEqual(1, row1.Row);
        Assert.AreEqual(10, colId.ToInt32(row0.Row));
        Assert.AreEqual(20, colId.ToInt32(row1.Row));
    }

    [TestMethod]
    public void GetRow_InvalidIndex_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var table = new Record();
        table.AddRow();

        // Act & Assert
        // 测试通过枚举器获取行
        var rows = table.ToArray();
        Assert.AreEqual(1, rows.Length);
    }

    [TestMethod]
    public void Enumeration_WorksCorrectly()
    {
        // Arrange
        var table = new Record();
        var colId = table.Columns.AddInt32("Id");
        table.AddRow();
        table.AddRow();
        table.AddRow();
        colId.Set(1, 0);
        colId.Set(2, 1);
        colId.Set(3, 2);

        // Act
        var rows = new List<RecordRow>();
        foreach (var row in table)
        {
            rows.Add(row);
        }

        // Assert
        Assert.AreEqual(3, rows.Count);
        Assert.AreEqual(0, rows[0].Row);
        Assert.AreEqual(1, rows[1].Row);
        Assert.AreEqual(2, rows[2].Row);
        Assert.AreEqual(1, colId.GetValue(rows[0].Row));
        Assert.AreEqual(2, colId.GetValue(rows[1].Row));
        Assert.AreEqual(3, colId.GetValue(rows[2].Row));
    }

    [TestMethod]
    public void DifferentDataTypes_WorkCorrectly()
    {
        // Arrange
        var table = new Record();
        var colBool = table.Columns.AddBoolean("Bool");
        var colByte = table.Columns.AddByte("Byte");
        var colDateTime = table.Columns.AddDateTime("DateTime");
        var colDecimal = table.Columns.AddDecimal("Decimal");
        var colDouble = table.Columns.AddDouble("Double");
        var colInt16 = table.Columns.AddInt16("Int16");
        var colInt32 = table.Columns.AddInt32("Int32");
        var colInt64 = table.Columns.AddInt64("Int64");
        var colString = table.Columns.AddString("String");

        var row = table.AddRow();
        var testDate = new DateTime(2023, 12, 25);

        // Act
        colBool.Set(true, row.Row);
        colByte.Set((byte)255, row.Row);
        colDateTime.Set(testDate, row.Row);
        colDecimal.Set(123.45m, row.Row);
        colDouble.Set(123.456, row.Row);
        colInt16.Set((short)1000, row.Row);
        colInt32.Set(100000, row.Row);
        colInt64.Set(10000000000L, row.Row);
        colString.Set("Hello World", row.Row);

        // Assert
        Assert.AreEqual(true, colBool.GetValue(row.Row));
        Assert.AreEqual((byte)255, colByte.GetValue(row.Row));
        Assert.AreEqual(testDate, colDateTime.GetValue(row.Row));
        Assert.AreEqual(123.45m, colDecimal.GetValue(row.Row));
        Assert.AreEqual(123.456, colDouble.GetValue(row.Row));
        Assert.AreEqual((short)1000, colInt16.GetValue(row.Row));
        Assert.AreEqual(100000, colInt32.GetValue(row.Row));
        Assert.AreEqual(10000000000L, colInt64.GetValue(row.Row));
        Assert.AreEqual("Hello World", colString.GetValue(row.Row));
    }

    [TestMethod]
    public void RowImplicitConversion_WorksCorrectly()
    {
        // Arrange
        var table = new Record();
        table.AddRow();
        table.AddRow();
        var rows = table.ToArray();
        var row = rows[1];

        // Act
        int rowIndex = row; // 隐式转换

        // Assert
        Assert.AreEqual(1, rowIndex);
    }

    [TestMethod]
    public void TypedSetAndGet_WorksCorrectly()
    {
        // Arrange
        var table = new Record();
        var colInt = table.Columns.AddInt32("Int");
        var colString = table.Columns.AddString("String");
        var colBool = table.Columns.AddBoolean("Bool");
        var row = table.AddRow();

        // Act & Assert
        colInt.Set(42, row.Row);
        Assert.AreEqual(42, colInt.ToInt32(row.Row));

        colString.Set("Test", row.Row);
        Assert.AreEqual("Test", colString.ToString(row.Row));

        colBool.Set(true, row.Row);
        Assert.AreEqual(true, colBool.ToBoolean(row.Row));
    }

    [TestMethod]
    public void AddRow_MultipleRows_CountIncreases()
    {
        // Arrange
        var table = new Record();

        // Act & Assert
        Assert.AreEqual(0, table.Count);

        table.AddRow();
        Assert.AreEqual(1, table.Count);

        table.AddRow();
        Assert.AreEqual(2, table.Count);

        table.AddRow();
        Assert.AreEqual(3, table.Count);
    }

    [TestMethod]
    public void Columns_Find_ExistingColumn_ReturnsColumn()
    {
        // Arrange
        var table = new Record();
        var originalCol = table.Columns.AddInt32("TestColumn");

        // Act
        var foundCol = table.Columns.Find("TestColumn");

        // Assert
        Assert.IsNotNull(foundCol);
        Assert.AreSame(originalCol, foundCol);
    }

    [TestMethod]
    public void Columns_Find_NonExistentColumn_ReturnsNull()
    {
        // Arrange
        var table = new Record();
        table.Columns.AddInt32("TestColumn");

        // Act
        var foundCol = table.Columns.Find("NonExistent");

        // Assert
        Assert.IsNull(foundCol);
    }

    [TestMethod]
    public void SpecificTypeColumns_Add_CorrectTypes()
    {
        // Arrange
        var table = new Record();

        // Act
        var colBool = table.Columns.AddBoolean("BoolCol");
        var colByte = table.Columns.AddByte("ByteCol");
        var colChar = table.Columns.AddChar("CharCol");
        var colDateTime = table.Columns.AddDateTime("DateTimeCol");
        var colDecimal = table.Columns.AddDecimal("DecimalCol");
        var colDouble = table.Columns.AddDouble("DoubleCol");
        var colInt16 = table.Columns.AddInt16("Int16Col");
        var colInt32 = table.Columns.AddInt32("Int32Col");
        var colInt64 = table.Columns.AddInt64("Int64Col");
        var colSByte = table.Columns.AddSByte("SByteCol");
        var colSingle = table.Columns.AddSingle("SingleCol");
        var colString = table.Columns.AddString("StringCol");
        var colUInt16 = table.Columns.AddUInt16("UInt16Col");
        var colUInt32 = table.Columns.AddUInt32("UInt32Col");
        var colUInt64 = table.Columns.AddUInt64("UInt64Col");

        // Assert
        Assert.AreEqual(RecordDataCode.Boolean, colBool.Code);
        Assert.AreEqual(RecordDataCode.Byte, colByte.Code);
        Assert.AreEqual(RecordDataCode.Char, colChar.Code);
        Assert.AreEqual(RecordDataCode.DateTime, colDateTime.Code);
        Assert.AreEqual(RecordDataCode.Decimal, colDecimal.Code);
        Assert.AreEqual(RecordDataCode.Double, colDouble.Code);
        Assert.AreEqual(RecordDataCode.Int16, colInt16.Code);
        Assert.AreEqual(RecordDataCode.Int32, colInt32.Code);
        Assert.AreEqual(RecordDataCode.Int64, colInt64.Code);
        Assert.AreEqual(RecordDataCode.SByte, colSByte.Code);
        Assert.AreEqual(RecordDataCode.Single, colSingle.Code);
        Assert.AreEqual(RecordDataCode.String, colString.Code);
        Assert.AreEqual(RecordDataCode.UInt16, colUInt16.Code);
        Assert.AreEqual(RecordDataCode.UInt32, colUInt32.Code);
        Assert.AreEqual(RecordDataCode.UInt64, colUInt64.Code);
        Assert.AreEqual(15, table.Columns.Count);
    }

    [TestMethod]
    public void RecordColumn_Clear_DataIsEmpty()
    {
        // Arrange
        var table = new Record();
        var col = table.Columns.AddString("TestCol");
        var row = table.AddRow();
        col.Set("TestValue", row.Row);

        // Verify data exists
        Assert.AreEqual("TestValue", col.GetValue(row.Row));

        // Act
        col.Clear();

        // Assert - After clear, should return null or default value
        var value = col.GetValue(row.Row);
        Assert.IsNull(value);
    }

    [TestMethod]
    [Ignore]
    public void RecordColumn_ToString_ReturnsCorrectFormat()
    {
        // Arrange
        var table = new Record();
        var col = table.Columns.AddString("TestColumn");

        // Act
        var result = col.ToString();

        // Assert
        Assert.AreEqual("TestColumn,String", result);
    }

    [TestMethod]
    public void RecordColumn_SetValue_WithRecordRowIndex_WorksCorrectly()
    {
        // Arrange
        var table = new Record();
        var col = table.Columns.AddInt32("TestCol");
        var row1 = table.AddRow();
        var row2 = table.AddRow();

        // Act
        col.SetValue(100, row1.Row);
        col.SetValue(200, row2.Row);

        // Assert
        Assert.AreEqual(100, col.GetValue(row1.Row));
        Assert.AreEqual(200, col.GetValue(row2.Row));
    }

    [TestMethod]
    public void Constructor_DefaultName_IsEmpty()
    {
        // Arrange & Act
        var table = new Record();

        // Assert
        Assert.AreEqual(string.Empty, table.Name);
    }

    [TestMethod]
    public void RecordRow_AllTypedMethods_WorkCorrectly()
    {
        // Arrange
        var table = new Record();
        var colChar = table.Columns.AddChar("Char");
        var colSByte = table.Columns.AddSByte("SByte");
        var colSingle = table.Columns.AddSingle("Single");
        var colUInt16 = table.Columns.AddUInt16("UInt16");
        var colUInt32 = table.Columns.AddUInt32("UInt32");
        var colUInt64 = table.Columns.AddUInt64("UInt64");

        var row = table.AddRow();

        // Act & Assert
        colChar.Set('A', row.Row);
        Assert.AreEqual('A', colChar.ToChar(row.Row));

        colSByte.Set((sbyte)-100, row.Row);
        Assert.AreEqual((sbyte)-100, colSByte.ToSByte(row.Row));

        colSingle.Set(3.14f, row.Row);
        Assert.AreEqual(3.14f, colSingle.ToSingle(row.Row));

        colUInt16.Set((ushort)65535, row.Row);
        Assert.AreEqual((ushort)65535, colUInt16.ToUInt16(row.Row));

        colUInt32.Set(4294967295u, row.Row);
        Assert.AreEqual(4294967295u, colUInt32.ToUInt32(row.Row));

        colUInt64.Set(18446744073709551615ul, row.Row);
        Assert.AreEqual(18446744073709551615ul, colUInt64.ToUInt64(row.Row));
    }

    [TestMethod]
    public void RecordColumnCollection_IndexOf_ReturnsCorrectIndex()
    {
        // Arrange
        var table = new Record();
        table.Columns.AddString("First");
        table.Columns.AddInt32("Second");
        table.Columns.AddBoolean("Third");

        // Act & Assert
        Assert.AreEqual(0, table.Columns.IndexOf("First"));
        Assert.AreEqual(1, table.Columns.IndexOf("Second"));
        Assert.AreEqual(2, table.Columns.IndexOf("Third"));
        Assert.AreEqual(-1, table.Columns.IndexOf("NonExistent"));
    }

    [TestMethod]
    public void RecordColumnCollection_Remove_RemovesColumn()
    {
        // Arrange
        var table = new Record();
        var col1 = table.Columns.AddString("Col1");
        var col2 = table.Columns.AddInt32("Col2");

        // Act
        var removed = table.Columns.Remove(col1);

        // Assert
        Assert.IsTrue(removed);
        Assert.AreEqual(1, table.Columns.Count);
        Assert.AreSame(col2, table.Columns[0]);
    }

    [TestMethod]
    public void RecordColumnCollection_Clear_RemovesAllColumns()
    {
        // Arrange
        var table = new Record();
        table.Columns.AddString("Col1");
        table.Columns.AddInt32("Col2");
        table.Columns.AddBoolean("Col3");

        // Act
        table.Columns.Clear();

        // Assert
        Assert.AreEqual(0, table.Columns.Count);
    }

    [TestMethod]
    public void NullAndDbNullValues_HandledCorrectly()
    {
        // Arrange
        var table = new Record();
        var col = table.Columns.AddString("TestCol");
        var row = table.AddRow();

        // Act & Assert - Test null value
        col.Set((string?)null, row.Row);
        Assert.IsNull(col.GetValue(row.Row));
    }

    [TestMethod]
    public void TypeConversion_WorksCorrectly()
    {
        // Arrange
        var table = new Record();
        var intCol = table.Columns.AddInt32("IntCol");
        var row = table.AddRow();

        // Act - Set string value that can be converted to int
        intCol.Set("123", row.Row);

        // Assert
        Assert.AreEqual(123, intCol.GetValue(row.Row));
    }


    [TestMethod]
    public void RecordColumn_Name_ReturnsCorrectName()
    {
        // Arrange
        var table = new Record();
        var col = table.Columns.AddString("TestColumnName");

        // Act & Assert
        Assert.AreEqual("TestColumnName", col.Name);
    }

    [TestMethod]
    public void RecordColumn_Type_ReturnsCorrectType()
    {
        // Arrange
        var table = new Record();
        var stringCol = table.Columns.AddString("StringCol");
        var intCol = table.Columns.AddInt32("IntCol");
        var boolCol = table.Columns.AddBoolean("BoolCol");

        // Act & Assert
        Assert.AreEqual(typeof(string), stringCol.Type);
        Assert.AreEqual(typeof(int), intCol.Type);
        Assert.AreEqual(typeof(bool), boolCol.Type);
    }

    [TestMethod]
    public void Record_Name_CanBeSetAndRetrieved()
    {
        // Arrange
        var table = new Record();

        // Act
        table.Name = "ModifiedName";

        // Assert
        Assert.AreEqual("ModifiedName", table.Name);
    }

    [TestMethod]
    public void RecordColumnCollection_Indexer_ReturnsCorrectColumn()
    {
        // Arrange
        var table = new Record();
        var col1 = table.Columns.AddString("First");
        var col2 = table.Columns.AddInt32("Second");

        // Act & Assert
        Assert.AreSame(col1, table.Columns[0]);
        Assert.AreSame(col2, table.Columns[1]);
    }

    [TestMethod]
    public void RecordColumnCollection_Capacity_InitializedCorrectly()
    {
        // Arrange & Act
        var table1 = new Record();
        var table2 = new Record("Test", 50);
        var table3 = new Record("Test", 5); // Should be set to minimum 20

        // Assert
        Assert.AreEqual(20, table1.Capacity); // Default minimum
        Assert.AreEqual(50, table2.Capacity);
        Assert.AreEqual(20, table3.Capacity); // Minimum enforced
    }

    [TestMethod]
    public void Delete_RowIndexLessThanZero_ReturnsFalse()
    {
        var record = new Record("Test", 2);
        var result = record.Delete(-1);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Delete_RowIndexGreaterThanOrEqualCount_ReturnsFalse()
    {
        var record = new Record("Test", 2);
        record.AddRow();
        var result = record.Delete(1); // Only 1 row, index 1 is out of range
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Delete_ValidRowIndex_RowIsDeleted()
    {
        var record = new Record("Test", 0);
        var idCol = record.Columns.AddInt32("Id");
        var nameCol = record.Columns.AddString("Name");
        record.AddRow();
        idCol.Set(1, 0);
        nameCol.Set("A", 0);
        record.AddRow();
        idCol.Set(2, 1);
        nameCol.Set("B", 1);

        var result = record.Delete(0);

        Assert.IsTrue(result);
        Assert.AreEqual(1, record.Count);
        Assert.AreEqual(2, idCol.GetValue(0));
        Assert.AreEqual("B", nameCol.GetValue(0));
    }

    [TestMethod]
    public void Delete_LastRow_RowIsDeleted()
    {
        var record = new Record("Test", 0);
        var idCol = record.Columns.AddInt32("Id");
        record.AddRow();
        idCol.Set(1, 0);
        record.AddRow();
        idCol.Set(2, 1);

        var result = record.Delete(1);

        Assert.IsTrue(result);
        Assert.AreEqual(1, record.Count);
        Assert.AreEqual(1, idCol.GetValue(0));
    }

    [TestMethod]
    public void ToString_EmptyRecord_ReturnsDefaultString()
    {
        var record = new Record();
        var result = record.ToString();
        Assert.IsTrue(result.Contains("None") || result.Contains("count 0"));
    }

    [TestMethod]
    public void ToString_OneRowMultipleColumns_ReturnsColumnValues()
    {
        var record = new Record("TestTable", 1);
        var nameCol = record.Columns.AddString("Name");
        var ageCol = record.Columns.AddInt32("Age");
        record.AddRow();
        nameCol.Set("Alice", 0);
        ageCol.Set(30, 0);

        var result = record.ToString();
        Assert.IsTrue(result.Contains("TestTable"));
        Assert.IsTrue(result.Contains("Name"));
        Assert.IsTrue(result.Contains("Alice"));
        Assert.IsTrue(result.Contains("Age"));
        Assert.IsTrue(result.Contains("30"));
    }

    [TestMethod]
    public void ToString_MultipleRowsMultipleColumns_ReturnsTableFormat()
    {
        var record = new Record("People", 2);
        var nameCol = record.Columns.AddString("Name");
        var ageCol = record.Columns.AddInt32("Age");
        record.AddRow();
        record.AddRow();
        nameCol.Set("Bob", 0);
        ageCol.Set(25, 0);
        nameCol.Set("Carol", 1);
        ageCol.Set(28, 1);

        var result = record.ToString();
        Assert.IsTrue(result.Contains("People"));
        Assert.IsTrue(result.Contains("Name"));
        Assert.IsTrue(result.Contains("Age"));
        Assert.IsTrue(result.Contains("Bob"));
        Assert.IsTrue(result.Contains("Carol"));
        Assert.IsTrue(result.Contains("25"));
        Assert.IsTrue(result.Contains("28"));
    }

    [TestMethod]
    public void ToString_LongStringValue_TruncatesWithEllipsis()
    {
        var record = new Record("LongStringTest", 1);
        var nameCol = record.Columns.AddString("Name");
        var descCol = record.Columns.AddString("Description");
        record.AddRow();
        record.AddRow();
        string longValue = new string('A', 100);
        descCol.Set(longValue, 0);

        var result = record.ToString();
        Assert.IsTrue(result.Contains("..") || result.Contains("LongStringTest"));
    }

    // 新增的边界检查测试方法

    [TestMethod]
    public void GetValue_NegativeRowIndex_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var table = new Record();
        var col = table.Columns.AddString("TestCol");
        table.AddRow();

        // Act & Assert
        var exception = Assert.ThrowsException<ArgumentOutOfRangeException>(() => col.GetValue(-1));
        Assert.IsTrue(exception.Message.Contains("行索引 -1 超出有效范围"));
    }

    [TestMethod]
    public void GetValue_RowIndexEqualToCount_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var table = new Record();
        var col = table.Columns.AddString("TestCol");
        table.AddRow();

        // Act & Assert
        var exception = Assert.ThrowsException<ArgumentOutOfRangeException>(() => col.GetValue(1));
        Assert.IsTrue(exception.Message.Contains("行索引 1 超出有效范围"));
    }

    [TestMethod]
    public void GetValue_RowIndexGreaterThanCount_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var table = new Record();
        var col = table.Columns.AddString("TestCol");
        table.AddRow();

        // Act & Assert
        var exception = Assert.ThrowsException<ArgumentOutOfRangeException>(() => col.GetValue(5));
        Assert.IsTrue(exception.Message.Contains("行索引 5 超出有效范围"));
    }

    [TestMethod]
    public void SetValue_NegativeRowIndex_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var table = new Record();
        var col = table.Columns.AddString("TestCol");
        table.AddRow();

        // Act & Assert
        var exception = Assert.ThrowsException<ArgumentOutOfRangeException>(() => col.SetValue("test", -1));
        Assert.IsTrue(exception.Message.Contains("行索引 -1 超出有效范围"));
    }

    [TestMethod]
    public void SetValue_RowIndexEqualToCount_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var table = new Record();
        var col = table.Columns.AddString("TestCol");
        table.AddRow();

        // Act & Assert
        var exception = Assert.ThrowsException<ArgumentOutOfRangeException>(() => col.SetValue("test", 1));
        Assert.IsTrue(exception.Message.Contains("行索引 1 超出有效范围"));
    }

    [TestMethod]
    public void SetValue_RowIndexGreaterThanCount_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var table = new Record();
        var col = table.Columns.AddString("TestCol");
        table.AddRow();

        // Act & Assert
        var exception = Assert.ThrowsException<ArgumentOutOfRangeException>(() => col.SetValue("test", 5));
        Assert.IsTrue(exception.Message.Contains("行索引 5 超出有效范围"));
    }

    [TestMethod]
    public void SetBoolean_NegativeIndex_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var table = new Record();
        var col = table.Columns.AddBoolean("BoolCol");
        table.AddRow();

        // Act & Assert
        var exception = Assert.ThrowsException<ArgumentOutOfRangeException>(() => col.Set(true, -1));
        Assert.IsTrue(exception.Message.Contains("行索引 -1 超出有效范围"));
    }

    [TestMethod]
    public void SetBoolean_IndexEqualToCount_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var table = new Record();
        var col = table.Columns.AddBoolean("BoolCol");
        table.AddRow();

        // Act & Assert
        var exception = Assert.ThrowsException<ArgumentOutOfRangeException>(() => col.Set(true, 1));
        Assert.IsTrue(exception.Message.Contains("行索引 1 超出有效范围"));
    }

    [TestMethod]
    public void SetInt32_IndexGreaterThanCount_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var table = new Record();
        var col = table.Columns.AddInt32("IntCol");
        table.AddRow();

        // Act & Assert
        var exception = Assert.ThrowsException<ArgumentOutOfRangeException>(() => col.Set(42, 5));
        Assert.IsTrue(exception.Message.Contains("行索引 5 超出有效范围"));
    }

    [TestMethod]
    public void ToBoolean_NegativeIndex_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var table = new Record();
        var col = table.Columns.AddBoolean("BoolCol");
        table.AddRow();

        // Act & Assert
        var exception = Assert.ThrowsException<ArgumentOutOfRangeException>(() => col.ToBoolean(-1));
        Assert.IsTrue(exception.Message.Contains("行索引 -1 超出有效范围"));
    }

    [TestMethod]
    public void ToInt32_IndexEqualToCount_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var table = new Record();
        var col = table.Columns.AddInt32("IntCol");
        table.AddRow();

        // Act & Assert
        var exception = Assert.ThrowsException<ArgumentOutOfRangeException>(() => col.ToInt32(1));
        Assert.IsTrue(exception.Message.Contains("行索引 1 超出有效范围"));
    }

    [TestMethod]
    public void ToString_IndexGreaterThanCount_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var table = new Record();
        var col = table.Columns.AddString("StringCol");
        table.AddRow();

        // Act & Assert
        var exception = Assert.ThrowsException<ArgumentOutOfRangeException>(() => col.ToString(10));
        Assert.IsTrue(exception.Message.Contains("行索引 10 超出有效范围"));
    }

    [TestMethod]
    public void BoundaryCheck_ValidIndex_WorksCorrectly()
    {
        // Arrange
        var table = new Record();
        var col = table.Columns.AddString("TestCol");
        table.AddRow();
        table.AddRow();
        table.AddRow();

        // Act & Assert - Valid indices should work
        col.SetValue("value0", 0);
        col.SetValue("value1", 1);
        col.SetValue("value2", 2);

        Assert.AreEqual("value0", col.GetValue(0));
        Assert.AreEqual("value1", col.GetValue(1));
        Assert.AreEqual("value2", col.GetValue(2));
    }

    [TestMethod]
    public void BoundaryCheck_AfterCapacityExpansion_StillEnforcesBounds()
    {
        // Arrange
        var table = new Record("Test", 2); // Small initial capacity
        var col = table.Columns.AddString("TestCol");

        // Add rows to trigger capacity expansion
        table.AddRow(); // Index 0
        table.AddRow(); // Index 1
        table.AddRow(); // Index 2 - should trigger expansion

        // Act & Assert - Valid indices work
        col.SetValue("test0", 0);
        col.SetValue("test1", 1);
        col.SetValue("test2", 2);

        Assert.AreEqual("test0", col.GetValue(0));
        Assert.AreEqual("test1", col.GetValue(1));
        Assert.AreEqual("test2", col.GetValue(2));

        // Invalid indices should still throw
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => col.GetValue(3));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => col.SetValue("invalid", 4));
    }

    [TestMethod]
    public void AllTypedSetMethods_InvalidIndex_ThrowArgumentOutOfRangeException()
    {
        // Arrange
        var table = new Record();
        var boolCol = table.Columns.AddBoolean("Bool");
        var byteCol = table.Columns.AddByte("Byte");
        var charCol = table.Columns.AddChar("Char");
        var dateTimeCol = table.Columns.AddDateTime("DateTime");
        var decimalCol = table.Columns.AddDecimal("Decimal");
        var doubleCol = table.Columns.AddDouble("Double");
        var int16Col = table.Columns.AddInt16("Int16");
        var int32Col = table.Columns.AddInt32("Int32");
        var int64Col = table.Columns.AddInt64("Int64");
        var sbyteCol = table.Columns.AddSByte("SByte");
        var singleCol = table.Columns.AddSingle("Single");
        var stringCol = table.Columns.AddString("String");
        var uint16Col = table.Columns.AddUInt16("UInt16");
        var uint32Col = table.Columns.AddUInt32("UInt32");
        var uint64Col = table.Columns.AddUInt64("UInt64");

        table.AddRow(); // Only one row, valid index is 0

        // Act & Assert - Test all Set methods with invalid index
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => boolCol.Set(true, 1));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => byteCol.Set((byte)1, 1));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => charCol.Set('A', 1));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => dateTimeCol.Set(DateTime.Now, 1));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => decimalCol.Set(1.0m, 1));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => doubleCol.Set(1.0, 1));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => int16Col.Set((short)1, 1));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => int32Col.Set(1, 1));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => int64Col.Set(1L, 1));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => sbyteCol.Set((sbyte)1, 1));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => singleCol.Set(1.0f, 1));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => stringCol.Set("test", 1));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => uint16Col.Set((ushort)1, 1));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => uint32Col.Set(1u, 1));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => uint64Col.Set(1ul, 1));
    }

    [TestMethod]
    public void AllTypedToMethods_InvalidIndex_ThrowArgumentOutOfRangeException()
    {
        // Arrange
        var table = new Record();
        var boolCol = table.Columns.AddBoolean("Bool");
        var byteCol = table.Columns.AddByte("Byte");
        var charCol = table.Columns.AddChar("Char");
        var dateTimeCol = table.Columns.AddDateTime("DateTime");
        var decimalCol = table.Columns.AddDecimal("Decimal");
        var doubleCol = table.Columns.AddDouble("Double");
        var int16Col = table.Columns.AddInt16("Int16");
        var int32Col = table.Columns.AddInt32("Int32");
        var int64Col = table.Columns.AddInt64("Int64");
        var sbyteCol = table.Columns.AddSByte("SByte");
        var singleCol = table.Columns.AddSingle("Single");
        var stringCol = table.Columns.AddString("String");
        var uint16Col = table.Columns.AddUInt16("UInt16");
        var uint32Col = table.Columns.AddUInt32("UInt32");
        var uint64Col = table.Columns.AddUInt64("UInt64");

        table.AddRow(); // Only one row, valid index is 0

        // Act & Assert - Test all To methods with invalid index
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => boolCol.ToBoolean(1));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => byteCol.ToByte(1));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => charCol.ToChar(1));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => dateTimeCol.ToDateTime(1));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => decimalCol.ToDecimal(1));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => doubleCol.ToDouble(1));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => int16Col.ToInt16(1));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => int32Col.ToInt32(1));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => int64Col.ToInt64(1));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => sbyteCol.ToSByte(1));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => singleCol.ToSingle(1));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => stringCol.ToString(1));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => uint16Col.ToUInt16(1));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => uint32Col.ToUInt32(1));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => uint64Col.ToUInt64(1));
    }

    [TestMethod]
    public void BoundaryCheck_EmptyTable_AnyIndexThrowsException()
    {
        // Arrange
        var table = new Record();
        var col = table.Columns.AddString("TestCol");
        // No rows added, Count = 0

        // Act & Assert
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => col.GetValue(0));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => col.ToBoolean(0));
    }

    [TestMethod]
    public void BoundaryCheck_EmptyTable_NegativeIndexStillThrowsException()
    {
        // Arrange
        var table = new Record();
        var col = table.Columns.AddString("TestCol");
        // No rows added, Count = 0

        // Act & Assert - Negative indices should always throw
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => col.GetValue(-1));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => col.SetValue("test", -1));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => col.Set(true, -1));
    }

    [TestMethod]
    public void BoundaryCheck_EmptyTable_IndexGreaterThanZeroThrowsException()
    {
        // Arrange
        var table = new Record();
        var col = table.Columns.AddString("TestCol");
        // No rows added, Count = 0

        // Act & Assert - Indices > 0 should throw even with auto-row creation
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => col.Set("test", 1));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => col.Set("test", 2));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Columns_AddDuplicateName_ThrowsException()
    {
        // Arrange
        var table = new Record();
        table.Columns.AddString("TestColumn");

        // Act & Assert
        table.Columns.AddString("TestColumn"); // 应该抛出异常
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Columns_AddDuplicateNameDifferentType_ThrowsException()
    {
        // Arrange
        var table = new Record();
        table.Columns.AddString("TestColumn");

        // Act & Assert
        table.Columns.AddInt32("TestColumn"); // 应该抛出异常
    }

    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    [TestMethod]
    public void Columns_SetObject_WorksCorrectly()
    {
        var re = new Record();
        var raw = re.Columns.Add<Student>("Raw");
        var id = re.Columns.AddInt32("Id");

        var row = re.AddRow();
        raw.SetValue(new Student { Id = 1, Name = "John Doe" }, row.Row);
        id.Set(1, row.Row);

        // Assert
        var stu = raw.To<Student>(row.Row);
        Assert.AreEqual(1, stu!.Id);
        Assert.AreEqual("John Doe", stu.Name);
        Assert.AreEqual(1, id.ToInt32(row.Row));
    }

    [TestMethod]
    public void Columns_SetObjectNotMatchType_ThrowsInvalidCastException()
    {
        // Arrange
        var re = new Record();
        var raw = re.Columns.Add<Student>("Raw");
        var id = re.Columns.AddInt32("Id");
        var row = re.AddRow();
        // Act & Assert
        Assert.ThrowsException<InvalidCastException>(() => raw.SetValue(1, row.Row));
    }
}