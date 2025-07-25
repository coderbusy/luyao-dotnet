﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

    private readonly RecordColumnCollection _cols;

    /// <summary>
    /// 表列集合
    /// </summary>
    public RecordColumnCollection Columns => _cols;

    /// <summary>
    /// 数据条数
    /// </summary>
    public int Count => _cols.Rows;

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
        _cols = new RecordColumnCollection(this, c);
    }

    /// <summary>
    /// 添加一行数据。
    /// </summary>
    /// <returns>新添加行的索引。</returns>
    public RecordRow AddRow()
    {
        var row = _cols.AddRow();
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
        RecordColumn? col = _cols.Find(column);
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
        RecordColumn? col = _cols.Find(column);
        if (col == null) throw new KeyNotFoundException();
        return col.GetValue(row);
    }

    /// <summary>
    /// 判断是否包含指定列。
    /// </summary>
    /// <param name="column">列名称。</param>
    /// <returns>如果包含指定列则返回 true，否则返回 false。</returns>
    public bool Contains(string column) => _cols.Contains(column);

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
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
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
        foreach (RecordColumn col in this._cols)
        {
            var data = col.Data;
            for (int i = row; i < Count - 1; i++) data.SetValue(data.GetValue(i + 1), i);
        }
        this._cols.Rows--;
        return true;
    }

    ///<inheritdoc/>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append(string.IsNullOrWhiteSpace(this.Name) ? "None" : this.Name);
        sb.AppendFormat(" count {0} column {1}", this.Count, this._cols.Count);
        sb.AppendLine();
        if (this.Count == 1)
        {
            //只有一行数据时，输出每列的值
            int max = this._cols.Max(f => f.Name.Length);
            foreach (RecordColumn col in this._cols)
            {
                sb.AppendFormat("{0} | {1}", col.Name.PadRight(max), col.GetValue(0));
                sb.AppendLine();
            }
        }
        else
        {
            //多行时，输出表格
            const int MAX_LENGTH = 40;
            int[] heads = new int[this._cols.Count];
            string[,] arr = new string[Count, this._cols.Count];
            for (int k = 0; k < this._cols.Count; k++)
            {
                RecordColumn col = this._cols[k];
                int max = col.Name.Length;

                for (int i = 0; i < Count; i++)
                {
                    string s = col.ToString(i);
                    int len = bLength(s);
                    if (len > MAX_LENGTH)
                    {
                        s = bSubstring(s, MAX_LENGTH + 2) + "..";
                        len = MAX_LENGTH;
                    }
                    arr[i, k] = s;

                    if (len > max) max = len;
                }

                heads[k] = max;
            }

            //写表头
            for (int k = 0; k < this._cols.Count; k++)
            {
                if (k > 0) sb.Append(" | ");
                sb.Append(this._cols[k].Name.PadRight(heads[k]));
            }

            //写数据行
            for (int i = 0; i < Count; i++)
            {
                sb.AppendLine();
                for (int k = 0; k < this._cols.Count; k++)
                {
                    if (k > 0) sb.Append(" | ");

                    string s = arr[i, k];
                    int len = bLength(s);
                    int max = heads[k];
                    sb.Append(s);
                    if (max > len) sb.Append(new string(' ', max - len));
                }
            }
        }
        return sb.ToString();
    }

    static string bSubstring(string s, int len)
    {
        string ret = string.Empty;
        char[] chars = s.ToCharArray();
        for (int i = 0, idx = 0; i < s.Length; ++i, ++idx)
        {
            if (Encoding.UTF8.GetByteCount(chars, i, 1) > 1) ++idx;
            if (idx >= len) break;
            ret += s[i];
        }

        return ret;
    }

    static int bLength(string s) // 单字节长度
    {
        if (s == null) return 0;
        int len = 0;
        char[] chars = s.ToCharArray();
        for (int i = 0; i < s.Length; i++)
        {
            if (Encoding.UTF8.GetByteCount(chars, i, 1) > 1) len += 2;
            else len++;
        }
        return len;
    }
}
