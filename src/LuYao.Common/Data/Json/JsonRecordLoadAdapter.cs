using System;
using System.Collections.Generic;
using System.Globalization;
using LuYao.Data.Models;
using LuYao.Text.Json;

namespace LuYao.Data.Json;

/// <summary>
/// 提供从JSON格式流中读取记录数据的适配器实现。
/// </summary>
/// <remarks>
/// 该类继承自 <see cref="RecordLoadAdapter"/>，专门用于从JSON格式反序列化记录数据。
/// 它使用 <see cref="JsonReader"/> 从底层流中读取各种数据类型。
/// JSON结构包含header、columns和rows等部分。
/// </remarks>
public class JsonRecordLoadAdapter : RecordLoadAdapter
{
    private RecordSection _section = RecordSection.Head;
    private string _name = string.Empty;
    private readonly Dictionary<string, object?> _currentRowData = new();
    private readonly List<string> _currentRowKeys = new();
    private int _currentFieldIndex = -1;

    /// <summary>
    /// 获取用于读取JSON数据的 <see cref="JsonReader"/> 实例。
    /// </summary>
    /// <value>用于执行JSON读取操作的 <see cref="JsonReader"/> 对象。</value>
    public JsonReader Reader { get; }

    /// <inheritdoc/>
    public override RecordLoadKeyKind KeyKind => RecordLoadKeyKind.Name;

    /// <inheritdoc/>
    public override RecordSection Section => _section;

    /// <inheritdoc/>
    public override int Index => -1;

    /// <inheritdoc/>
    public override string Name => _name;

    /// <summary>
    /// 使用指定的JSON读取器初始化 <see cref="JsonRecordLoadAdapter"/> 类的新实例。
    /// </summary>
    /// <param name="reader">用于读取JSON数据的 <see cref="JsonReader"/> 实例。</param>
    /// <exception cref="ArgumentNullException">当 <paramref name="reader"/> 为 null 时抛出。</exception>
    public JsonRecordLoadAdapter(JsonReader reader)
    {
        Reader = reader ?? throw new ArgumentNullException(nameof(reader));
    }

