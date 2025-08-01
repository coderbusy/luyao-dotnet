﻿using System;
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
        var colId = table.Columns.AddInternal("Id", RecordDataType.Int32);
        var colName = table.Columns.AddInternal("Name", RecordDataType.String);

        // Act
        var row = table.AddRow();
        row.Set(1, colId);
        row.Set("Test", colName);

        // Assert
        Assert.AreEqual(1, table.Count);
        Assert.AreEqual(2, table.Columns.Count);
        Assert.AreEqual(1, colId.GetValue(row));
        Assert.AreEqual("Test", colName.GetValue(row));
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
        var colId = table.Columns.AddInternal("Id", RecordDataType.Int32);
        var colName = table.Columns.AddInternal("Name", RecordDataType.String);
        var colAge = table.Columns.AddInternal("Age", RecordDataType.Int32);

        // Act
        var row1 = table.AddRow();
        row1.Set(1, colId);
        row1.Set("Alice", colName);
        row1.Set(25, colAge);

        var row2 = table.AddRow();
        row2.Set(2, colId);
        row2.Set("Bob", colName);
        row2.Set(30, colAge);

        // Assert
        Assert.AreEqual(2, table.Count);
        Assert.AreEqual(3, table.Columns.Count);

        Assert.AreEqual(1, colId.GetValue(row1));
        Assert.AreEqual("Alice", colName.GetValue(row1));
        Assert.AreEqual(25, colAge.GetValue(row1));

        Assert.AreEqual(2, colId.GetValue(row2));
        Assert.AreEqual("Bob", colName.GetValue(row2));
        Assert.AreEqual(30, colAge.GetValue(row2));
    }

    [TestMethod]
    public void SetValue_ByColumnName_WorksCorrectly()
    {
        // Arrange
        var table = new Record();
        table.Columns.AddInternal("Id", RecordDataType.Int32);
        table.Columns.AddInternal("Name", RecordDataType.String);
        table.AddRow();

        // Act
        table.SetValue("Id", 0, 42);
        table.SetValue("Name", 0, "TestUser");

        // Assert
        Assert.AreEqual(42, table.GetValue("Id", 0));
        Assert.AreEqual("TestUser", table.GetValue("Name", 0));
    }

    [TestMethod]
    public void GetValue_ByColumnName_WorksCorrectly()
    {
        // Arrange
        var table = new Record();
        var colId = table.Columns.AddInternal("Id", RecordDataType.Int32);
        var colName = table.Columns.AddInternal("Name", RecordDataType.String);
        var row = table.AddRow();
        row.Set(123, colId);
        row.Set("TestValue", colName);

        // Act & Assert
        Assert.AreEqual(123, table.GetValue("Id", 0));
        Assert.AreEqual("TestValue", table.GetValue("Name", 0));
    }

    [TestMethod]
    public void Contains_ExistingColumn_ReturnsTrue()
    {
        // Arrange
        var table = new Record();
        table.Columns.AddInternal("Id", RecordDataType.Int32);
        table.Columns.AddInternal("Name", RecordDataType.String);

        // Act & Assert
        Assert.IsTrue(table.Contains("Id"));
        Assert.IsTrue(table.Contains("Name"));
        Assert.IsFalse(table.Contains("NonExistent"));
    }

    [TestMethod]
    public void GetValue_NonExistentColumn_ThrowsKeyNotFoundException()
    {
        // Arrange
        var table = new Record();
        table.AddRow();

        // Act & Assert
        Assert.ThrowsException<KeyNotFoundException>(() => table.GetValue("NonExistent", 0));
    }

    [TestMethod]
    public void SetValue_NonExistentColumn_ThrowsKeyNotFoundException()
    {
        // Arrange
        var table = new Record();
        table.AddRow();

        // Act & Assert
        Assert.ThrowsException<KeyNotFoundException>(() => table.SetValue("NonExistent", 0, "value"));
    }

    [TestMethod]
    public void GetRow_ValidIndex_ReturnsCorrectRow()
    {
        // Arrange
        var table = new Record();
        var colId = table.Columns.AddInternal("Id", RecordDataType.Int32);
        table.AddRow();
        table.AddRow();
        table.SetValue("Id", 0, 10);
        table.SetValue("Id", 1, 20);

        // Act
        var row0 = table.GetRow(0);
        var row1 = table.GetRow(1);

        // Assert
        Assert.AreEqual(0, row0.RowIndex);
        Assert.AreEqual(1, row1.RowIndex);
        Assert.AreEqual(10, row0.ToInt32(colId));
        Assert.AreEqual(20, row1.ToInt32(colId));
    }

    [TestMethod]
    public void GetRow_InvalidIndex_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var table = new Record();
        table.AddRow();

        // Act & Assert
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => table.GetRow(-1));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => table.GetRow(1));
    }

    [TestMethod]
    public void Enumeration_WorksCorrectly()
    {
        // Arrange
        var table = new Record();
        var colId = table.Columns.AddInternal("Id", RecordDataType.Int32);
        table.AddRow();
        table.AddRow();
        table.AddRow();
        table.SetValue("Id", 0, 1);
        table.SetValue("Id", 1, 2);
        table.SetValue("Id", 2, 3);

        // Act
        var rows = new List<RecordRow>();
        foreach (var row in table)
        {
            rows.Add(row);
        }

        // Assert
        Assert.AreEqual(3, rows.Count);
        Assert.AreEqual(0, rows[0].RowIndex);
        Assert.AreEqual(1, rows[1].RowIndex);
        Assert.AreEqual(2, rows[2].RowIndex);
        Assert.AreEqual(1, colId.GetValue(rows[0]));
        Assert.AreEqual(2, colId.GetValue(rows[1]));
        Assert.AreEqual(3, colId.GetValue(rows[2]));
    }

    [TestMethod]
    public void DifferentDataTypes_WorkCorrectly()
    {
        // Arrange
        var table = new Record();
        var colBool = table.Columns.AddInternal("Bool", RecordDataType.Boolean);
        var colByte = table.Columns.AddInternal("Byte", RecordDataType.Byte);
        var colDateTime = table.Columns.AddInternal("DateTime", RecordDataType.DateTime);
        var colDecimal = table.Columns.AddInternal("Decimal", RecordDataType.Decimal);
        var colDouble = table.Columns.AddInternal("Double", RecordDataType.Double);
        var colInt16 = table.Columns.AddInternal("Int16", RecordDataType.Int16);
        var colInt32 = table.Columns.AddInternal("Int32", RecordDataType.Int32);
        var colInt64 = table.Columns.AddInternal("Int64", RecordDataType.Int64);
        var colString = table.Columns.AddInternal("String", RecordDataType.String);

        var row = table.AddRow();
        var testDate = new DateTime(2023, 12, 25);

        // Act
        row.Set(true, colBool);
        row.Set((byte)255, colByte);
        row.Set(testDate, colDateTime);
        row.Set(123.45m, colDecimal);
        row.Set(123.456, colDouble);
        row.Set((short)1000, colInt16);
        row.Set(100000, colInt32);
        row.Set(10000000000L, colInt64);
        row.Set("Hello World", colString);

        // Assert
        Assert.AreEqual(true, colBool.GetValue(row));
        Assert.AreEqual((byte)255, colByte.GetValue(row));
        Assert.AreEqual(testDate, colDateTime.GetValue(row));
        Assert.AreEqual(123.45m, colDecimal.GetValue(row));
        Assert.AreEqual(123.456, colDouble.GetValue(row));
        Assert.AreEqual((short)1000, colInt16.GetValue(row));
        Assert.AreEqual(100000, colInt32.GetValue(row));
        Assert.AreEqual(10000000000L, colInt64.GetValue(row));
        Assert.AreEqual("Hello World", colString.GetValue(row));
    }

    [TestMethod]
    public void RowImplicitConversion_WorksCorrectly()
    {
        // Arrange
        var table = new Record();
        table.AddRow();
        table.AddRow();
        var row = table.GetRow(1);

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
        var colInt = table.Columns.AddInternal("Int", RecordDataType.Int32);
        var colString = table.Columns.AddInternal("String", RecordDataType.String);
        var colBool = table.Columns.AddInternal("Bool", RecordDataType.Boolean);
        var row = table.AddRow();

        // Act & Assert
        row.Set(42, colInt);
        Assert.AreEqual(42, row.ToInt32(colInt));

        row.Set("Test", colString);
        Assert.AreEqual("Test", row.ToString(colString));

        row.Set(true, colBool);
        Assert.AreEqual(true, row.ToBoolean(colBool));
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
        var originalCol = table.Columns.AddInternal("TestColumn", RecordDataType.Int32);

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
        table.Columns.AddInternal("TestColumn", RecordDataType.Int32);

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
        Assert.AreEqual(RecordDataType.Boolean, colBool.Code);
        Assert.AreEqual(RecordDataType.Byte, colByte.Code);
        Assert.AreEqual(RecordDataType.Char, colChar.Code);
        Assert.AreEqual(RecordDataType.DateTime, colDateTime.Code);
        Assert.AreEqual(RecordDataType.Decimal, colDecimal.Code);
        Assert.AreEqual(RecordDataType.Double, colDouble.Code);
        Assert.AreEqual(RecordDataType.Int16, colInt16.Code);
        Assert.AreEqual(RecordDataType.Int32, colInt32.Code);
        Assert.AreEqual(RecordDataType.Int64, colInt64.Code);
        Assert.AreEqual(RecordDataType.SByte, colSByte.Code);
        Assert.AreEqual(RecordDataType.Single, colSingle.Code);
        Assert.AreEqual(RecordDataType.String, colString.Code);
        Assert.AreEqual(RecordDataType.UInt16, colUInt16.Code);
        Assert.AreEqual(RecordDataType.UInt32, colUInt32.Code);
        Assert.AreEqual(RecordDataType.UInt64, colUInt64.Code);
        Assert.AreEqual(15, table.Columns.Count);
    }

    [TestMethod]
    public void RecordColumn_Clear_DataIsEmpty()
    {
        // Arrange
        var table = new Record();
        var col = table.Columns.AddInternal("TestCol", RecordDataType.String);
        var row = table.AddRow();
        row.Set("TestValue", col);

        // Verify data exists
        Assert.AreEqual("TestValue", col.GetValue(row));

        // Act
        col.Clear();

        // Assert - After clear, should return null or default value
        var value = col.GetValue(row);
        Assert.IsNull(value);
    }

    [TestMethod]
    public void RecordColumn_ToString_ReturnsCorrectFormat()
    {
        // Arrange
        var table = new Record();
        var col = table.Columns.AddInternal("TestColumn", RecordDataType.String);

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
        var col = table.Columns.AddInternal("TestCol", RecordDataType.Int32);
        var row1 = table.AddRow();
        var row2 = table.AddRow();

        // Act
        col.SetValue(100, row1);
        col.SetValue(200, row2);

        // Assert
        Assert.AreEqual(100, col.GetValue(row1));
        Assert.AreEqual(200, col.GetValue(row2));
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
        var colChar = table.Columns.AddInternal("Char", RecordDataType.Char);
        var colSByte = table.Columns.AddInternal("SByte", RecordDataType.SByte);
        var colSingle = table.Columns.AddInternal("Single", RecordDataType.Single);
        var colUInt16 = table.Columns.AddInternal("UInt16", RecordDataType.UInt16);
        var colUInt32 = table.Columns.AddInternal("UInt32", RecordDataType.UInt32);
        var colUInt64 = table.Columns.AddInternal("UInt64", RecordDataType.UInt64);

        var row = table.AddRow();

        // Act & Assert
        row.Set('A', colChar);
        Assert.AreEqual('A', row.ToChar(colChar));

        row.Set((sbyte)-100, colSByte);
        Assert.AreEqual((sbyte)-100, row.ToSByte(colSByte));

        row.Set(3.14f, colSingle);
        Assert.AreEqual(3.14f, row.ToSingle(colSingle));

        row.Set((ushort)65535, colUInt16);
        Assert.AreEqual((ushort)65535, row.ToUInt16(colUInt16));

        row.Set(4294967295u, colUInt32);
        Assert.AreEqual(4294967295u, row.ToUInt32(colUInt32));

        row.Set(18446744073709551615ul, colUInt64);
        Assert.AreEqual(18446744073709551615ul, row.ToUInt64(colUInt64));
    }

    [TestMethod]
    public void RecordColumnCollection_IndexOf_ReturnsCorrectIndex()
    {
        // Arrange
        var table = new Record();
        table.Columns.AddInternal("First", RecordDataType.String);
        table.Columns.AddInternal("Second", RecordDataType.Int32);
        table.Columns.AddInternal("Third", RecordDataType.Boolean);

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
        var col1 = table.Columns.AddInternal("Col1", RecordDataType.String);
        var col2 = table.Columns.AddInternal("Col2", RecordDataType.Int32);

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
        table.Columns.AddInternal("Col1", RecordDataType.String);
        table.Columns.AddInternal("Col2", RecordDataType.Int32);
        table.Columns.AddInternal("Col3", RecordDataType.Boolean);

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
        var col = table.Columns.AddInternal("TestCol", RecordDataType.String);
        var row = table.AddRow();

        // Act & Assert - Test null value
        col.Set((string)null, row);
        Assert.IsNull(col.GetValue(row));
    }

    [TestMethod]
    public void TypeConversion_WorksCorrectly()
    {
        // Arrange
        var table = new Record();
        var intCol = table.Columns.AddInternal("IntCol", RecordDataType.Int32);
        var row = table.AddRow();

        // Act - Set string value that can be converted to int
        intCol.Set("123", row);

        // Assert
        Assert.AreEqual(123, intCol.GetValue(row));
    }

    [TestMethod]
    public void RecordSchema_DefaultConstructor_InitializesCorrectly()
    {
        // Arrange & Act
        var schema = new RecordHeader();

        // Assert
        Assert.AreEqual(1, schema.Version);
        Assert.AreEqual(string.Empty, schema.Name);
        Assert.AreEqual(0, schema.Columns);
        Assert.AreEqual(0, schema.Count);
    }

    [TestMethod]
    public void RecordSchema_ConstructorWithRecord_InitializesFromRecord()
    {
        // Arrange
        var table = new Record("TestTable", 10);
        table.Columns.AddInternal("Col1", RecordDataType.String);
        table.Columns.AddInternal("Col2", RecordDataType.Int32);
        table.AddRow();
        table.AddRow();

        // Act
        var schema = new RecordHeader(table);

        // Assert
        Assert.AreEqual(1, schema.Version);
        Assert.AreEqual("TestTable", schema.Name);
        Assert.AreEqual(2, schema.Columns);
        Assert.AreEqual(2, schema.Count);
    }

    [TestMethod]
    public void RecordSchema_ConstructorWithNull_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.ThrowsException<ArgumentNullException>(() => new RecordHeader(null!));
    }


    [TestMethod]
    public void RecordColumn_Name_ReturnsCorrectName()
    {
        // Arrange
        var table = new Record();
        var col = table.Columns.AddInternal("TestColumnName", RecordDataType.String);

        // Act & Assert
        Assert.AreEqual("TestColumnName", col.Name);
    }

    [TestMethod]
    public void RecordColumn_Type_ReturnsCorrectType()
    {
        // Arrange
        var table = new Record();
        var stringCol = table.Columns.AddInternal("StringCol", RecordDataType.String);
        var intCol = table.Columns.AddInternal("IntCol", RecordDataType.Int32);
        var boolCol = table.Columns.AddInternal("BoolCol", RecordDataType.Boolean);

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
        var col1 = table.Columns.AddInternal("First", RecordDataType.String);
        var col2 = table.Columns.AddInternal("Second", RecordDataType.Int32);

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
        Assert.AreEqual(20, table1.Columns.Capacity); // Default minimum
        Assert.AreEqual(50, table2.Columns.Capacity);
        Assert.AreEqual(20, table3.Columns.Capacity); // Minimum enforced
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
        record.Columns.AddInt32("Id");
        record.Columns.AddString("Name");
        record.AddRow();
        record.SetValue("Id", 0, 1);
        record.SetValue("Name", 0, "A");
        record.AddRow();
        record.SetValue("Id", 1, 2);
        record.SetValue("Name", 1, "B");

        var result = record.Delete(0);

        Assert.IsTrue(result);
        Assert.AreEqual(1, record.Count);
        Assert.AreEqual(2, record.GetValue("Id", 0));
        Assert.AreEqual("B", record.GetValue("Name", 0));
    }

    [TestMethod]
    public void Delete_LastRow_RowIsDeleted()
    {
        var record = new Record("Test", 0);
        record.Columns.AddInt32("Id");
        record.AddRow();
        record.SetValue("Id", 0, 1);
        record.AddRow();
        record.SetValue("Id", 1, 2);

        var result = record.Delete(1);

        Assert.IsTrue(result);
        Assert.AreEqual(1, record.Count);
        Assert.AreEqual(1, record.GetValue("Id", 0));
    }


    [TestMethod]
    public void ToString_EmptyRecord_ReturnsDefaultString()
    {
        var record = new Record();
        var result = record.ToString();
        Assert.IsTrue(result.Contains("None"));
        Assert.IsTrue(result.Contains("count 0"));
    }

    [TestMethod]
    public void ToString_OneRowMultipleColumns_ReturnsColumnValues()
    {
        var record = new Record("TestTable", 1);
        record.Columns.AddString("Name");
        record.Columns.AddInt32("Age");
        record.AddRow();
        record.SetValue("Name", 0, "Alice");
        record.SetValue("Age", 0, 30);

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
        record.Columns.AddString("Name");
        record.Columns.AddInt32("Age");
        record.AddRow();
        record.AddRow();
        record.SetValue("Name", 0, "Bob");
        record.SetValue("Age", 0, 25);
        record.SetValue("Name", 1, "Carol");
        record.SetValue("Age", 1, 28);

        var result = record.ToString();
        Assert.IsTrue(result.Contains("People"));
        Assert.IsTrue(result.Contains("Name"));
        Assert.IsTrue(result.Contains("Age"));
        Assert.IsTrue(result.Contains("Bob"));
        Assert.IsTrue(result.Contains("Carol"));
        Assert.IsTrue(result.Contains("25"));
        Assert.IsTrue(result.Contains("28"));
        Assert.IsTrue(result.Contains("|"));
    }

    [TestMethod]
    public void ToString_LongStringValue_TruncatesWithEllipsis()
    {
        var record = new Record("LongStringTest", 1);
        record.Columns.AddString("Name");
        record.Columns.AddString("Description");
        record.AddRow();
        record.AddRow();
        string longValue = new string('A', 100);
        record.SetValue("Description", 0, longValue);

        var result = record.ToString();
        Assert.IsTrue(result.Contains(".."));
    }

    // 新增的边界检查测试方法

    [TestMethod]
    public void GetValue_NegativeRowIndex_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var table = new Record();
        var col = table.Columns.AddInternal("TestCol", RecordDataType.String);
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
        var col = table.Columns.AddInternal("TestCol", RecordDataType.String);
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
        var col = table.Columns.AddInternal("TestCol", RecordDataType.String);
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
        var col = table.Columns.AddInternal("TestCol", RecordDataType.String);
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
        var col = table.Columns.AddInternal("TestCol", RecordDataType.String);
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
        var col = table.Columns.AddInternal("TestCol", RecordDataType.String);
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
        var col = table.Columns.AddInternal("BoolCol", RecordDataType.Boolean);
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
        var col = table.Columns.AddInternal("BoolCol", RecordDataType.Boolean);
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
        var col = table.Columns.AddInternal("IntCol", RecordDataType.Int32);
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
        var col = table.Columns.AddInternal("BoolCol", RecordDataType.Boolean);
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
        var col = table.Columns.AddInternal("IntCol", RecordDataType.Int32);
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
        var col = table.Columns.AddInternal("StringCol", RecordDataType.String);
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
        var col = table.Columns.AddInternal("TestCol", RecordDataType.String);
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
        var col = table.Columns.AddInternal("TestCol", RecordDataType.String);

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
        var boolCol = table.Columns.AddInternal("Bool", RecordDataType.Boolean);
        var byteCol = table.Columns.AddInternal("Byte", RecordDataType.Byte);
        var charCol = table.Columns.AddInternal("Char", RecordDataType.Char);
        var dateTimeCol = table.Columns.AddInternal("DateTime", RecordDataType.DateTime);
        var decimalCol = table.Columns.AddInternal("Decimal", RecordDataType.Decimal);
        var doubleCol = table.Columns.AddInternal("Double", RecordDataType.Double);
        var int16Col = table.Columns.AddInternal("Int16", RecordDataType.Int16);
        var int32Col = table.Columns.AddInternal("Int32", RecordDataType.Int32);
        var int64Col = table.Columns.AddInternal("Int64", RecordDataType.Int64);
        var sbyteCol = table.Columns.AddInternal("SByte", RecordDataType.SByte);
        var singleCol = table.Columns.AddInternal("Single", RecordDataType.Single);
        var stringCol = table.Columns.AddInternal("String", RecordDataType.String);
        var uint16Col = table.Columns.AddInternal("UInt16", RecordDataType.UInt16);
        var uint32Col = table.Columns.AddInternal("UInt32", RecordDataType.UInt32);
        var uint64Col = table.Columns.AddInternal("UInt64", RecordDataType.UInt64);

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
        var boolCol = table.Columns.AddInternal("Bool", RecordDataType.Boolean);
        var byteCol = table.Columns.AddInternal("Byte", RecordDataType.Byte);
        var charCol = table.Columns.AddInternal("Char", RecordDataType.Char);
        var dateTimeCol = table.Columns.AddInternal("DateTime", RecordDataType.DateTime);
        var decimalCol = table.Columns.AddInternal("Decimal", RecordDataType.Decimal);
        var doubleCol = table.Columns.AddInternal("Double", RecordDataType.Double);
        var int16Col = table.Columns.AddInternal("Int16", RecordDataType.Int16);
        var int32Col = table.Columns.AddInternal("Int32", RecordDataType.Int32);
        var int64Col = table.Columns.AddInternal("Int64", RecordDataType.Int64);
        var sbyteCol = table.Columns.AddInternal("SByte", RecordDataType.SByte);
        var singleCol = table.Columns.AddInternal("Single", RecordDataType.Single);
        var stringCol = table.Columns.AddInternal("String", RecordDataType.String);
        var uint16Col = table.Columns.AddInternal("UInt16", RecordDataType.UInt16);
        var uint32Col = table.Columns.AddInternal("UInt32", RecordDataType.UInt32);
        var uint64Col = table.Columns.AddInternal("UInt64", RecordDataType.UInt64);

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
        var col = table.Columns.AddInternal("TestCol", RecordDataType.String);
        // No rows added, Count = 0

        // Act & Assert
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => col.GetValue(0));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => col.SetValue("test", 0));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => col.ToBoolean(0));
    }

    [TestMethod]
    public void BoundaryCheck_EmptyTable_NegativeIndexStillThrowsException()
    {
        // Arrange
        var table = new Record();
        var col = table.Columns.AddInternal("TestCol", RecordDataType.String);
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
        var col = table.Columns.AddInternal("TestCol", RecordDataType.String);
        // No rows added, Count = 0

        // Act & Assert - Indices > 0 should throw even with auto-row creation
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => col.Set("test", 1));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => col.Set("test", 2));
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Columns_AddDuplicateName_ThrowsException()
    {
        // Arrange
        var table = new Record();
        table.Columns.AddString("TestColumn");

        // Act & Assert
        table.Columns.AddString("TestColumn"); // 应该抛出异常
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
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
        public string Name { get; set; }
    }

    [TestMethod]
    public void Columns_SetObject_WorksCorrectly()
    {
        var re = new Record();
        var raw = re.Columns.Add<Student>("Raw");
        var id = re.Columns.AddInt32("Id");

        var row = re.AddRow();
        row.SetValue(new Student { Id = 1, Name = "John Doe" }, raw);
        row.Set(1, id);

        // Assert

        var stu = row.To<Student>(raw);
        Assert.AreEqual(1, stu.Id);
        Assert.AreEqual("John Doe", stu.Name);
        Assert.AreEqual(1, row.ToInt32(id));
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
        Assert.ThrowsException<InvalidCastException>(() => row.SetValue(1, raw));
    }
}
