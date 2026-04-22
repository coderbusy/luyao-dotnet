using System;
using System.IO;

namespace LuYao.Data;

[TestClass]
public class RecordSerializationTests
{
    private enum FavoriteType
    {
        Common = 100,
        Chat = 200,
    }

    private static Record CreateTestRecord()
    {
        var record = new Record("Orders", 2);
        var idCol = record.Columns.Add<int>("Id");
        var nameCol = record.Columns.Add<string>("Name");
        record.Page = 2;
        record.PageSize = 10;
        record.MaxCount = 50;

        var row1 = record.AddRow();
        idCol.SetField(row1.Row, 1);
        nameCol.SetField(row1.Row, "Order-1");

        var row2 = record.AddRow();
        idCol.SetField(row2.Row, 2);
        nameCol.SetField(row2.Row, "Order-2");

        return record;
    }

    #region Binary Serialization

    [TestMethod]
    public void WhenBinaryRoundTripThenDataPreserved()
    {
        var original = CreateTestRecord();

        var bytes = original.ToBytes();
        var deserialized = Record.FromBytes(bytes);

        Assert.AreEqual("Orders", deserialized.Name);
        Assert.AreEqual(2, deserialized.Count);
        Assert.AreEqual(2, deserialized.Columns.Count);
        Assert.AreEqual(2, deserialized.Page);
        Assert.AreEqual(10, deserialized.PageSize);
        Assert.AreEqual(50, deserialized.MaxCount);

        Assert.AreEqual(1, deserialized.Columns.Find<int>("Id")!.Get(0));
        Assert.AreEqual("Order-1", deserialized.Columns.Find<string>("Name")!.Get(0));
        Assert.AreEqual(2, deserialized.Columns.Find<int>("Id")!.Get(1));
        Assert.AreEqual("Order-2", deserialized.Columns.Find<string>("Name")!.Get(1));
    }

    [TestMethod]
    public void WhenBinaryRoundTripEmptyRecordThenSchemaPreserved()
    {
        var original = new Record("Empty", 0);
        original.Columns.Add<int>("Id");
        original.Columns.Add<string>("Name");

        var bytes = original.ToBytes();
        var deserialized = Record.FromBytes(bytes);

        Assert.AreEqual("Empty", deserialized.Name);
        Assert.AreEqual(0, deserialized.Count);
        Assert.AreEqual(2, deserialized.Columns.Count);
        Assert.AreEqual("Id", deserialized.Columns[0].Name);
        Assert.AreEqual(typeof(int), deserialized.Columns[0].Type);
    }

    [TestMethod]
    public void WhenBinaryRoundTripWithNullValuesThenPreserved()
    {
        var record = new Record("Test", 1);
        record.Columns.Add<string>("Name");
        record.Columns.Add<int?>("Value");
        record.AddRow();

        var bytes = record.ToBytes();
        var deserialized = Record.FromBytes(bytes);

        Assert.AreEqual(1, deserialized.Count);
        Assert.IsNull(deserialized.Columns[0].Get(0));
        Assert.IsNull(deserialized.Columns[1].Get(0));
    }

    [TestMethod]
    public void WhenBinaryRoundTripWithByteArrayThenPreserved()
    {
        var record = new Record("Binary", 1);
        var col = record.Columns.Add<byte[]>("Data");
        var row = record.AddRow();
        col.SetField(row.Row, new byte[] { 1, 2, 3, 4, 5 });

        var bytes = record.ToBytes();
        var deserialized = Record.FromBytes(bytes);

        var data = deserialized.Columns.Find<byte[]>("Data")!.Field(0);
        CollectionAssert.AreEqual(new byte[] { 1, 2, 3, 4, 5 }, data);
    }

