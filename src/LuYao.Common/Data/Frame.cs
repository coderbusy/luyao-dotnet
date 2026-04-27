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
[DebuggerTypeProxy(typeof(FrameDebuggerTypeProxy))]
public partial class Frame : IEnumerable<FrameRow>
{
    /// <summary>
    /// 初始化 <see cref="Frame"/> 类的新实例。
    /// </summary>
    public Frame() : this(null, 0)
    {

    }

    /// <summary>
    /// 初始化 <see cref="Frame"/> 类的新实例。
    /// </summary>
    /// <param name="name">表名称。</param>
    public Frame(string name) : this(name, 0)
    {

    }

    /// <summary>
    /// 使用指定的表名和行数初始化 <see cref="Frame"/> 类的新实例。
    /// </summary>
    /// <param name="name">表名称。</param>
    /// <param name="rows">初始行数。</param>
    public Frame(string? name, int rows)
    {
        if (!string.IsNullOrWhiteSpace(name)) this.Name = name!;
        int c = rows;
        if (c < 20) c = 20;
        this.Capacity = c;
        _cols = new FrameColumnCollection(this);
    }

    /// <summary>
    /// 集合名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 列集合的私有字段
    /// </summary>
    private readonly FrameColumnCollection _cols;

    /// <summary>
    /// 表列集合
    /// </summary>
    public FrameColumnCollection Columns => _cols;
    /// <summary>
    /// 容量
    /// </summary>
    public int Capacity { get; internal set; }
    /// <summary>
    /// 数据条数
    /// </summary>
    public int Count { get; internal set; } = 0;

    #region 服务端翻页

    /// <summary>
    /// 当前页码（从 1 开始）。
    /// </summary>
    public int Page { get; set; } = 1;

    private int _maxCount;

    /// <summary>
    /// 总数据条数。当值大于 0 时返回设置值，否则返回 <see cref="Count"/>。
    /// </summary>
    public int MaxCount
    {
        get => _maxCount > 0 ? _maxCount : Count;
        set => _maxCount = value;
    }

    private int _pageSize;

