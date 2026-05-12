using System;
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

    #region Helper Methods

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
