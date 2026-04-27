using LuYao.Collections.Generic;
using LuYao.Data.Meta;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LuYao.Data;

/// <summary>
/// 列集合。
/// </summary>
/// <remarks>
/// 内部以 <see cref="KeyedList{TKey, TValue}"/> 维护数据，对外提供 <see cref="IReadOnlyList{T}"/> 视图，
/// 不再继承 <see cref="List{T}"/>，避免外部通过基类接口绕过列名/类型校验。
///
/// 语义约定：
/// <list type="bullet">
///   <item><description><see cref="Find(string)"/>：列不存在时返回 <see langword="null"/>。</description></item>
///   <item><description><see cref="Get(string)"/>：列不存在时抛 <see cref="KeyNotFoundException"/>。</description></item>
///   <item><description><see cref="Add(string, Type)"/> / <see cref="Add{T}(string)"/>：同名同类型返回已有列；
///     同名不同类型抛 <see cref="InvalidOperationException"/>。</description></item>
/// </list>
/// </remarks>
public class FrameColumnCollection : IReadOnlyList<FrameColumn>
{
    private readonly KeyedList<string, FrameColumn> _items
        = new KeyedList<string, FrameColumn>(c => c.Name, StringComparer.Ordinal);

    /// <summary>
    /// 关联的记录。
    /// </summary>
    public Frame Frame { get; }

    /// <summary>
    /// 初始化 <see cref="FrameColumnCollection"/> 类的新实例。
    /// </summary>
    /// <param name="record">关联的记录实例。</param>
    /// <exception cref="ArgumentNullException">当 <paramref name="record"/> 为 null 时抛出。</exception>
    internal FrameColumnCollection(Frame record)
    {
        this.Frame = record ?? throw new ArgumentNullException(nameof(record));
    }

    /// <summary>
    /// 列数量。
    /// </summary>
    public int Count => _items.Count;

    /// <summary>
    /// 按索引访问列。
    /// </summary>
    /// <param name="index">从零开始的列索引。</param>
    public FrameColumn this[int index] => _items[index];

    /// <summary>
    /// 按列名访问列。列不存在时返回 <see langword="null"/>。
    /// </summary>
    /// <param name="name">要查找的列名。</param>
    public FrameColumn? this[string name]
    {
        get
        {
            if (string.IsNullOrWhiteSpace(name)) return null;
            return this.Find(name);
        }
    }

    /// <summary>
    /// 当添加行时调用，用于扩展列的容量以适应新行。
    /// </summary>
    internal void OnAddRow()
    {
        if (_items.Count == 0) return;
        int num = this.Frame.Count;
        foreach (FrameColumn col in _items)
        {
            if (col.Capacity >= num) continue;
            col.Extend(num);
        }
        this.Frame.Capacity = _items.Min(f => f.Capacity);
    }

    /// <summary>
    /// 查找指定列名的索引。
    /// </summary>
    /// <param name="name">要查找的列名。</param>
    /// <returns>如果找到列则返回索引，否则返回 -1。</returns>
    public int IndexOf(string name)
    {
        if (name == null) return -1;
        return _items.IndexOfKey(name);
    }

    /// <summary>
    /// 判断指定的列名是否存在。
    /// </summary>
    /// <param name="name">要检查的列名。</param>
    /// <returns>如果列名存在则返回 true，否则返回 false。</returns>
    public bool Contains(string name)
    {
        if (name == null) return false;
        return _items.ContainsKey(name);
    }

    /// <summary>
    /// 根据列名查找列。
    /// </summary>
    /// <param name="name">要查找的列名。</param>
    /// <returns>如果找到列则返回 <see cref="FrameColumn"/> 实例，否则返回 <see langword="null"/>。</returns>
    public FrameColumn? Find(string name)
    {
        if (name == null) return null;
        int idx = _items.IndexOfKey(name);
        return idx > -1 ? _items[idx] : null;
    }

    /// <summary>
    /// 根据列名获取 <see cref="FrameColumn"/> 实例。
    /// </summary>
    /// <param name="name">要查找的列名。</param>
    /// <returns>对应的 <see cref="FrameColumn"/> 实例。</returns>
    /// <exception cref="KeyNotFoundException">当列名不存在时抛出。</exception>
    public FrameColumn Get(string name)
    {
        var col = Find(name);
        if (col == null) throw new KeyNotFoundException($"列 '{name}' 不存在");
        return col;
    }

