using System;
using System.Collections.Generic;

namespace LuYao.Data;

/// <summary>
/// 列存储数据表
/// </summary>
public partial class ColumnTable
{
    /// <summary>
    /// 表名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    private readonly ColumnCollection _columns;

    /// <summary>
    /// 表列集合
    /// </summary>
    public ColumnCollection Columns => _columns;

    /// <summary>
    /// 数据条数
    /// </summary>
    public int Count => _columns.Rows;

    /// <summary>
    /// 初始化 <see cref="ColumnTable"/> 类的新实例。
    /// </summary>
    public ColumnTable() : this(null, 0)
    {

    }

    /// <summary>
    /// 使用指定的表名和行数初始化 <see cref="ColumnTable"/> 类的新实例。
    /// </summary>
    /// <param name="name">表名称。</param>
    /// <param name="rows">初始行数。</param>
    public ColumnTable(string? name, int rows)
    {
        if (!string.IsNullOrWhiteSpace(name)) this.Name = name!;
        int c = rows;
        if (c < 20) c = 20;
        _columns = new ColumnCollection(this, c);
    }

    /// <summary>
    /// 添加一行数据。
    /// </summary>
    /// <returns>新添加行的索引。</returns>
    public int AddRow()
    {
        var idx = _columns.AddRow();
        this.Cursor = idx;
        return idx;
    }

    /// <summary>
    /// 设置指定列的指定行的值。
    /// </summary>
    /// <param name="column">列名称。</param>
    /// <param name="row">行索引。</param>
    /// <param name="value">要设置的值。</param>
    public void Set(string column, int row, object? value)
    {
        Column? col = _columns.Find(column);
        if (col == null) throw new KeyNotFoundException();
        col.Set(value, row);
    }

    /// <summary>
    /// 判断是否包含指定列。
    /// </summary>
    /// <param name="column">列名称。</param>
    /// <returns>如果包含指定列则返回 true，否则返回 false。</returns>
    public bool Contains(string column) => _columns.Contains(column);
}
