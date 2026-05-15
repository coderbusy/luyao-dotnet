using LuYao.Data.Meta;
using System;

namespace LuYao.Data;

partial struct RecordRow
{
    /// <summary>
    /// 将当前行的列值填充到已有对象 <paramref name="data"/> 的对应属性中。
    /// 使用运行时实际类型，派生类新增属性会被正确处理。
    /// </summary>
    /// <param name="data">要被填充的对象实例，不可为 null。</param>
    /// <exception cref="ArgumentNullException">当 <paramref name="data"/> 为 null 时抛出。</exception>
    public void CopyTo(object data)
    {
        XCopy.CopyFrom(data, this);
    }

    /// <summary>
    /// 将对象 <paramref name="data"/> 的可读属性值写入当前行对应的列。
    /// 使用运行时实际类型，派生类新增属性会被正确处理。
    /// </summary>
    /// <param name="data">属性值的来源对象，不可为 null。</param>
    /// <exception cref="ArgumentNullException">当 <paramref name="data"/> 为 null 时抛出。</exception>
    public void CopyFrom(object data) => XCopy.CopyTo(data, this);

    /// <summary>
    /// 创建一个新的 <typeparamref name="T"/> 实例，并将当前行的列值填充到其属性中。
    /// </summary>
    /// <typeparam name="T">目标对象类型，必须有无参构造函数。</typeparam>
    /// <returns>已填充属性的新对象实例。</returns>
    public T To<T>() where T : class, new()
    {
        var ret = new T();
        this.CopyTo(ret);
        return ret;
    }

}
