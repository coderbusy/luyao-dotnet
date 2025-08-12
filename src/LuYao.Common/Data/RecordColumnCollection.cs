using LuYao.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LuYao.Data;

/// <summary>
/// 列集合
/// </summary>
public class RecordColumnCollection : IReadOnlyList<RecordColumn>
{
    private readonly KeyedList<string, RecordColumn> _list = new KeyedList<string, RecordColumn>(c => c.Name);
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

    #region IReadOnlyList

    ///<inheritdoc/>
    public RecordColumn this[int index] => _list[index];

    ///<inheritdoc/>
    public int Count => _list.Count;

    ///<inheritdoc/>
    public IEnumerator<RecordColumn> GetEnumerator() => _list.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion

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
        return _list.IndexOfKey(name);
    }


    /// <summary>
    /// 判断指定的列名是否存在
    /// </summary>
    /// <param name="name">要检查的列名</param>
    /// <returns>如果列名存在则返回 true，否则返回 false</returns>
    public bool Contains(string name) { return _list.ContainsKey(name); }

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
    /// <param name="column">要删除的列</param>
    public bool Remove(RecordColumn column)
    {
        if (column == null) throw new ArgumentNullException(nameof(column), "要删除的列不能为空");
        if (column.Record != this.Record) return false;
        return this._list.Remove(column);
    }

    /// <summary>
    /// 删除一个列
    /// </summary>
    /// <param name="name">要删除的列名</param>
    public bool Remove(string name)
    {
        RecordColumn? col = null;
        int idx = this.IndexOf(name);
        if (idx > -1)
        {
            col = this[idx];
            this._list.RemoveAt(idx);
        }
        return col != null;
    }

    /// <summary>
    /// 清理所有列
    /// </summary>
    public void Clear()
    {
        this._list.Clear();
        this.Record.OnClear();
    }

    #region Add

    /// <summary>
    /// 验证添加列时的列名有效性
    /// </summary>
    /// <param name="name">要验证的列名</param>
    /// <exception cref="ArgumentNullException">当列名为 null 或空白时抛出</exception>
    /// <exception cref="ArgumentException">当列名已存在时抛出</exception>
    private void OnAdd(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name), "列名不能为空");
        if (this.Contains(name)) throw new DuplicateNameException($"列名 '{name}' 已经存在");
    }

    ///<summary>
    /// 根据列名和类型添加一个列。
    /// </summary>
    /// <param name="name">列名</param>
    /// <param name="type">列的数据类型</param>
    /// <returns>添加的 <see cref="RecordColumn"/> 实例</returns>
    public RecordColumn Add(string name, Type type)
    {
        this.OnAdd(name);
        RecordColumn col = Helpers.MakeRecordColumn(this.Record, name, type);
        this._list.Add(col);
        return col;
    }

    /// <summary>
    /// 添加指定泛型类型的列
    /// </summary>
    /// <typeparam name="T">列的数据类型</typeparam>
    /// <param name="name">列名</param>
    /// <returns>添加的 <see cref="RecordColumn{T}"/> 实例</returns>
    public RecordColumn<T> Add<T>(string name)
    {
        this.OnAdd(name);
        var col = new RecordColumn<T>(this.Record, name, typeof(T));
        this._list.Add(col);
        return col;
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
}