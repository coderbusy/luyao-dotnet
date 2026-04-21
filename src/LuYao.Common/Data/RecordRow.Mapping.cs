using LuYao.Data.Meta;

namespace LuYao.Data;

partial struct RecordRow
{
    /// <summary>
    /// 将当前行的列值填充到已有对象 <paramref name="data"/> 的对应属性中。
    /// </summary>
    /// <typeparam name="T">目标对象类型。</typeparam>
    /// <param name="data">要被填充的对象实例。</param>
    public void Fill<T>(T data) where T : class
    {
        XCopy<T>.CopyFrom(data, this);
    }

    /// <summary>
    /// 创建一个新的 <typeparamref name="T"/> 实例，并将当前行的列值填充到其属性中。
    /// </summary>
    /// <typeparam name="T">目标对象类型，必须有无参构造函数。</typeparam>
    /// <returns>已填充属性的新对象实例。</returns>
    public T To<T>() where T : class, new()
    {
        var ret = new T();
        this.Fill(ret);
        return ret;
    }

    /// <summary>
    /// 将对象 <paramref name="data"/> 的可读属性值写入当前行对应的列。
    /// </summary>
    /// <typeparam name="T">数据来源的对象类型。</typeparam>
    /// <param name="data">属性值的来源对象。</param>
    public void CopyFrom<T>(T data) where T : class => XCopy<T>.CopyTo(data, this);
}
