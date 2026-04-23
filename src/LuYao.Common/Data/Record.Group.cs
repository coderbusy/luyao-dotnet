using System;
using System.Collections.Generic;
using System.Linq;

namespace LuYao.Data;

partial class Record
{
    /// <summary>
    /// 按指定列的值对记录进行分组。
    /// </summary>
    /// <typeparam name="T">分组键的类型。</typeparam>
    /// <param name="fld">要分组的列名。</param>
    /// <returns>针对所指定列名分组的记录行的字典。</returns>
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

    /// <summary>
    /// 按指定列的字符串值对记录进行分组。
    /// </summary>
    /// <param name="fld">要分组的列名。</param>
    /// <returns>针对所指定列名分组的记录行的字典。</returns>
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

    /// <summary>
    /// 按指定多个列的字符串值拼接后对记录进行分组。
    /// </summary>
    /// <param name="flds">要分组的列名数组。</param>
    /// <returns>针对所指定列名拼接后分组的记录行的字典。</returns>
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
    /// 按指定的两个列的值对记录进行分组。
    /// </summary>
    /// <typeparam name="T1">第一列的类型。</typeparam>
    /// <typeparam name="T2">第二列的类型。</typeparam>
    /// <param name="fld1">第一列的名称。</param>
    /// <param name="fld2">第二列的名称。</param>
    /// <returns>按指定两个列的值分组的记录行的字典。</returns>
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
    /// 按指定的三个列的值对记录进行分组。
    /// </summary>
    /// <typeparam name="T1">第一列的类型。</typeparam>
    /// <typeparam name="T2">第二列的类型。</typeparam>
    /// <typeparam name="T3">第三列的类型。</typeparam>
    /// <param name="fld1">第一列的名称。</param>
    /// <param name="fld2">第二列的名称。</param>
    /// <param name="fld3">第三列的名称。</param>
    /// <returns>按指定三个列的值分组的记录行的字典。</returns>
    public IDictionary<(T1?, T2?, T3?), IList<RecordRow>> Group<T1, T2, T3>(string fld1, string fld2, string fld3)
    {
        var ret = new Dictionary<(T1?, T2?, T3?), IList<RecordRow>>();
        var col1 = this.Columns.Find(fld1);
        var col2 = this.Columns.Find(fld2);
        var col3 = this.Columns.Find(fld3);
        foreach (var row in this)
        {
            T1? val1 = default;
            T2? val2 = default;
            T3? val3 = default;
            if (col1 != null) val1 = col1.To<T1>(row);
            if (col2 != null) val2 = col2.To<T2>(row);
            if (col3 != null) val3 = col3.To<T3>(row);
            var key = (val1, val2, val3);
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
