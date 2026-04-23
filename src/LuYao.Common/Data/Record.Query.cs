using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

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


    /// <summary>
    /// 按指定列的值对所有数据行进行分组，返回以列值为键、行列表为值的字典。
    /// </summary>
    /// <remarks>
    /// <para>当指定列不存在时，返回空字典。</para>
    /// <para>分组键的类型 <typeparamref name="T"/> 需与列的实际类型兼容，否则转换失败时行为取决于 <see cref="RecordColumn.To{T}(int)"/> 的实现。</para>
    /// </remarks>
    /// <typeparam name="T">分组键的类型，必须为非空类型。</typeparam>
    /// <param name="fld">用于分组的列名。</param>
    /// <returns>
    /// 以列值为键、对应 <see cref="RecordRow"/> 列表为值的字典。
    /// 若指定列不存在，则返回空字典。
    /// </returns>
    public IDictionary<T, List<RecordRow>> Group<T>(string fld) where T : struct
    {
        var ret = new Dictionary<T, List<RecordRow>>();
        var col = this.Columns.Find(fld);
        foreach (var row in this)
        {
            T key = col?.To<T>(row) ?? default;
            if (!ret.TryGetValue(key, out var tmp))
            {
                tmp = new List<RecordRow>();
                ret.Add(key, tmp);
            }
            tmp.Add(row);
        }
        return ret;
    }

    public IDictionary<String, IList<RecordRow>> Group(string fld)
    {
        var ret = new Dictionary<String, IList<RecordRow>>();
        var col = this.Columns.Find(fld);
        foreach (var row in this)
        {
            String key = col?.To<string>(row) ?? string.Empty;
            if (!ret.TryGetValue(key, out var tmp))
            {
                tmp = new List<RecordRow>();
                ret.Add(key, tmp);
            }
            tmp.Add(row);
        }
        return ret;
    }
    public IDictionary<String, IList<RecordRow>> Group(params string[] flds)
    {
        var ret = new Dictionary<String, IList<RecordRow>>();
        var cols = flds.Select(Columns.Find).ToArray();
        foreach (var row in this)
        {
            String key = string.Join("-", cols.Select(c => c?.To<string>(row) ?? string.Empty));
            if (!ret.TryGetValue(key, out var tmp))
            {
                tmp = new List<RecordRow>();
                ret.Add(key, tmp);
            }
            tmp.Add(row);
        }
        return ret;
    }

