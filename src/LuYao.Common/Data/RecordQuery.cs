using System;
using System.Collections.Generic;
using System.Linq;

namespace LuYao.Data;

/// <summary>
/// 延迟执行的查询对象，通过链式调用记录执行计划，最终通过 <see cref="ToRecord"/> 物化结果。
/// 内部采用索引传递管道：可纯索引变换的操作（Where/OrderBy/Skip/Take/Distinct）仅传递 <c>int[]</c> 行索引，
/// 遇到必须物化的操作（Select/Join/GroupBy/Set 等）时才产生新 <see cref="Record"/>，从而大幅减少中间拷贝。
/// </summary>
public class RecordQuery
{
    private readonly Record _source;
    private readonly QueryOptions _options;
    private readonly List<IQueryStep> _steps = new();

    internal RecordQuery(Record source, QueryOptions options)
    {
        _source = source ?? throw new ArgumentNullException(nameof(source));
        _options = options ?? new QueryOptions();
    }

    #region Step abstractions

    /// <summary>查询管道步骤标记接口。</summary>
    private interface IQueryStep { }

    /// <summary>
    /// 仅变换行索引数组，不产生新 Record 的步骤。
    /// </summary>
    private sealed class IndexStep : IQueryStep
    {
        private readonly Func<Record, int[], QueryOptions, int[]> _transform;
        public IndexStep(Func<Record, int[], QueryOptions, int[]> transform) => _transform = transform;
        public int[] Execute(Record source, int[] indices, QueryOptions opts) => _transform(source, indices, opts);
    }

    /// <summary>
    /// 必须物化为新 Record 的步骤（Select / Join / GroupBy / Set ops 等）。
    /// 接收上游 Record + 当前行索引，返回全新 Record。
    /// </summary>
    private sealed class MaterializeStep : IQueryStep
    {
        private readonly Func<Record, int[], QueryOptions, Record> _execute;
        public MaterializeStep(Func<Record, int[], QueryOptions, Record> execute) => _execute = execute;
        public Record Execute(Record source, int[] indices, QueryOptions opts) => _execute(source, indices, opts);
    }

    #endregion

    #region Public chain API

