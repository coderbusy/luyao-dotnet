using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace LuYao.Data;

/// <summary>
/// 列存储数据集合
/// </summary>
public partial class Record : IEnumerable<RecordRow>, IRecordCursor
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
    /// 游标位置
    /// </summary>
    public int Cursor { get; private set; } = 0;

    /// <summary>
    /// 获取数据是否为空
    /// </summary>
    public bool IsEmpty { get { return Count > 0 ? false : true; } }

    /// <summary>
    /// 添加一行数据。
    /// </summary>
    /// <returns>新添加的行数据。</returns>
    public RecordRow AddRow()
    {
        this.Cursor = this.Count;
        this.Count++;
        this.Columns.OnAddRow();
        return new RecordRow(this, this.Cursor);
    }

    /// <summary>
    /// 删除当前游标位置的行。
    /// </summary>
    public void Delete() => this.Delete(this.Cursor);

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
    /// 读取一行，成功返回 true，失败返回 false。
    /// 当游标位置已经到达最后一行时，重置游标到第一行并返回 false。
    /// </summary>
    /// <returns>如果成功读取到下一行则返回 true，否则返回 false。</returns>
    public bool Read()
    {
        if (this.Cursor < this.Count)
        {
            this.Cursor++;
            return true;
        }
        this.Cursor = 0;
        return false;
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
    /// 内部清理方法，重置计数器和游标位置。
    /// </summary>
    internal void OnClear()
    {
        this.Count = 0;
        this.Cursor = 0;
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

    #region Get

    /// <summary>
    /// 根据指定的列获取当前游标位置的泛型类型值。
    /// </summary>
    /// <typeparam name="T">要获取的值的类型。</typeparam>
    /// <param name="col">要获取值的列。</param>
    /// <returns>如果列属于此记录则返回转换后的值，否则返回默认值。</returns>
    public T? Get<T>(RecordColumn col) => col.Record == this ? col.To<T>() : default;

    /// <summary>
    /// 根据列名获取当前游标位置的泛型类型值。
    /// </summary>
    /// <typeparam name="T">要获取的值的类型。</typeparam>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到列则返回转换后的值，否则返回默认值。</returns>
    public T? Get<T>(string name)
    {
        var col = this.Columns.Find(name);
        return col != null ? col.To<T>() : default;
    }

    /// <summary>
    /// 根据列名获取当前游标位置的布尔值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到列则返回布尔值，否则返回默认值。</returns>
    public Boolean GetBoolean(string name)
    {
        var col = this.Columns.Find(name);
        return col != null ? col.ToBoolean() : default;
    }

    /// <summary>
    /// 根据指定的列获取当前游标位置的布尔值。
    /// </summary>
    /// <param name="col">要获取值的列。</param>
    /// <returns>如果列属于此记录则返回布尔值，否则通过列名获取。</returns>
    public Boolean GetBoolean(RecordColumn col) => col.Record == this ? col.ToBoolean() : GetBoolean(col.Name);

    /// <summary>
    /// 根据列名获取当前游标位置的字节值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到列则返回字节值，否则返回默认值。</returns>
    public Byte GetByte(string name)
    {
        var col = this.Columns.Find(name);
        return col != null ? col.ToByte() : default;
    }

    /// <summary>
    /// 根据指定的列获取当前游标位置的字节值。
    /// </summary>
    /// <param name="col">要获取值的列。</param>
    /// <returns>如果列属于此记录则返回字节值，否则通过列名获取。</returns>
    public Byte GetByte(RecordColumn col) => col.Record == this ? col.ToByte() : GetByte(col.Name);

    /// <summary>
    /// 根据列名获取当前游标位置的字符值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到列则返回字符值，否则返回默认值。</returns>
    public Char GetChar(string name)
    {
        var col = this.Columns.Find(name);
        return col != null ? col.ToChar() : default;
    }

    /// <summary>
    /// 根据指定的列获取当前游标位置的字符值。
    /// </summary>
    /// <param name="col">要获取值的列。</param>
    /// <returns>如果列属于此记录则返回字符值，否则通过列名获取。</returns>
    public Char GetChar(RecordColumn col) => col.Record == this ? col.ToChar() : GetChar(col.Name);

    /// <summary>
    /// 根据列名获取当前游标位置的日期时间值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到列则返回日期时间值，否则返回默认值。</returns>
    public DateTime GetDateTime(string name)
    {
        var col = this.Columns.Find(name);
        return col != null ? col.ToDateTime() : default;
    }

    /// <summary>
    /// 根据指定的列获取当前游标位置的日期时间值。
    /// </summary>
    /// <param name="col">要获取值的列。</param>
    /// <returns>如果列属于此记录则返回日期时间值，否则通过列名获取。</returns>
    public DateTime GetDateTime(RecordColumn col) => col.Record == this ? col.ToDateTime() : GetDateTime(col.Name);

    /// <summary>
    /// 根据列名获取当前游标位置的十进制数值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到列则返回十进制数值，否则返回默认值。</returns>
    public Decimal GetDecimal(string name)
    {
        var col = this.Columns.Find(name);
        return col != null ? col.ToDecimal() : default;
    }

    /// <summary>
    /// 根据指定的列获取当前游标位置的十进制数值。
    /// </summary>
    /// <param name="col">要获取值的列。</param>
    /// <returns>如果列属于此记录则返回十进制数值，否则通过列名获取。</returns>
    public Decimal GetDecimal(RecordColumn col) => col.Record == this ? col.ToDecimal() : GetDecimal(col.Name);

    /// <summary>
    /// 根据列名获取当前游标位置的双精度浮点数值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到列则返回双精度浮点数值，否则返回默认值。</returns>
    public Double GetDouble(string name)
    {
        var col = this.Columns.Find(name);
        return col != null ? col.ToDouble() : default;
    }

    /// <summary>
    /// 根据指定的列获取当前游标位置的双精度浮点数值。
    /// </summary>
    /// <param name="col">要获取值的列。</param>
    /// <returns>如果列属于此记录则返回双精度浮点数值，否则通过列名获取。</returns>
    public Double GetDouble(RecordColumn col) => col.Record == this ? col.ToDouble() : GetDouble(col.Name);

    /// <summary>
    /// 根据列名获取当前游标位置的16位有符号整数值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到列则返回16位有符号整数值，否则返回默认值。</returns>
    public Int16 GetInt16(string name)
    {
        var col = this.Columns.Find(name);
        return col != null ? col.ToInt16() : default;
    }

    /// <summary>
    /// 根据指定的列获取当前游标位置的16位有符号整数值。
    /// </summary>
    /// <param name="col">要获取值的列。</param>
    /// <returns>如果列属于此记录则返回16位有符号整数值，否则通过列名获取。</returns>
    public Int16 GetInt16(RecordColumn col) => col.Record == this ? col.ToInt16() : GetInt16(col.Name);

    /// <summary>
    /// 根据列名获取当前游标位置的32位有符号整数值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到列则返回32位有符号整数值，否则返回默认值。</returns>
    public Int32 GetInt32(string name)
    {
        var col = this.Columns.Find(name);
        return col != null ? col.ToInt32() : default;
    }

    /// <summary>
    /// 根据指定的列获取当前游标位置的32位有符号整数值。
    /// </summary>
    /// <param name="col">要获取值的列。</param>
    /// <returns>如果列属于此记录则返回32位有符号整数值，否则通过列名获取。</returns>
    public Int32 GetInt32(RecordColumn col) => col.Record == this ? col.ToInt32() : GetInt32(col.Name);

    /// <summary>
    /// 根据列名获取当前游标位置的64位有符号整数值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到列则返回64位有符号整数值，否则返回默认值。</returns>
    public Int64 GetInt64(string name)
    {
        var col = this.Columns.Find(name);
        return col != null ? col.ToInt64() : default;
    }

    /// <summary>
    /// 根据指定的列获取当前游标位置的64位有符号整数值。
    /// </summary>
    /// <param name="col">要获取值的列。</param>
    /// <returns>如果列属于此记录则返回64位有符号整数值，否则通过列名获取。</returns>
    public Int64 GetInt64(RecordColumn col) => col.Record == this ? col.ToInt64() : GetInt64(col.Name);

    /// <summary>
    /// 根据列名获取当前游标位置的8位有符号整数值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到列则返回8位有符号整数值，否则返回默认值。</returns>
    public SByte GetSByte(string name)
    {
        var col = this.Columns.Find(name);
        return col != null ? col.ToSByte() : default;
    }

    /// <summary>
    /// 根据指定的列获取当前游标位置的8位有符号整数值。
    /// </summary>
    /// <param name="col">要获取值的列。</param>
    /// <returns>如果列属于此记录则返回8位有符号整数值，否则通过列名获取。</returns>
    public SByte GetSByte(RecordColumn col) => col.Record == this ? col.ToSByte() : GetSByte(col.Name);

    /// <summary>
    /// 根据列名获取当前游标位置的单精度浮点数值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到列则返回单精度浮点数值，否则返回默认值。</returns>
    public Single GetSingle(string name)
    {
        var col = this.Columns.Find(name);
        return col != null ? col.ToSingle() : default;
    }

    /// <summary>
    /// 根据指定的列获取当前游标位置的单精度浮点数值。
    /// </summary>
    /// <param name="col">要获取值的列。</param>
    /// <returns>如果列属于此记录则返回单精度浮点数值，否则通过列名获取。</returns>
    public Single GetSingle(RecordColumn col) => col.Record == this ? col.ToSingle() : GetSingle(col.Name);

    /// <summary>
    /// 根据列名获取当前游标位置的字符串值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到列则返回字符串值，否则返回默认值。</returns>
    public String? GetString(string name)
    {
        var col = this.Columns.Find(name);
        return col != null ? col.ToString() : default;
    }

    /// <summary>
    /// 根据指定的列获取当前游标位置的字符串值。
    /// </summary>
    /// <param name="col">要获取值的列。</param>
    /// <returns>如果列属于此记录则返回字符串值，否则通过列名获取。</returns>
    public String? GetString(RecordColumn col) => col.Record == this ? col.ToString() : GetString(col.Name);

    /// <summary>
    /// 根据列名获取当前游标位置的16位无符号整数值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到列则返回16位无符号整数值，否则返回默认值。</returns>
    public UInt16 GetUInt16(string name)
    {
        var col = this.Columns.Find(name);
        return col != null ? col.ToUInt16() : default;
    }

    /// <summary>
    /// 根据指定的列获取当前游标位置的16位无符号整数值。
    /// </summary>
    /// <param name="col">要获取值的列。</param>
    /// <returns>如果列属于此记录则返回16位无符号整数值，否则通过列名获取。</returns>
    public UInt16 GetUInt16(RecordColumn col) => col.Record == this ? col.ToUInt16() : GetUInt16(col.Name);

    /// <summary>
    /// 根据列名获取当前游标位置的32位无符号整数值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到列则返回32位无符号整数值，否则返回默认值。</returns>
    public UInt32 GetUInt32(string name)
    {
        var col = this.Columns.Find(name);
        return col != null ? col.ToUInt32() : default;
    }

    /// <summary>
    /// 根据指定的列获取当前游标位置的32位无符号整数值。
    /// </summary>
    /// <param name="col">要获取值的列。</param>
    /// <returns>如果列属于此记录则返回32位无符号整数值，否则通过列名获取。</returns>
    public UInt32 GetUInt32(RecordColumn col) => col.Record == this ? col.ToUInt32() : GetUInt32(col.Name);

    /// <summary>
    /// 根据列名获取当前游标位置的64位无符号整数值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到列则返回64位无符号整数值，否则返回默认值。</returns>
    public UInt64 GetUInt64(string name)
    {
        var col = this.Columns.Find(name);
        return col != null ? col.ToUInt64() : default;
    }

    /// <summary>
    /// 根据指定的列获取当前游标位置的64位无符号整数值。
    /// </summary>
    /// <param name="col">要获取值的列。</param>
    /// <returns>如果列属于此记录则返回64位无符号整数值，否则通过列名获取。</returns>
    public UInt64 GetUInt64(RecordColumn col) => col.Record == this ? col.ToUInt64() : GetUInt64(col.Name);
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
    #endregion
}