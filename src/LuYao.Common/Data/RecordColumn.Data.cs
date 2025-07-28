using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Data;

partial class RecordColumn
{
    /// <inheritdoc/>
    public void Set(Boolean value, int index)
    {
        if (index < 0 || index >= _table.Count)
            throw new ArgumentOutOfRangeException(nameof(index), $"行索引 {index} 超出有效范围 [0, {_table.Count - 1}]");
        if (_table.Count == 0) _table.AddRow();
        this.Data.Set(value, index);
    }

    /// <inheritdoc/>
    public void Set(Byte value, int index)
    {
        if (index < 0 || index >= _table.Count)
            throw new ArgumentOutOfRangeException(nameof(index), $"行索引 {index} 超出有效范围 [0, {_table.Count - 1}]");
        if (_table.Count == 0) _table.AddRow();
        this.Data.Set(value, index);
    }

    /// <inheritdoc/>
    public void Set(Char value, int index)
    {
        if (index < 0 || index >= _table.Count)
            throw new ArgumentOutOfRangeException(nameof(index), $"行索引 {index} 超出有效范围 [0, {_table.Count - 1}]");
        if (_table.Count == 0) _table.AddRow();
        this.Data.Set(value, index);
    }

    /// <inheritdoc/>
    public void Set(DateTime value, int index)
    {
        if (index < 0 || index >= _table.Count)
            throw new ArgumentOutOfRangeException(nameof(index), $"行索引 {index} 超出有效范围 [0, {_table.Count - 1}]");
        if (_table.Count == 0) _table.AddRow();
        this.Data.Set(value, index);
    }

    /// <inheritdoc/>
    public void Set(Decimal value, int index)
    {
        if (index < 0 || index >= _table.Count)
            throw new ArgumentOutOfRangeException(nameof(index), $"行索引 {index} 超出有效范围 [0, {_table.Count - 1}]");
        if (_table.Count == 0) _table.AddRow();
        this.Data.Set(value, index);
    }

    /// <inheritdoc/>
    public void Set(Double value, int index)
    {
        if (index < 0 || index >= _table.Count)
            throw new ArgumentOutOfRangeException(nameof(index), $"行索引 {index} 超出有效范围 [0, {_table.Count - 1}]");
        if (_table.Count == 0) _table.AddRow();
        this.Data.Set(value, index);
    }

    /// <inheritdoc/>
    public void Set(Int16 value, int index)
    {
        if (index < 0 || index >= _table.Count)
            throw new ArgumentOutOfRangeException(nameof(index), $"行索引 {index} 超出有效范围 [0, {_table.Count - 1}]");
        if (_table.Count == 0) _table.AddRow();
        this.Data.Set(value, index);
    }

    /// <inheritdoc/>
    public void Set(Int32 value, int index)
    {
        if (index < 0 || index >= _table.Count)
            throw new ArgumentOutOfRangeException(nameof(index), $"行索引 {index} 超出有效范围 [0, {_table.Count - 1}]");
        if (_table.Count == 0) _table.AddRow();
        this.Data.Set(value, index);
    }

    /// <inheritdoc/>
    public void Set(Int64 value, int index)
    {
        if (index < 0 || index >= _table.Count)
            throw new ArgumentOutOfRangeException(nameof(index), $"行索引 {index} 超出有效范围 [0, {_table.Count - 1}]");
        if (_table.Count == 0) _table.AddRow();
        this.Data.Set(value, index);
    }

    /// <inheritdoc/>
    public void Set(SByte value, int index)
    {
        if (index < 0 || index >= _table.Count)
            throw new ArgumentOutOfRangeException(nameof(index), $"行索引 {index} 超出有效范围 [0, {_table.Count - 1}]");
        if (_table.Count == 0) _table.AddRow();
        this.Data.Set(value, index);
    }

    /// <inheritdoc/>
    public void Set(Single value, int index)
    {
        if (index < 0 || index >= _table.Count)
            throw new ArgumentOutOfRangeException(nameof(index), $"行索引 {index} 超出有效范围 [0, {_table.Count - 1}]");
        if (_table.Count == 0) _table.AddRow();
        this.Data.Set(value, index);
    }

    /// <inheritdoc/>
    public void Set(String value, int index)
    {
        if (index < 0 || index >= _table.Count)
            throw new ArgumentOutOfRangeException(nameof(index), $"行索引 {index} 超出有效范围 [0, {_table.Count - 1}]");
        if (_table.Count == 0) _table.AddRow();
        this.Data.Set(value, index);
    }

    /// <inheritdoc/>
    public void Set(UInt16 value, int index)
    {
        if (index < 0 || index >= _table.Count)
            throw new ArgumentOutOfRangeException(nameof(index), $"行索引 {index} 超出有效范围 [0, {_table.Count - 1}]");
        if (_table.Count == 0) _table.AddRow();
        this.Data.Set(value, index);
    }

    /// <inheritdoc/>
    public void Set(UInt32 value, int index)
    {
        if (index < 0 || index >= _table.Count)
            throw new ArgumentOutOfRangeException(nameof(index), $"行索引 {index} 超出有效范围 [0, {_table.Count - 1}]");
        if (_table.Count == 0) _table.AddRow();
        this.Data.Set(value, index);
    }

