using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace LuYao.Data;

/// <summary>
/// 代表一行数据，提供对列存储数据集合中特定行数据的访问。
/// </summary>
public partial struct RecordRow : IDynamicMetaObjectProvider
{
    /// <summary>
    /// 初始化 <see cref="RecordRow"/> 结构体的新实例。
    /// </summary>
    /// <param name="record">包含此行的数据集合。</param>
    /// <param name="row">行索引，必须在有效范围内。</param>
    /// <exception cref="ArgumentOutOfRangeException">当行索引超出记录范围时抛出。</exception>
    /// <exception cref="ArgumentNullException">当记录参数为 null 时抛出。</exception>
    internal RecordRow(Record record, int row)
    {
        this.Record = record ?? throw new ArgumentNullException(nameof(record));
        if (row < 0 || row >= record.Count) throw new ArgumentOutOfRangeException(nameof(row));
        this.Row = row;
    }

    /// <summary>
    /// 获取包含此行的数据集合。
    /// </summary>
    /// <value>关联的 <see cref="Record"/> 实例。</value>
    public Record Record { get; }

    /// <summary>
    /// 获取当前行在数据集合中的索引位置。
    /// </summary>
    /// <value>从零开始的行索引。</value>
    public int Row { get; }

    /// <summary>
    /// 返回 <see cref="DynamicMetaObject"/>，支持动态成员访问。
    /// </summary>
    public DynamicMetaObject GetMetaObject(System.Linq.Expressions.Expression parameter)
        => new RecordRowMetaObject(parameter, this);

    /// <summary>
    /// 定义从 <see cref="RecordRow"/> 到 <see cref="int"/> 的隐式转换。
    /// 返回行的索引位置。
    /// </summary>
    /// <param name="rowRef">要转换的 <see cref="RecordRow"/> 实例。</param>
    /// <returns>该行在数据集合中的索引位置。</returns>
    public static implicit operator int(RecordRow rowRef) => rowRef.Row;

    /// <summary>
    /// 返回包含行号与当前行列值的字符串表示。
    /// </summary>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("{ Row = ").Append(this.Row).Append(", Data = { ");
        bool first = true;
        foreach (var col in this.Record.Columns)
        {
            if (!first) sb.Append(", ");
            var value = col.Get(this);
            sb.Append(col.Name).Append(" = ").Append(value?.ToString() ?? string.Empty);
            first = false;
        }
        sb.Append(" } }");
        return sb.ToString();
    }

    /// <summary>
    /// 将当前行的所有列值转换为字典。
    /// </summary>
    /// <returns>键为列名、值为当前行对应列值的字典。</returns>
    public Dictionary<string, object?> ToDictionary()
    {
        var ret = new Dictionary<string, object?>(this.Record.Columns.Count, StringComparer.Ordinal);
        foreach (var col in this.Record.Columns)
        {
            ret[col.Name] = col.Get(this);
        }
        return ret;
    }

    /// <summary>
    /// 使用列名访问或设置当前行的列值。
    /// </summary>
    /// <param name="name">要访问或设置的列名。若为 null、空或仅空白，将返回默认值；查找列时使用 Record.Columns.Find(name)。</param>
    /// <returns>若指定列存在，返回该列在当前行的值；否则返回 null（或默认）。在设置器中：若列不存在且 value 非 null，将根据 value 的运行时类型创建新列并赋值。</returns>
    public object? this[String name]
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                var col = this.Record.Columns.Find(name);
                if (col != null) return col.Get(this);
            }
            return default;
        }
        set
        {
            var col = this.Record.Columns.Find(name);
            if (col == null)
            {
                if (value == null) return;
                col = this.Record.Columns.Add(name, value.GetType());
            }
            col.Set(this, value);
        }
    }
}
