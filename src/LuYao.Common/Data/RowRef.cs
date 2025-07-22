using System;
using System.Collections.Generic;

namespace LuYao.Data;

/// <summary>
/// 代表一个行
/// </summary>
public readonly struct RowRef
{
    private readonly ColumnTable _table;
    private readonly int _rowIndex;

    /// <summary>
    /// 初始化 <see cref="RowRef"/> 的新实例。
    /// </summary>
    /// <param name="table">所属的 <see cref="ColumnTable"/> 实例。</param>
    /// <param name="rowIndex">行索引。</param>
    public RowRef(ColumnTable table, int rowIndex)
    {
        _table = table;
        _rowIndex = rowIndex;
    }

    /// <summary>
    /// 行号
    /// </summary>
    public int RowIndex => _rowIndex;

    #region Set
    public void Set(Column column, object? value)
    {
        if (column == null) throw new ArgumentNullException(nameof(column));
        column.Set(value, _rowIndex);
    }
    public void Set(int columnIndex, object? value)
    {
        var column = _table.Columns[columnIndex];
        column.Set(value, _rowIndex);
    }
    public void Set(string columnName, object? value)
    {
        var column = _table.Columns.Find(columnName);
        if (column == null) throw new KeyNotFoundException($"Column '{columnName}' not found.");
        column.Set(value, _rowIndex);
    }
    #endregion

    #region Get
    public object? Get(Column column)
    {
        if (column == null) throw new ArgumentNullException(nameof(column));
        return column.Get(_rowIndex);
    }
    public object? Get(int columnIndex)
    {
        var column = _table.Columns[columnIndex];
        return column.Get(_rowIndex);
    }
    public object? Get(string columnName)
    {
        var column = _table.Columns.Find(columnName);
        if (column == null) throw new KeyNotFoundException($"Column '{columnName}' not found.");
        return column.Get(_rowIndex);
    }
    #endregion
}
