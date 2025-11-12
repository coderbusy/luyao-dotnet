using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Data;

[TestClass]
public class RecordObjectTests
{
    #region 测试模型类

    /// <summary>
    /// 用于测试的简单数据模型
    /// </summary>
    public class TestModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal Price { get; set; }
    }

    /// <summary>
    /// 用于测试的空类
    /// </summary>
    public class EmptyModel
    {
    }

    /// <summary>
    /// 用于测试的复杂数据模型
    /// </summary>
    public class ComplexModel
    {
        public int IntValue { get; set; }
        public long LongValue { get; set; }
        public double DoubleValue { get; set; }
        public float FloatValue { get; set; }
        public byte ByteValue { get; set; }
        public char CharValue { get; set; }
        public bool BoolValue { get; set; }
        public string StringValue { get; set; } = string.Empty;
        public DateTime DateTimeValue { get; set; }
        public decimal DecimalValue { get; set; }
        public short ShortValue { get; set; }
        public sbyte SByteValue { get; set; }
        public ushort UShortValue { get; set; }
        public uint UIntValue { get; set; }
        public ulong ULongValue { get; set; }
    }

    #endregion

    #region Add<T> 方法测试

    /// <summary>
    /// 测试 Add 方法能够成功添加对象并创建列结构
    /// </summary>
    [TestMethod]
    public void Add_ValidObject_ShouldCreateColumnsAndAddRow()
    {
        // Arrange
        var record = new Record();
        var testModel = new TestModel { Id = 1, Name = "Test", IsActive = true, CreatedDate = DateTime.Now, Price = 100.50m };

        // Act
        var row = record.Add(testModel);

        // Assert
        Assert.IsNotNull(row);
        Assert.AreEqual(1, record.Count);
        Assert.AreEqual(5, record.Columns.Count); // TestModel 有 5 个属性
        Assert.IsTrue(record.Columns.Any(c => c.Name == nameof(TestModel.Id)));
        Assert.IsTrue(record.Columns.Any(c => c.Name == nameof(TestModel.Name)));
        Assert.IsTrue(record.Columns.Any(c => c.Name == nameof(TestModel.IsActive)));
        Assert.IsTrue(record.Columns.Any(c => c.Name == nameof(TestModel.CreatedDate)));
        Assert.IsTrue(record.Columns.Any(c => c.Name == nameof(TestModel.Price)));
    }

    /// <summary>
    /// 测试 Add 方法传入 null 对象时抛出异常
    /// </summary>
    [TestMethod]
    public void Add_NullObject_ShouldThrowArgumentNullException()
    {
        // Arrange
        var record = new Record();

        // Act & Assert
        Assert.ThrowsException<ArgumentNullException>(() => record.Add<TestModel>(null!));
    }

    /// <summary>
    /// 测试 Add 方法对空类的处理
    /// </summary>
    [TestMethod]
    public void Add_EmptyObject_ShouldCreateRowWithNoColumns()
    {
        // Arrange
        var record = new Record();
        var emptyModel = new EmptyModel();

        // Act
        var row = record.Add(emptyModel);

        // Assert
        Assert.IsNotNull(row);
        Assert.AreEqual(1, record.Count);
        Assert.AreEqual(0, record.Columns.Count);
    }

    #endregion

    #region From<T> 单个对象方法测试

    /// <summary>
    /// 测试 From 方法能够从单个对象创建 Record
    /// </summary>
    [TestMethod]
    public void From_SingleObject_ShouldCreateRecordWithData()
    {
        // Arrange
        var testModel = new TestModel
        {
            Id = 1,
            Name = "Test",
            IsActive = true,
            CreatedDate = new DateTime(2023, 1, 1),
            Price = 100.50m
        };

        // Act
        var record = Record.From(testModel);

        // Assert
        Assert.IsNotNull(record);
        Assert.AreEqual(1, record.Count);
        Assert.AreEqual(5, record.Columns.Count);

        // 验证数据是否正确填充
        record.Cursor = 0;
        Assert.AreEqual(1, record.Get<int>(nameof(TestModel.Id)));
        Assert.AreEqual("Test", record.Get<string>(nameof(TestModel.Name)));
        Assert.AreEqual(true, record.Get<Boolean>(nameof(TestModel.IsActive)));
        Assert.AreEqual(new DateTime(2023, 1, 1), record.Get<DateTime>(nameof(TestModel.CreatedDate)));
        Assert.AreEqual(100.50m, record.Get<Decimal>(nameof(TestModel.Price)));
    }

    /// <summary>
    /// 测试 From 方法传入 null 对象时抛出异常
    /// </summary>
    [TestMethod]
    public void From_SingleObject_Null_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.ThrowsException<ArgumentNullException>(() => Record.From<TestModel>((TestModel)null!));
    }

    /// <summary>
    /// 测试 From 方法处理复杂数据类型
    /// </summary>
    [TestMethod]
    public void From_ComplexObject_ShouldCreateRecordWithAllTypes()
    {
        // Arrange
        var complexModel = new ComplexModel
        {
            IntValue = 42,
            LongValue = 1234567890L,
            DoubleValue = 3.14159,
            FloatValue = 2.71f,
            ByteValue = 255,
            CharValue = 'A',
            BoolValue = true,
            StringValue = "Complex Test",
            DateTimeValue = new DateTime(2023, 12, 25),
            DecimalValue = 999.99m,
            ShortValue = 32767,
            SByteValue = -128,
            UShortValue = 65535,
            UIntValue = 4294967295,
            ULongValue = 18446744073709551615
        };

        // Act
        var record = Record.From(complexModel);

        // Assert
        Assert.IsNotNull(record);
        Assert.AreEqual(1, record.Count);
        Assert.AreEqual(15, record.Columns.Count);

        // 验证各种数据类型
        record.Cursor = 0;
        Assert.AreEqual(42, record.Get<Int32>(nameof(ComplexModel.IntValue)));
        Assert.AreEqual(1234567890L, record.Get<long>(nameof(ComplexModel.LongValue)));
        Assert.AreEqual(3.14159, record.Get<double>(nameof(ComplexModel.DoubleValue)), 0.00001);
        Assert.AreEqual(2.71f, record.Get<float>(nameof(ComplexModel.FloatValue)), 0.001f);
        Assert.AreEqual((byte)255, record.Get<byte>(nameof(ComplexModel.ByteValue)));
        Assert.AreEqual('A', record.Get<char>(nameof(ComplexModel.CharValue)));
        Assert.AreEqual(true, record.Get<bool>(nameof(ComplexModel.BoolValue)));
        Assert.AreEqual("Complex Test", record.Get<string>(nameof(ComplexModel.StringValue)));
        Assert.AreEqual(new DateTime(2023, 12, 25), record.Get<DateTime>(nameof(ComplexModel.DateTimeValue)));
        Assert.AreEqual(999.99m, record.Get<decimal>(nameof(ComplexModel.DecimalValue)));
        Assert.AreEqual((short)32767, record.Get<short>(nameof(ComplexModel.ShortValue)));
        Assert.AreEqual((sbyte)-128, record.Get<sbyte>(nameof(ComplexModel.SByteValue)));
        Assert.AreEqual((ushort)65535, record.Get<ushort>(nameof(ComplexModel.UShortValue)));
        Assert.AreEqual(4294967295u, record.Get<uint>(nameof(ComplexModel.UIntValue)));
        Assert.AreEqual(18446744073709551615ul, record.Get<ulong>(nameof(ComplexModel.ULongValue)));
    }

    #endregion

    #region From<T> 集合方法测试

    /// <summary>
    /// 测试 From 方法能够从对象集合创建 Record
    /// </summary>
    [TestMethod]
    public void From_Collection_ShouldCreateRecordWithMultipleRows()
    {
        // Arrange
        var testModels = new List<TestModel>
        {
            new TestModel { Id = 1, Name = "First", IsActive = true, CreatedDate = new DateTime(2023, 1, 1), Price = 100.00m },
            new TestModel { Id = 2, Name = "Second", IsActive = false, CreatedDate = new DateTime(2023, 2, 1), Price = 200.00m },
            new TestModel { Id = 3, Name = "Third", IsActive = true, CreatedDate = new DateTime(2023, 3, 1), Price = 300.00m }
        };

        // Act
        var record = Record.FromList(testModels);

        // Assert
        Assert.IsNotNull(record);
        Assert.AreEqual(3, record.Count);
        Assert.AreEqual(5, record.Columns.Count);

        // 验证每行数据
        var list = record.ToList<TestModel>();
        Assert.AreEqual(3, list.Count);
        Assert.AreEqual(1, list[0].Id);
        Assert.AreEqual("First", list[0].Name);
        Assert.AreEqual(2, list[1].Id);
        Assert.AreEqual("Second", list[1].Name);
        Assert.AreEqual(3, list[2].Id);
        Assert.AreEqual("Third", list[2].Name);
    }

    /// <summary>
    /// 测试 From 方法处理空集合
    /// </summary>
    [TestMethod]
    public void From_EmptyCollection_ShouldCreateEmptyRecord()
    {
        // Arrange
        var emptyList = new List<TestModel>();

        // Act
        var record = Record.FromList(emptyList);

        // Assert
        Assert.IsNotNull(record);
        Assert.AreEqual(0, record.Count);
        Assert.AreEqual(5, record.Columns.Count); // 列结构仍然会被创建
    }

    /// <summary>
    /// 测试 From 方法处理包含 null 元素的集合
    /// </summary>
    [TestMethod]
    public void From_CollectionWithNulls_ShouldSkipNullItems()
    {
        // Arrange
        var testModels = new List<TestModel>
        {
            new TestModel { Id = 1, Name = "First", IsActive = true, CreatedDate = new DateTime(2023, 1, 1), Price = 100.00m },
            null,
            new TestModel { Id = 2, Name = "Second", IsActive = false, CreatedDate = new DateTime(2023, 2, 1), Price = 200.00m },
            null
        };

        // Act
        var record = Record.FromList(testModels!);

        // Assert
        Assert.IsNotNull(record);
        Assert.AreEqual(2, record.Count); // null 元素被跳过
        Assert.AreEqual(5, record.Columns.Count);
    }

    /// <summary>
    /// 测试 From 方法传入 null 集合时抛出异常
    /// </summary>
    [TestMethod]
    public void From_NullCollection_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.ThrowsException<ArgumentNullException>(() => Record.FromList<TestModel>((TestModel[])null!));
    }

    #endregion

    #region To<T> 方法测试

    /// <summary>
    /// 测试 To 方法能够将记录转换为对象
    /// </summary>
    [TestMethod]
    public void To_ValidRecord_ShouldReturnObjectWithData()
    {
        // Arrange
        var originalModel = new TestModel
        {
            Id = 1,
            Name = "Test",
            IsActive = true,
            CreatedDate = new DateTime(2023, 1, 1),
            Price = 100.50m
        };
        var record = Record.From(originalModel);

        // Act
        var result = record.To<TestModel>();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(originalModel.Id, result.Id);
        Assert.AreEqual(originalModel.Name, result.Name);
        Assert.AreEqual(originalModel.IsActive, result.IsActive);
        Assert.AreEqual(originalModel.CreatedDate, result.CreatedDate);
        Assert.AreEqual(originalModel.Price, result.Price);
    }

    /// <summary>
    /// 测试 To 方法处理空记录
    /// </summary>
    [TestMethod]
    public void To_EmptyRecord_ShouldReturnObjectWithDefaultValues()
    {
        // Arrange
        var record = new Record();

        // Act
        var result = record.To<TestModel>();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Id);
        Assert.AreEqual(string.Empty, result.Name);
        Assert.AreEqual(false, result.IsActive);
        Assert.AreEqual(default(DateTime), result.CreatedDate);
        Assert.AreEqual(0m, result.Price);
    }

    /// <summary>
    /// 测试 To 方法处理复杂数据类型
    /// </summary>
    [TestMethod]
    public void To_ComplexRecord_ShouldReturnComplexObjectWithData()
    {
        // Arrange
        var originalModel = new ComplexModel
        {
            IntValue = 42,
            StringValue = "Test",
            BoolValue = true,
            DateTimeValue = new DateTime(2023, 1, 1),
            DecimalValue = 123.45m
        };
        var record = Record.From(originalModel);

        // Act
        var result = record.To<ComplexModel>();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(originalModel.IntValue, result.IntValue);
        Assert.AreEqual(originalModel.StringValue, result.StringValue);
        Assert.AreEqual(originalModel.BoolValue, result.BoolValue);
        Assert.AreEqual(originalModel.DateTimeValue, result.DateTimeValue);
        Assert.AreEqual(originalModel.DecimalValue, result.DecimalValue);
    }

    #endregion

    #region ToList<T> 方法测试

    /// <summary>
    /// 测试 ToList 方法能够将记录转换为对象列表
    /// </summary>
    [TestMethod]
    public void ToList_ValidRecord_ShouldReturnObjectList()
    {
        // Arrange
        var originalModels = new List<TestModel>
        {
            new TestModel { Id = 1, Name = "First", IsActive = true, CreatedDate = new DateTime(2023, 1, 1), Price = 100.00m },
            new TestModel { Id = 2, Name = "Second", IsActive = false, CreatedDate = new DateTime(2023, 2, 1), Price = 200.00m },
            new TestModel { Id = 3, Name = "Third", IsActive = true, CreatedDate = new DateTime(2023, 3, 1), Price = 300.00m }
        };
        var record = Record.FromList(originalModels);

        // Act
        var result = record.ToList<TestModel>();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(3, result.Count);

        for (int i = 0; i < originalModels.Count; i++)
        {
            Assert.AreEqual(originalModels[i].Id, result[i].Id);
            Assert.AreEqual(originalModels[i].Name, result[i].Name);
            Assert.AreEqual(originalModels[i].IsActive, result[i].IsActive);
            Assert.AreEqual(originalModels[i].CreatedDate, result[i].CreatedDate);
            Assert.AreEqual(originalModels[i].Price, result[i].Price);
        }
    }

    /// <summary>
    /// 测试 ToList 方法处理空记录
    /// </summary>
    [TestMethod]
    public void ToList_EmptyRecord_ShouldReturnEmptyList()
    {
        // Arrange
        var record = new Record();

        // Act
        var result = record.ToList<TestModel>();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }

    /// <summary>
    /// 测试 ToList 方法处理单行记录
    /// </summary>
    [TestMethod]
    public void ToList_SingleRowRecord_ShouldReturnSingleItemList()
    {
        // Arrange
        var originalModel = new TestModel
        {
            Id = 1,
            Name = "Test",
            IsActive = true,
            CreatedDate = new DateTime(2023, 1, 1),
            Price = 100.50m
        };
        var record = Record.From(originalModel);

        // Act
        var result = record.ToList<TestModel>();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(originalModel.Id, result[0].Id);
        Assert.AreEqual(originalModel.Name, result[0].Name);
        Assert.AreEqual(originalModel.IsActive, result[0].IsActive);
        Assert.AreEqual(originalModel.CreatedDate, result[0].CreatedDate);
        Assert.AreEqual(originalModel.Price, result[0].Price);
    }

    /// <summary>
    /// 测试 ToList 方法返回的列表容量
    /// </summary>
    [TestMethod]
    public void ToList_ShouldReturnListWithCorrectCapacity()
    {
        // Arrange
        var models = Enumerable.Range(1, 10)
            .Select(i => new TestModel { Id = i, Name = $"Test{i}" })
            .ToList();
        var record = Record.FromList(models);

        // Act
        var result = record.ToList<TestModel>();

        // Assert
        Assert.AreEqual(record.Count, result.Count);
        Assert.AreEqual(10, result.Count);
    }

    #endregion

    #region 集成测试

    /// <summary>
    /// 测试完整的往返转换：对象 -> Record -> 对象
    /// </summary>
    [TestMethod]
    public void RoundTripConversion_ShouldPreserveData()
    {
        // Arrange
        var originalModel = new TestModel
        {
            Id = 42,
            Name = "Round Trip Test",
            IsActive = true,
            CreatedDate = new DateTime(2023, 6, 15, 14, 30, 0),
            Price = 1234.56m
        };

        // Act
        var record = Record.From(originalModel);
        var convertedModel = record.To<TestModel>();

        // Assert
        Assert.AreEqual(originalModel.Id, convertedModel.Id);
        Assert.AreEqual(originalModel.Name, convertedModel.Name);
        Assert.AreEqual(originalModel.IsActive, convertedModel.IsActive);
        Assert.AreEqual(originalModel.CreatedDate, convertedModel.CreatedDate);
        Assert.AreEqual(originalModel.Price, convertedModel.Price);
    }

    /// <summary>
    /// 测试完整的往返转换：列表 -> Record -> 列表
    /// </summary>
    [TestMethod]
    public void RoundTripConversion_List_ShouldPreserveData()
    {
        // Arrange
        var originalModels = new List<TestModel>
        {
            new TestModel { Id = 1, Name = "First", IsActive = true, CreatedDate = new DateTime(2023, 1, 1), Price = 100.00m },
            new TestModel { Id = 2, Name = "Second", IsActive = false, CreatedDate = new DateTime(2023, 2, 1), Price = 200.00m },
            new TestModel { Id = 3, Name = "Third", IsActive = true, CreatedDate = new DateTime(2023, 3, 1), Price = 300.00m }
        };

        // Act
        var record = Record.FromList(originalModels);
        var convertedModels = record.ToList<TestModel>();

        // Assert
        Assert.AreEqual(originalModels.Count, convertedModels.Count);
        for (int i = 0; i < originalModels.Count; i++)
        {
            Assert.AreEqual(originalModels[i].Id, convertedModels[i].Id);
            Assert.AreEqual(originalModels[i].Name, convertedModels[i].Name);
            Assert.AreEqual(originalModels[i].IsActive, convertedModels[i].IsActive);
            Assert.AreEqual(originalModels[i].CreatedDate, convertedModels[i].CreatedDate);
            Assert.AreEqual(originalModels[i].Price, convertedModels[i].Price);
        }
    }

    #endregion
}
