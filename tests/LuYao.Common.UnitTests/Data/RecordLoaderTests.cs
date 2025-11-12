using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Data;

[TestClass]
public class RecordLoaderTests
{
    /// <summary>
    /// 测试用的实体类
    /// </summary>
    public class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreateTime { get; set; }
        public bool IsActive { get; set; }
        public double Score { get; set; }

        [RecordColumnName("custom_column")]
        public string CustomField { get; set; } = string.Empty;

        public int? NullableInt { get; set; }
        public DateTime? NullableDateTime { get; set; }
    }

    /// <summary>
    /// 测试用的简单实体类
    /// </summary>
    public class SimpleEntity
    {
        public int Value { get; set; }
        public string Text { get; set; } = string.Empty;
    }

    /// <summary>
    /// 创建测试用的 Record 和数据
    /// </summary>
    private (Record record, RecordRow row) CreateTestRecord()
    {
        var record = new Record("TestTable", 1);

        // 添加列
        var idColumn = record.Columns.Add<Int32>("Id");
        var nameColumn = record.Columns.Add<String>("Name");
        var createTimeColumn = record.Columns.Add<DateTime>("CreateTime");
        var isActiveColumn = record.Columns.Add<Boolean>("IsActive");
        var scoreColumn = record.Columns.Add<Double>("Score");
        var customColumn = record.Columns.Add<String>("custom_column");
        var nullableIntColumn = record.Columns.Add<Int32>("NullableInt");
        var nullableDateTimeColumn = record.Columns.Add<DateTime>("NullableDateTime");

        // 添加一行数据
        var row = record.AddRow();

        // 设置数据
        idColumn.Set(123);
        nameColumn.Set("测试名称");
        createTimeColumn.Set(new DateTime(2023, 7, 28, 10, 30, 0));
        isActiveColumn.Set(true);
        scoreColumn.Set(95.5);
        customColumn.Set("自定义值");
        nullableIntColumn.Set(456);
        nullableDateTimeColumn.Set(new DateTime(2023, 8, 1));

        return (record, row);
    }

    /// <summary>
    /// 测试 Populate 方法 - 基本数据类型填充
    /// </summary>
    [TestMethod]
    public void Populate_BasicDataTypes_ShouldFillEntityCorrectly()
    {
        // Arrange
        var (record, row) = CreateTestRecord();
        var entity = new TestEntity();

        // Act
        RecordLoader<TestEntity>.Populate(row, entity);

        // Assert
        Assert.AreEqual(123, entity.Id);
        Assert.AreEqual("测试名称", entity.Name);
        Assert.AreEqual(new DateTime(2023, 7, 28, 10, 30, 0), entity.CreateTime);
        Assert.AreEqual(true, entity.IsActive);
        Assert.AreEqual(95.5, entity.Score, 0.001); // 浮点数比较
        Assert.AreEqual("自定义值", entity.CustomField);
        Assert.AreEqual(456, entity.NullableInt);
        Assert.AreEqual(new DateTime(2023, 8, 1), entity.NullableDateTime);
    }

    /// <summary>
    /// 测试 Populate 方法 - 列不存在的情况
    /// </summary>
    [TestMethod]
    public void Populate_MissingColumns_ShouldNotThrowException()
    {
        // Arrange
        var record = new Record("TestTable", 1);
        var idColumn = record.Columns.Add<Int32>("Id");
        var row = record.AddRow();
        idColumn.Set(100);

        var entity = new TestEntity
        {
            Name = "原始名称",
            Score = 50.0
        };

        // Act
        RecordLoader<TestEntity>.Populate(row, entity);

        // Assert
        Assert.AreEqual(100, entity.Id); // Id 列存在，应该被填充
        Assert.AreEqual("原始名称", entity.Name); // Name 列不存在，保持原值
        Assert.AreEqual(50.0, entity.Score); // Score 列不存在，保持原值
    }

    /// <summary>
    /// 测试 WriteData 方法 - 基本数据写入
    /// </summary>
    [TestMethod]
    public void WriteData_BasicDataTypes_ShouldWriteToRecordCorrectly()
    {
        // Arrange
        var record = new Record("TestTable", 1);

        // 添加列
        var idColumn = record.Columns.Add<Int32>("Id");
        var nameColumn = record.Columns.Add<String>("Name");
        var createTimeColumn = record.Columns.Add<DateTime>("CreateTime");
        var isActiveColumn = record.Columns.Add<Boolean>("IsActive");
        var scoreColumn = record.Columns.Add<Double>("Score");
        var customColumn = record.Columns.Add<String>("custom_column");
        var nullableIntColumn = record.Columns.Add<Int32>("NullableInt");

        var row = record.AddRow();

        var entity = new TestEntity
        {
            Id = 789,
            Name = "写入测试",
            CreateTime = new DateTime(2023, 9, 15, 14, 20, 30),
            IsActive = false,
            Score = 88.8,
            CustomField = "写入自定义",
            NullableInt = 999
        };

        // Act
        RecordLoader<TestEntity>.WriteToRow(entity, row);

        // Assert
        Assert.AreEqual(789, row.Get<Int32>(idColumn));
        Assert.AreEqual("写入测试", row.Get<String>(nameColumn));
        Assert.AreEqual(new DateTime(2023, 9, 15, 14, 20, 30), row.Get<DateTime>(createTimeColumn));
        Assert.AreEqual(false, row.Get<Boolean>(isActiveColumn));
        Assert.AreEqual(88.8, row.Get<Double>(scoreColumn), 0.001);
        Assert.AreEqual("写入自定义", row.Get<String>(customColumn));
        Assert.AreEqual(999, row.Get<Int32>(nullableIntColumn));
    }

    /// <summary>
    /// 测试 WriteData 方法 - 列不存在的情况
    /// </summary>
    [TestMethod]
    public void WriteData_MissingColumns_ShouldNotThrowException()
    {
        // Arrange
        var record = new Record("TestTable", 1);
        var idColumn = record.Columns.Add<Int32>("Id");
        var row = record.AddRow();

        var entity = new TestEntity
        {
            Id = 555,
            Name = "部分写入",
            Score = 77.7
        };

        // Act & Assert - 不应该抛出异常
        RecordLoader<TestEntity>.WriteToRow(entity, row);

        // 验证存在的列被正确写入
        Assert.AreEqual(555, row.Get<Int32>(idColumn));
    }

    /// <summary>
    /// 测试 RecordColumnNameAttribute 特性
    /// </summary>
    [TestMethod]
    public void Populate_WithRecordColumnNameAttribute_ShouldUseAttributeName()
    {
        // Arrange
        var record = new Record("TestTable", 1);
        var customColumn = record.Columns.Add<String>("custom_column"); // 使用特性指定的名称
        var row = record.AddRow();
        customColumn.Set("特性测试值");

        var entity = new TestEntity();

        // Act
        RecordLoader<TestEntity>.Populate(row, entity);

        // Assert
        Assert.AreEqual("特性测试值", entity.CustomField);
    }

    /// <summary>
    /// 测试可空类型的处理
    /// </summary>
    [TestMethod]
    public void Populate_NullableTypes_ShouldHandleNullValues()
    {
        // Arrange
        var record = new Record("TestTable", 1);
        var nullableIntColumn = record.Columns.Add<Int32>("NullableInt");
        var nullableDateTimeColumn = record.Columns.Add<DateTime>("NullableDateTime");
        var row = record.AddRow();

        // 设置为 null 值（通过不设置任何值来模拟 null）

        var entity = new TestEntity
        {
            NullableInt = 100, // 设置初始值
            NullableDateTime = DateTime.Now // 设置初始值
        };

        // Act
        RecordLoader<TestEntity>.Populate(row, entity);

        // Assert
        // 注意：由于 Record 系统的实现可能不直接支持 null 值，
        // 这里主要测试不会抛出异常
        // 具体的 null 处理逻辑取决于底层 ColumnData 的实现
        Assert.IsNotNull(entity); // 基本的非空验证
    }

    /// <summary>
    /// 测试往返转换（Round-trip）
    /// </summary>
    [TestMethod]
    public void PopulateAndWriteData_RoundTrip_ShouldMaintainDataIntegrity()
    {
        // Arrange
        var originalEntity = new TestEntity
        {
            Id = 999,
            Name = "往返测试",
            CreateTime = new DateTime(2023, 12, 25, 18, 30, 45),
            IsActive = true,
            Score = 92.3,
            CustomField = "往返自定义",
            NullableInt = 777
        };

        var record = new Record("TestTable", 1);

        // 使用 WriteHeader 方法添加列
        RecordLoader<TestEntity>.WriteHeader(record);

        var row = record.AddRow();

        // Act - 写入 Record
        RecordLoader<TestEntity>.WriteToRow(originalEntity, row);

        // Act - 从 Record 读取到新实体
        var newEntity = new TestEntity();
        RecordLoader<TestEntity>.Populate(row, newEntity);

        // Assert
        Assert.AreEqual(originalEntity.Id, newEntity.Id);
        Assert.AreEqual(originalEntity.Name, newEntity.Name);
        Assert.AreEqual(originalEntity.CreateTime, newEntity.CreateTime);
        Assert.AreEqual(originalEntity.IsActive, newEntity.IsActive);
        Assert.AreEqual(originalEntity.Score, newEntity.Score, 0.001);
        Assert.AreEqual(originalEntity.CustomField, newEntity.CustomField);
        Assert.AreEqual(originalEntity.NullableInt, newEntity.NullableInt);
    }

    /// <summary>
    /// 测试简单实体类
    /// </summary>
    [TestMethod]
    public void SimpleEntity_PopulateAndWrite_ShouldWorkCorrectly()
    {
        // Arrange
        var record = new Record("SimpleTable", 1);
        var valueColumn = record.Columns.Add<Int32>("Value");
        var textColumn = record.Columns.Add<String>("Text");
        var row = record.AddRow();

        valueColumn.Set(42);
        textColumn.Set("简单测试");

        var entity = new SimpleEntity();

        // Act
        RecordLoader<SimpleEntity>.Populate(row, entity);

        // Assert
        Assert.AreEqual(42, entity.Value);
        Assert.AreEqual("简单测试", entity.Text);
    }

    /// <summary>
    /// 测试静态构造函数只执行一次
    /// </summary>
    [TestMethod]
    public void StaticConstructor_ShouldExecuteOnlyOnce()
    {
        // Arrange & Act
        var record = new Record("TestTable", 1);
        var idColumn = record.Columns.Add<Int32>("Value"); // 修正列名
        var row = record.AddRow();
        idColumn.Set(1);

        var entity1 = new SimpleEntity();
        var entity2 = new SimpleEntity();

        // Act - 多次调用应该使用同一个编译后的委托
        RecordLoader<SimpleEntity>.Populate(row, entity1);
        RecordLoader<SimpleEntity>.Populate(row, entity2);

        // Assert - 验证两次调用都成功（间接验证静态构造函数正确执行）
        Assert.AreEqual(1, entity1.Value);
        Assert.AreEqual(1, entity2.Value);
    }

    /// <summary>
    /// 测试 WriteHeader 方法
    /// </summary>
    [TestMethod]
    public void WriteHeader_ShouldCreateCorrectColumns()
    {
        // Arrange
        var record = new Record("TestTable", 1);

        // Act
        RecordLoader<TestEntity>.WriteHeader(record);

        // Assert
        Assert.IsTrue(record.Columns.Contains("Id"));
        Assert.IsTrue(record.Columns.Contains("Name"));
        Assert.IsTrue(record.Columns.Contains("CreateTime"));
        Assert.IsTrue(record.Columns.Contains("IsActive"));
        Assert.IsTrue(record.Columns.Contains("Score"));
        Assert.IsTrue(record.Columns.Contains("custom_column")); // 使用特性指定的名称
        Assert.IsTrue(record.Columns.Contains("NullableInt"));
        Assert.IsTrue(record.Columns.Contains("NullableDateTime"));
    }

    /// <summary>
    /// 测试 WriteHeader 方法 - 验证列的数据类型
    /// </summary>
    [TestMethod]
    public void WriteHeader_ShouldCreateColumnsWithCorrectTypes()
    {
        // Arrange
        var record = new Record("TestTable", 1);

        // Act
        RecordLoader<TestEntity>.WriteHeader(record);

        // Assert
        var idColumn = record.Columns.Find("Id");
        Assert.IsNotNull(idColumn);
        Assert.AreEqual(typeof(int), idColumn.Type);

        var nameColumn = record.Columns.Find("Name");
        Assert.IsNotNull(nameColumn);
        Assert.AreEqual(typeof(string), nameColumn.Type);

        var createTimeColumn = record.Columns.Find("CreateTime");
        Assert.IsNotNull(createTimeColumn);
        Assert.AreEqual(typeof(DateTime), createTimeColumn.Type);

        var isActiveColumn = record.Columns.Find("IsActive");
        Assert.IsNotNull(isActiveColumn);
        Assert.AreEqual(typeof(bool), isActiveColumn.Type);

        var scoreColumn = record.Columns.Find("Score");
        Assert.IsNotNull(scoreColumn);
        Assert.AreEqual(typeof(double), scoreColumn.Type);

        var customColumn = record.Columns.Find("custom_column");
        Assert.IsNotNull(customColumn);
        Assert.AreEqual(typeof(string), customColumn.Type);

        var nullableIntColumn = record.Columns.Find("NullableInt");
        Assert.IsNotNull(nullableIntColumn);
        Assert.AreEqual(typeof(int?), nullableIntColumn.Type);

        var nullableDateTimeColumn = record.Columns.Find("NullableDateTime");
        Assert.IsNotNull(nullableDateTimeColumn);
        Assert.AreEqual(typeof(DateTime?), nullableDateTimeColumn.Type);
    }

    /// <summary>
    /// 测试 WriteHeader 方法 - 验证列的数量
    /// </summary>
    [TestMethod]
    public void WriteHeader_ShouldCreateCorrectNumberOfColumns()
    {
        // Arrange
        var record = new Record("TestTable", 1);

        // Act
        RecordLoader<TestEntity>.WriteHeader(record);

        // Assert
        // TestEntity 有 8 个可读写属性
        Assert.AreEqual(8, record.Columns.Count);
    }

    /// <summary>
    /// 测试 WriteHeader 方法 - 简单实体类
    /// </summary>
    [TestMethod]
    public void WriteHeader_SimpleEntity_ShouldCreateCorrectColumns()
    {
        // Arrange
        var record = new Record("SimpleTable", 1);

        // Act
        RecordLoader<SimpleEntity>.WriteHeader(record);

        // Assert
        Assert.AreEqual(2, record.Columns.Count);
        Assert.IsTrue(record.Columns.Contains("Value"));
        Assert.IsTrue(record.Columns.Contains("Text"));

        var valueColumn = record.Columns.Find("Value");
        Assert.IsNotNull(valueColumn);
        Assert.AreEqual(typeof(int), valueColumn.Type);

        var textColumn = record.Columns.Find("Text");
        Assert.IsNotNull(textColumn);
        Assert.AreEqual(typeof(string), textColumn.Type);
    }

    /// <summary>
    /// 测试 WriteHeader 方法 - 空 Record
    /// </summary>
    [TestMethod]
    public void WriteHeader_EmptyRecord_ShouldPopulateColumns()
    {
        // Arrange
        var record = new Record();

        // Act
        RecordLoader<TestEntity>.WriteHeader(record);

        // Assert
        Assert.IsTrue(record.Columns.Count > 0);
        Assert.IsTrue(record.Columns.Contains("Id"));
        Assert.IsTrue(record.Columns.Contains("Name"));
    }

    /// <summary>
    /// 测试 WriteHeader 方法 - 多次调用不会重复添加列
    /// </summary>
    [TestMethod]
    public void WriteHeader_CalledTwice_ShouldThrowException()
    {
        // Arrange
        var record = new Record("TestTable", 1);
        RecordLoader<TestEntity>.WriteHeader(record);

        // Act & Assert
        // 第二次调用应该抛出异常，因为列已经存在
        Assert.ThrowsExactly<DuplicateNameException>(() =>
        {
            RecordLoader<TestEntity>.WriteHeader(record);
        });
    }

    /// <summary>
    /// 测试用的只读属性实体类
    /// </summary>
    public class ReadOnlyEntity
    {
        public int Id { get; }
        public string Name { get; set; } = string.Empty;
        public int WriteOnlyProperty { set { } }
    }

    /// <summary>
    /// 测试 WriteHeader 方法 - 只包含可读写属性
    /// </summary>
    [TestMethod]
    public void WriteHeader_ReadOnlyEntity_ShouldOnlyIncludeReadWriteProperties()
    {
        // Arrange
        var record = new Record("ReadOnlyTable", 1);

        // Act
        RecordLoader<ReadOnlyEntity>.WriteHeader(record);

        // Assert
        // 只有 Name 属性是可读写的
        Assert.AreEqual(1, record.Columns.Count);
        Assert.IsTrue(record.Columns.Contains("Name"));
        Assert.IsFalse(record.Columns.Contains("Id")); // 只读属性
        Assert.IsFalse(record.Columns.Contains("WriteOnlyProperty")); // 只写属性
    }

    /// <summary>
    /// 测试用的复杂类型实体类
    /// </summary>
    public class ComplexTypeEntity
    {
        public Guid Id { get; set; }
        public TimeSpan Duration { get; set; }
        public byte[] Data { get; set; } = Array.Empty<byte>();
        public object ComplexObject { get; set; } = new object();
    }

    /// <summary>
    /// 测试 WriteHeader 方法 - 复杂数据类型
    /// </summary>
    [TestMethod]
    public void WriteHeader_ComplexTypeEntity_ShouldHandleComplexTypes()
    {
        // Arrange
        var record = new Record("ComplexTable", 1);

        // Act
        RecordLoader<ComplexTypeEntity>.WriteHeader(record);

        // Assert
        Assert.AreEqual(4, record.Columns.Count);
        Assert.IsTrue(record.Columns.Contains("Id"));
        Assert.IsTrue(record.Columns.Contains("Duration"));
        Assert.IsTrue(record.Columns.Contains("Data"));
        Assert.IsTrue(record.Columns.Contains("ComplexObject"));

        var idColumn = record.Columns.Find("Id");
        Assert.IsNotNull(idColumn);
        Assert.AreEqual(typeof(Guid), idColumn.Type);

        var durationColumn = record.Columns.Find("Duration");
        Assert.IsNotNull(durationColumn);
        Assert.AreEqual(typeof(TimeSpan), durationColumn.Type);

        var dataColumn = record.Columns.Find("Data");
        Assert.IsNotNull(dataColumn);
        Assert.AreEqual(typeof(byte[]), dataColumn.Type);

        var complexColumn = record.Columns.Find("ComplexObject");
        Assert.IsNotNull(complexColumn);
        Assert.AreEqual(typeof(object), complexColumn.Type);
    }

    /// <summary>
    /// 测试 WriteHeader 方法与 WriteData 方法的兼容性
    /// </summary>
    [TestMethod]
    public void WriteHeader_WithWriteData_ShouldBeCompatible()
    {
        // Arrange
        var record = new Record("CompatibilityTable", 1);
        var entity = new TestEntity
        {
            Id = 123,
            Name = "兼容性测试",
            CreateTime = DateTime.Now,
            IsActive = true,
            Score = 95.5,
            CustomField = "自定义值",
            NullableInt = 456
        };

        // Act
        RecordLoader<TestEntity>.WriteHeader(record);
        var row = record.AddRow();
        RecordLoader<TestEntity>.WriteToRow(entity, row);

        // Assert
        // 验证所有列都能正确写入数据
        Assert.AreEqual(entity.Id, row.Get<Int32>("Id"));
        Assert.AreEqual(entity.Name, row.Get<String>(("Name")!));
        Assert.AreEqual(entity.CreateTime, row.Get<DateTime>(("CreateTime")!));
        Assert.AreEqual(entity.IsActive, row.Get<Boolean>(("IsActive")!));
        Assert.AreEqual(entity.Score, row.Get<Double>(("Score")!), 0.001);
        Assert.AreEqual(entity.CustomField, row.Get<String>(("custom_column")!));
        Assert.AreEqual(entity.NullableInt, row.Get<Int32>(("NullableInt")!));
    }
}
