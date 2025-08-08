using LuYao.Data.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace LuYao.Data;

/// <summary>
/// 列存储数据集合
/// </summary>
[DebuggerTypeProxy(typeof(RecordDebuggerTypeProxy))]
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
    public bool Delete() => this.Delete(this.Cursor);

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
    private bool _isReading = false;
    /// <summary>
    /// 读取一行，成功返回 true，失败返回 false。
    /// 当游标位置已经到达最后一行时，重置游标到第一行并返回 false。
    /// </summary>
    /// <returns>如果成功读取到下一行则返回 true，否则返回 false。</returns>
    public bool Read()
    {
        if (this._isReading)
        {
            if (this.Cursor < this.Count - 1)
            {
                this.Cursor++;
                return true;
            }
            this._isReading = false;
            return false;
        }
        else
        {
            if (this.Count == 0) return false;
            this.Cursor = 0;
            _isReading = true;
            return true;
        }
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
    public T? Get<T>(RecordColumn col) => col.Record == this ? col.Get<T>() : default;

    /// <summary>
    /// 根据列名获取当前游标位置的泛型类型值。
    /// </summary>
    /// <typeparam name="T">要获取的值的类型。</typeparam>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到列则返回转换后的值，否则返回默认值。</returns>
    public T? Get<T>(string name)
    {
        var col = this.Columns.Find(name);
        return col != null ? col.Get<T>() : default;
    }

    /// <summary>
    /// 根据列名获取当前游标位置的布尔值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到列则返回布尔值，否则返回默认值。</returns>
    public Boolean GetBoolean(string name)
    {
        var col = this.Columns.Find(name);
        return col != null ? col.GetBoolean() : default;
    }

    /// <summary>
    /// 根据指定的列获取当前游标位置的布尔值。
    /// </summary>
    /// <param name="col">要获取值的列。</param>
    /// <returns>如果列属于此记录则返回布尔值，否则通过列名获取。</returns>
    public Boolean GetBoolean(RecordColumn col) => col.Record == this ? col.GetBoolean() : GetBoolean(col.Name);

    /// <summary>
    /// 根据列名获取当前游标位置的字节值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到列则返回字节值，否则返回默认值。</returns>
    public Byte GetByte(string name)
    {
        var col = this.Columns.Find(name);
        return col != null ? col.GetByte() : default;
    }

    /// <summary>
    /// 根据指定的列获取当前游标位置的字节值。
    /// </summary>
    /// <param name="col">要获取值的列。</param>
    /// <returns>如果列属于此记录则返回字节值，否则通过列名获取。</returns>
    public Byte GetByte(RecordColumn col) => col.Record == this ? col.GetByte() : GetByte(col.Name);

    /// <summary>
    /// 根据列名获取当前游标位置的字符值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到列则返回字符值，否则返回默认值。</returns>
    public Char GetChar(string name)
    {
        var col = this.Columns.Find(name);
        return col != null ? col.GetChar() : default;
    }

    /// <summary>
    /// 根据指定的列获取当前游标位置的字符值。
    /// </summary>
    /// <param name="col">要获取值的列。</param>
    /// <returns>如果列属于此记录则返回字符值，否则通过列名获取。</returns>
    public Char GetChar(RecordColumn col) => col.Record == this ? col.GetChar() : GetChar(col.Name);

    /// <summary>
    /// 根据列名获取当前游标位置的日期时间值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到列则返回日期时间值，否则返回默认值。</returns>
    public DateTime GetDateTime(string name)
    {
        var col = this.Columns.Find(name);
        return col != null ? col.GetDateTime() : default;
    }

    /// <summary>
    /// 根据指定的列获取当前游标位置的日期时间值。
    /// </summary>
    /// <param name="col">要获取值的列。</param>
    /// <returns>如果列属于此记录则返回日期时间值，否则通过列名获取。</returns>
    public DateTime GetDateTime(RecordColumn col) => col.Record == this ? col.GetDateTime() : GetDateTime(col.Name);

    /// <summary>
    /// 根据列名获取当前游标位置的十进制数值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到列则返回十进制数值，否则返回默认值。</returns>
    public Decimal GetDecimal(string name)
    {
        var col = this.Columns.Find(name);
        return col != null ? col.GetDecimal() : default;
    }

    /// <summary>
    /// 根据指定的列获取当前游标位置的十进制数值。
    /// </summary>
    /// <param name="col">要获取值的列。</param>
    /// <returns>如果列属于此记录则返回十进制数值，否则通过列名获取。</returns>
    public Decimal GetDecimal(RecordColumn col) => col.Record == this ? col.GetDecimal() : GetDecimal(col.Name);

    /// <summary>
    /// 根据列名获取当前游标位置的双精度浮点数值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到列则返回双精度浮点数值，否则返回默认值。</returns>
    public Double GetDouble(string name)
    {
        var col = this.Columns.Find(name);
        return col != null ? col.GetDouble() : default;
    }

    /// <summary>
    /// 根据指定的列获取当前游标位置的双精度浮点数值。
    /// </summary>
    /// <param name="col">要获取值的列。</param>
    /// <returns>如果列属于此记录则返回双精度浮点数值，否则通过列名获取。</returns>
    public Double GetDouble(RecordColumn col) => col.Record == this ? col.GetDouble() : GetDouble(col.Name);

    /// <summary>
    /// 根据列名获取当前游标位置的16位有符号整数值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到列则返回16位有符号整数值，否则返回默认值。</returns>
    public Int16 GetInt16(string name)
    {
        var col = this.Columns.Find(name);
        return col != null ? col.GetInt16() : default;
    }

    /// <summary>
    /// 根据指定的列获取当前游标位置的16位有符号整数值。
    /// </summary>
    /// <param name="col">要获取值的列。</param>
    /// <returns>如果列属于此记录则返回16位有符号整数值，否则通过列名获取。</returns>
    public Int16 GetInt16(RecordColumn col) => col.Record == this ? col.GetInt16() : GetInt16(col.Name);

    /// <summary>
    /// 根据列名获取当前游标位置的32位有符号整数值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到列则返回32位有符号整数值，否则返回默认值。</returns>
    public Int32 GetInt32(string name)
    {
        var col = this.Columns.Find(name);
        return col != null ? col.GetInt32() : default;
    }

    /// <summary>
    /// 根据指定的列获取当前游标位置的32位有符号整数值。
    /// </summary>
    /// <param name="col">要获取值的列。</param>
    /// <returns>如果列属于此记录则返回32位有符号整数值，否则通过列名获取。</returns>
    public Int32 GetInt32(RecordColumn col) => col.Record == this ? col.GetInt32() : GetInt32(col.Name);

    /// <summary>
    /// 根据列名获取当前游标位置的64位有符号整数值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到列则返回64位有符号整数值，否则返回默认值。</returns>
    public Int64 GetInt64(string name)
    {
        var col = this.Columns.Find(name);
        return col != null ? col.GetInt64() : default;
    }

    /// <summary>
    /// 根据指定的列获取当前游标位置的64位有符号整数值。
    /// </summary>
    /// <param name="col">要获取值的列。</param>
    /// <returns>如果列属于此记录则返回64位有符号整数值，否则通过列名获取。</returns>
    public Int64 GetInt64(RecordColumn col) => col.Record == this ? col.GetInt64() : GetInt64(col.Name);

    /// <summary>
    /// 根据列名获取当前游标位置的8位有符号整数值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到列则返回8位有符号整数值，否则返回默认值。</returns>
    public SByte GetSByte(string name)
    {
        var col = this.Columns.Find(name);
        return col != null ? col.GetSByte() : default;
    }

    /// <summary>
    /// 根据指定的列获取当前游标位置的8位有符号整数值。
    /// </summary>
    /// <param name="col">要获取值的列。</param>
    /// <returns>如果列属于此记录则返回8位有符号整数值，否则通过列名获取。</returns>
    public SByte GetSByte(RecordColumn col) => col.Record == this ? col.GetSByte() : GetSByte(col.Name);

    /// <summary>
    /// 根据列名获取当前游标位置的单精度浮点数值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到列则返回单精度浮点数值，否则返回默认值。</returns>
    public Single GetSingle(string name)
    {
        var col = this.Columns.Find(name);
        return col != null ? col.GetSingle() : default;
    }

    /// <summary>
    /// 根据指定的列获取当前游标位置的单精度浮点数值。
    /// </summary>
    /// <param name="col">要获取值的列。</param>
    /// <returns>如果列属于此记录则返回单精度浮点数值，否则通过列名获取。</returns>
    public Single GetSingle(RecordColumn col) => col.Record == this ? col.GetSingle() : GetSingle(col.Name);

    /// <summary>
    /// 根据列名获取当前游标位置的字符串值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到列则返回字符串值，否则返回默认值。</returns>
    public String? GetString(string name)
    {
        var col = this.Columns.Find(name);
        return col != null ? col.GetString() : default;
    }

    /// <summary>
    /// 根据指定的列获取当前游标位置的字符串值。
    /// </summary>
    /// <param name="col">要获取值的列。</param>
    /// <returns>如果列属于此记录则返回字符串值，否则通过列名获取。</returns>
    public String? GetString(RecordColumn col) => col.Record == this ? col.GetString() : GetString(col.Name);

    /// <summary>
    /// 根据列名获取当前游标位置的16位无符号整数值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到列则返回16位无符号整数值，否则返回默认值。</returns>
    public UInt16 GetUInt16(string name)
    {
        var col = this.Columns.Find(name);
        return col != null ? col.GetUInt16() : default;
    }

    /// <summary>
    /// 根据指定的列获取当前游标位置的16位无符号整数值。
    /// </summary>
    /// <param name="col">要获取值的列。</param>
    /// <returns>如果列属于此记录则返回16位无符号整数值，否则通过列名获取。</returns>
    public UInt16 GetUInt16(RecordColumn col) => col.Record == this ? col.GetUInt16() : GetUInt16(col.Name);

    /// <summary>
    /// 根据列名获取当前游标位置的32位无符号整数值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到列则返回32位无符号整数值，否则返回默认值。</returns>
    public UInt32 GetUInt32(string name)
    {
        var col = this.Columns.Find(name);
        return col != null ? col.GetUInt32() : default;
    }

    /// <summary>
    /// 根据指定的列获取当前游标位置的32位无符号整数值。
    /// </summary>
    /// <param name="col">要获取值的列。</param>
    /// <returns>如果列属于此记录则返回32位无符号整数值，否则通过列名获取。</returns>
    public UInt32 GetUInt32(RecordColumn col) => col.Record == this ? col.GetUInt32() : GetUInt32(col.Name);

    /// <summary>
    /// 根据列名获取当前游标位置的64位无符号整数值。
    /// </summary>
    /// <param name="name">列的名称。</param>
    /// <returns>如果找到列则返回64位无符号整数值，否则返回默认值。</returns>
    public UInt64 GetUInt64(string name)
    {
        var col = this.Columns.Find(name);
        return col != null ? col.GetUInt64() : default;
    }

    /// <summary>
    /// 根据指定的列获取当前游标位置的64位无符号整数值。
    /// </summary>
    /// <param name="col">要获取值的列。</param>
    /// <returns>如果列属于此记录则返回64位无符号整数值，否则通过列名获取。</returns>
    public UInt64 GetUInt64(RecordColumn col) => col.Record == this ? col.GetUInt64() : GetUInt64(col.Name);
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
                    string s = col.GetString(i);
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

    #region Loader
    /// <summary>
    /// 向记录中添加一个对象并创建相应的列结构。
    /// </summary>
    /// <typeparam name="T">要添加的对象类型，必须为引用类型。</typeparam>
    /// <param name="item">要添加的对象实例。</param>
    /// <returns>新添加的行数据。</returns>
    /// <exception cref="ArgumentNullException">当 <paramref name="item"/> 为 null 时抛出。</exception>
    /// <remarks>
    /// 此方法会根据对象的属性自动创建列结构，并添加一行数据。
    /// 注意：该方法只创建列结构但不会将数据写入行中，需要手动设置数据。
    /// </remarks>
    public RecordRow Add<T>(T item) where T : class
    {
        if (item == null) throw new ArgumentNullException(nameof(item));
        var row = this.AddRow();
        RecordLoader<T>.WriteHeader(this);
        return row;
    }

    /// <summary>
    /// 从单个对象创建一个新的 <see cref="Record"/> 实例。
    /// </summary>
    /// <typeparam name="T">要转换的对象类型，必须为引用类型。</typeparam>
    /// <param name="item">用于创建记录的对象实例。</param>
    /// <returns>包含该对象数据的新 <see cref="Record"/> 实例。</returns>
    /// <exception cref="ArgumentNullException">当 <paramref name="item"/> 为 null 时抛出。</exception>
    /// <remarks>
    /// 此方法会根据对象的属性自动创建列结构，并将对象的属性值填充到记录中。
    /// </remarks>
    public static Record From<T>(T item) where T : class
    {
        if (item == null) throw new ArgumentNullException(nameof(item));
        var record = new Record();
        RecordLoader<T>.WriteHeader(record);
        var row = record.AddRow();
        RecordLoader<T>.WriteToRow(item, row);
        return record;
    }

    /// <summary>
    /// 从对象集合创建一个新的 <see cref="Record"/> 实例。
    /// </summary>
    /// <typeparam name="T">集合中对象的类型，必须为引用类型。</typeparam>
    /// <param name="items">用于创建记录的对象集合。</param>
    /// <returns>包含集合中所有对象数据的新 <see cref="Record"/> 实例。</returns>
    /// <exception cref="ArgumentNullException">当 <paramref name="items"/> 为 null 时抛出。</exception>
    /// <remarks>
    /// 此方法会根据对象的属性自动创建列结构，并将集合中每个非空对象的属性值填充到记录的相应行中。
    /// 集合中的 null 值会被跳过。
    /// </remarks>
    public static Record FromList<T>(IEnumerable<T> items) where T : class
    {
        if (items == null) throw new ArgumentNullException(nameof(items));
        var record = new Record();
        RecordLoader<T>.WriteHeader(record);
        foreach (var item in items)
        {
            if (item == null) continue;
            var row = record.AddRow();
            RecordLoader<T>.WriteToRow(item, row);
        }
        return record;
    }

    /// <summary>
    /// 将记录的第一行数据转换为指定类型的对象。
    /// </summary>
    /// <typeparam name="T">要转换到的目标类型，必须为引用类型且具有无参构造函数。</typeparam>
    /// <returns>根据记录数据创建的对象实例。如果记录为空，返回具有默认值的对象实例。</returns>
    /// <remarks>
    /// 此方法会创建目标类型的新实例，并将记录第一行的数据填充到对象的相应属性中。
    /// 如果记录中没有数据，返回的对象将包含属性的默认值。
    /// </remarks>
    public T To<T>() where T : class, new() => this.To<T>(this.Cursor);

    /// <summary>
    /// 将记录的第一行数据转换为指定类型的对象。
    /// </summary>
    /// <typeparam name="T">要转换到的目标类型，必须为引用类型且具有无参构造函数。</typeparam>
    /// <returns>根据记录数据创建的对象实例。如果记录为空，返回具有默认值的对象实例。</returns>
    /// <remarks>
    /// 此方法会创建目标类型的新实例，并将记录第一行的数据填充到对象的相应属性中。
    /// 如果记录中没有数据，返回的对象将包含属性的默认值。
    /// </remarks>
    public T To<T>(int row) where T : class, new()
    {
        var item = new T();
        if (this.Count > 0) RecordLoader<T>.Populate(new RecordRow(this, row), item);
        return item;
    }
    /// <summary>
    /// 将记录中的所有行数据转换为指定类型的对象列表。
    /// </summary>
    /// <typeparam name="T">要转换到的目标类型，必须为引用类型且具有无参构造函数。</typeparam>
    /// <returns>包含记录中所有行数据转换后的对象列表。</returns>
    /// <remarks>
    /// 此方法会遍历记录中的每一行，为每行创建一个目标类型的新实例，
    /// 并将行数据填充到对象的相应属性中。返回的列表容量与记录的行数相同。
    /// </remarks>
    public IList<T> ToList<T>() where T : class, new()
    {
        var ret = new List<T>(this.Count);
        foreach (var row in this)
        {
            var item = new T();
            RecordLoader<T>.Populate(row, item);
            ret.Add(item);
        }
        return ret;
    }
    #endregion

    #region Adapter

    /// <summary>
    /// 使用指定的适配器保存记录数据。
    /// </summary>
    /// <param name="adapter">用于保存数据的适配器，不能为 null。</param>
    /// <exception cref="ArgumentNullException">当 <paramref name="adapter"/> 为 null 时抛出。</exception>
    /// <remarks>
    /// 此方法会将记录的表头信息、列定义和所有行数据通过适配器写入目标存储。
    /// 保存过程包括：写入记录头、写入列信息、遍历所有行并根据列的数据类型写入相应的值。
    /// 保存过程中会重置游标到第一行，并在保存完成后游标位置可能发生变化。
    /// </remarks>
    public void Save(RecordSaveAdapter adapter)
    {
        if (adapter == null) throw new ArgumentNullException(nameof(adapter));
        adapter.WriteStart();
        foreach (var section in adapter.Layout)
        {
            switch (section)
            {
                case RecordSection.Head:
                    {
                        adapter.WriteStartSection(section);
                        var header = new RecordHeader(this);
                        adapter.WriteHeader(header);
                        adapter.WriteEndSection();
                    }
                    break;
                case RecordSection.Columns:
                    if (this.Columns.Count > 0)
                    {
                        adapter.WriteStartSection(section);
                        foreach (var column in this.Columns) adapter.WriteColumn(new RecordColumnInfo(column));
                        adapter.WriteEndSection();
                    }
                    break;
                case RecordSection.Rows:

                    if (this.Count > 0)
                    {
                        this.MoveFirst();
                        adapter.WriteStartSection(section);
                        while (this.Read())
                        {
                            adapter.WriteStarRow();
                            for (int i = 0; i < Columns.Count; i++)
                            {
                                RecordColumn col = this.Columns[i];
                                switch (col.Code)
                                {
                                    case RecordDataCode.Boolean: adapter.WriteBoolean(col.Name, i, col.GetBoolean()); break;
                                    case RecordDataCode.Byte: adapter.WriteByte(col.Name, i, col.GetByte()); break;
                                    case RecordDataCode.Char: adapter.WriteChar(col.Name, i, col.GetChar()); break;
                                    case RecordDataCode.DateTime: adapter.WriteDateTime(col.Name, i, col.GetDateTime()); break;
                                    case RecordDataCode.Decimal: adapter.WriteDecimal(col.Name, i, col.GetDecimal()); break;
                                    case RecordDataCode.Double: adapter.WriteDouble(col.Name, i, col.GetDouble()); break;
                                    case RecordDataCode.Int16: adapter.WriteInt16(col.Name, i, col.GetInt16()); break;
                                    case RecordDataCode.Int32: adapter.WriteInt32(col.Name, i, col.GetInt32()); break;
                                    case RecordDataCode.Int64: adapter.WriteInt64(col.Name, i, col.GetInt64()); break;
                                    case RecordDataCode.SByte: adapter.WriteSByte(col.Name, i, col.GetSByte()); break;
                                    case RecordDataCode.Single: adapter.WriteSingle(col.Name, i, col.GetSingle()); break;
                                    case RecordDataCode.String: adapter.WriteString(col.Name, i, col.GetString()); break;
                                    case RecordDataCode.UInt16: adapter.WriteUInt16(col.Name, i, col.GetUInt16()); break;
                                    case RecordDataCode.UInt32: adapter.WriteUInt32(col.Name, i, col.GetUInt32()); break;
                                    case RecordDataCode.UInt64: adapter.WriteUInt64(col.Name, i, col.GetUInt64()); break;
                                    default: adapter.WriteObject(col.Name, i, col.GetValue(this.Cursor)); break;
                                }
                            }
                            adapter.WriteEndRow();
                        }
                        adapter.WriteEndSection();
                    }
                    break;
                default: throw new NotSupportedException("未知的记录适配器布局类型：" + section);
            }
        }
        adapter.WriteEnd();
    }

    /// <summary>
    /// 使用指定的适配器加载记录数据并创建新的 <see cref="Record"/> 实例。
    /// </summary>
    /// <param name="adapter">用于加载数据的适配器，不能为 null。</param>
    /// <returns>从适配器加载数据后创建的新 <see cref="Record"/> 实例。</returns>
    /// <exception cref="ArgumentNullException">当 <paramref name="adapter"/> 为 null 时抛出。</exception>
    /// <remarks>
    /// 此方法会从适配器读取记录的完整结构和数据，包括：
    /// <list type="number">
    /// <item><description>读取记录头信息（名称、行数、列数）</description></item>
    /// <item><description>读取所有列的定义信息（名称和类型）</description></item>
    /// <item><description>读取所有行数据，根据列的数据类型进行相应的类型转换</description></item>
    /// </list>
    /// 加载过程中会根据适配器的键类型（名称或索引）来匹配相应的列，
    /// 如果列不存在则跳过该字段的数据。
    /// </remarks>
    public static Record Load(RecordLoadAdapter adapter)
    {
        if (adapter == null) throw new ArgumentNullException(nameof(adapter));
        var record = new Record();
        RecordHeader? header = null;
        int rows = 0, cols = 0;
        while (adapter.ReadSection())
        {
            switch (adapter.Section)
            {
                case RecordSection.Head:
                    header = adapter.ReadHeader();
                    rows = header.Count;
                    cols = header.Columns;
                    if (!string.IsNullOrWhiteSpace(header.Name)) record.Name = header.Name;
                    break;
                case RecordSection.Rows:

                    while (adapter.ReadRow())
                    {
                        record.AddRow();
                        while (adapter.ReadField())
                        {
                            RecordColumn? col = null;
                            switch (adapter.KeyKind)
                            {
                                case RecordLoadKeyKind.Name:
                                    col = record.Columns.Find(adapter.Name);
                                    break;
                                case RecordLoadKeyKind.Index:
                                    col = record.Columns[adapter.Index];
                                    break;
                            }
                            if (col == null) continue; // 如果列不存在，跳过
                            switch (col.Code)
                            {
                                case RecordDataCode.Boolean: col.Set(adapter.ReadBoolean()); break;
                                case RecordDataCode.Byte: col.Set(adapter.ReadByte()); break;
                                case RecordDataCode.Char: col.Set(adapter.ReadChar()); break;
                                case RecordDataCode.DateTime: col.Set(adapter.ReadDateTime()); break;
                                case RecordDataCode.Decimal: col.Set(adapter.ReadDecimal()); break;
                                case RecordDataCode.Double: col.Set(adapter.ReadDouble()); break;
                                case RecordDataCode.Int16: col.Set(adapter.ReadInt16()); break;
                                case RecordDataCode.Int32: col.Set(adapter.ReadInt32()); break;
                                case RecordDataCode.Int64: col.Set(adapter.ReadInt64()); break;
                                case RecordDataCode.SByte: col.Set(adapter.ReadSByte()); break;
                                case RecordDataCode.Single: col.Set(adapter.ReadSingle()); break;
                                case RecordDataCode.String: col.Set(adapter.ReadString()); break;
                                case RecordDataCode.UInt16: col.Set(adapter.ReadUInt16()); break;
                                case RecordDataCode.UInt32: col.Set(adapter.ReadUInt32()); break;
                                case RecordDataCode.UInt64: col.Set(adapter.ReadUInt64()); break;
                                default: col.SetValue(adapter.ReadObject(col.Type), record.Cursor); break;
                            }
                        }
                    }
                    break;
                case RecordSection.Columns:
                    for (int i = 0; i < cols; i++)
                    {
                        var col = adapter.ReadColumn();
                        record.Columns.Add(col.Name, col.Type);
                    }
                    break;
                default: throw new NotSupportedException("未知的记录适配器布局类型：" + adapter.Section);
            }
        }
        return record;
    }
    #endregion

    #region Cursor
    /// <summary>
    /// 获取或设置当前游标位置，用于指示当前操作的行索引。
    /// </summary>
    /// <value>
    /// 游标位置的整数值，范围从 0 到 <see cref="Count"/> - 1。
    /// 当设置超出有效范围的值时，可能会导致数据访问异常。
    /// </value>
    /// <remarks>
    /// 游标用于跟踪当前正在操作的数据行，许多数据读取和写入操作都基于当前游标位置执行。
    /// </remarks>
    public int Cursor { get; set; } = 0;

    /// <summary>
    /// 将游标移动到第一行（索引为 0）。
    /// </summary>
    /// <remarks>
    /// 此方法将游标重置到数据集的开始位置。如果数据集为空，游标仍会被设置为 0。
    /// </remarks>
    public void MoveFirst() { this.Cursor = 0; }

    /// <summary>
    /// 将游标移动到最后一行。
    /// </summary>
    /// <remarks>
    /// 此方法将游标设置为 <see cref="Count"/> - 1。如果数据集为空（<see cref="Count"/> 为 0），
    /// 游标将被设置为 -1，这可能会在后续操作中导致异常。
    /// </remarks>
    public void MoveLast() { this.Cursor = this.Count - 1; }

    /// <summary>
    /// 获取一个值，该值指示数据集是否为空（不包含任何行）。
    /// </summary>
    /// <value>
    /// 如果 <see cref="Count"/> 为 0，则为 <see langword="true"/>；否则为 <see langword="false"/>。
    /// </value>
    /// <remarks>
    /// 此属性提供了一种简便的方法来检查数据集是否包含数据行。
    /// </remarks>
    public bool IsEmpty { get { return Count > 0 ? false : true; } }

    /// <summary>
    /// 获取一个值，该值指示当前游标是否位于第一行。
    /// </summary>
    /// <value>
    /// 如果 <see cref="Cursor"/> 为 0，则为 <see langword="true"/>；否则为 <see langword="false"/>。
    /// </value>
    /// <remarks>
    /// 此属性用于确定游标是否处于数据集的开始位置。
    /// </remarks>
    public bool IsFirst => this.Cursor == 0;

    /// <summary>
    /// 获取一个值，该值指示当前游标是否位于最后一行。
    /// </summary>
    /// <value>
    /// 如果 <see cref="Cursor"/> 等于 <see cref="Count"/> - 1，则为 <see langword="true"/>；否则为 <see langword="false"/>。
    /// </value>
    /// <remarks>
    /// 此属性用于确定游标是否处于数据集的末尾位置。当数据集为空时，此属性返回 <see langword="false"/>。
    /// </remarks>
    public bool IsLast => this.Cursor == this.Count - 1;

    /// <summary>
    /// 获取一个值，该值指示当前游标是否已超出记录范围或记录为空。
    /// </summary>
    /// <value>
    /// 如果 <see cref="Cursor"/> 大于或等于 <see cref="Count"/>，或者 <see cref="Count"/> 为 0，
    /// 则为 <see langword="true"/>；否则为 <see langword="false"/>。
    /// </value>
    /// <remarks>
    /// 此属性用于检查游标是否处于无效位置，通常在遍历数据或执行读取操作前进行检查。
    /// 当此属性返回 <see langword="true"/> 时，基于游标的数据操作可能会失败。
    /// </remarks>
    public bool IsEndOfRecord => this.Cursor >= this.Count || this.Count == 0;
    #endregion
}