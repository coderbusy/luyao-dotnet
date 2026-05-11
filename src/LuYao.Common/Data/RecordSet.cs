using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace LuYao.Data;

/// <summary>
/// 命名 Record 集合，用于组织和管理多个 <see cref="RecordTable"/>。
/// 以 <see cref="RecordTable.Name"/> 作为管理键，支持按名称增删改查。
/// </summary>
public partial class RecordSet : IEnumerable<RecordTable>
{
    private readonly SortedDictionary<string, RecordTable> _records;
    private readonly StringComparer _comparer;

    /// <summary>
    /// 使用默认名称比较策略（区分大小写）初始化 <see cref="RecordSet"/> 的新实例。
    /// </summary>
    public RecordSet() : this(StringComparer.Ordinal)
    {
    }

    /// <summary>
    /// 使用指定的名称比较策略初始化 <see cref="RecordSet"/> 的新实例。
    /// </summary>
    /// <param name="comparer">用于比较 Record 名称的比较器。</param>
    /// <exception cref="ArgumentNullException">当 <paramref name="comparer"/> 为 null 时抛出。</exception>
    public RecordSet(StringComparer comparer)
    {
        _comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
        _records = new SortedDictionary<string, RecordTable>(_comparer);
    }

    /// <summary>
    /// 获取集合中 Record 的数量。
    /// </summary>
    public int Count => _records.Count;

    /// <summary>
    /// 获取集合中所有 Record 的名称。
    /// </summary>
    public IEnumerable<string> Names => _records.Keys;

    /// <summary>
    /// 按名称获取 Record。
    /// </summary>
    /// <param name="name">Record 的名称。</param>
    /// <returns>对应的 <see cref="RecordTable"/> 实例。</returns>
    /// <exception cref="KeyNotFoundException">当名称不存在时抛出。</exception>
    public RecordTable this[string name] => Get(name);

    /// <summary>
    /// 按名称添加一个 Record。如果名称已存在则抛出异常。
    /// </summary>
    /// <param name="name">Record 的名称。</param>
    /// <param name="table">要添加的 <see cref="RecordTable"/> 实例。</param>
    /// <exception cref="ArgumentException">当 <paramref name="name"/> 为空或空白时抛出。</exception>
    /// <exception cref="ArgumentNullException">当 <paramref name="table"/> 为 null 时抛出。</exception>
    /// <exception cref="ArgumentException">当名称已存在时抛出。</exception>
    public void Add(string name, RecordTable table)
    {
        ValidateName(name);
        if (table == null) throw new ArgumentNullException(nameof(table));
        if (_records.ContainsKey(name)) throw new ArgumentException($"名称 '{name}' 已经存在", nameof(name));
        table.Name = name;
        _records.Add(name, table);
    }

    /// <summary>
    /// 按名称设置一个 Record。如果名称已存在则覆盖，不存在则添加。
    /// </summary>
    /// <param name="name">Record 的名称。</param>
    /// <param name="table">要设置的 <see cref="RecordTable"/> 实例。</param>
    /// <exception cref="ArgumentException">当 <paramref name="name"/> 为空或空白时抛出。</exception>
    /// <exception cref="ArgumentNullException">当 <paramref name="table"/> 为 null 时抛出。</exception>
    public void Set(string name, RecordTable table)
    {
        ValidateName(name);
        if (table == null) throw new ArgumentNullException(nameof(table));
        table.Name = name;
        _records[name] = table;
    }

    /// <summary>
    /// 按名称获取 Record。
    /// </summary>
    /// <param name="name">Record 的名称。</param>
    /// <returns>对应的 <see cref="RecordTable"/> 实例。</returns>
    /// <exception cref="KeyNotFoundException">当名称不存在时抛出。</exception>
    public RecordTable Get(string name)
    {
        if (!_records.TryGetValue(name, out var table))
            throw new KeyNotFoundException($"名称 '{name}' 不存在");
        return table;
    }

