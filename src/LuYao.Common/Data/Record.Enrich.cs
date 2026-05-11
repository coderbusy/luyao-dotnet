using System;
using System.Collections.Generic;

namespace LuYao.Data;

public partial class Record
{
    /// <summary>
    /// 用 <paramref name="source"/> 的数据补全当前记录：对每一行，在 <paramref name="source"/> 中查找与
    /// 共享列 <paramref name="sharedColumn"/> 值匹配的第一行，将 <paramref name="source"/> 中当前记录没有
    /// 的列追加进来，并把对应的值填入。未找到匹配行时该行保持不变。
    /// </summary>
    /// <param name="source">数据来源记录。</param>
    /// <param name="sharedColumn">两个记录中同名的关联列名。</param>
    /// <exception cref="ArgumentNullException">当 <paramref name="source"/> 或 <paramref name="sharedColumn"/> 为 null 时抛出。</exception>
    /// <exception cref="ArgumentException">当 <paramref name="sharedColumn"/> 在当前记录或 <paramref name="source"/> 中不存在时抛出。</exception>
    public void Enrich(Record source, string sharedColumn)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (string.IsNullOrWhiteSpace(sharedColumn)) throw new ArgumentException("列名不能为空。", nameof(sharedColumn));
        Enrich(source, sharedColumn, sharedColumn, null);
    }

    /// <summary>
    /// 用 <paramref name="source"/> 的数据补全当前记录：对每一行，在 <paramref name="source"/> 中查找
    /// <paramref name="sourceColumn"/> 与当前行 <paramref name="selfColumn"/> 值匹配的第一行，将
    /// <paramref name="source"/> 中当前记录没有的列追加进来，并把对应的值填入。未找到匹配行时该行保持不变。
    /// </summary>
    /// <param name="source">数据来源记录。</param>
    /// <param name="selfColumn">当前记录中用于匹配的列名。</param>
    /// <param name="sourceColumn"><paramref name="source"/> 中用于匹配的列名。</param>
    /// <exception cref="ArgumentNullException">当任意参数为 null 时抛出。</exception>
    /// <exception cref="ArgumentException">当关联列在对应记录中不存在时抛出。</exception>
    public void Enrich(Record source, string selfColumn, string sourceColumn)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (string.IsNullOrWhiteSpace(selfColumn)) throw new ArgumentException("列名不能为空。", nameof(selfColumn));
        if (string.IsNullOrWhiteSpace(sourceColumn)) throw new ArgumentException("列名不能为空。", nameof(sourceColumn));
        Enrich(source, selfColumn, sourceColumn, null);
    }

    /// <summary>
    /// 用 <paramref name="source"/> 的数据补全当前记录：对每一行，在 <paramref name="source"/> 中查找
    /// <paramref name="sourceColumn"/> 与当前行 <paramref name="selfColumn"/> 值匹配的第一行，仅将
    /// <paramref name="columnsToEnrich"/> 指定的列（当前记录中不存在的）追加进来并填入对应值。
    /// 未找到匹配行时该行保持不变。
    /// </summary>
    /// <param name="source">数据来源记录。</param>
    /// <param name="selfColumn">当前记录中用于匹配的列名。</param>
    /// <param name="sourceColumn"><paramref name="source"/> 中用于匹配的列名。</param>
    /// <param name="columnsToEnrich">需要从 <paramref name="source"/> 补充的列名集合；为 null 时补充全部不存在的列。</param>
    /// <exception cref="ArgumentNullException">当 <paramref name="source"/>、<paramref name="selfColumn"/> 或 <paramref name="sourceColumn"/> 为 null 时抛出。</exception>
    /// <exception cref="ArgumentException">当关联列在对应记录中不存在时抛出。</exception>
    public void Enrich(Record source, string selfColumn, string sourceColumn, IReadOnlyList<string>? columnsToEnrich)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (string.IsNullOrWhiteSpace(selfColumn)) throw new ArgumentException("列名不能为空。", nameof(selfColumn));
        if (string.IsNullOrWhiteSpace(sourceColumn)) throw new ArgumentException("列名不能为空。", nameof(sourceColumn));

        var selfKeyCol = this.Columns.Find(selfColumn)
            ?? throw new ArgumentException($"当前记录中不存在列 '{selfColumn}'。", nameof(selfColumn));
        var srcKeyCol = source.Columns.Find(sourceColumn)
            ?? throw new ArgumentException($"来源记录中不存在列 '{sourceColumn}'。", nameof(sourceColumn));

        // 确定需要补充的来源列（排除匹配键列本身，以及当前记录已有的列）
        var enrichCols = BuildEnrichColumns(source, sourceColumn, columnsToEnrich);
        if (enrichCols.Count == 0) return;

        // 为每一行查找匹配，按需追加列定义并填值
        for (int i = 0; i < this.Count; i++)
        {
            var keyValue = selfKeyCol.Get(i);
            var matched = FindRowByKey(source, srcKeyCol, keyValue);
            if (matched == null) continue;

            int srcRow = matched.Value.Row;
            foreach (var srcCol in enrichCols)
            {
                var dstCol = this.Columns.Find(srcCol.Name)
                    ?? this.Columns.Add(srcCol.Name, srcCol.Type);
                dstCol.Set(i, srcCol.Get(srcRow));
            }
        }
    }

    // 收集来源记录中需要补充的列（已存在于当前记录的列跳过；受 columnsToEnrich 过滤）
    private List<RecordColumn> BuildEnrichColumns(
        Record source, string sourceKeyColumn, IReadOnlyList<string>? columnsToEnrich)
    {
        var result = new List<RecordColumn>();
        foreach (var col in source.Columns)
        {
            // 跳过匹配键列
            if (string.Equals(col.Name, sourceKeyColumn, StringComparison.Ordinal)) continue;

            // 如果指定了列名过滤，则只处理其中的列
            if (columnsToEnrich != null && !ContainsName(columnsToEnrich, col.Name)) continue;

            result.Add(col);
        }
        return result;
    }

    // 在 source 中按键列查找第一条匹配行
    private static RecordRow? FindRowByKey(Record source, RecordColumn keyCol, object? keyValue)
    {
        for (int i = 0; i < source.Count; i++)
        {
            var v = keyCol.Get(i);
            if (Equals(v, keyValue)) return new RecordRow(source, i);
        }
        return null;
    }

    private static bool ContainsName(IReadOnlyList<string> list, string name)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (string.Equals(list[i], name, StringComparison.Ordinal)) return true;
        }
        return false;
    }
}
