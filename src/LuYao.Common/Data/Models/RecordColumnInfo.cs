using System;
using System.IO;

namespace LuYao.Data.Models;

/// <summary>
/// 表示记录列的信息，包含列的名称、数据类型代码和实际类型。
/// </summary>
public class RecordColumnInfo
{
    /// <summary>
    /// 初始化 <see cref="RecordColumnInfo"/> 类的新实例。
    /// </summary>
    public RecordColumnInfo()
    {
        this.Name = string.Empty;
        this.Code = RecordDataCode.Object;
        this.Type = typeof(Object);
    }

    /// <summary>
    /// 使用指定的 <see cref="RecordColumn"/> 初始化 <see cref="RecordColumnInfo"/> 类的新实例。
    /// </summary>
    /// <param name="column">用于初始化列信息的记录列对象。</param>
    /// <exception cref="ArgumentNullException">当 <paramref name="column"/> 为 null 时抛出。</exception>
    public RecordColumnInfo(RecordColumn column) : this()
    {
        this.Name = column.Name;
        this.Code = column.Code;
        this.Type = column.Type;
    }

    /// <summary>
    /// 获取或设置列的名称。
    /// </summary>
    /// <value>列的名称字符串。</value>
    public string Name { get; set; }

    /// <summary>
    /// 获取或设置列的数据类型代码。
    /// </summary>
    /// <value>表示列数据类型的 <see cref="RecordDataCode"/> 枚举值。</value>
    public RecordDataCode Code { get; set; }

    /// <summary>
    /// 获取或设置列的实际数据类型。
    /// </summary>
    /// <value>表示列实际数据类型的 <see cref="Type"/> 对象。</value>
    public Type Type { get; set; }

    /// <summary>
    /// 获取类型名称。如果实际类型与代码对应的类型不匹配，返回程序集限定名；否则返回代码字符串表示。
    /// </summary>
    /// <returns>类型名称字符串。如果实际类型与 <see cref="Code"/> 对应的类型不匹配，返回 <see cref="Type.AssemblyQualifiedName"/>；否则返回 <see cref="Code"/> 的字符串表示。</returns>
    public string GetTypeName()
    {
        var t = Helpers.ToType(Code);
        if (t != Type) return Type.AssemblyQualifiedName!;
        return this.Code.ToString();
    }

    /// <summary>
    /// 解析类型名称并设置 <see cref="Type"/> 属性。
    /// </summary>
    /// <param name="text">要解析的类型名称字符串，可以为 null 或空。</param>
    /// <exception cref="InvalidDataException">当无法找到对应的类型时抛出。包含列名和类型信息的详细错误信息。</exception>
    public void ParseTypeName(string? text)
    {
        var type = Helpers.ToType(this.Code);
        if (type == null && !string.IsNullOrWhiteSpace(text)) type = Type.GetType(text, false);
        if (type == null)
        {
            if (string.IsNullOrWhiteSpace(text)) throw new InvalidDataException($"数据列 {this.Name} 的类型信息缺失");
            throw new InvalidDataException($"数据列 {this.Name} 的类型找不到: {text}");
        }
        this.Type = type;
    }
}