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
        var colId = table.Columns.Add("Id", TypeCode.Int32);
        var colName = table.Columns.Add("Name", TypeCode.String);

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
        var colId = table.Columns.Add("Id", TypeCode.Int32);
        var colName = table.Columns.Add("Name", TypeCode.String);
        var colAge = table.Columns.Add("Age", TypeCode.Int32);

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
        table.Columns.Add("Id", TypeCode.Int32);
        table.Columns.Add("Name", TypeCode.String);
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
        var colId = table.Columns.Add("Id", TypeCode.Int32);
        var colName = table.Columns.Add("Name", TypeCode.String);
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
        table.Columns.Add("Id", TypeCode.Int32);
        table.Columns.Add("Name", TypeCode.String);

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
        var colId = table.Columns.Add("Id", TypeCode.Int32);
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
        Assert.AreEqual(10, colId.GetValue(row0));
        Assert.AreEqual(20, colId.GetValue(row1));
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
        var colId = table.Columns.Add("Id", TypeCode.Int32);
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
        var colBool = table.Columns.Add("Bool", TypeCode.Boolean);
        var colByte = table.Columns.Add("Byte", TypeCode.Byte);
        var colDateTime = table.Columns.Add("DateTime", TypeCode.DateTime);
        var colDecimal = table.Columns.Add("Decimal", TypeCode.Decimal);
        var colDouble = table.Columns.Add("Double", TypeCode.Double);
        var colInt16 = table.Columns.Add("Int16", TypeCode.Int16);
        var colInt32 = table.Columns.Add("Int32", TypeCode.Int32);
        var colInt64 = table.Columns.Add("Int64", TypeCode.Int64);
        var colString = table.Columns.Add("String", TypeCode.String);

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
        var colInt = table.Columns.Add("Int", TypeCode.Int32);
        var colString = table.Columns.Add("String", TypeCode.String);
        var colBool = table.Columns.Add("Bool", TypeCode.Boolean);
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
    public void Columns_AddDuplicateName_ReturnsSameColumn()
    {
        // Arrange
        var table = new Record();

        // Act
        var col1 = table.Columns.Add("TestColumn", TypeCode.String);
        var col2 = table.Columns.Add("TestColumn", TypeCode.String);

        // Assert
        Assert.AreSame(col1, col2);
        Assert.AreEqual(1, table.Columns.Count);
    }

    [TestMethod]
    public void Columns_Find_ExistingColumn_ReturnsColumn()
    {
        // Arrange
        var table = new Record();
        var originalCol = table.Columns.Add("TestColumn", TypeCode.Int32);

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
        table.Columns.Add("TestColumn", TypeCode.Int32);

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
        Assert.AreEqual(TypeCode.Boolean, colBool.Code);
        Assert.AreEqual(TypeCode.Byte, colByte.Code);
        Assert.AreEqual(TypeCode.Char, colChar.Code);
        Assert.AreEqual(TypeCode.DateTime, colDateTime.Code);
        Assert.AreEqual(TypeCode.Decimal, colDecimal.Code);
        Assert.AreEqual(TypeCode.Double, colDouble.Code);
        Assert.AreEqual(TypeCode.Int16, colInt16.Code);
        Assert.AreEqual(TypeCode.Int32, colInt32.Code);
        Assert.AreEqual(TypeCode.Int64, colInt64.Code);
        Assert.AreEqual(TypeCode.SByte, colSByte.Code);
        Assert.AreEqual(TypeCode.Single, colSingle.Code);
        Assert.AreEqual(TypeCode.String, colString.Code);
        Assert.AreEqual(TypeCode.UInt16, colUInt16.Code);
        Assert.AreEqual(TypeCode.UInt32, colUInt32.Code);
        Assert.AreEqual(TypeCode.UInt64, colUInt64.Code);
        Assert.AreEqual(15, table.Columns.Count);
    }

    [TestMethod]
    public void RecordColumn_Clear_DataIsEmpty()
    {
        // Arrange
        var table = new Record();
        var col = table.Columns.Add("TestCol", TypeCode.String);
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
        var col = table.Columns.Add("TestColumn", TypeCode.String);

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
        var col = table.Columns.Add("TestCol", TypeCode.Int32);
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
        var colChar = table.Columns.Add("Char", TypeCode.Char);
        var colSByte = table.Columns.Add("SByte", TypeCode.SByte);
        var colSingle = table.Columns.Add("Single", TypeCode.Single);
        var colUInt16 = table.Columns.Add("UInt16", TypeCode.UInt16);
        var colUInt32 = table.Columns.Add("UInt32", TypeCode.UInt32);
        var colUInt64 = table.Columns.Add("UInt64", TypeCode.UInt64);

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
        table.Columns.Add("First", TypeCode.String);
        table.Columns.Add("Second", TypeCode.Int32);
        table.Columns.Add("Third", TypeCode.Boolean);

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
        var col1 = table.Columns.Add("Col1", TypeCode.String);
        var col2 = table.Columns.Add("Col2", TypeCode.Int32);

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
        table.Columns.Add("Col1", TypeCode.String);
        table.Columns.Add("Col2", TypeCode.Int32);
        table.Columns.Add("Col3", TypeCode.Boolean);

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
        var col = table.Columns.Add("TestCol", TypeCode.String);
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
        var intCol = table.Columns.Add("IntCol", TypeCode.Int32);
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
        var schema = new RecordSchema();

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
        table.Columns.Add("Col1", TypeCode.String);
        table.Columns.Add("Col2", TypeCode.Int32);
        table.AddRow();
        table.AddRow();

        // Act
        var schema = new RecordSchema(table);

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
        Assert.ThrowsException<ArgumentNullException>(() => new RecordSchema(null!));
    }

    [TestMethod]
    public void RecordSchema_FromString_WithEmptyString_ReturnsDefault()
    {
        // Act
        var schema1 = RecordSchema.FromString("");
        var schema2 = RecordSchema.FromString(null!);
        var schema3 = RecordSchema.FromString("   ");

        // Assert
        Assert.AreEqual(1, schema1.Version);
        Assert.AreEqual(string.Empty, schema1.Name);
        Assert.AreEqual(0, schema1.Columns);
        Assert.AreEqual(0, schema1.Count);

        Assert.AreEqual(1, schema2.Version);
        Assert.AreEqual(string.Empty, schema2.Name);
        Assert.AreEqual(0, schema2.Columns);
        Assert.AreEqual(0, schema2.Count);

        Assert.AreEqual(1, schema3.Version);
        Assert.AreEqual(string.Empty, schema3.Name);
        Assert.AreEqual(0, schema3.Columns);
        Assert.AreEqual(0, schema3.Count);
    }

    [TestMethod]
    public void RecordColumn_Name_ReturnsCorrectName()
    {
        // Arrange
        var table = new Record();
        var col = table.Columns.Add("TestColumnName", TypeCode.String);

        // Act & Assert
        Assert.AreEqual("TestColumnName", col.Name);
    }

    [TestMethod]
    public void RecordColumn_Type_ReturnsCorrectType()
    {
        // Arrange
        var table = new Record();
        var stringCol = table.Columns.Add("StringCol", TypeCode.String);
        var intCol = table.Columns.Add("IntCol", TypeCode.Int32);
        var boolCol = table.Columns.Add("BoolCol", TypeCode.Boolean);

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
        var col1 = table.Columns.Add("First", TypeCode.String);
        var col2 = table.Columns.Add("Second", TypeCode.Int32);

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
}
