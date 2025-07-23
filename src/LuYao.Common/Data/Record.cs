using System;
using System.Collections;
using System.Collections.Generic;

namespace LuYao.Data;

/// <summary>
/// 列存储数据表
/// </summary>
public partial class Record : IEnumerable<RecordRow>
{
    /// <summary>
    /// 表名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    private readonly RecordColumnCollection _columns;

    /// <summary>
    /// 表列集合
    /// </summary>
    public RecordColumnCollection Columns => _columns;

    /// <summary>
    /// 数据条数
    /// </summary>
    public int Count => _columns.Rows;

    /// <summary>
    /// 初始化 <see cref="Record"/> 类的新实例。
    /// </summary>
    public Record() : this(null, 0)
    {

    }

    /// <summary>
    /// 使用指定的表名和行数初始化 <see cref="Record"/> 类的新实例。
    /// </summary>
    /// <param name="name">表名称。</param>
    /// <param name="rows">初始行数。</param>
    public Record(string? name, int rows)
    {
        if (!string.IsNullOrWhiteSpace(name)) this.Name = name!;
        int c = rows;
        if (c < 20) c = 20;
        _columns = new RecordColumnCollection(this, c);
    }

    /// <summary>
    /// 添加一行数据。
    /// </summary>
    /// <returns>新添加行的索引。</returns>
    public RecordRow AddRow()
    {
        var row = _columns.AddRow();
        return new RecordRow(this, row);
    }

    /// <summary>
    /// 设置指定列的指定行的值。
    /// </summary>
    /// <param name="column">列名称。</param>
    /// <param name="row">行索引。</param>
    /// <param name="value">要设置的值。</param>
    public void SetValue(string column, int row, object? value)
    {
        RecordColumn? col = _columns.Find(column);
        if (col == null) throw new KeyNotFoundException();
        col.SetValue(value, row);
    }

    /// <summary>
    /// 获取指定列的指定行的值。
    /// </summary>
    /// <param name="column">列名称。</param>
    /// <param name="row">行索引。</param>
    /// <returns>指定单元格的值。</returns>
    public object? GetValue(string column, int row)
    {
        RecordColumn? col = _columns.Find(column);
        if (col == null) throw new KeyNotFoundException();
        return col.GetValue(row);
    }

    /// <summary>
    /// 判断是否包含指定列。
    /// </summary>
    /// <param name="column">列名称。</param>
    /// <returns>如果包含指定列则返回 true，否则返回 false。</returns>
    public bool Contains(string column) => _columns.Contains(column);

    #region IEnumerable
    /// <summary>
    /// 返回一个循环访问集合的枚举器。
    /// </summary>
    /// <returns>用于遍历集合的枚举器。</returns>
    public IEnumerator<RecordRow> GetEnumerator()
    {
        for (int i = 0; i < this.Count; i++)
        {
            yield return new RecordRow(this, i);
        }
    }

    /// <summary>
    /// 返回一个循环访问集合的枚举器（非泛型）。
    /// </summary>
    /// <returns>用于遍历集合的枚举器。</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    #endregion

    /// <summary>
    /// 获取指定索引的行。
    /// </summary>
    /// <param name="rowIndex">行索引。</param>
    /// <returns>指定索引的 <see cref="RecordRow"/> 实例。</returns>
    public RecordRow GetRow(int rowIndex)
    {
        if (rowIndex < 0 || rowIndex >= this.Count) throw new ArgumentOutOfRangeException(nameof(rowIndex));
        return new RecordRow(this, rowIndex);
    }

    /// <summary>
    /// 删除指定索引的行。
    /// </summary>
    public bool Delete(int row)
    {
        if (row < 0 || row >= this.Count) return false;
        foreach (RecordColumn col in this.Columns)
        {
            var data = col.Data;
            for (int i = row; i < Count - 1; i++) data.SetValue(data.GetValue(i + 1), i);
        }
        this.Columns.Rows--;
        return true;
    }
}
