using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LuYao.Collections.Generic;

/// <summary>
/// 提供一个泛型集合，它同时支持通过索引和键值访问元素。
/// </summary>
/// <typeparam name="TKey">用于标识集合中元素的键的类型。</typeparam>
/// <typeparam name="TValue">集合中元素的类型。</typeparam>
public class KeyedList<TKey, TValue> : IList<TValue>, IComparer<KeyedList<TKey, TValue>.Entry>
{
    /// <summary>
    /// 获取用于从集合元素中提取键值的函数。
    /// </summary>
    public Func<TValue, TKey> KeySelector { get; }

    /// <summary>
    /// 获取用于比较键的比较器。
    /// </summary>
    public IComparer<TKey> KeyComparer { get; }

    private List<TValue> _list = new List<TValue>();
    private Entry[]? _cache;
    private Entry[] BuildCache()
    {
        int count = _list.Count;
        if (count == 0) return Arrays.Empty<Entry>();
        return this._list
            .Select((item, index) => new Entry
            {
                Key = KeySelector(item),
                Index = index
            })
            .OrderBy(e => e.Key, KeyComparer)
            .ThenBy(e => e.Index)
            .ToArray();
    }
    /// <summary>
    /// 表示键和对应索引的内部结构。
    /// </summary>
    internal struct Entry
    {
        /// <summary>
        /// 获取或设置元素的键。
        /// </summary>
        public TKey Key { get; set; }

        /// <summary>
        /// 获取或设置元素在集合中的索引。
        /// </summary>
        public int Index { get; set; }
    }

    /// <summary>
    /// 使用指定的键选择器函数初始化 <see cref="KeyedList{TKey, TValue}"/> 类的新实例。
    /// </summary>
    /// <param name="keySelector">用于从元素中提取键的函数。</param>
    /// <exception cref="ArgumentNullException">keySelector 参数为 null。</exception>
    public KeyedList(Func<TValue, TKey> keySelector)
        : this(keySelector, Comparer<TKey>.Default)
    {
    }

    /// <summary>
    /// 使用指定的键选择器函数和键比较器初始化 <see cref="KeyedList{TKey, TValue}"/> 类的新实例。
    /// </summary>
    /// <param name="keySelector">用于从元素中提取键的函数。</param>
    /// <param name="comparer">用于比较键的比较器。</param>
    /// <exception cref="ArgumentNullException">keySelector 或 comparer 参数为 null。</exception>
    public KeyedList(Func<TValue, TKey> keySelector, IComparer<TKey> comparer)
    {
        KeySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
        KeyComparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
    }

    int IComparer<KeyedList<TKey, TValue>.Entry>.Compare(Entry x, Entry y) => KeyComparer.Compare(x.Key, y.Key);

    /// <summary>
    /// 获取或设置指定索引处的元素。
    /// </summary>
    /// <param name="index">要获取或设置的元素从零开始的索引。</param>
    /// <returns>指定索引处的元素。</returns>
    /// <exception cref="ArgumentOutOfRangeException">index 小于 0 或大于等于 <see cref="Count"/>。</exception>
    public TValue this[int index]
    {
        get => _list[index];
        set
        {
            _list[index] = value;
            InvalidateCache();
        }
    }

    /// <summary>
    /// 获取集合中包含的元素数。
    /// </summary>
    public int Count => _list.Count;

    /// <summary>
    /// 获取一个值，该值指示集合是否为只读。
    /// </summary>
    public bool IsReadOnly => false;

    /// <summary>
    /// 将项添加到集合的末尾。
    /// </summary>
    /// <param name="item">要添加到集合的对象。</param>
    public void Add(TValue item)
    {
        _list.Add(item);
        InvalidateCache();
    }

    /// <summary>
    /// 将指定的项集合添加到集合的末尾。
    /// </summary>
    /// <param name="items">要添加到集合的项的枚举集合。</param>
    /// <exception cref="ArgumentNullException">items 参数为 null。</exception>
    public void AddRange(IEnumerable<TValue> items)
    {
        if (items == null) throw new ArgumentNullException(nameof(items));
        _list.AddRange(items);
        InvalidateCache();
    }

    /// <summary>
    /// 从集合中移除所有元素。
    /// </summary>
    public void Clear()
    {
        _list.Clear();
        InvalidateCache();
    }

    /// <summary>
    /// 确定集合是否包含具有指定键的元素。
    /// </summary>
    /// <param name="key">要在集合中定位的键。</param>
    /// <returns>如果集合包含具有指定键的元素，则为 true；否则为 false。</returns>
    public bool ContainsKey(TKey key) => this.IndexOfKey(key) >= 0;

