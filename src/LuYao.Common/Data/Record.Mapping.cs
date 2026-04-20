using LuYao.Data.Meta;
using System.Collections.Generic;

namespace LuYao.Data;

partial class Record
{
    /// <summary>
    /// 根据单个对象创建一个 <see cref="Record"/>，自动推断列结构并写入一行数据。
    /// </summary>
    /// <typeparam name="T">数据来源的对象类型。</typeparam>
    /// <param name="data">用于初始化列结构和行数据的对象实例。</param>
    /// <returns>包含一行数据的新 <see cref="Record"/>。</returns>
    public static Record From<T>(T data) where T : class
    {
        var re = new Record();
        var props = XProp.GetAll(typeof(T));
        foreach (var p in props)
        {
            if (!Helpers.IsSupportedForReading(p)) continue;
            re.Columns.Add(p.Name, p.Type);
        }

        re.AddRow().CopyFrom(data);

        return re;
    }

    /// <summary>
    /// 根据对象集合创建一个 <see cref="Record"/>，自动推断列结构并将每个对象写入一行。
    /// </summary>
    /// <typeparam name="T">集合元素的对象类型。</typeparam>
    /// <param name="items">用于填充行数据的对象集合。</param>
    /// <returns>包含与集合等量行数据的新 <see cref="Record"/>。</returns>
    public static Record From<T>(IEnumerable<T> items) where T : class
    {
        var props = XProp.GetAll(typeof(T));
        var re = new Record();
        foreach (var p in props)
        {
            if (!Helpers.IsSupportedForReading(p)) continue;
            re.Columns.Add(p.Name, p.Type);
        }

        foreach (var item in items)
        {
            re.AddRow().CopyFrom(item);
        }

        return re;
    }

    /// <summary>
    /// 按照类型 <typeparamref name="T"/> 的可读属性向当前 <see cref="Record"/> 追加对应的列定义。
    /// </summary>
    /// <typeparam name="T">提供列定义的对象类型。</typeparam>
    public void FillColumns<T>() where T : class
    {
        var props = XProp.GetAll(typeof(T));
        foreach (var p in props)
        {
            if (!Helpers.IsSupportedForReading(p)) continue;
            this.Columns.Add(p.Name, p.Type);
        }
    }

    /// <summary>
    /// 向当前 <see cref="Record"/> 追加一行，并将 <paramref name="item"/> 的属性值写入该行。
    /// </summary>
    /// <typeparam name="T">数据来源的对象类型。</typeparam>
    /// <param name="item">要导入的对象实例，不能为 <see langword="null"/>。</param>
    /// <exception cref="System.ArgumentNullException"><paramref name="item"/> 为 <see langword="null"/>。</exception>
    public void Import<T>(T item) where T : class
    {
        if (item == null) throw new System.ArgumentNullException(nameof(item));
        this.AddRow().CopyFrom(item);
    }

    /// <summary>
    /// 向当前 <see cref="Record"/> 批量追加行，每个 <paramref name="items"/> 元素对应一行。
    /// </summary>
    /// <typeparam name="T">集合元素的对象类型。</typeparam>
    /// <param name="items">要批量导入的对象集合。</param>
    public void Import<T>(IEnumerable<T> items) where T : class
    {
        foreach (var item in items)
        {
            this.AddRow().CopyFrom(item);
        }
    }

    /// <summary>
    /// 将当前 <see cref="Record"/> 的所有行转换为 <typeparamref name="T"/> 对象列表。
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
}
