using System;
using System.Linq;

namespace LuYao.Data;

/// <summary>
/// 测试 RecordTable 对数组类型的支持。
/// </summary>
[TestClass]
public class RecordArrayTypeTests
{
    #region 基础功能测试

    [TestMethod]
    public void WhenAddOneDimensionalArrayColumnThenArrayRankIsOne()
    {
        var table = new RecordTable("Test");
        var col = table.Columns.Add<int[]>("Scores");

        Assert.AreEqual(1, col.ArrayRank);
        Assert.AreEqual(typeof(int[]), col.Type);
        Assert.AreEqual(RecordColumnType.Int32, col.ColumnType);
    }

    [TestMethod]
    public void WhenAddTwoDimensionalArrayColumnThenArrayRankIsTwo()
    {
        var table = new RecordTable("Test");
        var col = table.Columns.Add<int[,]>("Matrix");

        Assert.AreEqual(2, col.ArrayRank);
        Assert.AreEqual(typeof(int[,]), col.Type);
        Assert.AreEqual(RecordColumnType.Int32, col.ColumnType);
    }

    [TestMethod]
    public void WhenAddThreeDimensionalArrayColumnThenArrayRankIsThree()
    {
        var table = new RecordTable("Test");
        var col = table.Columns.Add<int[,,]>("Cube");

        Assert.AreEqual(3, col.ArrayRank);
        Assert.AreEqual(typeof(int[,,]), col.Type);
        Assert.AreEqual(RecordColumnType.Int32, col.ColumnType);
    }

    [TestMethod]
    public void WhenAddNonArrayColumnThenArrayRankIsZero()
    {
        var table = new RecordTable("Test");
        var col = table.Columns.Add<int>("Value");

        Assert.AreEqual(0, col.ArrayRank);
        Assert.AreEqual(typeof(int), col.Type);
    }

    #endregion

    #region 一维数组数据操作

    [TestMethod]
    public void WhenSetAndGetIntArrayThenValuesPreserved()
    {
        var table = new RecordTable("Test");
        table.Columns.Add<int[]>("Scores");

        var row = table.AddRow();
        row["Scores"] = new[] { 85, 90, 88, 92 };

        var retrieved = row.To<int[]>("Scores");
        Assert.IsNotNull(retrieved);
        Assert.AreEqual(4, retrieved.Length);
        Assert.AreEqual(85, retrieved[0]);
        Assert.AreEqual(90, retrieved[1]);
        Assert.AreEqual(88, retrieved[2]);
        Assert.AreEqual(92, retrieved[3]);
    }

    [TestMethod]
    public void WhenSetAndGetStringArrayThenValuesPreserved()
    {
        var table = new RecordTable("Test");
        table.Columns.Add<string[]>("Tags");

        var row = table.AddRow();
        row["Tags"] = new[] { "VIP", "Premium", "Active" };

        var retrieved = row.To<string[]>("Tags");
        Assert.IsNotNull(retrieved);
        Assert.AreEqual(3, retrieved.Length);
        Assert.AreEqual("VIP", retrieved[0]);
        Assert.AreEqual("Premium", retrieved[1]);
        Assert.AreEqual("Active", retrieved[2]);
    }

    [TestMethod]
    public void WhenSetNullArrayThenGetReturnsNull()
    {
        var table = new RecordTable("Test");
        table.Columns.Add<int[]>("Scores");

        var row = table.AddRow();
        row["Scores"] = null;

        var retrieved = row.To<int[]>("Scores");
        Assert.IsNull(retrieved);
    }

    [TestMethod]
    public void WhenSetEmptyArrayThenGetReturnsEmptyArray()
    {
        var table = new RecordTable("Test");
        table.Columns.Add<int[]>("Scores");

        var row = table.AddRow();
        row["Scores"] = new int[0];

        var retrieved = row.To<int[]>("Scores");
        Assert.IsNotNull(retrieved);
        Assert.AreEqual(0, retrieved.Length);
    }

    #endregion

    #region 多维数组数据操作

    [TestMethod]
    public void WhenSetAndGetTwoDimensionalArrayThenValuesPreserved()
    {
        var table = new RecordTable("Test");
        table.Columns.Add<decimal[,]>("PriceMatrix");

        var row = table.AddRow();
        var matrix = new decimal[,] { { 1.1m, 2.2m }, { 3.3m, 4.4m } };
        row["PriceMatrix"] = matrix;

        var retrieved = row.To<decimal[,]>("PriceMatrix");
        Assert.IsNotNull(retrieved);
        Assert.AreEqual(2, retrieved.GetLength(0));
        Assert.AreEqual(2, retrieved.GetLength(1));
        Assert.AreEqual(1.1m, retrieved[0, 0]);
        Assert.AreEqual(2.2m, retrieved[0, 1]);
        Assert.AreEqual(3.3m, retrieved[1, 0]);
        Assert.AreEqual(4.4m, retrieved[1, 1]);
    }

