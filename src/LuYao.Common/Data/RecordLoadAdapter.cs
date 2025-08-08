using LuYao.Data.Models;
using System;

namespace LuYao.Data;

/// <summary>
/// 表示记录加载适配器的抽象基类。
/// </summary>
/// <remarks>
/// 此类定义了记录数据加载操作的通用接口，支持按名称或索引访问字段，
/// 并提供了读取不同数据类型的方法。具体的实现类需要继承此类并实现所有抽象成员。
/// </remarks>
public abstract class RecordLoadAdapter
{
    /// <summary>
    /// 获取用于标识字段的键类型。
    /// </summary>
    /// <value>表示是通过字段名称还是索引来访问字段的 <see cref="RecordLoadKeyKind"/> 枚举值。</value>
    public abstract RecordLoadKeyKind KeyKind { get; }

    /// <summary>
    /// 获取当前字段的索引位置。
    /// </summary>
    /// <value>从 0 开始的字段索引值。</value>
    /// <remarks>
    /// 当 <see cref="KeyKind"/> 为 <see cref="RecordLoadKeyKind.Index"/> 时，此属性用于定位字段。
    /// </remarks>
    public abstract int Index { get; }

    /// <summary>
    /// 获取当前字段的名称。
    /// </summary>
    /// <value>字段的名称字符串。</value>
    /// <remarks>
    /// 当 <see cref="KeyKind"/> 为 <see cref="RecordLoadKeyKind.Name"/> 时，此属性用于定位字段。
    /// </remarks>
    public abstract string Name { get; }

    /// <summary>
    /// 读取下一个记录项。
    /// </summary>
    /// <returns>如果成功读取到记录项则返回 <c>true</c>；否则返回 <c>false</c>。</returns>
    public abstract bool Read();

    /// <summary>
    /// 获取当前读取位置所在的记录区段。
    /// </summary>
    /// <value>表示当前区段的 <see cref="RecordSection"/> 枚举值。</value>
    public abstract RecordSection Section { get; }

    /// <summary>
    /// 读取记录头信息。
    /// </summary>
    /// <returns>包含记录模式信息的 <see cref="RecordHeader"/> 对象。</returns>
    public abstract RecordHeader ReadHeader();

    /// <summary>
    /// 读取列信息。
    /// </summary>
    /// <returns>包含列定义信息的 <see cref="RecordColumnInfo"/> 对象。</returns>
    public abstract RecordColumnInfo ReadColumn();

    /// <summary>
    /// 读取下一行记录。
    /// </summary>
    /// <returns>如果成功读取到行记录则返回 <c>true</c>；否则返回 <c>false</c>。</returns>
    public abstract bool ReadRow();

    /// <summary>
    /// 读取下一个字段。
    /// </summary>
    /// <returns>如果成功读取到字段则返回 <c>true</c>；否则返回 <c>false</c>。</returns>
    public abstract bool ReadField();

    /// <summary>
    /// 读取当前字段的值并转换为布尔值。
    /// </summary>
    /// <returns>转换后的布尔值。</returns>
    /// <exception cref="InvalidCastException">当字段值无法转换为布尔值时抛出。</exception>
    public abstract bool ReadBoolean();

    /// <summary>
    /// 读取当前字段的值并转换为字节值。
    /// </summary>
    /// <returns>转换后的字节值。</returns>
    /// <exception cref="InvalidCastException">当字段值无法转换为字节值时抛出。</exception>
    public abstract byte ReadByte();

    /// <summary>
    /// 读取当前字段的值并转换为字符值。
    /// </summary>
    /// <returns>转换后的字符值。</returns>
    /// <exception cref="InvalidCastException">当字段值无法转换为字符值时抛出。</exception>
    public abstract char ReadChar();

    /// <summary>
    /// 读取当前字段的值并转换为日期时间值。
    /// </summary>
    /// <returns>转换后的日期时间值。</returns>
    /// <exception cref="InvalidCastException">当字段值无法转换为日期时间值时抛出。</exception>
    public abstract DateTime ReadDateTime();

