using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Data;

[TestClass]
public class DelimiterBasedStringConverterTests
{
    public class TestItem
    {
        public string Str { get; set; }
        public int Int32 { get; set; }
        public bool Boolean { get; set; }
    }

    [TestMethod]
    public void Serialize_CommonTypes_ShouldSerializeCorrectly()
    {
        var converter = new DelimiterBasedStringConverter<TestItem>("|");
        converter.Add(x => x.Str);
        converter.Add(x => x.Int32);
        converter.Add(x => x.Boolean);
        var item = new TestItem
        {
            Str = "Hello",
            Int32 = 123,
            Boolean = true
        };
        var serialized = converter.Serialize(item);

        Assert.AreEqual("Hello|123|1", serialized);
    }

    [TestMethod]
    public void Serialize_NullItem_ShouldReturnEmptyString()
    {
        var converter = new DelimiterBasedStringConverter<TestItem>("|");
        converter.Add(x => x.Str);
        converter.Add(x => x.Int32);
        converter.Add(x => x.Boolean);
        var serialized = converter.Serialize(null);
        Assert.AreEqual(string.Empty, serialized);
    }

    [TestMethod]
    public void Deserialize_CommonTypes_ShouldDeserializeCorrectly()
    {
        var converter = new DelimiterBasedStringConverter<TestItem>("|");
        converter.Add(x => x.Str);
        converter.Add(x => x.Int32);
        converter.Add(x => x.Boolean);
        var serialized = "Hello|123|1";
        var deserialized = converter.Deserialize(serialized);

        Assert.IsNotNull(deserialized);
        Assert.AreEqual("Hello", deserialized.Str);
        Assert.AreEqual(123, deserialized.Int32);
        Assert.IsTrue(deserialized.Boolean);
    }
}