    [TestMethod]
    public void WhenSetAndGetThreeDimensionalArrayThenValuesPreserved()
    {
        var table = new RecordTable("Test");
        table.Columns.Add<int[,,]>("Cube");

        var row = table.AddRow();
        var cube = new int[2, 2, 2]
        {
            { { 1, 2 }, { 3, 4 } },
            { { 5, 6 }, { 7, 8 } }
        };
        row["Cube"] = cube;

        var retrieved = row.To<int[,,]>("Cube");
        Assert.IsNotNull(retrieved);
        Assert.AreEqual(2, retrieved.GetLength(0));
        Assert.AreEqual(2, retrieved.GetLength(1));
        Assert.AreEqual(2, retrieved.GetLength(2));
        Assert.AreEqual(1, retrieved[0, 0, 0]);
        Assert.AreEqual(8, retrieved[1, 1, 1]);
    }

    #endregion

    #region 数组元素可空性

    [TestMethod]
    public void WhenSetNullableIntArrayWithNullElementsThenPreserved()
    {
        var table = new RecordTable("Test");
        table.Columns.Add<int?[]>("NullableScores");

        var row = table.AddRow();
        row["NullableScores"] = new int?[] { 85, null, 88, null, 92 };

        var retrieved = row.To<int?[]>("NullableScores");
        Assert.IsNotNull(retrieved);
        Assert.AreEqual(5, retrieved.Length);
        Assert.AreEqual(85, retrieved[0]);
        Assert.IsNull(retrieved[1]);
        Assert.AreEqual(88, retrieved[2]);
        Assert.IsNull(retrieved[3]);
        Assert.AreEqual(92, retrieved[4]);
    }

    [TestMethod]
    public void WhenSetStringArrayWithNullElementsThenPreserved()
    {
        var table = new RecordTable("Test");
        table.Columns.Add<string[]>("Tags");

        var row = table.AddRow();
        row["Tags"] = new string[] { "VIP", null, "Active" };

        var retrieved = row.To<string[]>("Tags");
        Assert.IsNotNull(retrieved);
        Assert.AreEqual(3, retrieved.Length);
        Assert.AreEqual("VIP", retrieved[0]);
        Assert.IsNull(retrieved[1]);
        Assert.AreEqual("Active", retrieved[2]);
    }

    #endregion

    #region 二进制序列化测试

    [TestMethod]
    public void WhenSerializeOneDimensionalArrayThenRoundTripSucceeds()
    {
        var original = new RecordTable("Test");
        original.Columns.Add<int[]>("Scores");
        original.Columns.Add<string[]>("Tags");

        var row = original.AddRow();
        row["Scores"] = new[] { 85, 90, 88, 92 };
        row["Tags"] = new[] { "VIP", "Premium" };

        var bytes = original.ToBytes();
        var deserialized = RecordTable.FromBytes(bytes);

        Assert.AreEqual(1, deserialized.Count);
        Assert.AreEqual(2, deserialized.Columns.Count);

        var scoresCol = deserialized.Columns.Find<int[]>("Scores");
        Assert.IsNotNull(scoresCol);
        Assert.AreEqual(1, scoresCol.ArrayRank);

        var scores = deserialized[0].To<int[]>("Scores");
        Assert.IsNotNull(scores);
        CollectionAssert.AreEqual(new[] { 85, 90, 88, 92 }, scores);

        var tags = deserialized[0].To<string[]>("Tags");
        Assert.IsNotNull(tags);
        CollectionAssert.AreEqual(new[] { "VIP", "Premium" }, tags);
    }

    [TestMethod]
    public void WhenSerializeTwoDimensionalArrayThenRoundTripSucceeds()
    {
        var original = new RecordTable("Test");
        original.Columns.Add<int[,]>("Matrix");

        var row = original.AddRow();
        var matrix = new int[,] { { 1, 2, 3 }, { 4, 5, 6 } };
        row["Matrix"] = matrix;

        var bytes = original.ToBytes();
        var deserialized = RecordTable.FromBytes(bytes);

        var retrieved = deserialized[0].To<int[,]>("Matrix");
        Assert.IsNotNull(retrieved);
        Assert.AreEqual(2, retrieved.GetLength(0));
        Assert.AreEqual(3, retrieved.GetLength(1));
        Assert.AreEqual(1, retrieved[0, 0]);
        Assert.AreEqual(6, retrieved[1, 2]);
    }

