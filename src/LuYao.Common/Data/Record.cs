using LuYao.Text.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

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
        this.MoveFirst();
        while (this.Read())
        {
            DataRow row = dt.Rows.Add();
            for (int i = 0; i < this.Columns.Count; i++)
            {
                row[i] = this.Columns[i].GetValue(this.Cursor);
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

}