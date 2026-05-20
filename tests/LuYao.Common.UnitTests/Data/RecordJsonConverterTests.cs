using LuYao.Data.Binary;
using LuYao.Data.Json;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.Json;

namespace LuYao.Data;

[TestClass]
public class RecordJsonConverterTests
{
    #region RecordTableJsonConverter Tests

    [TestMethod]
    public void RecordTableJsonConverter_WhenSerializeAndDeserializeThenDataPreserved()
    {
        var original = CreateTestRecordTable();
        var options = new JsonSerializerOptions();
        options.Converters.Add(new RecordTableJsonConverter());

        var json = JsonSerializer.Serialize(original, options);
        var deserialized = JsonSerializer.Deserialize<RecordTable>(json, options);

        Assert.IsNotNull(deserialized);
        Assert.AreEqual("Orders", deserialized.Name);
        Assert.AreEqual(2, deserialized.Count);
        Assert.AreEqual(2, deserialized.Columns.Count);
        Assert.AreEqual(2, deserialized.Page);
        Assert.AreEqual(10, deserialized.PageSize);
        Assert.AreEqual(50, deserialized.MaxCount);

        var idCol = deserialized.Columns["Id"] as RecordColumn<int>;
        var nameCol = deserialized.Columns["Name"] as RecordColumn<string>;
        Assert.IsNotNull(idCol);
        Assert.IsNotNull(nameCol);

        Assert.AreEqual(1, idCol.GetValue(0));
        Assert.AreEqual("Order-1", nameCol.GetValue(0));
        Assert.AreEqual(2, idCol.GetValue(1));
        Assert.AreEqual("Order-2", nameCol.GetValue(1));
    }

