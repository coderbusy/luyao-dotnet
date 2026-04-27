using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Data;

[TestClass]
public class FrameTests
{
    [TestMethod]
    public void Constructor_ValidParameters_InitializesCorrectly()
    {
        // Arrange
        var dt = new Frame();

        // Assert
        Assert.AreEqual(0, dt.Count);
        Assert.IsNotNull(dt.Columns);
        Assert.AreEqual(0, dt.Columns.Count);
    }

    [TestMethod]
    public void AddRowAndSetColumnValue_WorksCorrectly()
    {
        // Arrange
        var table = new Frame();
        var colId = table.Columns.Add<int>("Id"); // 替换 Add<Int32> 为 Add<int>
        var colName = table.Columns.Add<string>("Name"); // 替换 Add<String> 为 Add<string>

        // Act
        var row = table.AddRow();
        colId.SetValue(row.Row, 1);
        colName.SetValue(row.Row, "Test");

        // Assert
        Assert.AreEqual(1, table.Count);
        Assert.AreEqual(2, table.Columns.Count);
        Assert.AreEqual(1, colId.To<int>(row.Row)); // 替换 GetValue 为 Get<T>
        Assert.AreEqual("Test", colName.To<string>(row.Row)); // 替换 GetValue 为 Get<T>
    }

    [TestMethod]
    public void Constructor_WithNameAndRows_InitializesCorrectly()
    {
        // Arrange & Act
        var table = new Frame("TestTable", 10);

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
        var table = new Frame();
        var colId = table.Columns.Add<int>("Id"); // 替换 Add<Int32> 为 Add<int>
        var colName = table.Columns.Add<string>("Name"); // 替换 Add<String> 为 Add<string>
        var colAge = table.Columns.Add<int>("Age"); // 替换 Add<Int32> 为 Add<int>

        // Act
        var row1 = table.AddRow();
        colId.SetValue(row1.Row, 1);
        colName.SetValue(row1.Row, "Alice");
        colAge.SetValue(row1.Row, 25);

        var row2 = table.AddRow();
        colId.SetValue(row2.Row, 2);
        colName.SetValue(row2.Row, "Bob");
        colAge.SetValue(row2.Row, 30);

        // Assert
        Assert.AreEqual(2, table.Count);
        Assert.AreEqual(3, table.Columns.Count);

        Assert.AreEqual(1, colId.To<int>(row1.Row)); // 替换 GetValue 为 Get<T>
        Assert.AreEqual("Alice", colName.To<string>(row1.Row)); // 替换 GetValue 为 Get<T>
        Assert.AreEqual(25, colAge.To<int>(row1.Row)); // 替换 GetValue 为 Get<T>

        Assert.AreEqual(2, colId.To<int>(row2.Row)); // 替换 GetValue 为 Get<T>
        Assert.AreEqual("Bob", colName.To<string>(row2.Row)); // 替换 GetValue 为 Get<T>
        Assert.AreEqual(30, colAge.To<int>(row2.Row)); // 替换 GetValue 为 Get<T>
    }

    [TestMethod]
    public void SetValue_ByColumnName_WorksCorrectly()
    {
        // Arrange
        var table = new Frame();
        var idCol = table.Columns.Add<int>("Id");
        var nameCol = table.Columns.Add<string>("Name");
        table.AddRow();

        // Act
        idCol.SetValue(0, 42);
        nameCol.SetValue(0, "TestUser");

        // Assert
        Assert.AreEqual(42, idCol.To<int>(0));
        Assert.AreEqual("TestUser", nameCol.To<string>(0));
    }

    [TestMethod]
    public void GetValue_ByColumnName_WorksCorrectly()
    {
        // Arrange
        var table = new Frame();
        var colId = table.Columns.Add<int>("Id");
        var colName = table.Columns.Add<string>("Name");
        var row = table.AddRow();
        colId.SetValue(row.Row, 123);
        colName.SetValue(row.Row, "TestValue");

        // Act & Assert
        Assert.AreEqual(123, colId.To<int>(0));
        Assert.AreEqual("TestValue", colName.To<string>(0));
    }

    [TestMethod]
    public void Contains_ExistingColumn_ReturnsTrue()
    {
        // Arrange
        var table = new Frame();
        table.Columns.Add<int>("Id");
        table.Columns.Add<string>("Name");

        // Act & Assert
        Assert.IsTrue(table.Columns.Find("Id") != null);
        Assert.IsTrue(table.Columns.Find("Name") != null);
        Assert.IsFalse(table.Columns.Find("NonExistent") != null);
    }

    [TestMethod]
    public void GetValue_NonExistentColumn_ThrowsKeyNotFoundException()
    {
        // Arrange
        var table = new Frame();
        table.AddRow();

        // Act & Assert
        var col = table.Columns.Find("NonExistent");
        Assert.IsNull(col);
    }

    [TestMethod]
    public void SetValue_NonExistentColumn_ThrowsKeyNotFoundException()
    {
        // Arrange
        var table = new Frame();
        table.AddRow();

        // Act & Assert
        var col = table.Columns.Find("NonExistent");
        Assert.IsNull(col);
    }