    [TestMethod]
    public void WhenSerializeArrayWithNullElementsThenRoundTripSucceeds()
    {
        var original = new RecordTable("Test");
        original.Columns.Add<int?[]>("NullableScores");
        original.Columns.Add<string[]>("Tags");

        var row = original.AddRow();
        row["NullableScores"] = new int?[] { 85, null, 88 };
        row["Tags"] = new string[] { "VIP", null, "Active" };

        var bytes = original.ToBytes();
        var deserialized = RecordTable.FromBytes(bytes);

        // 验证可空元素数组
        var scoresObj = deserialized[0]["NullableScores"];
        Assert.IsNotNull(scoresObj);
        Assert.IsInstanceOfType(scoresObj, typeof(int?[]));
        var scores = (int?[])scoresObj;
        Assert.AreEqual(3, scores.Length);
        Assert.AreEqual(85, scores[0]);
        Assert.IsNull(scores[1]);
        Assert.AreEqual(88, scores[2]);

        // 验证引用类型元素数组
        var tags = deserialized[0].To<string[]>("Tags");
        Assert.IsNotNull(tags);
        Assert.AreEqual(3, tags.Length);
        Assert.AreEqual("VIP", tags[0]);
        Assert.IsNull(tags[1]);
        Assert.AreEqual("Active", tags[2]);
    }

    [TestMethod]
    public void WhenSerializeNullArrayThenRoundTripSucceeds()
    {
        var original = new RecordTable("Test");
        original.Columns.Add<int[]>("Scores");

        var row = original.AddRow();
        row["Scores"] = null;

        var bytes = original.ToBytes();
        var deserialized = RecordTable.FromBytes(bytes);

        var scores = deserialized[0].To<int[]>("Scores");
        Assert.IsNull(scores);
    }

    [TestMethod]
    public void WhenSerializeEmptyArrayThenRoundTripSucceeds()
    {
        var original = new RecordTable("Test");
        original.Columns.Add<int[]>("Scores");

        var row = original.AddRow();
        row["Scores"] = new int[0];

        var bytes = original.ToBytes();
        var deserialized = RecordTable.FromBytes(bytes);

        var scores = deserialized[0].To<int[]>("Scores");
        Assert.IsNotNull(scores);
        Assert.AreEqual(0, scores.Length);
    }

    [TestMethod]
    public void WhenSerializeMultipleRowsWithArraysThenAllRowsPreserved()
    {
        var original = new RecordTable("Test");
        original.Columns.Add<int>("Id");
        original.Columns.Add<int[]>("Scores");

        for (int i = 1; i <= 3; i++)
        {
            var row = original.AddRow();
            row["Id"] = i;
            row["Scores"] = new[] { i * 10, i * 20, i * 30 };
        }

        var bytes = original.ToBytes();
        var deserialized = RecordTable.FromBytes(bytes);

        Assert.AreEqual(3, deserialized.Count);

        for (int i = 0; i < 3; i++)
        {
            var id = deserialized[i].To<int>("Id");
            var scores = deserialized[i].To<int[]>("Scores");

            Assert.AreEqual(i + 1, id);
            Assert.IsNotNull(scores);
            Assert.AreEqual(3, scores.Length);
            Assert.AreEqual((i + 1) * 10, scores[0]);
            Assert.AreEqual((i + 1) * 20, scores[1]);
            Assert.AreEqual((i + 1) * 30, scores[2]);
        }
    }

    #endregion

    #region ToString JSON 格式测试

    [TestMethod]
    public void WhenToStringOnOneDimensionalIntArrayThenReturnsJsonFormat()
    {
        var table = new RecordTable("Test");
        table.Columns.Add<int[]>("Scores");

        var row = table.AddRow();
        row["Scores"] = new[] { 85, 90, 88 };

        var str = row.ToString("Scores");
        Assert.AreEqual("[85, 90, 88]", str);
    }

    [TestMethod]
    public void WhenToStringOnOneDimensionalStringArrayThenReturnsJsonFormat()
    {
        var table = new RecordTable("Test");
        table.Columns.Add<string[]>("Tags");

        var row = table.AddRow();
        row["Tags"] = new[] { "VIP", "Premium" };

        var str = row.ToString("Tags");
        Assert.AreEqual("[\"VIP\", \"Premium\"]", str);
    }

