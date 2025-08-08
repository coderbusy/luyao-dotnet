using LuYao.Data.Models;
using System;
using System.Collections.Generic;
using System.Xml;

namespace LuYao.Data.Xml;

/// <summary>
/// 提供从XML格式流中读取记录数据的适配器实现。
/// </summary>
/// <remarks>
/// 该类继承自 <see cref="RecordLoadAdapter"/>，专门用于从XML格式反序列化记录数据。
/// 它使用 <see cref="XmlReader"/> 从底层流中读取各种数据类型。
/// XML结构包含header和row元素，其他数据均存储在attribute中。
/// </remarks>
public class XmlRecordLoadAdapter : RecordLoadAdapter
{
    private RecordSection _section = RecordSection.Head;
    private string _name = string.Empty;

    /// <summary>
    /// 获取用于读取XML数据的 <see cref="XmlReader"/> 实例。
    /// </summary>
    /// <value>用于执行XML读取操作的 <see cref="XmlReader"/> 对象。</value>
    public XmlReader Reader { get; }

    /// <inheritdoc/>
    public override RecordLoadKeyKind KeyKind => RecordLoadKeyKind.Name;

    /// <inheritdoc/>
    public override RecordSection Section => _section;

    /// <inheritdoc/>
    public override int Index => -1;

    /// <inheritdoc/>
    public override string Name => _name;


    /// <summary>
    /// 使用指定的XML读取器初始化 <see cref="XmlRecordLoadAdapter"/> 类的新实例。
    /// </summary>
    /// <param name="reader">用于读取XML数据的 <see cref="XmlReader"/> 实例。</param>
    /// <exception cref="ArgumentNullException">当 <paramref name="reader"/> 为 null 时抛出。</exception>
    public XmlRecordLoadAdapter(XmlReader reader)
    {
        Reader = reader ?? throw new ArgumentNullException(nameof(reader));
    }

    /// <inheritdoc/>
    public override bool Read()
    {
        while (Reader.Read())
        {
            if (Reader.NodeType == XmlNodeType.Element)
            {
                switch (Reader.LocalName.ToLowerInvariant())
                {
                    case "head":
                        _section = RecordSection.Head;
                        return true;
                    case "columns":
                        _section = RecordSection.Columns;
                        return true;
                    case "rows":
                        _section = RecordSection.Rows;
                        return true;
                }
            }
        }
        return false;
    }

    /// <inheritdoc/>
    public override RecordHeader ReadHeader()
    {
        var header = new RecordHeader();

        while (this.Reader.Read())
        {
            if (this.Reader.NodeType == XmlNodeType.Element)
            {
                break;
            }
        }
        if (this.Reader.LocalName == "header" && this.Reader.HasAttributes)
        {
            while (this.Reader.MoveToNextAttribute())
            {
                string n = this.Reader.Name;
                string v = this.Reader.Value;
                switch (n)
                {
                    case "name": header.Name = v; break;
                    case "columns": header.Columns = Valid.ToInt32(v); break;
                    case "count": header.Count = Valid.ToInt32(v); break;
                }
            }
        }

        return header;
    }

    /// <inheritdoc/>
    public override RecordColumnInfo ReadColumn()
    {
        var ret = new RecordColumnInfo { Type = typeof(Object) };
        while (this.Reader.Read())
        {
            if (this.Reader.NodeType == XmlNodeType.Element)
            {
                break;
            }
        }
        if (this.Reader.LocalName == "column" && this.Reader.HasAttributes)
        {
            while (this.Reader.MoveToNextAttribute())
            {
                string n = this.Reader.Name;
                string v = this.Reader.Value;
                switch (n)
                {
                    case "name": ret.Name = v; break;
                    case "code":
                        {
                            ret.Code = Enum<RecordDataCode>.Parse(v);
                            ret.Type = Helpers.ToType(ret.Code) ?? typeof(Object);
                        }
                        break;
                }
            }
        }
        return ret;
    }

    /// <inheritdoc/>
    public override bool ReadRow()
    {
        while (this.Reader.Read())
        {
            if (this.Reader.NodeType != XmlNodeType.Element) continue;
            if (this.Reader.LocalName != "row") continue;
            if (this.Reader.HasAttributes == false) continue;
            return true;
        }
        return false;
    }

    /// <inheritdoc/>
    public override bool ReadField()
    {
        if (this.Reader.MoveToNextAttribute())
        {
            _name = this.Reader.Name;
            return true;
        }
        return false;
    }
    private bool TryGetCurrentValue(out string value)
    {
        if (this.Reader.NodeType == XmlNodeType.Attribute)
        {
            value = this.Reader.Value;
            return true;
        }
        value = string.Empty;
        return false;
    }

    /// <inheritdoc/>
    public override bool ReadBoolean() => TryGetCurrentValue(out var value) ? Valid.ToBoolean(value) : default;


    /// <inheritdoc/>
    public override byte ReadByte() => TryGetCurrentValue(out var value) ? Valid.ToByte(value) : default;

    /// <inheritdoc/>
    public override char ReadChar() => TryGetCurrentValue(out var value) ? Valid.ToChar(value) : default;

    /// <inheritdoc/>
    public override DateTime ReadDateTime()
    {
        if (!TryGetCurrentValue(out var value)) return default;
        return Valid.ToDateTime(value);
    }

    /// <inheritdoc/>
    public override decimal ReadDecimal() => TryGetCurrentValue(out var value) ? Valid.ToDecimal(value) : default;

    /// <inheritdoc/>
    public override double ReadDouble() => TryGetCurrentValue(out var value) ? Valid.ToDouble(value) : default;

    /// <inheritdoc/>
    public override short ReadInt16() => TryGetCurrentValue(out var value) ? Valid.ToInt16(value) : default;

    /// <inheritdoc/>
    public override int ReadInt32() => TryGetCurrentValue(out var value) ? Valid.ToInt32(value) : default;

    /// <inheritdoc/>
    public override long ReadInt64() => TryGetCurrentValue(out var value) ? Valid.ToInt64(value) : default;

    /// <inheritdoc/>
    public override sbyte ReadSByte() => TryGetCurrentValue(out var value) ? Valid.ToSByte(value) : default;

    /// <inheritdoc/>
    public override float ReadSingle() => TryGetCurrentValue(out var value) ? Valid.ToSingle(value) : default;

    /// <inheritdoc/>
    public override string ReadString() => TryGetCurrentValue(out var value) ? value : string.Empty;

    /// <inheritdoc/>
    public override ushort ReadUInt16() => TryGetCurrentValue(out var value) ? Valid.ToUInt16(value) : default;

    /// <inheritdoc/>
    public override uint ReadUInt32() => TryGetCurrentValue(out var value) ? Valid.ToUInt32(value) : default;

    /// <inheritdoc/>
    public override ulong ReadUInt64() => TryGetCurrentValue(out var value) ? Valid.ToUInt64(value) : default;

    /// <exception cref="NotImplementedException">复杂类型的XML读写暂不支持。</exception>
    public override object? ReadObject(object type)
    {
        throw new NotImplementedException("复杂类型的XML读写暂不支持");
    }
}