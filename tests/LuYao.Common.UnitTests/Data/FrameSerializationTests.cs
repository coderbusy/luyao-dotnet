using System;
using System.IO;

namespace LuYao.Data;

[TestClass]
public class FrameSerializationTests
{
    private enum FavoriteType
    {
        Common = 100,
        Chat = 200,
    }

    private static Frame CreateTestFrame()
    {
        var record = new Frame("Orders", 2);
        var idCol = record.Columns.Add<int>("Id");
        var nameCol = record.Columns.Add<string>("Name");
        record.Page = 2;
        record.PageSize = 10;
        record.MaxCount = 50;

        var row1 = record.AddRow();
        idCol.SetValue(row1.Row, 1);
        nameCol.SetValue(row1.Row, "Order-1");

        var row2 = record.AddRow();
        idCol.SetValue(row2.Row, 2);
        nameCol.SetValue(row2.Row, "Order-2");

        return record;
    }

    #region Binary Serialization

    [TestMethod]
    public void WhenBinaryRoundTripThenDataPreserved()
    {
        var original = CreateTestFrame();

        var bytes = original.ToBytes();
        var deserialized = Frame.FromBytes(bytes);

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
    public void WhenBinaryRoundTripEmptyFrameThenSchemaPreserved()
    {
        var original = new Frame("Empty", 0);
        original.Columns.Add<int>("Id");
        original.Columns.Add<string>("Name");

        var bytes = original.ToBytes();
        var deserialized = Frame.FromBytes(bytes);

        Assert.AreEqual("Empty", deserialized.Name);
        Assert.AreEqual(0, deserialized.Count);
        Assert.AreEqual(2, deserialized.Columns.Count);
        Assert.AreEqual("Id", deserialized.Columns[0].Name);
        Assert.AreEqual(typeof(int), deserialized.Columns[0].Type);
    }

    [TestMethod]
    public void WhenBinaryRoundTripWithNullValuesThenPreserved()
    {
        var record = new Frame("Test", 1);
        record.Columns.Add<string>("Name");
        record.Columns.Add<int?>("Value");
        record.AddRow();

        var bytes = record.ToBytes();
        var deserialized = Frame.FromBytes(bytes);

        Assert.AreEqual(1, deserialized.Count);
        Assert.IsNull(deserialized.Columns[0].Get(0));
        Assert.IsNull(deserialized.Columns[1].Get(0));
    }

    [TestMethod]
    public void WhenBinaryRoundTripWithByteArrayThenPreserved()
    {
        var record = new Frame("Binary", 1);
        var col = record.Columns.Add<byte[]>("Data");
        var row = record.AddRow();
        col.SetValue(row.Row, new byte[] { 1, 2, 3, 4, 5 });

        var bytes = record.ToBytes();
        var deserialized = Frame.FromBytes(bytes);

        var data = deserialized.Columns.Find<byte[]>("Data")!.GetValue(0);
        CollectionAssert.AreEqual(new byte[] { 1, 2, 3, 4, 5 }, data);
    }

    [TestMethod]
    public void WhenBinaryRoundTripWithAllTypesThenPreserved()
    {
        var record = new Frame("AllTypes", 1);
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
        record.Columns.Find<bool>("Bool")!.SetValue(r.Row, true);
        record.Columns.Find<sbyte>("SByte")!.SetValue(r.Row, -1);
        record.Columns.Find<short>("Short")!.SetValue(r.Row, short.MaxValue);
        record.Columns.Find<int>("Int")!.SetValue(r.Row, 42);
        record.Columns.Find<long>("Long")!.SetValue(r.Row, long.MaxValue);
        record.Columns.Find<byte>("Byte")!.SetValue(r.Row, 255);
        record.Columns.Find<ushort>("UShort")!.SetValue(r.Row, ushort.MaxValue);
        record.Columns.Find<uint>("UInt")!.SetValue(r.Row, uint.MaxValue);
        record.Columns.Find<ulong>("ULong")!.SetValue(r.Row, ulong.MaxValue);
        record.Columns.Find<float>("Float")!.SetValue(r.Row, 3.14f);
        record.Columns.Find<double>("Double")!.SetValue(r.Row, 3.14159265);
        record.Columns.Find<decimal>("Decimal")!.SetValue(r.Row, 99.99m);
        record.Columns.Find<char>("Char")!.SetValue(r.Row, 'A');
        record.Columns.Find<string>("String")!.SetValue(r.Row, "Hello");
        var dt = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc);
        record.Columns.Find<DateTime>("DateTime")!.SetValue(r.Row, dt);
        var dto = new DateTimeOffset(2024, 1, 15, 10, 30, 0, TimeSpan.FromHours(8));
        record.Columns.Find<DateTimeOffset>("DateTimeOffset")!.SetValue(r.Row, dto);
        record.Columns.Find<TimeSpan>("TimeSpan")!.SetValue(r.Row, TimeSpan.FromHours(2.5));
        var guid = Guid.NewGuid();
        record.Columns.Find<Guid>("Guid")!.SetValue(r.Row, guid);

        var bytes = record.ToBytes();
        var d = Frame.FromBytes(bytes);

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
        var record = new Frame("EnumFrame", 1);
        var favoriteCol = record.Columns.Add<FavoriteType>("Favorite");
        var nullableFavoriteCol = record.Columns.Add<FavoriteType?>("NullableFavorite");
        var row = record.AddRow();
        favoriteCol.SetValue(row.Row, FavoriteType.Chat);
        nullableFavoriteCol.SetValue(row.Row, FavoriteType.Common);

        var bytes = record.ToBytes();
        var deserialized = Frame.FromBytes(bytes);

        Assert.AreEqual(200, deserialized.Columns.Find<int>("Favorite")!.Get(0));
        Assert.AreEqual(100, deserialized.Columns.Find<int?>("NullableFavorite")!.Get(0));
    }

