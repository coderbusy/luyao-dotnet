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
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="rows"></param>
    public ColumnTable(string? name, int rows)
    {
        if (!string.IsNullOrWhiteSpace(name)) this.Name = name!;
        int c = rows;
        if (c < 20) c = 20;
        _columns = new ColumnCollection(this, c);
    }
    /// <summary>
    /// 添加一行
    /// </summary>
    /// <returns></returns>
    public int AddRow() => _columns.AddRow();
    /// <summary>
    /// 设置指定列的指定行的值
    /// </summary>
    public void Set(string column, int row, object? value)
    {
        Column? col = _columns.Find(column);
        if (col == null) throw new KeyNotFoundException();
        col.Set(value, row);
    }
    /// <summary>
    /// 判断是否包含指定列
    /// </summary>
    public bool Contains(string column) => _columns.Contains(column);
}
