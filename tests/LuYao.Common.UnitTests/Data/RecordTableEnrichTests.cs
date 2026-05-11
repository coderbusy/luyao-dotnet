namespace LuYao.Data;

[TestClass]
public class RecordTableEnrichTests
{
    // ── Enrich(source, sharedColumn) ─────────────────────────────────────────

    [TestMethod]
    public void Enrich_WithSharedColumn_ShouldAddMissingColumns()
    {
        // Arrange
        var target = new RecordTable();
        var idCol = target.Columns.Add<int>("Id");
        var nameCol = target.Columns.Add<string>("Name");
        target.AddRow();
        target.AddRow();
        idCol.SetValue(0, 1);
        nameCol.SetValue(0, "Alice");
        idCol.SetValue(1, 2);
        nameCol.SetValue(1, "Bob");

        var source = new RecordTable();
        var srcIdCol = source.Columns.Add<int>("Id");
        var srcAgeCol = source.Columns.Add<int>("Age");
        var srcCityCol = source.Columns.Add<string>("City");
        source.AddRow();
        source.AddRow();
        srcIdCol.SetValue(0, 1);
        srcAgeCol.SetValue(0, 25);
        srcCityCol.SetValue(0, "Beijing");
        srcIdCol.SetValue(1, 2);
        srcAgeCol.SetValue(1, 30);
        srcCityCol.SetValue(1, "Shanghai");

        // Act
        target.Enrich(source, "Id");

        // Assert
        Assert.AreEqual(2, target.Count);
        Assert.AreEqual(4, target.Columns.Count);
        Assert.IsNotNull(target.Columns.Find("Age"));
        Assert.IsNotNull(target.Columns.Find("City"));
        Assert.AreEqual(25, target[0]["Age"]);
        Assert.AreEqual("Beijing", target[0]["City"]);
        Assert.AreEqual(30, target[1]["Age"]);
        Assert.AreEqual("Shanghai", target[1]["City"]);
    }

    [TestMethod]
    public void Enrich_WithSharedColumn_NoMatch_ShouldKeepRowUnchanged()
    {
        // Arrange
        var target = new RecordTable();
        var idCol = target.Columns.Add<int>("Id");
        var nameCol = target.Columns.Add<string>("Name");
        target.AddRow();
        idCol.SetValue(0, 99);
        nameCol.SetValue(0, "Unknown");

        var source = new RecordTable();
        var srcIdCol = source.Columns.Add<int>("Id");
        var srcAgeCol = source.Columns.Add<int>("Age");
        source.AddRow();
        srcIdCol.SetValue(0, 1);
        srcAgeCol.SetValue(0, 25);

        // Act
        target.Enrich(source, "Id");

        // Assert - 当没有任何匹配时，列不会被添加
        Assert.AreEqual(1, target.Count);
        Assert.AreEqual(2, target.Columns.Count);
        Assert.AreEqual(99, target[0]["Id"]);
        Assert.AreEqual("Unknown", target[0]["Name"]);
        Assert.IsNull(target.Columns.Find("Age"));
    }

