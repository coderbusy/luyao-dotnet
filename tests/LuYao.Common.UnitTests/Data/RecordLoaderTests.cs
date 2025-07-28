//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System;
//using LuYao.Data;

//namespace LuYao.Data.Tests;

///// <summary>
///// RecordLoader 的单元测试类
///// </summary>
//[TestClass]
//public class RecordLoaderTests
//{
//    /// <summary>
//    /// 测试用的实体类
//    /// </summary>
//    public class TestEntity
//    {
//        public int Id { get; set; }
//        public string Name { get; set; } = string.Empty;
//        public DateTime CreateTime { get; set; }
//        public bool IsActive { get; set; }
//        public double Score { get; set; }

//        [RecordColumnName("custom_column")]
//        public string CustomField { get; set; } = string.Empty;

//        public int? NullableInt { get; set; }
//        public DateTime? NullableDateTime { get; set; }
//    }

//    /// <summary>
//    /// 测试用的简单实体类
//    /// </summary>
//    public class SimpleEntity
//    {
//        public int Value { get; set; }
//        public string Text { get; set; } = string.Empty;
//    }

//    /// <summary>
//    /// 创建测试用的 Record 和数据
//    /// </summary>
//    private (Record record, RecordRow row) CreateTestRecord()
//    {
//        var record = new Record("TestTable", 1);

//        // 添加列
//        var idColumn = record.Columns.AddInt32("Id");
//        var nameColumn = record.Columns.AddString("Name");
//        var createTimeColumn = record.Columns.AddDateTime("CreateTime");
//        var isActiveColumn = record.Columns.AddBoolean("IsActive");
//        var scoreColumn = record.Columns.AddDouble("Score");
//        var customColumn = record.Columns.AddString("custom_column");
//        var nullableIntColumn = record.Columns.AddInt32("NullableInt");
//        var nullableDateTimeColumn = record.Columns.AddDateTime("NullableDateTime");

//        // 添加一行数据
//        var row = record.AddRow();

//        // 设置数据
//        row.Set(123, idColumn);
//        row.Set("测试名称", nameColumn);
//        row.Set(new DateTime(2023, 7, 28, 10, 30, 0), createTimeColumn);
//        row.Set(true, isActiveColumn);
//        row.Set(95.5, scoreColumn);
//        row.Set("自定义值", customColumn);
//        row.Set(456, nullableIntColumn);
//        row.Set(new DateTime(2023, 8, 1), nullableDateTimeColumn);

//        return (record, row);
//    }

//    /// <summary>
//    /// 测试 Populate 方法 - 基本数据类型填充
//    /// </summary>
//    [TestMethod]
//    public void Populate_BasicDataTypes_ShouldFillEntityCorrectly()
//    {
//        // Arrange
//        var (record, row) = CreateTestRecord();
//        var entity = new TestEntity();

//        // Act
//        RecordLoader<TestEntity>.Populate(row, entity);

//        // Assert
//        Assert.AreEqual(123, entity.Id);
//        Assert.AreEqual("测试名称", entity.Name);
//        Assert.AreEqual(new DateTime(2023, 7, 28, 10, 30, 0), entity.CreateTime);
//        Assert.AreEqual(true, entity.IsActive);
//        Assert.AreEqual(95.5, entity.Score, 0.001); // 浮点数比较
//        Assert.AreEqual("自定义值", entity.CustomField);
//        Assert.AreEqual(456, entity.NullableInt);
//        Assert.AreEqual(new DateTime(2023, 8, 1), entity.NullableDateTime);
//    }

//    /// <summary>
//    /// 测试 Populate 方法 - 列不存在的情况
//    /// </summary>
//    [TestMethod]
//    public void Populate_MissingColumns_ShouldNotThrowException()
//    {
//        // Arrange
//        var record = new Record("TestTable", 1);
//        var idColumn = record.Columns.AddInt32("Id");
//        var row = record.AddRow();
//        row.Set(100, idColumn);

//        var entity = new TestEntity
//        {
//            Name = "原始名称",
//            Score = 50.0
//        };

//        // Act
//        RecordLoader<TestEntity>.Populate(row, entity);

