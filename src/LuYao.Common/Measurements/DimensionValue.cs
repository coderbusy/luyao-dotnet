namespace LuYao.Measurements;

/// <summary>
/// 表示单个尺寸值。
/// </summary>
public class DimensionValue
{
    /// <summary>
    /// 尺寸数值。
    /// </summary>
    public decimal Value { get; set; }

    /// <summary>
    /// 尺寸类型。
    /// </summary>
    public DimensionKind Kind { get; set; }

    /// <summary>
    /// 初始化 <see cref="DimensionValue"/> 类的新实例。
    /// </summary>
    /// <param name="value">尺寸数值。</param>
    /// <param name="kind">尺寸类型。</param>
    public DimensionValue(decimal value, DimensionKind kind = DimensionKind.Unspecified)
    {
        Value = value;
        Kind = kind;
    }
}