    [TestMethod]
    public void GetRow_ValidIndex_ReturnsCorrectRow()
    {
        // Arrange
        var table = new Frame();
        var colId = table.Columns.Add<int>("Id");
        table.AddRow();
        table.AddRow();
        colId.SetValue(0, 10);
        colId.SetValue(1, 20);

        // Act
        var rows = table.ToArray();
        var row0 = rows[0];
        var row1 = rows[1];

        // Assert
        Assert.AreEqual(0, row0.Row);
        Assert.AreEqual(1, row1.Row);
        Assert.AreEqual(10, colId.To<int>(row0.Row));
        Assert.AreEqual(20, colId.To<int>(row1.Row));
    }

    [TestMethod]
    public void GetRow_InvalidIndex_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var table = new Frame();
        table.AddRow();

        // Act & Assert
        var rows = table.ToArray();
        Assert.AreEqual(1, rows.Length);
    }

    [TestMethod]
    public void Enumeration_WorksCorrectly()
    {
        // Arrange
        var table = new Frame();
        var colId = table.Columns.Add<int>("Id");
        table.AddRow();
        table.AddRow();
        table.AddRow();
        colId.SetValue(0, 1);
        colId.SetValue(1, 2);
        colId.SetValue(2, 3);

        // Act
        var rows = new List<FrameRow>();
        foreach (var row in table)
        {
            rows.Add(row);
        }

        // Assert
        Assert.AreEqual(3, rows.Count);
        Assert.AreEqual(0, rows[0].Row);
        Assert.AreEqual(1, rows[1].Row);
        Assert.AreEqual(2, rows[2].Row);
        Assert.AreEqual(1, colId.To<int>(rows[0].Row));
        Assert.AreEqual(2, colId.To<int>(rows[1].Row));
        Assert.AreEqual(3, colId.To<int>(rows[2].Row));
    }

    [TestMethod]
    public void DifferentDataTypes_WorkCorrectly()
    {
        // Arrange
        var table = new Frame();
        var colBool = table.Columns.Add<Boolean>("Bool");
        var colByte = table.Columns.Add<Byte>("Byte");
        var colDateTime = table.Columns.Add<DateTime>("DateTime");
        var colDecimal = table.Columns.Add<Decimal>("Decimal");
        var colDouble = table.Columns.Add<Double>("Double");
        var colInt16 = table.Columns.Add<Int16>("Int16");
        var colInt32 = table.Columns.Add<Int32>("Int32");
        var colInt64 = table.Columns.Add<Int64>("Int64");
        var colString = table.Columns.Add<String>("String");

        var row = table.AddRow();
        var testDate = new DateTime(2023, 12, 25);

        // Act
        colBool.SetValue(row.Row, true);
        colByte.SetValue(row.Row, (byte)255);
        colDateTime.SetValue(row.Row, testDate);
        colDecimal.SetValue(row.Row, 123.45m);
        colDouble.SetValue(row.Row, 123.456);
        colInt16.SetValue(row.Row, (short)1000);
        colInt32.SetValue(row.Row, 100000);
        colInt64.SetValue(row.Row, 10000000000L);
        colString.SetValue(row.Row, "Hello World");

        // Assert
        Assert.AreEqual(true, (object?)colBool.Get(row.Row));
        Assert.AreEqual((byte)255, (object?)colByte.Get(row.Row));
        Assert.AreEqual(testDate, (object?)colDateTime.Get(row.Row));
        Assert.AreEqual(123.45m, (object?)colDecimal.Get(row.Row));
        Assert.AreEqual(123.456, (object?)colDouble.Get(row.Row));
        Assert.AreEqual((short)1000, (object?)colInt16.Get(row.Row));
        Assert.AreEqual(100000, (object?)colInt32.Get(row.Row));
        Assert.AreEqual(10000000000L, (object?)colInt64.Get(row.Row));
        Assert.AreEqual("Hello World", (object?)colString.Get(row.Row));
    }

    [TestMethod]
    public void RowImplicitConversion_WorksCorrectly()
    {
        // Arrange
        var table = new Frame();
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
        var table = new Frame();
        var colInt = table.Columns.Add<Int32>("Int");
        var colString = table.Columns.Add<String>("String");
        var colBool = table.Columns.Add<Boolean>("Bool");
        var row = table.AddRow();

        // Act & Assert
        colInt.SetValue(row.Row, 42);
        Assert.AreEqual(42, colInt.To<Int32>(row.Row));

        colString.SetValue(row.Row, "Test");
        Assert.AreEqual("Test", colString.To<String>(row.Row));

        colBool.SetValue(row.Row, true);
        Assert.AreEqual(true, colBool.To<Boolean>(row.Row));
    }

    [TestMethod]
    public void AddRow_MultipleRows_CountIncreases()
    {
        // Arrange
        var table = new Frame();

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
        var table = new Frame();
        var originalCol = table.Columns.Add<Int32>("TestColumn");

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
        var table = new Frame();
        table.Columns.Add<Int32>("TestColumn");

        // Act
        var foundCol = table.Columns.Find("NonExistent");

        // Assert
        Assert.IsNull(foundCol);
    }

    [TestMethod]
    public void FrameColumn_Clear_DataIsEmpty()
    {
        // Arrange
        var table = new Frame();
        var col = table.Columns.Add<String>("TestCol");
        var row = table.AddRow();
        col.SetValue(row.Row, "TestValue");

        // Verify data exists
        Assert.AreEqual("TestValue", (object?)col.Get(row.Row));

        // Act
        col.Clear();

        // Assert - After clear, should return null or default value
        var value = col.Get(row.Row);
        Assert.IsNull(value);
    }

    [TestMethod]
    public void FrameColumn_SetValue_WithFrameRowIndex_WorksCorrectly()
    {
        // Arrange
        var table = new Frame();
        var col = table.Columns.Add<Int32>("TestCol");
        var row1 = table.AddRow();
        var row2 = table.AddRow();

        // Act
        col.Set(row1.Row, (object)100);
        col.Set(row2.Row, (object)200);

        // Assert
        Assert.AreEqual(100, (object?)col.Get(row1.Row));
        Assert.AreEqual(200, (object?)col.Get(row2.Row));
    }

    [TestMethod]
    public void Constructor_DefaultName_IsEmpty()
    {
        // Arrange & Act
        var table = new Frame();

        // Assert
        Assert.AreEqual(string.Empty, table.Name);
    }

    [TestMethod]
    public void FrameRow_AllTypedMethods_WorkCorrectly()
    {
        // Arrange
        var table = new Frame();
        var colChar = table.Columns.Add<Char>("Char");
        var colSByte = table.Columns.Add<SByte>("SByte");
        var colSingle = table.Columns.Add<Single>("Single");
        var colUInt16 = table.Columns.Add<UInt16>("UInt16");
        var colUInt32 = table.Columns.Add<UInt32>("UInt32");
        var colUInt64 = table.Columns.Add<UInt64>("UInt64");

        var row = table.AddRow();

        // Act & Assert
        colChar.SetValue(row.Row, 'A');
        Assert.AreEqual('A', colChar.To<Char>(row.Row));

        colSByte.SetValue(row.Row, (sbyte)-100);
        Assert.AreEqual((sbyte)-100, colSByte.To<SByte>(row.Row));

        colSingle.SetValue(row.Row, 3.14f);
        Assert.AreEqual(3.14f, colSingle.To<Single>(row.Row));

        colUInt16.SetValue(row.Row, (ushort)65535);
        Assert.AreEqual((ushort)65535, colUInt16.To<UInt16>(row.Row));

        colUInt32.SetValue(row.Row, 4294967295u);
        Assert.AreEqual(4294967295u, colUInt32.To<UInt32>(row.Row));

        colUInt64.SetValue(row.Row, 18446744073709551615ul);
        Assert.AreEqual(18446744073709551615ul, colUInt64.To<UInt64>(row.Row));
    }

    [TestMethod]
    public void FrameColumnCollection_IndexOf_ReturnsCorrectIndex()
    {
        // Arrange
        var table = new Frame();
        table.Columns.Add<String>("First");
        table.Columns.Add<Int32>("Second");
        table.Columns.Add<Boolean>("Third");

        // Act & Assert
        Assert.AreEqual(0, table.Columns.IndexOf("First"));
        Assert.AreEqual(1, table.Columns.IndexOf("Second"));
        Assert.AreEqual(2, table.Columns.IndexOf("Third"));
        Assert.AreEqual(-1, table.Columns.IndexOf("NonExistent"));
    }

    [TestMethod]
    public void FrameColumnCollection_Remove_RemovesColumn()
    {
        // Arrange
        var table = new Frame();
        var col1 = table.Columns.Add<String>("Col1");
        var col2 = table.Columns.Add<Int32>("Col2");

        // Act
        var removed = table.Columns.Remove(col1.Name);

        // Assert
        Assert.IsTrue(removed);
        Assert.AreEqual(1, table.Columns.Count);
        Assert.AreSame(col2, table.Columns[0]);
    }

    [TestMethod]
    public void FrameColumnCollection_Clear_RemovesAllColumns()
    {
        // Arrange
        var table = new Frame();
        table.Columns.Add<String>("Col1");
        table.Columns.Add<Int32>("Col2");
        table.Columns.Add<Boolean>("Col3");

        // Act
        table.Columns.Clear();

        // Assert
        Assert.AreEqual(0, table.Columns.Count);
    }

    [TestMethod]
    public void NullAndDbNullValues_HandledCorrectly()
    {
        // Arrange
        var table = new Frame();
        var col = table.Columns.Add<String>("TestCol");
        var row = table.AddRow();

        // Act & Assert - Test null value
        col.SetValue(row.Row, (string?)null);
        Assert.IsNull(col.Get(row.Row));
    }

    [TestMethod]
    public void FrameColumn_Name_ReturnsCorrectName()
    {
        // Arrange
        var table = new Frame();
        var col = table.Columns.Add<String>("TestColumnName");

        // Act & Assert
        Assert.AreEqual("TestColumnName", col.Name);
    }

    [TestMethod]
    public void FrameColumn_Type_ReturnsCorrectType()
    {
        // Arrange
        var table = new Frame();
        var stringCol = table.Columns.Add<String>("StringCol");
        var intCol = table.Columns.Add<Int32>("IntCol");
        var boolCol = table.Columns.Add<Boolean>("BoolCol");

        // Act & Assert
        Assert.AreEqual(typeof(string), stringCol.Type);
        Assert.AreEqual(typeof(int), intCol.Type);
        Assert.AreEqual(typeof(bool), boolCol.Type);
    }

    [TestMethod]
    public void Frame_Name_CanBeSetAndRetrieved()
    {
        // Arrange
        var table = new Frame();

        // Act
        table.Name = "ModifiedName";

        // Assert
        Assert.AreEqual("ModifiedName", table.Name);
    }

    [TestMethod]
    public void FrameColumnCollection_Indexer_ReturnsCorrectColumn()
    {
        // Arrange
        var table = new Frame();
        var col1 = table.Columns.Add<String>("First");
        var col2 = table.Columns.Add<Int32>("Second");

        // Act & Assert
        Assert.AreSame(col1, table.Columns[0]);
        Assert.AreSame(col2, table.Columns[1]);
    }

    [TestMethod]
    public void FrameColumnCollection_Capacity_InitializedCorrectly()
    {
        // Arrange & Act
        var table1 = new Frame();
        var table2 = new Frame("Test", 50);
        var table3 = new Frame("Test", 5); // Should be set to minimum 20

        // Assert
        Assert.AreEqual(20, table1.Capacity); // Default minimum
        Assert.AreEqual(50, table2.Capacity);
        Assert.AreEqual(20, table3.Capacity); // Minimum enforced
    }

    [TestMethod]
    public void Delete_RowIndexLessThanZero_ReturnsFalse()
    {
        var record = new Frame("Test", 2);
        var result = record.Delete(-1);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Delete_RowIndexGreaterThanOrEqualCount_ReturnsFalse()
    {
        var record = new Frame("Test", 2);
        record.AddRow();
        var result = record.Delete(1); // Only 1 row, index 1 is out of range
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Delete_ValidRowIndex_RowIsDeleted()
    {
        var record = new Frame("Test", 0);
        var idCol = record.Columns.Add<Int32>("Id");
        var nameCol = record.Columns.Add<String>("Name");
        record.AddRow();
        idCol.SetValue(0, 1);
        nameCol.SetValue(0, "A");
        record.AddRow();
        idCol.SetValue(1, 2);
        nameCol.SetValue(1, "B");

        var result = record.Delete(0);

        Assert.IsTrue(result);
        Assert.AreEqual(1, record.Count);
        Assert.AreEqual(2, (object?)idCol.Get(0));
        Assert.AreEqual("B", (object?)nameCol.Get(0));
    }

    [TestMethod]
    public void Delete_LastRow_RowIsDeleted()
    {
        var record = new Frame("Test", 0);
        var idCol = record.Columns.Add<Int32>("Id");
        record.AddRow();
        idCol.SetValue(0, 1);
        record.AddRow();
        idCol.SetValue(1, 2);

        var result = record.Delete(1);

        Assert.IsTrue(result);
        Assert.AreEqual(1, record.Count);
        Assert.AreEqual(1, (object?)idCol.Get(0));
    }

    [TestMethod]
    public void ToString_EmptyFrame_ReturnsDefaultString()
    {
        var record = new Frame();
        var result = record.ToString();
        Assert.IsTrue(result.Contains("None") || result.Contains("count 0"));
    }

    [TestMethod]
    public void ToString_OneRowMultipleColumns_ReturnsColumnValues()
    {
        var record = new Frame("TestTable", 1);
        var nameCol = record.Columns.Add<String>("Name");
        var ageCol = record.Columns.Add<Int32>("Age");
        record.AddRow();
        nameCol.SetValue(0, "Alice");
        ageCol.SetValue(0, 30);

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
        var record = new Frame("People", 2);
        var nameCol = record.Columns.Add<String>("Name");
        var ageCol = record.Columns.Add<Int32>("Age");
        record.AddRow();
        record.AddRow();
        nameCol.SetValue(0, "Bob");
        ageCol.SetValue(0, 25);
        nameCol.SetValue(1, "Carol");
        ageCol.SetValue(1, 28);

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
        var record = new Frame("LongStringTest", 1);
        var nameCol = record.Columns.Add<String>("Name");
        var descCol = record.Columns.Add<String>("Description");
        record.AddRow();
        record.AddRow();
        string longValue = new string('A', 100);
        descCol.SetValue(0, longValue);

        var result = record.ToString();
        Assert.IsTrue(result.Contains("..") || result.Contains("LongStringTest"));
    }

    // 新增的边界检查测试方法

    [TestMethod]
    public void GetValue_NegativeRowIndex_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var table = new Frame();
        var col = table.Columns.Add<String>("TestCol");
        table.AddRow();

        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => col.Get(-1));
        Assert.IsTrue(exception.Message.Contains("行索引 -1 超出有效范围"));
    }

    [TestMethod]
    public void GetValue_RowIndexEqualToCount_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var table = new Frame();
        var col = table.Columns.Add<String>("TestCol");
        table.AddRow();

        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => col.Get(1));
        Assert.IsTrue(exception.Message.Contains("行索引 1 超出有效范围"));
    }

    [TestMethod]
    public void GetValue_RowIndexGreaterThanCount_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var table = new Frame();
        var col = table.Columns.Add<String>("TestCol");
        table.AddRow();

        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => col.Get(5));
        Assert.IsTrue(exception.Message.Contains("行索引 5 超出有效范围"));
    }

    [TestMethod]
    public void SetValue_NegativeRowIndex_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var table = new Frame();
        var col = table.Columns.Add<String>("TestCol");
        table.AddRow();

        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => col.Set(-1, (object)"test"));
        Assert.IsTrue(exception.Message.Contains("行索引 -1 超出有效范围"));
    }

    [TestMethod]
    public void SetValue_RowIndexEqualToCount_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var table = new Frame();
        var col = table.Columns.Add<String>("TestCol");
        table.AddRow();

        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => col.Set(1, (object)"test"));
        Assert.IsTrue(exception.Message.Contains("行索引 1 超出有效范围"));
    }

    [TestMethod]
    public void SetValue_RowIndexGreaterThanCount_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var table = new Frame();
        var col = table.Columns.Add<String>("TestCol");
        table.AddRow();

        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => col.Set(5, (object)"test"));
        Assert.IsTrue(exception.Message.Contains("行索引 5 超出有效范围"));
    }

    [TestMethod]
    public void SetBoolean_NegativeIndex_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var table = new Frame();
        var col = table.Columns.Add<Boolean>("BoolCol");
        table.AddRow();

        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => col.SetValue(-1, true));
        Assert.IsTrue(exception.Message.Contains("行索引 -1 超出有效范围"));
    }

    [TestMethod]
    public void SetBoolean_IndexEqualToCount_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var table = new Frame();
        var col = table.Columns.Add<Boolean>("BoolCol");
        table.AddRow();

        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => col.SetValue(1, true));
        Assert.IsTrue(exception.Message.Contains("行索引 1 超出有效范围"));
    }

    [TestMethod]
    public void SetInt32_IndexGreaterThanCount_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var table = new Frame();
        var col = table.Columns.Add<Int32>("IntCol");
        table.AddRow();

        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => col.SetValue(5, 42));
        Assert.IsTrue(exception.Message.Contains("行索引 5 超出有效范围"));
    }

    [TestMethod]
    public void ToBoolean_NegativeIndex_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var table = new Frame();
        var col = table.Columns.Add<Boolean>("BoolCol");
        table.AddRow();

        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => col.To<Boolean>(-1));
        Assert.IsTrue(exception.Message.Contains("行索引 -1 超出有效范围"));
    }

    [TestMethod]
    public void ToInt32_IndexEqualToCount_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var table = new Frame();
        var col = table.Columns.Add<Int32>("IntCol");
        table.AddRow();

        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => col.To<Int32>(1));
        Assert.IsTrue(exception.Message.Contains("行索引 1 超出有效范围"));
    }

    [TestMethod]
    public void ToString_IndexGreaterThanCount_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var table = new Frame();
        var col = table.Columns.Add<String>("StringCol");
        table.AddRow();

        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => col.To<String>(10));
        Assert.IsTrue(exception.Message.Contains("行索引 10 超出有效范围"));
    }

    [TestMethod]
    public void BoundaryCheck_ValidIndex_WorksCorrectly()
    {
        // Arrange
        var table = new Frame();
        var col = table.Columns.Add<String>("TestCol");
        table.AddRow();
        table.AddRow();
        table.AddRow();

        // Act & Assert - Valid indices should work
        col.Set(0, (object)"value0");
        col.Set(1, (object)"value1");
        col.Set(2, (object)"value2");

        Assert.AreEqual("value0", (object?)col.Get(0));
        Assert.AreEqual("value1", (object?)col.Get(1));
        Assert.AreEqual("value2", (object?)col.Get(2));
    }

    [TestMethod]
    public void BoundaryCheck_AfterCapacityExpansion_StillEnforcesBounds()
    {
        // Arrange
        var table = new Frame("Test", 2); // Small initial capacity
        var col = table.Columns.Add<String>("TestCol");

        // Add rows to trigger capacity expansion
        table.AddRow(); // Index 0
        table.AddRow(); // Index 1
        table.AddRow(); // Index 2 - should trigger expansion

        // Act & Assert - Valid indices work
        col.Set(0, (object)"test0");
        col.Set(1, (object)"test1");
        col.Set(2, (object)"test2");

        Assert.AreEqual("test0", (object?)col.Get(0));
        Assert.AreEqual("test1", (object?)col.Get(1));
        Assert.AreEqual("test2", (object?)col.Get(2));

        // Invalid indices should still throw
        Assert.Throws<ArgumentOutOfRangeException>(() => col.Get(3));
        Assert.Throws<ArgumentOutOfRangeException>(() => col.Set(4, (object)"invalid"));
    }

    [TestMethod]
    public void AllTypedSetMethods_InvalidIndex_ThrowArgumentOutOfRangeException()
    {
        // Arrange
        var table = new Frame();
        var boolCol = table.Columns.Add<Boolean>("Bool");
        var byteCol = table.Columns.Add<Byte>("Byte");
        var charCol = table.Columns.Add<Char>("Char");
        var dateTimeCol = table.Columns.Add<DateTime>("DateTime");
        var decimalCol = table.Columns.Add<Decimal>("Decimal");
        var doubleCol = table.Columns.Add<Double>("Double");
        var int16Col = table.Columns.Add<Int16>("Int16");
        var int32Col = table.Columns.Add<Int32>("Int32");
        var int64Col = table.Columns.Add<Int64>("Int64");
        var sbyteCol = table.Columns.Add<SByte>("SByte");
        var singleCol = table.Columns.Add<Single>("Single");
        var stringCol = table.Columns.Add<String>("String");
        var uint16Col = table.Columns.Add<UInt16>("UInt16");
        var uint32Col = table.Columns.Add<UInt32>("UInt32");
        var uint64Col = table.Columns.Add<UInt64>("UInt64");

        table.AddRow(); // Only one row, valid index is 0

        // Act & Assert - Test all Set methods with invalid index
        Assert.Throws<ArgumentOutOfRangeException>(() => boolCol.SetValue(1, true));
        Assert.Throws<ArgumentOutOfRangeException>(() => byteCol.SetValue(1, (byte)1));
        Assert.Throws<ArgumentOutOfRangeException>(() => charCol.SetValue(1, 'A'));
        Assert.Throws<ArgumentOutOfRangeException>(() => dateTimeCol.SetValue(1, DateTime.Now));
        Assert.Throws<ArgumentOutOfRangeException>(() => decimalCol.SetValue(1, 1.0m));
        Assert.Throws<ArgumentOutOfRangeException>(() => doubleCol.SetValue(1, 1.0));
        Assert.Throws<ArgumentOutOfRangeException>(() => int16Col.SetValue(1, (short)1));
        Assert.Throws<ArgumentOutOfRangeException>(() => int32Col.SetValue(1, 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => int64Col.SetValue(1, 1L));
        Assert.Throws<ArgumentOutOfRangeException>(() => sbyteCol.SetValue(1, (sbyte)1));
        Assert.Throws<ArgumentOutOfRangeException>(() => singleCol.SetValue(1, 1.0f));
        Assert.Throws<ArgumentOutOfRangeException>(() => stringCol.SetValue(1, "test"));
        Assert.Throws<ArgumentOutOfRangeException>(() => uint16Col.SetValue(1, (ushort)1));
        Assert.Throws<ArgumentOutOfRangeException>(() => uint32Col.SetValue(1, 1u));
        Assert.Throws<ArgumentOutOfRangeException>(() => uint64Col.SetValue(1, 1ul));
    }

    [TestMethod]
    public void AllTypedToMethods_InvalidIndex_ThrowArgumentOutOfRangeException()
    {
        // Arrange
        var table = new Frame();
        var boolCol = table.Columns.Add<Boolean>("Bool");
        var byteCol = table.Columns.Add<Byte>("Byte");
        var charCol = table.Columns.Add<Char>("Char");
        var dateTimeCol = table.Columns.Add<DateTime>("DateTime");
        var decimalCol = table.Columns.Add<Decimal>("Decimal");
        var doubleCol = table.Columns.Add<Double>("Double");
        var int16Col = table.Columns.Add<Int16>("Int16");
        var int32Col = table.Columns.Add<Int32>("Int32");
        var int64Col = table.Columns.Add<Int64>("Int64");
        var sbyteCol = table.Columns.Add<SByte>("SByte");
        var singleCol = table.Columns.Add<Single>("Single");
        var stringCol = table.Columns.Add<String>("String");
        var uint16Col = table.Columns.Add<UInt16>("UInt16");
        var uint32Col = table.Columns.Add<UInt32>("UInt32");
        var uint64Col = table.Columns.Add<UInt64>("UInt64");

        table.AddRow(); // Only one row, valid index is 0

        // Act & Assert - Test all To methods with invalid index
        Assert.Throws<ArgumentOutOfRangeException>(() => boolCol.To<Boolean>(1));
        Assert.Throws<ArgumentOutOfRangeException>(() => byteCol.To<Byte>(1));
        Assert.Throws<ArgumentOutOfRangeException>(() => charCol.To<Char>(1));
        Assert.Throws<ArgumentOutOfRangeException>(() => dateTimeCol.To<DateTime>(1));
        Assert.Throws<ArgumentOutOfRangeException>(() => decimalCol.To<Decimal>(1));
        Assert.Throws<ArgumentOutOfRangeException>(() => doubleCol.To<Double>(1));
        Assert.Throws<ArgumentOutOfRangeException>(() => int16Col.To<Int16>(1));
        Assert.Throws<ArgumentOutOfRangeException>(() => int32Col.To<Int32>(1));
        Assert.Throws<ArgumentOutOfRangeException>(() => int64Col.To<Int64>(1));
        Assert.Throws<ArgumentOutOfRangeException>(() => sbyteCol.To<SByte>(1));
        Assert.Throws<ArgumentOutOfRangeException>(() => singleCol.To<Single>(1));
        Assert.Throws<ArgumentOutOfRangeException>(() => stringCol.To<String>(1));
        Assert.Throws<ArgumentOutOfRangeException>(() => uint16Col.To<UInt16>(1));
        Assert.Throws<ArgumentOutOfRangeException>(() => uint32Col.To<UInt32>(1));
        Assert.Throws<ArgumentOutOfRangeException>(() => uint64Col.To<UInt64>(1));
    }

    [TestMethod]
    public void BoundaryCheck_EmptyTable_AnyIndexThrowsException()
    {
        // Arrange
        var table = new Frame();
        var col = table.Columns.Add<String>("TestCol");
        // No rows added, Count = 0

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => col.Get(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => col.To<Boolean>(0));
    }

    [TestMethod]
    public void BoundaryCheck_EmptyTable_NegativeIndexStillThrowsException()
    {
        // Arrange
        var table = new Frame();
        var col = table.Columns.Add<String>("TestCol");
        // No rows added, Count = 0

        // Act & Assert - Negative indices should always throw
        Assert.Throws<ArgumentOutOfRangeException>(() => col.Get(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => col.Set(-1, (object)"test"));
    }

    [TestMethod]
    public void BoundaryCheck_EmptyTable_IndexGreaterThanZeroThrowsException()
    {
        // Arrange
        var table = new Frame();
        var col = table.Columns.Add<String>("TestCol");
        // No rows added, Count = 0

        // Act & Assert - Indices > 0 should throw even with auto-row creation
        Assert.Throws<ArgumentOutOfRangeException>(() => col.SetValue(1, "test"));
        Assert.Throws<ArgumentOutOfRangeException>(() => col.SetValue(2, "test"));
    }

    [TestMethod]
    public void Columns_AddDuplicateName_ReturnsExisting()
    {
        // Arrange
        var table = new Frame();
        var col1 = table.Columns.Add<String>("TestColumn");

        // Act
        var col2 = table.Columns.Add<String>("TestColumn");

        // Assert - should return the same column, not throw
        Assert.AreSame(col1, col2);
        Assert.AreEqual(1, table.Columns.Count);
    }

    [TestMethod]
    public void Columns_AddDuplicateNameDifferentType_ThrowsInvalidOperationException()
    {
        // Arrange
        var table = new Frame();
        table.Columns.Add<String>("TestColumn");

        // Act & Assert - 同名不同类型应抛 InvalidOperationException（不再是 InvalidCastException）
        Assert.Throws<InvalidOperationException>(() => table.Columns.Add<Int32>("TestColumn"));
    }

    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    [TestMethod]
    public void Columns_SetObject_WorksCorrectly()
    {
        var re = new Frame();
        var name = re.Columns.Add<string>("Name");
        var id = re.Columns.Add<Int32>("Id");

        var row = re.AddRow();
        name.Set(row.Row, (object)"John Doe");
        id.SetValue(row.Row, 1);

        // Assert
        Assert.AreEqual("John Doe", name.Get(row.Row));
        Assert.AreEqual(1, id.To<Int32>(row.Row));
    }

    [TestMethod]
    public void Columns_SetObjectNotMatchType_ThrowsInvalidCastException()
    {
        // Arrange
        var re = new Frame();
        var id = re.Columns.Add<Int32>("Id");
        var row = re.AddRow();
        // Act & Assert - DateTime cannot be converted to int via SetValue
        Assert.Throws<InvalidCastException>(() => id.Set(row.Row, DateTime.Now));
    }

    [TestMethod]
    public void AddRowFromValues_ExactNumberOfValues_SetsAllColumns()
    {
        // Arrange
        var table = new Frame();
        var colId = table.Columns.Add<Int32>("Id");
        var colName = table.Columns.Add<String>("Name");
        var colAge = table.Columns.Add<Int32>("Age");

        // Act
        var row = table.AddRowFromValues(1, "Alice", 25);

        // Assert
        Assert.AreEqual(1, table.Count);
        Assert.AreEqual(1, colId.To<Int32>(row.Row));
        Assert.AreEqual("Alice", colName.To<String>(row.Row));
        Assert.AreEqual(25, colAge.To<Int32>(row.Row));
    }

    [TestMethod]
    public void AddRowFromValues_FewerValuesThanColumns_SetsOnlyProvidedValues()
    {
        // Arrange
        var table = new Frame();
        var colId = table.Columns.Add<Int32>("Id");
        var colName = table.Columns.Add<String>("Name");
        var colAge = table.Columns.Add<Int32>("Age");

        // Act
        var row = table.AddRowFromValues(1, "Bob");

        // Assert
        Assert.AreEqual(1, table.Count);
        Assert.AreEqual(1, colId.To<Int32>(row.Row));
        Assert.AreEqual("Bob", colName.To<String>(row.Row));
        // colAge should remain default (not set)
        Assert.AreEqual(0, colAge.To<Int32>(row.Row));
    }

    [TestMethod]
    public void AddRowFromValues_MoreValuesThanColumns_IgnoresExtraValues()
    {
        // Arrange
        var table = new Frame();
        var colId = table.Columns.Add<Int32>("Id");
        var colName = table.Columns.Add<String>("Name");

        // Act
        var row = table.AddRowFromValues(1, "Charlie", 30, "ExtraValue");

        // Assert
        Assert.AreEqual(1, table.Count);
        Assert.AreEqual(1, colId.To<Int32>(row.Row));
        Assert.AreEqual("Charlie", colName.To<String>(row.Row));
        // Extra values should be ignored without errors
    }

    [TestMethod]
    public void AddRowFromValues_EmptyValuesArray_CreatesEmptyRow()
    {
        // Arrange
        var table = new Frame();
        var colId = table.Columns.Add<Int32>("Id");
        var colName = table.Columns.Add<String>("Name");

        // Act
        var row = table.AddRowFromValues();

        // Assert
        Assert.AreEqual(1, table.Count);
        // Columns should have default values
        Assert.AreEqual(0, colId.To<Int32>(row.Row));
        Assert.IsNull(colName.Get(row.Row));
    }

    [TestMethod]
    public void AddRowFromValues_NoColumns_CreatesRowWithoutErrors()
    {
        // Arrange
        var table = new Frame();

        // Act
        var row = table.AddRowFromValues(1, "Test", 30);

        // Assert
        Assert.AreEqual(1, table.Count);
        Assert.AreEqual(0, row.Row);
        // No columns, so values should be ignored
    }

    [TestMethod]
    public void AddRowFromValues_DifferentDataTypes_SetsCorrectly()
    {
        // Arrange
        var table = new Frame();
        var colBool = table.Columns.Add<Boolean>("Bool");
        var colByte = table.Columns.Add<Byte>("Byte");
        var colDateTime = table.Columns.Add<DateTime>("DateTime");
        var colDecimal = table.Columns.Add<Decimal>("Decimal");
        var colDouble = table.Columns.Add<Double>("Double");
        var testDate = new DateTime(2023, 12, 25);

        // Act
        var row = table.AddRowFromValues(true, (byte)255, testDate, 123.45m, 456.78);

        // Assert
        Assert.AreEqual(1, table.Count);
        Assert.AreEqual(true, colBool.To<Boolean>(row.Row));
        Assert.AreEqual((byte)255, colByte.To<Byte>(row.Row));
        Assert.AreEqual(testDate, colDateTime.To<DateTime>(row.Row));
        Assert.AreEqual(123.45m, colDecimal.To<Decimal>(row.Row));
        Assert.AreEqual(456.78, colDouble.To<Double>(row.Row));
    }

    [TestMethod]
    public void AddRowFromValues_WithNullValues_SetsNullCorrectly()
    {
        // Arrange
        var table = new Frame();
        var colId = table.Columns.Add<Int32>("Id");
        var colName = table.Columns.Add<String>("Name");
        var colAge = table.Columns.Add<Int32>("Age");

        // Act
        object? nullValue = null;
        var row = table.AddRowFromValues(1, nullValue!, 25);

        // Assert
        Assert.AreEqual(1, table.Count);
        Assert.AreEqual(1, colId.To<Int32>(row.Row));
        Assert.IsNull(colName.Get(row.Row));
        Assert.AreEqual(25, colAge.To<Int32>(row.Row));
    }

    [TestMethod]
    public void AddRowFromValues_MultipleRowsCalls_IncrementsCountCorrectly()
    {
        // Arrange
        var table = new Frame();
        var colId = table.Columns.Add<Int32>("Id");
        var colName = table.Columns.Add<String>("Name");

        // Act
        var row1 = table.AddRowFromValues(1, "First");
        var row2 = table.AddRowFromValues(2, "Second");
        var row3 = table.AddRowFromValues(3, "Third");

        // Assert
        Assert.AreEqual(3, table.Count);
        Assert.AreEqual(0, row1.Row);
        Assert.AreEqual(1, row2.Row);
        Assert.AreEqual(2, row3.Row);
        Assert.AreEqual(1, colId.To<Int32>(row1.Row));
        Assert.AreEqual("First", colName.To<String>(row1.Row));
        Assert.AreEqual(2, colId.To<Int32>(row2.Row));
        Assert.AreEqual("Second", colName.To<String>(row2.Row));
        Assert.AreEqual(3, colId.To<Int32>(row3.Row));
        Assert.AreEqual("Third", colName.To<String>(row3.Row));
    }

    [TestMethod]
    public void AddRowFromValues_SingleValue_SetsFirstColumnOnly()
    {
        // Arrange
        var table = new Frame();
        var colId = table.Columns.Add<Int32>("Id");
        var colName = table.Columns.Add<String>("Name");
        var colAge = table.Columns.Add<Int32>("Age");

        // Act
        var row = table.AddRowFromValues(42);

        // Assert
        Assert.AreEqual(1, table.Count);
        Assert.AreEqual(42, colId.To<Int32>(row.Row));
        // Other columns should have default values
        Assert.IsNull(colName.Get(row.Row));
        Assert.AreEqual(0, colAge.To<Int32>(row.Row));
    }
}
