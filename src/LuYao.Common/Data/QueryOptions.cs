using System;
using System.Collections.Generic;

namespace LuYao.Data;

/// <summary>
/// 查询选项，用于控制 <see cref="RecordQuery"/> 的执行行为。
/// </summary>
public class QueryOptions
{
    /// <summary>
    /// 获取或设置是否启用索引优化。
    /// </summary>
    public bool EnableIndexing { get; set; }

    /// <summary>
    /// 获取或设置显式声明的索引列（支持单列和复合列）。
    /// 每个元素为一组列名，代表一个索引。
    /// </summary>
    public IReadOnlyList<string[]>? Indexes { get; set; }

    /// <summary>
    /// 获取或设置字符串比较策略（用于键比较与筛选）。
    /// </summary>
    public StringComparison StringComparison { get; set; } = StringComparison.Ordinal;
}