    [TestMethod]
    public void WhenToStringOnTwoDimensionalArrayThenReturnsJsonFormat()
    {
        var table = new RecordTable("Test");
        table.Columns.Add<int[,]>("Matrix");

        var row = table.AddRow();
        row["Matrix"] = new int[,] { { 1, 2 }, { 3, 4 } };

        var str = row.ToString("Matrix");
        Assert.AreEqual("[[1, 2], [3, 4]]", str);
    }

    [TestMethod]
    public void WhenToStringOnEmptyArrayThenReturnsEmptyJsonArray()
    {
        var table = new RecordTable("Test");
        table.Columns.Add<int[]>("Scores");

        var row = table.AddRow();
        row["Scores"] = new int[0];

        var str = row.ToString("Scores");
        Assert.AreEqual("[]", str);
    }

    [TestMethod]
    public void WhenToStringOnNullArrayThenReturnsEmptyString()
    {
        var table = new RecordTable("Test");
        table.Columns.Add<int[]>("Scores");

        var row = table.AddRow();
        row["Scores"] = null;

        var str = row.ToString("Scores");
        Assert.AreEqual("", str);
    }

    [TestMethod]
    public void WhenToStringOnArrayWithNullElementsThenReturnsJsonWithNull()
    {
        var table = new RecordTable("Test");
        table.Columns.Add<string[]>("Tags");

        var row = table.AddRow();
        row["Tags"] = new string[] { "VIP", null, "Active" };

        var str = row.ToString("Tags");
        Assert.AreEqual("[\"VIP\", null, \"Active\"]", str);
    }

    [TestMethod]
    public void WhenToStringOnStringArrayWithSpecialCharactersThenEscaped()
    {
        var table = new RecordTable("Test");
        table.Columns.Add<string[]>("Messages");

        var row = table.AddRow();
        row["Messages"] = new[] { "Hello \"World\"", "Line1\nLine2", "Tab\tSeparated" };

        var str = row.ToString("Messages");
        Assert.AreEqual("[\"Hello \\\"World\\\"\", \"Line1\\nLine2\", \"Tab\\tSeparated\"]", str);
    }

    #endregion

    #region RecordSchema 测试

    [TestMethod]
    public void WhenGetSchemaWithArrayColumnsThenArrayRankIncluded()
    {
        var table = new RecordTable("Test");
        table.Columns.Add<int>("Id");
        table.Columns.Add<int[]>("Scores");
        table.Columns.Add<string[,]>("Matrix");

        var schema = table.GetSchema();

        Assert.AreEqual(3, schema.Columns.Count);

        var idCol = schema.Columns[0];
        Assert.AreEqual("Id", idCol.Name);
        Assert.AreEqual(0, idCol.ArrayRank);
        Assert.AreEqual(typeof(int), idCol.Type);

        var scoresCol = schema.Columns[1];
        Assert.AreEqual("Scores", scoresCol.Name);
        Assert.AreEqual(1, scoresCol.ArrayRank);
        Assert.AreEqual(typeof(int[]), scoresCol.Type);

        var matrixCol = schema.Columns[2];
        Assert.AreEqual("Matrix", matrixCol.Name);
        Assert.AreEqual(2, matrixCol.ArrayRank);
        Assert.AreEqual(typeof(string[,]), matrixCol.Type);
    }

    #endregion

    #region 各种元素类型测试

    [TestMethod]
    public void WhenUseBoolArrayThenSerializationWorks()
    {
        var table = new RecordTable("Test");
        table.Columns.Add<bool[]>("Flags");

        var row = table.AddRow();
        row["Flags"] = new[] { true, false, true };

        var bytes = table.ToBytes();
        var deserialized = RecordTable.FromBytes(bytes);

        var flags = deserialized[0].To<bool[]>("Flags");
        CollectionAssert.AreEqual(new[] { true, false, true }, flags);
    }

    [TestMethod]
    public void WhenUseLongArrayThenSerializationWorks()
    {
        var table = new RecordTable("Test");
        table.Columns.Add<long[]>("Ids");

        var row = table.AddRow();
        row["Ids"] = new[] { 1000000000L, 2000000000L, 3000000000L };

        var bytes = table.ToBytes();
        var deserialized = RecordTable.FromBytes(bytes);

        var ids = deserialized[0].To<long[]>("Ids");
        CollectionAssert.AreEqual(new[] { 1000000000L, 2000000000L, 3000000000L }, ids);
    }

