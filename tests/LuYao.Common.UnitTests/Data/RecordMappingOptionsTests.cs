using LuYao.Data.Attributes;
using LuYao.Data.Mapping;
using System;

namespace LuYao.Data;

[TestClass]
public class RecordMappingOptionsTests
{
    // ─── 测试用 DTO ───────────────────────────────────────────────────────────────

    private class PersonDto
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public int Age { get; set; }
    }

    private class DtoWithColumnName
    {
        [RecordColumnNameAttribute("full_name")]
        public string? FullName { get; set; }

        public int Age { get; set; }
    }

    private class DtoWithUnsupportedType
    {
        public int Id { get; set; }
        // object 不是支持的列类型
        public object? Extra { get; set; }
    }

    // ─── RecordColumnNameAttribute ────────────────────────────────────────────────

    [TestMethod]
    public void ColumnNameAttribute_DtoToTable_UsesAttributeName()
    {
        var options = new RecordMappingOptions();
        var table = RecordTable.From(new DtoWithColumnName { FullName = "Alice", Age = 30 }, options);

        // 列名应为 full_name 而非 FullName
        Assert.IsNotNull(table.Columns.Find("full_name"));
        Assert.IsNull(table.Columns.Find("FullName"));
        Assert.AreEqual("Alice", table.Columns.Find("full_name")!.To<string>(0));
    }

    [TestMethod]
    public void ColumnNameAttribute_TableToDto_UsesAttributeName()
    {
        var table = new RecordTable();
        table.Columns.Add<string>("full_name");
        table.Columns.Add<int>("Age");
        var row = table.AddRow();
        table.Columns.Find("full_name")!.Set(row, "Bob");
        table.Columns.Find("Age")!.Set(row, 25);

        var options = new RecordMappingOptions();
        var dto = table.To<DtoWithColumnName>(options);

        Assert.AreEqual("Bob", dto.FullName);
        Assert.AreEqual(25, dto.Age);
    }

    // ─── NamingPolicy: CamelCase ──────────────────────────────────────────────────

    [TestMethod]
    public void NamingPolicy_CamelCase_DtoToTable()
    {
        var options = new RecordMappingOptions { NamingPolicy = RecordNamingPolicy.CamelCase };
        var table = RecordTable.From(new PersonDto { Id = 1, FullName = "Alice", Age = 30 }, options);

        Assert.IsNotNull(table.Columns.Find("id"));
        Assert.IsNotNull(table.Columns.Find("fullName"));
        Assert.IsNotNull(table.Columns.Find("age"));
        Assert.AreEqual("Alice", table.Columns.Find("fullName")!.To<string>(0));
    }

    [TestMethod]
    public void NamingPolicy_CamelCase_TableToDto()
    {
        var table = new RecordTable();
        table.Columns.Add<int>("id");
        table.Columns.Add<string>("fullName");
        table.Columns.Add<int>("age");
        var row = table.AddRow();
        table.Columns.Find("id")!.Set(row, 42);
        table.Columns.Find("fullName")!.Set(row, "Carol");
        table.Columns.Find("age")!.Set(row, 20);

        var options = new RecordMappingOptions { NamingPolicy = RecordNamingPolicy.CamelCase };
        var dto = table.To<PersonDto>(options);

        Assert.AreEqual(42, dto.Id);
        Assert.AreEqual("Carol", dto.FullName);
        Assert.AreEqual(20, dto.Age);
    }

    // ─── NamingPolicy: SnakeCaseLower ─────────────────────────────────────────────

    [TestMethod]
    public void NamingPolicy_SnakeCaseLower_DtoToTable()
    {
        var options = new RecordMappingOptions { NamingPolicy = RecordNamingPolicy.SnakeCaseLower };
        var table = RecordTable.From(new PersonDto { Id = 1, FullName = "Dave", Age = 40 }, options);

        Assert.IsNotNull(table.Columns.Find("full_name"));
    }

    // ─── NamingPolicy: SnakeCaseUpper ─────────────────────────────────────────────

    [TestMethod]
    public void NamingPolicy_SnakeCaseUpper_DtoToTable()
    {
        var options = new RecordMappingOptions { NamingPolicy = RecordNamingPolicy.SnakeCaseUpper };
        var table = RecordTable.From(new PersonDto { Id = 1, FullName = "Eve", Age = 50 }, options);

        Assert.IsNotNull(table.Columns.Find("FULL_NAME"));
    }

    // ─── NamingPolicy: KebabCaseLower ────────────────────────────────────────────

    [TestMethod]
    public void NamingPolicy_KebabCaseLower_DtoToTable()
    {
        var options = new RecordMappingOptions { NamingPolicy = RecordNamingPolicy.KebabCaseLower };
        var table = RecordTable.From(new PersonDto { Id = 1, FullName = "Frank", Age = 55 }, options);

        Assert.IsNotNull(table.Columns.Find("full-name"));
    }

    // ─── NamingPolicy: KebabCaseUpper ────────────────────────────────────────────

    [TestMethod]
    public void NamingPolicy_KebabCaseUpper_DtoToTable()
    {
        var options = new RecordMappingOptions { NamingPolicy = RecordNamingPolicy.KebabCaseUpper };
        var table = RecordTable.From(new PersonDto { Id = 1, FullName = "Grace", Age = 60 }, options);

        Assert.IsNotNull(table.Columns.Find("FULL-NAME"));
    }

    // ─── ColumnNameAttribute 优先级高于 NamingPolicy ──────────────────────────────

    [TestMethod]
    public void ColumnNameAttribute_HasHigherPriorityThan_NamingPolicy()
    {
        // NamingPolicy 会把 FullName → fullName，但 RecordColumnNameAttribute 指定了 full_name
        var options = new RecordMappingOptions { NamingPolicy = RecordNamingPolicy.CamelCase };
        var table = RecordTable.From(new DtoWithColumnName { FullName = "Heidi", Age = 35 }, options);

        // 应优先使用 Attribute 的 full_name，而非 NamingPolicy 的 fullName
        Assert.IsNotNull(table.Columns.Find("full_name"));
        Assert.IsNull(table.Columns.Find("fullName"));
    }

    // ─── UnsupportedTypeHandling.Skip ────────────────────────────────────────────

    [TestMethod]
    public void UnsupportedTypeHandling_Skip_DoesNotThrow()
    {
        var options = new RecordMappingOptions
        {
            UnsupportedTypeHandling = UnsupportedTypeHandling.Skip
        };
        var table = new RecordTable();
        table.Columns.Add<int>("Id");
        var dto = new DtoWithUnsupportedType { Id = 7, Extra = new object() };
        var row = table.AddRowFrom(dto, options);
        // 不抛出异常，只写入了支持的列
        Assert.AreEqual(7, table.Columns.Find("Id")!.To<int>(0));
    }

    // ─── UnsupportedTypeHandling.Throw ───────────────────────────────────────────

    [TestMethod]
    public void UnsupportedTypeHandling_Throw_ThrowsNotSupportedException()
    {
        var options = new RecordMappingOptions
        {
            UnsupportedTypeHandling = UnsupportedTypeHandling.Throw
        };
        var table = new RecordTable();
        Assert.Throws<NotSupportedException>(() =>
            table.Columns.AddFrom<DtoWithUnsupportedType>(options));
    }

    // ─── ConversionFailureHandling.Skip ──────────────────────────────────────────

    [TestMethod]
    public void ConversionFailureHandling_Skip_KeepsDefault()
    {
        var table = new RecordTable();
        table.Columns.Add<string>("Id"); // 列为 string，但属性为 int
        table.Columns.Add<string>("FullName");
        table.Columns.Add<int>("Age");
        var row = table.AddRow();
        table.Columns.Find("Id")!.Set(row, "not_a_number");
        table.Columns.Find("FullName")!.Set(row, "Ivan");
        table.Columns.Find("Age")!.Set(row, 28);

        var options = new RecordMappingOptions
        {
            NamingPolicy = null,
            ConversionFailureHandling = ConversionFailureHandling.Skip
        };
        var dto = table.To<PersonDto>(options);

        // Id 转换失败，保持默认值 0；其他属性正常
        Assert.AreEqual(0, dto.Id);
        Assert.AreEqual("Ivan", dto.FullName);
        Assert.AreEqual(28, dto.Age);
    }

    // ─── ConversionFailureHandling.Throw ─────────────────────────────────────────

    [TestMethod]
    public void ConversionFailureHandling_Throw_ThrowsOnFailure()
    {
        var table = new RecordTable();
        table.Columns.Add<string>("Id");
        table.Columns.Add<string>("FullName");
        table.Columns.Add<int>("Age");
        var row = table.AddRow();
        table.Columns.Find("Id")!.Set(row, "not_a_number");
        table.Columns.Find("FullName")!.Set(row, "Jack");
        table.Columns.Find("Age")!.Set(row, 33);

        var options = new RecordMappingOptions
        {
            ConversionFailureHandling = ConversionFailureHandling.Throw
        };
        Assert.Throws<Exception>(() => table.To<PersonDto>(options));
    }

    // ─── Options 冻结机制 ─────────────────────────────────────────────────────────

    [TestMethod]
    public void Options_CanBeModified_BeforeMapping()
    {
        var options = new RecordMappingOptions();
        options.NamingPolicy = RecordNamingPolicy.CamelCase;
        options.UnsupportedTypeHandling = UnsupportedTypeHandling.Throw;
        options.ConversionFailureHandling = ConversionFailureHandling.Throw;
        // 不抛出异常即为通过
        Assert.IsFalse(options.IsReadOnly);
    }

    [TestMethod]
    public void Options_CannotBeModified_AfterMapping()
    {
        var options = new RecordMappingOptions();
        var table = RecordTable.From(new PersonDto { Id = 1, FullName = "Kate", Age = 22 }, options);
        Assert.IsTrue(options.IsReadOnly);
        Assert.Throws<InvalidOperationException>(() =>
            options.NamingPolicy = RecordNamingPolicy.SnakeCaseLower);
    }

    [TestMethod]
    public void Default_CannotBeModified_AfterMapping()
    {
        // 触发一次映射使 Default 被冻结
        var _ = RecordTable.From(new PersonDto { Id = 1 }, RecordMappingOptions.Default);
        Assert.IsTrue(RecordMappingOptions.Default.IsReadOnly);
        Assert.Throws<InvalidOperationException>(() =>
            RecordMappingOptions.Default.NamingPolicy = RecordNamingPolicy.CamelCase);
    }

    // ─── RecordColumnStorageAttribute ──────────────────────────────────────────────

    private class ComplexTag
    {
        public string Value { get; set; } = "default";
        public override string ToString() => Value;
    }

    [TestMethod]
    public void StorageAttribute_Skip_SkipsProperty()
    {
        // 属性上标注 [RecordColumnStorage(Skip)] 时，该列不应被创建
        var dto = new DtoWithSkipTag { Name = "Bob", Secret = new ComplexTag { Value = "hidden" } };
        var table = RecordTable.From(dto);
        Assert.IsNull(table.Columns.Find("Secret"));
        Assert.IsNotNull(table.Columns.Find("Name"));
    }

    private class DtoWithSkipTag
    {
        public string Name { get; set; } = "";

        [RecordColumnStorage(RecordColumnStorageTarget.Skip)]
        public ComplexTag Secret { get; set; } = new();
    }
}
