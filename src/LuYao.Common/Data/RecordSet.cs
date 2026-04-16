using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace LuYao.Data;

/// <summary>
/// 命名 Record 集合，用于组织和管理多个 <see cref="Record"/>。
/// 以 <see cref="Record.Name"/> 作为管理键，支持按名称增删改查。
/// </summary>
public partial class RecordSet : IEnumerable<Record>
{
    private readonly Dictionary<string, Record> _records;
    private readonly List<string> _names;
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
        _records = new Dictionary<string, Record>(_comparer);
        _names = new List<string>();
    }

    /// <summary>
    /// 获取集合中 Record 的数量。
    /// </summary>
    public int Count => _records.Count;

    /// <summary>
    /// 获取集合中所有 Record 的名称。
    /// </summary>
    public IReadOnlyList<string> Names => _names;

    /// <summary>
    /// 按名称获取 Record。
    /// </summary>
    /// <param name="name">Record 的名称。</param>
    /// <returns>对应的 <see cref="Record"/> 实例。</returns>
    /// <exception cref="KeyNotFoundException">当名称不存在时抛出。</exception>
    public Record this[string name] => Get(name);

    /// <summary>
    /// 按名称添加一个 Record。如果名称已存在则抛出异常。
    /// </summary>
    /// <param name="name">Record 的名称。</param>
    /// <param name="record">要添加的 <see cref="Record"/> 实例。</param>
    /// <exception cref="ArgumentException">当 <paramref name="name"/> 为空或空白时抛出。</exception>
    /// <exception cref="ArgumentNullException">当 <paramref name="record"/> 为 null 时抛出。</exception>
    /// <exception cref="ArgumentException">当名称已存在时抛出。</exception>
    public void Add(string name, Record record)
    {
        ValidateName(name);
        if (record == null) throw new ArgumentNullException(nameof(record));
        if (_records.ContainsKey(name)) throw new ArgumentException($"名称 '{name}' 已经存在", nameof(name));
        record.Name = name;
        _records.Add(name, record);
        _names.Add(name);
    }

    /// <summary>
    /// 按名称设置一个 Record。如果名称已存在则覆盖，不存在则添加。
    /// </summary>
    /// <param name="name">Record 的名称。</param>
    /// <param name="record">要设置的 <see cref="Record"/> 实例。</param>
    /// <exception cref="ArgumentException">当 <paramref name="name"/> 为空或空白时抛出。</exception>
    /// <exception cref="ArgumentNullException">当 <paramref name="record"/> 为 null 时抛出。</exception>
    public void Set(string name, Record record)
    {
        ValidateName(name);
        if (record == null) throw new ArgumentNullException(nameof(record));
        record.Name = name;
        if (_records.ContainsKey(name))
        {
            _records[name] = record;
        }
        else
        {
            _records.Add(name, record);
            _names.Add(name);
        }
    }

    /// <summary>
    /// 按名称获取 Record。
    /// </summary>
    /// <param name="name">Record 的名称。</param>
    /// <returns>对应的 <see cref="Record"/> 实例。</returns>
    /// <exception cref="KeyNotFoundException">当名称不存在时抛出。</exception>
    public Record Get(string name)
    {
        if (!_records.TryGetValue(name, out var record))
            throw new KeyNotFoundException($"名称 '{name}' 不存在");
        return record;
    }

    /// <summary>
    /// 尝试按名称获取 Record。
    /// </summary>
    /// <param name="name">Record 的名称。</param>
    /// <param name="record">如果找到则返回对应的 <see cref="Record"/> 实例，否则为 null。</param>
    /// <returns>如果找到则返回 true，否则返回 false。</returns>
    public bool TryGet(string name, out Record? record)
    {
        return _records.TryGetValue(name, out record);
    }

    /// <summary>
    /// 按名称删除 Record。
    /// </summary>
    /// <param name="name">要删除的 Record 的名称。</param>
    /// <returns>如果成功删除则返回 true，否则返回 false。</returns>
    public bool Remove(string name)
    {
        if (!_records.Remove(name)) return false;
        for (int i = _names.Count - 1; i >= 0; i--)
        {
            if (_comparer.Equals(_names[i], name))
            {
                _names.RemoveAt(i);
                break;
            }
        }
        return true;
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
        if (!_records.TryGetValue(oldName, out var record))
            throw new KeyNotFoundException($"名称 '{oldName}' 不存在");
        if (!_comparer.Equals(oldName, newName) && _records.ContainsKey(newName))
            throw new ArgumentException($"名称 '{newName}' 已经存在", nameof(newName));

        _records.Remove(oldName);
        record.Name = newName;
        _records.Add(newName, record);

        for (int i = 0; i < _names.Count; i++)
        {
            if (_comparer.Equals(_names[i], oldName))
            {
                _names[i] = newName;
                break;
            }
        }
    }

    /// <summary>
    /// 清空所有 Record。
    /// </summary>
    public void Clear()
    {
        _records.Clear();
        _names.Clear();
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
            var record = Record.Read(dt);
            set.Add(dt.TableName, record);
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
        foreach (var name in _names)
        {
            var record = _records[name];
            var dt = new DataTable(name);
            record.Write(dt);
            ds.Tables.Add(dt);
        }
    }

    /// <inheritdoc/>
    public IEnumerator<Record> GetEnumerator()
    {
        foreach (var name in _names)
        {
            yield return _records[name];
        }
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("名称不能为空或空白", nameof(name));
    }

    }