    [TestMethod]
    public void WhenBinaryRoundTripWithAllTypesThenPreserved()
    {
        var record = new Record("AllTypes", 1);
        record.Columns.Add<bool>("Bool");
        record.Columns.Add<sbyte>("SByte");
        record.Columns.Add<short>("Short");
        record.Columns.Add<int>("Int");
        record.Columns.Add<long>("Long");
        record.Columns.Add<byte>("Byte");
        record.Columns.Add<ushort>("UShort");
        record.Columns.Add<uint>("UInt");
        record.Columns.Add<ulong>("ULong");
        record.Columns.Add<float>("Float");
        record.Columns.Add<double>("Double");
        record.Columns.Add<decimal>("Decimal");
        record.Columns.Add<char>("Char");
        record.Columns.Add<string>("String");
        record.Columns.Add<DateTime>("DateTime");
        record.Columns.Add<DateTimeOffset>("DateTimeOffset");
        record.Columns.Add<TimeSpan>("TimeSpan");
        record.Columns.Add<Guid>("Guid");

        var r = record.AddRow();
        record.Columns.Find<bool>("Bool")!.SetField(r.Row, true);
        record.Columns.Find<sbyte>("SByte")!.SetField(r.Row, -1);
        record.Columns.Find<short>("Short")!.SetField(r.Row, short.MaxValue);
        record.Columns.Find<int>("Int")!.SetField(r.Row, 42);
        record.Columns.Find<long>("Long")!.SetField(r.Row, long.MaxValue);
        record.Columns.Find<byte>("Byte")!.SetField(r.Row, 255);
        record.Columns.Find<ushort>("UShort")!.SetField(r.Row, ushort.MaxValue);
        record.Columns.Find<uint>("UInt")!.SetField(r.Row, uint.MaxValue);
        record.Columns.Find<ulong>("ULong")!.SetField(r.Row, ulong.MaxValue);
        record.Columns.Find<float>("Float")!.SetField(r.Row, 3.14f);
        record.Columns.Find<double>("Double")!.SetField(r.Row, 3.14159265);
        record.Columns.Find<decimal>("Decimal")!.SetField(r.Row, 99.99m);
        record.Columns.Find<char>("Char")!.SetField(r.Row, 'A');
        record.Columns.Find<string>("String")!.SetField(r.Row, "Hello");
        var dt = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc);
        record.Columns.Find<DateTime>("DateTime")!.SetField(r.Row, dt);
        var dto = new DateTimeOffset(2024, 1, 15, 10, 30, 0, TimeSpan.FromHours(8));
        record.Columns.Find<DateTimeOffset>("DateTimeOffset")!.SetField(r.Row, dto);
        record.Columns.Find<TimeSpan>("TimeSpan")!.SetField(r.Row, TimeSpan.FromHours(2.5));
        var guid = Guid.NewGuid();
        record.Columns.Find<Guid>("Guid")!.SetField(r.Row, guid);

        var bytes = record.ToBytes();
        var d = Record.FromBytes(bytes);

        Assert.AreEqual(true, d.Columns.Find<bool>("Bool")!.Get(0));
        Assert.AreEqual((sbyte)-1, d.Columns.Find<sbyte>("SByte")!.Get(0));
        Assert.AreEqual(short.MaxValue, d.Columns.Find<short>("Short")!.Get(0));
        Assert.AreEqual(42, d.Columns.Find<int>("Int")!.Get(0));
        Assert.AreEqual(long.MaxValue, d.Columns.Find<long>("Long")!.Get(0));
        Assert.AreEqual((byte)255, d.Columns.Find<byte>("Byte")!.Get(0));
        Assert.AreEqual(ushort.MaxValue, d.Columns.Find<ushort>("UShort")!.Get(0));
        Assert.AreEqual(uint.MaxValue, d.Columns.Find<uint>("UInt")!.Get(0));
        Assert.AreEqual(ulong.MaxValue, d.Columns.Find<ulong>("ULong")!.Get(0));
        Assert.AreEqual(3.14f, d.Columns.Find<float>("Float")!.Get(0));
        Assert.AreEqual(3.14159265, d.Columns.Find<double>("Double")!.Get(0));
        Assert.AreEqual(99.99m, d.Columns.Find<decimal>("Decimal")!.Get(0));
        Assert.AreEqual('A', d.Columns.Find<char>("Char")!.Get(0));
        Assert.AreEqual("Hello", d.Columns.Find<string>("String")!.Get(0));
        Assert.AreEqual(dt, d.Columns.Find<DateTime>("DateTime")!.Get(0));
        Assert.AreEqual(dto, d.Columns.Find<DateTimeOffset>("DateTimeOffset")!.Get(0));
        Assert.AreEqual(TimeSpan.FromHours(2.5), d.Columns.Find<TimeSpan>("TimeSpan")!.Get(0));
        Assert.AreEqual(guid, d.Columns.Find<Guid>("Guid")!.Get(0));
    }

    [TestMethod]
    public void WhenBinaryRoundTripWithEnumColumnThenPreserved()
    {
        var record = new Record("EnumRecord", 1);
        var favoriteCol = record.Columns.Add<FavoriteType>("Favorite");
        var nullableFavoriteCol = record.Columns.Add<FavoriteType?>("NullableFavorite");
        var row = record.AddRow();
        favoriteCol.SetField(row.Row, FavoriteType.Chat);
        nullableFavoriteCol.SetField(row.Row, FavoriteType.Common);

        var bytes = record.ToBytes();
        var deserialized = Record.FromBytes(bytes);

        Assert.AreEqual(200, deserialized.Columns.Find<int>("Favorite")!.Get(0));
        Assert.AreEqual(100, deserialized.Columns.Find<int?>("NullableFavorite")!.Get(0));
    }

    [TestMethod]
    public void WhenSerializeToBytesThenRecordHeaderContainsRecordType()
    {
        var record = new Record("Orders", 1);
        record.Columns.Add<int>("Id");
        record.AddRow();

        var bytes = record.ToBytes();

        Assert.IsTrue(bytes.Length >= 5);
        Assert.AreEqual((byte)0xFF, bytes[0]);
        Assert.AreEqual((byte)'L', bytes[1]);
        Assert.AreEqual((byte)'Y', bytes[2]);
        Assert.AreEqual((byte)1, bytes[3]);
    }

    [TestMethod]
    public void WhenReadRecordFromRecordSetPayloadThenThrowTypeMismatch()
    {
        var set = new RecordSet();
        set.Add("Orders", new Record("Orders", 0));

        var bytes = set.ToBytes();

        Assert.Throws<InvalidOperationException>(() => Record.FromBytes(bytes));
    }

    [TestMethod]
    public void IsBinaryPayload_DetectsRecordAndRecordSetCorrectly()
    {
        var record = new Record("Orders", 0);
        var set = new RecordSet();
        set.Add("Orders", new Record("Orders", 0));

        var recordBytes = record.ToBytes();
        var setBytes = set.ToBytes();

        Assert.IsTrue(Record.IsBinaryPayload(recordBytes));
        Assert.IsFalse(Record.IsBinaryPayload(setBytes));
        Assert.IsFalse(Record.IsBinaryPayload(new byte[] { 1, 2, 3, 4 }));
        Assert.IsFalse(Record.IsBinaryPayload((byte[])null));
    }

    #endregion
}

