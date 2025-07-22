using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Data;

partial class Column
{/// <inheritdoc/>
    public void Set(Boolean value, int index)
    {
        if (_table.Count == 0) _table.AddRow();
        this.Data.Set(value, index);
    }

    /// <inheritdoc/>
    public void Set(Byte value, int index)
    {
        if (_table.Count == 0) _table.AddRow();
        this.Data.Set(value, index);
    }

    /// <inheritdoc/>
    public void Set(Char value, int index)
    {
        if (_table.Count == 0) _table.AddRow();
        this.Data.Set(value, index);
    }

    /// <inheritdoc/>
    public void Set(DateTime value, int index)
    {
        if (_table.Count == 0) _table.AddRow();
        this.Data.Set(value, index);
    }

    /// <inheritdoc/>
    public void Set(Decimal value, int index)
    {
        if (_table.Count == 0) _table.AddRow();
        this.Data.Set(value, index);
    }

    /// <inheritdoc/>
    public void Set(Double value, int index)
    {
        if (_table.Count == 0) _table.AddRow();
        this.Data.Set(value, index);
    }

    /// <inheritdoc/>
    public void Set(Int16 value, int index)
    {
        if (_table.Count == 0) _table.AddRow();
        this.Data.Set(value, index);
    }

    /// <inheritdoc/>
    public void Set(Int32 value, int index)
    {
        if (_table.Count == 0) _table.AddRow();
        this.Data.Set(value, index);
    }

    /// <inheritdoc/>
    public void Set(Int64 value, int index)
    {
        if (_table.Count == 0) _table.AddRow();
        this.Data.Set(value, index);
    }

    /// <inheritdoc/>
    public void Set(SByte value, int index)
    {
        if (_table.Count == 0) _table.AddRow();
        this.Data.Set(value, index);
    }

    /// <inheritdoc/>
    public void Set(Single value, int index)
    {
        if (_table.Count == 0) _table.AddRow();
        this.Data.Set(value, index);
    }

    /// <inheritdoc/>
    public void Set(String value, int index)
    {
        if (_table.Count == 0) _table.AddRow();
        this.Data.Set(value, index);
    }

    /// <inheritdoc/>
    public void Set(UInt16 value, int index)
    {
        if (_table.Count == 0) _table.AddRow();
        this.Data.Set(value, index);
    }

    /// <inheritdoc/>
    public void Set(UInt32 value, int index)
    {
        if (_table.Count == 0) _table.AddRow();
        this.Data.Set(value, index);
    }

    /// <inheritdoc/>
    public void Set(UInt64 value, int index)
    {
        if (_table.Count == 0) _table.AddRow();
        this.Data.Set(value, index);
    }
}
