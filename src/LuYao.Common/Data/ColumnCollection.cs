using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuYao.Data;

/// <summary>
/// 列集合
/// </summary>
public class ColumnCollection : IReadOnlyList<Column>
{
    private readonly List<Column> _list = new List<Column>();

    #region IReadOnlyList

    /// <inheritdoc/>
    public int Count => _list.Count;

    /// <inheritdoc/>
    public Column this[int index] => _list[index];

    /// <inheritdoc/>
    public IEnumerator<Column> GetEnumerator() => _list.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    #endregion
    private ColumnTable _table;
    private int _capacity;
    private int _count;
    /// <summary>
    /// 容量
    /// </summary>
    public int Capacity => _capacity;
    /// <summary>
    /// 数据行数
    /// </summary>
    public int Rows => _count;


    /// <summary>
    /// 
    /// </summary>
    /// <param name="table"></param>
    /// <param name="capacity"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public ColumnCollection(ColumnTable table, int capacity)
    {
        this._table = table ?? throw new ArgumentNullException(nameof(table), "表不能为空");
        if (capacity < 1) throw new ArgumentOutOfRangeException(nameof(capacity), "容量不能小于1");
        this._capacity = capacity;
    }

    /// <summary>
    /// 根据列名查找列
    /// </summary>
    public Column? Find(string name)
    {
        var idx = this.IndexOf(name);
        if (idx >= 0) return this[idx];
        return null;
    }
    /// <summary>
    /// 添加数据列
    /// </summary>
    /// <param name="name"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public Column Add(string name, TypeCode type) => Add(name, type, false);
    /// <summary>
    /// 添加数据列
    /// </summary>
    /// <param name="name"></param>
    /// <param name="type"></param>
    /// <param name="isArray"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public Column Add(string name, TypeCode type, bool isArray)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name), "列名不能为空");
        Column? col = this.Find(name);
        if (col != null) return col;
        col = new Column(name, type, isArray, this._capacity);
        this._list.Add(col);
        return col;
    }

    /// <summary>
    /// 添加一行
    /// </summary>
    /// <returns>行号</returns>
    public int AddRow()
    {
        this._count++;
        if (this._capacity < this._count)
        {
            this._capacity *= 2;
            foreach (Column col in this)
            {
                col.Extend(this._capacity);
            }
        }
        var idx = this._count - 1;
        return idx;
    }

    /// <summary>
    /// 查找指定列名的索引
    /// </summary>
    public int IndexOf(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name), "列名不能为空");
        for (int i = 0; i < this.Count; i++)
        {
            Column col = this[i];
            if (col.Name == name) return i;
        }
        return -1;
    }

    /// <summary>
    /// 判断指定的列名是否存在
    /// </summary>
    public bool Contains(string name) => this.IndexOf(name) >= 0;
}
