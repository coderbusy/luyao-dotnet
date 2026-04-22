using System;
using System.Collections.Generic;

namespace LuYao.Data;

public partial class Record
{
    #region Query

    /// <summary>
    /// 在当前记录的所有数据行中，查找指定列与目标值匹配的第一行数据。
    /// 如果指定的列名不存在，或者未找到匹配项，则直接返回 null。
    /// </summary>
    /// <typeparam name="T">要查找的列对应的数据类型。</typeparam>
    /// <param name="name">要查找的列名称。</param>
    /// <param name="value">要匹配的目标值。</param>
    /// <returns>符合条件的第一条 <see cref="RecordRow"/>，未找到则返回 null。</returns>
    public RecordRow? Find<T>(string name, T value)
    {
        var col = this.Columns.Find(name);
        if (col == null) return null;
        var comparer = EqualityComparer<T>.Default;
        for (int i = 0; i < this.Count; i++)
        {
            var val = col.To<T>(i);
            if (comparer.Equals(val, value)) return new RecordRow(this, i);
        }
        return null;
    }

    /// <summary>
    /// 在当前记录的所有数据行中，查找指定列与目标值匹配的所有行数据，并以可枚举的序列返回。
    /// </summary>
    /// <typeparam name="T">要查找的列对应的数据类型。</typeparam>
    /// <param name="name">要查找的列名称。</param>
    /// <param name="value">要匹配的目标值。</param>
    /// <returns>一个包含所有符合条件 <see cref="RecordRow"/> 的枚举器序列。</returns>
    public IEnumerable<RecordRow> FindAll<T>(string name, T value)
    {
        var col = this.Columns.Find(name);
        if (col == null) yield break;
        var comparer = EqualityComparer<T>.Default;
        for (int i = 0; i < this.Count; i++)
        {
            var val = col.To<T>(i);
            if (comparer.Equals(val, value)) yield return new RecordRow(this, i);
        }
    }

    /// <summary>
    /// 使用指定的条件筛选器，查找符合要求的第一条行数据。
    /// </summary>
    /// <param name="filter">用于筛选 <see cref="RecordRow"/> 数据行的条件委托。</param>
    /// <returns>符合条件的第一条 <see cref="RecordRow"/>，未找到则返回 null。</returns>
    /// <exception cref="ArgumentNullException">当 <paramref name="filter"/> 为 null 时抛出。</exception>
    public RecordRow? Find(Func<RecordRow, bool> filter)
    {
        if (filter == null) throw new ArgumentNullException(nameof(filter));
        for (int i = 0; i < this.Count; i++)
        {
            var row = new RecordRow(this, i);
            if (filter(row)) return row;
        }
        return null;
    }

    /// <summary>
    /// 使用指定的条件筛选器，查找所有符合要求的行数据。
    /// </summary>
    /// <param name="filter">用于筛选 <see cref="RecordRow"/> 数据行的条件委托。</param>
    /// <returns>所有符合条件的 <see cref="RecordRow"/> 数据流序列。</returns>
    /// <exception cref="ArgumentNullException">当 <paramref name="filter"/> 为 null 时抛出。</exception>
    public IEnumerable<RecordRow> FindAll(Func<RecordRow, bool> filter)
    {
        if (filter == null) throw new ArgumentNullException(nameof(filter));
        for (int i = 0; i < this.Count; i++)
        {
            var row = new RecordRow(this, i);
            if (filter(row)) yield return row;
        }
    }

    /// <summary>
    /// 使用基于 dynamic 的动态类型筛选器，查找符合要求的第一条行数据。
    /// （依赖 <see cref="RecordRow"/> 对 <see cref="System.Dynamic.IDynamicMetaObjectProvider"/> 的实现）
    /// </summary>
    /// <param name="filter">用于动态筛选数据行的条件委托。</param>
    /// <returns>符合条件的第一条 <see cref="RecordRow"/>，未找到则返回 null。</returns>
    /// <exception cref="ArgumentNullException">当 <paramref name="filter"/> 为 null 时抛出。</exception>
    public RecordRow? FindByDynamic(Func<dynamic, bool> filter)
    {
        if (filter == null) throw new ArgumentNullException(nameof(filter));
        for (int i = 0; i < this.Count; i++)
        {
            var row = new RecordRow(this, i);
            if (filter(row)) return row;
        }
        return null;
    }

    /// <summary>
    /// 使用基于 dynamic 的动态类型筛选器，查找所有符合要求的行数据。
    /// </summary>
    /// <param name="filter">用于动态筛选数据行的条件委托。</param>
    /// <returns>所有符合条件的 <see cref="RecordRow"/> 数据流序列。</returns>
    /// <exception cref="ArgumentNullException">当 <paramref name="filter"/> 为 null 时抛出。</exception>
    public IEnumerable<RecordRow> FindAllByDynamic(Func<dynamic, bool> filter)
    {
        if (filter == null) throw new ArgumentNullException(nameof(filter));
        for (int i = 0; i < this.Count; i++)
        {
            var row = new RecordRow(this, i);
            if (filter(row)) yield return row;
        }
    }

    #endregion
}