[TestClass]
public class RecordSetSerializationTests
{
    #region Binary Serialization

    [TestMethod]
    public void WhenBinaryRoundTripThenAllRecordsPreserved()
    {
        var set = new RecordSet();

        var orders = new Record("Orders", 1);
        orders.Columns.Add<int>("Id");
        var row = orders.AddRow();
        orders.Columns[0].Set(row.Row, 1);
        set.Add("Orders", orders);

        var customers = new Record("Customers", 1);
        customers.Columns.Add<string>("Name");
        var row2 = customers.AddRow();
        customers.Columns[0].Set(row2.Row, "Alice");
        set.Add("Customers", customers);

        var bytes = set.ToBytes();
        var deserialized = RecordSet.FromBytes(bytes);

        Assert.AreEqual(2, deserialized.Count);
        Assert.IsTrue(deserialized.Contains("Orders"));
        Assert.IsTrue(deserialized.Contains("Customers"));
        Assert.AreEqual(1, deserialized["Orders"].Count);
        Assert.AreEqual(1, deserialized["Customers"].Count);
    }

    [TestMethod]
    public void WhenBinaryRoundTripEmptySetThenEmpty()
    {
        var set = new RecordSet();

        var bytes = set.ToBytes();
        var deserialized = RecordSet.FromBytes(bytes);

        Assert.AreEqual(0, deserialized.Count);
    }

    [TestMethod]
    public void WhenSerializeToBytesThenRecordSetHeaderContainsRecordSetType()
    {
        var set = new RecordSet();
        set.Add("Orders", new Record("Orders", 0));

        var bytes = set.ToBytes();

        Assert.IsTrue(bytes.Length >= 5);
        Assert.AreEqual((byte)0xFF, bytes[0]);
        Assert.AreEqual((byte)'L', bytes[1]);
        Assert.AreEqual((byte)'Y', bytes[2]);
        Assert.AreEqual((byte)2, bytes[3]);
    }

    [TestMethod]
    public void WhenReadRecordSetFromRecordPayloadThenThrowTypeMismatch()
    {
        var record = new Record("Orders", 0);
        var bytes = record.ToBytes();

        Assert.Throws<InvalidOperationException>(() => RecordSet.FromBytes(bytes));
    }

    [TestMethod]
    public void IsBinaryPayload_DetectsRecordSetAndRecordCorrectly()
    {
        var record = new Record("Orders", 0);
        var set = new RecordSet();
        set.Add("Orders", new Record("Orders", 0));

        var recordBytes = record.ToBytes();
        var setBytes = set.ToBytes();

        Assert.IsTrue(RecordSet.IsBinaryPayload(setBytes));
        Assert.IsFalse(RecordSet.IsBinaryPayload(recordBytes));
        Assert.IsFalse(RecordSet.IsBinaryPayload(new byte[] { 0xFF, (byte)'L', (byte)'Y', 9 }));
        Assert.IsFalse(RecordSet.IsBinaryPayload((byte[])null));
    }

    #endregion
}
