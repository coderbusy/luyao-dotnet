using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace LuYao.Data;

/// <summary>
/// 列存储数据集合
/// </summary>
[DebuggerTypeProxy(typeof(RecordDebuggerTypeProxy))]
public partial class Record : IEnumerable<RecordRow>
{
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
        this.Capacity = c;
        _cols = new RecordColumnCollection(this);
    }

    /// <summary>
    /// 集合名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 列集合的私有字段
    /// </summary>
    private readonly RecordColumnCollection _cols;

    /// <summary>
    /// 表列集合
    /// </summary>
    public RecordColumnCollection Columns => _cols;
    /// <summary>
    /// 容量
    /// </summary>
    public int Capacity { get; internal set; }
    /// <summary>
    /// 数据条数
    /// </summary>
    public int Count { get; private set; } = 0;


    /// <summary>
    /// 添加一行数据。
    /// </summary>
    /// <returns>新添加的行数据。</returns>
    public RecordRow AddRow()
    {
        int row = this.Count;
        this.Count++;
        this.Columns.OnAddRow();
        return new RecordRow(this, row);
    }

    /// <summary>
    /// 删除指定索引的行。
    /// </summary>
    public bool Delete(int row)
    {
        if (row < 0 || row >= this.Count) return false;
        foreach (RecordColumn col in this._cols) col.Delete(row);
        this.Count--;
        return true;
    }

    /// <summary>
    /// 清除所有数据。
    /// </summary>
    public void ClearRows()
    {
        this.OnClear();
        foreach (RecordColumn col in this.Columns)
        {
            col.Clear();
        }
    }

    /// <summary>
    /// 内部清理方法，重置计数器。
    /// </summary>
    internal void OnClear()
    {
        this.Count = 0;
    }

    #region IEnumerable

    /// <inheritdoc/> 
    public IEnumerator<RecordRow> GetEnumerator()
    {
        for (int i = 0; i < Count; i++)
        {
            yield return new RecordRow(this, i);
        }
    }

    /// <summary>
    /// 返回循环访问集合的枚举器。
    /// </summary>
    /// <returns>可用于循环访问集合的 <see cref="IEnumerator"/> 对象。</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    #endregion

    #region IDataReader
    /// <summary>
    /// 从指定的 <see cref="IDataReader"/> 读取数据并填充到当前 <see cref="Record"/> 实例。
    /// </summary>
    /// <param name="dr">用于读取数据的 <see cref="IDataReader"/> 实例。</param>
    public void Read(IDataReader dr)
    {
        if (dr == null) throw new ArgumentNullException(nameof(dr));

        this.Columns.Clear();
        var count = dr.FieldCount;
        if (count <= 0) return;
        for (int i = 0; i < count; i++)
        {
            string n = dr.GetName(i);
            Type t = dr.GetFieldType(i);
            this.Columns.Add(n, t);
        }

        while (dr.Read())
        {
            var row = this.AddRow();
            for (int i = 0; i < count; i++)
            {
                object val = dr.GetValue(i);
                if (val == DBNull.Value) continue;
                this.Columns[i].SetValue(val, row);
            }
        }
    }
    #endregion

    #region ToString

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
                    string s = Convert.ToString(col.GetValue(i)) ?? string.Empty;
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
    #endregion

    /// <summary>
    /// 获取一个值，该值指示数据集是否为空（不包含任何行）。
    /// </summary>
    /// <value>
    /// 如果 <see cref="Count"/> 为 0，则为 <see langword="true"/>；否则为 <see langword="false"/>。
    /// </value>
    public bool IsEmpty => this.Count == 0;

    /// <summary>
    /// 获取指定索引处的 <see cref="RecordRow"/> 实例。
    /// </summary>
    /// <param name="row">要获取的行的索引。</param>
    /// <returns>指定索引处的 <see cref="RecordRow"/>。</returns>
    /// <exception cref="ArgumentOutOfRangeException">当索引超出范围时抛出。</exception>
    public RecordRow this[int row]
    {
        get
        {
            if (row < 0 || row >= this.Count) throw new ArgumentOutOfRangeException(nameof(row), "索引超出范围");
            return new RecordRow(this, row);
        }
    }

    #region DataTable

    /// <summary>
    /// 将当前 <see cref="Record"/> 实例的数据写入指定的 <see cref="DataTable"/>。
    /// </summary>
    /// <param name="dt">用于接收数据的 <see cref="DataTable"/> 实例。</param>
    /// <remarks>
    /// 此方法会将当前记录的所有列结构和行数据写入到指定的 <see cref="DataTable"/> 中。
    /// 如果 <paramref name="dt"/> 为 null，则会抛出 <see cref="ArgumentNullException"/>。
    /// </remarks>
    public void Write(DataTable dt)
    {
        if (dt == null) throw new ArgumentNullException(nameof(dt));
        foreach (RecordColumn col in this.Columns)
        {
            dt.Columns.Add(col.Name, col.Type);
        }
        for (int r = 0; r < this.Count; r++)
        {
            DataRow row = dt.Rows.Add();
            for (int i = 0; i < this.Columns.Count; i++)
            {
                var val = this.Columns[i].GetValue(r);
                if (val is not null) row[i] = val;
            }
        }
    }

    /// <summary>
    /// 从指定的 <see cref="DataTable"/> 读取数据并返回一个新的 <see cref="Record"/> 实例。
    /// </summary>
    /// <param name="dt">用于读取数据的 <see cref="DataTable"/> 实例。</param>
    /// <returns>读取到的 <see cref="Record"/> 实例。</returns>
    public static Record Read(DataTable dt)
    {
        var ret = new Record(dt.TableName, dt.Rows.Count);
        foreach (DataColumn col in dt.Columns)
        {
            ret.Columns.Add(col.ColumnName, col.DataType);
        }
        foreach (DataRow row in dt.Rows)
        {
            RecordRow recordRow = ret.AddRow();
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                var col = ret.Columns[i];
                if (row.IsNull(i)) continue;
                col.SetValue(row[i], recordRow);
            }
        }
        return ret;
    }

    /// <summary>
    /// 将当前 <see cref="Record"/> 实例转换为 <see cref="DataTable"/>。
    /// </summary>
    /// <returns>包含当前记录所有数据的 <see cref="DataTable"/> 实例。</returns>
    /// <remarks>
    /// 此方法会创建一个新的 <see cref="DataTable"/>，表名与当前记录名称一致，并将所有列结构和行数据写入到该表中。
    /// </remarks>
    public DataTable ToDataTable()
    {
        var dt = new DataTable(this.Name);
        this.Write(dt);
        return dt;
    }

    #endregion

    #region Schema Operations

    /// <summary>
    /// 重命名指定列。
    /// </summary>
    /// <param name="oldName">原列名。</param>
    /// <param name="newName">新列名。</param>
    public void RenameColumn(string oldName, string newName)
    {
        this.Columns.Rename(oldName, newName);
    }

    /// <summary>
    /// 转换指定列的数据类型，逐行转换数据值。
    /// </summary>
    /// <param name="name">要转换的列名。</param>
    /// <param name="newType">目标数据类型。</param>
    /// <exception cref="KeyNotFoundException">当列名不存在时抛出。</exception>
    /// <exception cref="ArgumentNullException">当 <paramref name="newType"/> 为 null 时抛出。</exception>
    public void CastColumn(string name, Type newType)
    {
        if (newType == null) throw new ArgumentNullException(nameof(newType));
        var oldCol = this.Columns.Find(name) ?? throw new KeyNotFoundException($"列 '{name}' 不存在");
        if (oldCol.Type == newType) return;

        int idx = this.Columns.IndexOf(name);
        var newCol = Helpers.MakeRecordColumn(this, name, newType);

        for (int r = 0; r < this.Count; r++)
        {
            var val = oldCol.GetValue(r);
            if (val is not null)
            {
                newCol.SetValue(Valid.To(val, newType), r);
            }
        }

        this.Columns.ReplaceAt(idx, newCol);
    }

    /// <summary>
    /// 按指定顺序重新排列列。
    /// </summary>
    /// <param name="names">按期望顺序排列的列名数组。</param>
    public void ReorderColumns(params string[] names)
    {
        this.Columns.Reorder(names);
    }

    /// <summary>
    /// 仅复制列结构（零行），返回新 <see cref="Record"/>。
    /// </summary>
    /// <returns>具有相同列结构但零行的新 <see cref="Record"/> 实例。</returns>
    public Record CloneSchema()
    {
        var clone = new Record(this.Name, 0);
        foreach (RecordColumn col in this.Columns)
        {
            clone.Columns.Add(col.Name, col.Type);
        }
        return clone;
    }

    /// <summary>
    /// 复制列结构与全部行数据，返回新 <see cref="Record"/>。
    /// </summary>
    /// <returns>包含相同列结构和全部数据的新 <see cref="Record"/> 实例。</returns>
    public Record Clone()
    {
        var clone = new Record(this.Name, this.Count);
        foreach (RecordColumn col in this.Columns)
        {
            clone.Columns.Add(col.Name, col.Type);
        }
        for (int r = 0; r < this.Count; r++)
        {
            clone.AddRow();
            for (int c = 0; c < this.Columns.Count; c++)
            {
                var val = this.Columns[c].GetValue(r);
                if (val is not null)
                {
                    clone.Columns[c].SetValue(val, r);
                }
            }
        }
        return clone;
    }

    /// <summary>
    /// 导出列定义信息（列名 + 类型）。
    /// </summary>
    /// <returns>表示当前列结构的 <see cref="RecordSchema"/> 实例。</returns>
    public RecordSchema GetSchema()
    {
        var columns = new List<RecordSchema.ColumnDef>(this.Columns.Count);
        foreach (RecordColumn col in this.Columns)
        {
            columns.Add(new RecordSchema.ColumnDef(col.Name, col.Type));
        }
        return new RecordSchema(columns);
    }

    #endregion

}