    [TestMethod]
    public void WhenUseDoubleArrayThenSerializationWorks()
    {
        var table = new RecordTable("Test");
        table.Columns.Add<double[]>("Values");

        var row = table.AddRow();
        row["Values"] = new[] { 1.1, 2.2, 3.3 };

        var bytes = table.ToBytes();
        var deserialized = RecordTable.FromBytes(bytes);

        var values = deserialized[0].To<double[]>("Values");
        Assert.AreEqual(3, values.Length);
        Assert.AreEqual(1.1, values[0], 0.0001);
        Assert.AreEqual(2.2, values[1], 0.0001);
        Assert.AreEqual(3.3, values[2], 0.0001);
    }

    [TestMethod]
    public void WhenUseDateTimeArrayThenSerializationWorks()
    {
        var table = new RecordTable("Test");
        table.Columns.Add<DateTime[]>("Dates");

        var row = table.AddRow();
        var dates = new[] { new DateTime(2024, 1, 1), new DateTime(2024, 6, 15), new DateTime(2024, 12, 31) };
        row["Dates"] = dates;

        var bytes = table.ToBytes();
        var deserialized = RecordTable.FromBytes(bytes);

        var retrievedDates = deserialized[0].To<DateTime[]>("Dates");
        Assert.AreEqual(3, retrievedDates.Length);
        Assert.AreEqual(new DateTime(2024, 1, 1), retrievedDates[0]);
        Assert.AreEqual(new DateTime(2024, 6, 15), retrievedDates[1]);
        Assert.AreEqual(new DateTime(2024, 12, 31), retrievedDates[2]);
    }

    [TestMethod]
    public void WhenUseGuidArrayThenSerializationWorks()
    {
        var table = new RecordTable("Test");
        table.Columns.Add<Guid[]>("Ids");

        var row = table.AddRow();
        var guids = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
        row["Ids"] = guids;

        var bytes = table.ToBytes();
        var deserialized = RecordTable.FromBytes(bytes);

        var retrievedGuids = deserialized[0].To<Guid[]>("Ids");
        CollectionAssert.AreEqual(guids, retrievedGuids);
    }

    #endregion

    #region 删除操作测试

    [TestMethod]
    public void WhenDeleteRowWithArrayColumnThenDataShiftsCorrectly()
    {
        var table = new RecordTable("Test");
        table.Columns.Add<int>("Id");
        table.Columns.Add<int[]>("Scores");

        for (int i = 1; i <= 3; i++)
        {
            var row = table.AddRow();
            row["Id"] = i;
            row["Scores"] = new[] { i * 10, i * 20 };
        }

        table.Delete(1); // 删除第二行

        Assert.AreEqual(2, table.Count);
        Assert.AreEqual(1, table[0].To<int>("Id"));
        CollectionAssert.AreEqual(new[] { 10, 20 }, table[0].To<int[]>("Scores"));
        Assert.AreEqual(3, table[1].To<int>("Id"));
        CollectionAssert.AreEqual(new[] { 30, 60 }, table[1].To<int[]>("Scores"));
    }

    #endregion

    #region Clone 操作测试

    [TestMethod]
    public void WhenCloneRecordWithArrayColumnsThenDataCopied()
    {
        var original = new RecordTable("Test");
        original.Columns.Add<int>("Id");
        original.Columns.Add<int[]>("Scores");

        var row = original.AddRow();
        row["Id"] = 1;
        row["Scores"] = new[] { 85, 90, 88 };

        var clone = original.Clone();

        Assert.AreEqual(1, clone.Count);
        Assert.AreEqual(2, clone.Columns.Count);
        Assert.AreEqual(1, clone.Columns.Find<int[]>("Scores")!.ArrayRank);

        var scores = clone[0].To<int[]>("Scores");
        CollectionAssert.AreEqual(new[] { 85, 90, 88 }, scores);

        // 修改克隆不影响原始
        var cloneRow = clone[0];
        cloneRow["Scores"] = new[] { 100, 100 };
        var originalScores = original[0].To<int[]>("Scores");
        CollectionAssert.AreEqual(new[] { 85, 90, 88 }, originalScores);
    }

    [TestMethod]
    public void WhenCloneSchemaWithArrayColumnsThenStructureCopied()
    {
        var original = new RecordTable("Test");
        original.Columns.Add<int>("Id");
        original.Columns.Add<int[]>("Scores");
        original.Columns.Add<string[,]>("Matrix");

        var clone = original.CloneSchema();

        Assert.AreEqual(0, clone.Count);
        Assert.AreEqual(3, clone.Columns.Count);

        Assert.AreEqual(0, clone.Columns[0].ArrayRank);
        Assert.AreEqual(1, clone.Columns[1].ArrayRank);
        Assert.AreEqual(2, clone.Columns[2].ArrayRank);
    }

    #endregion
}