    /// <summary>
    /// 尝试按名称获取 Record。
    /// </summary>
    /// <param name="name">Record 的名称。</param>
    /// <param name="record">如果找到则返回对应的 <see cref="Record"/> 实例，否则为 null。</param>
    /// <returns>如果找到则返回 true，否则返回 false。</returns>

#if NETCOREAPP2_0_OR_GREATER
    public bool TryGet(string name, [MaybeNullWhen(false)] out RecordTable table)
    {
        return _records.TryGetValue(name, out table);
    }
#else
    public bool TryGet(string name, out RecordTable table)
    {
        return _records.TryGetValue(name, out table);
    }
#endif

    /// <summary>
    /// 按名称删除 Record。
    /// </summary>
    /// <param name="name">要删除的 Record 的名称。</param>
    /// <returns>如果成功删除则返回 true，否则返回 false。</returns>
    public bool Remove(string name)
    {
        return _records.Remove(name);
    }

    /// <summary>
    /// 判断指定名称的 Record 是否存在。
    /// </summary>
    /// <param name="name">要检查的名称。</param>
    /// <returns>如果存在则返回 true，否则返回 false。</returns>
    public bool Contains(string name)
    {
        return _records.ContainsKey(name);
    }

    /// <summary>
    /// 重命名一个 Record。
    /// </summary>
    /// <param name="oldName">原名称。</param>
    /// <param name="newName">新名称。</param>
    /// <exception cref="KeyNotFoundException">当 <paramref name="oldName"/> 不存在时抛出。</exception>
    /// <exception cref="ArgumentException">当 <paramref name="newName"/> 为空或空白时抛出。</exception>
    /// <exception cref="ArgumentException">当 <paramref name="newName"/> 已存在时抛出。</exception>
    public void Rename(string oldName, string newName)
    {
        ValidateName(newName);
        if (!_records.TryGetValue(oldName, out var table))
            throw new KeyNotFoundException($"名称 '{oldName}' 不存在");
        if (!_comparer.Equals(oldName, newName) && _records.ContainsKey(newName))
            throw new ArgumentException($"名称 '{newName}' 已经存在", nameof(newName));

        _records.Remove(oldName);
        table.Name = newName;
        _records.Add(newName, table);
    }

    /// <summary>
    /// 清空所有 Record。
    /// </summary>
    public void Clear()
    {
        _records.Clear();
    }

    /// <summary>
    /// 从 <see cref="DataSet"/> 创建 <see cref="RecordSet"/>。
    /// </summary>
    /// <param name="ds">源 <see cref="DataSet"/> 实例。</param>
    /// <returns>包含所有表数据的 <see cref="RecordSet"/> 实例。</returns>
    /// <exception cref="ArgumentNullException">当 <paramref name="ds"/> 为 null 时抛出。</exception>
    public static RecordSet FromDataSet(DataSet ds)
    {
        if (ds == null) throw new ArgumentNullException(nameof(ds));
        var set = new RecordSet();
        foreach (DataTable dt in ds.Tables)
        {
            var table = RecordTable.Read(dt);
            set.Add(dt.TableName, table);
        }
        return set;
    }

    /// <summary>
    /// 将当前 <see cref="RecordSet"/> 导出为 <see cref="DataSet"/>。
    /// </summary>
    /// <returns>包含所有 Record 数据的 <see cref="DataSet"/> 实例。</returns>
    public DataSet ToDataSet()
    {
        var ds = new DataSet();
        WriteTo(ds);
        return ds;
    }

    /// <summary>
    /// 将当前内容写入指定的 <see cref="DataSet"/>。
    /// </summary>
    /// <param name="ds">目标 <see cref="DataSet"/> 实例。</param>
    /// <exception cref="ArgumentNullException">当 <paramref name="ds"/> 为 null 时抛出。</exception>
    public void WriteTo(DataSet ds)
    {
        if (ds == null) throw new ArgumentNullException(nameof(ds));
        foreach (var kvp in _records)
        {
            var dt = new DataTable(kvp.Key);
            kvp.Value.Write(dt);
            ds.Tables.Add(dt);
        }
    }

    /// <inheritdoc/>
    public IEnumerator<RecordTable> GetEnumerator() => _records.Values.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("名称不能为空或空白", nameof(name));
    }
}
