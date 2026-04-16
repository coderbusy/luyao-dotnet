using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace LuYao.Data;

/// <summary>
/// 列存储数据集合
/// </summary>
[DebuggerTypeProxy(typeof(RecordDebuggerTypeProxy))]
[Serializable]
public partial class Record : IEnumerable<RecordRow>, ISerializable, IXmlSerializable
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
        clone.Page = this.Page;
        clone._maxCount = this._maxCount;
        clone._pageSize = this._pageSize;
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

    #region Query

    /// <summary>
    /// 创建延迟执行的查询对象，支持链式调用。
    /// </summary>
    /// <param name="options">查询选项，可声明索引列等优化参数。</param>
    /// <returns>可链式组合的 <see cref="RecordQuery"/> 实例。</returns>
    public RecordQuery AsQuery(QueryOptions? options = null)
    {
        return new RecordQuery(this, options ?? new QueryOptions());
    }

    #endregion

    #region ISerializable

    /// <summary>
    /// 从序列化数据重建 <see cref="Record"/> 实例。
    /// </summary>
    protected Record(SerializationInfo info, StreamingContext context)
        : this(info.GetString("Name"), info.GetInt32("Count"))
    {
        Page = info.GetInt32("Page");
        _maxCount = info.GetInt32("MaxCount");
        _pageSize = info.GetInt32("PageSize");

        int colCount = info.GetInt32("ColCount");
        int rowCount = info.GetInt32("Count");

        for (int c = 0; c < colCount; c++)
        {
            string colName = info.GetString($"Col_{c}_Name")!;
            string typeName = info.GetString($"Col_{c}_Type")!;
            Type type = Type.GetType(typeName, throwOnError: true)!;
            this.Columns.Add(colName, type);
        }

        for (int r = 0; r < rowCount; r++)
        {
            this.AddRow();
            for (int c = 0; c < colCount; c++)
            {
                var val = info.GetValue($"V_{r}_{c}", typeof(object));
                if (val is not null)
                {
                    this.Columns[c].SetValue(val, r);
                }
            }
        }
    }

    /// <inheritdoc/>
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("Name", this.Name);
        info.AddValue("Page", this.Page);
        info.AddValue("MaxCount", _maxCount);
        info.AddValue("PageSize", _pageSize);
        info.AddValue("ColCount", this.Columns.Count);
        info.AddValue("Count", this.Count);

        for (int c = 0; c < this.Columns.Count; c++)
        {
            var col = this.Columns[c];
            info.AddValue($"Col_{c}_Name", col.Name);
            info.AddValue($"Col_{c}_Type", col.Type.AssemblyQualifiedName);
        }

        for (int r = 0; r < this.Count; r++)
        {
            for (int c = 0; c < this.Columns.Count; c++)
            {
                info.AddValue($"V_{r}_{c}", this.Columns[c].GetValue(r));
            }
        }
    }

    #endregion

    #region IXmlSerializable

    XmlSchema? IXmlSerializable.GetSchema() => null;

    void IXmlSerializable.ReadXml(XmlReader reader)
    {
        this.Name = reader.GetAttribute("Name") ?? string.Empty;
        this.Page = int.TryParse(reader.GetAttribute("Page"), out var p) ? p : 1;
        this._pageSize = int.TryParse(reader.GetAttribute("PageSize"), out var ps) ? ps : 0;
        this._maxCount = int.TryParse(reader.GetAttribute("MaxCount"), out var mc) ? mc : 0;

        this.Columns.Clear();

        if (reader.IsEmptyElement) { reader.Read(); return; }
        reader.ReadStartElement(); // <Record>

        // <Columns>
        if (reader.IsStartElement("Columns"))
        {
            if (reader.IsEmptyElement) { reader.Read(); }
            else
            {
                reader.ReadStartElement("Columns");
                while (reader.IsStartElement("Column"))
                {
                    string colName = reader.GetAttribute("Name")!;
                    string typeName = reader.GetAttribute("Type")!;
                    Type type = Type.GetType(typeName, throwOnError: true)!;
                    this.Columns.Add(colName, type);
                    reader.Read(); // self-closing <Column />
                }
                reader.ReadEndElement(); // </Columns>
            }
        }

        // <Rows>
        if (reader.IsStartElement("Rows"))
        {
            if (reader.IsEmptyElement) { reader.Read(); }
            else
            {
                reader.ReadStartElement("Rows");
                while (reader.IsStartElement("Row"))
                {
                    var row = this.AddRow();
                    if (reader.IsEmptyElement) { reader.Read(); continue; }
                    reader.ReadStartElement("Row");
                    for (int c = 0; c < this.Columns.Count; c++)
                    {
                        if (!reader.IsStartElement("V")) break;
                        bool isNil = reader.GetAttribute("nil", "http://www.w3.org/2001/XMLSchema-instance") == "true";
                        if (isNil || reader.IsEmptyElement)
                        {
                            reader.Read();
                            continue;
                        }
                        reader.ReadStartElement("V");
                        string text = reader.ReadContentAsString();
                        reader.ReadEndElement(); // </V>

                        var col = this.Columns[c];
                        object? val = DeserializeColumnValue(text, col.Type);
                        if (val is not null) col.SetValue(val, row.Row);
                    }
                    reader.ReadEndElement(); // </Row>
                }
                reader.ReadEndElement(); // </Rows>
            }
        }

        reader.ReadEndElement(); // </Record>
    }

    void IXmlSerializable.WriteXml(XmlWriter writer)
    {
        writer.WriteAttributeString("Name", this.Name);
        writer.WriteAttributeString("Page", this.Page.ToString());
        writer.WriteAttributeString("PageSize", _pageSize.ToString());
        writer.WriteAttributeString("MaxCount", _maxCount.ToString());

        writer.WriteStartElement("Columns");
        foreach (RecordColumn col in this.Columns)
        {
            writer.WriteStartElement("Column");
            writer.WriteAttributeString("Name", col.Name);
            writer.WriteAttributeString("Type", col.Type.FullName);
            writer.WriteEndElement();
        }
        writer.WriteEndElement();

        writer.WriteStartElement("Rows");
        for (int r = 0; r < this.Count; r++)
        {
            writer.WriteStartElement("Row");
            for (int c = 0; c < this.Columns.Count; c++)
            {
                var val = this.Columns[c].GetValue(r);
                writer.WriteStartElement("V");
                if (val is null)
                {
                    writer.WriteAttributeString("xsi", "nil", "http://www.w3.org/2001/XMLSchema-instance", "true");
                }
                else
                {
                    writer.WriteString(SerializeColumnValue(val, this.Columns[c].Type));
                }
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
        writer.WriteEndElement();
    }

    private static string SerializeColumnValue(object value, Type type)
    {
        if (type == typeof(byte[]))
            return Convert.ToBase64String((byte[])value);
        return Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty;
    }

    private static object? DeserializeColumnValue(string text, Type type)
    {
        if (string.IsNullOrEmpty(text)) return null;
        if (type == typeof(byte[]))
            return Convert.FromBase64String(text);

        var underlying = Nullable.GetUnderlyingType(type);
        var targetType = underlying ?? type;
        return Convert.ChangeType(text, targetType, System.Globalization.CultureInfo.InvariantCulture);
    }

    #endregion

}