    [TestMethod]
    public void Enrich_WithSharedColumn_NullSource_ShouldThrowArgumentNullException()
    {
        // Arrange
        var target = new RecordTable();
        target.Columns.Add<int>("Id");

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => target.Enrich(null!, "Id"));
    }

    [TestMethod]
    public void Enrich_WithSharedColumn_NullColumnName_ShouldThrowArgumentNullException()
    {
        // Arrange
        var target = new RecordTable();
        var source = new RecordTable();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => target.Enrich(source, null!));
    }

    [TestMethod]
    public void Enrich_WithSharedColumn_EmptyColumnName_ShouldThrowArgumentException()
    {
        // Arrange
        var target = new RecordTable();
        var source = new RecordTable();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => target.Enrich(source, ""));
    }

    [TestMethod]
    public void Enrich_WithSharedColumn_ColumnNotInTarget_ShouldThrowArgumentException()
    {
        // Arrange
        var target = new RecordTable();
        target.Columns.Add<int>("Id");

        var source = new RecordTable();
        source.Columns.Add<int>("Id");

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => target.Enrich(source, "NonExistent"));
        Assert.IsTrue(ex.Message.Contains("当前记录中不存在列"));
    }

    [TestMethod]
    public void Enrich_WithSharedColumn_ColumnNotInSource_ShouldThrowArgumentException()
    {
        // Arrange
        var target = new RecordTable();
        target.Columns.Add<int>("Id");

        var source = new RecordTable();
        source.Columns.Add<int>("OtherId");

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => target.Enrich(source, "Id"));
        Assert.IsTrue(ex.Message.Contains("来源记录中不存在列"));
    }

    // ── Enrich(source, selfColumn, sourceColumn) ─────────────────────────────

    [TestMethod]
    public void Enrich_WithDifferentColumns_ShouldAddMissingColumns()
    {
        // Arrange
        var target = new RecordTable();
        var userIdCol = target.Columns.Add<int>("UserId");
        var nameCol = target.Columns.Add<string>("Name");
        target.AddRow();
        userIdCol.SetValue(0, 101);
        nameCol.SetValue(0, "Alice");

        var source = new RecordTable();
        var empIdCol = source.Columns.Add<int>("EmployeeId");
        var deptCol = source.Columns.Add<string>("Department");
        source.AddRow();
        empIdCol.SetValue(0, 101);
        deptCol.SetValue(0, "Engineering");

        // Act
        target.Enrich(source, "UserId", "EmployeeId");

        // Assert
        Assert.AreEqual(1, target.Count);
        Assert.AreEqual(3, target.Columns.Count);
        Assert.IsNotNull(target.Columns.Find("Department"));
        Assert.AreEqual("Engineering", target[0]["Department"]);
    }

    [TestMethod]
    public void Enrich_WithDifferentColumns_NullSelfColumn_ShouldThrowArgumentNullException()
    {
        // Arrange
        var target = new RecordTable();
        var source = new RecordTable();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => target.Enrich(source, null!, "SourceCol"));
    }

    [TestMethod]
    public void Enrich_WithDifferentColumns_NullSourceColumn_ShouldThrowArgumentNullException()
    {
        // Arrange
        var target = new RecordTable();
        var source = new RecordTable();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => target.Enrich(source, "SelfCol", null!));
    }

    [TestMethod]
    public void Enrich_WithDifferentColumns_EmptySelfColumn_ShouldThrowArgumentException()
    {
        // Arrange
        var target = new RecordTable();
        var source = new RecordTable();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => target.Enrich(source, "", "SourceCol"));
    }

    [TestMethod]
    public void Enrich_WithDifferentColumns_EmptySourceColumn_ShouldThrowArgumentException()
    {
        // Arrange
        var target = new RecordTable();
        var source = new RecordTable();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => target.Enrich(source, "SelfCol", ""));
    }

    // ── Enrich(source, selfColumn, sourceColumn, columnsToEnrich) ────────────

    [TestMethod]
    public void Enrich_WithSpecificColumns_ShouldOnlyAddSpecifiedColumns()
    {
        // Arrange
        var target = new RecordTable();
        var idCol = target.Columns.Add<int>("Id");
        var nameCol = target.Columns.Add<string>("Name");
        target.AddRow();
        idCol.SetValue(0, 1);
        nameCol.SetValue(0, "Alice");

        var source = new RecordTable();
        var srcIdCol = source.Columns.Add<int>("Id");
        var srcAgeCol = source.Columns.Add<int>("Age");
        var srcCityCol = source.Columns.Add<string>("City");
        var srcCountryCol = source.Columns.Add<string>("Country");
        source.AddRow();
        srcIdCol.SetValue(0, 1);
        srcAgeCol.SetValue(0, 25);
        srcCityCol.SetValue(0, "Beijing");
        srcCountryCol.SetValue(0, "China");

        // Act
        target.Enrich(source, "Id", "Id", new[] { "Age", "City" });

        // Assert
        Assert.AreEqual(1, target.Count);
        Assert.AreEqual(4, target.Columns.Count);
        Assert.IsNotNull(target.Columns.Find("Age"));
        Assert.IsNotNull(target.Columns.Find("City"));
        Assert.IsNull(target.Columns.Find("Country"));
        Assert.AreEqual(25, target[0]["Age"]);
        Assert.AreEqual("Beijing", target[0]["City"]);
    }

    [TestMethod]
    public void Enrich_WithSpecificColumns_NullList_ShouldAddAllColumns()
    {
        // Arrange
        var target = new RecordTable();
        var idCol = target.Columns.Add<int>("Id");
        target.AddRow();
        idCol.SetValue(0, 1);

        var source = new RecordTable();
        var srcIdCol = source.Columns.Add<int>("Id");
        var srcAgeCol = source.Columns.Add<int>("Age");
        var srcCityCol = source.Columns.Add<string>("City");
        source.AddRow();
        srcIdCol.SetValue(0, 1);
        srcAgeCol.SetValue(0, 25);
        srcCityCol.SetValue(0, "Beijing");

        // Act
        target.Enrich(source, "Id", "Id", null);

        // Assert
        Assert.AreEqual(1, target.Count);
        Assert.AreEqual(3, target.Columns.Count);
        Assert.IsNotNull(target.Columns.Find("Age"));
        Assert.IsNotNull(target.Columns.Find("City"));
        Assert.AreEqual(25, target[0]["Age"]);
        Assert.AreEqual("Beijing", target[0]["City"]);
    }

    [TestMethod]
    public void Enrich_WithSpecificColumns_EmptyList_ShouldAddNoColumns()
    {
        // Arrange
        var target = new RecordTable();
        var idCol = target.Columns.Add<int>("Id");
        target.AddRow();
        idCol.SetValue(0, 1);

        var source = new RecordTable();
        var srcIdCol = source.Columns.Add<int>("Id");
        var srcAgeCol = source.Columns.Add<int>("Age");
        source.AddRow();
        srcIdCol.SetValue(0, 1);
        srcAgeCol.SetValue(0, 25);

        // Act
        target.Enrich(source, "Id", "Id", Array.Empty<string>());

        // Assert
        Assert.AreEqual(1, target.Count);
        Assert.AreEqual(1, target.Columns.Count);
        Assert.IsNull(target.Columns.Find("Age"));
    }

    [TestMethod]
    public void Enrich_WithExistingColumns_ShouldNotOverwriteExistingColumns()
    {
        // Arrange
        var target = new RecordTable();
        var idCol = target.Columns.Add<int>("Id");
        var ageCol = target.Columns.Add<int>("Age");
        target.AddRow();
        idCol.SetValue(0, 1);
        ageCol.SetValue(0, 20);

        var source = new RecordTable();
        var srcIdCol = source.Columns.Add<int>("Id");
        var srcAgeCol = source.Columns.Add<int>("Age");
        var srcCityCol = source.Columns.Add<string>("City");
        source.AddRow();
        srcIdCol.SetValue(0, 1);
        srcAgeCol.SetValue(0, 25);
        srcCityCol.SetValue(0, "Beijing");

        // Act
        target.Enrich(source, "Id", "Id", null);

        // Assert
        Assert.AreEqual(1, target.Count);
        Assert.AreEqual(3, target.Columns.Count);
        Assert.AreEqual(20, target[0]["Age"]); // 应该保持原值，不被覆盖
        Assert.AreEqual("Beijing", target[0]["City"]);
    }

    [TestMethod]
    public void Enrich_WithMultipleRows_ShouldEnrichAllRows()
    {
        // Arrange
        var target = new RecordTable();
        var idCol = target.Columns.Add<int>("Id");
        var nameCol = target.Columns.Add<string>("Name");
        target.AddRow();
        target.AddRow();
        target.AddRow();
        idCol.SetValue(0, 1);
        nameCol.SetValue(0, "Alice");
        idCol.SetValue(1, 2);
        nameCol.SetValue(1, "Bob");
        idCol.SetValue(2, 3);
        nameCol.SetValue(2, "Carol");

        var source = new RecordTable();
        var srcIdCol = source.Columns.Add<int>("Id");
        var srcAgeCol = source.Columns.Add<int>("Age");
        source.AddRow();
        source.AddRow();
        srcIdCol.SetValue(0, 1);
        srcAgeCol.SetValue(0, 25);
        srcIdCol.SetValue(1, 3);
        srcAgeCol.SetValue(1, 35);

        // Act
        target.Enrich(source, "Id", "Id", null);

        // Assert - 列被添加，匹配的行有值，未匹配的行有默认值
        Assert.AreEqual(3, target.Count);
        Assert.AreEqual(25, target[0]["Age"]);
        Assert.AreEqual(0, target[1]["Age"]); // Bob (Id=2) 没有匹配，使用默认值 0
        Assert.AreEqual(35, target[2]["Age"]);
    }

    [TestMethod]
    public void Enrich_WithNoSourceRows_ShouldDoNothing()
    {
        // Arrange
        var target = new RecordTable();
        var idCol = target.Columns.Add<int>("Id");
        target.AddRow();
        idCol.SetValue(0, 1);

        var source = new RecordTable();
        source.Columns.Add<int>("Id");
        source.Columns.Add<int>("Age");

        // Act
        target.Enrich(source, "Id", "Id", null);

        // Assert - 当源没有行时，列不会被添加
        Assert.AreEqual(1, target.Count);
        Assert.AreEqual(1, target.Columns.Count);
        Assert.IsNull(target.Columns.Find("Age"));
    }

    [TestMethod]
    public void Enrich_WithNoTargetRows_ShouldDoNothing()
    {
        // Arrange
        var target = new RecordTable();
        target.Columns.Add<int>("Id");

        var source = new RecordTable();
        var srcIdCol = source.Columns.Add<int>("Id");
        var srcAgeCol = source.Columns.Add<int>("Age");
        source.AddRow();
        srcIdCol.SetValue(0, 1);
        srcAgeCol.SetValue(0, 25);

        // Act
        target.Enrich(source, "Id", "Id", null);

        // Assert
        Assert.AreEqual(0, target.Count);
        Assert.AreEqual(1, target.Columns.Count);
    }
}