    [TestMethod]
    public void WhenSerializeToBytesThenFrameHeaderContainsFrameType()
    {
        var record = new Frame("Orders", 1);
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
    public void WhenReadFrameFromFrameSetPayloadThenThrowTypeMismatch()
    {
        var set = new FrameSet();
        set.Add("Orders", new Frame("Orders", 0));

        var bytes = set.ToBytes();

        Assert.Throws<InvalidOperationException>(() => Frame.FromBytes(bytes));
    }

    [TestMethod]
    public void IsBinaryPayload_DetectsFrameAndFrameSetCorrectly()
    {
        var record = new Frame("Orders", 0);
        var set = new FrameSet();
        set.Add("Orders", new Frame("Orders", 0));

        var recordBytes = record.ToBytes();
        var setBytes = set.ToBytes();

        Assert.IsTrue(Frame.IsBinaryPayload(recordBytes));
        Assert.IsFalse(Frame.IsBinaryPayload(setBytes));
        Assert.IsFalse(Frame.IsBinaryPayload(new byte[] { 1, 2, 3, 4 }));
        Assert.IsFalse(Frame.IsBinaryPayload((byte[])null));
    }

    #endregion
}

[TestClass]
public class FrameSetSerializationTests
{
    #region Binary Serialization

    [TestMethod]
    public void WhenBinaryRoundTripThenAllFramesPreserved()
    {
        var set = new FrameSet();

        var orders = new Frame("Orders", 1);
        orders.Columns.Add<int>("Id");
        var row = orders.AddRow();
        orders.Columns[0].Set(row.Row, 1);
        set.Add("Orders", orders);

        var customers = new Frame("Customers", 1);
        customers.Columns.Add<string>("Name");
        var row2 = customers.AddRow();
        customers.Columns[0].Set(row2.Row, "Alice");
        set.Add("Customers", customers);

        var bytes = set.ToBytes();
        var deserialized = FrameSet.FromBytes(bytes);

        Assert.AreEqual(2, deserialized.Count);
        Assert.IsTrue(deserialized.Contains("Orders"));
        Assert.IsTrue(deserialized.Contains("Customers"));
        Assert.AreEqual(1, deserialized["Orders"].Count);
        Assert.AreEqual(1, deserialized["Customers"].Count);
    }

    [TestMethod]
    public void WhenBinaryRoundTripEmptySetThenEmpty()
    {
        var set = new FrameSet();

        var bytes = set.ToBytes();
        var deserialized = FrameSet.FromBytes(bytes);

        Assert.AreEqual(0, deserialized.Count);
    }

    [TestMethod]
    public void WhenSerializeToBytesThenFrameSetHeaderContainsFrameSetType()
    {
        var set = new FrameSet();
        set.Add("Orders", new Frame("Orders", 0));

        var bytes = set.ToBytes();

        Assert.IsTrue(bytes.Length >= 5);
        Assert.AreEqual((byte)0xFF, bytes[0]);
        Assert.AreEqual((byte)'L', bytes[1]);
        Assert.AreEqual((byte)'Y', bytes[2]);
        Assert.AreEqual((byte)2, bytes[3]);
    }

    [TestMethod]
    public void WhenReadFrameSetFromFramePayloadThenThrowTypeMismatch()
    {
        var record = new Frame("Orders", 0);
        var bytes = record.ToBytes();

        Assert.Throws<InvalidOperationException>(() => FrameSet.FromBytes(bytes));
    }

    [TestMethod]
    public void IsBinaryPayload_DetectsFrameSetAndFrameCorrectly()
    {
        var record = new Frame("Orders", 0);
        var set = new FrameSet();
        set.Add("Orders", new Frame("Orders", 0));

        var recordBytes = record.ToBytes();
        var setBytes = set.ToBytes();

        Assert.IsTrue(FrameSet.IsBinaryPayload(setBytes));
        Assert.IsFalse(FrameSet.IsBinaryPayload(recordBytes));
        Assert.IsFalse(FrameSet.IsBinaryPayload(new byte[] { 0xFF, (byte)'L', (byte)'Y', 9 }));
        Assert.IsFalse(FrameSet.IsBinaryPayload((byte[])null));
    }

    #endregion
}
