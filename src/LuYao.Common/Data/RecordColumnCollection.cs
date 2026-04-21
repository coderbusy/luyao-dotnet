using LuYao.Data.Meta;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LuYao.Data;

/// <summary>
/// 列集合
/// </summary>
public class RecordColumnCollection : List<RecordColumn>
{
    /// <summary>
    /// 关联的记录
    /// </summary>
    public Record Record { get; }

    /// <summary>
    /// 初始化 <see cref="RecordColumnCollection"/> 类的新实例
    /// </summary>
    /// <param name="record">关联的记录实例</param>
    /// <exception cref="ArgumentNullException">当 <paramref name="record"/> 为 null 时抛出</exception>
    internal RecordColumnCollection(Record record)
    {
        this.Record = record ?? throw new ArgumentNullException(nameof(record));
    }

    /// <summary>
    /// 当添加行时调用，用于扩展列的容量以适应新行
    /// </summary>
    internal void OnAddRow()
    {
        if (this.Count == 0) return;
        int num = this.Record.Count;
        foreach (RecordColumn col in this)
        {
            if (col.Capacity >= num) continue;
            col.Extend(num);
        }
        this.Record.Capacity = this.Min(f => f.Capacity);
    }

    /// <summary>
    /// 查找指定列名的索引
    /// </summary>
    /// <param name="name">要查找的列名</param>
    /// <returns>如果找到列则返回索引，否则返回 -1</returns>
    public int IndexOf(string name)
    {
        for (int i = 0; i < this.Count; i++)
        {
            if (this[i].Name == name) return i;
        }
        return -1;
    }

    /// <summary>
    /// 判断指定的列名是否存在
    /// </summary>
    /// <param name="name">要检查的列名</param>
    /// <returns>如果列名存在则返回 true，否则返回 false</returns>
    public bool Contains(string name) { return this.IndexOf(name) > -1; }

    /// <summary>
    /// 根据列名查找列
    /// </summary>
    /// <param name="name">要查找的列名</param>
    /// <returns>如果找到列则返回 <see cref="RecordColumn"/> 实例，否则返回 null</returns>
    public RecordColumn? Find(string name)
    {
        int idx = this.IndexOf(name);
        return idx > -1 ? this[idx] : null;
    }

    /// <summary>
    /// 根据列名获取 <see cref="RecordColumn"/> 实例，如果列不存在则抛出 <see cref="KeyNotFoundException"/>。
    /// </summary>
    /// <param name="name">要查找的列名</param>
    /// <returns>对应的 <see cref="RecordColumn"/> 实例</returns>
    /// <exception cref="KeyNotFoundException">当列名不存在时抛出</exception>
    public RecordColumn Get(string name)
    {
        var col = Find(name);
        if (col == null) throw new KeyNotFoundException($"列 '{name}' 不存在");
        return col;
    }

    /// <summary>
    /// 根据列名查找指定泛型类型的列。
    /// </summary>
    /// <typeparam name="T">要查找的列的数据类型</typeparam>
    /// <param name="name">要查找的列名</param>
    /// <returns>如果找到且类型匹配则返回 <see cref="RecordColumn{T}"/> 实例，否则返回 null</returns>
    /// <exception cref="InvalidCastException">当找到的列类型与 <typeparamref name="T"/> 不匹配时抛出</exception>
    public RecordColumn<T>? Find<T>(string name)
    {
        var col = this.Find(name);
        if (col == null) return null;
        if (col.Type == typeof(T)) return (RecordColumn<T>)col;
        throw new InvalidCastException($"列 '{name}' 的类型为 {col.Type.Name}，无法转换为 {typeof(T).Name}");
    }

