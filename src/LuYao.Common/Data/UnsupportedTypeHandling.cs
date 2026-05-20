namespace LuYao.Data;

/// <summary>
/// 在 DTO 到 RecordTable 映射时，遇到不支持的属性类型的处理策略。
/// </summary>
public enum UnsupportedTypeHandling
{
    /// <summary>跳过不支持的属性，不创建对应列（默认）。</summary>
    Skip = 0,

    /// <summary>抛出 <see cref="System.NotSupportedException"/>。</summary>
    Throw = 1,

    /// <summary>
    /// 将属性值转换为 <see cref="string"/> 后存入列。
    /// 必须在 <see cref="RecordMappingOptions"/> 中注册该属性类型对应的 <see cref="RecordConverter"/>，否则将抛出异常。
    /// </summary>
    ConvertToString = 2,

    /// <summary>
    /// 将属性值转换为 <see cref="byte"/>[] 后存入列。
    /// 必须在 <see cref="RecordMappingOptions"/> 中注册该属性类型对应的 <see cref="RecordConverter"/>，否则将抛出异常。
    /// </summary>
    ConvertToBytes = 3,
}