#if NETSTANDARD2_0_OR_GREATER || NET6_0_OR_GREATER
    /// <summary>
    /// 按两个指定列的组合值对所有数据行进行分组，返回以列值元组为键、行列表为值的字典。
    /// </summary>
    /// <remarks>
    /// <para>当任意指定列不存在时，返回空字典。</para>
    /// <para>分组键的类型需与对应列的实际类型兼容。</para>
    /// </remarks>
    /// <typeparam name="T1">第一个分组列的类型。</typeparam>
    /// <typeparam name="T2">第二个分组列的类型。</typeparam>
    /// <param name="fld1">第一个分组列的列名。</param>
    /// <param name="fld2">第二个分组列的列名。</param>
    /// <returns>以 <c>(T1, T2)</c> 元组为键、对应 <see cref="RecordRow"/> 列表为值的字典。若任意列不存在，则返回空字典。</returns>
    public IDictionary<(T1?, T2?), IList<RecordRow>> Group<T1, T2>(string fld1, string fld2)
    {
        var ret = new Dictionary<(T1?, T2?), IList<RecordRow>>();
        var col1 = this.Columns.Find(fld1);
        var col2 = this.Columns.Find(fld2);
        foreach (var row in this)
        {
            T1? val1 = default;
            T2? val2 = default;
            if (col1 != null) val1 = col1.To<T1>(row);
            if (col2 != null) val2 = col2.To<T2>(row);
            (T1?, T2?) key = (val1, val2);
            if (!ret.TryGetValue(key, out var tmp))
            {
                tmp = new List<RecordRow>();
                ret.Add(key, tmp);
            }
            tmp.Add(row);
        }
        return ret;
    }

    /// <summary>
    /// 按三个指定列的组合值对所有数据行进行分组，返回以列值元组为键、行列表为值的字典。
    /// </summary>
    /// <remarks>
    /// <para>当任意指定列不存在时，返回空字典。</para>
    /// </remarks>
    /// <typeparam name="T1">第一个分组列的类型。</typeparam>
    /// <typeparam name="T2">第二个分组列的类型。</typeparam>
    /// <typeparam name="T3">第三个分组列的类型。</typeparam>
    /// <param name="fld1">第一个分组列的列名。</param>
    /// <param name="fld2">第二个分组列的列名。</param>
    /// <param name="fld3">第三个分组列的列名。</param>
    /// <returns>以 <c>(T1, T2, T3)</c> 元组为键、对应 <see cref="RecordRow"/> 列表为值的字典。若任意列不存在，则返回空字典。</returns>
    public IDictionary<(T1, T2, T3), IList<RecordRow>> Group<T1, T2, T3>(string fld1, string fld2, string fld3)
    {
        var ret = new Dictionary<(T1, T2, T3), IList<RecordRow>>();
        var col1 = this.Columns.Find<T1>(fld1);
        var col2 = this.Columns.Find<T2>(fld2);
        var col3 = this.Columns.Find<T3>(fld3);
        if (col1 == null || col2 == null || col3 == null) return ret;
        foreach (var row in this)
        {
            var key = (col1.GetValue(row), col2.GetValue(row), col3.GetValue(row));
            if (!ret.TryGetValue(key, out var tmp))
            {
                tmp = new List<RecordRow>();
                ret.Add(key, tmp);
            }
            tmp.Add(row);
        }
        return ret;
    }

    /// <summary>
    /// 按四个指定列的组合值对所有数据行进行分组，返回以列值元组为键、行列表为值的字典。
    /// </summary>
    /// <remarks>
    /// <para>当任意指定列不存在时，返回空字典。</para>
    /// </remarks>
    /// <typeparam name="T1">第一个分组列的类型。</typeparam>
    /// <typeparam name="T2">第二个分组列的类型。</typeparam>
    /// <typeparam name="T3">第三个分组列的类型。</typeparam>
    /// <typeparam name="T4">第四个分组列的类型。</typeparam>
    /// <param name="fld1">第一个分组列的列名。</param>
    /// <param name="fld2">第二个分组列的列名。</param>
    /// <param name="fld3">第三个分组列的列名。</param>
    /// <param name="fld4">第四个分组列的列名。</param>
    /// <returns>以 <c>(T1, T2, T3, T4)</c> 元组为键、对应 <see cref="RecordRow"/> 列表为值的字典。若任意列不存在，则返回空字典。</returns>
    public IDictionary<(T1, T2, T3, T4), IList<RecordRow>> Group<T1, T2, T3, T4>(string fld1, string fld2, string fld3, string fld4)
    {
        var ret = new Dictionary<(T1, T2, T3, T4), IList<RecordRow>>();
        var col1 = this.Columns.Find<T1>(fld1);
        var col2 = this.Columns.Find<T2>(fld2);
        var col3 = this.Columns.Find<T3>(fld3);
        var col4 = this.Columns.Find<T4>(fld4);
        if (col1 == null || col2 == null || col3 == null || col4 == null) return ret;
        foreach (var row in this)
        {
            var key = (col1.GetValue(row), col2.GetValue(row), col3.GetValue(row), col4.GetValue(row));
            if (!ret.TryGetValue(key, out var tmp))
            {
                tmp = new List<RecordRow>();
                ret.Add(key, tmp);
            }
            tmp.Add(row);
        }
        return ret;
    }

    /// <summary>
    /// 按五个指定列的组合值对所有数据行进行分组，返回以列值元组为键、行列表为值的字典。
    /// </summary>
    /// <remarks>
    /// <para>当任意指定列不存在时，返回空字典。</para>
    /// </remarks>
    /// <typeparam name="T1">第一个分组列的类型。</typeparam>
    /// <typeparam name="T2">第二个分组列的类型。</typeparam>
    /// <typeparam name="T3">第三个分组列的类型。</typeparam>
    /// <typeparam name="T4">第四个分组列的类型。</typeparam>
    /// <typeparam name="T5">第五个分组列的类型。</typeparam>
    /// <param name="fld1">第一个分组列的列名。</param>
    /// <param name="fld2">第二个分组列的列名。</param>
    /// <param name="fld3">第三个分组列的列名。</param>
    /// <param name="fld4">第四个分组列的列名。</param>
    /// <param name="fld5">第五个分组列的列名。</param>
    /// <returns>以 <c>(T1, T2, T3, T4, T5)</c> 元组为键、对应 <see cref="RecordRow"/> 列表为值的字典。若任意列不存在，则返回空字典。</returns>
    public IDictionary<(T1, T2, T3, T4, T5), IList<RecordRow>> Group<T1, T2, T3, T4, T5>(string fld1, string fld2, string fld3, string fld4, string fld5)
    {
        var ret = new Dictionary<(T1, T2, T3, T4, T5), IList<RecordRow>>();
        var col1 = this.Columns.Find<T1>(fld1);
        var col2 = this.Columns.Find<T2>(fld2);
        var col3 = this.Columns.Find<T3>(fld3);
        var col4 = this.Columns.Find<T4>(fld4);
        var col5 = this.Columns.Find<T5>(fld5);
        if (col1 == null || col2 == null || col3 == null || col4 == null || col5 == null) return ret;
        foreach (var row in this)
        {
            var key = (col1.GetValue(row), col2.GetValue(row), col3.GetValue(row), col4.GetValue(row), col5.GetValue(row));
            if (!ret.TryGetValue(key, out var tmp))
            {
                tmp = new List<RecordRow>();
                ret.Add(key, tmp);
            }
            tmp.Add(row);
        }
        return ret;
    }

    /// <summary>
    /// 按六个指定列的组合值对所有数据行进行分组，返回以列值元组为键、行列表为值的字典。
    /// </summary>
    /// <remarks>
    /// <para>当任意指定列不存在时，返回空字典。</para>
    /// </remarks>
    /// <typeparam name="T1">第一个分组列的类型。</typeparam>
    /// <typeparam name="T2">第二个分组列的类型。</typeparam>
    /// <typeparam name="T3">第三个分组列的类型。</typeparam>
    /// <typeparam name="T4">第四个分组列的类型。</typeparam>
    /// <typeparam name="T5">第五个分组列的类型。</typeparam>
    /// <typeparam name="T6">第六个分组列的类型。</typeparam>
    /// <param name="fld1">第一个分组列的列名。</param>
    /// <param name="fld2">第二个分组列的列名。</param>
    /// <param name="fld3">第三个分组列的列名。</param>
    /// <param name="fld4">第四个分组列的列名。</param>
    /// <param name="fld5">第五个分组列的列名。</param>
    /// <param name="fld6">第六个分组列的列名。</param>
    /// <returns>以 <c>(T1, T2, T3, T4, T5, T6)</c> 元组为键、对应 <see cref="RecordRow"/> 列表为值的字典。若任意列不存在，则返回空字典。</returns>
    public IDictionary<(T1, T2, T3, T4, T5, T6), IList<RecordRow>> Group<T1, T2, T3, T4, T5, T6>(string fld1, string fld2, string fld3, string fld4, string fld5, string fld6)
    {
        var ret = new Dictionary<(T1, T2, T3, T4, T5, T6), IList<RecordRow>>();
        var col1 = this.Columns.Find<T1>(fld1);
        var col2 = this.Columns.Find<T2>(fld2);
        var col3 = this.Columns.Find<T3>(fld3);
        var col4 = this.Columns.Find<T4>(fld4);
        var col5 = this.Columns.Find<T5>(fld5);
        var col6 = this.Columns.Find<T6>(fld6);
        if (col1 == null || col2 == null || col3 == null || col4 == null || col5 == null || col6 == null) return ret;
        foreach (var row in this)
        {
            var key = (col1.GetValue(row), col2.GetValue(row), col3.GetValue(row), col4.GetValue(row), col5.GetValue(row), col6.GetValue(row));
            if (!ret.TryGetValue(key, out var tmp))
            {
                tmp = new List<RecordRow>();
                ret.Add(key, tmp);
            }
            tmp.Add(row);
        }
        return ret;
    }
#endif
}
