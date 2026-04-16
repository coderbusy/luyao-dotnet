using System;

namespace LuYao.Data;

/// <summary>
/// 对象映射选项，控制 <see cref="Record"/> 与 CLR 对象之间的映射行为。
/// </summary>
public class RecordMappingOptions
{
    /// <summary>
    /// 名称比较策略（默认 <see cref="StringComparison.OrdinalIgnoreCase"/>）。
    /// </summary>
    public StringComparison NameComparison { get; set; } = StringComparison.OrdinalIgnoreCase;

    /// <summary>
    /// 属性名 → 列名 单向转换函数（默认 null，不转换）。
    /// </summary>
    public Func<string, string>? NameTransform { get; set; }

    /// <summary>
    /// AddRow 时是否自动添加不存在的列（默认 false）。
    /// </summary>
    public bool AutoAddColumns { get; set; }

    /// <summary>
    /// ToList/To 时是否要求 T 的所有属性都有对应列（默认 false）。
    /// </summary>
    public bool RequireAllProperties { get; set; }

    /// <summary>
    /// 自定义写入转换：(列名, 列类型, 属性值) → 列值。
    /// 设置 <see cref="Mapper"/> 时此钩子被忽略。
    /// </summary>
    public Func<string, Type, object?, object?>? SerializeValue { get; set; }

    /// <summary>
    /// 自定义读取转换：(列名, 属性类型, 列值) → 属性值。
    /// 设置 <see cref="Mapper"/> 时此钩子被忽略。
    /// </summary>
    public Func<string, Type, object?, object?>? DeserializeValue { get; set; }

    /// <summary>
    /// 自定义映射器，优先级最高。设置后内置映射逻辑全部跳过。
    /// 与 <see cref="SerializeValue"/>/<see cref="DeserializeValue"/> 互斥。
    /// </summary>
    public IRecordMapper? Mapper { get; set; }
}