    /// <summary>
    /// 搜索指定的键，并返回整个集合中第一个匹配元素的从零开始的索引。
    /// </summary>
    /// <param name="key">要在集合中定位的键。</param>
    /// <returns>如果在整个集合中找到具有指定键的元素，则为该元素的从零开始的索引；否则为负数。</returns>
    public int IndexOfKey(TKey key)
    {
        this._cache ??= BuildCache();

        var idx = Array.BinarySearch(_cache, new Entry { Key = key }, this);
        if (idx < 0) return -1;

        // 查找第一个匹配的元素
        while (idx > 0 && KeyComparer.Compare(_cache[idx - 1].Key, key) == 0)
        {
            idx--;
        }

        return _cache[idx].Index;
    }

    /// <summary>
    /// 确定集合是否包含特定元素。
    /// </summary>
    /// <param name="item">要在集合中定位的对象。</param>
    /// <returns>如果在集合中找到 item，则为 true；否则为 false。</returns>
    public bool Contains(TValue item) => _list.Contains(item);

    /// <summary>
    /// 从特定的集合索引处开始，将集合元素复制到一个数组中。
    /// </summary>
    /// <param name="array">作为从集合复制的元素的目标位置的一维数组。数组必须具有从零开始的索引。</param>
    /// <param name="arrayIndex">array 中从零开始的索引，从此处开始复制。</param>
    /// <exception cref="ArgumentNullException">array 为 null。</exception>
    /// <exception cref="ArgumentOutOfRangeException">arrayIndex 小于 0。</exception>
    /// <exception cref="ArgumentException">源集合中的元素数目大于从 arrayIndex 到目标数组末尾的可用空间。</exception>
    public void CopyTo(TValue[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

    /// <summary>
    /// 返回循环访问集合的枚举数。
    /// </summary>
    /// <returns>用于集合的枚举数。</returns>
    public IEnumerator<TValue> GetEnumerator() => _list.GetEnumerator();

    /// <summary>
    /// 搜索指定的对象，并返回整个集合中第一个匹配项的从零开始的索引。
    /// </summary>
    /// <param name="item">要在集合中定位的对象。</param>
    /// <returns>如果在整个集合中找到 item 的第一个匹配项，则为该项的从零开始的索引；否则为 -1。</returns>
    public int IndexOf(TValue item) => _list.IndexOf(item);

    /// <summary>
    /// 将元素插入集合的指定索引处。
    /// </summary>
    /// <param name="index">应插入 item 的从零开始的索引。</param>
    /// <param name="item">要插入的对象。</param>
    /// <exception cref="ArgumentOutOfRangeException">index 小于 0 或大于 <see cref="Count"/>。</exception>
    public void Insert(int index, TValue item)
    {
        _list.Insert(index, item);
        InvalidateCache();
    }

    /// <summary>
    /// 从集合中移除特定对象的第一个匹配项。
    /// </summary>
    /// <param name="item">要从集合中移除的对象。</param>
    /// <returns>如果已成功移除 item，则为 true；否则为 false。如果在原始集合中没有找到 item，该方法也会返回 false。</returns>
    public bool Remove(TValue item)
    {
        bool result = _list.Remove(item);
        if (result) InvalidateCache();
        return result;
    }

    /// <summary>
    /// 移除集合指定索引处的元素。
    /// </summary>
    /// <param name="index">要移除的元素的从零开始的索引。</param>
    /// <exception cref="ArgumentOutOfRangeException">index 小于 0 或大于等于 <see cref="Count"/>。</exception>
    public void RemoveAt(int index)
    {
        _list.RemoveAt(index);
        InvalidateCache();
    }

    /// <summary>
    /// 返回循环访问集合的枚举数。
    /// </summary>
    /// <returns>可用于循环访问集合的 <see cref="IEnumerator"/>。</returns>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// 返回集合中所有具有指定键的元素序列。
    /// </summary>
    /// <param name="key">要查找的元素键。</param>
    /// <returns>一个 <see cref="IEnumerable{TValue}"/>，包含所有具有指定键的元素。</returns>
    public IEnumerable<TValue> Read(TKey key)
    {
        this._cache ??= BuildCache();
        var idx = Array.BinarySearch(_cache, new Entry { Key = key }, this);
        if (idx < 0)
        {
            yield break; // 如果没有找到，返回空集合
        }
        else
        {
            // 查找第一个匹配的元素
            var first = idx;
            var last = idx;

            while (first > 0 && KeyComparer.Compare(_cache[first - 1].Key, key) == 0) first--;

            while (last < _cache.Length - 1 && KeyComparer.Compare(_cache[last + 1].Key, key) == 0) last++;

            for (int i = first; i <= last; i++)
            {
                yield return _list[_cache[i].Index];
            }
        }
    }

    /// <summary>
    /// 使内部缓存失效，强制下次访问时重新构建缓存。
    /// </summary>
    public void InvalidateCache() => _cache = null;
}