    [TestMethod]
    public void RecordTableJsonConverter_WhenSerializeNullThenReturnsNull()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new RecordTableJsonConverter());

        var json = JsonSerializer.Serialize<RecordTable>(null, options);
        Assert.AreEqual("null", json);
    }

    [TestMethod]
    public void RecordTableJsonConverter_WhenDeserializeNullThenReturnsNull()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new RecordTableJsonConverter());

        var deserialized = JsonSerializer.Deserialize<RecordTable>("null", options);
        Assert.IsNull(deserialized);
    }

    [TestMethod]
    public void RecordTableJsonConverter_WhenDeserializeInvalidBase64ThenThrowsJsonException()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new RecordTableJsonConverter());

        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<RecordTable>("\"invalid-base64!!!\"", options));
    }

    [TestMethod]
    public void RecordTableJsonConverter_WhenDeserializeInvalidTokenTypeThenThrowsJsonException()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new RecordTableJsonConverter());

        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<RecordTable>("123", options));
    }

    [TestMethod]
    public void RecordTableJsonConverter_WhenDeserializeInvalidBinaryPayloadThenThrowsJsonException()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new RecordTableJsonConverter());

        var invalidBytes = new byte[] { 0x00, 0x01, 0x02, 0x03 };
        var base64 = Convert.ToBase64String(invalidBytes);
        var json = $"\"{base64}\"";

        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<RecordTable>(json, options));
    }

    #endregion

    #region RecordSetJsonConverter Tests

    [TestMethod]
    public void RecordSetJsonConverter_WhenSerializeAndDeserializeThenDataPreserved()
    {
        var original = CreateTestRecordSet();
        var options = new JsonSerializerOptions();
        options.Converters.Add(new RecordSetJsonConverter());

        var json = JsonSerializer.Serialize(original, options);
        var deserialized = JsonSerializer.Deserialize<RecordSet>(json, options);

        Assert.IsNotNull(deserialized);
        Assert.AreEqual(2, deserialized.Count);
        Assert.IsTrue(deserialized.Contains("Orders"));
        Assert.IsTrue(deserialized.Contains("Products"));

        var orders = deserialized["Orders"];
        Assert.IsNotNull(orders);
        Assert.AreEqual("Orders", orders.Name);
        Assert.AreEqual(1, orders.Count);

        var products = deserialized["Products"];
        Assert.IsNotNull(products);
        Assert.AreEqual("Products", products.Name);
        Assert.AreEqual(1, products.Count);
    }

    [TestMethod]
    public void RecordSetJsonConverter_WhenSerializeNullThenReturnsNull()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new RecordSetJsonConverter());

        var json = JsonSerializer.Serialize<RecordSet>(null, options);
        Assert.AreEqual("null", json);
    }

    [TestMethod]
    public void RecordSetJsonConverter_WhenDeserializeNullThenReturnsNull()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new RecordSetJsonConverter());

        var deserialized = JsonSerializer.Deserialize<RecordSet>("null", options);
        Assert.IsNull(deserialized);
    }

    [TestMethod]
    public void RecordSetJsonConverter_WhenDeserializeInvalidBase64ThenThrowsJsonException()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new RecordSetJsonConverter());

        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<RecordSet>("\"invalid-base64!!!\"", options));
    }

    [TestMethod]
    public void RecordSetJsonConverter_WhenDeserializeInvalidTokenTypeThenThrowsJsonException()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new RecordSetJsonConverter());

        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<RecordSet>("123", options));
    }

    [TestMethod]
    public void RecordSetJsonConverter_WhenDeserializeInvalidBinaryPayloadThenThrowsJsonException()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new RecordSetJsonConverter());

        var invalidBytes = new byte[] { 0x00, 0x01, 0x02, 0x03 };
        var base64 = Convert.ToBase64String(invalidBytes);
        var json = $"\"{base64}\"";

        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<RecordSet>(json, options));
    }

    [TestMethod]
    public void RecordSetJsonConverter_WhenRoundTripWithEmptySetThenSucceeds()
    {
        var original = new RecordSet();
        var options = new JsonSerializerOptions();
        options.Converters.Add(new RecordSetJsonConverter());

        var json = JsonSerializer.Serialize(original, options);
        var deserialized = JsonSerializer.Deserialize<RecordSet>(json, options);

        Assert.IsNotNull(deserialized);
        Assert.AreEqual(0, deserialized.Count);
    }

    #endregion

    #region Security / Compatibility Tests

    // 版本 1 头部（6 字节）：0xFE 'L' 'Y' 'Z' + version=1 + algo=GZip=1
    private static readonly byte[] CompressedHeader = { 0xFE, (byte)'L', (byte)'Y', (byte)'Z', 0x01, 0x01 };

    [TestMethod]
    public void RecordSetJsonConverter_WhenDeserializeUncompressedLegacyPayloadThenSucceeds()
    {
        // 旧的无压缩 RecordSet.ToBytes() 直接 Base64，应当走 passthrough 分支。
        var original = CreateTestRecordSet();
        var rawBytes = original.ToBytes();
        var json = "\"" + Convert.ToBase64String(rawBytes) + "\"";

        var options = new JsonSerializerOptions();
        options.Converters.Add(new RecordSetJsonConverter());

        var deserialized = JsonSerializer.Deserialize<RecordSet>(json, options);

        Assert.IsNotNull(deserialized);
        Assert.AreEqual(2, deserialized.Count);
        Assert.IsTrue(deserialized.Contains("Orders"));
        Assert.IsTrue(deserialized.Contains("Products"));
    }

    [TestMethod]
    public void RecordTableJsonConverter_WhenDeserializeUncompressedLegacyPayloadThenSucceeds()
    {
        // 旧的无压缩 RecordTable 二进制（直接 WriteTo），应当走 passthrough 分支。
        var original = CreateTestRecordTable();
        byte[] rawBytes;
        using (var ms = new MemoryStream())
        using (var bw = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true))
        {
            original.WriteTo(bw);
            bw.Flush();
            rawBytes = ms.ToArray();
        }
        var json = "\"" + Convert.ToBase64String(rawBytes) + "\"";

        var options = new JsonSerializerOptions();
        options.Converters.Add(new RecordTableJsonConverter());

        var deserialized = JsonSerializer.Deserialize<RecordTable>(json, options);

        Assert.IsNotNull(deserialized);
        Assert.AreEqual("Orders", deserialized.Name);
        Assert.AreEqual(2, deserialized.Count);
    }

    [TestMethod]
    public void RecordSetJsonConverter_WhenPayloadHasCompressionMarkerButInvalidGZipThenThrowsJsonException()
    {
        // 头部正确，但 gzip 主体为乱码，应当被包装为 JsonException。
        var bad = new byte[CompressedHeader.Length + 16];
        Buffer.BlockCopy(CompressedHeader, 0, bad, 0, CompressedHeader.Length);
        for (int i = CompressedHeader.Length; i < bad.Length; i++) bad[i] = 0xAB;
        var json = "\"" + Convert.ToBase64String(bad) + "\"";

        var options = new JsonSerializerOptions();
        options.Converters.Add(new RecordSetJsonConverter());

        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<RecordSet>(json, options));
    }

    [TestMethod]
    public void RecordTableJsonConverter_WhenPayloadHasCompressionMarkerButInvalidGZipThenThrowsJsonException()
    {
        var bad = new byte[CompressedHeader.Length + 16];
        Buffer.BlockCopy(CompressedHeader, 0, bad, 0, CompressedHeader.Length);
        for (int i = CompressedHeader.Length; i < bad.Length; i++) bad[i] = 0xAB;
        var json = "\"" + Convert.ToBase64String(bad) + "\"";

        var options = new JsonSerializerOptions();
        options.Converters.Add(new RecordTableJsonConverter());

        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<RecordTable>(json, options));
    }

    [TestMethod]
    public void RecordSetJsonConverter_WhenDecompressedSizeExceedsLimitThenThrowsJsonException()
    {
        // 构造 zip bomb：极小压缩输入，解压后远超 64MB 上限。
        byte[] compressed;
        using (var ms = new MemoryStream())
        {
            ms.Write(CompressedHeader, 0, CompressedHeader.Length);
            using (var gz = new GZipStream(ms, CompressionLevel.Optimal, leaveOpen: true))
            {
                var chunk = new byte[64 * 1024]; // 全零块，压缩比极高
                // 写入 ~80MB，超过 64MB 上限
                for (int i = 0; i < 1280; i++) gz.Write(chunk, 0, chunk.Length);
            }
            compressed = ms.ToArray();
        }

        var json = "\"" + Convert.ToBase64String(compressed) + "\"";
        var options = new JsonSerializerOptions();
        options.Converters.Add(new RecordSetJsonConverter());

        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<RecordSet>(json, options));
    }

    [TestMethod]
    public void RecordSetJsonConverter_WhenBase64PayloadTooLargeThenThrowsJsonException()
    {
        var json = BuildOversizeBase64Json();

        var options = new JsonSerializerOptions { MaxDepth = 64 };
        options.Converters.Add(new RecordSetJsonConverter());

        // System.Text.Json 默认对单个 token 的字符串长度也有上限，但这里我们关心的是
        // 在分配 byte[] 之前就被守卫拦截。无论命中守卫还是底层读取限制，都应是 JsonException。
        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<RecordSet>(json, options));
    }

    [TestMethod]
    public void RecordTableJsonConverter_WhenBase64PayloadTooLargeThenThrowsJsonException()
    {
        var json = BuildOversizeBase64Json();

        var options = new JsonSerializerOptions { MaxDepth = 64 };
        options.Converters.Add(new RecordTableJsonConverter());

        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<RecordTable>(json, options));
    }

    private static string BuildOversizeBase64Json()
    {
        // 构造一个超过 MaxBase64Length 的字符串（内容无所谓，长度用 'A' 填充）。
        // MaxBase64Length 来自 16MB 压缩上限，约 22.4MB Base64 字符。
        const int oversize = 25 * 1024 * 1024;
        var sb = new StringBuilder(oversize + 2);
        sb.Append('"');
        sb.Append('A', oversize);
        sb.Append('"');
        return sb.ToString();
    }

    [TestMethod]
    public void RecordBinaryPayloadCodec_WhenNoCompressionThenRoundTripSucceeds()
    {
        var codec = new RecordBinaryPayloadCodec { Compression = RecordPayloadCompression.None };
        var original = new byte[] { 1, 2, 3, 4, 5 };
        var encoded = codec.Encode(original);
        var decoded = codec.Decode(encoded);
        CollectionAssert.AreEqual(original, decoded);
    }

    [TestMethod]
    public void RecordBinaryPayloadCodec_WhenGZipThenRoundTripSucceeds()
    {
        var codec = new RecordBinaryPayloadCodec { Compression = RecordPayloadCompression.GZip };
        var original = new byte[] { 1, 2, 3, 4, 5 };
        var encoded = codec.Encode(original);
        var decoded = codec.Decode(encoded);
        CollectionAssert.AreEqual(original, decoded);
    }

    [TestMethod]
    public void RecordTableJsonConverter_WhenCustomNoCompressionCodecThenRoundTripSucceeds()
    {
        var codec = new RecordBinaryPayloadCodec { Compression = RecordPayloadCompression.None };
        var original = CreateTestRecordTable();
        var options = new JsonSerializerOptions();
        options.Converters.Add(new RecordTableJsonConverter(codec));

        var json = JsonSerializer.Serialize(original, options);
        var deserialized = JsonSerializer.Deserialize<RecordTable>(json, options);

        Assert.IsNotNull(deserialized);
        Assert.AreEqual("Orders", deserialized.Name);
        Assert.AreEqual(2, deserialized.Count);
    }

    [TestMethod]
    public void RecordSetJsonConverter_WhenCustomNoCompressionCodecThenRoundTripSucceeds()
    {
        var codec = new RecordBinaryPayloadCodec { Compression = RecordPayloadCompression.None };
        var original = CreateTestRecordSet();
        var options = new JsonSerializerOptions();
        options.Converters.Add(new RecordSetJsonConverter(codec));

        var json = JsonSerializer.Serialize(original, options);
        var deserialized = JsonSerializer.Deserialize<RecordSet>(json, options);

        Assert.IsNotNull(deserialized);
        Assert.AreEqual(2, deserialized.Count);
    }

    [TestMethod]
    public void RecordBinaryPayloadCodec_WhenCustomDoSLimitExceededThenThrowsInvalidDataException()
    {
        var codec = new RecordBinaryPayloadCodec
        {
            Compression = RecordPayloadCompression.GZip,
            MaxDecompressedBytes = 100
        };

        // 构造能解压出 >100 字节的 GZip 数据
        byte[] encoded;
        using (var ms = new MemoryStream())
        {
            ms.WriteByte(0xFE); ms.WriteByte((byte)'L'); ms.WriteByte((byte)'Y'); ms.WriteByte((byte)'Z');
            ms.WriteByte(0x01); ms.WriteByte(0x01); // version=1, algo=GZip
            using (var gz = new GZipStream(ms, CompressionLevel.Fastest, leaveOpen: true))
            {
                var chunk = new byte[200];
                gz.Write(chunk, 0, chunk.Length);
            }
            encoded = ms.ToArray();
        }

        Assert.Throws<InvalidDataException>(() => codec.Decode(encoded));
    }



    private static RecordTable CreateTestRecordTable()
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

    private static RecordSet CreateTestRecordSet()
    {
        var set = new RecordSet();

        var orders = new RecordTable("Orders", 1);
        var orderId = orders.Columns.Add<int>("Id");
        var orderRow = orders.AddRow();
        orderId.SetValue(orderRow.Row, 1);
        set.Add("Orders", orders);

        var products = new RecordTable("Products", 1);
        var productId = products.Columns.Add<int>("Id");
        var productRow = products.AddRow();
        productId.SetValue(productRow.Row, 100);
        set.Add("Products", products);

        return set;
    }

    #endregion
}
