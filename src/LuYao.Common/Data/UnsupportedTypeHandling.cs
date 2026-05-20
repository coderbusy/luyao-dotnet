namespace LuYao.Data;

/// <summary>
/// 在 DTO 到 RecordTable 映射时，遇到不支持的属性类型的处理策略。
/// </summary>
public enum UnsupportedTypeHandling
{
    /// <summary>跳过不支持的属性（默认）。</summary>
    Skip = 0,

    /// <summary>抛出 <see cref="System.NotSupportedException"/>。</summary>
    Throw = 1,
}
