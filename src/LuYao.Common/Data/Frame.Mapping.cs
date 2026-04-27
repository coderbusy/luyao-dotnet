using LuYao.Data.Meta;
using System;
using System.Collections.Generic;

namespace LuYao.Data;

partial class Frame
{
    /// <summary>
    /// 根据单个对象创建一个 <see cref="Frame"/>，自动推断列结构并写入一行数据。
    /// </summary>
    /// <typeparam name="T">数据来源的对象类型。</typeparam>
    /// <param name="data">用于初始化列结构和行数据的对象实例。</param>
    /// <returns>包含一行数据的新 <see cref="Frame"/>。</returns>
    public static Frame From<T>(T data) where T : class
    {
        var re = new Frame();
        re.Columns.AddFrom<T>();
        re.AddRowFrom(data);
        return re;
    }

    /// <summary>
    /// 根据对象集合创建一个 <see cref="Frame"/>，自动推断列结构并将每个对象写入一行。
    /// </summary>
    /// <typeparam name="T">集合元素的对象类型。</typeparam>
    /// <param name="items">用于填充行数据的对象集合。</param>
    /// <returns>包含与集合等量行数据的新 <see cref="Frame"/>。</returns>
    public static Frame FromList<T>(IEnumerable<T> items) where T : class
    {
        var re = new Frame();
        re.Columns.AddFrom<T>();
        re.AddRowsFromList(items);
        return re;
    }

    /// <summary>
    /// 向当前 <see cref="Frame"/> 追加一行，并将 <paramref name="item"/> 的属性值写入该行。
    /// </summary>
    /// <typeparam name="T">数据来源的对象类型。</typeparam>
    /// <param name="item">要追加的对象实例，不能为 <see langword="null"/>。</param>
    /// <exception cref="ArgumentNullException"><paramref name="item"/> 为 <see langword="null"/>。</exception>
    public FrameRow AddRowFrom<T>(T item) where T : class
    {
        if (item == null) throw new ArgumentNullException(nameof(item));
        var ret = this.AddRow();
        ret.CopyFrom(item);
        return ret;
    }

    /// <summary>
    /// 向当前 <see cref="Frame"/> 批量追加行，每个 <paramref name="items"/> 元素对应一行。
    /// </summary>
    /// <typeparam name="T">集合元素的对象类型。</typeparam>
    /// <param name="items">要批量追加的对象集合。</param>
    public void AddRowsFromList<T>(IEnumerable<T> items) where T : class
    {
        foreach (var item in items) this.AddRowFrom(item);
    }

    /// <summary>
    /// 将当前 <see cref="Frame"/> 的所有行转换为 <typeparamref name="T"/> 对象列表。
    /// </summary>
    /// <typeparam name="T">目标对象类型，必须有无参构造函数。</typeparam>
    /// <returns>与行数等量的对象列表。</returns>
    public List<T> ToList<T>() where T : class, new()
    {
        var list = new List<T>();
        foreach (var row in this)
        {
            var item = row.To<T>();
            list.Add(item);
        }
        return list;
    }

    /// <summary>
    /// 将当前 <see cref="Frame"/> 的第一行转换为 <typeparamref name="T"/> 对象。
    /// 如果 <see cref="Frame"/> 没有任何行，则返回一个使用无参构造函数创建的默认实例。
    /// </summary>
    /// <typeparam name="T">目标对象类型，必须有无参构造函数。</typeparam>
    /// <returns>转换后的对象实例。</returns>
    public T To<T>() where T : class, new()
    {
        var ret = new T();
        if (this.Count > 0)
        {
            XCopy<T>.CopyFrom(ret, this[0]);
        }
        return ret;
    }
}
