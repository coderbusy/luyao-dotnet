using System;
using System.Collections.Generic;
using System.Linq;

namespace LuYao.Data;

/// <summary>
/// 延迟执行的查询对象，通过链式调用记录执行计划，最终通过 <see cref="ToRecord"/> 物化结果。
/// </summary>
public class RecordQuery
{
    private readonly Record _source;
    private readonly QueryOptions _options;
    private readonly List<Func<Record, QueryOptions, Record>> _steps = new();

    internal RecordQuery(Record source, QueryOptions options)
    {
        _source = source ?? throw new ArgumentNullException(nameof(source));
        _options = options ?? new QueryOptions();
    }

    /// <summary>
    /// 按条件过滤行。
    /// </summary>
    /// <param name="predicate">筛选谓词，参数为 <see cref="RecordRow"/>。</param>
    /// <returns>当前查询对象，支持链式调用。</returns>
    public RecordQuery Where(Func<RecordRow, bool> predicate)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        _steps.Add((record, opts) => ExecuteWhere(record, predicate));
        return this;
    }

    /// <summary>
    /// 按列投影。
    /// </summary>
    /// <param name="columnNames">要保留的列名。</param>
    /// <returns>当前查询对象，支持链式调用。</returns>
    public RecordQuery Select(params string[] columnNames)
    {
        if (columnNames == null) throw new ArgumentNullException(nameof(columnNames));
        // 捕获副本防止外部修改
        var names = (string[])columnNames.Clone();
        _steps.Add((record, opts) => ExecuteSelect(record, names));
        return this;
    }

    /// <summary>
    /// 按指定列升序排序。
    /// </summary>
    /// <param name="columnName">排序列名。</param>
    /// <param name="descending">是否降序。</param>
    /// <returns>当前查询对象，支持链式调用。</returns>
    public RecordQuery OrderBy(string columnName, bool descending = false)
    {
        if (columnName == null) throw new ArgumentNullException(nameof(columnName));
        _steps.Add((record, opts) => ExecuteOrderBy(record, columnName, descending, isThenBy: false));
        return this;
    }

    /// <summary>
    /// 在已有排序基础上追加排序条件。
    /// </summary>
    /// <param name="columnName">排序列名。</param>
    /// <param name="descending">是否降序。</param>
    /// <returns>当前查询对象，支持链式调用。</returns>
    public RecordQuery ThenBy(string columnName, bool descending = false)
    {
        if (columnName == null) throw new ArgumentNullException(nameof(columnName));
        _steps.Add((record, opts) => ExecuteOrderBy(record, columnName, descending, isThenBy: true));
        return this;
    }

    /// <summary>
    /// 去重（基于指定列，若不指定则基于全部列）。
    /// </summary>
    /// <param name="columnNames">用于判断重复的列名，为空时使用全部列。</param>
    /// <returns>当前查询对象，支持链式调用。</returns>
    public RecordQuery Distinct(params string[] columnNames)
    {
        var names = columnNames?.Length > 0 ? (string[])columnNames.Clone() : null;
        _steps.Add((record, opts) => ExecuteDistinct(record, names));
        return this;
    }

    /// <summary>
    /// 取前 N 行。
    /// </summary>
    /// <param name="count">要获取的行数。</param>
    /// <returns>当前查询对象，支持链式调用。</returns>
    public RecordQuery Take(int count)
    {
        _steps.Add((record, opts) => ExecuteTake(record, count));
        return this;
    }

    /// <summary>
    /// 跳过前 N 行。
    /// </summary>
    /// <param name="count">要跳过的行数。</param>
    /// <returns>当前查询对象，支持链式调用。</returns>
    public RecordQuery Skip(int count)
    {
        _steps.Add((record, opts) => ExecuteSkip(record, count));
        return this;
    }

    /// <summary>
    /// 物化查询结果，返回新的 <see cref="Record"/> 实例。
    /// 可多次调用，每次产生独立结果。物化不改变原始 <see cref="Record"/> 数据。
    /// </summary>
    /// <returns>物化后的 <see cref="Record"/> 实例。</returns>
    public Record ToRecord()
    {
        var current = _source.Clone();
        foreach (var step in _steps)
        {
            current = step(current, _options);
        }
        return current;
    }

    #region Execution

    private static Record ExecuteWhere(Record source, Func<RecordRow, bool> predicate)
    {
        var result = source.CloneSchema();
        for (int i = 0; i < source.Count; i++)
        {
            var row = new RecordRow(source, i);
            if (!predicate(row)) continue;
            var newRow = result.AddRow();
            for (int c = 0; c < source.Columns.Count; c++)
            {
                var val = source.Columns[c].GetValue(i);
                if (val is not null)
                {
                    result.Columns[c].SetValue(val, newRow.Row);
                }
            }
        }
        return result;
    }

    private static Record ExecuteSelect(Record source, string[] columnNames)
    {
        if (columnNames.Length == 0) throw new ArgumentException("投影列名不能为空");

        // 校验列名
        foreach (var name in columnNames)
        {
            if (source.Columns.Find(name) == null)
                throw new InvalidOperationException($"列 '{name}' 不存在");
        }

        var result = new Record(source.Name, source.Count);
        var sourceColumns = new RecordColumn[columnNames.Length];
        for (int i = 0; i < columnNames.Length; i++)
        {
            var srcCol = source.Columns.Get(columnNames[i]);
            sourceColumns[i] = srcCol;
            result.Columns.Add(srcCol.Name, srcCol.Type);
        }

        for (int r = 0; r < source.Count; r++)
        {
            result.AddRow();
            for (int c = 0; c < columnNames.Length; c++)
            {
                var val = sourceColumns[c].GetValue(r);
                if (val is not null)
                {
                    result.Columns[c].SetValue(val, r);
                }
            }
        }
        return result;
    }

    private static Record ExecuteOrderBy(Record source, string columnName, bool descending, bool isThenBy)
    {
        var col = source.Columns.Find(columnName)
            ?? throw new InvalidOperationException($"列 '{columnName}' 不存在");

        // 构建行索引数组并排序
        var indices = Enumerable.Range(0, source.Count).ToArray();
        var comparer = Comparer<object>.Default;

        Array.Sort(indices, (a, b) =>
        {
            var va = col.GetValue(a);
            var vb = col.GetValue(b);
            int cmp;
            if (va == null && vb == null) cmp = 0;
            else if (va == null) cmp = -1;
            else if (vb == null) cmp = 1;
            else if (va is IComparable ca) cmp = ca.CompareTo(vb);
            else cmp = 0;
            return descending ? -cmp : cmp;
        });

        return BuildFromIndices(source, indices);
    }

    private static Record ExecuteDistinct(Record source, string[]? columnNames)
    {
        var targetNames = columnNames ?? source.Columns.Cast<RecordColumn>().Select(c => c.Name).ToArray();

        // 校验列名
        foreach (var name in targetNames)
        {
            if (source.Columns.Find(name) == null)
                throw new InvalidOperationException($"列 '{name}' 不存在");
        }

        var targetCols = new RecordColumn[targetNames.Length];
        for (int i = 0; i < targetNames.Length; i++)
        {
            targetCols[i] = source.Columns.Get(targetNames[i]);
        }

        var seen = new HashSet<string>();
        var keepIndices = new List<int>();
        for (int r = 0; r < source.Count; r++)
        {
            var key = BuildRowKey(targetCols, r);
            if (seen.Add(key))
            {
                keepIndices.Add(r);
            }
        }

        return BuildFromIndices(source, keepIndices);
    }

    private static Record ExecuteTake(Record source, int count)
    {
        if (count <= 0) return source.CloneSchema();
        if (count >= source.Count) return source.Clone();

        var indices = Enumerable.Range(0, Math.Min(count, source.Count)).ToList();
        return BuildFromIndices(source, indices);
    }

    private static Record ExecuteSkip(Record source, int count)
    {
        if (count <= 0) return source.Clone();
        if (count >= source.Count) return source.CloneSchema();

        var indices = Enumerable.Range(count, source.Count - count).ToList();
        return BuildFromIndices(source, indices);
    }

    #endregion

    #region Helpers

    private static Record BuildFromIndices(Record source, IList<int> indices)
    {
        var result = source.CloneSchema();
        for (int i = 0; i < indices.Count; i++)
        {
            var srcRow = indices[i];
            var newRow = result.AddRow();
            for (int c = 0; c < source.Columns.Count; c++)
            {
                var val = source.Columns[c].GetValue(srcRow);
                if (val is not null)
                {
                    result.Columns[c].SetValue(val, newRow.Row);
                }
            }
        }
        return result;
    }

    private static string BuildRowKey(RecordColumn[] columns, int row)
    {
        if (columns.Length == 1)
        {
            return Convert.ToString(columns[0].GetValue(row)) ?? string.Empty;
        }
        var parts = new string[columns.Length];
        for (int i = 0; i < columns.Length; i++)
        {
            parts[i] = Convert.ToString(columns[i].GetValue(row)) ?? string.Empty;
        }
        return string.Join("\0", parts);
    }

    #endregion
}
