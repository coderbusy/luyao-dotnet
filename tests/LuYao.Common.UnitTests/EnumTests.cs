namespace LuYao;

public enum TestEnum
{
    None = 0,
    First = 1,
    Second = 2,
    Third = 4
}

[TestClass]
public class EnumTests
{
    [TestMethod]
    public void IsDefined_ValueExists_ReturnsTrue()
    {
        Assert.IsTrue(Enum<TestEnum>.IsDefined(TestEnum.First));
    }

    [TestMethod]
    public void IsDefined_ValueDoesNotExist_ReturnsFalse()
    {
        Assert.IsFalse(Enum<TestEnum>.IsDefined((TestEnum)999));
    }

    [TestMethod]
    public void IsDefined_StringExists_ReturnsTrue()
    {
        Assert.IsTrue(Enum<TestEnum>.IsDefined("First"));
    }

    [TestMethod]
    public void IsDefined_StringDoesNotExist_ReturnsFalse()
    {
        Assert.IsFalse(Enum<TestEnum>.IsDefined("NonExistent"));
    }

    [TestMethod]
    public void IsDefined_IntExists_ReturnsTrue()
    {
        Assert.IsTrue(Enum<TestEnum>.IsDefined(1));
    }

    [TestMethod]
    public void IsDefined_IntDoesNotExist_ReturnsFalse()
    {
        Assert.IsFalse(Enum<TestEnum>.IsDefined(999));
    }

    [TestMethod]
    public void GetValues_Always_ReturnsAllValues()
    {
        var values = Enum<TestEnum>.GetValues().ToList();
        CollectionAssert.AreEquivalent(new[] { TestEnum.None, TestEnum.First, TestEnum.Second, TestEnum.Third }, values);
    }

    [TestMethod]
    public void GetNames_Always_ReturnsAllNames()
    {
        var names = Enum<TestEnum>.GetNames();
        CollectionAssert.AreEquivalent(new[] { "None", "First", "Second", "Third" }, names.ToList());
    }

    [TestMethod]
    public void GetName_ValueExists_ReturnsName()
    {
        Assert.AreEqual("First", Enum<TestEnum>.GetName(TestEnum.First));
    }

    [TestMethod]
    public void GetName_ValueDoesNotExist_ReturnsNull()
    {
        Assert.IsNull(Enum<TestEnum>.GetName((TestEnum)999));
    }

    [TestMethod]
    public void Parse_ValidString_ReturnsEnumValue()
    {
        Assert.AreEqual(TestEnum.First, Enum<TestEnum>.Parse("First"));
    }

    [TestMethod]
    public void Parse_InvalidString_ThrowsExactly()
    {
        Assert.ThrowsExactly<ArgumentException>(() => Enum<TestEnum>.Parse("NonExistent"));
    }

    [TestMethod]
    public void Parse_ValidStringIgnoreCase_ReturnsEnumValue()
    {
        Assert.AreEqual(TestEnum.First, Enum<TestEnum>.Parse("first", true));
    }

    [TestMethod]
    public void TryParse_ValidString_ReturnsTrueAndValue()
    {
        Assert.IsTrue(Enum<TestEnum>.TryParse("First", out var result));
        Assert.AreEqual(TestEnum.First, result);
    }

    [TestMethod]
    public void TryParse_InvalidString_ReturnsFalse()
    {
        Assert.IsFalse(Enum<TestEnum>.TryParse("NonExistent", out var result));
    }

    [TestMethod]
    public void ParseOrNull_ValidString_ReturnsEnumValue()
    {
        Assert.AreEqual(TestEnum.First, Enum<TestEnum>.ParseOrNull("First"));
    }

    [TestMethod]
    public void ParseOrNull_InvalidString_ReturnsNull()
    {
        Assert.IsNull(Enum<TestEnum>.ParseOrNull("NonExistent"));
    }

    [TestMethod]
    public void CastOrNull_ValidInt_ReturnsEnumValue()
    {
        Assert.AreEqual(TestEnum.First, Enum<TestEnum>.CastOrNull(1));
    }

    [TestMethod]
    public void CastOrNull_InvalidInt_ReturnsNull()
    {
        Assert.IsNull(Enum<TestEnum>.CastOrNull(999));
    }

    [TestMethod]
    public void GetFlags_ValidFlagEnum_ReturnsFlags()
    {
        var flags = Enum<TestEnum>.GetFlags(TestEnum.First | TestEnum.Second).ToList();
        CollectionAssert.AreEquivalent(new[] { TestEnum.First, TestEnum.Second }, flags);
    }
}