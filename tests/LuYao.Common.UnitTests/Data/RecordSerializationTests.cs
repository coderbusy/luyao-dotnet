using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Serialization;

namespace LuYao.Data;

[TestClass]
public class RecordSerializationTests
{
    private static Record CreateTestRecord()
    {
        var record = new Record("Orders", 2);
        var idCol = record.Columns.Add<int>("Id");
        var nameCol = record.Columns.Add<string>("Name");
        record.Page = 2;
        record.PageSize = 10;
        record.MaxCount = 50;

        var row1 = record.AddRow();
        idCol.Set(1, row1.Row);
        nameCol.Set("Order-1", row1.Row);

        var row2 = record.AddRow();
        idCol.Set(2, row2.Row);
        nameCol.Set("Order-2", row2.Row);

        return record;
    }

    #region XML Serialization

    [TestMethod]
    public void WhenXmlRoundTripThenDataPreserved()
    {
        var original = CreateTestRecord();

        var serializer = new XmlSerializer(typeof(Record));
        using var sw = new StringWriter();
        serializer.Serialize(sw, original);
        var xml = sw.ToString();

        using var sr = new StringReader(xml);
        var deserialized = (Record)serializer.Deserialize(sr)!;

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
    public void WhenXmlRoundTripEmptyRecordThenSchemaPreserved()
    {
        var original = new Record("Empty", 0);
        original.Columns.Add<int>("Id");
        original.Columns.Add<string>("Name");

        var serializer = new XmlSerializer(typeof(Record));
        using var sw = new StringWriter();
        serializer.Serialize(sw, original);

        using var sr = new StringReader(sw.ToString());
        var deserialized = (Record)serializer.Deserialize(sr)!;

        Assert.AreEqual("Empty", deserialized.Name);
        Assert.AreEqual(0, deserialized.Count);
        Assert.AreEqual(2, deserialized.Columns.Count);
        Assert.AreEqual("Id", deserialized.Columns[0].Name);
        Assert.AreEqual(typeof(int), deserialized.Columns[0].Type);
    }

    [TestMethod]
    public void WhenXmlRoundTripWithNullValuesThenPreserved()
    {
        var record = new Record("Test", 1);
        record.Columns.Add<string>("Name");
        record.Columns.Add<int?>("Value");
        record.AddRow();
        // leave values as null/default

        var serializer = new XmlSerializer(typeof(Record));
        using var sw = new StringWriter();
        serializer.Serialize(sw, record);

        using var sr = new StringReader(sw.ToString());
        var deserialized = (Record)serializer.Deserialize(sr)!;

        Assert.AreEqual(1, deserialized.Count);
        Assert.IsNull(deserialized.Columns[0].GetValue(0));
        Assert.IsNull(deserialized.Columns[1].GetValue(0));
    }

    [TestMethod]
    public void WhenXmlRoundTripWithByteArrayThenPreserved()
    {
        var record = new Record("Binary", 1);
        var col = record.Columns.Add<byte[]>("Data");
        var row = record.AddRow();
        col.Set(new byte[] { 1, 2, 3, 4, 5 }, row.Row);

        var serializer = new XmlSerializer(typeof(Record));
        using var sw = new StringWriter();
        serializer.Serialize(sw, record);

        using var sr = new StringReader(sw.ToString());
        var deserialized = (Record)serializer.Deserialize(sr)!;

        var data = deserialized.Columns.Find<byte[]>("Data")!.Get(0);
        CollectionAssert.AreEqual(new byte[] { 1, 2, 3, 4, 5 }, data);
    }

    #endregion
}

[TestClass]
public class RecordSetSerializationTests
{
    #region XML Serialization

    [TestMethod]
    public void WhenXmlRoundTripThenAllRecordsPreserved()
    {
        var set = new RecordSet();

        var orders = new Record("Orders", 1);
        orders.Columns.Add<int>("Id");
        var row = orders.AddRow();
        orders.Columns[0].SetValue(1, row.Row);
        set.Add("Orders", orders);

        var customers = new Record("Customers", 1);
        customers.Columns.Add<string>("Name");
        var row2 = customers.AddRow();
        customers.Columns[0].SetValue("Alice", row2.Row);
        set.Add("Customers", customers);

        var serializer = new XmlSerializer(typeof(RecordSet));
        using var sw = new StringWriter();
        serializer.Serialize(sw, set);

        using var sr = new StringReader(sw.ToString());
        var deserialized = (RecordSet)serializer.Deserialize(sr)!;

        Assert.AreEqual(2, deserialized.Count);
        Assert.IsTrue(deserialized.Contains("Orders"));
        Assert.IsTrue(deserialized.Contains("Customers"));
        Assert.AreEqual(1, deserialized["Orders"].Count);
        Assert.AreEqual(1, deserialized["Customers"].Count);
    }

    [TestMethod]
    public void WhenXmlRoundTripEmptySetThenEmpty()
    {
        var set = new RecordSet();

        var serializer = new XmlSerializer(typeof(RecordSet));
        using var sw = new StringWriter();
        serializer.Serialize(sw, set);

        using var sr = new StringReader(sw.ToString());
        var deserialized = (RecordSet)serializer.Deserialize(sr)!;

        Assert.AreEqual(0, deserialized.Count);
    }

    #endregion
}
