using System;

namespace LuYao.Data;

/// <summary>
/// 聚合函数类型。
/// </summary>
public enum AggregateFunction
{
    /// <summary>计数</summary>
    Count,
    /// <summary>求和</summary>
    Sum,
    /// <summary>最小值</summary>
    Min,
    /// <summary>最大值</summary>
    Max,
    /// <summary>平均值</summary>
    Avg
}

/// <summary>
/// 聚合定义，描述对哪一列应用哪种聚合函数以及输出列名。
/// </summary>
public sealed class AggregateDefinition
{
    /// <summary>
    /// 源列名。对于 <see cref="AggregateFunction.Count"/> 可为 null（表示 COUNT(*)）。
    /// </summary>
    public string? SourceColumn { get; }

    /// <summary>
    /// 聚合函数类型。
    /// </summary>
    public AggregateFunction Function { get; }

    /// <summary>
    /// 输出列名。
    /// </summary>
    public string OutputColumn { get; }

    /// <summary>
    /// 创建聚合定义。
    /// </summary>
    /// <param name="function">聚合函数类型。</param>
    /// <param name="sourceColumn">源列名（Count 可为 null）。</param>
    /// <param name="outputColumn">输出列名。</param>
    public AggregateDefinition(AggregateFunction function, string? sourceColumn, string outputColumn)
    {
        if (string.IsNullOrWhiteSpace(outputColumn)) throw new ArgumentException("输出列名不能为空", nameof(outputColumn));
        if (function != AggregateFunction.Count && string.IsNullOrWhiteSpace(sourceColumn))
            throw new ArgumentException("非 Count 聚合必须指定源列名", nameof(sourceColumn));
        Function = function;
        SourceColumn = sourceColumn;
        OutputColumn = outputColumn;
    }

    /// <summary>创建 COUNT(*) 聚合。</summary>
    public static AggregateDefinition Count(string outputColumn = "Count")
        => new AggregateDefinition(AggregateFunction.Count, null, outputColumn);

    /// <summary>创建 COUNT(column) 聚合。</summary>
    public static AggregateDefinition CountOf(string sourceColumn, string? outputColumn = null)
        => new AggregateDefinition(AggregateFunction.Count, sourceColumn, outputColumn ?? $"Count_{sourceColumn}");

    /// <summary>创建 SUM 聚合。</summary>
    public static AggregateDefinition Sum(string sourceColumn, string? outputColumn = null)
        => new AggregateDefinition(AggregateFunction.Sum, sourceColumn, outputColumn ?? $"Sum_{sourceColumn}");

    /// <summary>创建 MIN 聚合。</summary>
    public static AggregateDefinition Min(string sourceColumn, string? outputColumn = null)
        => new AggregateDefinition(AggregateFunction.Min, sourceColumn, outputColumn ?? $"Min_{sourceColumn}");

    /// <summary>创建 MAX 聚合。</summary>
    public static AggregateDefinition Max(string sourceColumn, string? outputColumn = null)
        => new AggregateDefinition(AggregateFunction.Max, sourceColumn, outputColumn ?? $"Max_{sourceColumn}");

    /// <summary>创建 AVG 聚合。</summary>
    public static AggregateDefinition Avg(string sourceColumn, string? outputColumn = null)
        => new AggregateDefinition(AggregateFunction.Avg, sourceColumn, outputColumn ?? $"Avg_{sourceColumn}");
}