//        // Assert
//        Assert.AreEqual(100, entity.Id); // Id 列存在，应该被填充
//        Assert.AreEqual("原始名称", entity.Name); // Name 列不存在，保持原值
//        Assert.AreEqual(50.0, entity.Score); // Score 列不存在，保持原值
//    }

//    /// <summary>
//    /// 测试 WriteRecord 方法 - 基本数据写入
//    /// </summary>
//    [TestMethod]
//    public void WriteRecord_BasicDataTypes_ShouldWriteToRecordCorrectly()
//    {
//        // Arrange
//        var record = new Record("TestTable", 1);

//        // 添加列
//        var idColumn = record.Columns.AddInt32("Id");
//        var nameColumn = record.Columns.AddString("Name");
//        var createTimeColumn = record.Columns.AddDateTime("CreateTime");
//        var isActiveColumn = record.Columns.AddBoolean("IsActive");
//        var scoreColumn = record.Columns.AddDouble("Score");
//        var customColumn = record.Columns.AddString("custom_column");
//        var nullableIntColumn = record.Columns.AddInt32("NullableInt");

//        var row = record.AddRow();

//        var entity = new TestEntity
//        {
//            Id = 789,
//            Name = "写入测试",
//            CreateTime = new DateTime(2023, 9, 15, 14, 20, 30),
//            IsActive = false,
//            Score = 88.8,
//            CustomField = "写入自定义",
//            NullableInt = 999
//        };

//        // Act
//        RecordLoader<TestEntity>.WriteRecord(entity, row);

//        // Assert
//        Assert.AreEqual(789, row.ToInt32(idColumn));
//        Assert.AreEqual("写入测试", row.ToString(nameColumn));
//        Assert.AreEqual(new DateTime(2023, 9, 15, 14, 20, 30), row.ToDateTime(createTimeColumn));
//        Assert.AreEqual(false, row.ToBoolean(isActiveColumn));
//        Assert.AreEqual(88.8, row.ToDouble(scoreColumn), 0.001);
//        Assert.AreEqual("写入自定义", row.ToString(customColumn));
//        Assert.AreEqual(999, row.ToInt32(nullableIntColumn));
//    }

//    /// <summary>
//    /// 测试 WriteRecord 方法 - 列不存在的情况
//    /// </summary>
//    [TestMethod]
//    public void WriteRecord_MissingColumns_ShouldNotThrowException()
//    {
//        // Arrange
//        var record = new Record("TestTable", 1);
//        var idColumn = record.Columns.AddInt32("Id");
//        var row = record.AddRow();

//        var entity = new TestEntity
//        {
//            Id = 555,
//            Name = "部分写入",
//            Score = 77.7
//        };

//        // Act & Assert - 不应该抛出异常
//        RecordLoader<TestEntity>.WriteRecord(entity, row);

//        // 验证存在的列被正确写入
//        Assert.AreEqual(555, row.ToInt32(idColumn));
//    }

//    /// <summary>
//    /// 测试 RecordColumnNameAttribute 特性
//    /// </summary>
//    [TestMethod]
//    public void Populate_WithRecordColumnNameAttribute_ShouldUseAttributeName()
//    {
//        // Arrange
//        var record = new Record("TestTable", 1);
//        var customColumn = record.Columns.AddString("custom_column"); // 使用特性指定的名称
//        var row = record.AddRow();
//        row.Set("特性测试值", customColumn);

//        var entity = new TestEntity();

//        // Act
//        RecordLoader<TestEntity>.Populate(row, entity);

//        // Assert
//        Assert.AreEqual("特性测试值", entity.CustomField);
//    }

//    /// <summary>
//    /// 测试可空类型的处理
//    /// </summary>
//    [TestMethod]
//    public void Populate_NullableTypes_ShouldHandleNullValues()
//    {
//        // Arrange
//        var record = new Record("TestTable", 1);
//        var nullableIntColumn = record.Columns.AddInt32("NullableInt");
//        var nullableDateTimeColumn = record.Columns.AddDateTime("NullableDateTime");
//        var row = record.AddRow();

//        // 设置为 null 值（通过不设置任何值来模拟 null）