    /// <summary>
    /// 每页数据条数。
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value;
    }

    /// <summary>
    /// 总页数（只读）。当 <see cref="MaxCount"/> 为 0 时返回 0；<see cref="PageSize"/> 为 0 时按默认值 20 计算。
    /// </summary>
    public int MaxPage
    {
        get
        {
            if (_maxCount == 0) return 0;
            int size = _pageSize == 0 ? 20 : _pageSize;
            return (_maxCount - 1) / size + 1;
        }
    }

    #endregion


    /// <summary>
    /// 添加一行数据。
    /// </summary>
    /// <returns>新添加的行数据。</returns>
    public FrameRow AddRow()
    {
        int row = this.Count;
        this.Count++;
        this.Columns.OnAddRow();
        return new FrameRow(this, row);
    }

    /// <summary>
    /// 添加一行数据并使用指定的值填充各列。
    /// </summary>
    /// <param name="values">用于填充列的值数组，按列顺序赋值。</param>
    /// <returns>新添加的行数据。</returns>
    /// <remarks>
    /// 如果提供的值数量少于列数，则只填充对应的列；如果值数量多于列数，则忽略多余的值。
    /// </remarks>
    public FrameRow AddRowFromValues(params object[] values)
    {
        var row = AddRow();
        for (int i = 0; i < values.Length && i < this.Columns.Count; i++)
        {
            this.Columns[i].Set(row, values[i]);
        }
        return row;
    }

    /// <summary>
    /// 删除指定索引的行。
    /// </summary>
    public bool Delete(int row)
    {
        if (row < 0 || row >= this.Count) return false;
        foreach (FrameColumn col in this._cols) col.Delete(row);
        this.Count--;
        return true;
    }

    /// <summary>
    /// 批量删除满足条件的所有行。从后向前删除以避免索引偏移问题。
    /// </summary>
    /// <param name="predicate">行筛选谓词，返回 true 的行将被删除。</param>
    /// <returns>实际删除的行数。</returns>
    /// <exception cref="ArgumentNullException">当 <paramref name="predicate"/> 为 null 时抛出。</exception>
    public int DeleteWhere(Func<FrameRow, bool> predicate)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        int deleted = 0;
        for (int i = this.Count - 1; i >= 0; i--)
        {
            if (predicate(new FrameRow(this, i)))
            {
                Delete(i);
                deleted++;
            }
        }
        return deleted;
    }

    /// <summary>
    /// 批量删除指定索引集合的行。索引将自动去重并从大到小排序后删除。
    /// </summary>
    /// <param name="rows">要删除的行索引集合。</param>
    /// <returns>实际删除的行数。</returns>
    /// <exception cref="ArgumentNullException">当 <paramref name="rows"/> 为 null 时抛出。</exception>
    public int DeleteRows(IEnumerable<int> rows)
    {
        if (rows == null) throw new ArgumentNullException(nameof(rows));
        // 去重并降序排列，从后向前删除
        var sorted = new SortedSet<int>(rows);
        int deleted = 0;
        foreach (int row in sorted.Reverse())
        {
            if (Delete(row)) deleted++;
        }
        return deleted;
    }

    /// <summary>
    /// 清除所有数据。
    /// </summary>
    public void ClearRows()
    {
        this.OnClear();
        foreach (FrameColumn col in this.Columns)
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
    public IEnumerator<FrameRow> GetEnumerator()
    {
        for (int i = 0; i < Count; i++)
        {
            yield return new FrameRow(this, i);
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
    /// 从指定的 <see cref="IDataReader"/> 读取数据并填充到当前 <see cref="Frame"/> 实例。
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
                this.Columns[i].Set(row, val);
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
        if (this._cols.Count == 0) return sb.ToString();
        if (this.Count == 1)
        {
            //只有一行数据时，输出每列的值
            int max = this._cols.Max(f => DisplayWidth(f.Name));
            foreach (FrameColumn col in this._cols)
            {
                sb.Append(PadRightByWidth(col.Name, max));
                sb.Append(" | ");
                sb.Append(col.ToString(0));
                sb.AppendLine();
            }
        }
        else
        {
            //多行时，输出表格
            const int MAX_WIDTH = 40;
            int[] heads = new int[this._cols.Count];
            string[,] arr = new string[Count, this._cols.Count];
            for (int k = 0; k < this._cols.Count; k++)
            {
                FrameColumn col = this._cols[k];
                int max = DisplayWidth(col.Name);

                for (int i = 0; i < Count; i++)
                {
                    string s = col.ToString(i);
                    int w = DisplayWidth(s);
                    if (w > MAX_WIDTH)
                    {
                        s = SubstringByWidth(s, MAX_WIDTH - 2) + "..";
                        w = DisplayWidth(s);
                    }
                    arr[i, k] = s;

                    if (w > max) max = w;
                }

                heads[k] = max;
            }

            //写表头
            for (int k = 0; k < this._cols.Count; k++)
            {
                if (k > 0) sb.Append(" | ");
                sb.Append(PadRightByWidth(this._cols[k].Name, heads[k]));
            }
            sb.AppendLine();

            //写分隔线
            for (int k = 0; k < this._cols.Count; k++)
            {
                if (k > 0) sb.Append("-+-");
                sb.Append(new string('-', heads[k]));
            }

            //写数据行
            for (int i = 0; i < Count; i++)
            {
                sb.AppendLine();
                for (int k = 0; k < this._cols.Count; k++)
                {
                    if (k > 0) sb.Append(" | ");

                    string s = arr[i, k];
                    sb.Append(PadRightByWidth(s, heads[k]));
                }
            }
        }
        return sb.ToString();
    }

    /// <summary>
    /// 按显示宽度截取字符串，保证不超过指定的列宽。
    /// </summary>
    static string SubstringByWidth(string s, int maxWidth)
    {
        var buf = new StringBuilder();
        int width = 0;
        for (int i = 0; i < s.Length; i++)
        {
            int cw = IsWide(s[i]) ? 2 : 1;
            if (width + cw > maxWidth) break;
            buf.Append(s[i]);
            width += cw;
        }
        return buf.ToString();
    }

    /// <summary>
    /// 将字符串用空格填充到指定的显示宽度。
    /// </summary>
    static string PadRightByWidth(string s, int totalWidth)
    {
        int w = DisplayWidth(s);
        if (w >= totalWidth) return s;
        return s + new string(' ', totalWidth - w);
    }

    /// <summary>
    /// 计算字符串在等宽字体终端中的显示列宽。
    /// 东亚宽字符（CJK、全角标点等）占 2 列，其余占 1 列。
    /// </summary>
    static int DisplayWidth(string s)
    {
        if (s == null) return 0;
        int width = 0;
        for (int i = 0; i < s.Length; i++)
        {
            width += IsWide(s[i]) ? 2 : 1;
        }
        return width;
    }

    /// <summary>
    /// 判断字符是否为东亚宽字符（在等宽终端中占 2 列）。
    /// 覆盖 CJK 统一汉字、全角 ASCII/标点、片假名、韩文音节等常见范围。
    /// </summary>
    static bool IsWide(char c)
    {
        // CJK Unified Ideographs, CJK Extension A, Compatibility Ideographs
        if (c >= 0x4E00 && c <= 0x9FFF) return true;
        if (c >= 0x3400 && c <= 0x4DBF) return true;
        if (c >= 0xF900 && c <= 0xFAFF) return true;
        // Fullwidth Forms (全角 ASCII、全角标点)
        if (c >= 0xFF01 && c <= 0xFF60) return true;
        if (c >= 0xFFE0 && c <= 0xFFE6) return true;
        // CJK Symbols and Punctuation, Hiragana, Katakana, Bopomofo
        if (c >= 0x3000 && c <= 0x33FF) return true;
        // Hangul Syllables
        if (c >= 0xAC00 && c <= 0xD7AF) return true;
        // CJK Unified Ideographs Extension B+ (surrogates, treat high surrogate range)
        if (c >= 0xD800 && c <= 0xDFFF) return true;
        return false;
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
    /// 获取指定索引处的 <see cref="FrameRow"/> 实例。
    /// </summary>
    /// <param name="row">要获取的行的索引。</param>
    /// <returns>指定索引处的 <see cref="FrameRow"/>。</returns>
    /// <exception cref="ArgumentOutOfRangeException">当索引超出范围时抛出。</exception>
    public FrameRow this[int row]
    {
        get
        {
            if (row < 0 || row >= this.Count) throw new ArgumentOutOfRangeException(nameof(row), "索引超出范围");
            return new FrameRow(this, row);
        }
    }

    #region DataTable

    /// <summary>
    /// 将当前 <see cref="Frame"/> 实例的数据写入指定的 <see cref="DataTable"/>。
    /// </summary>
    /// <param name="dt">用于接收数据的 <see cref="DataTable"/> 实例。</param>
    /// <remarks>
    /// 此方法会将当前记录的所有列结构和行数据写入到指定的 <see cref="DataTable"/> 中。
    /// 如果 <paramref name="dt"/> 为 null，则会抛出 <see cref="ArgumentNullException"/>。
    /// </remarks>
    public void Write(DataTable dt)
    {
        if (dt == null) throw new ArgumentNullException(nameof(dt));
        foreach (FrameColumn col in this.Columns)
        {
            dt.Columns.Add(col.Name, col.Type);
        }
        for (int r = 0; r < this.Count; r++)
        {
            DataRow row = dt.Rows.Add();
            for (int i = 0; i < this.Columns.Count; i++)
            {
                var val = this.Columns[i].Get(r);
                if (val is not null) row[i] = val;
            }
        }
    }

    /// <summary>
    /// 从指定的 <see cref="DataTable"/> 读取数据并返回一个新的 <see cref="Frame"/> 实例。
    /// </summary>
    /// <param name="dt">用于读取数据的 <see cref="DataTable"/> 实例。</param>
    /// <returns>读取到的 <see cref="Frame"/> 实例。</returns>
    public static Frame Read(DataTable dt)
    {
        var ret = new Frame(dt.TableName, dt.Rows.Count);
        foreach (DataColumn col in dt.Columns)
        {
            ret.Columns.Add(col.ColumnName, col.DataType);
        }
        foreach (DataRow row in dt.Rows)
        {
            FrameRow recordRow = ret.AddRow();
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                var col = ret.Columns[i];
                if (row.IsNull(i)) continue;
                col.Set(recordRow, row[i]);
            }
        }
        return ret;
    }

    /// <summary>
    /// 将当前 <see cref="Frame"/> 实例转换为 <see cref="DataTable"/>。
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
        var newCol = Helpers.MakeFrameColumn(this, name, newType);

        for (int r = 0; r < this.Count; r++)
        {
            var val = oldCol.Get(r);
            if (val is not null)
            {
                newCol.Set(r, Valid.To(val, newType));
            }
        }

        this.Columns.ReplaceAt(idx, newCol);
    }

    /// <summary>
    /// 仅复制列结构（零行），返回新 <see cref="Frame"/>。
    /// </summary>
    /// <returns>具有相同列结构但零行的新 <see cref="Frame"/> 实例。</returns>
    public Frame CloneSchema()
    {
        var clone = new Frame(this.Name, 0);
        foreach (FrameColumn col in this.Columns)
        {
            clone.Columns.Add(col.Name, col.Type);
        }
        return clone;
    }

    /// <summary>
    /// 复制列结构与全部行数据，返回新 <see cref="Frame"/>。
    /// </summary>
    /// <returns>包含相同列结构和全部数据的新 <see cref="Frame"/> 实例。</returns>
    public Frame Clone()
    {
        var clone = new Frame(this.Name, this.Count);
        clone.Page = this.Page;
        clone._maxCount = this._maxCount;
        clone._pageSize = this._pageSize;
        foreach (FrameColumn col in this.Columns)
        {
            clone.Columns.Add(col.Name, col.Type);
        }
        for (int r = 0; r < this.Count; r++)
        {
            clone.AddRow();
            for (int c = 0; c < this.Columns.Count; c++)
            {
                var val = this.Columns[c].Get(r);
                if (val is not null)
                {
                    clone.Columns[c].Set(r, val);
                }
            }
        }
        return clone;
    }

    /// <summary>
    /// 导出列定义信息（列名 + 类型）。
    /// </summary>
    /// <returns>表示当前列结构的 <see cref="FrameSchema"/> 实例。</returns>
    public FrameSchema GetSchema()
    {
        var columns = new List<FrameSchema.ColumnDef>(this.Columns.Count);
        foreach (FrameColumn col in this.Columns)
        {
            columns.Add(new FrameSchema.ColumnDef(col.Name, col.ColumnType, col.IsNullable));
        }
        return new FrameSchema(columns);
    }

    #endregion

}