    /// <summary>
    /// 删除一个列
    /// </summary>
    /// <param name="name">要删除的列名</param>
    public bool Remove(string name)
    {
        int idx = this.IndexOf(name);
        if (idx > -1)
        {
            this.RemoveAt(idx);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 清理所有列并重置记录行数
    /// </summary>
    public new void Clear()
    {
        base.Clear();
        this.Record.OnClear();
    }

    #region Add

    /// <summary>
    /// 根据列名和类型添加一个列。如果列名已存在，直接返回已有列。
    /// </summary>
    /// <param name="name">列名</param>
    /// <param name="type">列的数据类型</param>
    /// <returns>添加的或已有的 <see cref="RecordColumn"/> 实例</returns>
    public RecordColumn Add(string name, Type type)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name), "列名不能为空");
        var existing = Find(name);
        if (existing != null) return existing;
        RecordColumn col = Helpers.MakeRecordColumn(this.Record, name, type);
        base.Add(col);
        return col;
    }

    /// <summary>
    /// 添加指定泛型类型的列。如果列名已存在，直接返回已有列。
    /// </summary>
    /// <typeparam name="T">列的数据类型</typeparam>
    /// <param name="name">列名</param>
    /// <returns>添加的或已有的 <see cref="RecordColumn{T}"/> 实例</returns>
    public RecordColumn<T> Add<T>(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name), "列名不能为空");
        var existing = Find(name);
        if (existing != null) return (RecordColumn<T>)existing;
        Helpers.ValidateColumnType(typeof(T));
        var col = new RecordColumn<T>(this.Record, name, typeof(T));
        base.Add(col);
        return col;
    }

    #endregion

    #region AddFrom

    /// <summary>
    /// 按照类型 <typeparamref name="T"/> 的可读属性向当前集合追加对应的列定义。
    /// </summary>
    /// <typeparam name="T">提供列定义的对象类型。</typeparam>
    public void AddFrom<T>() where T : class
    {
        var props = XProp.GetAll(typeof(T));
        foreach (var p in props)
        {
            if (!Helpers.IsSupportedForReading(p)) continue;
            this.Add(p.Name, p.Type);
        }
    }

    /// <summary>
    /// 根据对象实例（例如匿名类型实例）的可读属性向当前集合追加对应的列定义。
    /// </summary>
    /// <typeparam name="T">提供列定义的对象类型。</typeparam>
    /// <param name="template">用于推断列结构的对象实例；仅使用其编译时类型信息，不读取其属性值。</param>
    /// <exception cref="ArgumentNullException"><paramref name="template"/> 为 <see langword="null"/>。</exception>
    public void AddFrom<T>(T template) where T : class
    {
        if (template == null) throw new ArgumentNullException(nameof(template));
        this.AddFrom<T>();
    }

    /// <summary>
    /// 按照指定的属性名列表向当前集合追加列定义，列的追加顺序与 <paramref name="names"/> 的顺序一致。
    /// 不在类型 <typeparamref name="T"/> 中或不受支持的属性名将被忽略。
    /// 若 <paramref name="names"/> 为 <see langword="null"/> 或空数组，则不追加任何列。
    /// </summary>
    /// <typeparam name="T">提供列类型信息的对象类型。</typeparam>
    /// <param name="names">要追加的属性名数组，列将按此顺序添加。</param>
    public void AddFrom<T>(string[] names) where T : class
    {
        if (names == null || names.Length == 0) return;
        var propMap = new Dictionary<string, XProp>(StringComparer.Ordinal);
        foreach (var p in XProp.GetAll(typeof(T)))
        {
            if (Helpers.IsSupportedForReading(p))
                propMap[p.Name] = p;
        }
        foreach (var name in names)
        {
            if (propMap.TryGetValue(name, out var prop))
                this.Add(prop.Name, prop.Type);
        }
    }

    /// <summary>
    /// 通过 <see cref="NameFilter{T}"/> 的链式配置向当前集合追加列定义。
    /// 列的追加顺序与 <paramref name="filter"/> 中配置的属性选取顺序一致。
    /// </summary>
    /// <typeparam name="T">提供列定义的对象类型。</typeparam>
    /// <param name="filter">用于配置属性过滤规则的委托，接收一个 <see cref="NameFilter{T}"/> 实例。</param>
    public void AddFrom<T>(Action<NameFilter<T>> filter) where T : class
    {
        var arg = new NameFilter<T>();
        filter(arg);
        this.AddFrom<T>(arg.ToNames());
    }

    #endregion

    /// <summary>
    /// 根据列名获取 <see cref="RecordColumn"/> 实例，如果列不存在则返回 null。
    /// </summary>
    /// <param name="name">要查找的列名</param>
    /// <returns>对应的 <see cref="RecordColumn"/> 实例，如果不存在则为 null</returns>
    public RecordColumn? this[string name]
    {
        get
        {
            if (string.IsNullOrWhiteSpace(name)) return null;
            return this.Find(name);
        }
    }

    /// <summary>
    /// 重命名指定列。
    /// </summary>
    /// <param name="oldName">原列名。</param>
    /// <param name="newName">新列名。</param>
    /// <exception cref="ArgumentException">当 <paramref name="newName"/> 为空或空白时抛出。</exception>
    /// <exception cref="KeyNotFoundException">当 <paramref name="oldName"/> 不存在时抛出。</exception>
    /// <exception cref="DuplicateNameException">当 <paramref name="newName"/> 已被其他列使用时抛出。</exception>
    public void Rename(string oldName, string newName)
    {
        if (string.IsNullOrWhiteSpace(newName)) throw new ArgumentException("列名不能为空", nameof(newName));
        var col = Find(oldName) ?? throw new KeyNotFoundException($"列 '{oldName}' 不存在");
        if (oldName != newName && this.Contains(newName))
            throw new DuplicateNameException($"列名 '{newName}' 已经存在");
        col.Name = newName;
    }

    /// <summary>
    /// 替换指定索引处的列实例。
    /// </summary>
    /// <param name="index">要替换的列索引。</param>
    /// <param name="column">新的列实例。</param>
    internal void ReplaceAt(int index, RecordColumn column)
    {
        this[index] = column;
    }
}