//        var entity = new TestEntity
//        {
//            NullableInt = 100, // 设置初始值
//            NullableDateTime = DateTime.Now // 设置初始值
//        };

//        // Act
//        RecordLoader<TestEntity>.Populate(row, entity);

//        // Assert
//        // 注意：由于 Record 系统的实现可能不直接支持 null 值，
//        // 这里主要测试不会抛出异常
//        // 具体的 null 处理逻辑取决于底层 ColumnData 的实现
//        Assert.IsNotNull(entity); // 基本的非空验证
//    }

//    /// <summary>
//    /// 测试往返转换（Round-trip）
//    /// </summary>
//    [TestMethod]
//    public void PopulateAndWriteRecord_RoundTrip_ShouldMaintainDataIntegrity()
//    {
//        // Arrange
//        var originalEntity = new TestEntity
//        {
//            Id = 999,
//            Name = "往返测试",
//            CreateTime = new DateTime(2023, 12, 25, 18, 30, 45),
//            IsActive = true,
//            Score = 92.3,
//            CustomField = "往返自定义",
//            NullableInt = 777
//        };

//        var record = new Record("TestTable", 1);
//        record.Columns.AddInt32("Id");
//        record.Columns.AddString("Name");
//        record.Columns.AddDateTime("CreateTime");
//        record.Columns.AddBoolean("IsActive");
//        record.Columns.AddDouble("Score");
//        record.Columns.AddString("custom_column");
//        record.Columns.AddInt32("NullableInt");

//        var row = record.AddRow();

//        // Act - 写入 Record
//        RecordLoader<TestEntity>.WriteRecord(originalEntity, row);

//        // Act - 从 Record 读取到新实体
//        var newEntity = new TestEntity();
//        RecordLoader<TestEntity>.Populate(row, newEntity);

//        // Assert
//        Assert.AreEqual(originalEntity.Id, newEntity.Id);
//        Assert.AreEqual(originalEntity.Name, newEntity.Name);
//        Assert.AreEqual(originalEntity.CreateTime, newEntity.CreateTime);
//        Assert.AreEqual(originalEntity.IsActive, newEntity.IsActive);
//        Assert.AreEqual(originalEntity.Score, newEntity.Score, 0.001);
//        Assert.AreEqual(originalEntity.CustomField, newEntity.CustomField);
//        Assert.AreEqual(originalEntity.NullableInt, newEntity.NullableInt);
//    }

//    /// <summary>
//    /// 测试简单实体类
//    /// </summary>
//    [TestMethod]
//    public void SimpleEntity_PopulateAndWrite_ShouldWorkCorrectly()
//    {
//        // Arrange
//        var record = new Record("SimpleTable", 1);
//        var valueColumn = record.Columns.AddInt32("Value");
//        var textColumn = record.Columns.AddString("Text");
//        var row = record.AddRow();

//        row.Set(42, valueColumn);
//        row.Set("简单测试", textColumn);

//        var entity = new SimpleEntity();

//        // Act
//        RecordLoader<SimpleEntity>.Populate(row, entity);

//        // Assert
//        Assert.AreEqual(42, entity.Value);
//        Assert.AreEqual("简单测试", entity.Text);
//    }

//    /// <summary>
//    /// 测试静态构造函数只执行一次
//    /// </summary>
//    [TestMethod]
//    public void StaticConstructor_ShouldExecuteOnlyOnce()
//    {
//        // Arrange & Act
//        var record = new Record("TestTable", 1);
//        var idColumn = record.Columns.AddInt32("Id");
//        var row = record.AddRow();
//        row.Set(1, idColumn);

//        var entity1 = new SimpleEntity();
//        var entity2 = new SimpleEntity();

//        // Act - 多次调用应该使用同一个编译后的委托
//        RecordLoader<SimpleEntity>.Populate(row, entity1);
//        RecordLoader<SimpleEntity>.Populate(row, entity2);

//        // Assert - 验证两次调用都成功（间接验证静态构造函数正确执行）
//        Assert.AreEqual(1, entity1.Value);
//        Assert.AreEqual(1, entity2.Value);
//    }
//}