    /// <summary>
    /// 读取当前字段的值并转换为十进制数值。
    /// </summary>
    /// <returns>转换后的十进制数值。</returns>
    /// <exception cref="InvalidCastException">当字段值无法转换为十进制数值时抛出。</exception>
    public abstract decimal ReadDecimal();

    /// <summary>
    /// 读取当前字段的值并转换为双精度浮点数值。
    /// </summary>
    /// <returns>转换后的双精度浮点数值。</returns>
    /// <exception cref="InvalidCastException">当字段值无法转换为双精度浮点数值时抛出。</exception>
    public abstract double ReadDouble();

    /// <summary>
    /// 读取当前字段的值并转换为16位有符号整数值。
    /// </summary>
    /// <returns>转换后的16位有符号整数值。</returns>
    /// <exception cref="InvalidCastException">当字段值无法转换为16位有符号整数值时抛出。</exception>
    public abstract short ReadInt16();

    /// <summary>
    /// 读取当前字段的值并转换为32位有符号整数值。
    /// </summary>
    /// <returns>转换后的32位有符号整数值。</returns>
    /// <exception cref="InvalidCastException">当字段值无法转换为32位有符号整数值时抛出。</exception>
    public abstract int ReadInt32();

    /// <summary>
    /// 读取当前字段的值并转换为64位有符号整数值。
    /// </summary>
    /// <returns>转换后的64位有符号整数值。</returns>
    /// <exception cref="InvalidCastException">当字段值无法转换为64位有符号整数值时抛出。</exception>
    public abstract long ReadInt64();

    /// <summary>
    /// 读取当前字段的值并转换为8位有符号整数值。
    /// </summary>
    /// <returns>转换后的8位有符号整数值。</returns>
    /// <exception cref="InvalidCastException">当字段值无法转换为8位有符号整数值时抛出。</exception>
    public abstract sbyte ReadSByte();

    /// <summary>
    /// 读取当前字段的值并转换为单精度浮点数值。
    /// </summary>
    /// <returns>转换后的单精度浮点数值。</returns>
    /// <exception cref="InvalidCastException">当字段值无法转换为单精度浮点数值时抛出。</exception>
    public abstract float ReadSingle();

    /// <summary>
    /// 读取当前字段的值并转换为字符串。
    /// </summary>
    /// <returns>转换后的字符串值，可能为 null。</returns>
    public abstract string ReadString();

    /// <summary>
    /// 读取当前字段的值并转换为16位无符号整数值。
    /// </summary>
    /// <returns>转换后的16位无符号整数值。</returns>
    /// <exception cref="InvalidCastException">当字段值无法转换为16位无符号整数值时抛出。</exception>
    public abstract ushort ReadUInt16();

    /// <summary>
    /// 读取当前字段的值并转换为32位无符号整数值。
    /// </summary>
    /// <returns>转换后的32位无符号整数值。</returns>
    /// <exception cref="InvalidCastException">当字段值无法转换为32位无符号整数值时抛出。</exception>
    public abstract uint ReadUInt32();

    /// <summary>
    /// 读取当前字段的值并转换为64位无符号整数值。
    /// </summary>
    /// <returns>转换后的64位无符号整数值。</returns>
    /// <exception cref="InvalidCastException">当字段值无法转换为64位无符号整数值时抛出。</exception>
    public abstract ulong ReadUInt64();

    /// <summary>
    /// 读取当前字段的值并转换为指定类型的对象。
    /// </summary>
    /// <param name="type">目标类型对象。</param>
    /// <returns>转换后的对象，可能为 null。</returns>
    /// <exception cref="ArgumentNullException">当 <paramref name="type"/> 为 null 时抛出。</exception>
    /// <exception cref="InvalidCastException">当字段值无法转换为指定类型时抛出。</exception>
    public abstract object? ReadObject(object type);
}