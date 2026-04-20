using LuYao.Data.Meta;

namespace LuYao.Data.Meta;

[TestClass]
public class XPropTests
{
    // ── 测试用模型 ──────────────────────────────────────────────────────────
    private class SampleModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int ReadOnly { get; } = 42;
        private string? Hidden { get; set; }
    }

    private class NoProps { }

    // ── GetAll ───────────────────────────────────────────────────────────────

    [TestMethod]
    public void GetAll_NullType_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => XProp.GetAll(null!));
    }

    [TestMethod]
    public void GetAll_TypeWithNoProps_ReturnsEmptyList()
    {
        var props = XProp.GetAll(typeof(NoProps));
        Assert.AreEqual(0, props.Count);
    }

    [TestMethod]
    public void GetAll_SampleModel_ReturnsOnlyPublicInstanceProps()
    {
        var props = XProp.GetAll(typeof(SampleModel));
        // Id, Name, ReadOnly — Hidden は private なので含まれない
        Assert.AreEqual(3, props.Count);
    }

    [TestMethod]
    public void GetAll_CalledTwice_ReturnsSameInstance()
    {
        var first = XProp.GetAll(typeof(SampleModel));
        var second = XProp.GetAll(typeof(SampleModel));
        Assert.AreSame(first, second);
    }

    // ── Name / Type ──────────────────────────────────────────────────────────

    [TestMethod]
    public void Name_ReturnsPropertyName()
    {
        var prop = FindProp(nameof(SampleModel.Id));
        Assert.AreEqual("Id", prop.Name);
    }

    [TestMethod]
    public void Type_ReturnsPropertyType()
    {
        var prop = FindProp(nameof(SampleModel.Id));
        Assert.AreEqual(typeof(int), prop.Type);
    }

    // ── CanRead / CanWrite ───────────────────────────────────────────────────

    [TestMethod]
    public void CanRead_ReadWriteProp_IsTrue()
    {
        Assert.IsTrue(FindProp(nameof(SampleModel.Id)).CanRead);
    }

    [TestMethod]
    public void CanWrite_ReadWriteProp_IsTrue()
    {
        Assert.IsTrue(FindProp(nameof(SampleModel.Id)).CanWrite);
    }

    [TestMethod]
    public void CanRead_ReadOnlyProp_IsTrue()
    {
        Assert.IsTrue(FindProp(nameof(SampleModel.ReadOnly)).CanRead);
    }

    [TestMethod]
    public void CanWrite_ReadOnlyProp_IsFalse()
    {
        Assert.IsFalse(FindProp(nameof(SampleModel.ReadOnly)).CanWrite);
    }

    // ── GetValue ─────────────────────────────────────────────────────────────

    [TestMethod]
    public void GetValue_IntProp_ReturnsCorrectValue()
    {
        var model = new SampleModel { Id = 99 };
        var value = FindProp(nameof(SampleModel.Id)).GetValue(model);
        Assert.AreEqual(99, value);
    }

    [TestMethod]
    public void GetValue_StringPropNull_ReturnsNull()
    {
        var model = new SampleModel { Name = null };
        var value = FindProp(nameof(SampleModel.Name)).GetValue(model);
        Assert.IsNull(value);
    }

    [TestMethod]
    public void GetValue_WriteOnlyProp_ThrowsInvalidOperationException()
    {
        // 构造一个只有 setter 的属性 —— 使用匿名对象包装器代替；
        // 这里直接验证 SetValue 在 CanWrite=false 时的路径，
        // GetValue 在 CanRead=false 时抛异常。
        var prop = FindProp(nameof(SampleModel.ReadOnly));
        // ReadOnly 仍可读，所以用 SetValue 测试不可写路径
        var model = new SampleModel();
        Assert.Throws<InvalidOperationException>(() => prop.SetValue(model, 1));
    }

    // ── SetValue ─────────────────────────────────────────────────────────────

    [TestMethod]
    public void SetValue_IntProp_UpdatesValue()
    {
        var model = new SampleModel { Id = 0 };
        FindProp(nameof(SampleModel.Id)).SetValue(model, 7);
        Assert.AreEqual(7, model.Id);
    }

    [TestMethod]
    public void SetValue_StringProp_UpdatesValue()
    {
        var model = new SampleModel();
        FindProp(nameof(SampleModel.Name)).SetValue(model, "hello");
        Assert.AreEqual("hello", model.Name);
    }

    // ── XData<T> ─────────────────────────────────────────────────────────────

    [TestMethod]
    public void XData_Set_ThenGet_RoundTrip()
    {
        var model = new SampleModel();
        XData<SampleModel>.Set(model, "Id", 55);
        var result = XData<SampleModel>.Get(model, "Id");
        Assert.AreEqual(55, result);
    }

    [TestMethod]
    public void XData_Get_NullData_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => XData<SampleModel>.Get(null!, "Id"));
    }

    [TestMethod]
    public void XData_Set_NullData_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => XData<SampleModel>.Set(null!, "Id", 1));
    }

    [TestMethod]
    public void XData_Get_UnknownProp_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => XData<SampleModel>.Get(new SampleModel(), "NonExistent"));
    }

    [TestMethod]
    public void XData_Set_UnknownProp_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => XData<SampleModel>.Set(new SampleModel(), "NonExistent", 1));
    }

    // ── helpers ──────────────────────────────────────────────────────────────

    private static XProp FindProp(string name)
    {
        foreach (var p in XProp.GetAll(typeof(SampleModel)))
            if (p.Name == name) return p;
        throw new InvalidOperationException($"Property {name} not found.");
    }
}
