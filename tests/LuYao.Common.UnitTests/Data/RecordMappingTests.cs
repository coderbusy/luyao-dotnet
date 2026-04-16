using System;
using System.Collections.Generic;
using System.Linq;

namespace LuYao.Data;

[TestClass]
public class ColumnTypeWhitelistTests
{
    [TestMethod]
    public void WhenAddSupportedTypeThenSucceeds()
    {
        var record = new Record("Test", 1);
        record.Columns.Add<int>("Int");
        record.Columns.Add<string>("String");
        record.Columns.Add<DateTime>("DateTime");
        record.Columns.Add<Guid>("Guid");
        record.Columns.Add<decimal>("Decimal");
        record.Columns.Add<bool>("Bool");
        record.Columns.Add<byte[]>("Bytes");
        record.Columns.Add<int?>("NullableInt");
        record.Columns.Add<DateTime?>("NullableDateTime");
        record.Columns.Add<long>("Long");
        record.Columns.Add<double>("Double");
        record.Columns.Add<float>("Float");
        record.Columns.Add<short>("Short");
        record.Columns.Add<byte>("Byte");
        record.Columns.Add<char>("Char");
        record.Columns.Add<DateTimeOffset>("DateTimeOffset");
        record.Columns.Add<TimeSpan>("TimeSpan");
        record.Columns.Add<sbyte>("SByte");
        record.Columns.Add<ushort>("UShort");
        record.Columns.Add<uint>("UInt");
        record.Columns.Add<ulong>("ULong");

        Assert.AreEqual(21, record.Columns.Count);
    }

    [TestMethod]
    public void WhenAddUnsupportedTypeThenThrowsNotSupportedException()
    {
        var record = new Record("Test", 1);
        Assert.Throws<NotSupportedException>(() => record.Columns.Add<object>("Obj"));
    }

    [TestMethod]
    public void WhenAddUnsupportedTypeByTypeThenThrowsNotSupportedException()
    {
        var record = new Record("Test", 1);
        Assert.Throws<NotSupportedException>(() => record.Columns.Add("Obj", typeof(List<int>)));
    }

    [TestMethod]
    public void WhenAddCustomClassThenThrowsNotSupportedException()
    {
        var record = new Record("Test", 1);
        Assert.Throws<NotSupportedException>(() => record.Columns.Add("Custom", typeof(ColumnTypeWhitelistTests)));
    }
}

