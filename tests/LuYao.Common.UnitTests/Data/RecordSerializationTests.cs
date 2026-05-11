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

    private static RecordTable CreateTestRecord()
    {
        var table = new RecordTable("Orders", 2);
        var idCol = table.Columns.Add<int>("Id");
        var nameCol = table.Columns.Add<string>("Name");
        table.Page = 2;
        table.PageSize = 10;
        table.MaxCount = 50;

        var row1 = table.AddRow();
        idCol.SetValue(row1.Row, 1);
        nameCol.SetValue(row1.Row, "Order-1");

        var row2 = table.AddRow();
        idCol.SetValue(row2.Row, 2);
        nameCol.SetValue(row2.Row, "Order-2");

        return table;
    }

    #region Binary Serialization

    [TestMethod]
    public void WhenBinaryRoundTripThenDataPreserved()
    {
        var original = CreateTestRecord();

        var bytes = original.ToBytes();
        var deserialized = RecordTable.FromBytes(bytes);

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
        var original = new RecordTable("Empty", 0);
        original.Columns.Add<int>("Id");
        original.Columns.Add<string>("Name");

        var bytes = original.ToBytes();
        var deserialized = RecordTable.FromBytes(bytes);

        Assert.AreEqual("Empty", deserialized.Name);
        Assert.AreEqual(0, deserialized.Count);
        Assert.AreEqual(2, deserialized.Columns.Count);
        Assert.AreEqual("Id", deserialized.Columns[0].Name);
        Assert.AreEqual(typeof(int), deserialized.Columns[0].Type);
    }

    [TestMethod]
    public void WhenBinaryRoundTripWithNullValuesThenPreserved()
    {
        var table = new RecordTable("Test", 1);
        table.Columns.Add<string>("Name");
        table.Columns.Add<int?>("Value");
        table.AddRow();

        var bytes = table.ToBytes();
        var deserialized = RecordTable.FromBytes(bytes);

        Assert.AreEqual(1, deserialized.Count);
        Assert.IsNull(deserialized.Columns[0].Get(0));
        Assert.IsNull(deserialized.Columns[1].Get(0));
    }

    [TestMethod]
    public void WhenBinaryRoundTripWithByteArrayThenPreserved()
    {
        var table = new RecordTable("Binary", 1);
        var col = table.Columns.Add<byte[]>("Data");
        var row = table.AddRow();
        col.SetValue(row.Row, new byte[] { 1, 2, 3, 4, 5 });

        var bytes = table.ToBytes();
        var deserialized = RecordTable.FromBytes(bytes);

        var data = deserialized.Columns.Find<byte[]>("Data")!.GetValue(0);
        CollectionAssert.AreEqual(new byte[] { 1, 2, 3, 4, 5 }, data);
    }

    [TestMethod]
    public void WhenBinaryRoundTripWithAllTypesThenPreserved()
    {
        var table = new RecordTable("AllTypes", 1);
        table.Columns.Add<bool>("Bool");
        table.Columns.Add<sbyte>("SByte");
        table.Columns.Add<short>("Short");
        table.Columns.Add<int>("Int");
        table.Columns.Add<long>("Long");
        table.Columns.Add<byte>("Byte");
        table.Columns.Add<ushort>("UShort");
        table.Columns.Add<uint>("UInt");
        table.Columns.Add<ulong>("ULong");
        table.Columns.Add<float>("Float");
        table.Columns.Add<double>("Double");
        table.Columns.Add<decimal>("Decimal");
        table.Columns.Add<char>("Char");
        table.Columns.Add<string>("String");
        table.Columns.Add<DateTime>("DateTime");
        table.Columns.Add<DateTimeOffset>("DateTimeOffset");
        table.Columns.Add<TimeSpan>("TimeSpan");
        table.Columns.Add<Guid>("Guid");

        var r = table.AddRow();
        table.Columns.Find<bool>("Bool")!.SetValue(r.Row, true);
        table.Columns.Find<sbyte>("SByte")!.SetValue(r.Row, -1);
        table.Columns.Find<short>("Short")!.SetValue(r.Row, short.MaxValue);
        table.Columns.Find<int>("Int")!.SetValue(r.Row, 42);
        table.Columns.Find<long>("Long")!.SetValue(r.Row, long.MaxValue);
        table.Columns.Find<byte>("Byte")!.SetValue(r.Row, 255);
        table.Columns.Find<ushort>("UShort")!.SetValue(r.Row, ushort.MaxValue);
        table.Columns.Find<uint>("UInt")!.SetValue(r.Row, uint.MaxValue);
        table.Columns.Find<ulong>("ULong")!.SetValue(r.Row, ulong.MaxValue);
        table.Columns.Find<float>("Float")!.SetValue(r.Row, 3.14f);
        table.Columns.Find<double>("Double")!.SetValue(r.Row, 3.14159265);
        table.Columns.Find<decimal>("Decimal")!.SetValue(r.Row, 99.99m);
        table.Columns.Find<char>("Char")!.SetValue(r.Row, 'A');
        table.Columns.Find<string>("String")!.SetValue(r.Row, "Hello");
        var dt = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc);
        table.Columns.Find<DateTime>("DateTime")!.SetValue(r.Row, dt);
        var dto = new DateTimeOffset(2024, 1, 15, 10, 30, 0, TimeSpan.FromHours(8));
        table.Columns.Find<DateTimeOffset>("DateTimeOffset")!.SetValue(r.Row, dto);
        table.Columns.Find<TimeSpan>("TimeSpan")!.SetValue(r.Row, TimeSpan.FromHours(2.5));
        var guid = Guid.NewGuid();
        table.Columns.Find<Guid>("Guid")!.SetValue(r.Row, guid);

        var bytes = table.ToBytes();
        var d = RecordTable.FromBytes(bytes);

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
        var table = new RecordTable("EnumRecord", 1);
        var favoriteCol = table.Columns.Add<FavoriteType>("Favorite");
        var nullableFavoriteCol = table.Columns.Add<FavoriteType?>("NullableFavorite");
        var row = table.AddRow();
        favoriteCol.SetValue(row.Row, FavoriteType.Chat);
        nullableFavoriteCol.SetValue(row.Row, FavoriteType.Common);

        var bytes = table.ToBytes();
        var deserialized = RecordTable.FromBytes(bytes);

        Assert.AreEqual(200, deserialized.Columns.Find<int>("Favorite")!.Get(0));
        Assert.AreEqual(100, deserialized.Columns.Find<int?>("NullableFavorite")!.Get(0));
    }

    [TestMethod]
    public void WhenSerializeToBytesThenRecordHeaderContainsRecordType()
    {
        var table = new RecordTable("Orders", 1);
        table.Columns.Add<int>("Id");
        table.AddRow();

        var bytes = table.ToBytes();

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
        set.Add("Orders", new RecordTable("Orders", 0));

        var bytes = set.ToBytes();

        Assert.Throws<InvalidOperationException>(() => RecordTable.FromBytes(bytes));
    }

    [TestMethod]
    public void IsBinaryPayload_DetectsRecordAndRecordSetCorrectly()
    {
        var table = new RecordTable("Orders", 0);
        var set = new RecordSet();
        set.Add("Orders", new RecordTable("Orders", 0));

        var recordBytes = table.ToBytes();
        var setBytes = set.ToBytes();

        Assert.IsTrue(RecordTable.IsBinaryPayload(recordBytes));
        Assert.IsFalse(RecordTable.IsBinaryPayload(setBytes));
        Assert.IsFalse(RecordTable.IsBinaryPayload(new byte[] { 1, 2, 3, 4 }));
        Assert.IsFalse(RecordTable.IsBinaryPayload((byte[])null));
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

        var orders = new RecordTable("Orders", 1);
        orders.Columns.Add<int>("Id");
        var row = orders.AddRow();
        orders.Columns[0].Set(row.Row, 1);
        set.Add("Orders", orders);

        var customers = new RecordTable("Customers", 1);
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
        set.Add("Orders", new RecordTable("Orders", 0));

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
        var table = new RecordTable("Orders", 0);
        var bytes = table.ToBytes();

        Assert.Throws<InvalidOperationException>(() => RecordSet.FromBytes(bytes));
    }

    [TestMethod]
    public void IsBinaryPayload_DetectsRecordSetAndRecordCorrectly()
    {
        var table = new RecordTable("Orders", 0);
        var set = new RecordSet();
        set.Add("Orders", new RecordTable("Orders", 0));

        var recordBytes = table.ToBytes();
        var setBytes = set.ToBytes();

        Assert.IsTrue(RecordSet.IsBinaryPayload(setBytes));
        Assert.IsFalse(RecordSet.IsBinaryPayload(recordBytes));
        Assert.IsFalse(RecordSet.IsBinaryPayload(new byte[] { 0xFF, (byte)'L', (byte)'Y', 9 }));
        Assert.IsFalse(RecordSet.IsBinaryPayload((byte[])null));
    }

    #endregion
}