    /// <summary>
    /// 按条件过滤行。
    /// </summary>
    /// <param name="predicate">筛选谓词，参数为 <see cref="RecordRow"/>。</param>
    /// <returns>当前查询对象，支持链式调用。</returns>
    public RecordQuery Where(Func<RecordRow, bool> predicate)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        _steps.Add(new IndexStep((source, indices, _) =>
        {
            var result = new List<int>(indices.Length);
            foreach (var i in indices)
            {
                if (predicate(new RecordRow(source, i)))
                    result.Add(i);
            }
            return result.ToArray();
        }));
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
        var names = (string[])columnNames.Clone();
        _steps.Add(new MaterializeStep((source, indices, _) => ExecuteSelect(source, indices, names)));
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
        _steps.Add(new IndexStep((source, indices, _) => ExecuteCompositeSort(source, indices, sortKeys)));
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
        _steps.Add(new IndexStep((source, indices, _) => ExecuteDistinct(source, indices, names)));
        return this;
    }

    /// <summary>
    /// 取前 N 行。
    /// </summary>
    /// <param name="count">要获取的行数。</param>
    /// <returns>当前查询对象，支持链式调用。</returns>
    public RecordQuery Take(int count)
    {
        _steps.Add(new IndexStep((source, indices, _) =>
        {
            if (count <= 0) return new int[0];
            if (count >= indices.Length) return indices;
            var result = new int[count];
            Array.Copy(indices, result, count);
            return result;
        }));
        return this;
    }

    /// <summary>
    /// 跳过前 N 行。
    /// </summary>
    /// <param name="count">要跳过的行数。</param>
    /// <returns>当前查询对象，支持链式调用。</returns>
    public RecordQuery Skip(int count)
    {
        _steps.Add(new IndexStep((source, indices, _) =>
        {
            if (count <= 0) return indices;
            if (count >= indices.Length) return new int[0];
            var result = new int[indices.Length - count];
            Array.Copy(indices, count, result, 0, result.Length);
            return result;
        }));
        return this;
    }

    #region Join

    /// <summary>
    /// 等价内连接（<see cref="InnerJoin"/> 的别名）。
    /// </summary>
    public RecordQuery Join(Record right, string leftKey, string rightKey, string? rightPrefix = null)
        => InnerJoin(right, leftKey, rightKey, rightPrefix);

    /// <summary>
    /// 等价内连接（<see cref="InnerJoin"/> 的别名），右表为查询对象。
    /// </summary>
    public RecordQuery Join(RecordQuery right, string leftKey, string rightKey, string? rightPrefix = null)
        => InnerJoin(right, leftKey, rightKey, rightPrefix);

    /// <summary>
    /// 内连接。
    /// </summary>
    public RecordQuery InnerJoin(Record right, string leftKey, string rightKey, string? rightPrefix = null)
    {
        if (right == null) throw new ArgumentNullException(nameof(right));
        if (leftKey == null) throw new ArgumentNullException(nameof(leftKey));
        if (rightKey == null) throw new ArgumentNullException(nameof(rightKey));
        _steps.Add(new MaterializeStep((source, indices, opts) =>
            ExecuteJoin(Materialize(source, indices), right.Clone(), leftKey, rightKey, rightPrefix, JoinType.Inner, opts)));
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
        _steps.Add(new MaterializeStep((source, indices, opts) =>
            ExecuteJoin(Materialize(source, indices), right.ToRecord(), leftKey, rightKey, rightPrefix, JoinType.Inner, opts)));
        return this;
    }

    /// <summary>
    /// 左连接。
    /// </summary>
    public RecordQuery LeftJoin(Record right, string leftKey, string rightKey, string? rightPrefix = null)
    {
        if (right == null) throw new ArgumentNullException(nameof(right));
        if (leftKey == null) throw new ArgumentNullException(nameof(leftKey));
        if (rightKey == null) throw new ArgumentNullException(nameof(rightKey));
        _steps.Add(new MaterializeStep((source, indices, opts) =>
            ExecuteJoin(Materialize(source, indices), right.Clone(), leftKey, rightKey, rightPrefix, JoinType.Left, opts)));
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
        _steps.Add(new MaterializeStep((source, indices, opts) =>
            ExecuteJoin(Materialize(source, indices), right.ToRecord(), leftKey, rightKey, rightPrefix, JoinType.Left, opts)));
        return this;
    }

    /// <summary>
    /// 右连接。
    /// </summary>
    public RecordQuery RightJoin(Record right, string leftKey, string rightKey, string? rightPrefix = null)
    {
        if (right == null) throw new ArgumentNullException(nameof(right));
        if (leftKey == null) throw new ArgumentNullException(nameof(leftKey));
        if (rightKey == null) throw new ArgumentNullException(nameof(rightKey));
        _steps.Add(new MaterializeStep((source, indices, opts) =>
            ExecuteJoin(Materialize(source, indices), right.Clone(), leftKey, rightKey, rightPrefix, JoinType.Right, opts)));
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
        _steps.Add(new MaterializeStep((source, indices, opts) =>
            ExecuteJoin(Materialize(source, indices), right.ToRecord(), leftKey, rightKey, rightPrefix, JoinType.Right, opts)));
        return this;
    }

    /// <summary>
    /// 全外连接：保留左右两表的所有行，无匹配一侧填 null。
    /// </summary>
    public RecordQuery FullOuterJoin(Record right, string leftKey, string rightKey, string? rightPrefix = null)
    {
        if (right == null) throw new ArgumentNullException(nameof(right));
        if (leftKey == null) throw new ArgumentNullException(nameof(leftKey));
        if (rightKey == null) throw new ArgumentNullException(nameof(rightKey));
        _steps.Add(new MaterializeStep((source, indices, opts) =>
            ExecuteJoin(Materialize(source, indices), right.Clone(), leftKey, rightKey, rightPrefix, JoinType.FullOuter, opts)));
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
        _steps.Add(new MaterializeStep((source, indices, opts) =>
            ExecuteJoin(Materialize(source, indices), right.ToRecord(), leftKey, rightKey, rightPrefix, JoinType.FullOuter, opts)));
        return this;
    }

    /// <summary>
    /// 交叉连接（笛卡尔积）：左表每行与右表每行两两组合，结果行数 = 左表行数 × 右表行数。
    /// </summary>
    public RecordQuery CrossJoin(Record right, string? rightPrefix = null)
    {
        if (right == null) throw new ArgumentNullException(nameof(right));
        _steps.Add(new MaterializeStep((source, indices, opts) =>
            ExecuteCrossJoin(Materialize(source, indices), right.Clone(), rightPrefix)));
        return this;
    }

    /// <summary>
    /// 交叉连接（笛卡尔积），右表为查询对象。
    /// </summary>
    public RecordQuery CrossJoin(RecordQuery right, string? rightPrefix = null)
    {
        if (right == null) throw new ArgumentNullException(nameof(right));
        _steps.Add(new MaterializeStep((source, indices, opts) =>
            ExecuteCrossJoin(Materialize(source, indices), right.ToRecord(), rightPrefix)));
        return this;
    }

    #endregion

    #region Set Algebra

    /// <summary>合并两个结果集并去重。</summary>
    public RecordQuery Union(Record other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
        _steps.Add(new MaterializeStep((source, indices, opts) =>
            ExecuteConcat(Materialize(source, indices), other.Clone(), deduplicate: true)));
        return this;
    }

    /// <summary>合并两个结果集并去重，另一方为查询对象。</summary>
    public RecordQuery Union(RecordQuery other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
        _steps.Add(new MaterializeStep((source, indices, opts) =>
            ExecuteConcat(Materialize(source, indices), other.ToRecord(), deduplicate: true)));
        return this;
    }

    /// <summary>合并两个结果集，保留所有行（不去重）。</summary>
    public RecordQuery UnionAll(Record other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
        _steps.Add(new MaterializeStep((source, indices, opts) =>
            ExecuteConcat(Materialize(source, indices), other.Clone(), deduplicate: false)));
        return this;
    }

    /// <summary>合并两个结果集，保留所有行（不去重），另一方为查询对象。</summary>
    public RecordQuery UnionAll(RecordQuery other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
        _steps.Add(new MaterializeStep((source, indices, opts) =>
            ExecuteConcat(Materialize(source, indices), other.ToRecord(), deduplicate: false)));
        return this;
    }

    /// <summary>返回两个结果集的交集（去重）。</summary>
    public RecordQuery Intersect(Record other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
        _steps.Add(new MaterializeStep((source, indices, opts) =>
            ExecuteIntersect(Materialize(source, indices), other.Clone())));
        return this;
    }

    /// <summary>返回两个结果集的交集（去重），另一方为查询对象。</summary>
    public RecordQuery Intersect(RecordQuery other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
        _steps.Add(new MaterializeStep((source, indices, opts) =>
            ExecuteIntersect(Materialize(source, indices), other.ToRecord())));
        return this;
    }

    /// <summary>返回在当前结果集中但不在另一个结果集中的行（去重）。</summary>
    public RecordQuery Except(Record other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
        _steps.Add(new MaterializeStep((source, indices, opts) =>
            ExecuteExcept(Materialize(source, indices), other.Clone())));
        return this;
    }

    /// <summary>返回在当前结果集中但不在另一个结果集中的行（去重），另一方为查询对象。</summary>
    public RecordQuery Except(RecordQuery other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
        _steps.Add(new MaterializeStep((source, indices, opts) =>
            ExecuteExcept(Materialize(source, indices), other.ToRecord())));
        return this;
    }

    /// <summary>将另一个结果集的行追加到当前结果集（不去重）。</summary>
    public RecordQuery Concat(Record other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
        _steps.Add(new MaterializeStep((source, indices, opts) =>
            ExecuteConcat(Materialize(source, indices), other.Clone(), deduplicate: false)));
        return this;
    }

    /// <summary>将另一个结果集的行追加到当前结果集（不去重），另一方为查询对象。</summary>
    public RecordQuery Concat(RecordQuery other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
        _steps.Add(new MaterializeStep((source, indices, opts) =>
            ExecuteConcat(Materialize(source, indices), other.ToRecord(), deduplicate: false)));
        return this;
    }

    #endregion

    #region GroupBy + Aggregate

    /// <summary>
    /// 按指定列分组并应用聚合函数。
    /// </summary>
    public RecordQuery GroupBy(string[] keyColumns, params AggregateDefinition[] aggregates)
    {
        if (keyColumns == null) throw new ArgumentNullException(nameof(keyColumns));
        if (aggregates == null) throw new ArgumentNullException(nameof(aggregates));
        if (keyColumns.Length == 0) throw new ArgumentException("分组键列不能为空", nameof(keyColumns));
        var keys = (string[])keyColumns.Clone();
        var aggs = (AggregateDefinition[])aggregates.Clone();
        _steps.Add(new MaterializeStep((source, indices, opts) =>
            ExecuteGroupBy(Materialize(source, indices), keys, aggs)));
        return this;
    }

    #endregion

    #endregion

    /// <summary>
    /// 物化查询结果，返回新的 <see cref="Record"/> 实例。
    /// 可多次调用，每次产生独立结果。物化不改变原始 <see cref="Record"/> 数据。
    /// </summary>
    /// <returns>物化后的 <see cref="Record"/> 实例。</returns>
    public Record ToRecord()
    {
        // 管道起点：source Record + 全行索引
        Record current = _source;
        int[] indices = Enumerable.Range(0, _source.Count).ToArray();
        bool ownsRecord = false; // 是否已经 clone/物化过 source

        foreach (var step in _steps)
        {
            if (step is IndexStep indexStep)
            {
                // 纯索引变换，不产生新 Record
                indices = indexStep.Execute(current, indices, _options);
            }
            else if (step is MaterializeStep materializeStep)
            {
                // 物化步骤：先将累积的索引应用到 current 得到实际 Record，
                // 然后执行物化操作（该步骤内部可能再次调用 Materialize 来处理 join 参数等）
                var materialized = materializeStep.Execute(current, indices, _options);
                current = materialized;
                indices = Enumerable.Range(0, current.Count).ToArray();
                ownsRecord = true;
            }
        }

        // 管道结束：如果最后仍有未物化的索引变换，执行最终物化
        if (!ownsRecord || !IsIdentityIndices(current, indices))
        {
            current = Materialize(current, indices);
        }

        return current;
    }

    #region Execution

    private enum JoinType { Inner, Left, Right, FullOuter }

    private sealed class SortKey
    {
        public string ColumnName { get; }
        public bool Descending { get; }
        public SortKey(string columnName, bool descending) { ColumnName = columnName; Descending = descending; }
    }

    private readonly Dictionary<int, List<SortKey>> _sortKeysMap = new();

    private List<SortKey>? FindLastSortKeys()
    {
        for (int i = _steps.Count - 1; i >= 0; i--)
        {
            if (_sortKeysMap.TryGetValue(i, out var keys)) return keys;
            break;
        }
        return null;
    }

    /// <summary>
    /// 判断索引数组是否为 [0, 1, 2, ..., count-1] 恒等映射。
    /// </summary>
    private static bool IsIdentityIndices(Record record, int[] indices)
    {
        if (indices.Length != record.Count) return false;
        for (int i = 0; i < indices.Length; i++)
        {
            if (indices[i] != i) return false;
        }
        return true;
    }

    /// <summary>
    /// 将 source 按 indices 物化为新的 Record。
    /// </summary>
    private static Record Materialize(Record source, int[] indices)
    {
        if (IsIdentityIndices(source, indices)) return source.Clone();

        var result = source.CloneSchema();
        for (int i = 0; i < indices.Length; i++)
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

    private static Record ExecuteSelect(Record source, int[] indices, string[] columnNames)
    {
        if (columnNames.Length == 0) throw new ArgumentException("投影列名不能为空");

        foreach (var name in columnNames)
        {
            if (source.Columns.Find(name) == null)
                throw new InvalidOperationException($"列 '{name}' 不存在");
        }

        var result = new Record(source.Name, indices.Length);
        var sourceColumns = new RecordColumn[columnNames.Length];
        for (int i = 0; i < columnNames.Length; i++)
        {
            var srcCol = source.Columns.Get(columnNames[i]);
            sourceColumns[i] = srcCol;
            result.Columns.Add(srcCol.Name, srcCol.Type);
        }

        for (int i = 0; i < indices.Length; i++)
        {
            var srcRow = indices[i];
            result.AddRow();
            for (int c = 0; c < columnNames.Length; c++)
            {
                var val = sourceColumns[c].GetValue(srcRow);
                if (val is not null)
                {
                    result.Columns[c].SetValue(val, i);
                }
            }
        }
        return result;
    }

    private static int[] ExecuteCompositeSort(Record source, int[] indices, List<SortKey> sortKeys)
    {
        var cols = new RecordColumn[sortKeys.Count];
        var descs = new bool[sortKeys.Count];
        for (int i = 0; i < sortKeys.Count; i++)
        {
            cols[i] = source.Columns.Find(sortKeys[i].ColumnName)
                ?? throw new InvalidOperationException($"列 '{sortKeys[i].ColumnName}' 不存在");
            descs[i] = sortKeys[i].Descending;
        }

        var result = (int[])indices.Clone();
        Array.Sort(result, (a, b) =>
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

        return result;
    }

    private static int[] ExecuteDistinct(Record source, int[] indices, string[]? columnNames)
    {
        var targetNames = columnNames ?? source.Columns.Cast<RecordColumn>().Select(c => c.Name).ToArray();

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
        foreach (var r in indices)
        {
            var key = BuildRowKey(targetCols, r);
            if (seen.Add(key))
            {
                keepIndices.Add(r);
            }
        }

        return keepIndices.ToArray();
    }

    private static Record ExecuteJoin(Record left, Record right, string leftKey, string rightKey, string? rightPrefix, JoinType joinType, QueryOptions opts)
    {
        var leftCol = left.Columns.Find(leftKey)
            ?? throw new InvalidOperationException($"左表列 '{leftKey}' 不存在");
        var rightCol = right.Columns.Find(rightKey)
            ?? throw new InvalidOperationException($"右表列 '{rightKey}' 不存在");

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
            result.Columns.Add(destName, col.Type);
            rightColSources.Add(col);
            rightColDestIndices.Add(result.Columns.IndexOf(destName));
        }

        var rightIndex = new Dictionary<string, List<int>>();
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
                var newRow = result.AddRow();
                CopyRow(left, lr, result, newRow.Row, 0, left.Columns.Count);
            }
        }

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

        for (int r = 0; r < left.Count; r++)
        {
            var newRow = result.AddRow();
            for (int c = 0; c < allCols.Count; c++)
            {
                var val = left.Columns[c].GetValue(r);
                if (val is not null) allCols[c].SetValue(val, newRow.Row);
            }
        }

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
            // 对合并结果做 Distinct
            var indices = Enumerable.Range(0, result.Count).ToArray();
            var distinctIndices = ExecuteDistinct(result, indices, null);
            return Materialize(result, distinctIndices);
        }

        return result;
    }

    private static Record ExecuteIntersect(Record left, Record right)
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
            if (rightKeys.Contains(key) && seen.Add(key))
            {
                keepIndices.Add(r);
            }
        }

        return Materialize(left, keepIndices.ToArray());
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

        return Materialize(left, keepIndices.ToArray());
    }

    private static Record ExecuteGroupBy(Record source, string[] keyColumns, AggregateDefinition[] aggregates)
    {
        var keyCols = new RecordColumn[keyColumns.Length];
        for (int i = 0; i < keyColumns.Length; i++)
        {
            keyCols[i] = source.Columns.Find(keyColumns[i])
                ?? throw new InvalidOperationException($"分组键列 '{keyColumns[i]}' 不存在");
        }

        foreach (var agg in aggregates)
        {
            if (agg.SourceColumn != null && source.Columns.Find(agg.SourceColumn) == null)
                throw new InvalidOperationException($"聚合源列 '{agg.SourceColumn}' 不存在");
        }

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

        foreach (var groupKey in groupOrder)
        {
            var rows = groups[groupKey];
            var firstRow = rows[0];
            var newRow = result.AddRow();

            for (int k = 0; k < keyCols.Length; k++)
            {
                var val = keyCols[k].GetValue(firstRow);
                if (val is not null)
                    result.Columns[k].SetValue(val, newRow.Row);
            }

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

    private static string BuildRowKey(RecordColumn[] columns, int row)
    {
        if (columns.Length == 1)
        {
            var v = columns[0].GetValue(row);
            if (v is null) return "\0N";
            var s = Convert.ToString(v) ?? string.Empty;
            return "\0V" + s;
        }
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