[TestClass]
public class RecordMappingTests
{
    public class SimpleEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }

    public class DifferentEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Extra { get; set; }
    }

    public enum Color { Red, Green, Blue }

    public class EntityWithEnum
    {
        public int Id { get; set; }
        public Color Color { get; set; }
    }

    public class EntityWithNullable
    {
        public int Id { get; set; }
        public int? Score { get; set; }
    }

    public class EntityWithComplexProp
    {
        public int Id { get; set; }
        public List<int> Items { get; set; } = new List<int>();
    }

    public class RecordRowCtorEntity
    {
        public int Id { get; }
        public string Name { get; }

        public RecordRowCtorEntity(RecordRow row)
        {
            Id = row.Get<int>("Id");
            Name = row.Get<string>("Name") ?? string.Empty;
        }
    }

    public class MarkedCtorEntity
    {
        public int Id { get; }
        public string Name { get; }

        public MarkedCtorEntity() { Id = 0; Name = string.Empty; }

        [RecordConstructor]
        public MarkedCtorEntity(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    #region AddRow<T>

    [TestMethod]
    public void WhenAddRowThenWritesMatchedColumns()
    {
        var record = new Record("Test", 5);
        record.Columns.Add<int>("Id");
        record.Columns.Add<string>("Name");
        record.Columns.Add<decimal>("Amount");

        var entity = new SimpleEntity { Id = 1, Name = "Alice", Amount = 99.5m };
        record.AddRow(entity);

        Assert.AreEqual(1, record.Count);
        Assert.AreEqual(1, record.Columns.Find<int>("Id")!.Get(0));
        Assert.AreEqual("Alice", record.Columns.Find<string>("Name")!.Get(0));
        Assert.AreEqual(99.5m, record.Columns.Find<decimal>("Amount")!.Get(0));
    }

    [TestMethod]
    public void WhenAddRowWithExtraPropertyThenIgnored()
    {
        var record = new Record("Test", 5);
        record.Columns.Add<int>("Id");
        record.Columns.Add<string>("Name");

        var entity = new SimpleEntity { Id = 1, Name = "Alice", Amount = 99.5m };
        record.AddRow(entity);

        Assert.AreEqual(1, record.Count);
        Assert.AreEqual(2, record.Columns.Count);
    }

    [TestMethod]
    public void WhenAddRowWithAutoAddColumnsThenAddsColumns()
    {
        var record = new Record("Test", 5);
        record.Columns.Add<int>("Id");

        var options = new RecordMappingOptions { AutoAddColumns = true };
        var entity = new SimpleEntity { Id = 1, Name = "Alice", Amount = 99.5m };
        record.AddRow(entity, options);

        Assert.AreEqual(3, record.Columns.Count);
        Assert.IsNotNull(record.Columns.Find("Name"));
        Assert.IsNotNull(record.Columns.Find("Amount"));
    }

    [TestMethod]
    public void WhenAddRowsDifferentTypesThenDuckTyping()
    {
        var record = new Record("Test", 5);
        record.Columns.Add<int>("Id");
        record.Columns.Add<string>("Name");

        record.AddRow(new SimpleEntity { Id = 1, Name = "Alice", Amount = 100m });
        record.AddRow(new DifferentEntity { Id = 2, Name = "Bob", Extra = 3.14 });

        Assert.AreEqual(2, record.Count);
        Assert.AreEqual(1, record.Columns.Find<int>("Id")!.Get(0));
        Assert.AreEqual(2, record.Columns.Find<int>("Id")!.Get(1));
    }

    [TestMethod]
    public void WhenAddRowsMultipleThenAllWritten()
    {
        var record = new Record("Test", 5);
        record.Columns.Add<int>("Id");
        record.Columns.Add<string>("Name");

        var items = new[]
        {
            new SimpleEntity { Id = 1, Name = "A" },
            new SimpleEntity { Id = 2, Name = "B" },
            new SimpleEntity { Id = 3, Name = "C" },
        };
        record.AddRows(items);

        Assert.AreEqual(3, record.Count);
    }

    [TestMethod]
    public void WhenAddRowWithComplexPropertyThenSkipped()
    {
        var record = new Record("Test", 5);
        record.Columns.Add<int>("Id");

        var entity = new EntityWithComplexProp { Id = 1, Items = new List<int> { 1, 2, 3 } };
        record.AddRow(entity);

        Assert.AreEqual(1, record.Count);
        Assert.AreEqual(1, record.Columns.Count);
    }

    [TestMethod]
    public void WhenAddRowWithEnumToIntColumnThenConverts()
    {
        var record = new Record("Test", 5);
        record.Columns.Add<int>("Id");
        record.Columns.Add<int>("Color");

        record.AddRow(new EntityWithEnum { Id = 1, Color = Color.Green });

        Assert.AreEqual(1, record.Columns.Find<int>("Color")!.Get(0));
    }

    [TestMethod]
    public void WhenAddRowWithEnumToStringColumnThenConverts()
    {
        var record = new Record("Test", 5);
        record.Columns.Add<int>("Id");
        record.Columns.Add<string>("Color");

        record.AddRow(new EntityWithEnum { Id = 1, Color = Color.Blue });

        Assert.AreEqual("Blue", record.Columns.Find<string>("Color")!.Get(0));
    }

    #endregion

    #region ToList<T>

    [TestMethod]
    public void WhenToListThenMapsAllRows()
    {
        var record = new Record("Test", 5);
        var idCol = record.Columns.Add<int>("Id");
        var nameCol = record.Columns.Add<string>("Name");
        var amountCol = record.Columns.Add<decimal>("Amount");

        var row1 = record.AddRow();
        idCol.Set(1, row1.Row);
        nameCol.Set("Alice", row1.Row);
        amountCol.Set(99.5m, row1.Row);

        var row2 = record.AddRow();
        idCol.Set(2, row2.Row);
        nameCol.Set("Bob", row2.Row);
        amountCol.Set(50.0m, row2.Row);

        var list = record.ToList<SimpleEntity>();

        Assert.AreEqual(2, list.Count);
        Assert.AreEqual(1, list[0].Id);
        Assert.AreEqual("Alice", list[0].Name);
        Assert.AreEqual(99.5m, list[0].Amount);
        Assert.AreEqual(2, list[1].Id);
        Assert.AreEqual("Bob", list[1].Name);
    }

    [TestMethod]
    public void WhenToListWithExtraColumnsThenIgnored()
    {
        var record = new Record("Test", 5);
        var idCol = record.Columns.Add<int>("Id");
        record.Columns.Add<string>("Extra");

        var row = record.AddRow();
        idCol.Set(42, row.Row);

        var list = record.ToList<SimpleEntity>();

        Assert.AreEqual(1, list.Count);
        Assert.AreEqual(42, list[0].Id);
        Assert.AreEqual(string.Empty, list[0].Name); // default
    }

    [TestMethod]
    public void WhenToListWithMissingColumnThenDefaultValue()
    {
        var record = new Record("Test", 5);
        var idCol = record.Columns.Add<int>("Id");

        var row = record.AddRow();
        idCol.Set(1, row.Row);

        var list = record.ToList<SimpleEntity>();

        Assert.AreEqual(1, list.Count);
        Assert.AreEqual(1, list[0].Id);
        Assert.AreEqual(0m, list[0].Amount);
    }

    [TestMethod]
    public void WhenToListWithRequireAllPropertiesThenThrows()
    {
        var record = new Record("Test", 5);
        record.Columns.Add<int>("Id");

        var row = record.AddRow();

        var options = new RecordMappingOptions { RequireAllProperties = true };
        Assert.Throws<InvalidOperationException>(() => record.ToList<SimpleEntity>(options));
    }

    [TestMethod]
    public void WhenToListEmptyRecordThenEmptyList()
    {
        var record = new Record("Test", 5);
        record.Columns.Add<int>("Id");

        var list = record.ToList<SimpleEntity>();

        Assert.AreEqual(0, list.Count);
    }

    [TestMethod]
    public void WhenToListEnumFromIntThenConverts()
    {
        var record = new Record("Test", 5);
        var idCol = record.Columns.Add<int>("Id");
        var colorCol = record.Columns.Add<int>("Color");

        var row = record.AddRow();
        idCol.Set(1, row.Row);
        colorCol.Set(2, row.Row); // Blue

        var list = record.ToList<EntityWithEnum>();

        Assert.AreEqual(Color.Blue, list[0].Color);
    }

    [TestMethod]
    public void WhenToListEnumFromStringThenConverts()
    {
        var record = new Record("Test", 5);
        var idCol = record.Columns.Add<int>("Id");
        var colorCol = record.Columns.Add<string>("Color");

        var row = record.AddRow();
        idCol.Set(1, row.Row);
        colorCol.Set("Green", row.Row);

        var list = record.ToList<EntityWithEnum>();

        Assert.AreEqual(Color.Green, list[0].Color);
    }

    [TestMethod]
    public void WhenNullToNonNullableValueTypeThenDefault()
    {
        var record = new Record("Test", 5);
        var idCol = record.Columns.Add<int?>("Id");
        var scoreCol = record.Columns.Add<int?>("Score");

        var row = record.AddRow();
        // Id is null (default for int?), Score is null

        var list = record.ToList<EntityWithNullable>();

        Assert.AreEqual(0, list[0].Id); // null int? -> int = default(int) = 0
        Assert.IsNull(list[0].Score); // null int? -> int? = null
    }

    #endregion

    #region To<T> on RecordRow

    [TestMethod]
    public void WhenRowToThenMapsSingleRow()
    {
        var record = new Record("Test", 5);
        var idCol = record.Columns.Add<int>("Id");
        var nameCol = record.Columns.Add<string>("Name");

        var row = record.AddRow();
        idCol.Set(42, row.Row);
        nameCol.Set("Test", row.Row);

        var entity = row.To<SimpleEntity>();

        Assert.AreEqual(42, entity.Id);
        Assert.AreEqual("Test", entity.Name);
    }

    #endregion

    #region Constructor strategies

    [TestMethod]
    public void WhenRecordRowCtorThenUsesIt()
    {
        var record = new Record("Test", 5);
        var idCol = record.Columns.Add<int>("Id");
        var nameCol = record.Columns.Add<string>("Name");

        var row = record.AddRow();
        idCol.Set(7, row.Row);
        nameCol.Set("Custom", row.Row);

        var entity = row.To<RecordRowCtorEntity>();

        Assert.AreEqual(7, entity.Id);
        Assert.AreEqual("Custom", entity.Name);
    }

    [TestMethod]
    public void WhenMarkedCtorThenUsesIt()
    {
        var record = new Record("Test", 5);
        var idCol = record.Columns.Add<int>("Id");
        var nameCol = record.Columns.Add<string>("Name");

        var row = record.AddRow();
        idCol.Set(10, row.Row);
        nameCol.Set("Marked", row.Row);

        var entity = row.To<MarkedCtorEntity>();

        Assert.AreEqual(10, entity.Id);
        Assert.AreEqual("Marked", entity.Name);
    }

    #endregion

    #region NameTransform

    [TestMethod]
    public void WhenNameTransformThenMapsPropertyToColumnName()
    {
        var record = new Record("Test", 5);
        var idCol = record.Columns.Add<int>("entity_id");
        var nameCol = record.Columns.Add<string>("entity_name");

        var row = record.AddRow();
        idCol.Set(1, row.Row);
        nameCol.Set("Transformed", row.Row);

        // Simple transform: "Id" -> "entity_id", "Name" -> "entity_name"
        var options = new RecordMappingOptions
        {
            NameTransform = name => "entity_" + name.ToLowerInvariant()
        };

        var list = record.ToList<SimpleEntity>(options);

        Assert.AreEqual(1, list.Count);
        Assert.AreEqual(1, list[0].Id);
        Assert.AreEqual("Transformed", list[0].Name);
    }

    [TestMethod]
    public void WhenNameTransformAddRowThenMapsPropertyToColumnName()
    {
        var record = new Record("Test", 5);
        record.Columns.Add<int>("entity_id");
        record.Columns.Add<string>("entity_name");

        var options = new RecordMappingOptions
        {
            NameTransform = name => "entity_" + name.ToLowerInvariant()
        };

        record.AddRow(new SimpleEntity { Id = 5, Name = "Hello" }, options);

        Assert.AreEqual(5, record.Columns.Find<int>("entity_id")!.Get(0));
        Assert.AreEqual("Hello", record.Columns.Find<string>("entity_name")!.Get(0));
    }

    #endregion

    #region IRecordMapper

    public class TestMapper : IRecordMapper<SimpleEntity>
    {
        public void Write(SimpleEntity item, Record record, int row)
        {
            record.Columns.Get("Id").SetValue(item.Id * 10, row);
            record.Columns.Get("Name").SetValue(item.Name + "!", row);
        }

        public SimpleEntity Read(Record record, int row)
        {
            return new SimpleEntity
            {
                Id = (int)record.Columns.Get("Id").GetValue(row)! / 10,
                Name = ((string)record.Columns.Get("Name").GetValue(row)!).TrimEnd('!'),
            };
        }

        void IRecordMapper.Write(object item, Record record, int row) => Write((SimpleEntity)item, record, row);
        object IRecordMapper.Read(Record record, int row) => Read(record, row);
    }

    [TestMethod]
    public void WhenCustomMapperAddRowThenUsesMapper()
    {
        var record = new Record("Test", 5);
        record.Columns.Add<int>("Id");
        record.Columns.Add<string>("Name");

        var options = new RecordMappingOptions { Mapper = new TestMapper() };
        record.AddRow(new SimpleEntity { Id = 5, Name = "Test" }, options);

        Assert.AreEqual(50, record.Columns.Find<int>("Id")!.Get(0));
        Assert.AreEqual("Test!", record.Columns.Find<string>("Name")!.Get(0));
    }

    [TestMethod]
    public void WhenCustomMapperToListThenUsesMapper()
    {
        var record = new Record("Test", 5);
        var idCol = record.Columns.Add<int>("Id");
        var nameCol = record.Columns.Add<string>("Name");

        var row = record.AddRow();
        idCol.Set(50, row.Row);
        nameCol.Set("Test!", row.Row);

        var options = new RecordMappingOptions { Mapper = new TestMapper() };
        var list = record.ToList<SimpleEntity>(options);

        Assert.AreEqual(5, list[0].Id);
        Assert.AreEqual("Test", list[0].Name);
    }

    #endregion

    #region Roundtrip

    [TestMethod]
    public void WhenAddRowThenToListThenRoundtrips()
    {
        var record = new Record("Test", 5);
        record.Columns.Add<int>("Id");
        record.Columns.Add<string>("Name");
        record.Columns.Add<decimal>("Amount");

        var original = new SimpleEntity { Id = 42, Name = "Roundtrip", Amount = 123.45m };
        record.AddRow(original);

        var list = record.ToList<SimpleEntity>();

        Assert.AreEqual(1, list.Count);
        Assert.AreEqual(42, list[0].Id);
        Assert.AreEqual("Roundtrip", list[0].Name);
        Assert.AreEqual(123.45m, list[0].Amount);
    }

    #endregion
}
