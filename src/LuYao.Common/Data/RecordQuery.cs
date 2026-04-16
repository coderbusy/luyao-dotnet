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
        var sortKeys = new List<SortKey> { new SortKey(columnName, descending) };
        int stepIndex = _steps.Count;
        _steps.Add((record, opts) => ExecuteCompositeSort(record, sortKeys));
        _sortKeysMap[stepIndex] = sortKeys;
        return this;
    }

    /// <summary>
    /// 在已有排序基础上追加排序条件。
    /// </summary>
    /// <param name="columnName">排序列名。</param>
    /// <param name="descending">是否降序。</param>
    /// <returns>当前查询对象，支持链式调用。</returns>
    /// <exception cref="InvalidOperationException">当没有前置 OrderBy 调用时抛出。</exception>
    public RecordQuery ThenBy(string columnName, bool descending = false)
    {
        if (columnName == null) throw new ArgumentNullException(nameof(columnName));
        // 查找最后一个排序 step 并追加键
        var lastSortKeys = FindLastSortKeys();
        if (lastSortKeys == null)
            throw new InvalidOperationException("ThenBy 必须在 OrderBy 之后调用");
        lastSortKeys.Add(new SortKey(columnName, descending));
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

    #region Join

    /// <summary>
    /// 等价内连接（<see cref="InnerJoin"/> 的别名）。
    /// </summary>
    /// <param name="right">右表。</param>
    /// <param name="leftKey">左表键列名。</param>
    /// <param name="rightKey">右表键列名。</param>
    /// <param name="rightPrefix">右表重名列前缀，为 null 时重名抛异常。</param>
    /// <returns>当前查询对象，支持链式调用。</returns>
    public RecordQuery Join(Record right, string leftKey, string rightKey, string? rightPrefix = null)
    {
        return InnerJoin(right, leftKey, rightKey, rightPrefix);
    }

    /// <summary>
    /// 等价内连接（<see cref="InnerJoin"/> 的别名），右表为查询对象。
    /// </summary>
    public RecordQuery Join(RecordQuery right, string leftKey, string rightKey, string? rightPrefix = null)
    {
        return InnerJoin(right, leftKey, rightKey, rightPrefix);
    }

    /// <summary>
    /// 内连接。
    /// </summary>
    /// <param name="right">右表。</param>
    /// <param name="leftKey">左表键列名。</param>
    /// <param name="rightKey">右表键列名。</param>
    /// <param name="rightPrefix">右表重名列前缀，为 null 时重名抛异常。</param>
    /// <returns>当前查询对象，支持链式调用。</returns>
    public RecordQuery InnerJoin(Record right, string leftKey, string rightKey, string? rightPrefix = null)
    {
        if (right == null) throw new ArgumentNullException(nameof(right));
        if (leftKey == null) throw new ArgumentNullException(nameof(leftKey));
        if (rightKey == null) throw new ArgumentNullException(nameof(rightKey));
        _steps.Add((record, opts) => ExecuteJoin(record, right.Clone(), leftKey, rightKey, rightPrefix, JoinType.Inner, opts));
        return this;
    }

    /// <summary>
    /// 内连接，右表为查询对象。
    /// </summary>
    public RecordQuery InnerJoin(RecordQuery right, string leftKey, string rightKey, string? rightPrefix = null)
    {
        if (right == null) throw new ArgumentNullException(nameof(right));
        if (leftKey == null) throw new ArgumentNullException(nameof(leftKey));
        if (rightKey == null) throw new ArgumentNullException(nameof(rightKey));
        _steps.Add((record, opts) => ExecuteJoin(record, right.ToRecord(), leftKey, rightKey, rightPrefix, JoinType.Inner, opts));
        return this;
    }

    /// <summary>
    /// 左连接。
    /// </summary>
    /// <param name="right">右表。</param>
    /// <param name="leftKey">左表键列名。</param>
    /// <param name="rightKey">右表键列名。</param>
    /// <param name="rightPrefix">右表重名列前缀，为 null 时重名抛异常。</param>
    /// <returns>当前查询对象，支持链式调用。</returns>
    public RecordQuery LeftJoin(Record right, string leftKey, string rightKey, string? rightPrefix = null)
    {
        if (right == null) throw new ArgumentNullException(nameof(right));
        if (leftKey == null) throw new ArgumentNullException(nameof(leftKey));
        if (rightKey == null) throw new ArgumentNullException(nameof(rightKey));
        _steps.Add((record, opts) => ExecuteJoin(record, right.Clone(), leftKey, rightKey, rightPrefix, JoinType.Left, opts));
        return this;
    }

    /// <summary>
    /// 左连接，右表为查询对象。
    /// </summary>
    public RecordQuery LeftJoin(RecordQuery right, string leftKey, string rightKey, string? rightPrefix = null)
    {
        if (right == null) throw new ArgumentNullException(nameof(right));
        if (leftKey == null) throw new ArgumentNullException(nameof(leftKey));
        if (rightKey == null) throw new ArgumentNullException(nameof(rightKey));
        _steps.Add((record, opts) => ExecuteJoin(record, right.ToRecord(), leftKey, rightKey, rightPrefix, JoinType.Left, opts));
        return this;
    }

    /// <summary>
    /// 右连接。
    /// </summary>
    /// <param name="right">右表。</param>
    /// <param name="leftKey">左表键列名。</param>
    /// <param name="rightKey">右表键列名。</param>
    /// <param name="rightPrefix">右表重名列前缀，为 null 时重名抛异常。</param>
    /// <returns>当前查询对象，支持链式调用。</returns>
    public RecordQuery RightJoin(Record right, string leftKey, string rightKey, string? rightPrefix = null)
    {
        if (right == null) throw new ArgumentNullException(nameof(right));
        if (leftKey == null) throw new ArgumentNullException(nameof(leftKey));
        if (rightKey == null) throw new ArgumentNullException(nameof(rightKey));
        _steps.Add((record, opts) => ExecuteJoin(record, right.Clone(), leftKey, rightKey, rightPrefix, JoinType.Right, opts));
        return this;
    }

    /// <summary>
    /// 右连接，右表为查询对象。
    /// </summary>
    public RecordQuery RightJoin(RecordQuery right, string leftKey, string rightKey, string? rightPrefix = null)
    {
        if (right == null) throw new ArgumentNullException(nameof(right));
        if (leftKey == null) throw new ArgumentNullException(nameof(leftKey));
        if (rightKey == null) throw new ArgumentNullException(nameof(rightKey));
        _steps.Add((record, opts) => ExecuteJoin(record, right.ToRecord(), leftKey, rightKey, rightPrefix, JoinType.Right, opts));
        return this;
    }

    /// <summary>
    /// 全外连接：保留左右两表的所有行，无匹配一侧填 null。
    /// </summary>
    /// <param name="right">右表。</param>
    /// <param name="leftKey">左表键列名。</param>
    /// <param name="rightKey">右表键列名。</param>
    /// <param name="rightPrefix">右表重名列前缀，为 null 时重名抛异常。</param>
    /// <returns>当前查询对象，支持链式调用。</returns>
    public RecordQuery FullOuterJoin(Record right, string leftKey, string rightKey, string? rightPrefix = null)
    {
        if (right == null) throw new ArgumentNullException(nameof(right));
        if (leftKey == null) throw new ArgumentNullException(nameof(leftKey));
        if (rightKey == null) throw new ArgumentNullException(nameof(rightKey));
        _steps.Add((record, opts) => ExecuteJoin(record, right.Clone(), leftKey, rightKey, rightPrefix, JoinType.FullOuter, opts));
        return this;
    }

    /// <summary>
    /// 全外连接，右表为查询对象。
    /// </summary>
    public RecordQuery FullOuterJoin(RecordQuery right, string leftKey, string rightKey, string? rightPrefix = null)
    {
        if (right == null) throw new ArgumentNullException(nameof(right));
        if (leftKey == null) throw new ArgumentNullException(nameof(leftKey));
        if (rightKey == null) throw new ArgumentNullException(nameof(rightKey));
        _steps.Add((record, opts) => ExecuteJoin(record, right.ToRecord(), leftKey, rightKey, rightPrefix, JoinType.FullOuter, opts));
        return this;
    }

    /// <summary>
    /// 交叉连接（笛卡尔积）：左表每行与右表每行两两组合，结果行数 = 左表行数 × 右表行数。
    /// </summary>
    /// <param name="right">右表。</param>
    /// <param name="rightPrefix">右表重名列前缀，为 null 时重名抛异常。</param>
    /// <returns>当前查询对象，支持链式调用。</returns>
    public RecordQuery CrossJoin(Record right, string? rightPrefix = null)
    {
        if (right == null) throw new ArgumentNullException(nameof(right));
        _steps.Add((record, opts) => ExecuteCrossJoin(record, right.Clone(), rightPrefix));
        return this;
    }

    /// <summary>
    /// 交叉连接（笛卡尔积），右表为查询对象。
    /// </summary>
    public RecordQuery CrossJoin(RecordQuery right, string? rightPrefix = null)
    {
        if (right == null) throw new ArgumentNullException(nameof(right));
        _steps.Add((record, opts) => ExecuteCrossJoin(record, right.ToRecord(), rightPrefix));
        return this;
    }

    #endregion

    #region Set Algebra

    /// <summary>
    /// 合并两个结果集并去重。
    /// </summary>
    /// <param name="other">另一个 Record。</param>
    /// <returns>当前查询对象，支持链式调用。</returns>
    public RecordQuery Union(Record other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
        _steps.Add((record, opts) => ExecuteConcat(record, other.Clone(), deduplicate: true));
        return this;
    }

    /// <summary>
    /// 合并两个结果集并去重，另一方为查询对象。
    /// </summary>
    public RecordQuery Union(RecordQuery other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
        _steps.Add((record, opts) => ExecuteConcat(record, other.ToRecord(), deduplicate: true));
        return this;
    }

    /// <summary>
    /// 合并两个结果集，保留所有行（不去重）。
    /// </summary>
    /// <param name="other">另一个 Record。</param>
    /// <returns>当前查询对象，支持链式调用。</returns>
    public RecordQuery UnionAll(Record other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
        _steps.Add((record, opts) => ExecuteConcat(record, other.Clone(), deduplicate: false));
        return this;
    }

    /// <summary>
    /// 合并两个结果集，保留所有行（不去重），另一方为查询对象。
    /// </summary>
    public RecordQuery UnionAll(RecordQuery other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
        _steps.Add((record, opts) => ExecuteConcat(record, other.ToRecord(), deduplicate: false));
        return this;
    }

    /// <summary>
    /// 返回两个结果集的交集（去重）。
    /// </summary>
    /// <param name="other">另一个 Record。</param>
    /// <returns>当前查询对象，支持链式调用。</returns>
    public RecordQuery Intersect(Record other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
        _steps.Add((record, opts) => ExecuteIntersect(record, other.Clone()));
        return this;
    }

    /// <summary>
    /// 返回两个结果集的交集（去重），另一方为查询对象。
    /// </summary>
    public RecordQuery Intersect(RecordQuery other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
        _steps.Add((record, opts) => ExecuteIntersect(record, other.ToRecord()));
        return this;
    }

    /// <summary>
    /// 返回在当前结果集中但不在另一个结果集中的行（去重）。
    /// </summary>
    /// <param name="other">另一个 Record。</param>
    /// <returns>当前查询对象，支持链式调用。</returns>
    public RecordQuery Except(Record other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
        _steps.Add((record, opts) => ExecuteExcept(record, other.Clone()));
        return this;
    }

    /// <summary>
    /// 返回在当前结果集中但不在另一个结果集中的行（去重），另一方为查询对象。
    /// </summary>
    public RecordQuery Except(RecordQuery other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
        _steps.Add((record, opts) => ExecuteExcept(record, other.ToRecord()));
        return this;
    }

    /// <summary>
    /// 将另一个结果集的行追加到当前结果集（不去重，等价于 SQL UNION ALL 但不要求 Schema 完全一致时的别名）。
    /// </summary>
    /// <param name="other">另一个 Record。</param>
    /// <returns>当前查询对象，支持链式调用。</returns>
    public RecordQuery Concat(Record other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
        _steps.Add((record, opts) => ExecuteConcat(record, other.Clone(), deduplicate: false));
        return this;
    }

    /// <summary>
    /// 将另一个结果集的行追加到当前结果集（不去重），另一方为查询对象。
    /// </summary>
    public RecordQuery Concat(RecordQuery other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
        _steps.Add((record, opts) => ExecuteConcat(record, other.ToRecord(), deduplicate: false));
        return this;
    }

    #endregion

    #region GroupBy + Aggregate

    /// <summary>
    /// 按指定列分组并应用聚合函数。
    /// </summary>
    /// <param name="keyColumns">分组键列名。</param>
    /// <param name="aggregates">聚合定义数组。</param>
    /// <returns>当前查询对象，支持链式调用。</returns>
    public RecordQuery GroupBy(string[] keyColumns, params AggregateDefinition[] aggregates)
    {
        if (keyColumns == null) throw new ArgumentNullException(nameof(keyColumns));
        if (aggregates == null) throw new ArgumentNullException(nameof(aggregates));
        if (keyColumns.Length == 0) throw new ArgumentException("分组键列不能为空", nameof(keyColumns));
        var keys = (string[])keyColumns.Clone();
        var aggs = (AggregateDefinition[])aggregates.Clone();
        _steps.Add((record, opts) => ExecuteGroupBy(record, keys, aggs));
        return this;
    }

    #endregion

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

    private enum JoinType { Inner, Left, Right, FullOuter }

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

    private sealed class SortKey
    {
        public string ColumnName { get; }
        public bool Descending { get; }
        public SortKey(string columnName, bool descending) { ColumnName = columnName; Descending = descending; }
    }

    /// <summary>
    /// 查找最后一个排序 step 关联的 SortKey 列表，用于 ThenBy 追加。
    /// </summary>
    private List<SortKey>? FindLastSortKeys()
    {
        // 从后往前找包含 SortKey 列表的排序 step
        for (int i = _steps.Count - 1; i >= 0; i--)
        {
            if (_sortKeysMap.TryGetValue(i, out var keys)) return keys;
            break; // 只允许紧跟在最后一个 step 后追加
        }
        return null;
    }

    private readonly Dictionary<int, List<SortKey>> _sortKeysMap = new();

    private static Record ExecuteCompositeSort(Record source, List<SortKey> sortKeys)
    {
        // 解析所有排序列
        var cols = new RecordColumn[sortKeys.Count];
        var descs = new bool[sortKeys.Count];
        for (int i = 0; i < sortKeys.Count; i++)
        {
            cols[i] = source.Columns.Find(sortKeys[i].ColumnName)
                ?? throw new InvalidOperationException($"列 '{sortKeys[i].ColumnName}' 不存在");
            descs[i] = sortKeys[i].Descending;
        }

        var indices = Enumerable.Range(0, source.Count).ToArray();
        Array.Sort(indices, (a, b) =>
        {
            for (int k = 0; k < cols.Length; k++)
            {
                var va = cols[k].GetValue(a);
                var vb = cols[k].GetValue(b);
                int cmp;
                if (va == null && vb == null) cmp = 0;
                else if (va == null) cmp = -1;
                else if (vb == null) cmp = 1;
                else if (va is IComparable ca) cmp = ca.CompareTo(vb);
                else cmp = 0;
                if (descs[k]) cmp = -cmp;
                if (cmp != 0) return cmp;
            }
            return 0;
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

    private static Record ExecuteJoin(Record left, Record right, string leftKey, string rightKey, string? rightPrefix, JoinType joinType, QueryOptions opts)
    {
        var leftCol = left.Columns.Find(leftKey)
            ?? throw new InvalidOperationException($"左表列 '{leftKey}' 不存在");
        var rightCol = right.Columns.Find(rightKey)
            ?? throw new InvalidOperationException($"右表列 '{rightKey}' 不存在");

        // 构建结果 schema: 左表全部列 + 右表非键列（或全部列）
        var result = new Record(left.Name, 0);
        foreach (RecordColumn col in left.Columns)
        {
            result.Columns.Add(col.Name, col.Type);
        }

        var rightColSources = new List<RecordColumn>();
        var rightColDestIndices = new List<int>();
        foreach (RecordColumn col in right.Columns)
        {
            var destName = col.Name;
            if (left.Columns.Find(destName) != null)
            {
                if (rightPrefix == null)
                    throw new InvalidOperationException($"右表列 '{destName}' 与左表列重名，请指定 rightPrefix 参数");
                destName = rightPrefix + destName;
            }
            var destCol = result.Columns.Add(destName, col.Type);
            rightColSources.Add(col);
            rightColDestIndices.Add(result.Columns.IndexOf(destName));
        }

        // 构建右表键索引: key -> list of row indices
        var rightIndex = new Dictionary<string, List<int>>();
        var comparison = opts.StringComparison;
        for (int r = 0; r < right.Count; r++)
        {
            var key = ConvertToKeyString(rightCol.GetValue(r));
            if (!rightIndex.TryGetValue(key, out var list))
            {
                list = new List<int>();
                rightIndex[key] = list;
            }
            list.Add(r);
        }

        var matchedRight = (joinType == JoinType.Right || joinType == JoinType.FullOuter)
            ? new HashSet<int>()
            : null;

        // 遍历左表
        for (int lr = 0; lr < left.Count; lr++)
        {
            var leftKeyVal = ConvertToKeyString(leftCol.GetValue(lr));
            var hasMatch = rightIndex.TryGetValue(leftKeyVal, out var rightRows);

            if (hasMatch && rightRows != null)
            {
                foreach (var rr in rightRows)
                {
                    matchedRight?.Add(rr);
                    var newRow = result.AddRow();
                    CopyRow(left, lr, result, newRow.Row, 0, left.Columns.Count);
                    for (int ci = 0; ci < rightColSources.Count; ci++)
                    {
                        var val = rightColSources[ci].GetValue(rr);
                        if (val is not null)
                            result.Columns[rightColDestIndices[ci]].SetValue(val, newRow.Row);
                    }
                }
            }
            else if (joinType == JoinType.Left || joinType == JoinType.FullOuter)
            {
                // Left / FullOuter: 左行保留，右侧填 null
                var newRow = result.AddRow();
                CopyRow(left, lr, result, newRow.Row, 0, left.Columns.Count);
            }
            // Inner / Right: 不匹配的左行丢弃
        }

        // Right / FullOuter: 追加未匹配的右表行（左侧列填 null）
        if (joinType == JoinType.Right || joinType == JoinType.FullOuter)
        {
            for (int rr = 0; rr < right.Count; rr++)
            {
                if (matchedRight != null && matchedRight.Contains(rr)) continue;

                var newRow = result.AddRow();
                for (int ci = 0; ci < rightColSources.Count; ci++)
                {
                    var val = rightColSources[ci].GetValue(rr);
                    if (val is not null)
                        result.Columns[rightColDestIndices[ci]].SetValue(val, newRow.Row);
                }
            }
        }

        return result;
    }

    private static Record ExecuteCrossJoin(Record left, Record right, string? rightPrefix)
    {
        var result = new Record(left.Name, 0);
        foreach (RecordColumn col in left.Columns)
        {
            result.Columns.Add(col.Name, col.Type);
        }

        var crossRightSources = new List<RecordColumn>();
        var crossRightDestIdx = new List<int>();
        foreach (RecordColumn col in right.Columns)
        {
            var destName = col.Name;
            if (left.Columns.Find(destName) != null)
            {
                if (rightPrefix == null)
                    throw new InvalidOperationException($"右表列 '{destName}' 与左表列重名，请指定 rightPrefix 参数");
                destName = rightPrefix + destName;
            }
            result.Columns.Add(destName, col.Type);
            crossRightSources.Add(col);
            crossRightDestIdx.Add(result.Columns.IndexOf(destName));
        }

        // 笛卡尔积：左表每行 × 右表每行
        for (int lr = 0; lr < left.Count; lr++)
        {
            for (int rr = 0; rr < right.Count; rr++)
            {
                var newRow = result.AddRow();
                CopyRow(left, lr, result, newRow.Row, 0, left.Columns.Count);
                for (int ci = 0; ci < crossRightSources.Count; ci++)
                {
                    var val = crossRightSources[ci].GetValue(rr);
                    if (val is not null)
                        result.Columns[crossRightDestIdx[ci]].SetValue(val, newRow.Row);
                }
            }
        }

        return result;
    }

    private static void CopyRow(Record src, int srcRow, Record dest, int destRow, int destColOffset, int colCount)
    {
        for (int c = 0; c < colCount; c++)
        {
            var val = src.Columns[c].GetValue(srcRow);
            if (val is not null)
                dest.Columns[destColOffset + c].SetValue(val, destRow);
        }
    }

    private static string ConvertToKeyString(object? value)
    {
        if (value is null) return "\0NULL\0";
        return Convert.ToString(value) ?? string.Empty;
    }

    private static void ValidateSchemaCompatibility(Record a, Record b)
    {
        if (a.Columns.Count != b.Columns.Count)
            throw new InvalidOperationException($"Schema 不兼容：左表有 {a.Columns.Count} 列，右表有 {b.Columns.Count} 列");
        for (int i = 0; i < a.Columns.Count; i++)
        {
            if (a.Columns[i].Type != b.Columns[i].Type)
                throw new InvalidOperationException($"Schema 不兼容：第 {i} 列类型不匹配（'{a.Columns[i].Name}': {a.Columns[i].Type.Name} vs '{b.Columns[i].Name}': {b.Columns[i].Type.Name}）");
        }
    }

    private static Record ExecuteConcat(Record left, Record right, bool deduplicate)
    {
        ValidateSchemaCompatibility(left, right);

        var result = left.CloneSchema();
        var allCols = result.Columns;

        // 复制左表行
        for (int r = 0; r < left.Count; r++)
        {
            var newRow = result.AddRow();
            for (int c = 0; c < allCols.Count; c++)
            {
                var val = left.Columns[c].GetValue(r);
                if (val is not null) allCols[c].SetValue(val, newRow.Row);
            }
        }

        // 复制右表行
        for (int r = 0; r < right.Count; r++)
        {
            var newRow = result.AddRow();
            for (int c = 0; c < allCols.Count; c++)
            {
                var val = right.Columns[c].GetValue(r);
                if (val is not null) allCols[c].SetValue(val, newRow.Row);
            }
        }

        if (deduplicate)
        {
            var targetCols = new RecordColumn[allCols.Count];
            for (int i = 0; i < allCols.Count; i++) targetCols[i] = allCols[i];
            return ExecuteDistinct(result, null);
        }

        return result;
    }

    private static Record ExecuteIntersect(Record left, Record right)
    {
        ValidateSchemaCompatibility(left, right);

        // 构建右表行键集合
        var rightCols = new RecordColumn[right.Columns.Count];
        for (int i = 0; i < rightCols.Length; i++) rightCols[i] = right.Columns[i];
        var rightKeys = new HashSet<string>();
        for (int r = 0; r < right.Count; r++)
        {
            rightKeys.Add(BuildRowKey(rightCols, r));
        }

        var leftCols = new RecordColumn[left.Columns.Count];
        for (int i = 0; i < leftCols.Length; i++) leftCols[i] = left.Columns[i];

        var seen = new HashSet<string>();
        var keepIndices = new List<int>();
        for (int r = 0; r < left.Count; r++)
        {
            var key = BuildRowKey(leftCols, r);
            if (rightKeys.Contains(key) && seen.Add(key))
            {
                keepIndices.Add(r);
            }
        }

        return BuildFromIndices(left, keepIndices);
    }

    private static Record ExecuteExcept(Record left, Record right)
    {
        ValidateSchemaCompatibility(left, right);

        var rightCols = new RecordColumn[right.Columns.Count];
        for (int i = 0; i < rightCols.Length; i++) rightCols[i] = right.Columns[i];
        var rightKeys = new HashSet<string>();
        for (int r = 0; r < right.Count; r++)
        {
            rightKeys.Add(BuildRowKey(rightCols, r));
        }

        var leftCols = new RecordColumn[left.Columns.Count];
        for (int i = 0; i < leftCols.Length; i++) leftCols[i] = left.Columns[i];

        var seen = new HashSet<string>();
        var keepIndices = new List<int>();
        for (int r = 0; r < left.Count; r++)
        {
            var key = BuildRowKey(leftCols, r);
            if (!rightKeys.Contains(key) && seen.Add(key))
            {
                keepIndices.Add(r);
            }
        }

        return BuildFromIndices(left, keepIndices);
    }

    private static Record ExecuteGroupBy(Record source, string[] keyColumns, AggregateDefinition[] aggregates)
    {
        // 校验键列
        var keyCols = new RecordColumn[keyColumns.Length];
        for (int i = 0; i < keyColumns.Length; i++)
        {
            keyCols[i] = source.Columns.Find(keyColumns[i])
                ?? throw new InvalidOperationException($"分组键列 '{keyColumns[i]}' 不存在");
        }

        // 校验聚合源列
        foreach (var agg in aggregates)
        {
            if (agg.SourceColumn != null && source.Columns.Find(agg.SourceColumn) == null)
                throw new InvalidOperationException($"聚合源列 '{agg.SourceColumn}' 不存在");
        }

        // 分组: key -> list of row indices
        var groups = new Dictionary<string, List<int>>();
        var groupOrder = new List<string>();
        for (int r = 0; r < source.Count; r++)
        {
            var key = BuildRowKey(keyCols, r);
            if (!groups.TryGetValue(key, out var list))
            {
                list = new List<int>();
                groups[key] = list;
                groupOrder.Add(key);
            }
            list.Add(r);
        }

        // 构建结果 schema
        var result = new Record(source.Name, groups.Count);
        foreach (var keyCol in keyCols)
        {
            result.Columns.Add(keyCol.Name, keyCol.Type);
        }
        foreach (var agg in aggregates)
        {
            var outputType = GetAggregateOutputType(agg, source);
            result.Columns.Add(agg.OutputColumn, outputType);
        }

        // 填充结果
        foreach (var groupKey in groupOrder)
        {
            var rows = groups[groupKey];
            var firstRow = rows[0];
            var newRow = result.AddRow();

            // 填充键列
            for (int k = 0; k < keyCols.Length; k++)
            {
                var val = keyCols[k].GetValue(firstRow);
                if (val is not null)
                    result.Columns[k].SetValue(val, newRow.Row);
            }

            // 填充聚合列
            for (int a = 0; a < aggregates.Length; a++)
            {
                var agg = aggregates[a];
                var destIdx = keyCols.Length + a;
                var aggVal = ComputeAggregate(agg, source, rows);
                if (aggVal is not null)
                    result.Columns[destIdx].SetValue(aggVal, newRow.Row);
            }
        }

        return result;
    }

    private static Type GetAggregateOutputType(AggregateDefinition agg, Record source)
    {
        switch (agg.Function)
        {
            case AggregateFunction.Count:
                return typeof(int);
            case AggregateFunction.Avg:
                return typeof(double);
            case AggregateFunction.Sum:
            case AggregateFunction.Min:
            case AggregateFunction.Max:
                if (agg.SourceColumn != null)
                {
                    var col = source.Columns.Find(agg.SourceColumn);
                    if (col != null) return agg.Function == AggregateFunction.Sum ? typeof(double) : col.Type;
                }
                return typeof(double);
            default:
                return typeof(object);
        }
    }

    private static object? ComputeAggregate(AggregateDefinition agg, Record source, List<int> rows)
    {
        RecordColumn? srcCol = agg.SourceColumn != null ? source.Columns.Find(agg.SourceColumn) : null;

        switch (agg.Function)
        {
            case AggregateFunction.Count:
                if (srcCol == null) return rows.Count;
                int count = 0;
                foreach (var r in rows)
                {
                    if (srcCol.GetValue(r) != null) count++;
                }
                return count;

            case AggregateFunction.Sum:
                double sum = 0;
                foreach (var r in rows)
                {
                    var val = srcCol!.GetValue(r);
                    if (val != null) sum += Convert.ToDouble(val);
                }
                return sum;

            case AggregateFunction.Avg:
                double total = 0;
                int cnt = 0;
                foreach (var r in rows)
                {
                    var val = srcCol!.GetValue(r);
                    if (val != null) { total += Convert.ToDouble(val); cnt++; }
                }
                return cnt > 0 ? total / cnt : 0.0;

            case AggregateFunction.Min:
                IComparable? min = null;
                foreach (var r in rows)
                {
                    var val = srcCol!.GetValue(r);
                    if (val is IComparable c)
                    {
                        if (min == null || c.CompareTo(min) < 0) min = c;
                    }
                }
                return min;

            case AggregateFunction.Max:
                IComparable? max = null;
                foreach (var r in rows)
                {
                    var val = srcCol!.GetValue(r);
                    if (val is IComparable c)
                    {
                        if (max == null || c.CompareTo(max) > 0) max = c;
                    }
                }
                return max;

            default:
                return null;
        }
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
            var v = columns[0].GetValue(row);
            if (v is null) return "\0N";
            var s = Convert.ToString(v) ?? string.Empty;
            return "\0V" + s;
        }
        // 使用长度前缀编码避免分隔符碰撞：每个值编码为 "len:value"
        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < columns.Length; i++)
        {
            if (i > 0) sb.Append('\0');
            var val = columns[i].GetValue(row);
            if (val is null)
            {
                sb.Append("-1:");
            }
            else
            {
                var s = Convert.ToString(val) ?? string.Empty;
                sb.Append(s.Length).Append(':').Append(s);
            }
        }
        return sb.ToString();
    }

    #endregion
}
