using System;
using System.Collections.Generic;

namespace LuYao.Data;

public partial class Record
{
    #region Sort

    /// <summary>
    /// 按指定的排序键集合对记录进行原地排序，行为与 SQLite 的 ORDER BY 子句相同。
    /// <para>NULL 值视为最小值：升序时排在最前，降序时排在最后。</para>
    /// <para>多个键相等时保持原有行的相对顺序（稳定排序）。</para>
    /// </summary>
    /// <param name="keys">排序键数组，按优先级从高到低排列，每个键包含列名和排序方向。</param>
    /// <remarks>
    /// 当 <paramref name="keys"/> 为 null 或空数组时直接返回，不修改数据。
    /// </remarks>
    /// <exception cref="KeyNotFoundException">当 <paramref name="keys"/> 中包含不存在的列名时抛出。</exception>
    /// <exception cref="ArgumentException">当 <paramref name="keys"/> 中同一列名出现多次时抛出。</exception>
    public void Sort(params RecordSortKey[] keys)
    {
        if (keys == null || keys.Length == 0) return;

        var resolved = ResolveKeys(keys);

        if (this.Count <= 1) return;

        // 构造行索引数组并排序
        int count = this.Count;
        int[] perm = new int[count];
        for (int i = 0; i < count; i++) perm[i] = i;

        var comparer = BuildComparer(resolved);
        Array.Sort(perm, comparer);

        // 检查是否实际需要重排（全部相等则跳过）
        bool needReorder = false;
        for (int i = 0; i < count; i++)
        {
            if (perm[i] != i) { needReorder = true; break; }
        }
        if (!needReorder) return;

        // 按置换对每列原地重排
        foreach (RecordColumn col in this.Columns)
        {
            col.Reorder(perm);
        }
    }

    /// <summary>
    /// 按 SQL 风格的 ORDER BY 字符串对记录进行原地排序。
    /// <para>语法：<c>列名 [ASC|DESC]</c>，多个键以逗号分隔，列名区分大小写，方向关键字不区分大小写。</para>
    /// <para>示例：<c>"name ASC, age DESC"</c>、<c>"salary"</c>。</para>
    /// </summary>
    /// <param name="orderBy">排序子句字符串，格式与 SQL ORDER BY 相同（不含 ORDER BY 关键字本身）。</param>
    /// <remarks>当 <paramref name="orderBy"/> 为 null 或空白字符串时直接返回，不修改数据。</remarks>
    /// <exception cref="FormatException">当 <paramref name="orderBy"/> 格式无法解析时抛出。</exception>
    public void Sort(string orderBy)
    {
        if (string.IsNullOrWhiteSpace(orderBy)) return;

        var keys = ParseOrderBy(orderBy);
        Sort(keys);
    }

    // ── 私有辅助 ────────────────────────────────────────────────────────────

    private static RecordSortKey[] ParseOrderBy(string orderBy)
    {
        string[] parts = orderBy.Split(',');
        var keys = new RecordSortKey[parts.Length];
        for (int i = 0; i < parts.Length; i++)
        {
            string part = parts[i].Trim();
            if (part.Length == 0)
                throw new FormatException($"排序字符串第 {i + 1} 段为空，请检查格式。");

            // 按空白拆出 1 或 2 个 token
            string[] tokens = part.Split((char[])null!, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 1)
            {
                keys[i] = new RecordSortKey(tokens[0], false);
            }
            else if (tokens.Length == 2)
            {
                bool desc = tokens[1].Equals("DESC", StringComparison.OrdinalIgnoreCase);
                bool asc = tokens[1].Equals("ASC", StringComparison.OrdinalIgnoreCase);
                if (!desc && !asc)
                    throw new FormatException(
                        $"排序方向 '{tokens[1]}' 无法识别，仅支持 ASC 或 DESC。");
                keys[i] = new RecordSortKey(tokens[0], desc);
            }
            else
            {
                throw new FormatException(
                    $"无法解析排序段 '{part}'，期望格式为 '列名' 或 '列名 ASC|DESC'。");
            }
        }
        return keys;
    }

    // net45/net461 没有内置 ValueTuple，用私有 struct 代替
    private readonly struct ResolvedSortKey
    {
        internal readonly RecordColumn Col;
        internal readonly bool Descending;
        internal ResolvedSortKey(RecordColumn col, bool descending) { Col = col; Descending = descending; }
    }

    private ResolvedSortKey[] ResolveKeys(RecordSortKey[] keys)
    {
        var result = new ResolvedSortKey[keys.Length];
        var seen = new HashSet<string>(StringComparer.Ordinal);
        for (int i = 0; i < keys.Length; i++)
        {
            string colName = keys[i].Column;
            if (string.IsNullOrWhiteSpace(colName))
                throw new ArgumentException($"第 {i + 1} 个排序键的列名不能为空。");
            if (!seen.Add(colName))
                throw new ArgumentException($"排序键中列名 '{colName}' 重复。");
            RecordColumn col = this.Columns.Get(colName);   // 列不存在时抛 KeyNotFoundException
            result[i] = new ResolvedSortKey(col, keys[i].Descending);
        }
        return result;
    }

    private static Comparison<int> BuildComparer(ResolvedSortKey[] resolved)
    {
        return (a, b) =>
        {
            for (int k = 0; k < resolved.Length; k++)
            {
                RecordColumn col = resolved[k].Col;
                bool desc = resolved[k].Descending;

                object? va = col.Get(a);
                object? vb = col.Get(b);

                int cmp;
                if (va is null && vb is null)
                {
                    cmp = 0;
                }
                else if (va is null)
                {
                    // null 视为最小值：升序靠前，降序靠后
                    cmp = -1;
                }
                else if (vb is null)
                {
                    cmp = 1;
                }
                else
                {
                    if (va is string sa && vb is string sb)
                    {
                        // SQLite 默认使用二进制（Ordinal）比较
                        cmp = string.Compare(sa, sb, StringComparison.Ordinal);
                    }
                    else if (va is byte[] ba && vb is byte[] bb)
                    {
                        // BLOB：逐字节比较（与 SQLite BLOB 排序行为一致）
                        int len = Math.Min(ba.Length, bb.Length);
                        cmp = 0;
                        for (int j = 0; j < len; j++)
                        {
                            cmp = ba[j].CompareTo(bb[j]);
                            if (cmp != 0) break;
                        }
                        if (cmp == 0) cmp = ba.Length.CompareTo(bb.Length);
                    }
                    else
                    {
                        cmp = Comparer<object>.Default.Compare(va, vb);
                    }
                }

                if (cmp != 0)
                    return desc ? -cmp : cmp;
            }

            // 全部键相等时以原始行号作 tie-breaker，保证稳定性
            return a.CompareTo(b);
        };
    }

    #endregion
}
