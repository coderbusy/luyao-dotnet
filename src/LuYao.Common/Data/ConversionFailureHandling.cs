namespace LuYao.Data;

/// <summary>
/// 在 RecordTable 到 DTO 映射时，列值类型转换失败的处理策略。
/// </summary>
public enum ConversionFailureHandling
{
    /// <summary>跳过转换失败的列，对应属性保持默认值（默认）。</summary>
    Skip = 0,

    /// <summary>转换失败时抛出异常。</summary>
    Throw = 1,
}