    /// <summary>
    /// 根据列名查找指定泛型类型的列。
    /// </summary>
    /// <typeparam name="T">要查找的列的数据类型。</typeparam>
    /// <param name="name">要查找的列名。</param>
    /// <returns>如果找到且类型匹配则返回 <see cref="FrameColumn{T}"/> 实例；列不存在时返回 <see langword="null"/>。</returns>
    /// <exception cref="InvalidCastException">当找到的列类型与 <typeparamref name="T"/> 不匹配时抛出。</exception>
    public FrameColumn<T>? Find<T>(string name)
    {
        var col = this.Find(name);
        if (col == null) return null;
        if (col.Type == typeof(T)) return (FrameColumn<T>)col;
        throw new InvalidCastException($"列 '{name}' 的类型为 {col.Type.Name}，无法转换为 {typeof(T).Name}");
    }

    /// <summary>
    /// 根据列名删除列。
    /// </summary>
    /// <param name="name">要删除的列名。</param>
    /// <returns>是否删除成功。</returns>
    public bool Remove(string name)
    {
        int idx = this.IndexOf(name);
        if (idx > -1)
        {
            _items.RemoveAt(idx);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 清空所有列并重置记录行数。
    /// </summary>
    public void Clear()
    {
        _items.Clear();
        this.Frame.OnClear();
    }

    #region Add

    /// <summary>
    /// 根据列名和类型添加一个列。
    /// </summary>
    /// <param name="name">列名。</param>
    /// <param name="type">列的数据类型。</param>
    /// <returns>新建或已有的 <see cref="FrameColumn"/> 实例。</returns>
    /// <exception cref="ArgumentNullException">当 <paramref name="name"/> 为空或空白时抛出。</exception>
    /// <exception cref="InvalidOperationException">当列名已存在但类型与 <paramref name="type"/> 不一致时抛出。</exception>
    public FrameColumn Add(string name, Type type)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name), "列名不能为空");
        if (type == null) throw new ArgumentNullException(nameof(type));
        var existing = Find(name);
        if (existing != null)
        {
            if (existing.Type != type)
                throw new InvalidOperationException(
                    $"列 '{name}' 已存在且类型为 {existing.Type.Name}，无法以类型 {type.Name} 重新添加。");
            return existing;
        }
        FrameColumn col = Helpers.MakeFrameColumn(this.Frame, name, type);
        _items.Add(col);
        return col;
    }

    /// <summary>
    /// 添加指定泛型类型的列。
    /// </summary>
    /// <typeparam name="T">列的数据类型。</typeparam>
    /// <param name="name">列名。</param>
    /// <returns>新建或已有的 <see cref="FrameColumn{T}"/> 实例。</returns>
    /// <exception cref="ArgumentNullException">当 <paramref name="name"/> 为空或空白时抛出。</exception>
    /// <exception cref="InvalidOperationException">当列名已存在但类型与 <typeparamref name="T"/> 不一致时抛出。</exception>
    public FrameColumn<T> Add<T>(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name), "列名不能为空");
        var existing = Find(name);
        if (existing != null)
        {
            if (existing.Type != typeof(T))
                throw new InvalidOperationException(
                    $"列 '{name}' 已存在且类型为 {existing.Type.Name}，无法以类型 {typeof(T).Name} 重新添加。");
            return (FrameColumn<T>)existing;
        }
        Helpers.ValidateColumnType(typeof(T));
        var col = new FrameColumn<T>(this.Frame, name, typeof(T));
        _items.Add(col);
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
        // 列名变更后需让内部键缓存失效，否则按新名称的查找会失败。
        _items.InvalidateCache();
    }

    /// <summary>
    /// 替换指定索引处的列实例。
    /// </summary>
    /// <param name="index">要替换的列索引。</param>
    /// <param name="column">新的列实例。</param>
    internal void ReplaceAt(int index, FrameColumn column)
    {
        _items[index] = column;
    }

    /// <inheritdoc/>
    public IEnumerator<FrameColumn> GetEnumerator() => _items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();
}