    /// <inheritdoc/>
    public void Set(UInt64 value, int index)
    {
        if (index < 0 || index >= _table.Count)
            throw new ArgumentOutOfRangeException(nameof(index), $"行索引 {index} 超出有效范围 [0, {_table.Count - 1}]");
        if (_table.Count == 0) _table.AddRow();
        this.Data.Set(value, index);
    }


    /// <inheritdoc/>
    public Boolean ToBoolean(int index)
    {
        if (index < 0 || index >= _table.Count)
            throw new ArgumentOutOfRangeException(nameof(index), $"行索引 {index} 超出有效范围 [0, {_table.Count - 1}]");
        return this.Data.ToBoolean(index);
    }

    /// <inheritdoc/>
    public Byte ToByte(int index)
    {
        if (index < 0 || index >= _table.Count)
            throw new ArgumentOutOfRangeException(nameof(index), $"行索引 {index} 超出有效范围 [0, {_table.Count - 1}]");
        return this.Data.ToByte(index);
    }

    /// <inheritdoc/>
    public Char ToChar(int index)
    {
        if (index < 0 || index >= _table.Count)
            throw new ArgumentOutOfRangeException(nameof(index), $"行索引 {index} 超出有效范围 [0, {_table.Count - 1}]");
        return this.Data.ToChar(index);
    }

    /// <inheritdoc/>
    public DateTime ToDateTime(int index)
    {
        if (index < 0 || index >= _table.Count)
            throw new ArgumentOutOfRangeException(nameof(index), $"行索引 {index} 超出有效范围 [0, {_table.Count - 1}]");
        return this.Data.ToDateTime(index);
    }

    /// <inheritdoc/>
    public Decimal ToDecimal(int index)
    {
        if (index < 0 || index >= _table.Count)
            throw new ArgumentOutOfRangeException(nameof(index), $"行索引 {index} 超出有效范围 [0, {_table.Count - 1}]");
        return this.Data.ToDecimal(index);
    }

    /// <inheritdoc/>
    public Double ToDouble(int index)
    {
        if (index < 0 || index >= _table.Count)
            throw new ArgumentOutOfRangeException(nameof(index), $"行索引 {index} 超出有效范围 [0, {_table.Count - 1}]");
        return this.Data.ToDouble(index);
    }

    /// <inheritdoc/>
    public Int16 ToInt16(int index)
    {
        if (index < 0 || index >= _table.Count)
            throw new ArgumentOutOfRangeException(nameof(index), $"行索引 {index} 超出有效范围 [0, {_table.Count - 1}]");
        return this.Data.ToInt16(index);
    }

    /// <inheritdoc/>
    public Int32 ToInt32(int index)
    {
        if (index < 0 || index >= _table.Count)
            throw new ArgumentOutOfRangeException(nameof(index), $"行索引 {index} 超出有效范围 [0, {_table.Count - 1}]");
        return this.Data.ToInt32(index);
    }

    /// <inheritdoc/>
    public Int64 ToInt64(int index)
    {
        if (index < 0 || index >= _table.Count)
            throw new ArgumentOutOfRangeException(nameof(index), $"行索引 {index} 超出有效范围 [0, {_table.Count - 1}]");
        return this.Data.ToInt64(index);
    }

    /// <inheritdoc/>
    public SByte ToSByte(int index)
    {
        if (index < 0 || index >= _table.Count)
            throw new ArgumentOutOfRangeException(nameof(index), $"行索引 {index} 超出有效范围 [0, {_table.Count - 1}]");
        return this.Data.ToSByte(index);
    }

    /// <inheritdoc/>
    public Single ToSingle(int index)
    {
        if (index < 0 || index >= _table.Count)
            throw new ArgumentOutOfRangeException(nameof(index), $"行索引 {index} 超出有效范围 [0, {_table.Count - 1}]");
        return this.Data.ToSingle(index);
    }

    /// <inheritdoc/>
    public String ToString(int index)
    {
        if (index < 0 || index >= _table.Count)
            throw new ArgumentOutOfRangeException(nameof(index), $"行索引 {index} 超出有效范围 [0, {_table.Count - 1}]");
        return this.Data.ToString(index);
    }

    /// <inheritdoc/>
    public UInt16 ToUInt16(int index)
    {
        if (index < 0 || index >= _table.Count)
            throw new ArgumentOutOfRangeException(nameof(index), $"行索引 {index} 超出有效范围 [0, {_table.Count - 1}]");
        return this.Data.ToUInt16(index);
    }

    /// <inheritdoc/>
    public UInt32 ToUInt32(int index)
    {
        if (index < 0 || index >= _table.Count)
            throw new ArgumentOutOfRangeException(nameof(index), $"行索引 {index} 超出有效范围 [0, {_table.Count - 1}]");
        return this.Data.ToUInt32(index);
    }

    /// <inheritdoc/>
    public UInt64 ToUInt64(int index)
    {
        if (index < 0 || index >= _table.Count)
            throw new ArgumentOutOfRangeException(nameof(index), $"行索引 {index} 超出有效范围 [0, {_table.Count - 1}]");
        return this.Data.ToUInt64(index);
    }
}
