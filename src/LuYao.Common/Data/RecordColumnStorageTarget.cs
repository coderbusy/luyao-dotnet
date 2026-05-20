namespace LuYao.Data;

/// <summary>
/// 指定 DTO 属性在 <see cref="RecordTable"/> 中的列存储类型。
/// 当属性类型不是 RecordTable 原生支持的类型时，由此决定如何存储。
/// </summary>
public enum RecordColumnStorageTarget
{
    /// <summary>
    /// 跳过该属性，不在 RecordTable 中创建对应的列。
    /// </summary>
    Skip = 0,

    /// <summary>
    /// 将属性值转换为 <see cref="string"/> 后存储。
    /// 必须在 <see cref="RecordMappingOptions"/> 中注册该属性类型对应的 <see cref="RecordConverter"/>，否则将抛出异常。
    /// </summary>
    String = 1,

    /// <summary>
    /// 将属性值转换为 <see cref="byte"/>[] 后存储。
    /// 必须在 <see cref="RecordMappingOptions"/> 中注册该属性类型对应的 <see cref="RecordConverter"/>，否则将抛出异常。
    /// </summary>
    Bytes = 2,
}
