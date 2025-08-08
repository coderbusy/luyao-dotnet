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
    private readonly Dictionary<string, string> _currentAttributes = new();
    private readonly Queue<string> _attributeNames = new();

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
                    case "header":
                        _section = RecordSection.Head;
                        LoadCurrentAttributes();
                        return true;
                    case "column":
                        _section = RecordSection.Columns;
                        LoadCurrentAttributes();
                        return true;
                    case "row":
                        _section = RecordSection.Rows;
                        LoadCurrentAttributes();
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

        if (_currentAttributes.TryGetValue("name", out var name)) header.Name = name;
        if (_currentAttributes.TryGetValue("columns", out var columns)) header.Columns = Valid.ToInt32(columns);
        if (_currentAttributes.TryGetValue("count", out var count)) header.Count = Valid.ToInt32(count);

        return header;
    }

    /// <inheritdoc/>
    public override RecordColumnInfo ReadColumn()
    {
        var column = new RecordColumnInfo();

        if (_currentAttributes.TryGetValue("name", out var name)) column.Name = name;

        if (_currentAttributes.TryGetValue("code", out var codeStr))
        {
            if (Enum.TryParse<RecordDataCode>(codeStr, out var code)) column.Code = code;
        }

        if (_currentAttributes.TryGetValue("type", out var typeStr))
        {
            if (Enum.TryParse<RecordDataCode>(typeStr, out var typeCode))
            {
                column.Type = Helpers.ToType(typeCode) ?? typeof(object);
            }
            else
            {
                // 尝试解析为完整的类型名
                try
                {
                    column.Type = Type.GetType(typeStr) ?? typeof(object);
                }
                catch
                {
                    column.Type = typeof(object);
                }
            }
        }
        else
        {
            column.Type = Helpers.ToType(column.Code) ?? typeof(object);
        }

        return column;
    }

    /// <inheritdoc/>
    public override bool ReadRow()
    {
        if (Reader.LocalName == "row" && Reader.NodeType == XmlNodeType.Element)
        {
            LoadCurrentAttributes();
            PrepareFieldEnumeration();
            return true;
        }
        return false;
    }

    /// <inheritdoc/>
    public override bool ReadField()
    {
        if (_attributeNames.Count > 0)
        {
            _name = _attributeNames.Dequeue();
            return true;
        }
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

    private void LoadCurrentAttributes()
    {
        _currentAttributes.Clear();

        if (Reader.HasAttributes)
        {
            while (Reader.MoveToNextAttribute())
            {
                _currentAttributes[Reader.Name] = Reader.Value;
            }
        }
    }

    private void PrepareFieldEnumeration()
    {
        _attributeNames.Clear();

        foreach (var attributeName in _currentAttributes.Keys)
        {
            _attributeNames.Enqueue(attributeName);
        }
    }

    /// <summary>
    /// 尝试获取当前字段的值。
    /// </summary>
    /// <param name="value">当前字段的值，如果字段不存在则为 null。</param>
    /// <returns>如果成功获取到字段值则返回 <c>true</c>；否则返回 <c>false</c>。</returns>
    private bool TryGetCurrentValue(out string? value)
    {
        return _currentAttributes.TryGetValue(Name, out value);
    }
}