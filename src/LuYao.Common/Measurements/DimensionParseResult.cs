using System.Collections.Generic;

namespace LuYao.Measurements;

/// <summary>
/// 表示尺寸解析的结果。
/// </summary>
public class DimensionParseResult
{
    /// <summary>
    /// 尺寸单位。
    /// </summary>
    public DimensionUnit Unit { get; set; }

    /// <summary>
    /// 尺寸值列表。
    /// </summary>
    public IReadOnlyList<DimensionValue> Dimensions { get; set; }

    /// <summary>
    /// 初始化 <see cref="DimensionParseResult"/> 类的新实例。
    /// </summary>
    /// <param name="unit">尺寸单位。</param>
    /// <param name="dimensions">尺寸值列表。</param>
    public DimensionParseResult(DimensionUnit unit, IReadOnlyList<DimensionValue> dimensions)
    {
        Unit = unit;
        Dimensions = dimensions;
    }
}