    /// <inheritdoc/>
    public override bool ReadSection()
    {
        while (Reader.Read())
        {
            if (Reader.TokenType == JsonTokenType.PropertyName)
            {
                string sectionName = Reader.Value?.ToString()?.ToLowerInvariant() ?? string.Empty;
                switch (sectionName)
                {
                    case "head":
                        _section = RecordSection.Head;
                        return true;
                    case "columns":
                        _section = RecordSection.Columns;
                        Reader.Read();
                        return true;
                    case "rows":
                        _section = RecordSection.Rows;
                        Reader.Read();
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

        while (Reader.Read())
        {
            if (Reader.TokenType == JsonTokenType.StartObject)
            {
                continue;
            }
            else if (Reader.TokenType == JsonTokenType.EndObject)
            {
                break;
            }
            else if (Reader.TokenType == JsonTokenType.PropertyName)
            {
                string propertyName = Reader.Value?.ToString() ?? string.Empty;
                Reader.Read(); // Move to value

                string value = Reader.Value?.ToString() ?? string.Empty;
                switch (propertyName)
                {
                    case "name": header.Name = value; break;
                    case "columns": header.Columns = Valid.ToInt32(value); break;
                    case "count": header.Count = Valid.ToInt32(value); break;
                }
            }
        }

        return header;
    }

    /// <inheritdoc/>
    public override RecordColumnInfo ReadColumn()
    {
        var ret = new RecordColumnInfo { Type = typeof(Object) };

        // Expect to be at the start of column object or move to next one
        while (Reader.Read())
        {
            if (Reader.TokenType == JsonTokenType.StartObject)
            {
                break;
            }
            if (Reader.TokenType == JsonTokenType.EndArray)
            {
                return ret; // No more columns
            }
        }

        while (Reader.Read())
        {
            if (Reader.TokenType == JsonTokenType.EndObject)
            {
                break;
            }

            if (Reader.TokenType == JsonTokenType.PropertyName)
            {
                string propertyName = Reader.Value?.ToString() ?? string.Empty;
                Reader.Read(); // Move to value

                string value = Reader.Value?.ToString() ?? string.Empty;
                switch (propertyName)
                {
                    case "name": ret.Name = value; break;
                    case "code": ret.Code = Enum<RecordDataCode>.Parse(value); break;
                    case "type": ret.ParseTypeName(value); break;
                }
            }
        }

        return ret;
    }

    /// <inheritdoc/>
    public override bool ReadRow()
    {
        _currentRowData.Clear();
        _currentRowKeys.Clear();
        _currentFieldIndex = -1;

        // Look for next object in rows array
        while (Reader.Read())
        {
            if (Reader.TokenType == JsonTokenType.StartObject)
            {
                // Read all properties of this row object
                while (Reader.Read())
                {
                    if (Reader.TokenType == JsonTokenType.EndObject)
                    {
                        break;
                    }

                    if (Reader.TokenType == JsonTokenType.PropertyName)
                    {
                        string propertyName = Reader.Value?.ToString() ?? string.Empty;
                        Reader.Read(); // Move to value

                        _currentRowData[propertyName] = Reader.Value;
                        _currentRowKeys.Add(propertyName);
                    }
                }
                return true;
            }
            else if (Reader.TokenType == JsonTokenType.EndArray)
            {
                return false; // End of rows array
            }
        }
        return false;
    }

    /// <inheritdoc/>
    public override bool ReadField()
    {
        _currentFieldIndex++;
        if (_currentFieldIndex < _currentRowKeys.Count)
        {
            _name = _currentRowKeys[_currentFieldIndex];
            return true;
        }
        return false;
    }

    private bool TryGetCurrentValue(out object? value)
    {
        value = null;
        if (_currentFieldIndex >= 0 && _currentFieldIndex < _currentRowKeys.Count)
        {
            string fieldName = _currentRowKeys[_currentFieldIndex];
            return _currentRowData.TryGetValue(fieldName, out value);
        }
        return false;
    }

    /// <inheritdoc/>
    public override bool ReadBoolean()
    {
        if (TryGetCurrentValue(out var value))
        {
            if (value is bool boolValue)
                return boolValue;
            return Valid.ToBoolean(value?.ToString() ?? string.Empty);
        }
        return default;
    }

    /// <inheritdoc/>
    public override byte ReadByte()
    {
        if (TryGetCurrentValue(out var value))
        {
            if (value is long longValue)
                return (byte)longValue;
            return Valid.ToByte(value?.ToString() ?? string.Empty);
        }
        return default;
    }

    /// <inheritdoc/>
    public override char ReadChar()
    {
        if (TryGetCurrentValue(out var value))
        {
            return Valid.ToChar(value?.ToString() ?? string.Empty);
        }
        return default;
    }

    /// <inheritdoc/>
    public override DateTime ReadDateTime()
    {
        if (TryGetCurrentValue(out var value))
        {
            return Valid.ToDateTime(value?.ToString() ?? string.Empty);
        }
        return default;
    }

    /// <inheritdoc/>
    public override decimal ReadDecimal()
    {
        if (TryGetCurrentValue(out var value))
        {
            if (value is double doubleValue)
                return (decimal)doubleValue;
            return Valid.ToDecimal(value?.ToString() ?? string.Empty);
        }
        return default;
    }

    /// <inheritdoc/>
    public override double ReadDouble()
    {
        if (TryGetCurrentValue(out var value))
        {
            if (value is double doubleValue)
                return doubleValue;
            return Valid.ToDouble(value?.ToString() ?? string.Empty);
        }
        return default;
    }

    /// <inheritdoc/>
    public override short ReadInt16()
    {
        if (TryGetCurrentValue(out var value))
        {
            if (value is long longValue)
                return (short)longValue;
            return Valid.ToInt16(value?.ToString() ?? string.Empty);
        }
        return default;
    }

    /// <inheritdoc/>
    public override int ReadInt32()
    {
        if (TryGetCurrentValue(out var value))
        {
            if (value is long longValue)
                return (int)longValue;
            return Valid.ToInt32(value?.ToString() ?? string.Empty);
        }
        return default;
    }

    /// <inheritdoc/>
    public override long ReadInt64()
    {
        if (TryGetCurrentValue(out var value))
        {
            if (value is long longValue)
                return longValue;
            return Valid.ToInt64(value?.ToString() ?? string.Empty);
        }
        return default;
    }

    /// <inheritdoc/>
    public override sbyte ReadSByte()
    {
        if (TryGetCurrentValue(out var value))
        {
            if (value is long longValue)
                return (sbyte)longValue;
            return Valid.ToSByte(value?.ToString() ?? string.Empty);
        }
        return default;
    }

    /// <inheritdoc/>
    public override float ReadSingle()
    {
        if (TryGetCurrentValue(out var value))
        {
            if (value is double doubleValue)
                return (float)doubleValue;
            return Valid.ToSingle(value?.ToString() ?? string.Empty);
        }
        return default;
    }

    /// <inheritdoc/>
    public override string ReadString()
    {
        if (TryGetCurrentValue(out var value))
        {
            return value?.ToString() ?? string.Empty;
        }
        return string.Empty;
    }

    /// <inheritdoc/>
    public override ushort ReadUInt16()
    {
        if (TryGetCurrentValue(out var value))
        {
            if (value is long longValue)
                return (ushort)longValue;
            return Valid.ToUInt16(value?.ToString() ?? string.Empty);
        }
        return default;
    }

    /// <inheritdoc/>
    public override uint ReadUInt32()
    {
        if (TryGetCurrentValue(out var value))
        {
            if (value is long longValue)
                return (uint)longValue;
            return Valid.ToUInt32(value?.ToString() ?? string.Empty);
        }
        return default;
    }

    /// <inheritdoc/>
    public override ulong ReadUInt64()
    {
        if (TryGetCurrentValue(out var value))
        {
            if (value is long longValue)
                return (ulong)longValue;
            return Valid.ToUInt64(value?.ToString() ?? string.Empty);
        }
        return default;
    }

    /// <exception cref="NotImplementedException">复杂类型的JSON读写暂不支持。</exception>
    public override object? ReadObject(object type)
    {
        throw new NotImplementedException("复杂类型的JSON读写暂不支持");
    }
}