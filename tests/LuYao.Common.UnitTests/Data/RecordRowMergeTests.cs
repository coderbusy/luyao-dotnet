using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LuYao.Data;

/// <summary>
/// 测试 <see cref="RecordRow.Merge(RecordRow)"/> 和 <see cref="RecordRow.Merge{T}(T)"/> 方法。
/// </summary>
[TestClass]
public class RecordRowMergeTests
{
    // ------------------------------------------------------------------
    // Merge(RecordRow other)
    // ------------------------------------------------------------------

    /// <summary>
    /// 同名列的值应被覆盖。
    /// </summary>
    [TestMethod]
    public void Merge_Row_OverwritesExistingColumn()
    {
        // Arrange
        var r1 = new RecordTable();
        r1.Columns.Add<string>("Name");
        var row1 = r1.AddRow();
        row1["Name"] = "Alice";

        var r2 = new RecordTable();
        r2.Columns.Add<string>("Name");
        var row2 = r2.AddRow();
        row2["Name"] = "Bob";

        // Act
        row1.Merge(row2);

        // Assert
        Assert.AreEqual("Bob", row1["Name"]);
    }

    /// <summary>
    /// r2 中存在而 r1 中不存在的列应被追加。
    /// </summary>
    [TestMethod]
    public void Merge_Row_AppendsNewColumn()
    {
        // Arrange
        var r1 = new RecordTable();
        r1.Columns.Add<string>("Name");
        var row1 = r1.AddRow();
        row1["Name"] = "Alice";

        var r2 = new RecordTable();
        r2.Columns.Add<int>("Age");
        var row2 = r2.AddRow();
        row2["Age"] = 30;

        // Act
        row1.Merge(row2);

        // Assert
        Assert.AreEqual(30, row1["Age"]);
        Assert.AreEqual("Alice", row1["Name"]);
    }

    /// <summary>
    /// r2 既有同名列也有新列时，应同时覆盖和追加。
    /// </summary>
    [TestMethod]
    public void Merge_Row_OverwritesAndAppends()
    {
        // Arrange
        var r1 = new RecordTable();
        r1.Columns.Add<string>("Name");
        r1.Columns.Add<int>("Score");
        var row1 = r1.AddRow();
        row1["Name"] = "Alice";
        row1["Score"] = 80;

        var r2 = new RecordTable();
        r2.Columns.Add<string>("Name");
        r2.Columns.Add<string>("City");
        var row2 = r2.AddRow();
        row2["Name"] = "Bob";
        row2["City"] = "Beijing";

        // Act
        row1.Merge(row2);

        // Assert
        Assert.AreEqual("Bob", row1["Name"]);
        Assert.AreEqual(80, row1["Score"]);
        Assert.AreEqual("Beijing", row1["City"]);
    }

    /// <summary>
    /// r2 为空列时，r1 不应发生任何变化。
    /// </summary>
    [TestMethod]
    public void Merge_Row_EmptySource_NoChange()
    {
        // Arrange
        var r1 = new RecordTable();
        r1.Columns.Add<string>("Name");
        var row1 = r1.AddRow();
        row1["Name"] = "Alice";

        var r2 = new RecordTable();
        var row2 = r2.AddRow();

        // Act
        row1.Merge(row2);

        // Assert
        Assert.AreEqual("Alice", row1["Name"]);
        Assert.AreEqual(1, r1.Columns.Count);
    }

    // ------------------------------------------------------------------
    // Merge<T>(T model)
    // ------------------------------------------------------------------

    private class PersonModel
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
    }

    /// <summary>
    /// 同名列的值应被模型属性值覆盖。
    /// </summary>
    [TestMethod]
    public void Merge_Model_OverwritesExistingColumn()
    {
        // Arrange
        var r1 = new RecordTable();
        r1.Columns.Add<string>("Name");
        var row1 = r1.AddRow();
        row1["Name"] = "Alice";

        // Act
        row1.Merge(new PersonModel { Name = "Bob", Age = 25 });

        // Assert
        Assert.AreEqual("Bob", row1["Name"]);
    }

    /// <summary>
    /// 模型中存在而行中不存在的属性对应列应被追加。
    /// </summary>
    [TestMethod]
    public void Merge_Model_AppendsNewColumn()
    {
        // Arrange
        var r1 = new RecordTable();
        r1.Columns.Add<string>("Name");
        var row1 = r1.AddRow();
        row1["Name"] = "Alice";

        // Act
        row1.Merge(new PersonModel { Name = "Alice", Age = 25 });

        // Assert
        Assert.AreEqual(25, row1["Age"]);
    }

    /// <summary>
    /// 传入 null 时应抛出 <see cref="System.ArgumentNullException"/>。
    /// </summary>
    [TestMethod]
    public void Merge_Model_NullModel_Throws()
    {
        // Arrange
        var r1 = new RecordTable();
        var row1 = r1.AddRow();

        // Act & Assert
        Assert.Throws<System.ArgumentNullException>(() => row1.Merge<PersonModel>(null!));
    }
}
